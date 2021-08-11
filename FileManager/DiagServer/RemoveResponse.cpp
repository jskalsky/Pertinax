#include "RemoveResponse.h"
#include <string>
#include <unistd.h>

namespace diag
{
    RemoveResponse::RemoveResponse()
    {
    }

    RemoveResponse::~RemoveResponse()
    {
    }

    void RemoveResponse::MakeResponse(int client, int length, uint8* buffer)
    {
        std::string fileName = GetString(2, buffer);
//        printf("Remove %s\n", fileName.c_str());
        int res = unlink(fileName.c_str());
//        printf("res= %d\n", res);
        uint16 result = 0;
        if (res != 0)
        {
            result = (uint16) errno;
        }
        Add(result);
        int txSize = Offset + 2;
        Write(client, &txSize, 4);
        uint16 id = (uint16)Remove;
        Write(client, &id, 2);
        Write(client, pBuffer, Offset);
    }
}
