using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class FileWritterOptions :IDisposable
    {
        [DataMember]
        public int RowIndex { get; set; }

        [DataMember]
        public int VehicleId { get; set; }

        public MemoryStream ms { get; set; }

        public FileWritterOptions(int rowIndex, int vehicleId, MemoryStream ms)
        {
            RowIndex = rowIndex;
            VehicleId = vehicleId;
            this.ms = ms;
        }

        public void Dispose()
        {
            if (ms == null) return;
            ms.Dispose();
            ms = null;
        }
    }
}
