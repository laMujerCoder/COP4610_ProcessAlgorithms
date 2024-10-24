using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COP4610_ProcessAlgorithms
{
    public class RunningSingleQueueProcesses
    {
        public int Time = 0;
        public Queue<Process> Ready_Queue = new Queue<Process>();
        public List<Process> Processes_In_I_O = new List<Process>();
        public Process? RunningProcess; //Process currently running in cpu
        public List<Process> Processesfinished = new List<Process>();
        public bool AllProcessesDone = false;
        public bool IsFCFS;
        public int CPUNotInUse = 0;
        public int TimeAllProcessesFinished;
        public bool ContextSwitch = false; 

        public RunningSingleQueueProcesses()
        {

        }
        public RunningSingleQueueProcesses(List<Process> processes, bool isFCFS)// this will be the way the ready queue is set for first come first serve
        {
            IsFCFS = isFCFS;

            List<Process> OrderedProcesses= new List<Process>();
            if (IsFCFS)
            {
                OrderedProcesses = processes; 
            }
            else
            { // order into ready queue depending on size of first burst 
                OrderedProcesses = processes.OrderBy(x => x.burst_list[0]).ToList();

            }
            foreach (Process process in OrderedProcesses)
            {
                process.upcoming_cpuburst_time = process.burst_list[0]; 
                Ready_Queue.Enqueue(process);
                
            }
        }
        public void RunProcesses()
        {
            while (!AllProcessesDone)
            {
               
                Time++;
                 
                //Add from ready queue if not process is currently running
                if (RunningProcess == null)
                {
                    //first check if all processes are done running and nothing in i/o
                    if (!Ready_Queue.Any() && Processes_In_I_O.Count == 0)
                    {
                        TimeAllProcessesFinished = Time - 1; 
                        AllProcessesDone = true;
                        
                    }
                    else if (Ready_Queue.Any()) //check if the process needs i/0 first or check if ready queue is empty 
                    {
                        RunningProcess = Ready_Queue.First();
                        CheckIfFirstBurst(); // if first burst take note of response time
                        Ready_Queue.Dequeue();
                        ContextSwitch = true;
                    }
                }

                Console.WriteLine("Time:" + Time);
                //print info 
                if (ContextSwitch)
                {
                    PrintCurrentStates();
                }

                ContextSwitch = false;


                //Add waiting time to those in waiting in ready queue
                AddWaitingTime();
   
                //subtract i/o burst time from processes in i/0
                UpdateIOProcesses();

                //subtract cpu burst time from running process
                UpdateRunningProcess();
            }

            IEnumerable<Process> ProcessesList = Processesfinished.OrderBy(x => x.p_num);
            PrintProcessStats(ProcessesList);
        }
        public void PrintCurrentStates()
        {
            if (RunningProcess != null) { Console.WriteLine(
                "Running Process: " + RunningProcess.p_num);
            }
            else { Console.WriteLine("No Process in CPU"); }
            Console.WriteLine("Processes in Ready Queue   CPU Burst Time ");
            foreach (var i in Ready_Queue)
            {
                
                Console.WriteLine(i.p_num+ "                              " +i.upcoming_cpuburst_time);
                
            }
            if(Processes_In_I_O.Any())
            {
                Console.WriteLine("Processes in I/O   Remaining I/O Burst");
                foreach (var i_o in Processes_In_I_O)
                {
                    Console.WriteLine(i_o.p_num+ "                   " + FindNextBurstTime(i_o.burst_list));
                }
            }
            else
            {
                Console.WriteLine("No processes in I/O");
            }
            if (Processesfinished.Any())
            {
                Console.WriteLine("Finished Processes");
                foreach (var proc in Processesfinished)
                {
                    Console.WriteLine(proc.p_num);
                }
            }
            else
            {
                Console.WriteLine("No Finished Processes");
            }
        }

        public void CheckIfFirstBurst()
        {
            if (RunningProcess.burst_list[0] != 0)
            {
                RunningProcess.responsetime = Time - 1;
            }
        }

        public void AddWaitingTime()
        {
            foreach (var i in Ready_Queue)
            {
                i.waiting_time++;
            }
        }

        public void UpdateIOProcesses()
        {
            if (Processes_In_I_O.Count == 0)
            { return; }

            foreach (var i in Processes_In_I_O.ToList())
            {
                int bursttime = SubtractTimeFromBurst(i.burst_list);
                if (bursttime == 0) //if i/0 is done move to ready queue and remove from i/0
                {
                    ContextSwitch = true; //since we are adding to ready queue 
                    Ready_Queue.Enqueue(i);
                    Processes_In_I_O.Remove(i);

                    //RE order queue if SJF
                    if(!IsFCFS) //if false then SJF is true
                    {
                        ReOrderQueueForSJF(); 
                    }

                    if (Processes_In_I_O.Count == 0)
                    {
                        return;
                    }
                }
            }
        }

        public void ReOrderQueueForSJF()
        {
            List<Process> ready_queue_list = Ready_Queue.OrderBy(x=>x.upcoming_cpuburst_time).ToList();
            Ready_Queue.Clear(); 
            foreach(var process in ready_queue_list)
            {
                Ready_Queue.Enqueue(process); 
            }
        }

        public void UpdateRunningProcess()
        {
            //check if anything is in cpu
            if (RunningProcess == null)
            {
                //cpu not in use
                CPUNotInUse++; 
                return;
            }
            int burstTime = SubtractTimeFromBurst(RunningProcess.burst_list);
     

            if (burstTime == 0) //cpu burst is done
            {
                ContextSwitch = true; //since we are removing
                // check if last cpu burst
                if (BurstsAllFinished(RunningProcess.burst_list))
                {
                    RunningProcess.turnaroundtime = Time;
                    Processesfinished.Add(RunningProcess);
                }
                else
                {
                    //add process to i/o if not all burst are done
                    Processes_In_I_O.Add(RunningProcess);
                    //update next burst time (helps to quickly reorder list for sjf)
                    UpdateNextCpuBurstTime(); 

                }
                RunningProcess = null; // remove from cpu usage 
            }
            //check if burst is finished 
        }

        public void UpdateNextCpuBurstTime()
        {
            RunningProcess.upcoming_cpuburst_time = FindNextBurstTimeAfterI_O(RunningProcess.burst_list);
        }

        public bool BurstsAllFinished(int[] burstlist)
        {
            bool burstFinished = true;
            for (int i = 0; i < burstlist.Length; i++)
            {
                if (burstlist[i] != 0)
                {
                    burstFinished = false;
                }
            }
            return burstFinished;
        }
        public int FindNextBurstTimeAfterI_O(int[] burstlist)
        {
            bool foundBurst = false;
            int j = 0;

            while (!foundBurst)
            {
                for (int i = 0; i < burstlist.Length; i++)
                {
                    if (burstlist[i] != 0)
                    {
                        j = i;
                        foundBurst = true;
                        break;
                    }
                }
            }
            return burstlist[j+1]; // need to add one because next burst is i/o burst
        }

        public int SubtractTimeFromBurst(int[] burstlist)
        {
            bool foundBurst = false;
            int j = 0;

            while (!foundBurst)
            {
                for (int i = 0; i < burstlist.Length; i++)
                {
                    if (burstlist[i] != 0)
                    {
                        j = i;
                        foundBurst = true;
                        break;
                    }
                }
            }

            burstlist[j]--;
            return burstlist[j];
        }

        public int FindNextBurstTime(int[] burstlist)
        {
            bool foundBurst = false;
            int j = 0;

            while (!foundBurst)
            {
                for (int i = 0; i < burstlist.Length; i++)
                {
                    if (burstlist[i] != 0)
                    {
                        j = i;
                        foundBurst = true;
                        break;
                    }
                }
            }
            return burstlist[j]; 
        }
        public void PrintProcessStats(IEnumerable<Process> processList)
        {
            int twTotal = 0, ttrTotal = 0, trTotal = 0;
            double twAve, ttrAve, trAve, cpuUsage;

            string format = "{0,-15} {1,-10} {2,10} {3,20}" + Environment.NewLine;
            var stringBuilder = new StringBuilder().AppendFormat(format,"P", "Tw", "Ttr", "tr");

            foreach (var i in processList)
            {
                stringBuilder.AppendFormat(format,i.p_num, i.waiting_time, i.turnaroundtime, i.responsetime);
                twTotal += i.waiting_time;
                ttrTotal += i.turnaroundtime;
                trTotal += i.responsetime;
            }
            twAve = twTotal / 8.0;
            ttrAve = ttrTotal / 8.0;
            trAve = trTotal / 8.0;
            stringBuilder.AppendFormat(format, "Ave", twAve, ttrAve, trAve);

            Console.WriteLine(stringBuilder.ToString());

            cpuUsage = (TimeAllProcessesFinished- CPUNotInUse) / (double)TimeAllProcessesFinished ;
            Console.WriteLine("CPU utilization: " + cpuUsage.ToString("P"));
            Console.WriteLine("Total Time for All 8 Processes: " + TimeAllProcessesFinished);
        }
    } 
}
