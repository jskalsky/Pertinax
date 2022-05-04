// ServerOpcUa.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <open62541/plugin/log_stdout.h>
#include <open62541/server.h>
#include <open62541/server_config_default.h>
#include <signal.h>
#include <stdlib.h>

static volatile UA_Boolean running = true;
static void stopHandler(int sig) 
{
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "received ctrl-c");
    running = false;
}

int main()
{
    printf("Server start\n");
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);

    UA_Server* server = UA_Server_new();
    UA_ServerConfig* pConfig = UA_Server_getConfig(server);
    UA_ServerConfig_setDefault(pConfig);

    if (pConfig->endpointsSize != 0)
    {
        printf("end= %u\n", pConfig->endpointsSize);
        std::string endPoint((char*)pConfig->endpoints[0].endpointUrl.data, pConfig->endpoints[0].endpointUrl.length);
        printf("endPoint= %s\n", endPoint.c_str());
    }
    std::string appUri((char*)pConfig->applicationDescription.applicationUri.data, pConfig->applicationDescription.applicationUri.length);
    std::string productUri((char*)pConfig->applicationDescription.productUri.data, pConfig->applicationDescription.applicationUri.length);
    std::string gat((char*)pConfig->applicationDescription.gatewayServerUri.data, pConfig->applicationDescription.gatewayServerUri.length);
    std::string prof((char*)pConfig->applicationDescription.discoveryProfileUri.data, pConfig->applicationDescription.discoveryProfileUri.length);
    printf("appUri= %s, prUri= %s, gat= %s, prof= %s\n", appUri.c_str(), productUri.c_str(), gat.c_str(), prof.c_str());
    if (pConfig->applicationDescription.discoveryUrlsSize != 0)
    {
        printf("dis= %u\n", pConfig->applicationDescription.discoveryUrlsSize);
        for (size_t i = 0; i < pConfig->applicationDescription.discoveryUrlsSize; ++i)
        {
            std::string disUrl((char*)pConfig->applicationDescription.discoveryUrls[i].data, pConfig->applicationDescription.discoveryUrls[i].length);
        }
    }
    if (pConfig->serverUrlsSize != 0)
    {
        printf("ser %u\n", pConfig->serverUrlsSize);
        for (size_t i = 0; i < pConfig->serverUrlsSize; ++i)
        {
            std::string url((char*)pConfig->serverUrls[i].data, pConfig->serverUrls[i].length);
            printf("url= %s\n", url.c_str());
        }
    }

    UA_StatusCode retval = UA_Server_run(server, &running);

    UA_Server_delete(server);
    return retval == UA_STATUSCODE_GOOD ? EXIT_SUCCESS : EXIT_FAILURE;
}

