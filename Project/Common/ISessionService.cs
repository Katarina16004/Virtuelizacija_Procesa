using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface ISessionService
    {
        [OperationContract]
        OperationResult StartSession(int vehicleID);
        
        [OperationContract]
        [FaultContract(typeof(CustomException))]
        OperationResult PushSample(Sample sample);
        
        [OperationContract]
        void EndSession();
    }
}
