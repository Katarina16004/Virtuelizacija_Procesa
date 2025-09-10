using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace Common
{
    [DataContract]
    public enum ResultType
    {
        [EnumMember]
        Success,
        [EnumMember]
        Failed
    }
    [DataContract]
    public class OperationResult
    {
        public OperationResult()
        {
            ResultType = ResultType.Success;
        }
        [DataMember]
        public string ResultMessage { get; set; }
        [DataMember]
        public ResultType ResultType { get; set; }
    }
}
