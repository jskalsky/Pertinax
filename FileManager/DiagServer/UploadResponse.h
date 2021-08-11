/*
 * Upload.h
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#ifndef UPLOAD_H_
#define UPLOAD_H_

#include "DiagTypes.h"
#include "Response.h"

namespace diag {

class UploadResponse : public Response
{
public:
    UploadResponse();
    virtual ~UploadResponse();
    virtual void MakeResponse(int client, int length, uint8 *buffer);
};

} /* namespace diag */
#endif /* UPLOAD_H_ */
