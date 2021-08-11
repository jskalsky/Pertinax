#include <string>
#include <list>
#include <vector>
#include <sys/socket.h>
#include <fcntl.h>
#include <arpa/inet.h>
#include <ctime>
#include <cstdio>
#include <sys/resource.h>
#include <errno.h>
#include <string.h>
#include <dirent.h>
#include <signal.h>
#include <stdlib.h>
#include <unistd.h>
#include "sys/stat.h"
#include "DiagTypes.h"

#include "UploadResponse.h"
#include "DownloadResponse.h"
#include "GetDirectoriesResponse.h"
#include "GetFilesResponse.h"
#include "GetErrorLogResponse.h"
#include "ReadDirResponse.h"
#include "SymLinkResponse.h"
#include "RemoveResponse.h"

#define DIAG_PORT                20002
#define MAX_PENDING_CONNECTIONS    5
using namespace diag;

uint8    *pBuffer = NULL;
int        BufferSize = 0;

UploadResponse uploadResponse;
DownloadResponse downloadResponse;
GetDirectoriesResponse getDirectoriesResponse;
GetFilesResponse getFilesResponse;
GetErrorLogResponse getErrorLogResponse;
ReadDirResponse readDirResponse;
SymLinkResponse symlinkResponse;
RemoveResponse removeResponse;

using namespace std;

void Handler(int sigNum, siginfo_t *info, void *userContext)
{
    struct ucontext_t *uc = (struct ucontext_t *)userContext;

    printf("Unhandled signal %d (%s), address is %p from %p\n", sigNum, strsignal(sigNum), info->si_addr,
            (void *) uc->uc_mcontext.regs->link);
    exit(1);
}

const char *GetVersion(const char *arg)
{
    while(*arg != 0)
    {
        if(*arg == '.')
        {
            return arg + 1;
        }
        ++arg;
    }
    return NULL;
}

/*void SetCurrentProcessPriority(int priority)
{
    struct sched_param schedParams = { priority };
    (void)sched_setscheduler(getpid(), SCHED_RR, &schedParams);
}*/

int main (int argc,const char *argv[])
{
    try
    {
        for (int i = 1; i <= 31; ++i)
        {
            if ((i != SIGKILL) && (i != SIGSTOP) && (i != SIGCHLD))
            {
                struct sigaction sigact;

                sigact.sa_sigaction = Handler;
                sigact.sa_flags = SA_RESTART | SA_SIGINFO;

                if (sigaction(i, &sigact, (struct sigaction *)NULL) != 0)
                {
                    printf("Cannot install critical error handler for signal %d", i);
                }
            }
        }

        bool end_loop = false;
        // nastaveni velikosti stacku na 256K pro kazdy thread
        // defaultne je 8 M
        // nutne hned na zacatku Perun
        struct rlimit rlim;
        (void)getrlimit((int)RLIMIT_STACK,&rlim);
        rlim.rlim_cur = 0x40000;
        rlim.rlim_max = 0x40000;
        (void)setrlimit((int)RLIMIT_STACK,&rlim);

        //SetCurrentProcessPriority(50);
        string ver_perun = "v2.0.0";
        if(argc >= 1)
        {
            const char *version = GetVersion(argv[0]);
            if(version != NULL)
            {
                ver_perun = "v";
                ver_perun += version;
            }
        }

        printf ("\n\n********************************************************************************\n");
        printf ("* DiagServer          %s\t9.3.2021\t       (c) ZAT a.s. 2008-2021  *\n",ver_perun.c_str());
        printf ("********************************************************************************\n");

        int socket_desc = socket(AF_INET , (int)SOCK_STREAM , 0);
        if (socket_desc < 0)
        {
            printf("Create socket error %d\n", errno);
            return 1;
        }
        struct sockaddr_in server;
        server.sin_family = AF_INET;
        server.sin_addr.s_addr = INADDR_ANY;
        server.sin_port = htons(DIAG_PORT);
        if( bind(socket_desc, (struct sockaddr *)(void*)&server , sizeof(server)) < 0)
        {
            printf("Bind error %d\n", errno);
            return 1;
        }
        if(listen(socket_desc , MAX_PENDING_CONNECTIONS) < 0)
        {
            printf("Listen error %d\n", errno);
            return 1;
        }
        do
        {
            int size = sizeof(struct sockaddr_in);
            struct sockaddr_in client;
            int client_sock = accept(socket_desc, (struct sockaddr *)(void*)&client, (socklen_t*)&size);
            if (client_sock < 0)
            {
                printf("Accept failed, %d\n", errno);
                return 1;
            }
//            printf("Accept %s, %d\n",inet_ntoa(client.sin_addr), client.sin_port);
            uint8 buffer[0x400];
            int buffer_offset = 0;
            int length = 0;
            int rx_length = 0;
            while(length == 0)
            {
                int rx = read(client_sock, buffer, sizeof(int));
//                printf("rx1= %d\n", rx);
                if(rx < 0)
                {
                    printf("Read error %d\n", errno);
                    continue;
                }
                else
                {
//                    printf("%02X, %02X, %02X, %02X\n", buffer[0], buffer[1], buffer[2], buffer[3]);
                    memcpy(&length, buffer, sizeof(int));
                    if(length > BufferSize)
                    {
                        if(pBuffer != NULL)
                        {
                            delete[] pBuffer;
                        }
                        pBuffer = new uint8[(uint32)length];
                        BufferSize = length;
//                        printf("BufferSize= %d\n", BufferSize);
                    }
                }
            }
//            printf("length= %d\n", length);
            while(rx_length < length)
            {
//                printf("rx_length= %d, length= %d\n", rx_length, length);
                int rx = read(client_sock, buffer, 0x400);
//                printf("rx= %d\n", rx);
                if(rx < 0)
                {
                    printf("Read error %d\n", errno);
                    continue;
                }
                else
                {
//                    printf("buffer_offset= %d, rx= %d, length= %d\n",buffer_offset,rx,length);
                    if((buffer_offset + rx) <= length)
                    {
                        memcpy(pBuffer + buffer_offset, buffer, (uint32)rx);
                        buffer_offset += rx;
                        rx_length += rx;
                    }
                }
            }
            
            uint16 id = *(uint16*)(void*)&pBuffer[0];
//            printf("id= %d\n", id);
            try
            {
                switch((MessageType)id)
                {
                case Upload:
                    uploadResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case GetFiles:
                    getFilesResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case GetDirectories:
                    getDirectoriesResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case Download:
                    downloadResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case GetErrorLog:
                    getErrorLogResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case ReadDir:
                    readDirResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case SymLink:
                    symlinkResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case Remove:
                    removeResponse.MakeResponse(client_sock, length, pBuffer);
                    break;
                case None:
                    break;
                default:
                    break;
                }
            }
            catch(const exception& e)
            {
                printf("In exception %s\n", e.what());
            }
            (void)shutdown(client_sock, SHUT_RDWR);
            close(client_sock);
        }while(!end_loop);
    }
    catch (const exception &e)
    {
        printf("DiagServer,Exception - %s.",e.what());
        return 1;
    }
    return 0;
}

