/*
 * GetErrorLog.h
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#ifndef GETERRORLOG_H_
#define GETERRORLOG_H_
#include "Response.h"
#include "DiagTypes.h"

namespace diag {

class GetErrorLogResponse : public Response
{
public:
    GetErrorLogResponse();
    virtual ~GetErrorLogResponse();
    virtual void MakeResponse(int client, int length, uint8 *buffer);
};

} /* namespace diag */
#endif /* GETERRORLOG_H_ */
