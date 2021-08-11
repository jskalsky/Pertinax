/*
 * GetErrorLog.cpp
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */
#include <stdio.h>
#include "GetErrorLogResponse.h"
#include "Directory.h"

namespace diag {

GetErrorLogResponse::GetErrorLogResponse()
{
}

GetErrorLogResponse::~GetErrorLogResponse()
{
}

void GetErrorLogResponse::MakeResponse(int client_sock, int length, uint8 *buffer)
{
    char *logType = (char*)&buffer[4]; //diag nebo error
//    printf("GetErrorLog type= %s\n", logType);
    std::list<std::string> logs = Directory::ReadDirectory("/usr/pertinax/log", false);
    std::list<std::string>::iterator it;

//    for(it = logs.begin(); it != logs.end(); ++it)
//    {
//        printf("logs= %s\n",(*it).c_str());
//    }

    logs.sort();
//    for(it = logs.begin(); it != logs.end(); ++it)
//    {
//        printf("sort= %s\n",(*it).c_str());
//    }
    Offset = 0;
    std::list<std::string> fileNames;
    for(it = logs.begin(); it != logs.end(); ++it)
    {
//        printf("%s\n",(*it).c_str());
        if((*it).find(logType) != std::string::npos)
        {
            std::string fileName = "/usr/pertinax/log";
            if(fileName[fileName.size() - 1] != '/')
            {
                fileName += '/';
            }
            fileName += (*it);
            fileNames.push_back(fileName);
        }
    }
    fileNames.reverse();
//    for(it = fileNames.begin(); it != fileNames.end(); ++it)
//    {
//        printf("reverse= %s\n",(*it).c_str());
//    }

//    if(fileNames.size() > 1)
//    {
//        std::string tmp = fileNames.front();
//        fileNames.pop_front();
//        fileNames.push_back(tmp);
//    }
    for(it = fileNames.begin(); it != fileNames.end(); ++it)
    {
        AddFile((*it).c_str());
    }
//    if(Offset == 0) return;
    int txSize = Offset + 2;
    Write(client_sock, &txSize, 4);
    uint16 id = (uint16)GetErrorLog;
    Write(client_sock, &id, 2);
//    printf("pred Write %d\n", Offset);
    Write(client_sock, pBuffer, Offset);
//    printf("Po Write Ok\n");
}

} /* namespace diag */
