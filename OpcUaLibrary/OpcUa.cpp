#include "OpcUa.h"
#include "Client.h"
#include <exception>

Client client;

extern "C" OPCUA_API int __stdcall OpenClient(int security)
{
    try
    {
        return client.Open(security);
    }
    catch (...)
    {
        return 1;
    }
}

extern "C" OPCUA_API int __stdcall Connect(const char *address)
{
    try
    {
        return client.Connect(address);
    }
    catch (...)
    {
        return 1;
    }
}

extern "C" OPCUA_API int __stdcall Browse(unsigned short int namespaceIndex, unsigned long id, int* nr, BrowseResponse uk[])
{
    try
    {
        return client.Browse(namespaceIndex, id, nr, uk);
    }
    catch (...)
    {
        return 1;
    }
}

