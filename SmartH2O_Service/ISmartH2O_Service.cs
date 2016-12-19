using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SmartH2O_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceKnownType(typeof(AlarmInfo))]
    [ServiceContract()]
    public interface ISmartH2O_Service
    {
        // TODO: Add your service operations here
        [OperationContract]
        void PutParam(string xml);
        [OperationContract]
        void PutAlarm(string xml);

        [OperationContract]
        ParamVals[] GetParamHourly(DateTime dt);
        [OperationContract]
        ParamVals[] GetParamDaily(DateTime dtStart, DateTime dtEnd);
        [OperationContract]
        ParamVals[] GetParamWeekly(DateTime dt);
        [OperationContract]
        AlarmInfo[] GetAlarmDaily(DateTime dt);
        [OperationContract]
        AlarmInfo[] GetAlarmInterval(DateTime dtStart, DateTime dtEnd);
    }
    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class ParamVals
    {
        string type;
        int id;
        double[] min;
        double[] max;
        double[] average;

        [DataMember]
        public double[] Min
        {
            get { return min; }
            set { min = value; }
        }

        [DataMember]
        public double[] Max
        {
            get { return max; }
            set { max = value; }
        }
        [DataMember]
        public double[] Average
        {
            get { return average; }
            set { average = value; }
        }
        [DataMember]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }

    [DataContract]
    public class AlarmInfo
    {
        DateTime datetime;
        string message;
        int id;
        string type;
        float sensorVal;
        float triggerValue;
        string triggerType;
        float lowerBound;
        float upperBound;

        public AlarmInfo(DateTime datetime, int id, string type,float sensorVal, float triggerValue, string triggerType, float lowerBound, float upperBound, string message)
        {
            Datetime = datetime;
            Id = id;
            Type = type;
            SensorVal = sensorVal;
            TriggerValue = triggerValue;
            TriggerType = triggerType;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Message = message;
        }

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public float SensorVal
        {
            get { return sensorVal; }
            set { sensorVal = value; }
        }

        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        [DataMember]
        public float TriggerValue
        {
            get { return triggerValue; }
            set { triggerValue = value; }
        }

        [DataMember]
        public DateTime Datetime
        {
            get { return datetime; }
            set { datetime = value; }
        }

        [DataMember]
        public string TriggerType
        {
            get { return triggerType; }
            set { triggerType = value; }
        }

        [DataMember]
        public float LowerBound
        {
            get { return lowerBound; }
            set { lowerBound = value; }
        }

        [DataMember]
        public float UpperBound
        {
            get { return upperBound; }
            set { upperBound = value; }
        }

    }

}
