#include "MyTask.h"

MyTask::MyTask()
{
    pBits = new bool[MAX_BITS];
    pInt8 = new int8[MAX_INT8];
    pUint8 = new uint8[MAX_UINT8];
    pInt16 = new int16[MAX_INT16];
    pUint16 = new uint16[MAX_UINT16];
    pInt32 = new int32[MAX_INT32];
    pUint32 = new uint32[MAX_UINT32];
    pFloat = new float[MAX_REAL];
    pDouble = new double[MAX_DREAL];

    InFlags = new OpcUaFlag[MAX_INFLAGS]();
    for (int i = 0; i < MAX_INFLAGS; ++i)
    {
        InFlags[i].FlagSize = 0;
        InFlags[i].Flag = NULL;
    }
    OutFlags = new OpcUaFlag[MAX_OUTFLAGS]();
    for (int i = 0; i < MAX_OUTFLAGS; ++i)
    {
        OutFlags[i].FlagSize = 0;
        OutFlags[i].Flag = NULL;
    }
    TaskNo = 0;
    NrInFlags = 0;
    NrOutFlags = 0;
}

MyTask::~MyTask()
{
    delete pBits;
    delete pInt8;
    delete pUint8;
    delete pInt16;
    delete pUint16;
    delete pInt32;
    delete pUint32;
    delete pFloat;
    delete pDouble;
    for (int i = 0; i < NrInFlags; ++i)
    {
        if (InFlags[i].Flag != NULL)
        {
            delete InFlags[i].Flag;
        }
    }
    delete InFlags;
    for (int i = 0; i < NrOutFlags; ++i)
    {
        if (OutFlags[i].Flag != NULL)
        {
            delete OutFlags[i].Flag;
        }
    }
    delete OutFlags;
}

void MyTask::GetIoBIT(ptx::uint32 index, ptx::IO_BIT& val)
{
    if (index < MAX_BITS)
    {
        val.data = pBits[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoINT8(ptx::uint32 index, ptx::IO_INT8& val)
{
    if (index < MAX_INT8)
    {
        val.data = pInt8[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoUINT8(ptx::uint32 index, ptx::IO_UINT8& val)
{
    if (index < MAX_UINT8)
    {
        val.data = pUint8[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoINT16(ptx::uint32 index, ptx::IO_INT16& val)
{
    if (index < MAX_INT16)
    {
        val.data = pInt16[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoUINT16(ptx::uint32 index, ptx::IO_UINT16& val)
{
    if (index < MAX_UINT16)
    {
        val.data = pUint16[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoINT32(ptx::uint32 index, ptx::IO_INT32& val)
{
    if (index < MAX_INT32)
    {
        val.data = pInt32[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoUINT32(ptx::uint32 index, ptx::IO_UINT32& val)
{
    if (index < MAX_UINT32)
    {
        val.data = pUint32[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoREAL(ptx::uint32 index, ptx::IO_REAL& val)
{
    if (index < MAX_REAL)
    {
        val.data = pFloat[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void MyTask::GetIoDREAL(ptx::uint32 index, ptx::IO_DREAL& val)
{
    if (index < MAX_DREAL)
    {
        val.data = pDouble[index];
        val.platnost = PLATNA;
    }
    else
    {
        val.platnost = HW_CHYBA;
    }
}

void* MyTask::GetFlagIn(uint32 ord, uint32 index, uint16& size)
{
    if (index < MAX_INFLAGS)
    {
        size = InFlags[index].FlagSize;
        printf("index= %lu, size= %u\n", index, size);
        return (void*)InFlags[index].Flag;
    }
    return NULL;
}

void* MyTask::GetFlagIn(uint32 ord, uint32 index)
{
    if (index < MAX_INFLAGS)
    {
        return (void*)InFlags[index].Flag;
    }
    return NULL;
}

void MyTask::SetIoBIT(uint32 index, const IO_BIT& val)
{
    if (index < MAX_BITS)
    {
        pBits[index] = val.data;
    }
}

void MyTask::SetIoINT8(uint32 index, const IO_INT8& val)
{
    if (index < MAX_INT8)
    {
        pInt8[index] = val.data;
    }
}

void MyTask::SetIoUINT8(uint32 index, const IO_UINT8& val)
{
    if (index < MAX_UINT8)
    {
        pUint8[index] = val.data;
    }
}

void MyTask::SetIoINT16(uint32 index, const IO_INT16& val)
{
    if (index < MAX_INT16)
    {
        pInt16[index] = val.data;
    }
}

void MyTask::SetIoUINT16(uint32 index, const IO_UINT16& val)
{
    if (index < MAX_UINT16)
    {
        pUint16[index] = val.data;
    }
}

void MyTask::SetIoINT32(uint32 index, const IO_INT32& val)
{
    if (index < MAX_INT32)
    {
        pInt32[index] = val.data;
    }
}

void MyTask::SetIoUINT32(uint32 index, const IO_UINT32& val)
{
    if (index < MAX_UINT32)
    {
        pUint32[index] = val.data;
    }
}

void MyTask::SetIoREAL(uint32 index, const IO_REAL& val)
{
    if (index < MAX_REAL)
    {
        pFloat[index] = val.data;
    }
}

void MyTask::SetIoDREAL(uint32 index, const IO_DREAL& val)
{
    if (index < MAX_DREAL)
    {
        pDouble[index] = val.data;
    }
}

void* MyTask::GetFlagOut(uint32 ord, uint32 index, uint16& size)
{
    if (index < MAX_OUTFLAGS)
    {
        size = OutFlags[index].FlagSize;
        return (void*)OutFlags[index].Flag;
    }
    return NULL;
}

void* MyTask::GetFlagOut(uint32 ord, uint32 index)
{
    if (index < MAX_OUTFLAGS)
    {
        return (void*)OutFlags[index].Flag;
    }
    return NULL;
}

void MyTask::AddFlag(uint8* flag, uint8 compiledPtxType, int compiledSlot, uint16 compiledType, uint16 compiledIndex, uint32 compiledCachePtr, uint16 compiledArrayIndex,
    const char* compiledId)
{
    *flag = compiledPtxType;
    memcpy(flag + 1, &compiledSlot, 4);
    memcpy(flag + 5, &compiledType, 2);
    memcpy(flag + 7, &compiledIndex, 2);
    memcpy(flag + 9, &compiledCachePtr, 4);
    memcpy(flag + 13, &compiledArrayIndex, 2);
    memcpy(flag + 15, compiledId, strlen(compiledId));
}

void MyTask::AddFlag(uint8 compiledPtxType, int compiledSlot, uint16 compiledType, uint16 compiledIndex, uint32 compiledCachePtr, uint16 compiledArrayIndex,
    const char* compiledId, bool isInput)
{
    printf("AddFlag %d, %d\n", isInput, NrInFlags);
    if (isInput)
    {
        if (NrInFlags < MAX_INFLAGS)
        {
            InFlags[NrInFlags].FlagSize = (uint16)strlen(compiledId) + 15;
            InFlags[NrInFlags].Flag = new uint8[InFlags[NrInFlags].FlagSize];
            AddFlag(InFlags[NrInFlags].Flag, compiledPtxType, compiledSlot, compiledType, compiledIndex, compiledCachePtr, compiledArrayIndex, compiledId);
            ++NrInFlags;
        }
        else
        {
            if (NrOutFlags < MAX_OUTFLAGS)
            {
                OutFlags[NrOutFlags].FlagSize = (uint16)strlen(compiledId) + 15;
                OutFlags[NrOutFlags].Flag = new uint8[OutFlags[NrOutFlags].FlagSize];
                AddFlag(OutFlags[NrOutFlags].Flag, compiledPtxType, compiledSlot, compiledType, compiledIndex, compiledCachePtr, compiledArrayIndex, compiledId);
                ++NrOutFlags;
            }
        }
    }
}

