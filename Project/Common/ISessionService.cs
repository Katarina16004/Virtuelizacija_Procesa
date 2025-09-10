using System;
using System.Collections.Generic;
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
        OperationResult StartSession(int VehicleId);
        
        [OperationContract]
        [FaultContract(typeof(CustomException))]
        OperationResult PushSample(FileWritterOptions fw);
        
        [OperationContract]
        OperationResult EndSession(int VehicleId);
    }
}
