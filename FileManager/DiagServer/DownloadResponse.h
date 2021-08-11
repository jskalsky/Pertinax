/*
 * DownloadResponse.h
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#ifndef DOWNLOADRESPONSE_H_
#define DOWNLOADRESPONSE_H_

#include "DiagTypes.h"
#include "Response.h"

namespace diag {

class DownloadResponse : public Response
{
public:
    DownloadResponse();
    virtual ~DownloadResponse();
    virtual void MakeResponse(int client, int length, uint8 *buffer);
};

} /* namespace diag */
#endif /* DOWNLOADRESPONSE_H_ */
