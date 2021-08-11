/*
 * ptxstring.cpp
 *
 *  Created on: 8.8.2016
 *      Author: J-Skalsky
 */

#include "ptxstring.h"
#include <cstdarg>
#include <cstdio>

namespace diag {

ptx_string::ptx_string()
{

}

ptx_string::~ptx_string()
{

}

std::string ptx_string::format (const char *f,...)
{
    va_list argptr;
//lint -esym(530,argptr) # intended as is
    (void)va_start(argptr,f);
    std::string s;
    char buf[BUFF_SIZE];
#if defined(_WIN32) && _MSC_VER >= 1400
    vsprintf_s (buf,BUFF_SIZE,f,argptr);
#else
    if (vsprintf (buf,f,argptr) == 0)
    {
        return s;
    }
#endif
    s = buf;
    return s;
}

std::vector<std::string>ptx_string::split(const std::string& s)
{
    std::vector<std::string> v;
    std::string::const_iterator last = s.begin(),i = s.begin();
    for (; i != s.end(); i++)
    {
        if (*i == ' ' || *i == '\n' || *i == '\t' || *i == '\r')
        {
            v.push_back(std::string(last, i));
            last = i + 1;
        }
    }
    if (last != i)
        v.push_back(std::string(last, i));
    return v;
}

std::vector<std::string>ptx_string::split(const std::string& s, const char *del)
{
    std::vector<std::string> v;
    std::string::const_iterator last = s.begin(),i = s.begin();
    for (; i != s.end(); i++)
    {
        for (int j=0; del[j] != 0; ++j)
        {
            if (*i == del[j])
            {
                v.push_back(std::string(last, i));
                last = i + 1;
                break;
            }
        }
    }
    if (last != i)
        v.push_back(std::string(last, i));
    return v;
}
} /* namespace diag */
