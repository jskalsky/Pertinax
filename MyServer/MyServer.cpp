// MyServer.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <open62541/plugin/log_stdout.h>
#include <open62541/server.h>
#include <open62541/server_config_default.h>
#include <signal.h>
#include <stdlib.h>

static volatile UA_Boolean running = true;
static void stopHandler(int sig) {
    UA_LOG_INFO(UA_Log_Stdout, UA_LOGCATEGORY_USERLAND, "received ctrl-c");
    running = false;
}

UA_StatusCode InsertFolder(UA_Server* server, const char* name, UA_NodeId parent, UA_NodeId* result)
{
    UA_ObjectAttributes oAttr = UA_ObjectAttributes_default;
    oAttr.displayName = UA_LOCALIZEDTEXT((char*)"en-US", (char*)name);

    UA_StatusCode sc = UA_Server_addObjectNode(server, UA_NODEID_NULL,
        parent,
        UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES),
        UA_QUALIFIEDNAME(1, (char*)name), UA_NODEID_NUMERIC(0, UA_NS0ID_BASEOBJECTTYPE),
        oAttr, NULL, result);
    return sc;
}

int main()
{
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);
    UA_Server* server = UA_Server_new();
    UA_ServerConfig_setDefault(UA_Server_getConfig(server));

    UA_NodeId z1;
    UA_StatusCode sc = InsertFolder(server, "Z1xx", UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER), &z1);
    printf("sc= %x\n", sc);
    UA_NodeId z2;
    sc = InsertFolder(server, "Z200", z1, &z2);
    printf("sc= %x\n", sc);

    UA_StatusCode retval = UA_Server_run(server, &running);
    UA_Server_delete(server);
    return retval == UA_STATUSCODE_GOOD ? EXIT_SUCCESS : EXIT_FAILURE;
}
