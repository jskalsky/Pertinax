/*
 * GetDirectoriesResponse.h
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#ifndef GETDIRECTORIESRESPONSE_H_
#define GETDIRECTORIESRESPONSE_H_
#include "DirectoryResponse.h"
#include "DiagTypes.h"

namespace diag {

class GetDirectoriesResponse : public DirectoryResponse
{
public:
    GetDirectoriesResponse();
    virtual ~GetDirectoriesResponse();
    virtual void MakeResponse(int client, int length, uint8 *buffer);
};

} /* namespace diag */
#endif /* GETDIRECTORIESRESPONSE_H_ */
