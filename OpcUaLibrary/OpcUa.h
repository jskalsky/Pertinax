#pragma once
#define OPCUA_API __declspec(dllexport)

extern "C" OPCUA_API int __stdcall OpenClient(int security);

