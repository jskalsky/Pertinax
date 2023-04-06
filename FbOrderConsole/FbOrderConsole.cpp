// FbOrderConsole.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "FbOrder.h"
#include <vector>
#include <stack>

typedef unsigned long int DWORD;

std::vector<tagNODE*>					vNodes;		// Pole vrcholu
std::vector<size_t>						vSort;		// Serazene bloky, index do pole vrcholu
DWORD								dwNumOfComponents;

enum NodeTypes { NODE_UNKNOWN, NODE_H, NODE_EXTERNAL, NODE_BLOCK };

int Symmetrize(void)
{
    for (size_t i = 0; i < vNodes.size(); ++i)
    {
        std::list<size_t>::iterator it;
        for (it = vNodes[i]->succ.begin(); it != vNodes[i]->succ.end(); ++it)
        {
            vNodes[(*it)]->symmetry.push_back(i);
        }
    }
    return 0;
}

int Decompose(void)
{
    std::stack<size_t> stk;
    DWORD comp = 0;
    for (size_t i = 0; i < vNodes.size(); ++i)
    {
        if (vNodes[i]->used == 0)
        {
            vSort.push_back(i);
            vNodes[i]->used = 1;
            vNodes[i]->component_no = ++comp;
            stk.push(i);
        }
        while (stk.empty() == false)
        {
            size_t act = stk.top();
            printf("act= %u\n", act);
            stk.pop();
            std::list<size_t>::iterator it;
            for (it = vNodes[act]->succ.begin(); it != vNodes[act]->succ.end(); ++it)
            {
                if (vNodes[(*it)]->used == 0)
                {
                    stk.push((*it));
                    printf("push %u\n", (*it));
                    vSort.push_back((*it));
                    vNodes[(*it)]->component_no = comp;
                    vNodes[(*it)]->used = 1;
                }
            }
            for (it = vNodes[act]->symmetry.begin(); it != vNodes[act]->symmetry.end(); ++it)
            {
                if (vNodes[(*it)]->used == 0)
                {
                    stk.push((*it));
                    printf("push sym %u\n", (*it));
                    vSort.push_back((*it));
                    vNodes[(*it)]->component_no = comp;
                    vNodes[(*it)]->used = 1;
                }
            }
        }
    }
    dwNumOfComponents = comp;
    //	TRACE("\nSORT\n");
    //	for (size_t i=0;i<vSort.size();++i)
    //		TRACE ("%s ",vNodes[vSort[i]]->ref.c_str());
    //	TRACE("\n");
    return 0;
}

bool CycleQ(size_t node)
{
    std::stack<size_t> stk;
    stk.push(node);
    bool cycle = false;
    while (stk.empty() == false)
    {
        size_t act = stk.top();
        stk.pop();
        vNodes[act]->used = 2;
        std::list<size_t>::iterator it;
        for (it = vNodes[act]->succ.begin(); it != vNodes[act]->succ.end(); ++it)
        {
            if ((*it) == node)
            {
                cycle = true;
                break;
            }
            else
            {
                if (vNodes[(*it)]->used == 0)
                    stk.push((*it));
            }
        }
        if (cycle == true)
            break;
    }
    for (size_t i = 0; i < vNodes.size(); ++i)
        if (vNodes[i]->used == 2)
            vNodes[i]->used = 0;
    return cycle;
}

int LineUp(void)
{
    std::vector<size_t> sort2;
    sort2.reserve(vNodes.size());
    std::list<size_t> m, n;

    for (size_t i = 0; i < vNodes.size(); ++i)
    {
        vNodes[i]->used = 0;
        std::list<size_t>::iterator it;
        for (it = vNodes[i]->succ.begin(); it != vNodes[i]->succ.end(); ++it)
        {
            ++vNodes[(*it)]->weight;
        }
    }
    size_t cmpstart = 0;
    size_t cmpcnt = 0;
    size_t cnt = 0;
    for (size_t i = 0; i < dwNumOfComponents; ++i)
    {
        cmpcnt = 0;
        while ((cmpstart < vNodes.size()) && (vNodes[vSort[cmpstart]]->component_no == i + 1))
        {
            if (vNodes[vSort[cmpstart]]->weight == 0)
            {
                m.push_back(vSort[cmpstart]);
                vNodes[vSort[cmpstart]]->used = 1;
            }
            cmpstart++;
            cmpcnt++;
        }
        cnt = 0;
        while (cnt < cmpcnt)
        {
            while (m.empty() == false)
            {
                size_t act = m.back();
                m.pop_back();
                sort2.push_back(act);
                //				TRACE("Line %s\n",vNodes[act]->ref.c_str());
                ++cnt;
                std::list<size_t>::iterator it;
                for (it = vNodes[act]->succ.begin(); it != vNodes[act]->succ.end(); ++it)
                {
                    if (vNodes[(*it)]->used == 0)
                    {
                        vNodes[(*it)]->weight--;
                        size_t last = (*it);
                        if (vNodes[(*it)]->weight == 0)
                        {
                            m.push_back((*it));
                            vNodes[(*it)]->used = 1;
                            std::list<size_t>::iterator itn;
                            for (itn = n.begin(); itn != n.end(); ++itn)
                            {
                                if ((*itn) == (*it))
                                {
                                    n.erase(itn);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            bool del = true;
                            std::list<size_t>::iterator itn;
                            for (itn = n.begin(); itn != n.end(); ++itn)
                            {
                                if ((*itn) == (*it))
                                {
                                    del = false;
                                }
                            }
                            if (del == true)
                                n.push_back((*it));
                        }
                    }
                }
            }
            if (cnt < cmpcnt)
            {
                if (n.size() != 0)
                {
                    size_t nback = n.back();
                    n.pop_back();
                    if (CycleQ(nback) == true)
                    {
                        vNodes[nback]->weight = 0;
                        m.push_back(nback);
                        vNodes[nback]->used = 1;
                    }
                }
            }
        }
    }
    vSort.clear();
    for (size_t i = 0; i < sort2.size(); ++i)
    {
        if (vNodes[sort2[i]]->node_type == NODE_BLOCK)
            vSort.push_back(sort2[i]);
    }

    return 0;
}


int BlockOrder(void)
{
    int res = Symmetrize();
    if (res != 0)
        return res;
    printf("Symmetrize Ok\n");
    res = Decompose();
    if (res != 0)
        return res;
    printf("Decompose Ok\n");
    res = LineUp();
    if (res != 0)
        return res;
    printf("LineUp Ok\n");
    return res;
}

int main()
{
    printf("Start...\n");

    FILE* fp;
    fopen_s(&fp, "e:\\nodes.bin", "r");
    if (fp != NULL)
    {
        DWORD nrNodes = 0;
        fread(&nrNodes, 4, 1, fp);
        for (DWORD i = 0; i < nrNodes; ++i)
        {
            tagNODE* node = new tagNODE;
            DWORD h = 0;
            fread(&h, sizeof(DWORD), 1, fp);
            char buf[128];
            memset(buf, 0, 128);
            fread(buf, h, 1, fp);
            node->ref = buf;
            fread(&node->node_type, 4, 1, fp);
            fread(&node->component_no, 4, 1, fp);
            fread(&node->weight, 4, 1, fp);
            fread(&node->used, 4, 1, fp);
            DWORD sizeSucc = 0;
            fread(&sizeSucc, sizeof(DWORD), 1, fp);
            for (DWORD j = 0; j < sizeSucc; ++j)
            {
                fread(&h, sizeof(DWORD), 1, fp);
                node->succ.push_back(h);
            }
            DWORD sizeSym = 0;
            fread(&sizeSym, sizeof(DWORD), 1, fp);
            for (DWORD j = 0; j < sizeSym; ++j)
            {
                fread(&h, sizeof(DWORD), 1, fp);
                node->succ.push_back(h);
            }
            fread(&node->node_status, 4, 1, fp);
            vNodes.push_back(node);
        }
        fclose(fp);
    }

    BlockOrder();

    for (std::vector<tagNODE*>::iterator it = vNodes.begin(); it != vNodes.end(); ++it)
    {
        delete (*it);
    }
    printf("Konec...\n");
    return 0;
}


