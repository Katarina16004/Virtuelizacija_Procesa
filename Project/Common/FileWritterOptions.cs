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
    public class FileWritterOptions
    {
        [DataMember]
        public int RowIndex { get; set; }

        [DataMember]
        public int VehicleId { get; set; }

        [DataMember]
        public string line { get; set; }

        public FileWritterOptions(int rowIndex, int vehicleId, string line)
        {
            RowIndex = rowIndex;
            VehicleId = vehicleId;
            this.line = line;
        }

    }
}
