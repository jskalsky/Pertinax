#pragma once
#include "PtxTypes.h"
#include "PtxIos.h"

#define MAX_BITS        512
#define MAX_INT8        512
#define MAX_UINT8       512
#define MAX_INT16       512
#define MAX_UINT16      512
#define MAX_INT32       512
#define MAX_UINT32      512
#define MAX_REAL        512
#define MAX_DREAL       512

#define MAX_INFLAGS     32
#define MAX_OUTFLAGS    32
#define MAX_FLAGSIZE    128

using namespace ptx;

typedef struct
{
    uint16 FlagSize;
    uint8* Flag;
}OpcUaFlag;

class MyTask
{
public:
    MyTask();
    ~MyTask();
    void GetIoBIT(ptx::uint32 index, ptx::IO_BIT& val);
    void GetIoINT8(ptx::uint32 index, ptx::IO_INT8& val);
    void GetIoUINT8(ptx::uint32 index, ptx::IO_UINT8& val);
    void GetIoINT16(ptx::uint32 index, ptx::IO_INT16& val);
    void GetIoUINT16(ptx::uint32 index, ptx::IO_UINT16& val);
    void GetIoINT32(ptx::uint32 index, ptx::IO_INT32& val);
    void GetIoUINT32(ptx::uint32 index, ptx::IO_UINT32& val);
    void GetIoREAL(ptx::uint32 index, ptx::IO_REAL& val);
    void GetIoDREAL(ptx::uint32 index, ptx::IO_DREAL& val);

    void SetIoBIT(uint32 index, const IO_BIT& val);
    void SetIoINT8(uint32 index, const IO_INT8& val);
    void SetIoUINT8(uint32 index, const IO_UINT8& val);
    void SetIoINT16(uint32 index, const IO_INT16& val);
    void SetIoUINT16(uint32 index, const IO_UINT16& val);
    void SetIoINT32(uint32 index, const IO_INT32& val);
    void SetIoUINT32(uint32 index, const IO_UINT32& val);
    void SetIoREAL(uint32 index, const IO_REAL& val);
    void SetIoDREAL(uint32 index, const IO_DREAL& val);

    inline int GetNo() { return TaskNo; }
    inline void SetNo(int taskNo) { TaskNo = taskNo; }
    void* GetFlagIn(uint32 ord, uint32 index, uint16& size);
    void* GetFlagIn(uint32 ord, uint32 index);
    void* GetFlagOut(uint32 ord, uint32 index, uint16& size);
    void* GetFlagOut(uint32 ord, uint32 index);

    void AddFlag(uint8 compiledPtxType, int compiledSlot, uint16 compiledType, uint16 compiledIndex, uint32 compiledCachePtr, uint16 compiledArrayIndex,
        const char* compiledId, bool isInput = true);
private:
    void AddFlag(uint8* flag, uint8 compiledPtxType, int compiledSlot, uint16 compiledType, uint16 compiledIndex, uint32 compiledCachePtr, uint16 compiledArrayIndex,
        const char* compiledId);
private:
    bool* pBits;
    int8* pInt8;
    uint8* pUint8;
    int16* pInt16;
    uint16* pUint16;
    int32* pInt32;
    uint32* pUint32;
    float* pFloat;
    double* pDouble;
    int TaskNo;
    OpcUaFlag* InFlags;
    int NrInFlags;
    OpcUaFlag* OutFlags;
    int NrOutFlags;
};

