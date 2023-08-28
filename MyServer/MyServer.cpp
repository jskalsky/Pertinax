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

    UA_ByteString certificate = UA_BYTESTRING_NULL;
    UA_ByteString privateKey = UA_BYTESTRING_NULL;

    FILE* fp = NULL;
    fopen_s(&fp, "e:\\open62541\\tools\\certs\\server_cert.der", "rb");
    if (fp != NULL)
    {
        fseek(fp, 0, SEEK_END);
        certificate.length = (size_t)(ftell(fp) + 1);
        certificate.data = new UA_Byte[certificate.length];
        fseek(fp, 0, SEEK_SET);
        fread(certificate.data, 1, certificate.length, fp);
        fclose(fp);
    }
    fopen_s(&fp, "e:\\open62541\\tools\\certs\\server_key.der", "rb");
    if (fp != NULL)
    {
        fseek(fp, 0, SEEK_END);
        privateKey.length = (size_t)(ftell(fp) + 1);
        privateKey.data = new UA_Byte[privateKey.length];
        fseek(fp, 0, SEEK_SET);
        fread(privateKey.data, 1, privateKey.length, fp);
        fclose(fp);
    }
    printf("cert= %u, key= %u\n", certificate.length, privateKey.length);

    size_t trustListSize = 0;
    UA_STACKARRAY(UA_ByteString, trustList, trustListSize);

    /* Loading of an issuer list, not used in this application */
    size_t issuerListSize = 0;
    UA_ByteString* issuerList = NULL;

    /* Loading of a revocation list currently unsupported */
    UA_ByteString* revocationList = NULL;
    size_t revocationListSize = 0;

    UA_Server* server = UA_Server_new();
    UA_ServerConfig* config = UA_Server_getConfig(server);

    UA_StatusCode retval =
        UA_ServerConfig_setDefaultWithSecurityPolicies(config, 4840,
            &certificate, &privateKey,
            trustList, trustListSize,
            issuerList, issuerListSize,
            revocationList, revocationListSize);

#ifdef UA_ENABLE_WEBSOCKET_SERVER
    UA_ServerConfig_addNetworkLayerWS(UA_Server_getConfig(server), 7681, 0, 0, &certificate, &privateKey);
#endif

    UA_ByteString_clear(&certificate);
    UA_ByteString_clear(&privateKey);
    for (size_t i = 0; i < trustListSize; i++)
        UA_ByteString_clear(&trustList[i]);


    printf("1\n");
//    UA_NodeId z1;
//    UA_StatusCode sc = InsertFolder(server, "Z1xx", UA_NODEID_NUMERIC(0, UA_NS0ID_ROOTFOLDER), &z1);
//    printf("sc= %x\n", sc);
//    UA_NodeId z2;
//    sc = InsertFolder(server, "Z200", z1, &z2);
//    printf("sc= %x\n", sc);

//    UA_StatusCode sc = SetFolders(server);
//    printf("sc= %x\n", sc);

    retval = UA_Server_run(server, &running);
    UA_Server_delete(server);
    return retval == UA_STATUSCODE_GOOD ? EXIT_SUCCESS : EXIT_FAILURE;
}
