using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SmartH2O_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISmartH2O_Service
    {
        // TODO: Add your service operations here
        [OperationContract]
        void PutParam(string xml);
        [OperationContract]
        void PutAlarm(string xml);

        [OperationContract]
        string GetParamHourly(string day);
        [OperationContract]
        string GetParamDaily(string startDay , string endDay);
        [OperationContract]
        string GetParamWeekly(string week);
        [OperationContract]
        string GetAlarmDaily(string day);
        [OperationContract]
        string GetAlarmInterval(string startDay, string endDay);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
