using COP4610_ProcessAlgorithms;




int[] list1 = { 5, 27, 3, 31, 5, 43, 4, 18, 6, 22, 4, 26, 3, 24, 4 };
int[] list2 = { 4, 48, 5, 44, 7, 42, 12, 37, 9, 76, 4, 41, 9, 31, 7, 43, 8 };
int[] list3 = { 8, 33, 12, 41, 18, 65, 14, 21, 4, 61, 15, 18, 14, 26, 5, 31, 6 };
int[] list4 = { 3, 35, 4, 41, 5, 45, 3, 51, 4, 61, 5, 54, 6, 82, 5, 77, 3 };
int[] list5 = { 16, 24, 17, 21, 5, 36, 16, 26, 7, 31, 13, 28, 11, 21, 6, 13, 3, 11, 4 };
int[] list6 = { 11, 22, 4, 8, 5, 10, 6, 12, 7, 14, 9, 18, 12, 24, 15, 30, 8 };
int[] list7 = { 14, 46, 17, 41, 11, 42, 15, 21, 4, 32, 7, 19, 16, 33, 10 };
int[] list8 = { 4, 14, 5, 33, 6, 51, 14, 73, 16, 87, 6 };



Process p1 = new Process(list1, 1);
Process p2 = new Process(list2, 2);
Process p3 = new Process(list3, 3);
Process p4 = new Process(list4, 4);
Process p5 = new Process(list5, 5);
Process p6 = new Process(list6, 6);
Process p7 = new Process(list7, 7);
Process p8 = new Process(list8, 8);

List<Process> processList = new List<Process>();

processList.Add(p1);
processList.Add(p2);
processList.Add(p3);
processList.Add(p4);
processList.Add(p5);
processList.Add(p6);
processList.Add(p7);
processList.Add(p8);


RunningSingleQueueProcesses runProcessFCFS = new RunningSingleQueueProcesses(processList, true);
runProcessFCFS.RunProcesses(); 


//int[] list1 = { 6, 2, 3 };
//int[] list2 = { 3, 1, 4 };
//int[] list3 = { 5, 5, 8 };

//Process p1 = new Process(list1, 1);
//Process p2 = new Process(list2, 2);
//Process p3 = new Process(list3, 3);

//List<Process> processList = new List<Process>();

//processList.Add(p1);
//processList.Add(p2);
//processList.Add(p3);

////RunningSingleQueueProcesses runProcessSJC = new RunningSingleQueueProcesses(processList, true);
////runProcessSJC.RunProcesses(); 

//MultiLevelQueue runProcess = new MultiLevelQueue(processList);
//runProcess.RunProcesses();
