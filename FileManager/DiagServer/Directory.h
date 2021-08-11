/*
 * Directory.h
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#ifndef DIRECTORY_H_
#define DIRECTORY_H_

#include <string>
#include <list>
#include "DiagTypes.h"

namespace diag {

class Directory
{
public:
    Directory();
    virtual ~Directory();
    static std::string GetCurrentDir(void);
    static void MakeDirFromPath (const char *dir);
    static std::list<std::string> ReadDirectory(const char *dir, bool directories);
};

} /* namespace diag */
#endif /* DIRECTORY_H_ */
