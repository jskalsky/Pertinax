// ConsolePublisher.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <iostream>
#include <conio.h>
#include "DrvOpcUa.h"
#include "MyTask.h"
#include "MySystem.h"

DrvOpcUa Opc;
MyTask myTask;
MySystem mySystem;

int main()
{
    try
    {
/*        Opc.Open("e:\\Projects\\Pertinax\\WinOpcUa\\Xml\\Pub.xml");

        for (int i = 0; i < 20; ++i)
        {
            Sleep(5 * 1000);
        }*/

        myTask.AddFlag(8, -1, 3, 1, 0, 0, "Out1(Publisher1)");
        myTask.AddFlag(8, -1, 3, 1, 0, 0, "Out2(Publisher1)");

        Opc.Open("e:\\Projects\\Pertinax\\WinOpcUa\\Xml\\Sub.xml");
        printf("Open Ok\n");
        IO_REAL ioreal;
        uint16 indexy[2]{ 0,1 };
        for (int i = 0; i < 20; ++i)
        {
            Opc.BeforeRead();
            Opc.Read(2, 0, indexy, myTask, mySystem);
            myTask.GetIoREAL(0, ioreal);
            float out1 = ioreal.data;
            myTask.GetIoREAL(1, ioreal);
            float out2 = ioreal.data;
            printf("out1= %f, out2= %f\n", out1, out2);
            Sleep(5 * 1000);
        }
    }
    catch (std::exception exc)
    {
        printf("Exception: %s\n", exc.what());
    }
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
