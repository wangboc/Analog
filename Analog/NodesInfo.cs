using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Analog
{
    internal class NodesInfo
    {
        private string FileName;
        private XmlDocument xmlFile;

        public List<ElectricityOriginalData> Nodes = new List<ElectricityOriginalData>();

        public NodesInfo(string fileName)
        {
            xmlFile = new XmlDocument();
            FileName = fileName;
            xmlFile.Load(fileName);
            XmlNode rootNode = xmlFile.SelectSingleNode("Node");
            Nodes.Add(GetNodesInfo(rootNode, 0));
        }

        public void Save()
        {
            xmlFile.Save(FileName);
        }

        private ElectricityOriginalData GetNodesInfo(XmlNode node, int ParentID)
        {
            if (node.ChildNodes.Count == 0)
            {
                ElectricityOriginalData leafData = new ElectricityOriginalData(node, ParentID);
                leafData.HasChildren = false;
                return leafData;
            }

            ElectricityOriginalData data = new ElectricityOriginalData(node, ParentID);
            data.HasChildren = true;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                Nodes.Add(GetNodesInfo(childNode, data.NodeID));
            }
            return data;
        }

        public void RecordNodesInfo(ElectricityOriginalData data)
        {
            XmlNodeList xlist = xmlFile.SelectNodes("//Node");
            foreach (XmlNode xn in xlist)
            {
                if (xn.Attributes["NodeID"].Value == data.NodeID.ToString())
                {
                    xn.Attributes["UA"].InnerText = data.UA.ToString();
                    xn.Attributes["UB"].InnerText = data.UB.ToString();
                    xn.Attributes["UC"].InnerText = data.UC.ToString();
                    xn.Attributes["UAB"].InnerText = data.UAB.ToString();
                    xn.Attributes["UBC"].InnerText = data.UBC.ToString();
                    xn.Attributes["UCA"].InnerText = data.UCA.ToString();
                    xn.Attributes["IA"].InnerText = data.IA.ToString();
                    xn.Attributes["IB"].InnerText = data.IB.ToString();
                    xn.Attributes["IC"].InnerText = data.IC.ToString();
                    xn.Attributes["PA"].InnerText = data.PA.ToString();
                    xn.Attributes["PB"].InnerText = data.PB.ToString();
                    xn.Attributes["PC"].InnerText = data.PC.ToString();
                    xn.Attributes["PS"].InnerText = data.PS.ToString();
                    xn.Attributes["QA"].InnerText = data.QA.ToString();
                    xn.Attributes["QB"].InnerText = data.QB.ToString();
                    xn.Attributes["QC"].InnerText = data.QC.ToString();
                    xn.Attributes["QS"].InnerText = data.QS.ToString();
                    xn.Attributes["SA"].InnerText = data.SA.ToString();
                    xn.Attributes["SB"].InnerText = data.SB.ToString();
                    xn.Attributes["SC"].InnerText = data.SC.ToString();
                    xn.Attributes["SS"].InnerText = data.SS.ToString();
                    xn.Attributes["PFA"].InnerText = data.PFA.ToString();
                    xn.Attributes["PFB"].InnerText = data.PFB.ToString();
                    xn.Attributes["PFC"].InnerText = data.PFC.ToString();
                    xn.Attributes["PFS"].InnerText = data.PFS.ToString();
                    xn.Attributes["FR"].InnerText = data.FR.ToString();
                    xn.Attributes["WPP"].InnerText = data.WPP.ToString();
                    xn.Attributes["WPN"].InnerText = data.WPN.ToString();
                    xn.Attributes["WQP"].InnerText = data.WQP.ToString();
                    xn.Attributes["WQN"].InnerText = data.WQN.ToString();
                }
            }
        }
    }
}