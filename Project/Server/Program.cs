using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(SessionService));

            SampleListener listener = new SampleListener();

            SessionService.OnTransferStarted += listener.OnSampleEvent;
            SessionService.OnSampleReceived += listener.OnSampleEvent;
            SessionService.OnTransferCompleted += listener.OnSampleEvent;
            SessionService.OnWarningRaised += listener.OnSampleEvent;

            SessionService.VoltageSpike += listener.OnSampleEvent;
            SessionService.CurrentSpike += listener.OnSampleEvent;
            SessionService.PowerFactorWarning += listener.OnSampleEvent;

            Console.WriteLine("Welcome to Server!");
            host.Open();
            Console.ReadKey();
            host.Close();
            Logger.Dispose();
        }
    }
}
