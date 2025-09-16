using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SampleEventArgs:EventArgs
    {
        public int VehicleId { get; }
        public int RowIndex { get; }
        public string Message { get; }

        public SampleEventArgs(int vehicleId, int rowIndex, string message)
        {
            VehicleId = vehicleId;
            RowIndex = rowIndex;
            Message = message;
        }
    }
}
