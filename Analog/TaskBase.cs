using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Analog
{
    public class ConditionBase
    {
    }

    public class FilterCondition : ConditionBase
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int Mid { get; private set; }
        public int NodeId { get; set; }

        public FilterCondition(DateTime startTime, DateTime endTime, int mid)
        {
            StartTime = startTime;
            EndTime = endTime;
            NodeId = mid;
            Mid = NodeId;
        }
    }

    public class IPEndCondition : ConditionBase
    {
        public IPAddress ip;
        public int port;

        public IPEndCondition(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
    }

    public interface ITask
    {
        void Run();
    }

    public class TaskBase : ITask
    {
        public delegate void UpdateDataDelegate(string message);

        protected ConditionBase Condition { get; set; }

        protected String Name { get; set; }

        protected DateTime Time { get; set; }


        protected UpdateDataDelegate _updateDataData;


        public virtual void Run()
        {
        }

        public void UpdateDataByDelegate(string message)
        {
            _updateDataData(message);
        }

        public TaskBase(String name, ConditionBase condition, UpdateDataDelegate returnFuc)
        {
            this.Name = name;
            this._updateDataData = returnFuc;
            this.Condition = condition;
            this.Time = DateTime.Now;
        }
    }

    public class TaskSendMessageAsyn : TaskBase
    {
        private List<ElectricityOriginalData> data;
        private TcpClient tcpClient;

        public TaskSendMessageAsyn(String name, IPEndCondition condition, List<ElectricityOriginalData> data,
            UpdateDataDelegate returnFuc)
            : base(name, condition, returnFuc)
        {
            this.data = data;
            tcpClient = new TcpClient();
        }

        public override void Run()
        {
            try
            {
                tcpClient.Connect((Condition as IPEndCondition).ip, (Condition as IPEndCondition).port);
                UpdateDataByDelegate("\n已连接主机" + DateTime.Now.ToLongTimeString());
                string message = "";
                foreach (ElectricityOriginalData electricityOriginalData in data)
                {
                    message = FormatMessage(electricityOriginalData);
                    IAsyncResult result = SendMessageAsn(message);
                    while (!result.IsCompleted) ;
                    UpdateDataByDelegate("\r\n发送成功:" + message);
                }


                tcpClient.Close();
            }
            catch (Exception ee)
            {
                UpdateDataByDelegate("\n连接失败");
            }
            RecordData(data);
        }

        public void RecordData(List<ElectricityOriginalData> data)
        {
            NodesInfo node = new NodesInfo("NodesInfo.xml");
            foreach (ElectricityOriginalData electricityOriginalData in data)
            {
                node.RecordNodesInfo(electricityOriginalData);
            }
            node.Save();
        }

        public static string FormatMessage(ElectricityOriginalData data)
        {
            //[,T,1,12345,1,224,224,224,385,386,387,2,3,4,400,401,402,1200,400,400,400,1200,300,300,300,900,500,500,500,1500,50,1481,1482,1483,1484,FFFF,]
            // [,T,1,2,6,220,220,220,380,380,380,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,FFFF,]
            string message = "";
            message += "[,T,1,";
            message += data.PID + ",";
            message += data.MID + ",";
            message += data.UA + ",";
            message += data.UB + ",";
            message += data.UC + ",";
            message += data.UAB + ",";
            message += data.UBC + ",";
            message += data.UCA + ",";
            message += data.IA + ",";
            message += data.IB + ",";
            message += data.IC + ",";
            message += data.PA + ",";
            message += data.PB + ",";
            message += data.PC + ",";
            message += data.PS + ",";
            message += data.QA + ",";
            message += data.QB + ",";
            message += data.QC + ",";
            message += data.QS + ",";
            message += data.SA + ",";
            message += data.SB + ",";
            message += data.SC + ",";
            message += data.SS + ",";
            message += data.PFA + ",";
            message += data.PFB + ",";
            message += data.PFC + ",";
            message += data.PFS + ",";
            message += data.FR + ",";
            message += data.WPP + ",";
            message += data.WPN + ",";
            message += data.WQP + ",";
            message += data.WQN + ",";
            message += data.IStatus + ",";

            message += "]";
            return message;
        }

        private IAsyncResult SendMessageAsn(string message)
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] dataBytes = Encoding.ASCII.GetBytes(message);
            IAsyncResult result = stream.BeginWrite(dataBytes, 0, dataBytes.Length,
                new AsyncCallback(SendCallback), stream); //异步发送数据
            return result;
        }

        private static void SendCallback(IAsyncResult ar)
        {
        }
    }
}