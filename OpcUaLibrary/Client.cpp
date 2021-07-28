#include "Client.h"

int Client::Open(int security)
{
#ifdef NO_SIMULATION
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
#endif
    return 0;
}

int Client::Connect(const char* address)
{
#ifdef NO_SIMULATION
    UA_StatusCode sc = UA_Client_connect(UaClient, address);
    return (int)sc;
#else
    return 0;
#endif
}

int Client::Browse(unsigned long id, int* nr, BrowseResponse uk[])
{
#ifdef NO_SIMULATION
#else
    FILE* fp = fopen("c:\\Work\\nic.txt", "w");
    if (fp != NULL)
    {
        fprintf(fp, "id= %lu, nr= %p, *nr= %d\n", id, nr, *nr);
        fclose(fp);
    }
    if (*nr == 0)
    {
        *nr = 2;
    }
    else
    {
        for (int i = 0; i < *nr; ++i)
        {
            uk[i].numeric = i + 100;
        }
    }
#endif
    return 0;
}
