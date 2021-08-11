#pragma once
#include "DiagTypes.h"
#include "Response.h"

namespace diag
{
    enum ItemType : uint16
    {
        Unknown, Directory, Link, File
    };
    class ReadDirResponse : public Response
    {
      public:
        ReadDirResponse();
        virtual ~ReadDirResponse();
        virtual void MakeResponse(int client, int length, uint8* buffer);
    };

} /* namespace diag */
