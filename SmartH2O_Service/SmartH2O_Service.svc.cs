using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace SmartH2O_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SmartH2O_Service : ISmartH2O_Service
    {
        public string GetAlarmDaily(string day)
        {
            throw new NotImplementedException();
        }

        public string GetAlarmInterval(string startDay, string endDay)
        {
            throw new NotImplementedException();
        }

        public string GetParamDaily(string startDay, string endDay)
        {
            throw new NotImplementedException();
        }

        public string GetParamHourly(string day)
        {
            throw new NotImplementedException();
        }

        public string GetParamWeekly(string week)
        {
            throw new NotImplementedException();
        }

        public void PutAlarm(string xml)
        {
            string alarmDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xml";
            string paramDataXSD = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xsd";

            XmlDocument docAlarmData = new XmlDocument();
            docAlarmData.Load(alarmDataXML);

            XmlDocument docTrigger = new XmlDocument();
            docTrigger.LoadXml(xml);

            //check if there is a root
            XmlNode root = docAlarmData.SelectSingleNode("/alarms");
            if (root == null)
            {
                XmlElement rootEl = docAlarmData.CreateElement("alarms");
                docAlarmData.AppendChild(rootEl);
                root = docAlarmData.SelectSingleNode("/alarms");
            }

            XmlNode triggerEl = docAlarmData.ImportNode(docTrigger.SelectSingleNode("/alarmTrigger"), true);
            root.AppendChild(triggerEl);
            docAlarmData.Save(alarmDataXML);
        }

        public void PutParam(string xml)
        {
            string paramDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xml";
            string paramDataXSD = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xsd";

            XmlDocument docParamData = new XmlDocument();
            docParamData.Load(paramDataXML);

            XmlDocument docSensor = new XmlDocument();
            docSensor.LoadXml(xml);

            //check if there is a root
            XmlNode root = docParamData.SelectSingleNode("/sensors");
            if (root == null)
            {
                XmlElement rootEl = docParamData.CreateElement("sensors");
                docParamData.AppendChild(rootEl);
                root = docParamData.SelectSingleNode("/sensors");
            }

            XmlNode sensorElement = docParamData.ImportNode(docSensor.SelectSingleNode("/sensor"), true);
            root.AppendChild(sensorElement);
            docParamData.Save(paramDataXML);
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
