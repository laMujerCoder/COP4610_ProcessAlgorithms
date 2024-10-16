using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COP4610_ProcessAlgorithms
{
    public class Process
    {
        public int p_num;
        public int waiting_time = 0;
        public int upcoming_cpuburst_time = 0; 
        public int[] burst_list = new int[20];
        public int turnaroundtime;
        public int responsetime;
        public int? TqTimeLeft; //only used for queues with quantum time  
        public int ReadyQueueNumber; // only used for multilevel feedback loop

        public Process(int[] burstlist, int process_number)
        {
            p_num = process_number;
            burst_list = burstlist;
        }
    }
}
