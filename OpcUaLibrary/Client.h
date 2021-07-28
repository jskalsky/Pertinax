#pragma once
#include "open62541/client.h"
#include "open62541/client_config_default.h"
#include "open62541/client_highlevel.h"
#include <open62541/plugin/securitypolicy.h>

class Client
{
private:
    UA_Client* UaClient;
public:
    int Open(int security);
    int Connect(const char* address);
};