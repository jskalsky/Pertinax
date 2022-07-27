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

std::string GetBasicType(UA_NodeId typeId)
{
	if (typeId.identifierType != UA_NODEIDTYPE_NUMERIC)
	{
		return "Unknown";
	}
	switch (typeId.identifier.numeric)
	{
	case UA_NS0ID_BOOLEAN:
		return "Boolean";
	case UA_NS0ID_SBYTE:
		return "Int8";
	case UA_NS0ID_BYTE:
		return "UInt8";
	case UA_NS0ID_INT16:
		return "Int16";
	case UA_NS0ID_UINT16:
		return "UInt16";
	case UA_NS0ID_INT32:
		return "Int32";
	case UA_NS0ID_UINT32:
		return "UInt32";
	case UA_NS0ID_FLOAT:
		return "Float";
	case UA_NS0ID_DOUBLE:
		return "Double";
	}
	return "Unknown";
}

std::string GetAccess(UA_Byte access)
{
	// v Namespace 0 je to obracene, Read promenna vyrobi vlajku I.OPCUA a Write promenna O.OPCUA
	switch (access)
	{
	case 1:
		return "Write";
	case 2:
		return "Read";
	case 3:
		return "ReadWrite";
	}
	return "Unknown";
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
					WriteLine(6, fpCs, "node = new ModNodeFolder(\"%s\", new ModNodeIdNumeric(0,%u));\n", GetQualifiedName(refD.browseName).c_str(),
						refD.nodeId.nodeId.identifier.numeric);
					WriteLine(6, fpCs, "parent.AddSubNode(node);\n");
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
						UA_Variant* val = UA_Variant_new();
						UA_StatusCode scAttr = UA_Client_readValueAttribute(client, refD.nodeId.nodeId, val);
						fprintf(fp, "UA_Client_readValueAttribute= %x\n", scAttr);
						std::string basicType;
						std::string access;
						size_t arrayLength = -1;
						UA_Byte accessLevel = 0;
						bool isScalar = false;
						bool isArray = false;
						if (scAttr == UA_STATUSCODE_GOOD)
						{
							scAttr = UA_Client_readUserAccessLevelAttribute(client, refD.nodeId.nodeId, &accessLevel);
							if (scAttr == UA_STATUSCODE_GOOD)
							{
								if (val->type != NULL)
								{
									basicType = GetBasicType(val->type->typeId);
									if (basicType != "Unknown")
									{
										access = GetAccess(accessLevel);
										if (access != "Unknown")
										{
											if (UA_Variant_isScalar(val))
											{
												isScalar = true;
											}
											else
											{
												if (val->arrayDimensionsSize == 1)
												{
													arrayLength = val->arrayLength;
													isArray = true;
												}
											}
										}
										if (isScalar || isArray)
										{
											if (isScalar)
											{
												WriteLine(6, fpCs, "node = new ModNodeVariable(\"%s\", new ModNodeIdNumeric(0, %u), basic_type.%s, access.%s);\n",
													GetQualifiedName(refD.browseName).c_str(), refD.nodeId.nodeId.identifier.numeric, basicType.c_str(), access.c_str());
												WriteLine(6, fpCs, "parent.AddSubNode(node);\n");
											}
											else
											{
												if (isArray)
												{
													WriteLine(6, fpCs, "node = new ModNodeArrayVariable(\"%s\", new ModNodeIdNumeric(0, %u), basic_type.%s, access.%s, %d);\n",
														GetQualifiedName(refD.browseName).c_str(), refD.nodeId.nodeId.identifier.numeric, basicType.c_str(), access.c_str(), arrayLength);
													WriteLine(6, fpCs, "parent.AddSubNode(node);\n");
												}
											}
											fprintf(fp, "Variable: %s, %s\n", GetQualifiedName(refD.browseName).c_str(), GetExpandedNodeId(refD.typeDefinition).c_str());
										}
									}
								}
							}
						}
						UA_Variant_delete(val);
						WriteLine(6, fpCs, "stack.Push(parent);\n");
						WriteLine(6, fpCs, "parent = node;\n");
						browseRequest.nodesToBrowse[0].nodeId = refD.nodeId.nodeId;
						Browse(client, browseRequest, spaces + 2, fp, fpCs);
						WriteLine(6, fpCs, "parent = stack.Pop();\n");
					}
					else
					{
						if (refD.nodeClass == UA_NODECLASS_OBJECT)
						{
							fprintf(fp, "Object: %s\n", GetQualifiedName(refD.browseName).c_str());
							WriteLine(6, fpCs, "node = new ModNodeObject(\"%s\", new ModNodeIdNumeric(0,%u), \"%s\");\n", GetQualifiedName(refD.browseName).c_str(),
								refD.nodeId.nodeId.identifier.numeric, "ObjectType");
							WriteLine(6, fpCs, "parent.AddSubNode(node);\n");
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
	fprintf(fpCs, "\n");
	fprintf(fpCs, "namespace WpfControlLibrary.Model\n");
	fprintf(fpCs, "{\n");
	WriteLine(2, "internal static class DefaultDataModel\n", fpCs);
	WriteLine(2, "{\n", fpCs);
	WriteLine(4, "internal static void Setup(ModNodeNs ns0)\n", fpCs);
	WriteLine(4, "{\n", fpCs);

	WriteLine(6, fpCs, "ModNode parent = ns0;\n");
	WriteLine(6, fpCs, "Stack<ModNode> stack = new Stack<ModNode>();\n");
	WriteLine(6, fpCs, "ModNode node = null;\n");
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
	errno_t err = fopen_s(&fp, "e:\\Work\\BrowseResult.txt", "wt");
	if (fp != NULL)
	{
		err = fopen_s(&fpCs, "e:\\Work\\DefaultDataModel.cs", "wt");
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

