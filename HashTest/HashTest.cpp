// HashTest.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <stdlib.h>
#include <map>
#include <string>
#include <functional>

std::map<size_t, std::string> Hashes;

int main()
{
    for (int i = 0; i < 1000000; ++i)
    {
        std::string s;
        for (int j = 0; j < 16; ++j)
        {
            s  += (char)(rand() % 100);
        }
        size_t hash = std::hash<std::string>{}(s);
        std::map<size_t, std::string>::iterator it = Hashes.find(hash);
        if (it != Hashes.end())
        {
            printf("Existuje %x, %x\n", hash, (*it).first);
            for (int j = 0; j < 16; ++j)
            {
                printf(" %02X", s[j]);
            }
            printf("\n");
            for (int j = 0; j < 16; ++j)
            {
                printf(" %02X", (*it).second[j]);
            }
            printf("\n");
        }
        else
        {
            Hashes[hash] = s;
        }
    }
    printf("Count= %u\n", Hashes.size());
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
