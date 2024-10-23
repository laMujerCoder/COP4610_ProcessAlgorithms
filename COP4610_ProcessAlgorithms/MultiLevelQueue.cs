using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace COP4610_ProcessAlgorithms
{
    public class MultiLevelQueue
    {
        public int Time = 0;
        public List<Process> Processes_In_I_O = new List<Process>();
        public Process RunningProcess; //Process currently running in cpu
        public List<Process> Processesfinished = new List<Process>();
        public ReadyQueueModel ReadyQueue1; 
        public ReadyQueueModel ReadyQueue2;
        public ReadyQueueModel ReadyQueue3;
        public bool AllProcessesDone = false;
        public int QueueInUse = 1; // could be 1,2, or 3 


        //constants
        public int Queue1TQ = 5;
        public int Queue2TQ = 10; 

        //Classes used 
        public RunningSingleQueueProcesses RunningSingleQueueProcesses;

        public MultiLevelQueue(List<Process> processes)
        {
            Queue<Process> tempQueue = new Queue<Process>();
            foreach (Process process in processes)
            {
                process.ReadyQueueNumber = 1;
                process.TqTimeLeft = Queue1TQ; 
                tempQueue.Enqueue(process);
            }
            ReadyQueue1 = new ReadyQueueModel
            {
                Ready_Queue = tempQueue,
                TQ = Queue1TQ,
            };
            ReadyQueue2 = new ReadyQueueModel
            {
                TQ = Queue2TQ,

            };
            ReadyQueue3 = new ReadyQueueModel(); 

            RunningSingleQueueProcesses = new RunningSingleQueueProcesses();
        }

        public void RunProcesses()
        {
            while (!AllProcessesDone)
            {
                Console.WriteLine("Time:" +Time);
                Time++;

                //Add from ready queue if not process is currently running
                if (RunningProcess == null)
                {
                    //first check if all processes are done running and nothing in i/o
                    if (AllProcesssesDoneMethod())
                    {
                        RunningSingleQueueProcesses.TimeAllProcessesFinished = Time - 1; 
                        AllProcessesDone = true;
                    }
                    else if (ProcessInReadyQueue()) //check if there atleast one process in ready queue also updates which queue is in use based on priority 
                    {
                       AddProcessFromReadyQueue();
                        if(RunningProcess != null)
                        {
                            Console.WriteLine("Process: " +RunningProcess.p_num);
                        }
                        
                    }
                }

                AddWaitingTime();  

                UpdateIOProcesses();

                UpdateRunningProcess();
               
            }

            
            IEnumerable<Process> ProcessesList = Processesfinished.OrderBy(x => x.p_num);
            RunningSingleQueueProcesses.PrintProcessStats(ProcessesList);
            //var ProcessesList = Processesfinished.OrderBy(x => x.p_num);
            //foreach (var i in ProcessesList)
            //{
            //    Console.WriteLine(i.p_num + " " + i.waiting_time + " " + i.turnaroundtime + " " + i.responsetime);
            //}
        }

        public bool AllProcesssesDoneMethod()
        {
            if(!ProcessInReadyQueue() && Processes_In_I_O.Count == 0)
            {
                return true; 
            }
            return false;
        }

        public void AddWaitingTime()
        {
           
            foreach (var i in ReadyQueue1.Ready_Queue)
            {
                i.waiting_time++;
            }
                   
            foreach (var i in ReadyQueue2.Ready_Queue)
            {
                i.waiting_time++;
            }
                 
            foreach (var i in ReadyQueue3.Ready_Queue)
            {
                i.waiting_time++;
            }             
            
        }

        public void UpdateIOProcesses()
        {
            if (Processes_In_I_O.Count == 0)
            {
                RunningSingleQueueProcesses.CPUNotInUse++; 
                return; }

            foreach (var i in Processes_In_I_O.ToList())
            {
                int bursttime = RunningSingleQueueProcesses.SubtractTimeFromBurst(i.burst_list);
                if (bursttime == 0) //if i/0 is done move to ready queue and remove from i/0
                {
                    AddToReadyQueue(i);
                    Processes_In_I_O.Remove(i);

                    if (Processes_In_I_O.Count == 0)
                    {
                        return;
                    }
                }
            }
        }

        public void UpdateRunningProcess()
        {
            bool IsRR = false; 
            bool TqReached = false;
            //check if anything is in cpu
            if (RunningProcess == null)
            {
                return;
            }
            int burstTime = RunningSingleQueueProcesses.SubtractTimeFromBurst(RunningProcess.burst_list);

            //Check if in queue with Round Robin scheduling
            if (RunningProcess.ReadyQueueNumber == 1 || RunningProcess.ReadyQueueNumber == 2)
            {
               IsRR = true;
                RunningProcess.TqTimeLeft--;
            }
                         
            //check tq has been reached if in RR Queue 
            if ((IsRR) && RunningProcess.TqTimeLeft == 0 && burstTime !=0) 
            { 
                MoveToNextQueue(); 
                TqReached = true;
            }

            if (burstTime == 0 ) //cpu burst is done 
            {
                // check if last cpu burst
                if (RunningSingleQueueProcesses.BurstsAllFinished(RunningProcess.burst_list))
                {
                    RunningProcess.turnaroundtime = Time;
                    Processesfinished.Add(RunningProcess);
                }
                else
                {
                    //add process to i/o if not all burst are done
                    Processes_In_I_O.Add(RunningProcess);
                }
                RunningProcess = null; // remove from cpu usage 
            }
            else if (TqReached)
            {//re-add to ready queue 
                AddToReadyQueue(RunningProcess); //to do: Re name method later 
                RunningProcess = null; // remove from cpu usage 
            }
            
            
        }
        public void MoveToNextQueue()
        {
            RunningProcess.ReadyQueueNumber++;
        }

        public void AddToReadyQueue(Process process)
        {
            switch(process.ReadyQueueNumber)
            {
                case 1:
                    ReadyQueue1.Ready_Queue.Enqueue(process); 
                    break;

                case 2:
                    ReadyQueue2.Ready_Queue.Enqueue(process);
                    break;

                case 3:
                    ReadyQueue3.Ready_Queue.Enqueue(process);
                    break;
            }
        }
        public void CheckIfFirstBurst()
        {
            if (RunningProcess.burst_list[0] != 0 && RunningProcess.ReadyQueueNumber==1) // account for times when tq preempted process
            {
                RunningProcess.responsetime = Time - 1;
            }
        }
        public void AddProcessFromReadyQueue()
        {
            switch (QueueInUse)
            {
                case 1:
                    RunningProcess = ReadyQueue1.Ready_Queue.First();
                    CheckIfFirstBurst();
                    ReadyQueue1.Ready_Queue.Dequeue();
                    RunningProcess.TqTimeLeft = ReadyQueue1.TQ;//restarts Tq when added to running process 
                    break;
                    
                case 2:
                    RunningProcess = ReadyQueue2.Ready_Queue.First();
                    CheckIfFirstBurst();
                    ReadyQueue2.Ready_Queue.Dequeue();
                    RunningProcess.TqTimeLeft = ReadyQueue2.TQ;
                    break;

                case 3:
                    RunningProcess = ReadyQueue3.Ready_Queue.First();
                    CheckIfFirstBurst();
                    ReadyQueue3.Ready_Queue.Dequeue();
                    RunningProcess.TqTimeLeft = ReadyQueue3.TQ;
                    break;
            }
        }
        public bool ProcessInReadyQueue()
        {
            if (ReadyQueue1.Ready_Queue.Any())
            {
                QueueInUse = 1;
                return true; 
            }
            else if(ReadyQueue2.Ready_Queue.Any())
            {
                QueueInUse = 2;
                return true; 
            }
            else if(ReadyQueue3.Ready_Queue.Any())
            {
                QueueInUse = 3;
                return true;
            }
            return false; 
        }
      
    }
}
