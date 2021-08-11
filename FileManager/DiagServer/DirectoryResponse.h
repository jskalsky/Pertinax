/*
 * DirectoryResponse.h
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#ifndef DIRECTORYRESPONSE_H_
#define DIRECTORYRESPONSE_H_
#include <string>
#include <list>
#include "Response.h"

namespace diag {

class DirectoryResponse : public Response
{
public:
    DirectoryResponse();
    virtual ~DirectoryResponse();
protected:
    void DiagGetDirItems(int client_sock, MessageType mt, std::list<std::string>& items);
};

} /* namespace diag */
#endif /* DIRECTORYRESPONSE_H_ */
