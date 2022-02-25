#include "OpcUa.h"
#include <exception>
#include "DrvOpcUa.h"
DrvOpcUa Opc;

extern "C" OPCUA_API int Open(const char* path)
{
    try
    {
        bool res = Opc.Open(path);
        printf("res= %d, %s\n", res, path);
        return (int)res;
    }
    catch (...)
    {
        return 0;
    }
}
/*
extern "C" OPCUA_API int __stdcall OpenClient(int security, unsigned long localMaxMessage, unsigned long remoteMaxMessage)
{
    try
    {
        return client.Open(security, localMaxMessage, remoteMaxMessage);
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

extern "C" OPCUA_API int __stdcall Read(unsigned short int namespaceIndex, unsigned long id, int* length, int* type, int* arrayLength, unsigned char buffer[])
{
    try
    {
        return client.Read(namespaceIndex, id, length, type, arrayLength, buffer);
    }
    catch (...)
    {
        return 1;
    }
}

extern "C" OPCUA_API int __stdcall ServiceRead(unsigned short int namespaceIndex, int length, unsigned long id[], OpcValue values[])
{
    try
    {
        return client.ServiceRead(namespaceIndex, length, id, values);
    }
    catch (...)
    {
        return 1;
    }
}
*/