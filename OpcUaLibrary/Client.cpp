#include "Client.h"

int Client::Open(int security)
{
    if (security)
    {

    }
    else
    {
        UaClient = UA_Client_new();
        if (UaClient == NULL)
        {
            return 1;
        }
        UA_ClientConfig* pConfig = UA_Client_getConfig(UaClient);
        UA_ClientConfig_setDefault(pConfig);
        pConfig->timeout = 1000;
        UA_Client_newWithConfig(pConfig);
    }
    return 0;
}

int Client::Connect(const char* address)
{
    UA_StatusCode sc = UA_Client_connect(UaClient, address);
    return (int)sc;
}