using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COP4610_ProcessAlgorithms
{
    public class ReadyQueueModel
    {
        public Queue<Process> Ready_Queue = new Queue<Process>();
        public int? TQ; //quantum time  
    }
}
