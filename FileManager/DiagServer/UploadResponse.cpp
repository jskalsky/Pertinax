/*
 * Upload.cpp
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#include <stdio.h>
#include <errno.h>
#include <stdexcept>
#include "UploadResponse.h"
#include "ptxstring.h"

namespace diag {

UploadResponse::UploadResponse()
{

}

UploadResponse::~UploadResponse()
{

}

void UploadResponse::MakeResponse(int client_sock, int length, uint8 *buffer)
{
    //    uint16 length = *(uint16*)(void*)&pBuffer[2];
    char *name = (char*)&buffer[4];
//    printf("name= %s\n", name);

    Offset = 0;
    AddFile(name);
    int txSize = Offset + 2;
    Write(client_sock, &txSize, 4);
    uint16 id = (uint16)Upload;
    Write(client_sock, &id, 2);
    Write(client_sock, pBuffer, Offset);
}

} /* namespace diag */
