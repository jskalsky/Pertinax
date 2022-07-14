// MyClient.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <open62541/client_config_default.h>
#include <open62541/client_highlevel.h>
#include <open62541/plugin/log_stdout.h>
#include <stdlib.h>
#include <stdarg.h>

char Buffer[4096];

std::string GetNodeClass(UA_NodeClass nc)
{
	std::string s = "Unspecified";
	switch (nc)
	{
	case UA_NODECLASS_OBJECT:
		s = "Object";
		break;
	case UA_NODECLASS_VARIABLE:
		s = "Variable";
		break;
	case UA_NODECLASS_METHOD:
		s = "Method";
		break;
	case UA_NODECLASS_OBJECTTYPE:
		s = "ObjectType";
		break;
	case UA_NODECLASS_VARIABLETYPE:
		s = "VariableType";
		break;
	case UA_NODECLASS_REFERENCETYPE:
		s = "ReferenceType";
		break;
	case UA_NODECLASS_DATATYPE:
		s = "DataType";
		break;
	case UA_NODECLASS_VIEW:
		s = "View";
		break;
	default:
		break;
	}
	return s;
}

std::string GetString(UA_String s)
{
	std::string result;
	for (size_t i = 0; i < s.length; ++i)
	{
		result += (char)s.data[i];
	}
	return result;
}

std::string GetLocalizedText(UA_LocalizedText text)
{
	std::string s = GetString(text.locale);
	s += ':';
	s += GetString(text.text);
	return s;
}

std::string GetQualifiedName(UA_QualifiedName name)
{
	std::string s;
	char buf[16];
	snprintf(buf, 16, "%u", name.namespaceIndex);
	std::string uaString = GetString(name.name);
	s = buf;
	s += ':';
	s += uaString;
	return s;
}

std::string GetNodeId(UA_NodeId nodeId)
{
	std::string s;
	char buf[16];
	snprintf(buf, 16, "%u", nodeId.namespaceIndex);
	s = buf;
	s += ':';
	if (nodeId.identifierType == UA_NODEIDTYPE_NUMERIC)
	{
		snprintf(buf, 16, "%lu", nodeId.identifier.numeric);
		s += buf;
	}
	else
	{
		if (nodeId.identifierType == UA_NODEIDTYPE_STRING)
		{
			s += GetString(nodeId.identifier.string);
		}
		else
		{
			if (nodeId.identifierType == UA_NODEIDTYPE_GUID)
			{
				s += "GUID";
			}
			else
			{
				if (nodeId.identifierType == UA_NODEIDTYPE_BYTESTRING)
				{
					s += "BYTESTRING";
				}
			}
		}
	}
	return s;
}

std::string GetExpandedNodeId(UA_ExpandedNodeId nodeId)
{
	std::string s = GetNodeId(nodeId.nodeId);
	s += ':';
	s += GetString(nodeId.namespaceUri);
	s += ':';
	char buf[16];
	snprintf(buf, 16, "%lu", nodeId.serverIndex);
	s += buf;
	return s;
}

void WriteSpaces(int nr, FILE* fp)
{
	for (int i = 0; i < nr; ++i)
	{
		fprintf(fp, " ");
	}
}

void WriteLine(int nr, const char* line, FILE* fp)
{
	WriteSpaces(nr, fp);
	fprintf(fp, "%s", line);
}

void WriteLine(int nr, FILE* fp, const char* format, ...)
{
	WriteSpaces(nr, fp);
	va_list args;
	va_start(args, format);
	vsprintf_s(Buffer, format, args);
	fprintf(fp, Buffer);
	va_end(args);
}

void Browse(UA_Client* client, UA_BrowseRequest& browseRequest, int spaces, FILE* fp, FILE* fpCs)
{
	fprintf(fp, "Browse %u:%u\n", browseRequest.nodesToBrowse[0].nodeId.namespaceIndex, browseRequest.nodesToBrowse[0].nodeId.identifier.numeric);
	UA_BrowseResponse bResp = UA_Client_Service_browse(client, browseRequest);
	fprintf(fp, "spaces= %d,  response= %u\n", spaces, bResp.resultsSize);
	for (size_t i = 0; i < bResp.resultsSize; ++i)
	{
		if (bResp.results[i].statusCode == UA_STATUSCODE_GOOD)
		{
			fprintf(fp, "referencesSize= %u\n", bResp.results[i].referencesSize);
			for (size_t j = 0; j < bResp.results[i].referencesSize; ++j)
			{
				UA_ReferenceDescription refDescr = bResp.results[i].references[j];
				for (int nr = 0; nr < spaces; ++nr)
				{
					fprintf(fp, " ");
				}
				if (refDescr.referenceTypeId.identifierType == UA_NODEIDTYPE_NUMERIC)
				{
					if (refDescr.referenceTypeId.identifier.numeric == UA_NS0ID_ORGANIZES)
					{
						fprintf(fp, "refTypeId= %s, nodeId= %s, browseName= %s, nodeClass= %s, Type= %s\n", GetNodeId(refDescr.referenceTypeId).c_str(),
							GetExpandedNodeId(refDescr.nodeId).c_str(), GetQualifiedName(refDescr.browseName).c_str(),
							GetNodeClass(refDescr.nodeClass).c_str(), GetExpandedNodeId(refDescr.typeDefinition).c_str());
						if (refDescr.typeDefinition.nodeId.identifierType == UA_NODEIDTYPE_NUMERIC &&
							refDescr.typeDefinition.nodeId.identifier.numeric == UA_NS0ID_FOLDERTYPE)
						{
							WriteLine(6, fpCs, "parent = new DataModelFolder(%s, new NodeIdNumeric(0, %u), parent);\n",
								GetQualifiedName(refDescr.browseName).c_str(), refDescr.nodeId.nodeId.identifier.numeric);
							browseRequest.nodesToBrowse[0].nodeId = refDescr.nodeId.nodeId;
							Browse(client, browseRequest, spaces + 2, fp, fpCs);
						}
						else
						{
							if (refDescr.nodeClass == UA_NODECLASS_VARIABLE)
							{
								fprintf(fp, "Variable %s\n", GetQualifiedName(refDescr.browseName).c_str());
							}
							else
							{
								if (refDescr.nodeClass == UA_NODECLASS_OBJECT)
								{
									fprintf(fp, "Object %s\n", GetQualifiedName(refDescr.browseName).c_str());
									browseRequest.nodesToBrowse[0].nodeId = refDescr.nodeId.nodeId;
									Browse(client, browseRequest, spaces + 2, fp, fpCs);
								}
							}
						}
					}
				}
/*				fprintf(fp, "refTypeId= %s, isForward= %d, nodeId= %s, browseName= %s, displayName= %s, nodeClass= %s, typeDefinition= %s\n",
					GetNodeId(refDescr.referenceTypeId).c_str(), refDescr.isForward, GetExpandedNodeId(refDescr.nodeId).c_str(),
					GetQualifiedName(refDescr.browseName).c_str(), GetLocalizedText(refDescr.displayName).c_str(), GetNodeClass(refDescr.nodeClass).c_str(),
					GetExpandedNodeId(refDescr.typeDefinition).c_str());
				if (refDescr.nodeClass == UA_NODECLASS_OBJECT || refDescr.nodeClass == UA_NODECLASS_OBJECTTYPE)
				{
					browseRequest.nodesToBrowse[0].nodeId = refDescr.nodeId.nodeId;
					Browse(client, browseRequest, spaces + 2, fp);
				}*/
			}
		}
	}
	UA_BrowseResponse_clear(&bResp);
}

void StartCs(FILE* fpCs)
{
	fprintf(fpCs, "using System;\n");
	fprintf(fpCs, "System.Windows;\n");
	fprintf(fpCs, "using WpfControlLibrary.DataModel;\n");
	fprintf(fpCs, "\n");
	fprintf(fpCs, "namespace ConfigOpcUa\n");
	fprintf(fpCs, "{\n");
	WriteLine(2, "public class DefaultDataModel\n", fpCs);
	WriteLine(2, "{\n", fpCs);
	WriteLine(4, "public DataModelNamespace DataModelNamespace0 { get; set; }\n", fpCs);
	WriteLine(4, "public DataModelNamespace DataModelNamespace1 { get; set; }\n", fpCs);
	WriteLine(4, "public DataModelNamespace DataModelNamespace2 { get; set; }\n", fpCs);
	WriteLine(4, "\n", fpCs);
	WriteLine(4, "void Setup()\n", fpCs);
	WriteLine(4, "{\n", fpCs);
}

void EndCs(FILE* fpCs)
{
	WriteLine(4, "}\n", fpCs);
	WriteLine(2, "}\n", fpCs);
	WriteLine(0, "}\n", fpCs);
}

int main()
{
	UA_Client* client = UA_Client_new();
	UA_ClientConfig_setDefault(UA_Client_getConfig(client));
	UA_StatusCode retval = UA_Client_connect(client, "opc.tcp://localhost:4840");
//	UA_StatusCode retval = UA_Client_connect(client, "opc.tcp://10.10.13.252:4840");
	if (retval != UA_STATUSCODE_GOOD)
	{
		UA_Client_delete(client);
		return (int)retval;
	}
	printf("Connect Ok");

	UA_BrowseRequest bReq;
	UA_BrowseRequest_init(&bReq);
	bReq.requestedMaxReferencesPerNode = 0;
	bReq.nodesToBrowse = UA_BrowseDescription_new();
	bReq.nodesToBrowseSize = 1;
	bReq.nodesToBrowse[0].nodeId = UA_NODEID_NUMERIC(0, UA_NS0ID_ROOTFOLDER);
	bReq.nodesToBrowse[0].resultMask = UA_BROWSERESULTMASK_ALL; // return everything

	FILE* fp;
	FILE* fpCs;
	errno_t err = fopen_s(&fp, "c:\\Work\\BrowseResult.txt", "wt");
	if (fp != NULL)
	{
		err = fopen_s(&fpCs, "c:\\Work\\DefaultDataModel.cs", "wt");
		if (fpCs != NULL)
		{
			StartCs(fpCs);
			Browse(client, bReq, 0, fp, fpCs);
			EndCs(fpCs);
			fclose(fpCs);
		}
		fclose(fp);
	}
	printf("Ok\n");
	UA_Client_delete(client);
}

