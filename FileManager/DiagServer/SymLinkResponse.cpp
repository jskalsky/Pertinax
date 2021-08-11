#include "SymLinkResponse.h"
#include <string>
#include <unistd.h>

namespace diag
{
    SymLinkResponse::SymLinkResponse()
    {
    }

    SymLinkResponse::~SymLinkResponse()
    {
    }

    void SymLinkResponse::MakeResponse(int client, int length, uint8* buffer)
    {
        std::string oldpath = GetString(2, buffer);
        std::string newpath = GetString(2 + oldpath.size() + 2, buffer);
        printf("old= %s, link= %s\n", oldpath.c_str(), newpath.c_str());
        int res = symlink(oldpath.c_str(), newpath.c_str());
        uint16 result = 0;
        if (res == -1)
        {
            result = (uint16)errno;
        }
        Add(result);
        int txSize = Offset + 2;
        Write(client, &txSize, 4);
        uint16 id = (uint16)SymLink;
        Write(client, &id, 2);
        Write(client, pBuffer, Offset);
    }
}