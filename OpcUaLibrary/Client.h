#pragma once
#include "open62541/client.h"
#include "open62541/client_config_default.h"
#include "open62541/client_highlevel.h"
#include <open62541/plugin/securitypolicy.h>

//#define NO_SIMULATION

struct BrowseResponse
{
    int identifierType;
    unsigned long numeric;
    unsigned char str[32];
    int nodeClass;
    unsigned char browseName[32];
    unsigned char displayName[32];
};

class Client
{
private:
    UA_Client* UaClient;
public:
    int Open(int security);
    int Connect(const char* address);
    int Browse(unsigned long id, int* nr, BrowseResponse uk[]);
};