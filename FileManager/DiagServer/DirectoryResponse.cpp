/*
 * DirectoryResponse.cpp
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#include "DirectoryResponse.h"
#include <cstdio>

namespace diag {

DirectoryResponse::DirectoryResponse()
{

}

DirectoryResponse::~DirectoryResponse()
{

}

void DirectoryResponse::DiagGetDirItems(int client_sock, MessageType mt, std::list<std::string>& items)
{
    std::list<std::string>::iterator it;
    Offset = 0;
    for(it = items.begin(); it != items.end(); ++it)
    {
        Add((*it).c_str());
    }
    int length = Offset + 2;
    Write(client_sock, &length, 4);
    uint16 id = (uint16)mt;
    Write(client_sock, &id, 2);
    int nr = (int)items.size();
    Write(client_sock, &nr, 4);
    Write(client_sock, pBuffer, Offset);
}

} /* namespace diag */
