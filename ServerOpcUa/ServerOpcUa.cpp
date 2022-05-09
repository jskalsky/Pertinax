// ServerOpcUa.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <open62541/plugin/log_stdout.h>
#include <open62541/server.h>
#include <open62541/server_config_default.h>
#include <signal.h>
#include <stdlib.h>
#include <conio.h>

extern UA_Byte PrivateKey[];
extern int PrivateKeyLength;
extern UA_Byte Certificate[];
extern int CertificateLength;

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

    UA_ByteString privateKey;
    privateKey.length = PrivateKeyLength;
    privateKey.data = PrivateKey;
    UA_ByteString certificate;
    certificate.length = CertificateLength;
    certificate.data = Certificate;
    size_t trustListSize = 0;
    UA_STACKARRAY(UA_ByteString, trustList, trustListSize);

    UA_Server* server = UA_Server_new();
    UA_ServerConfig* pConfig = UA_Server_getConfig(server);

    UA_StatusCode retval =
        UA_ServerConfig_setDefaultWithSecurityPolicies(pConfig, 4840, &certificate, &privateKey, trustList, trustListSize, NULL, 0, NULL, 0);
    printf("setDefault= %x\n", retval);

//    UA_ServerConfig_setDefault(pConfig);

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
    pConfig->customHostname = UA_String_fromChars("localhost");
    retval = UA_Server_run_startup(server);
    if (retval != UA_STATUSCODE_GOOD)
    {
        printf("Run startup %x\n", retval);
    }
    else
    {
        while (running)
        {
            UA_Server_run_iterate(server, true);
            if (_kbhit())
            {
                break;
            }
        }
    }
    
    UA_Server_delete(server);
    return retval == UA_STATUSCODE_GOOD ? EXIT_SUCCESS : EXIT_FAILURE;
}

