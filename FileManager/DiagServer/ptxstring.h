/*
 * ptxstring.h
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#ifndef PTXSTRING_H_
#define PTXSTRING_H_

#include <string>
#include <vector>

#define    BUFF_SIZE    1024

namespace diag {

class ptx_string
{
public:
    ptx_string();
    virtual ~ptx_string();
    static std::vector<std::string> split(const std::string&);
    static std::vector<std::string> split(const std::string&, const char *del);
    static std::string format (const char *f,...);
};

} /* namespace diag */
#endif /* PTXSTRING_H_ */
