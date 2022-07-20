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
        UA_QUALIFIEDNAME(1, (char*)name), UA_NODEID_NUMERIC(0, UA_NS0ID_FOLDERTYPE),
        oAttr, NULL, result);
    return sc;
}

UA_StatusCode AddObjectNode(UA_Server* server, char* name, UA_NodeId* objectid, UA_NodeId parentid, UA_NodeId referenceid, UA_NodeId type_id)
{
    UA_ObjectAttributes object_attr = UA_ObjectAttributes_default;
    object_attr.displayName = UA_LOCALIZEDTEXT((char*)"en-US", name);
    return UA_Server_addObjectNode(server, UA_NODEID_NULL, parentid, referenceid, UA_QUALIFIEDNAME(1, name), type_id, object_attr, NULL, objectid);
}

UA_StatusCode SetFolders(UA_Server* server)
{
    UA_NodeId folderRoot;
    UA_StatusCode sc = AddObjectNode(server, (char*)"Z1xx", &folderRoot, UA_NODEID_NUMERIC(0, UA_NS0ID_ROOTFOLDER),
        UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES), UA_NODEID_NUMERIC(0, UA_NS0ID_FOLDERTYPE));
    if (sc != UA_STATUSCODE_GOOD)
    {
        return sc;
    }
    UA_NodeId folderVariables;
    sc = AddObjectNode(server, (char*)"Variables", &folderVariables, folderRoot,
        UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES), UA_NODEID_NUMERIC(0, UA_NS0ID_FOLDERTYPE));
    if (sc != UA_STATUSCODE_GOOD)
    {
        return sc;
    }
    UA_NodeId folderObjects;
    sc = AddObjectNode(server, (char*)"Objects", &folderObjects, folderRoot,
        UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES), UA_NODEID_NUMERIC(0, UA_NS0ID_FOLDERTYPE));
    if (sc != UA_STATUSCODE_GOOD)
    {
        return sc;
    }
    UA_NodeId folderObjectTypes;
    sc = AddObjectNode(server, (char*)"ObjectTypes", &folderObjectTypes, folderRoot,
        UA_NODEID_NUMERIC(0, UA_NS0ID_ORGANIZES), UA_NODEID_NUMERIC(0, UA_NS0ID_FOLDERTYPE));
    //  Terminal::Printf("4");
    return sc;
}
int main()
{
    signal(SIGINT, stopHandler);
    signal(SIGTERM, stopHandler);
    printf("Start\n");
    UA_Server* server = UA_Server_new();
    UA_ServerConfig_setDefault(UA_Server_getConfig(server));
    printf("1\n");
//    UA_NodeId z1;
//    UA_StatusCode sc = InsertFolder(server, "Z1xx", UA_NODEID_NUMERIC(0, UA_NS0ID_ROOTFOLDER), &z1);
//    printf("sc= %x\n", sc);
//    UA_NodeId z2;
//    sc = InsertFolder(server, "Z200", z1, &z2);
//    printf("sc= %x\n", sc);

//    UA_StatusCode sc = SetFolders(server);
//    printf("sc= %x\n", sc);

    UA_StatusCode retval = UA_Server_run(server, &running);
    UA_Server_delete(server);
    return retval == UA_STATUSCODE_GOOD ? EXIT_SUCCESS : EXIT_FAILURE;
}
