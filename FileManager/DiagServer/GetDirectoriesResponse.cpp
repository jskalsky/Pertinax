/*
 * GetDirectoriesResponse.cpp
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */
#include <string>
#include "GetDirectoriesResponse.h"
#include "Directory.h"

namespace diag {

GetDirectoriesResponse::GetDirectoriesResponse() {
    // TODO Auto-generated constructor stub

}

GetDirectoriesResponse::~GetDirectoriesResponse() {
    // TODO Auto-generated destructor stub
}

void GetDirectoriesResponse::MakeResponse(int client_sock, int length, uint8 *buffer)
{
    std::string dir = GetString(2, buffer);
    std::list<std::string> directories = Directory::ReadDirectory(dir.c_str(), true);
    DiagGetDirItems(client_sock, GetDirectories, directories);
}

} /* namespace diag */
