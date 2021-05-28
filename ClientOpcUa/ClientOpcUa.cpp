// ClientOpcUa.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <open62541/client_config_default.h>
#include <open62541/client_highlevel.h>
#include <open62541/plugin/log_stdout.h>

void Browse(UA_Client* client, UA_BrowseRequest& browseRequest)
{
    UA_BrowseResponse bResp = UA_Client_Service_browse(client, browseRequest);
    for (size_t i = 0; i < bResp.resultsSize; ++i)
    {
        printf("referencesSize= %u\n", bResp.results[i].referencesSize);
        for (size_t j = 0; j < bResp.results[i].referencesSize; ++j)
        {
            UA_ReferenceDescription* ref = &(bResp.results[i].references[j]);
            std::string displayName;
            for (size_t k = 0; k < ref->displayName.text.length; ++k)
            {
                displayName += (char)ref->displayName.text.data[k];
            }
            printf("-> %s\n", displayName.c_str());
            browseRequest.nodesToBrowse[0].nodeId = ref->nodeId.nodeId;
            Browse(client, browseRequest);
        }
    }
    UA_BrowseResponse_clear(&bResp);
}

int main()
{
    UA_Client* client = UA_Client_new();
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

    UA_Client_delete(client);
    return 0;
}


