// ClientOpcUa.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <open62541/client_config_default.h>
#include <open62541/client_highlevel.h>
#include <open62541/plugin/log_stdout.h>

UA_NodeId idFloats1, idFloats2, idFloats3, idFloats4;

void Browse(UA_Client* client, UA_BrowseRequest& browseRequest)
{
    UA_BrowseResponse bResp = UA_Client_Service_browse(client, browseRequest);
    for (size_t i = 0; i < bResp.resultsSize; ++i)
    {
        //        printf("referencesSize= %u\n", bResp.results[i].referencesSize);
        for (size_t j = 0; j < bResp.results[i].referencesSize; ++j)
        {
            UA_ReferenceDescription* ref = &(bResp.results[i].references[j]);
            std::string displayName;
            for (size_t k = 0; k < ref->displayName.text.length; ++k)
            {
                displayName += (char)ref->displayName.text.data[k];
            }
            printf("-> %s\n", displayName.c_str());
            if (displayName == "Floats1")
            {
                idFloats1 = ref->nodeId.nodeId;
                printf("1 Mam %lu", idFloats1.identifier.numeric);
            }
            if (displayName == "Floats2")
            {
                idFloats2 = ref->nodeId.nodeId;
                printf("2 Mam %lu", idFloats2.identifier.numeric);
            }
            if (displayName == "Floats3")
            {
                idFloats3 = ref->nodeId.nodeId;
                printf("3 Mam %lu", idFloats3.identifier.numeric);
            }
            if (displayName == "Floats4")
            {
                idFloats4 = ref->nodeId.nodeId;
                printf("4 Mam %lu", idFloats4.identifier.numeric);
            }
            browseRequest.nodesToBrowse[0].nodeId = ref->nodeId.nodeId;
            Browse(client, browseRequest);
        }
    }
    UA_BrowseResponse_clear(&bResp);
}

int main()
{
    UA_Client* client = UA_Client_new();
    UA_ClientConfig* pConfig = UA_Client_getConfig(client);
    UA_ClientConfig_setDefault(pConfig);
    pConfig->timeout = 10000;
    UA_Client_newWithConfig(pConfig);

    UA_ClientConfig_setDefault(UA_Client_getConfig(client));
    UA_StatusCode retval = UA_Client_connect(client, "opc.tcp://10.10.13.252:4840");
    if (retval != UA_STATUSCODE_GOOD)
    {
        printf("Connect %x\n", retval);
        UA_Client_delete(client);
        return (int)retval;
    }
    printf("Connect Ok\n");
    UA_BrowseRequest bReq;
    UA_BrowseRequest_init(&bReq);
    bReq.requestedMaxReferencesPerNode = 0;
    bReq.nodesToBrowse = UA_BrowseDescription_new();
    bReq.nodesToBrowseSize = 1;
    bReq.nodesToBrowse[0].nodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_OBJECTSFOLDER);
    bReq.nodesToBrowse[0].resultMask = UA_BROWSERESULTMASK_ALL; // return everything
    bool browse = true;

    Browse(client, bReq);

/*    UA_ReadRequest rr;
    UA_ReadRequest_init(&rr);
    rr.nodesToReadSize = 2;
    rr.nodesToRead = (UA_ReadValueId*)UA_Array_new(2, &UA_TYPES[UA_TYPES_READVALUEID]);
    UA_ReadValueId_init(&rr.nodesToRead[0]);
    rr.nodesToRead[0].attributeId = UA_ATTRIBUTEID_VALUE;
    rr.nodesToRead[0].nodeId = idFloats1;
    UA_ReadValueId_init(&rr.nodesToRead[1]);
    rr.nodesToRead[1].attributeId = UA_ATTRIBUTEID_VALUE;
    rr.nodesToRead[1].nodeId = idFloats2;

    UA_ReadResponse readResponse = UA_Client_Service_read(client, rr);
    printf("serviceResult= %x\n", readResponse.responseHeader.serviceResult);
    UA_ReadResponse_clear(&readResponse);*/

    UA_Variant* val = UA_Variant_new();
    UA_StatusCode sc = UA_Client_readValueAttribute(client, idFloats1, val);
    printf("1 Read= %x\n", sc);
    sc = UA_Client_readValueAttribute(client, idFloats2, val);
    printf("2 Read= %x\n", sc);
    sc = UA_Client_readValueAttribute(client, idFloats3, val);
    printf("3 Read= %x\n", sc);
    sc = UA_Client_readValueAttribute(client, idFloats4, val);
    printf("4 Read= %x\n", sc);
    UA_Variant_delete(val);

    UA_Client_delete(client);
    return 0;
}
