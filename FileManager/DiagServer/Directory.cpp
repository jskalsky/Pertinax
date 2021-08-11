/*
 * Directory.cpp
 *
 *  Created on: 9.8.2016
 *      Author: J-Skalsky
 */

#include "Directory.h"
#include "ptxstring.h"
#include <vector>
#include <errno.h>
#include <dirent.h>
#include <stdexcept>
#include <cstdio>
#include <unistd.h>
#include "sys/stat.h"

namespace diag {

Directory::Directory()
{

}

Directory::~Directory()
{

}

std::string Directory::GetCurrentDir(void)
{
    std::string current;
    char buf[PATH_MAX];
    char *result = getcwd(buf, PATH_MAX);
    if (result != NULL)
    {
        current = buf;
    }
    return current;
}

void Directory::MakeDirFromPath (const char *dir)
{
    uint32 mode = S_IRUSR | S_IWUSR | S_IRGRP | S_IWGRP;
    std::string pathName = dir;
    std::string::size_type ll = pathName.find_last_of('/');
    if (ll != std::string::npos)
    {
        std::vector<std::string> items = ptx_string::split(pathName.substr(0, ll), "/");
        std::string path;
        std::string current = GetCurrentDir();
        for (uint32 i=0; i < items.size(); ++i)
        {
            if (items[i].size() == 0)
            {
                path = "/";
            }
            else
            {
                if(path[path.size() - 1] != '/')
                {
                    path += '/';
                }
                path += items[i];
            }
            int result = chdir(path.c_str());
            if (result == -1)
            {
                if (errno == ENOENT)
                {
                    result = mkdir(path.c_str(), mode);
                    if (result != 0)
                    {
                        throw (std::runtime_error (ptx_string::format("Make directory error %d : %s,%d)",errno, __FILE__,__LINE__)));
                    }
                }
                else
                {
                    throw (std::runtime_error (ptx_string::format("Make directory error %d : %s,%d)",errno, __FILE__,__LINE__)));
                }
            }
        }
        if (chdir(current.c_str()) != 0)
        {
            throw (std::runtime_error (ptx_string::format("Make directory error %d : %s,%d)",errno, __FILE__,__LINE__)));
        }
    }
}

std::list<std::string> Directory::ReadDirectory(const char *dir, bool directories)
{
//    printf("ReadDirectory %s, %d\n",dir, directories);
    std::list<std::string> result;
    DIR *hdir = opendir (dir);
//    printf("hdir= %p\n", hdir);
    if (hdir == NULL)
        return result;
    struct dirent *pd;
    while ((pd = readdir (hdir)) != NULL)
    {
        std::string n = pd->d_name;
        if ((n.compare (".")==0) || (n.compare("..")==0))
            continue;
        std::string tmp = dir;
        tmp += '/';
        tmp += n;
        struct stat st;
        int res = lstat (tmp.c_str(), &st);
//        printf("res= %d\n", res);
        if (res == -1)
        {
            printf("errno= %d\n", errno);
            continue;
        }
//        printf("d_name= %s\n",pd->d_name);
        if(S_ISDIR(st.st_mode) && directories)
        {
            result.push_back(pd->d_name);
//            printf("dir add %s\n",pd->d_name);
        }
        else
        {
            if(S_ISREG(st.st_mode) && !directories)
            {
                result.push_back(pd->d_name);
//                printf("file add %s\n",pd->d_name);
            }
        }
    }
    (void)closedir (hdir);
    return result;
}

} /* namespace diag */
