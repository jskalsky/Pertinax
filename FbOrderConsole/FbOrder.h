#pragma once
#include <string>
#include <list>

typedef struct
{
	std::string			ref;
	enum NodeTypes		node_type;
	int					component_no;
	int					weight;
	int					used;
	std::list<size_t>	succ;
	std::list<size_t>	symmetry;
	enum NodeStatus     node_status;
}tagNODE;

