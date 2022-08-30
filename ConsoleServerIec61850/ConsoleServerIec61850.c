// ConsoleServerIec61850.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "iec61850_server.h"
#include "hal_thread.h"
#include "static_model.h"
#include <signal.h>
#include <stdlib.h>
#include <stdio.h>

extern IedModel iedModel;

static int running = 0;

void
sigint_handler(int signalId)
{
	running = 0;
}

int main()
{
	IedServer iedServer = IedServer_create(&iedModel);
	IedServer_start(iedServer, 102);
	if (!IedServer_isRunning(iedServer))
	{
		printf("Starting server failed! Exit.\n");
		IedServer_destroy(iedServer);
		exit(-1);
	}
	printf("Server started...\n");
	running = 1;

	signal(SIGINT, sigint_handler);
	bool bValue = false;
	int wValue = 0;

	while (running)
	{
		Thread_sleep(5000);
		IedServer_lockDataModel(iedServer);
		IedServer_updateBooleanAttributeValue(iedServer, IEDMODEL_LD0_DRRDRE1_RcdClr_stVal, bValue);
		IedServer_updateInt32AttributeValue(iedServer, IEDMODEL_LD0_LLN0_Health_stVal, wValue);
		bValue = !bValue;
		wValue = wValue + 1;
		IedServer_unlockDataModel(iedServer);
	}    IedServer_stop(iedServer);

	/* Cleanup - free all resources */
	IedServer_destroy(iedServer);

	return 0;
}
