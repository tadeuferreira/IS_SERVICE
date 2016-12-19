
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace SmartH2O_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SmartH2O_Service : ISmartH2O_Service
    {
        public AlarmInfo[] GetAlarmDaily(DateTime dt)
        {
            string alarmDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xml";
            XmlDocument docAlarmData = new XmlDocument();
            docAlarmData.Load(alarmDataXML);
            List<AlarmInfo> alarms = new List<AlarmInfo>();
            dt = dt.Date;

            XmlNodeList allAlarms = docAlarmData.SelectNodes("/alarms/alarmTrigger/message");

            for (int i = 0; i < allAlarms.Count; i++)
            {
                XmlNode node = allAlarms.Item(i);
                DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                DateTime dtaux = dts.Date;
                if (DateTime.Compare(dtaux, dt) == 0)
                {
                    try
                    {
                        int id = int.Parse(node.Attributes["sensorid"].Value);
                        float sensorVal = float.Parse(node.Attributes["val"].Value);
                        string sType = node.Attributes["sensorType"].Value;
                        float triggerVal = node.Attributes["alarmType"].Value != "ALARM_INTERVAL" ? float.Parse(node.Attributes["triggerValue"].Value) : 0;
                        string aType = node.Attributes["alarmType"].Value;
                        float lower = node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? float.Parse(node.Attributes["lowerTriggerValue"].Value) : 0;
                        float higher = node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? float.Parse(node.Attributes["higherTriggerValue"].Value) : 0;
                        string m = node.InnerText;
                        alarms.Add(new AlarmInfo(dts, id, sType, sensorVal, triggerVal, aType, lower, higher, m));
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                                  
                }
            }
            return alarms.ToArray<AlarmInfo>();
        }

        public AlarmInfo[] GetAlarmInterval(DateTime dtStart, DateTime dtEnd)
        {
            if (DateTime.Compare(dtStart, dtEnd) > 0)
                return null;
            string alarmDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xml";
            XmlDocument docAlarmData = new XmlDocument();
            docAlarmData.Load(alarmDataXML);
            dtStart = dtStart.Date;
            dtEnd = dtEnd.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
            int totalDays = (int)Math.Truncate((dtEnd - dtStart).TotalDays) + 1;

            List<AlarmInfo> alarms = new List<AlarmInfo>();
            XmlNodeList allAlarms = docAlarmData.SelectNodes("/alarms/alarmTrigger/message");

            for (int i = 0; i < allAlarms.Count; i++)
            {
                XmlNode node = allAlarms.Item(i);
                DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                if (DateTime.Compare(dts, dtStart) >= 0 && DateTime.Compare(dts, dtEnd) <= 0)
                {
                    try
                    {
                        int id = int.Parse(node.Attributes["sensorid"].Value);
                        float sensorVal = float.Parse(node.Attributes["val"].Value);
                        string sType = node.Attributes["sensorType"].Value;
                        float triggerVal = node.Attributes["alarmType"].Value != "ALARM_INTERVAL" ? float.Parse(node.Attributes["triggerValue"].Value) : 0;
                        string aType = node.Attributes["alarmType"].Value;
                        float lower = node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? float.Parse(node.Attributes["lowerTriggerValue"].Value) : 0;
                        float higher = node.Attributes["alarmType"].Value == "ALARM_INTERVAL" ? float.Parse(node.Attributes["higherTriggerValue"].Value) : 0;
                        string m = node.InnerText;
                        alarms.Add(new AlarmInfo(dts, id, sType, sensorVal,triggerVal, aType, lower, higher, m));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }

            return alarms.ToArray();
        }

        public ParamVals[] GetParamDaily(DateTime dtStart, DateTime dtEnd)
        {
            if (DateTime.Compare(dtStart, dtEnd) > 0)
                return null;
            string paramDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xml";
            XmlDocument docParamData = new XmlDocument();
            docParamData.Load(paramDataXML);

            dtStart = dtStart.Date;
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
            ParamVals[] paramsvals = new ParamVals[3];
            for (int i = 0; i < 3; i++)
            {
                ParamVals p = new ParamVals();
                p.Id = i + 1;
                p.Max = new double[totalDays];
                p.Min = new double[totalDays];
                for (int j = 0; j < totalDays; j++)
                    p.Min[j] = 1337.1337;
                p.Average = new double[totalDays];
                paramsvals[i] = p;
            }

            int[][] count = new int[3][];
            for (int i = 0; i < 3; i++)
            {
                count[i] = new int[totalDays];
            }

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
                        int id = int.Parse(node.Attributes["id"].Value);
                        string type = node.Attributes["type"].Value;
                        ParamVals p = paramsvals[id - 1];
                        p.Type = type;
                        p.Id = id;

                        if (p.Max[i] < val)
                        {
                            p.Max[i] = val;
                        }
                        if (p.Min[i] > val)
                        {
                            p.Min[i] = val;
                        }
                        p.Average[i] += val;
                        count[id - 1][i]++;
                    }
                }

                for (int j = 0; j < 3; j++)
                {
                    ParamVals p = paramsvals[j];
                    if (p.Average[i] != 0)
                        p.Average[i] = p.Average[i] / count[j][i];
                    if (p.Min[i] == 1337.1337)
                        p.Min[i] = 0;

                }
                foreach (XmlNode node in aux)
                {
                    daySensors.Remove(node);
                }
                dt = dt.AddDays(1);
            }

            return paramsvals;
        }

        public ParamVals[] GetParamHourly(DateTime dt)
        {

            string paramDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xml";
            XmlDocument docParamData = new XmlDocument();
            docParamData.Load(paramDataXML);

            XmlNodeList allSensors = docParamData.SelectNodes("/sensors/sensor/data");
            LinkedList<XmlNode> daySensors = new LinkedList<XmlNode>();


            for (int i = 0; i < allSensors.Count; i++)
            {
                DateTime dts = Convert.ToDateTime(allSensors.Item(i).Attributes["date"].Value);
                if (dt.Day == dts.Day && dt.Month == dts.Month && dt.Year == dts.Year)
                {
                    daySensors.AddLast(allSensors.Item(i));
                }
            }

            dt = dt.Date.AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

            ParamVals[] paramsvals = new ParamVals[3];
            for (int i = 0; i < 3; i++)
            {
                ParamVals p = new ParamVals();
                p.Id = i + 1;
                p.Max = new double[24];
                p.Min = new double[24];
                for (int j = 0; j < 24; j++)
                    p.Min[j] = 1337.1337;
                p.Average = new double[24];
                paramsvals[i] = p;
            }
            int[][] count = new int[3][];
            for (int i = 0; i < 3; i++)
            {
                count[i] = new int[24];
            }

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
                        int id = int.Parse(node.Attributes["id"].Value);
                        string type = node.Attributes["type"].Value;
                        ParamVals p = paramsvals[id - 1];
                        p.Type = type;
                        p.Id = id;

                        if (p.Max[i] < val)
                        {
                            p.Max[i] = val;
                        }
                        if (p.Min[i] > val)
                        {
                            p.Min[i] = val;
                        }
                        p.Average[i] += val;
                        count[id - 1][i]++;
                    }
                }

                for (int j = 0; j < 3; j++)
                {
                    ParamVals p = paramsvals[j];
                    if (p.Average[i] != 0)
                        p.Average[i] = p.Average[i] / count[j][i];
                    if (p.Min[i] == 1337.1337)
                        p.Min[i] = 0;

                }
                foreach (XmlNode node in aux)
                {
                    daySensors.Remove(node);
                }
                dt = dt.AddHours(1);
            }

            return paramsvals;
        }

        public ParamVals[] GetParamWeekly(DateTime dt)
        {
            string paramDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\param-data.xml";
            XmlDocument docParamData = new XmlDocument();
            docParamData.Load(paramDataXML);

            DateTime dtEnd = dt.Date;
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

            dt = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
            ParamVals[] paramsvals = new ParamVals[3];
            for (int i = 0; i < 3; i++)
            {
                ParamVals p = new ParamVals();
                p.Id = i + 1;
                p.Max = new double[7];
                p.Min = new double[7];
                for (int j = 0; j < 7; j++)
                    p.Min[j] = 1337.1337;
                p.Average = new double[7];
                paramsvals[i] = p;
            }
            int[][] count = new int[3][];
            for (int i = 0; i < 3; i++)
            {
                count[i] = new int[7];
            }

            for (int i = 0; i < 7; i++)
            {
                LinkedList<XmlNode> aux = new LinkedList<XmlNode>();
                foreach (XmlNode node in daySensors)
                {
                    DateTime dts = Convert.ToDateTime(node.Attributes["date"].Value);
                    if (dts.Day == dt.Day && dts.Month == dt.Month && dts.Year == dt.Year)
                    {
                        aux.AddLast(node);
                        double val = Double.Parse(node.Attributes["val"].Value);
                        int id = int.Parse(node.Attributes["id"].Value);
                        string type = node.Attributes["type"].Value;
                        ParamVals p = paramsvals[id - 1];
                        p.Type = type;
                        p.Id = id;

                        if (p.Max[i] < val)
                        {
                            p.Max[i] = val;
                        }
                        if (p.Min[i] > val)
                        {
                            p.Min[i] = val;
                        }
                        p.Average[i] += val;
                        count[id - 1][i]++;
                    }
                }

                for (int j = 0; j < 3; j++)
                {
                    ParamVals p = paramsvals[j];
                    if (p.Average[i] != 0)
                        p.Average[i] = p.Average[i] / count[j][i];
                    if (p.Min[i] == 1337.1337)
                        p.Min[i] = 0;

                }
                foreach (XmlNode node in aux)
                {
                    daySensors.Remove(node);
                }
                dt = dt.AddDays(1);
            }

            return paramsvals;
        }

        public void PutAlarm(string xml)
        {
            string alarmDataXML = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xml";
            string alarmDataXSD = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"App_Data\alarm-data.xsd";

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
