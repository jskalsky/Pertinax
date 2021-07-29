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

int Client::Browse(unsigned short namespaceIndex, unsigned long id, int* nr, BrowseResponse uk[])
{
#ifdef NO_SIMULATION
    if (UaClient == NULL)
    {
        return 1;
    }
    UA_BrowseRequest bReq;
    UA_BrowseRequest_init(&bReq);
    bReq.requestedMaxReferencesPerNode = 0;
    bReq.nodesToBrowse = UA_BrowseDescription_new();
    bReq.nodesToBrowseSize = 1;
    bReq.nodesToBrowse[0].nodeId = UA_NODEID_NUMERIC(namespaceIndex, id); /* browse objects folder */
    bReq.nodesToBrowse[0].resultMask = UA_BROWSERESULTMASK_ALL; /* return everything */
    UA_BrowseResponse bResp = UA_Client_Service_browse(UaClient, bReq);
    int resultIndex = 0;
    for (size_t i = 0; i < bResp.resultsSize; ++i)
    {
        for (size_t j = 0; j < bResp.results[i].referencesSize; ++j)
        {
            if (bResp.results[i].statusCode == UA_STATUSCODE_GOOD)
            {
                UA_ReferenceDescription* ref = &(bResp.results[i].references[j]);
                if (resultIndex < *nr)
                {
                    uk[resultIndex].namespaceIndex = ref->nodeId.nodeId.namespaceIndex;
                    uk[resultIndex].identifierType = ref->nodeId.nodeId.identifierType;
                    if (ref->nodeId.nodeId.identifierType == UA_NODEIDTYPE_NUMERIC)
                    {
                        uk[resultIndex].numeric = ref->nodeId.nodeId.identifier.numeric;
                    }
                    else
                    {
                        if (ref->nodeId.nodeId.identifierType == UA_NODEIDTYPE_STRING)
                        {
                            uk[resultIndex].strLength = (int)ref->nodeId.nodeId.identifier.string.length;
                            if (ref->nodeId.nodeId.identifier.string.length <= MAX_STRING_LENGTH)
                            {
                                memcpy(uk[resultIndex].str, ref->nodeId.nodeId.identifier.string.data, ref->nodeId.nodeId.identifier.string.length);
                            }
                        }
                    }
                    uk[resultIndex].browseNameLength = (int)ref->browseName.name.length;
                    if (ref->browseName.name.length <= MAX_STRING_LENGTH)
                    {
                        memcpy(uk[resultIndex].browseName, ref->browseName.name.data, ref->browseName.name.length);
                    }
                    uk[resultIndex].displayNameLength = (int)ref->displayName.text.length;
                    if (ref->displayName.text.length <= MAX_STRING_LENGTH)
                    {
                        memcpy(uk[resultIndex].displayName, ref->displayName.text.data, ref->displayName.text.length);
                    }
                    uk[resultIndex].nodeClass = ref->nodeClass;
                    ++resultIndex;
                }
            }
        }
    }
    UA_BrowseRequest_clear(&bReq);
    UA_BrowseResponse_clear(&bResp);
    *nr = resultIndex;
#else
    for (int i = 0; i < *nr; ++i)
    {
        uk[i].numeric = i + 100;
    }
#endif
    return 0;
}
