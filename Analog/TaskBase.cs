using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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


        public FilterCondition(DateTime startTime, DateTime endTime, int mid)
        {
            StartTime = startTime;
            EndTime = endTime;
            Mid = mid;
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


        protected UpdateDataDelegate _UpdateDataDelegate;


        public virtual void Run()
        {
        }

        public void UpdateDataByDelegate(string message)
        {
            _UpdateDataDelegate(message);
        }

        public TaskBase(String name, ConditionBase condition, UpdateDataDelegate returnFuc)
        {
            this.Name = name;
            this._UpdateDataDelegate = returnFuc;
            this.Condition = condition;
            this.Time = DateTime.Now;
        }
    }

    public class TaskSendMessageAsyn : TaskBase
    {
        private List<ElectricityOriginalData> data;
        private TcpClient tcpClient;

        public TaskSendMessageAsyn(String name, IPEndCondition condition, List<ElectricityOriginalData> data,
            UpdateDataDelegate returnFuc, TcpClient tcpClient)
            : base(name, condition, returnFuc)
        {
            this.data = data;
            this.tcpClient = tcpClient;
        }

        public override void Run()
        {
            try
            {
                if (!tcpClient.Connected)
                {
                    tcpClient = new TcpClient();tcpClient.Connect((Condition as IPEndCondition).ip, (Condition as IPEndCondition).port);
                }
                UpdateDataByDelegate("\n已连接主机" + DateTime.Now.ToLongTimeString());
                string message = "";
                int messageCount = 0;
                Stopwatch watch = new Stopwatch();
                watch.Start();
                foreach (ElectricityOriginalData electricityOriginalData in data)
                {
                    message = FormatMessage(electricityOriginalData);
                    messageCount += message.Length;
                    SendMessageAsn(message);
                    UpdateDataByDelegate("\r\n发送成功:" + message);
                }

                tcpClient.Close();
                watch.Stop();
            }
            catch (Exception ee)
            {
                UpdateDataByDelegate(ee.Message + "\n连接失败");
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
            string message = "";
            message += "[,T,1,";
            message += data.PID + ",";
            message += data.MID + ",";
            message += data.UA.ToString("0.00") + ",";
            message += data.UB.ToString("0.00") + ",";
            message += data.UC.ToString("0.00") + ",";
            message += data.UAB.ToString("0.00") + ",";
            message += data.UBC.ToString("0.00") + ",";
            message += data.UCA.ToString("0.00") + ",";
            message += data.IA.ToString("0.00") + ",";
            message += data.IB.ToString("0.00") + ",";
            message += data.IC.ToString("0.00") + ",";
            message += data.PA.ToString("0.00") + ",";
            message += data.PB.ToString("0.00") + ",";
            message += data.PC.ToString("0.00") + ",";
            message += data.PS.ToString("0.00") + ",";
            message += data.QA.ToString("0.00") + ",";
            message += data.QB.ToString("0.00") + ",";
            message += data.QC.ToString("0.00") + ",";
            message += data.QS.ToString("0.00") + ",";
            message += data.SA.ToString("0.00") + ",";
            message += data.SB.ToString("0.00") + ",";
            message += data.SC.ToString("0.00") + ",";
            message += data.SS.ToString("0.00") + ",";
            message += data.PFA + ",";
            message += data.PFB + ",";
            message += data.PFC + ",";
            message += data.PFS + ",";
            message += data.FR + ",";
            message += data.WPP.ToString("0.00") + ",";
            message += data.WPN.ToString("0.00") + ",";
            message += data.WQP.ToString("0.00") + ",";
            message += data.WQN.ToString("0.00") + ",";
            message += data.IStatus + ",";

            message += "]";
            return message;
        }

        private void SendMessageAsn(string message)
        {
            NetworkStream stream = tcpClient.GetStream();

            byte[] dataBytes = Encoding.ASCII.GetBytes(message);
            IAsyncResult result = stream.BeginWrite(dataBytes, 0, dataBytes.Length,
                new AsyncCallback(SendCallback), stream); //异步发送数据
//            stream.Write(dataBytes, 0, dataBytes.Length);
            // Thread.Sleep(100);
//            return result;
        }

        private static void SendCallback(IAsyncResult ar)
        {
        }
    }
}