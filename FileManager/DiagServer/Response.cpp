/*
 * Response.cpp
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#include "Response.h"
#include "ptxstring.h"
#include <cstdio>
#include <string.h>
#include <stdexcept>
#include <errno.h>
#include <unistd.h>

namespace diag {

uint8 *Response::pBuffer = new uint8[STARTING_BUF_SIZE];
int Response::BufferLength = STARTING_BUF_SIZE;
int Response::Offset = 0;

Response::Response()
{

}

Response::~Response()
{
    if(pBuffer != NULL)
    {
        delete[] pBuffer;
        pBuffer = 0;
    }
}

void Response::ReallocBuffer(int length) const
{
//    printf("ReallocBuffer Offset= %d, BufferLength= %d, length= %d\n",Offset, BufferLength, length);
    if((Offset + length) < BufferLength) return;
    BufferLength += ((length / ADD_BUF_SIZE) + 1) * ADD_BUF_SIZE;
    uint8 *tmp = new uint8[(uint32)BufferLength];
    memcpy(tmp, pBuffer, (uint32)Offset);
    delete[] pBuffer;
    pBuffer = tmp;
//    printf("New BufferLength= %d\n", BufferLength);
}

std::string Response::GetString(int idx, const uint8 *buffer) const
{
    uint16 length = *(uint16*)(void*)const_cast<uint8*>(&buffer[idx]);
    std::string result;
    for(uint16 i = 0; i < length; ++i)
    {
        result += (char)buffer[idx + i + 2];
    }
    return result;
}

void Response::Add(uint8 val) const
{
    ReallocBuffer(sizeof(uint8));
    pBuffer[Offset++] = val;
}

void Response::Add(uint16 val) const
{
    ReallocBuffer(sizeof(uint16));
    *(uint16*)(void*)(pBuffer + Offset) = val;
    Offset += (int)sizeof(uint16);
}

void Response::Add(uint32 val) const
{
    ReallocBuffer(sizeof(uint32));
    *(uint32*)(void*)(pBuffer + Offset) = val;
    Offset += (int)sizeof(uint32);
}

void Response::Add(const char *s) const
{
//    printf("Add %s\n",s);
    ReallocBuffer((int)(sizeof(uint16) + strlen(s) + 1));
    *(uint16*)(void*)(pBuffer + Offset) = (uint16)strlen(s);
    memcpy(pBuffer + Offset + sizeof(uint16), s, strlen(s) + 1);
    Offset += (int)(sizeof(uint16) + strlen(s) + 1);
}

void Response::Add(const uint8 *src, int length) const
{
    ReallocBuffer(length);
    memcpy(pBuffer + Offset, src, (uint32)length);
    Offset += length;
}

void Response::AddFile(const char *fileName) const
{
//    printf("AddFile %s\n",fileName);
    FILE *fp = fopen(fileName, "rb");
    if(fp == NULL)
    {
        throw (std::runtime_error (ptx_string::format("File %s open error %d : %s,%d)",fileName, errno, __FILE__,__LINE__)));
    }
    fseek(fp, 0, SEEK_END);
    uint32 size = (uint32)ftell(fp);
    fseek(fp, 0, SEEK_SET);
    ReallocBuffer((int)size);
    if(fread(pBuffer + Offset, 1, size, fp) != size)
    {
        fclose(fp);
        throw (std::runtime_error (ptx_string::format("File %s read error %d : %s,%d)",fileName, errno, __FILE__,__LINE__)));
    }
    Offset += (int)size;
    fclose(fp);
}

void Response::Write(int client_sock, const void *src, int length) const
{
    int wr = write(client_sock, src, (uint32)length);
    if(wr != length)
    {
        throw (std::runtime_error (ptx_string::format("Write error %d : %s,%d)", errno, __FILE__,__LINE__)));
    }
}

} /* namespace diag */
