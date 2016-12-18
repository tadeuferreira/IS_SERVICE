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
        public List<AlarmInfo> GetAlarmDaily(string day)
        {
            string alarmDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xml";
            XmlDocument docAlarmData = new XmlDocument();
            docAlarmData.Load(alarmDataXML);

            DateTime dt = Convert.ToDateTime(day);
          List<AlarmInfo> alarms = new List<AlarmInfo>();

            XmlNodeList allAlarms = docAlarmData.SelectNodes("/alarms/alarmTrigger/message");

            for (int i = 0; i < allAlarms.Count; i++)
            {
                XmlNode node = allAlarms.Item(i);
                DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                DateTime dtaux = dts.Date;
                if (DateTime.Compare(dtaux, dt) == 0)
                {
                    alarms.Add(new AlarmInfo(node.Attributes["alarmType"].Value, 
                          node.Attributes["sensorType"].Value, 
                          int.Parse(node.Attributes["sensorid"].Value),
                          node.Attributes["date"].Value, 
                          Double.Parse(node.Attributes["val"].Value),
                          node.Attributes["alarmType"].Value != "ALARM_INTERVAL" ? Double.Parse(node.Attributes["triggerValue"].Value) : 0,
                          node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? Double.Parse(node.Attributes["lowerTriggerValue"].Value) : 0,
                          node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? Double.Parse(node.Attributes["higherTriggerValue"].Value) : 0, 
                          node.InnerText));
                }
            }
            return alarms;
        }

        public List<AlarmInfo> GetAlarmInterval(string startDay, string endDay)
        {
            string alarmDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xml";
            XmlDocument docAlarmData = new XmlDocument();
            docAlarmData.Load(alarmDataXML);

            DateTime dtStart = Convert.ToDateTime(startDay);
            dtStart = dtStart.Date;
            DateTime dtEnd = Convert.ToDateTime(endDay);
            dtEnd = dtEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
            int totalDays = (int)Math.Truncate((dtEnd - dtStart).TotalDays) + 1;

            List<AlarmInfo> alarms = new List<AlarmInfo>();
            XmlNodeList allAlarms = docAlarmData.SelectNodes("/alarms/alarmTrigger/message");

            for (int i = 0; i < alarms.Count; i++)
            {
                XmlNode node = allAlarms.Item(i);
                DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                if (DateTime.Compare(dts, dtStart) >= 0 && DateTime.Compare(dts, dtEnd) <= 0)
                {
                    alarms.Add(new AlarmInfo(node.Attributes["alarmType"].Value,
                         node.Attributes["sensorType"].Value,
                         int.Parse(node.Attributes["sensorid"].Value),
                         node.Attributes["date"].Value,
                         Double.Parse(node.Attributes["val"].Value),
                         node.Attributes["alarmType"].Value != "ALARM_INTERVAL" ? Double.Parse(node.Attributes["triggerValue"].Value) : 0,
                         node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? Double.Parse(node.Attributes["lowerTriggerValue"].Value) : 0,
                         node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? Double.Parse(node.Attributes["higherTriggerValue"].Value) : 0,
                         node.InnerText));
                }
            }

            return alarms;
        }

        public ParamVals GetParamDaily(string startDay, string endDay)
        {
            string paramDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xml";
            XmlDocument docParamData = new XmlDocument();
            docParamData.Load(paramDataXML);

            DateTime dtStart = Convert.ToDateTime(startDay);
            dtStart = dtStart.Date;
            DateTime dtEnd = Convert.ToDateTime(endDay);
            dtEnd = dtEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
            int totalDays = (int)Math.Truncate((dtEnd - dtStart).TotalDays) +1;
            if ((dtEnd - dtStart).TotalDays <= 0)
                return null;
 
            XmlNodeList allSensors = docParamData.SelectNodes("/sensors/sensor/data");
            LinkedList<XmlNode> daySensors = new LinkedList<XmlNode>();


            for (int i = 0; i < allSensors.Count; i++)
            {
                DateTime dts = Convert.ToDateTime(allSensors.Item(i).Attributes["date"].Value);
                if (DateTime.Compare(dts, dtStart) >= 0 && DateTime.Compare(dts, dtEnd) <= 0)
                {
                    daySensors.AddLast(allSensors.Item(i));
                }
            }

            DateTime dt = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);

            double[] max = new double[totalDays];
            double[] min = new double[totalDays];
            for (int i = 0; i < totalDays; i++)
            {
                min[i] = 1337.1337;
            }
            double[] average = new double[totalDays];
            for (int i = 0; i < totalDays; i++)
            {
                LinkedList<XmlNode> aux = new LinkedList<XmlNode>();

                foreach (XmlNode node in daySensors)
                {
                    DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                    if (dts.Day == dt.Day && dts.Month == dt.Month && dts.Year == dt.Year)
                    {
                        aux.AddLast(node);
                        double val = Double.Parse(node.Attributes["val"].Value);
                        if (max[i] < val)
                        {
                            max[i] = val;
                        }
                        if (min[i] > val)
                        {
                            min[i] = val;
                        }
                        average[i] += val;
                    }
                }
                if (average[i] != 0)
                    average[i] = average[i] / aux.Count;
                foreach (XmlNode node in aux)
                {
                    daySensors.Remove(node);
                }
                dt = dt.AddDays(1);
            }
            for (int i = 0; i < totalDays; i++)
            {
                if (min[i] == 1337.1337)
                    min[i] = 0;
            }
            return new ParamVals(min, max, average);
        }

        public ParamVals GetParamHourly(string day)
        {

            string paramDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xml";
            XmlDocument docParamData = new XmlDocument();
            docParamData.Load(paramDataXML);
            DateTime dt = Convert.ToDateTime(day);

            XmlNodeList allSensors = docParamData.SelectNodes("/sensors/sensor/data");
            LinkedList<XmlNode> daySensors = new LinkedList<XmlNode>();


            for(int i = 0; i<allSensors.Count; i++)
            {
                DateTime dts = Convert.ToDateTime(allSensors.Item(i).Attributes["date"].Value);
                if(dt.Day == dts.Day && dt.Month == dts.Month && dt.Year == dts.Year)
                {
                    daySensors.AddLast(allSensors.Item(i));
                }
            }

            dt = dt.Date.AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
            double[] max = new double[24];
            double[] min = new double[24];
            for (int i = 0; i < 24; i++)
            {
                min[i] = 1337.1337;
            }
            double[] average = new double[24];
            for (int i = 0; i < 24; i++)
            {
                LinkedList<XmlNode> aux = new LinkedList<XmlNode>();
                
                foreach (XmlNode node in daySensors)
                {
                    DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                    if (dts.Hour == dt.Hour && dts.Minute <= dt.Minute && dts.Second <= dt.Second)
                    {
                        aux.AddLast(node);
                        double val = Double.Parse(node.Attributes["val"].Value);
                        if (max[i] < val)
                        {
                            max[i] = val;
                        }
                        if (min[i] > val)
                        {
                            min[i] = val;
                        }
                        average[i] += val;
                    }
                }
                if(average[i] != 0)
                average[i] = average[i] / aux.Count;
                foreach(XmlNode node in aux)
                {
                    daySensors.Remove(node);
                }
                dt = dt.AddHours(1);
            }
            for (int i = 0; i < 24; i++)
            {
                if (min[i] == 1337.1337)
                    min[i] = 0;              
            }
            return new ParamVals(min,max,average);
        }

        public ParamVals GetParamWeekly(string day)
        {
            string paramDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xml";
            XmlDocument docParamData = new XmlDocument();
            docParamData.Load(paramDataXML);

            DateTime dtEnd = Convert.ToDateTime(day);
            DateTime dtStart = dtEnd.AddDays(-6);
            dtStart = dtStart.Date; // meter 0:0:0
            dtEnd = dtEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
           
            int totalDays = (int)Math.Truncate((dtEnd - dtStart).TotalDays) + 1;
            if ((dtEnd - dtStart).TotalDays <= 0)
                return null;

            XmlNodeList allSensors = docParamData.SelectNodes("/sensors/sensor/data");
            LinkedList<XmlNode> daySensors = new LinkedList<XmlNode>();


            for (int i = 0; i < allSensors.Count; i++)
            {
                DateTime dts = Convert.ToDateTime(allSensors.Item(i).Attributes["date"].Value);
                if (DateTime.Compare(dts, dtStart) >= 0 && DateTime.Compare(dts, dtEnd) <= 0)
                {
                    daySensors.AddLast(allSensors.Item(i));
                }
            }

            DateTime dt = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);

            double[] max = new double[totalDays];
            double[] min = new double[totalDays];
            for (int i = 0; i < totalDays; i++)
            {
                min[i] = 1337.1337;
            }
            double[] average = new double[totalDays];
            for (int i = 0; i < totalDays; i++)
            {
                LinkedList<XmlNode> aux = new LinkedList<XmlNode>();

                foreach (XmlNode node in daySensors)
                {
                    DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                    if (dts.Day == dt.Day && dts.Month == dt.Month && dts.Year == dt.Year)
                    {
                        aux.AddLast(node);
                        double val = Double.Parse(node.Attributes["val"].Value);
                        if (max[i] < val)
                        {
                            max[i] = val;
                        }
                        if (min[i] > val)
                        {
                            min[i] = val;
                        }
                        average[i] += val;
                    }
                }
                if (average[i] != 0)
                    average[i] = average[i] / aux.Count;
                foreach (XmlNode node in aux)
                {
                    daySensors.Remove(node);
                }
                dt = dt.AddDays(1);
            }
            for (int i = 0; i < totalDays; i++)
            {
                if (min[i] == 1337.1337)
                    min[i] = 0;
            }
            return new ParamVals(min, max, average);
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
    }
}
