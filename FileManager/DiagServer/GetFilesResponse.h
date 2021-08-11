/*
 * GetFilesResponse.h
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#ifndef GETFILESRESPONSE_H_
#define GETFILESRESPONSE_H_
#include "DirectoryResponse.h"
#include "DiagTypes.h"
#include <list>
#include <string>

namespace diag {

class GetFilesResponse : public DirectoryResponse
{
public:
    GetFilesResponse();
    virtual ~GetFilesResponse();
    virtual void MakeResponse(int client, int length, uint8 *buffer);
};

} /* namespace diag */
#endif /* GETFILESRESPONSE_H_ */
