using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public  class SampleListener
    {
        public void OnSampleEvent(object sender, SampleEventArgs e)
        {
            Console.WriteLine($"Event for vehicle {e.VehicleId}, row {e.RowIndex}: {e.Message}");
            Logger.LogEvent(e.Message);
        }
    }
}
