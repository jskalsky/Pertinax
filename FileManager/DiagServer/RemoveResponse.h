#pragma once
#include "DiagTypes.h"
#include "Response.h"

namespace diag
{

    class RemoveResponse : public Response
    {
      public:
        RemoveResponse();
        virtual ~RemoveResponse();
        virtual void MakeResponse(int client, int length, uint8* buffer);
    };

} /* namespace diag */
