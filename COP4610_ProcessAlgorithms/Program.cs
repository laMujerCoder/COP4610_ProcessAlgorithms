using COP4610_ProcessAlgorithms;




PrepareProcessesData prepareProcesses = new PrepareProcessesData();
List<Process> run1 = prepareProcesses.ReturnProcesses(); 
List<Process> run2 = prepareProcesses.ReturnProcesses();
List<Process> run3 = prepareProcesses.ReturnProcesses();



//RunningSingleQueueProcesses runProcessFCFS = new RunningSingleQueueProcesses(run1, true);
//runProcessFCFS.RunProcesses();

//RunningSingleQueueProcesses runProcessSJC = new RunningSingleQueueProcesses(run2, false);
//runProcessSJC.RunProcesses();



MultiLevelQueue runProcess = new MultiLevelQueue(run3);
runProcess.RunProcesses();
