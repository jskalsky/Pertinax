/*
 * GetFilesResponse.cpp
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#include "GetFilesResponse.h"
#include "Directory.h"
#include <cstdio>

namespace diag {

GetFilesResponse::GetFilesResponse() {
    // TODO Auto-generated constructor stub

}

GetFilesResponse::~GetFilesResponse() {
    // TODO Auto-generated destructor stub
}

void GetFilesResponse::MakeResponse(int client_sock, int length, uint8 *buffer)
{
    std::string dir = GetString(2, buffer);
    std::list<std::string> files = Directory::ReadDirectory(dir.c_str(), false);
//    printf("files= %u\n", files.size());
    DiagGetDirItems(client_sock, GetFiles, files);
}

} /* namespace diag */
