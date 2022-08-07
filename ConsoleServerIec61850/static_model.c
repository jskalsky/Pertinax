/*
 * static_model.c
 *
 * automatically generated from Nachod.scd
 */
#include "static_model.h"

static void initializeValues();

extern DataSet iedModelds_LD0_LLN0_StatNrml;


extern DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda0;
extern DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda1;
extern DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda2;
extern DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda3;

DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda0 = {
  "LD0",
  false,
  "DRRDRE1$ST$MemUsedAlm", 
  -1,
  NULL,
  NULL,
  &iedModelds_LD0_LLN0_StatNrml_fcda1
};

DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda1 = {
  "LD0",
  false,
  "DRRDRE1$ST$RcdClr", 
  -1,
  NULL,
  NULL,
  &iedModelds_LD0_LLN0_StatNrml_fcda2
};

DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda2 = {
  "LD0",
  false,
  "DRRDRE1$ST$RcdMade", 
  -1,
  NULL,
  NULL,
  &iedModelds_LD0_LLN0_StatNrml_fcda3
};

DataSetEntry iedModelds_LD0_LLN0_StatNrml_fcda3 = {
  "LD0",
  false,
  "DRRDRE1$ST$RcdStr", 
  -1,
  NULL,
  NULL,
  NULL
};

DataSet iedModelds_LD0_LLN0_StatNrml = {
  "LD0",
  "LLN0$StatNrml",
  4,
  &iedModelds_LD0_LLN0_StatNrml_fcda0,
  NULL
};

LogicalDevice iedModel_LD0 = {
    LogicalDeviceModelType,
    "LD0",
    (ModelNode*) &iedModel,
    NULL,
    (ModelNode*) &iedModel_LD0_LLN0
};

LogicalNode iedModel_LD0_LLN0 = {
    LogicalNodeModelType,
    "LLN0",
    (ModelNode*) &iedModel_LD0,
    (ModelNode*) &iedModel_LD0_LPHD1,
    (ModelNode*) &iedModel_LD0_LLN0_Mod,
};

DataObject iedModel_LD0_LLN0_Mod = {
    DataObjectModelType,
    "Mod",
    (ModelNode*) &iedModel_LD0_LLN0,
    (ModelNode*) &iedModel_LD0_LLN0_Beh,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper,
    0
};

DataAttribute iedModel_LD0_LLN0_Mod_Oper = {
    DataAttributeModelType,
    "Oper",
    (ModelNode*) &iedModel_LD0_LLN0_Mod,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_stVal,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_ctlVal,
    0,
    IEC61850_FC_CO,
    IEC61850_CONSTRUCTED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_ctlVal = {
    DataAttributeModelType,
    "ctlVal",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_origin,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_ENUMERATED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_origin = {
    DataAttributeModelType,
    "origin",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_ctlNum,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_origin_orCat,
    0,
    IEC61850_FC_CO,
    IEC61850_CONSTRUCTED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_origin_orCat = {
    DataAttributeModelType,
    "orCat",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_origin,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_origin_orIdent,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_ENUMERATED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_origin_orIdent = {
    DataAttributeModelType,
    "orIdent",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_origin,
    NULL,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_OCTET_STRING_64,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_ctlNum = {
    DataAttributeModelType,
    "ctlNum",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_T,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_INT8U,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_T = {
    DataAttributeModelType,
    "T",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_Test,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_Test = {
    DataAttributeModelType,
    "Test",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper_Check,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_BOOLEAN,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_Oper_Check = {
    DataAttributeModelType,
    "Check",
    (ModelNode*) &iedModel_LD0_LLN0_Mod_Oper,
    NULL,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_CHECK,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_LLN0_Mod,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_ENUMERATED,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_LLN0_Mod,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_LLN0_Mod,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_ctlModel,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_ctlModel = {
    DataAttributeModelType,
    "ctlModel",
    (ModelNode*) &iedModel_LD0_LLN0_Mod,
    (ModelNode*) &iedModel_LD0_LLN0_Mod_d,
    NULL,
    0,
    IEC61850_FC_CF,
    IEC61850_ENUMERATED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Mod_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_LLN0_Mod,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_LLN0_Beh = {
    DataObjectModelType,
    "Beh",
    (ModelNode*) &iedModel_LD0_LLN0,
    (ModelNode*) &iedModel_LD0_LLN0_Health,
    (ModelNode*) &iedModel_LD0_LLN0_Beh_stVal,
    0
};

DataAttribute iedModel_LD0_LLN0_Beh_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_LLN0_Beh,
    (ModelNode*) &iedModel_LD0_LLN0_Beh_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_ENUMERATED,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Beh_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_LLN0_Beh,
    (ModelNode*) &iedModel_LD0_LLN0_Beh_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Beh_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_LLN0_Beh,
    (ModelNode*) &iedModel_LD0_LLN0_Beh_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Beh_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_LLN0_Beh,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_LLN0_Health = {
    DataObjectModelType,
    "Health",
    (ModelNode*) &iedModel_LD0_LLN0,
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt,
    (ModelNode*) &iedModel_LD0_LLN0_Health_stVal,
    0
};

DataAttribute iedModel_LD0_LLN0_Health_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_LLN0_Health,
    (ModelNode*) &iedModel_LD0_LLN0_Health_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_ENUMERATED,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Health_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_LLN0_Health,
    (ModelNode*) &iedModel_LD0_LLN0_Health_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Health_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_LLN0_Health,
    (ModelNode*) &iedModel_LD0_LLN0_Health_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_Health_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_LLN0_Health,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_LLN0_NamPlt = {
    DataObjectModelType,
    "NamPlt",
    (ModelNode*) &iedModel_LD0_LLN0,
    NULL,
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt_vendor,
    0
};

DataAttribute iedModel_LD0_LLN0_NamPlt_vendor = {
    DataAttributeModelType,
    "vendor",
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt,
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt_swRev,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_NamPlt_swRev = {
    DataAttributeModelType,
    "swRev",
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt,
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt_d,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_NamPlt_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt,
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt_configRev,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_NamPlt_configRev = {
    DataAttributeModelType,
    "configRev",
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt,
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt_ldNs,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LLN0_NamPlt_ldNs = {
    DataAttributeModelType,
    "ldNs",
    (ModelNode*) &iedModel_LD0_LLN0_NamPlt,
    NULL,
    NULL,
    0,
    IEC61850_FC_EX,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

LogicalNode iedModel_LD0_LPHD1 = {
    LogicalNodeModelType,
    "LPHD1",
    (ModelNode*) &iedModel_LD0,
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam,
};

DataObject iedModel_LD0_LPHD1_PhyNam = {
    DataObjectModelType,
    "PhyNam",
    (ModelNode*) &iedModel_LD0_LPHD1,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam_vendor,
    0
};

DataAttribute iedModel_LD0_LPHD1_PhyNam_vendor = {
    DataAttributeModelType,
    "vendor",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam_swRev,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_PhyNam_swRev = {
    DataAttributeModelType,
    "swRev",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam_serNum,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_PhyNam_serNum = {
    DataAttributeModelType,
    "serNum",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam_model,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_PhyNam_model = {
    DataAttributeModelType,
    "model",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyNam,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_LPHD1_PhyHealth = {
    DataObjectModelType,
    "PhyHealth",
    (ModelNode*) &iedModel_LD0_LPHD1,
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth_stVal,
    0
};

DataAttribute iedModel_LD0_LPHD1_PhyHealth_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_ENUMERATED,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_PhyHealth_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_PhyHealth_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth,
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_PhyHealth_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_LPHD1_PhyHealth,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_LPHD1_Proxy = {
    DataObjectModelType,
    "Proxy",
    (ModelNode*) &iedModel_LD0_LPHD1,
    NULL,
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy_stVal,
    0
};

DataAttribute iedModel_LD0_LPHD1_Proxy_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy,
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_BOOLEAN,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_Proxy_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy,
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_Proxy_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy,
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_LPHD1_Proxy_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_LPHD1_Proxy,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

LogicalNode iedModel_LD0_DRRDRE1 = {
    LogicalNodeModelType,
    "DRRDRE1",
    (ModelNode*) &iedModel_LD0,
    NULL,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod,
};

DataObject iedModel_LD0_DRRDRE1_Mod = {
    DataObjectModelType,
    "Mod",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper = {
    DataAttributeModelType,
    "Oper",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_stVal,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_ctlVal,
    0,
    IEC61850_FC_CO,
    IEC61850_CONSTRUCTED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_ctlVal = {
    DataAttributeModelType,
    "ctlVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_origin,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_ENUMERATED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_origin = {
    DataAttributeModelType,
    "origin",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_ctlNum,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_origin_orCat,
    0,
    IEC61850_FC_CO,
    IEC61850_CONSTRUCTED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_origin_orCat = {
    DataAttributeModelType,
    "orCat",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_origin,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_origin_orIdent,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_ENUMERATED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_origin_orIdent = {
    DataAttributeModelType,
    "orIdent",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_origin,
    NULL,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_OCTET_STRING_64,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_ctlNum = {
    DataAttributeModelType,
    "ctlNum",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_T,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_INT8U,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_T = {
    DataAttributeModelType,
    "T",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_Test,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_Test = {
    DataAttributeModelType,
    "Test",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper_Check,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_BOOLEAN,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_Oper_Check = {
    DataAttributeModelType,
    "Check",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_Oper,
    NULL,
    NULL,
    0,
    IEC61850_FC_CO,
    IEC61850_CHECK,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_ENUMERATED,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_ctlModel,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_ctlModel = {
    DataAttributeModelType,
    "ctlModel",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod_d,
    NULL,
    0,
    IEC61850_FC_CF,
    IEC61850_ENUMERATED,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Mod_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Mod,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_Beh = {
    DataObjectModelType,
    "Beh",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_Beh_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_ENUMERATED,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Beh_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Beh_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Beh_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Beh,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_Health = {
    DataObjectModelType,
    "Health",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_Health_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_ENUMERATED,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Health_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Health_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health,
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_Health_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_Health,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_NamPlt = {
    DataObjectModelType,
    "NamPlt",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade,
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt_vendor,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_NamPlt_vendor = {
    DataAttributeModelType,
    "vendor",
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt,
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt_swRev,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_NamPlt_swRev = {
    DataAttributeModelType,
    "swRev",
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt,
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt_d,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_NamPlt_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt,
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt_configRev,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_NamPlt_configRev = {
    DataAttributeModelType,
    "configRev",
    (ModelNode*) &iedModel_LD0_DRRDRE1_NamPlt,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_RcdMade = {
    DataObjectModelType,
    "RcdMade",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_RcdMade_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_BOOLEAN,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdMade_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdMade_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdMade_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdMade,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_FltNum = {
    DataObjectModelType,
    "FltNum",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_FltNum_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum,
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_INT32,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_FltNum_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum,
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_FltNum_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum,
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_FltNum_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_FltNum,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_RcdStr = {
    DataObjectModelType,
    "RcdStr",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_RcdStr_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_BOOLEAN,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdStr_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdStr_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdStr_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdStr,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_MemUsed = {
    DataObjectModelType,
    "MemUsed",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_MemUsed_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_INT32,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_MemUsed_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_MemUsed_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_MemUsed_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsed,
    NULL,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_RcdClr = {
    DataObjectModelType,
    "RcdClr",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_RcdClr_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_BOOLEAN,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdClr_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdClr_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdClr_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr,
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr_dataNs,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_RcdClr_dataNs = {
    DataAttributeModelType,
    "dataNs",
    (ModelNode*) &iedModel_LD0_DRRDRE1_RcdClr,
    NULL,
    NULL,
    0,
    IEC61850_FC_EX,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataObject iedModel_LD0_DRRDRE1_MemUsedAlm = {
    DataObjectModelType,
    "MemUsedAlm",
    (ModelNode*) &iedModel_LD0_DRRDRE1,
    NULL,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm_stVal,
    0
};

DataAttribute iedModel_LD0_DRRDRE1_MemUsedAlm_stVal = {
    DataAttributeModelType,
    "stVal",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm_q,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_BOOLEAN,
    0 + TRG_OPT_DATA_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_MemUsedAlm_q = {
    DataAttributeModelType,
    "q",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm_t,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_QUALITY,
    0 + TRG_OPT_QUALITY_CHANGED,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_MemUsedAlm_t = {
    DataAttributeModelType,
    "t",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm_d,
    NULL,
    0,
    IEC61850_FC_ST,
    IEC61850_TIMESTAMP,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_MemUsedAlm_d = {
    DataAttributeModelType,
    "d",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm,
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm_dataNs,
    NULL,
    0,
    IEC61850_FC_DC,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};

DataAttribute iedModel_LD0_DRRDRE1_MemUsedAlm_dataNs = {
    DataAttributeModelType,
    "dataNs",
    (ModelNode*) &iedModel_LD0_DRRDRE1_MemUsedAlm,
    NULL,
    NULL,
    0,
    IEC61850_FC_EX,
    IEC61850_VISIBLE_STRING_255,
    0,
    NULL,
    0};





extern SettingGroupControlBlock iedModel_LD0_LLN0_sgcb;

SettingGroupControlBlock iedModel_LD0_LLN0_sgcb = {&iedModel_LD0_LLN0, 1, 4, 0, false, 0, 0, NULL};




IedModel iedModel = {
    "AA1J1Q01A1",
    &iedModel_LD0,
    &iedModelds_LD0_LLN0_StatNrml,
    NULL,
    NULL,
    NULL,
    &iedModel_LD0_LLN0_sgcb,
    NULL,
    NULL,
    initializeValues
};

static void
initializeValues()
{

iedModel_LD0_LLN0_Mod_stVal.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_LLN0_Mod_ctlModel.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_LLN0_Mod_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_LLN0_Beh_stVal.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_LLN0_Beh_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_LLN0_Health_stVal.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_LLN0_Health_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_LLN0_NamPlt_vendor.mmsValue = MmsValue_newVisibleString("ABB");

iedModel_LD0_LLN0_NamPlt_swRev.mmsValue = MmsValue_newVisibleString("IED600");

iedModel_LD0_LLN0_NamPlt_configRev.mmsValue = MmsValue_newVisibleString("149625");

iedModel_LD0_LLN0_NamPlt_ldNs.mmsValue = MmsValue_newVisibleString("IEC 61850-7-4:2003");

iedModel_LD0_LPHD1_PhyNam_vendor.mmsValue = MmsValue_newVisibleString("ABB");

iedModel_LD0_LPHD1_PhyNam_model.mmsValue = MmsValue_newVisibleString("IED600");

iedModel_LD0_LPHD1_PhyHealth_stVal.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_LPHD1_PhyHealth_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_LPHD1_Proxy_stVal.mmsValue = MmsValue_newBoolean(false);

iedModel_LD0_LPHD1_Proxy_d.mmsValue = MmsValue_newVisibleString("Notification/alarm");

iedModel_LD0_DRRDRE1_Mod_stVal.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_DRRDRE1_Mod_ctlModel.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_DRRDRE1_Mod_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_DRRDRE1_Beh_stVal.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_DRRDRE1_Beh_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_DRRDRE1_Health_stVal.mmsValue = MmsValue_newIntegerFromInt32(1);

iedModel_LD0_DRRDRE1_Health_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_DRRDRE1_NamPlt_vendor.mmsValue = MmsValue_newVisibleString("ABB");

iedModel_LD0_DRRDRE1_NamPlt_swRev.mmsValue = MmsValue_newVisibleString("IED600");

iedModel_LD0_DRRDRE1_RcdMade_stVal.mmsValue = MmsValue_newBoolean(false);

iedModel_LD0_DRRDRE1_RcdMade_d.mmsValue = MmsValue_newVisibleString("Notification/alarm");

iedModel_LD0_DRRDRE1_FltNum_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_DRRDRE1_RcdStr_stVal.mmsValue = MmsValue_newBoolean(false);

iedModel_LD0_DRRDRE1_RcdStr_d.mmsValue = MmsValue_newVisibleString("Notification/alarm");

iedModel_LD0_DRRDRE1_MemUsed_d.mmsValue = MmsValue_newVisibleString("Integer/enum");

iedModel_LD0_DRRDRE1_RcdClr_d.mmsValue = MmsValue_newVisibleString("Notification/alarm");

iedModel_LD0_DRRDRE1_RcdClr_dataNs.mmsValue = MmsValue_newVisibleString("$");

iedModel_LD0_DRRDRE1_MemUsedAlm_d.mmsValue = MmsValue_newVisibleString("Notification/alarm");

iedModel_LD0_DRRDRE1_MemUsedAlm_dataNs.mmsValue = MmsValue_newVisibleString("$");
}
