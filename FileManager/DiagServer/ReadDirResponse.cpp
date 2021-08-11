#include "ReadDirResponse.h"
#include <string>
#include <dirent.h>
#include "sys/stat.h"
#include <errno.h>

namespace diag
{
    ReadDirResponse::ReadDirResponse()
    {
    }

    ReadDirResponse::~ReadDirResponse()
    {
    }

    void ReadDirResponse::MakeResponse(int client, int length, uint8* buffer)
    {
        std::string folder = GetString(2, buffer);
        DIR* hdir = opendir(folder.c_str());
        Offset = 0;
        if (hdir != NULL)
        {
            uint16 result = 0;
            Add(result);
            struct dirent* entry;
            while ((entry = readdir(hdir)) != NULL)
            {
                std::string tmp = folder;
                tmp += '/';
                tmp += entry->d_name;
//                printf("tmp= %s\n", tmp.c_str());
                struct stat st;
                int res = lstat(tmp.c_str(), &st);
                if (res == -1)
                {
                    continue;
                }
                ItemType itemType = Unknown;
                if (S_ISDIR(st.st_mode))
                {
                    itemType = Directory;
                }
                else
                {
                    if (S_ISREG(st.st_mode))
                    {
                        itemType = File;
                    }
                    else
                    {
                        if (S_ISLNK(st.st_mode))
                        {
                            itemType = Link;
                        }
                    }
                }
//                printf("Add %u, %s\n", (uint16)itemType, entry->d_name);
                Add((uint16)itemType);
                Add(entry->d_name);
            }
            (void)closedir(hdir);
        }
        else
        {
            uint16 result = (uint16)errno;
            Add(result);
        }
//        printf("Offset= %d\n", Offset);
        int txSize = Offset + 2;
        Write(client, &txSize, 4);
        uint16 id = (uint16)ReadDir;
        Write(client, &id, 2);
        Write(client, pBuffer, Offset);
    }
}