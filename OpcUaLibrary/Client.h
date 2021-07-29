#pragma once
#include "open62541/client.h"
#include "open62541/client_config_default.h"
#include "open62541/client_highlevel.h"
#include <open62541/plugin/securitypolicy.h>

#define NO_SIMULATION
#define MAX_STRING_LENGTH   32

struct BrowseResponse
{
    unsigned short int namespaceIndex;
    int identifierType;
    unsigned long numeric;
    int strLength;
    unsigned char str[MAX_STRING_LENGTH];
    int nodeClass;
    int browseNameLength;
    unsigned char browseName[MAX_STRING_LENGTH];
    int displayNameLength;
    unsigned char displayName[MAX_STRING_LENGTH];
};

class Client
{
private:
    UA_Client* UaClient;
public:
    int Open(int security);
    int Connect(const char* address);
    int Browse(unsigned short namespaceIndex, unsigned long id, int* nr, BrowseResponse uk[]);
};