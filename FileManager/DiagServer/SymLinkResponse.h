#pragma once
#include "DiagTypes.h"
#include "Response.h"

namespace diag
{

    class SymLinkResponse : public Response
    {
      public:
        SymLinkResponse();
        virtual ~SymLinkResponse();
        virtual void MakeResponse(int client, int length, uint8* buffer);
    };

} /* namespace diag */
