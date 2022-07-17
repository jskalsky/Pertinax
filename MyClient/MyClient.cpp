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

std::string GetQualifiedName(UA_QualifiedName name, bool withNamespace  = false)
{
	std::string s;
	char buf[16];
	snprintf(buf, 16, "%u", name.namespaceIndex);
	std::string uaString = GetString(name.name);
	if (withNamespace)
	{
		s = buf;
		s += ':';
	}
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
//	fprintf(fp, "spaces= %d,  response= %u\n", spaces, bResp.resultsSize);
	for (size_t i = 0; i < bResp.resultsSize; ++i)
	{
		if (bResp.results[i].statusCode == UA_STATUSCODE_GOOD)
		{
			fprintf(fp, "referencesSize= %u\n", bResp.results[i].referencesSize);
			for (size_t j = 0; j < bResp.results[i].referencesSize; ++j)
			{
				UA_ReferenceDescription refD = bResp.results[i].references[j];
				if (refD.typeDefinition.nodeId.identifierType == UA_NODEIDTYPE_NUMERIC && refD.typeDefinition.nodeId.identifier.numeric == UA_NS0ID_FOLDERTYPE)
				{
					fprintf(fp, "Folder: %s\n", GetQualifiedName(refD.browseName).c_str());
					WriteLine(6, fpCs, "node = new DataModelFolder(\"%s\", new NodeIdNumeric(0,%u), parent);\n", GetQualifiedName(refD.browseName).c_str(),
						refD.nodeId.nodeId.identifier.numeric);
					WriteLine(6, fpCs, "parent.AddChildren(node);\n");
					WriteLine(6, fpCs, "stack.Push(parent);\n");
					WriteLine(6, fpCs, "parent = node;\n");

					browseRequest.nodesToBrowse[0].nodeId = refD.nodeId.nodeId;
					Browse(client, browseRequest, spaces + 2, fp, fpCs);

					WriteLine(6, fpCs, "parent = stack.Pop();\n");
				}
				else
				{
					if (refD.nodeClass == UA_NODECLASS_VARIABLE)
					{

						WriteLine(6, fpCs, "node = new DataModelSimpleVariable(\"%s\", new NodeIdNumeric(0, %u), \"%s\", \"%s\", parent);\n", 
							GetQualifiedName(refD.browseName).c_str(), refD.nodeId.nodeId.identifier.numeric, "Float", "Read");
						WriteLine(6, fpCs, "parent.AddChildren(node);\n");
						fprintf(fp, "Variable: %s\n", GetQualifiedName(refD.browseName).c_str());
					}
					else
					{
						if (refD.nodeClass == UA_NODECLASS_OBJECT)
						{
							fprintf(fp, "Object: %s\n", GetQualifiedName(refD.browseName).c_str());
							WriteLine(6, fpCs, "node = new DataModelObjectVariable(\"%s\", new NodeIdNumeric(0,%u), \"%s\", parent);\n", GetQualifiedName(refD.browseName).c_str(),
								refD.nodeId.nodeId.identifier.numeric, "ObjectType");
							WriteLine(6, fpCs, "parent.AddChildren(node);\n");
							WriteLine(6, fpCs, "stack.Push(parent);\n");
							WriteLine(6, fpCs, "parent = node;\n");
							browseRequest.nodesToBrowse[0].nodeId = refD.nodeId.nodeId;
							Browse(client, browseRequest, spaces + 2, fp, fpCs);
							WriteLine(6, fpCs, "parent = stack.Pop();\n");
						}
					}
				}
			}
/*			for (size_t j = 0; j < bResp.results[i].referencesSize; ++j)
			{
				UA_ReferenceDescription refD = bResp.results[i].references[j];
				fprintf(fp, "START %u, %s\n", j, GetQualifiedName(refD.browseName).c_str());
				fprintf(fp, "refTypeId= %s, isForward= %d, nodeId= %s, browseName= %s, displayName= %s, nodeClass= %s, typeDefinition= %s\n",
					GetNodeId(refD.referenceTypeId).c_str(), refD.isForward, GetExpandedNodeId(refD.nodeId).c_str(),
					GetQualifiedName(refD.browseName).c_str(), GetLocalizedText(refD.displayName).c_str(), GetNodeClass(refD.nodeClass).c_str(),
					GetExpandedNodeId(refD.typeDefinition).c_str());

//				browseRequest.nodesToBrowse[0].nodeId = refD.nodeId.nodeId;
//				Browse(client, browseRequest, spaces + 2, fp, fpCs);
				fprintf(fp, "END %u, %s\n", j, GetQualifiedName(refD.browseName).c_str());

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
				fprintf(fp, "refTypeId= %s, isForward= %d, nodeId= %s, browseName= %s, displayName= %s, nodeClass= %s, typeDefinition= %s\n",
					GetNodeId(refDescr.referenceTypeId).c_str(), refDescr.isForward, GetExpandedNodeId(refDescr.nodeId).c_str(),
					GetQualifiedName(refDescr.browseName).c_str(), GetLocalizedText(refDescr.displayName).c_str(), GetNodeClass(refDescr.nodeClass).c_str(),
					GetExpandedNodeId(refDescr.typeDefinition).c_str());
				if (refDescr.nodeClass == UA_NODECLASS_OBJECT || refDescr.nodeClass == UA_NODECLASS_OBJECTTYPE)
				{
					browseRequest.nodesToBrowse[0].nodeId = refDescr.nodeId.nodeId;
					Browse(client, browseRequest, spaces + 2, fp);
				}
			}*/
		}
	}
	UA_BrowseResponse_clear(&bResp);
}

void StartCs(FILE* fpCs)
{
	fprintf(fpCs, "using System;\n");
	fprintf(fpCs, "using System.Windows;\n");
	fprintf(fpCs, "using System.Collections.Generic;\n");
	fprintf(fpCs, "using System.Collections.ObjectModel;\n");
	fprintf(fpCs, "using WpfControlLibrary.DataModel;\n");
	fprintf(fpCs, "\n");
	fprintf(fpCs, "namespace WpfControlLibrary.DataModel\n");
	fprintf(fpCs, "{\n");
	WriteLine(2, "public static class DefaultDataModel\n", fpCs);
	WriteLine(2, "{\n", fpCs);
	WriteLine(4, "public static DataModelNamespace DataModelNamespace0 { get; set; }\n", fpCs);
	WriteLine(4, "public static DataModelNamespace DataModelNamespace1 { get; set; }\n", fpCs);
	WriteLine(4, "public static DataModelNamespace DataModelNamespace2 { get; set; }\n", fpCs);
	WriteLine(4, "\n", fpCs);
	WriteLine(4, "public static void Setup(ObservableCollection<DataModelNode> dataModel)\n", fpCs);
	WriteLine(4, "{\n", fpCs);
	WriteLine(6, "DataModelNamespace0 = new DataModelNamespace(0);\n", fpCs);
	WriteLine(6, "DataModelNamespace1 = new DataModelNamespace(1);\n", fpCs);
	WriteLine(6, "DataModelNamespace2 = new DataModelNamespace(2);\n", fpCs);
	WriteLine(6, "dataModel.Add(DataModelNamespace0);\n", fpCs);
	WriteLine(6, "dataModel.Add(DataModelNamespace1);\n", fpCs);
	WriteLine(6, "dataModel.Add(DataModelNamespace2);\n", fpCs);

	WriteLine(6, fpCs, "DataModelFolder FolderZ2Xx = new DataModelFolder(\"%s\", NodeIdBase.GetNextSystemNodeId(1), DataModelNamespace1);\n", "Z2xx");
	WriteLine(6, fpCs, "DataModelFolder FolderObjectTypes = new DataModelFolder(\"%s\", NodeIdBase.GetNextSystemNodeId(1), DataModelNamespace1);\n", "ObjectTypes");
	WriteLine(6, fpCs, "DataModelFolder FolderObjects = new DataModelFolder(\"%s\", NodeIdBase.GetNextSystemNodeId(1), DataModelNamespace1);\n", "Objects");
	WriteLine(6, fpCs, "DataModelFolder FolderVariables = new DataModelFolder(\"%s\", NodeIdBase.GetNextSystemNodeId(1), DataModelNamespace1);\n", "Variables");
	WriteLine(6, fpCs, "FolderZ2Xx.AddChildren(FolderObjects);\n");
	WriteLine(6, fpCs, "FolderZ2Xx.AddChildren(FolderObjectTypes);\n");
	WriteLine(6, fpCs, "FolderZ2Xx.AddChildren(FolderVariables);\n");
	WriteLine(6, fpCs, "DataModelNamespace1.AddChildren(FolderZ2Xx);\n");

	WriteLine(6, fpCs, "DataModelNode parent = DataModelNamespace0;\n");
	WriteLine(6, fpCs, "Stack<DataModelNode> stack = new Stack<DataModelNode>();\n");
	WriteLine(6, fpCs, "DataModelNode node = null;\n");
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
			printf("1\n");
			StartCs(fpCs);
			printf("10\n");
			Browse(client, bReq, 0, fp, fpCs);
			printf("100\n");
			EndCs(fpCs);
			printf("1000\n");
			fclose(fpCs);
			printf("10000\n");
		}
		fclose(fp);
	}
	printf("Ok\n");
	UA_Client_delete(client);
}

