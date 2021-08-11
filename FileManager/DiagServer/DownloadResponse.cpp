/*
 * DownloadResponse.cpp
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#include "DownloadResponse.h"
#include "Directory.h"
#include "ptxstring.h"
#include <cstdio>
#include <string>
#include <stdexcept>
#include <errno.h>
#include <unistd.h>

namespace diag {

DownloadResponse::DownloadResponse()
{

}

DownloadResponse::~DownloadResponse()
{

}

void DownloadResponse::MakeResponse(int client, int length, uint8 *buffer)
{
    uint16 nameLength = *(uint16*)(void*)&buffer[2];
    char *name = (char*)&buffer[4];
//    printf("DownloadFile linux Name= %s\n", name);
    std::string fileName = name;
    int fileLength = *(int*)(void*)&buffer[4 + nameLength + 1];
//    printf("fileLength= %d\n", fileLength);
    Directory::MakeDirFromPath(fileName.c_str());
    size_t pos = fileName.find ("/lib/");
    bool renameOk = false;
    if (pos != std::string::npos)
    {
        fileName += ".new";
        renameOk = true;
    }
    else
    {
        pos = fileName.find ("/bin/");
        if (pos != std::string::npos)
        {
            fileName += ".new";
            renameOk = true;
        }
    }
    FILE *fp = fopen(fileName.c_str(),"wb");
    if(fp == NULL)
    {
        throw (std::runtime_error (ptx_string::format("File %s open error %d : %s,%d)",fileName.c_str(), errno, __FILE__,__LINE__)));
    }
    else
    {
        fwrite(&buffer[4 + nameLength + 1 + 4], 1, (uint32)fileLength, fp);
        fclose(fp);
        if(renameOk)
        {
            (void)rename(fileName.c_str(), name);
        }
        sync();
        uint16 result = 0;
        Add(result);
        int txSize = Offset + 2;
        Write(client, &txSize, 4);
        uint16 id = (uint16)Download;
        Write(client, &id, 2);
        Write(client, pBuffer, Offset);
    }
}

} /* namespace diag */
