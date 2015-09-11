using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModule
{
    public class MessageBus
    {
        private MessageBus() { Inspector = null; }
        private static MessageBus instance = new MessageBus();
        public static MessageBus Instance { get { return instance; } }

        public static Action<object> Inspector {get;set;}

        public void PublishResult(SimpleCalculationResult result)
        {
            if (Inspector != null)
            {
                Inspector(result);
            }
            Console.WriteLine("Result for {2} received at {0}. The Value was {1}.", DateTime.UtcNow.ToString(), result.Result, result.AnalysisName);
        }
    }
}
