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
        ParamVals GetParamHourly(DateTime dt);
        [OperationContract]
        ParamVals GetParamDaily(DateTime dtStart, DateTime dtEnd);
        [OperationContract]
        ParamVals GetParamWeekly(DateTime dt);
        [OperationContract]
        AlarmInfo[] GetAlarmDaily(DateTime dt);
        [OperationContract]
        AlarmInfo[] GetAlarmInterval(DateTime dtStart, DateTime dtEnd);
    }
    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class ParamVals
    {
        double[] min;
        double[] max;
        double[] average;

        public ParamVals(double[] min, double[] max, double[] average)
        {
            this.min = min;
            this.max = max;
            this.average = average;
        }
           

        [DataMember]
        public double[] Min
        {
            get { return min; }
        }
        [DataMember]
        public double[] Max
        {
            get { return max; }
        }
        [DataMember]
        public double[] Average
        {
            get { return average; }
        }
    }

    [DataContract]
    public class AlarmInfo
    {
        string alarmType;
        string sensorType;
        int sensorId;
        string date;
        double value;
        double triggerValue;
        double lowerTriggerValue;
        double higherTriggerValue;
        string message;
      
        public AlarmInfo( string alarmType, string sensorType, int sensorId, 
            string date, double value, double triggerValue, double lowerTriggerValue, double higherTriggerValue, string message)
        {
            this.alarmType = alarmType;
            this.sensorType = sensorType;
            this.sensorId = sensorId;
            this.date = date;
            this.value = value;
            this.triggerValue = triggerValue;
            this.lowerTriggerValue = lowerTriggerValue;
            this.higherTriggerValue = higherTriggerValue;
            this.message = message;
        }


        [DataMember]
        public string AlarmType
        {
            get { return alarmType; }
        }
        [DataMember]
        public string SensorType
        {
            get { return sensorType; }
        }
        [DataMember]
        public int SensorId
        {
            get { return sensorId; }
        }
        [DataMember]
        public string Date
        {
            get { return date; }
        }
        [DataMember]
        public double Value
        {
            get { return value; }
        }
        [DataMember]
        public double TriggerValue
        {
            get { return triggerValue; }
        }
        [DataMember]
        public double LowerTriggerValue
        {
            get { return lowerTriggerValue; }
        }
        [DataMember]
        public double HigherTriggerValue
        {
            get { return higherTriggerValue; }
        }
        [DataMember]
        public string Message
        {
            get { return message; }
        }

    }
}
