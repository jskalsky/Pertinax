// MulticastTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <stdlib.h>

struct in_addr localInterface;
struct sockaddr_in groupSock;
struct sockaddr_in localSock;
struct ip_mreq group;
int sd;
char databuf[512] = "Multicast test message";
int datalen = sizeof(databuf);

int main()
{
    WSADATA wsaData;
    WORD version = MAKEWORD(2,2);
    int err = WSAStartup(version, &wsaData);
    if (err != 0)
    {
        printf("Startup error %d\n", err);
        return 1;
    }

    sd = socket(AF_INET, SOCK_DGRAM, 0);
    if (sd < 0)
    {
        printf("Create socket error %d\n", WSAGetLastError());
        return 1;
    }

/*
    memset((char*)&groupSock, 0, sizeof(groupSock));
    groupSock.sin_family = AF_INET;
    groupSock.sin_addr.s_addr = inet_addr("226.1.1.1");
    groupSock.sin_port = htons(4321);
    localInterface.s_addr = inet_addr("10.10.13.251");
    if (setsockopt(sd, IPPROTO_IP, IP_MULTICAST_IF, (char*)&localInterface, sizeof(localInterface)) < 0)
    {
        printf("Setting local interface error\n");
        return 1;
    }
    if (sendto(sd, databuf, datalen, 0, (struct sockaddr*)&groupSock, sizeof(groupSock)) < 0)
    {
        printf("sendto error\n");
        return 1;
    }
    printf("Send Ok\n");*/

    int reuse = 1;
    if (setsockopt(sd, SOL_SOCKET, SO_REUSEADDR, (char*)&reuse, sizeof(reuse)) < 0)
    {
        perror("Setting SO_REUSEADDR error");
        exit(1);
    }
    else
    {
        printf("Setting SO_REUSEADDR...OK.\n");
    }

    memset((char*)&localSock, 0, sizeof(localSock));
    localSock.sin_family = AF_INET;
    localSock.sin_port = htons(4840);
    localSock.sin_addr.s_addr = INADDR_ANY;
    if (bind(sd, (struct sockaddr*)&localSock, sizeof(localSock)))
    {
        perror("Binding datagram socket error");
        exit(1);
    }
    else
    {
        printf("Binding datagram socket...OK.\n");
    }

    group.imr_multiaddr.s_addr = inet_addr("226.1.1.1");
    group.imr_interface.s_addr = inet_addr("10.10.13.251");
    if (setsockopt(sd, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char*)&group, sizeof(group)) < 0)
    {
        perror("Adding multicast group error");
        exit(1);
    }
    else
    {
        printf("Adding multicast group...OK.\n");
    }

    datalen = sizeof(databuf);

    if (recv(sd, databuf, datalen, 0) > 0)
    {
        printf("Recv Ok\n");
    }

    return 0;
}

