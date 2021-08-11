/*
 * Response.h
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#ifndef RESPONSE_H_
#define RESPONSE_H_

#include <string>
#include "DiagTypes.h"
#define    STARTING_BUF_SIZE    0x80000
#define ADD_BUF_SIZE        0x10000

namespace diag {

enum MessageType{None, Upload, Download, GetFiles, GetDirectories, GetErrorLog, ReadDir, SymLink, Remove};

class Response
{
public:
    Response();
    virtual ~Response();
    virtual void MakeResponse(int client, int length, uint8 *buffer)=0;
    std::string GetString(int index, const uint8 *pBuffer) const;

    void Add(uint8 val) const;
    void Add(uint16 val) const;
    void Add(uint32 val) const;
    void Add(const char *) const;
    void Add(const uint8 *, int length) const;
    void AddFile(const char *fileName) const;
    void Write(int client_sock, const void *src, int length) const;

protected:
    static uint8 *pBuffer;
    static int BufferLength;
    static int Offset;
    void ReallocBuffer(int length) const;
};

} /* namespace diag */
#endif /* RESPONSE_H_ */
