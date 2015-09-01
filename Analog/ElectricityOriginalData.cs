using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Analog
{
    public class ElectricityOriginalData
    {
        public ElectricityOriginalData(XmlNode node, int ParentID)
        {
            this.ParentID = ParentID;
            this.Name = node.Attributes["Name"].Value;
            this.PID = Convert.ToInt32(node.Attributes["PID"].Value);
            this.MID = Convert.ToInt32(node.Attributes["MID"].Value);
            this.UA = Convert.ToDouble(node.Attributes["UA"].Value);
            this.UB = Convert.ToDouble(node.Attributes["UB"].Value);
            this.UC = Convert.ToDouble(node.Attributes["UC"].Value);
            this.UAB = Convert.ToDouble(node.Attributes["UAB"].Value);
            this.UBC = Convert.ToDouble(node.Attributes["UBC"].Value);
            this.UCA = Convert.ToDouble(node.Attributes["UCA"].Value);
            this.IA = Convert.ToDouble(node.Attributes["IA"].Value);
            this.IB = Convert.ToDouble(node.Attributes["IB"].Value);
            this.IC = Convert.ToDouble(node.Attributes["IC"].Value);
            this.PA = Convert.ToDouble(node.Attributes["PA"].Value);
            this.PB = Convert.ToDouble(node.Attributes["PB"].Value);
            this.PC = Convert.ToDouble(node.Attributes["PC"].Value);
            this.PS = Convert.ToDouble(node.Attributes["PS"].Value);
            this.QA = Convert.ToDouble(node.Attributes["QA"].Value);
            this.QB = Convert.ToDouble(node.Attributes["QB"].Value);
            this.QC = Convert.ToDouble(node.Attributes["QC"].Value);
            this.QS = Convert.ToDouble(node.Attributes["QS"].Value);
            this.SS = Convert.ToDouble(node.Attributes["SS"].Value);
            this.SA = Convert.ToDouble(node.Attributes["SA"].Value);
            this.SB = Convert.ToDouble(node.Attributes["SB"].Value);
            this.SC = Convert.ToDouble(node.Attributes["SC"].Value);
            this.PFA = Convert.ToDouble(node.Attributes["PFA"].Value);
            this.PFB = Convert.ToDouble(node.Attributes["PFB"].Value);
            this.PFC = Convert.ToDouble(node.Attributes["PFC"].Value);
            this.PFS = Convert.ToDouble(node.Attributes["PFS"].Value);
            this.FR = Convert.ToDouble(node.Attributes["FR"].Value);
            this.WPP = Convert.ToDouble(node.Attributes["WPP"].Value);
            this.WPN = Convert.ToDouble(node.Attributes["WPN"].Value);
            this.WQN = Convert.ToDouble(node.Attributes["WQN"].Value);
            this.WQP = Convert.ToDouble(node.Attributes["WQP"].Value);
            this.IStatus = (node.Attributes["IStatus"].Value);
            this.EventTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            this.WPPIncre = 0.1;
            this.WPNIncre = 0.1;
            this.WQNIncre = 0.1;
            this.WQPIncre = 0.1;
        }


        public void ClearData()
        {
            this.IA = 0;
            this.IB = 0;
            this.IC = 0;
            this.PA = 0;
            this.PB = 0;
            this.PC = 0;
            this.PS = 0;
            this.QA = 0;
            this.QB = 0;
            this.QC = 0;
            this.QS = 0;
            this.SS = 0;
            this.SA = 0;
            this.SB = 0;
            this.SC = 0;


            this.WPP = 0;
            this.WPN = 0;
            this.WQN = 0;
            this.WQP = 0;
        }

        public override string ToString()
        {
            return Name;
        }

        public int ParentID { get; set; }

        public string Name { get; set; }
        public int NodeID { get; set; }

        /// <summary>
        /// //项目编号
        /// </summary>
        public int PID { get; set; }

        /// <summary>
        /// 表编号
        /// </summary>
        public int MID { get; set; }

        /// <summary>
        /// A相电压
        /// </summary>
        public Double UA { get; set; }

        /// <summary>
        /// B相电压
        /// </summary>
        public Double UB { get; set; }

        /// <summary>
        /// C相电压
        /// </summary>
        public Double UC { get; set; }

        /// <summary>
        /// A-B线电压
        /// </summary>
        public Double UAB { get; set; }

        /// <summary>
        /// B-C线电压
        /// </summary>
        public Double UBC { get; set; }

        /// <summary>
        /// C-A线电压
        /// </summary>
        public Double UCA { get; set; }

        /// <summary>
        /// A相电流
        /// </summary>
        public Double IA { get; set; }

        /// <summary>
        /// B相电流
        /// </summary>
        public Double IB { get; set; }

        /// <summary>
        /// C相电流
        /// </summary>
        public Double IC { get; set; }

        /// <summary>
        /// A相有功功率
        /// </summary>
        public Double PA { get; set; }

        /// <summary>
        /// B相有功功率
        /// </summary>
        public Double PB { get; set; }

        /// <summary>
        /// C相有功功率
        /// </summary>
        public Double PC { get; set; }

        /// <summary>
        /// 合相有功功率
        /// </summary>
        public Double PS { get; set; }

        /// <summary>
        /// A相无功功率
        /// </summary>
        public Double QA { get; set; }

        /// <summary>
        /// B相无功功率
        /// </summary>
        public Double QB { get; set; }

        /// <summary>
        /// C相无功功率
        /// </summary>
        public Double QC { get; set; }

        /// <summary>
        /// 合相无功功率
        /// </summary>
        public Double QS { get; set; }

        /// <summary>
        /// A相视在功率
        /// </summary>
        public Double SA { get; set; }

        /// <summary>
        /// B相视在功率
        /// </summary>
        public Double SB { get; set; }

        /// <summary>
        /// C相视在功率
        /// </summary>
        public Double SC { get; set; }

        /// <summary>
        /// 合相视在功率
        /// </summary>
        public Double SS { get; set; }

        /// <summary>
        /// A相功率因数
        /// </summary>
        public Double PFA { get; set; }

        /// <summary>
        /// B相功率因数
        /// </summary>
        public Double PFB { get; set; }

        /// <summary>
        /// C相功率因数
        /// </summary>
        public Double PFC { get; set; }

        /// <summary>
        /// 合相功率因数
        /// </summary>
        public Double PFS { get; set; }

        /// <summary>   
        /// 电网频率
        /// </summary>
        public Double FR { get; set; }

        /// <summary>
        /// 正向有功电能
        /// </summary>
        public Double WPP { get; set; }

        /// <summary>
        /// WPP增量，用于测试模拟数据
        /// </summary>
        public Double WPPIncre { get; set; }

        /// <summary>
        /// 负向有功电能
        /// </summary>
        public Double WPN { get; set; }

        /// <summary>
        /// WPN增量，用于测试模拟数据
        /// </summary>
        public Double WPNIncre { get; set; }

        /// <summary>
        /// 正向无功电能
        /// </summary>
        public Double WQP { get; set; }

        /// <summary>
        /// WQP增量，用于测试模拟数据
        /// </summary>
        public Double WQPIncre { get; set; }

        /// <summary>
        /// 负向无功电能
        /// </summary>
        public Double WQN { get; set; }

        /// <summary>
        /// WQN增量，用于测试模拟数据
        /// </summary>
        public Double WQNIncre { get; set; }

        /// <summary>
        /// 开关量输入状态
        /// </summary>
        public string IStatus { get; set; }

        /// <summary>
        /// 开关量输出状态
        /// </summary>
        public int OStatus { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public string EventTime { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceivedTime { get; set; }

        public bool HasChildren { get; set; }
    }
}