using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using Timer = System.Windows.Forms.Timer;

namespace Analog
{
    public partial class AnaloyForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Timer timer = new Timer();
        private List<ElectricityOriginalData> data;
        private ElectricityOriginalData selectedData;
        private List<TextBox> tBoxs = new List<TextBox>();

        public AnaloyForm()
        {
            InitializeComponent();InitNodesTree();
            InitTimer();
            InitTextBoxList();
        }

        public void InitTextBoxList()
        {
            tBoxs.Add(textBoxUA);
            tBoxs.Add(textBoxUB);
            tBoxs.Add(textBoxUC);
            tBoxs.Add(textBoxUAB);
            tBoxs.Add(textBoxUBC);
            tBoxs.Add(textBoxUCA);
            tBoxs.Add(textBoxIA);
            tBoxs.Add(textBoxIB);
            tBoxs.Add(textBoxIC);
            tBoxs.Add(textBoxPA);
            tBoxs.Add(textBoxPB);
            tBoxs.Add(textBoxPC);
            tBoxs.Add(textBoxPS);
            tBoxs.Add(textBoxQA);
            tBoxs.Add(textBoxQB);
            tBoxs.Add(textBoxQC);
            tBoxs.Add(textBoxQS);
            tBoxs.Add(textBoxSA);
            tBoxs.Add(textBoxSB);
            tBoxs.Add(textBoxSC);
            tBoxs.Add(textBoxSS);
            tBoxs.Add(textBoxPFA);
            tBoxs.Add(textBoxPFB);
            tBoxs.Add(textBoxPFC);
            tBoxs.Add(textBoxPFS);
            tBoxs.Add(textBoxFR);
            tBoxs.Add(textBoxWPP);
            tBoxs.Add(textBoxWPN);
            tBoxs.Add(textBoxWQN);
            tBoxs.Add(textBoxWQP);
            tBoxs.Add(textBoxIStatus);
        }


        private void InitNodesTree()
        {
            NodeTreeCtr.GetStateImage += TreeList_GetStateImage;
            NodesInfo nodes = new NodesInfo("NodesInfo.xml");
            data = nodes.Nodes;
            selectedData = data[0];
            TreeListColumn column = NodeTreeCtr.Columns.Add();
            column.Caption = "电表位置";
            column.FieldName = "Name";
            column.Visible = true;
            TreeListColumn columnHasChildren = NodeTreeCtr.Columns.Add();
            columnHasChildren.Caption = "是否叶子节点";
            columnHasChildren.FieldName = "HasChildren";
            columnHasChildren.Visible = false;
            TreeListColumn columnID = NodeTreeCtr.Columns.Add();
            columnID.Caption = "MID";
            columnID.FieldName = "MID";
            columnID.Visible = false;
            NodeTreeCtr.DataSource = nodes.Nodes;
            NodeTreeCtr.ParentFieldName = "ParentID";
            NodeTreeCtr.KeyFieldName = "MID";
            NodeTreeCtr.ExpandAll();
            NodeTreeCtr.FocusedNode = NodeTreeCtr.Nodes[0];
        }

        private void TreeList_GetStateImage(object sender, GetStateImageEventArgs e)
        {
            bool HasChildren = Convert.ToBoolean(e.Node.GetValue("HasChildren"));

            e.NodeImageIndex = HasChildren ? 2 : 1;
        }

        public void InitTimer()
        {
            timer.Interval = 100*60*10;
            timer.Tick += timer_Tick;
        }

        public ElectricityOriginalData Calculate(TreeListNode node, bool increase_or_not)
        {
            if (!node.HasChildren)
            {
                var result = data.Where(d => d.NodeID.ToString() == node.GetValue("NodeID").ToString()).ToList();
                ElectricityOriginalData childData = result[0] as ElectricityOriginalData;
                
                if (increase_or_not)
                {
                    childData.WPP += childData.WPPIncre;
                    childData.WPN += childData.WPNIncre;
                    childData.WQP += childData.WQPIncre;
                    childData.WQN += childData.WQNIncre;
                }
                return childData;
            }
            else
            {
                var result = data.Where(d => d.NodeID.ToString() == node.GetValue("NodeID").ToString()).ToList();
                ElectricityOriginalData childData = result[0] as ElectricityOriginalData;
                childData.ClearData();
                foreach (TreeListNode treeListNode in node.Nodes)
                {
                    ElectricityOriginalData temp = Calculate(treeListNode, increase_or_not);
                    childData.WPP += temp.WPP;
                    childData.WPN += temp.WPN;
                    childData.WQP += temp.WQP;
                    childData.WQN += temp.WQN;
                    childData.IA += temp.IA;
                    childData.IB += temp.IB;
                    childData.IC += temp.IC;
                    childData.PA += temp.PA;
                    childData.PB += temp.PB;
                    childData.PC += temp.PC;
                    childData.PS += temp.PS;
                    childData.QA += temp.QA;
                    childData.QB += temp.QB;
                    childData.QC += temp.QC;
                    childData.QS += temp.QS;
                    childData.SA += temp.SA;
                    childData.SB += temp.SB;
                    childData.SC += temp.SC;
                    childData.SS += temp.SS;
                }
                return childData;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            richTextBoxMessage.Text = "";
            Calculate(NodeTreeCtr.Nodes[0], true);
            ChangeTextBoxs(selectedData);
            TaskSendMessageAsyn task = new TaskSendMessageAsyn("异步发送数据",
                new IPEndCondition(IPAddress.Parse(IPCtr.EditValue.ToString()), int.Parse(PortCtr.EditValue.ToString())),
                data, UpdateMessageUI);
            TaskPool.AddTask(task, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateMessageUI(string message)
        {
            richTextBoxMessage.Text += "\r\n";
            richTextBoxMessage.Text += message;
        }


        private void StartBtn_Click(object sender, ItemClickEventArgs e)
        {
            if (StartBtn.Caption != "暂停")
            {
                StartBtn.Caption = "暂停";
                timer.Start();
                IPCtr.Enabled = false;
                PortCtr.Enabled = false;
                StartBtn.ImageIndex = 6;
            }
            else
            {
                StartBtn.Caption = "开始";
                timer.Stop();
                IPCtr.Enabled = true;
                PortCtr.Enabled = true;
                StartBtn.ImageIndex = 5;
            }
        }

        private void ClearBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            richTextBoxMessage.Text = "";
        }

        private void barCheckItem1_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            timer.Interval = 1000;
        }

        private void barCheckItem2_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            timer.Interval = 1000*60;
        }

        private void barCheckItem3_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            timer.Interval = 1000*60*10;
        }

        private void ribbon_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) //判断是否最小化
            {
                this.ShowInTaskbar = false;notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
               
                this.ShowInTaskbar = true; //显示在系统任务栏
                this.WindowState = FormWindowState.Maximized; //还原窗体
                //   notifyIcon1.Visible = false;  //托盘图标隐藏
            }
            else this.WindowState = FormWindowState.Minimized;
        }

        private void ChangeTextBoxs(ElectricityOriginalData data)
        {
            if (data == null) return;
            textBoxName.Text = data.Name;
            textBoxPID.Text = data.PID.ToString();
            textBoxMID.Text = data.MID.ToString();
            textBoxUA.Text = data.UA.ToString();
            textBoxUB.Text = data.UB.ToString();
            textBoxUC.Text = data.UC.ToString();
            textBoxUAB.Text = data.UAB.ToString();
            textBoxUBC.Text = data.UBC.ToString();
            textBoxUCA.Text = data.UCA.ToString();
            textBoxIA.Text = data.IA.ToString();
            textBoxIB.Text = data.IB.ToString();
            textBoxIC.Text = data.IC.ToString();
            textBoxPA.Text = data.PA.ToString();
            textBoxPB.Text = data.PB.ToString();
            textBoxPC.Text = data.PC.ToString();
            textBoxPS.Text = data.PS.ToString();
            textBoxQA.Text = data.QA.ToString();
            textBoxQB.Text = data.QB.ToString();
            textBoxQC.Text = data.QC.ToString();
            textBoxQS.Text = data.QS.ToString();
            textBoxSA.Text = data.SA.ToString();
            textBoxSB.Text = data.SB.ToString();
            textBoxSC.Text = data.SC.ToString();
            textBoxSS.Text = data.SS.ToString();
            textBoxPFA.Text = data.PFA.ToString();
            textBoxPFB.Text = data.PFB.ToString();
            textBoxPFC.Text = data.PFC.ToString();
            textBoxPFS.Text = data.PFS.ToString();
            textBoxFR.Text = data.FR.ToString();
            textBoxWPP.Text = data.WPP.ToString();
            textBoxWPN.Text = data.WPN.ToString();
            textBoxWQN.Text = data.WQN.ToString();
            textBoxWQP.Text = data.WQP.ToString();
            textBoxIStatus.Text = data.IStatus;
            StringFormatCtr.Text = "通信内容： " + TaskSendMessageAsyn.FormatMessage(data);
        }

        private void NodeTreeCtr_Click(object sender, EventArgs e)
        {
            int Nodeid = int.Parse(NodeTreeCtr.FocusedNode.GetValue("NodeID").ToString());
            var selectData =
                data.Where(d => d.NodeID == Nodeid).ToList();
            if (selectData == null) return;
            selectedData = selectData[0];
            ChangeTextBoxs(selectedData);
            if (selectedData.HasChildren)
            {
                foreach (TextBox tb in tBoxs)
                {
                    tb.ReadOnly = true;
                }
            }
            else
                foreach (TextBox tb in tBoxs)
                {
                    tb.ReadOnly = false;
                }
        }

        private void textBoxUA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUA.Text != "")
                selectedData.UA = double.Parse(textBoxUA.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxUB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUB.Text != "")
                selectedData.UB = double.Parse(textBoxUB.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxUC_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUC.Text != "")
                selectedData.UC = double.Parse(textBoxUC.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxUAB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUAB.Text != "")
                selectedData.UAB = double.Parse(textBoxUAB.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxUBC_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUBC.Text != "")
                selectedData.UBC = double.Parse(textBoxUBC.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxUCA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUCA.Text != "")
                selectedData.UCA = double.Parse(textBoxUCA.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxSA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSA.Text != "")
                selectedData.SA = double.Parse(textBoxSA.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxSB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSB.Text != "")
                selectedData.SB = double.Parse(textBoxSB.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxSC_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSC.Text != "")
                selectedData.SC = double.Parse(textBoxSC.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxSS_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSS.Text != "")
                selectedData.SS = double.Parse(textBoxSS.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPFA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPFA.Text != "")
                selectedData.PFA = double.Parse(textBoxPFA.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPFB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPFB.Text != "")
                selectedData.PFB = double.Parse(textBoxPFB.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPFC_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPFC.Text != "")
                selectedData.PFC = double.Parse(textBoxPFC.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPFS_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPFS.Text != "")
                selectedData.PFS = double.Parse(textBoxPFS.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxIA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxIA.Text != "")
                selectedData.IA = double.Parse(textBoxIA.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxIB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxIB.Text != "")
                selectedData.IB = double.Parse(textBoxIB.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxIC_TextChanged(object sender, EventArgs e)
        {
            if (textBoxIC.Text != "")
                selectedData.IC = double.Parse(textBoxIC.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPA.Text != "")
                selectedData.PA = double.Parse(textBoxPA.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPB.Text != "")
                selectedData.PB = double.Parse(textBoxPB.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPC_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPC.Text != "")
                selectedData.PC = double.Parse(textBoxPC.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxPS_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPS.Text != "")
                selectedData.PS = double.Parse(textBoxPS.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxQA_TextChanged(object sender, EventArgs e)
        {
            if (textBoxQA.Text != "")
                selectedData.QA = double.Parse(textBoxQA.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxQB_TextChanged(object sender, EventArgs e)
        {
            if (textBoxQB.Text != "")
                selectedData.QB = double.Parse(textBoxQB.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxQC_TextChanged(object sender, EventArgs e)
        {
            if (textBoxQC.Text != "")
                selectedData.QC = double.Parse(textBoxQC.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxQS_TextChanged(object sender, EventArgs e)
        {
            if (textBoxQS.Text != "")
                selectedData.QS = double.Parse(textBoxQS.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxFR_TextChanged(object sender, EventArgs e)
        {
            if (textBoxFR.Text != "")
                selectedData.FR = double.Parse(textBoxFR.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxWPP_TextChanged(object sender, EventArgs e)
        {
            if (textBoxWPP.Text != "")
                selectedData.WPP = double.Parse(textBoxWPP.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxWPN_TextChanged(object sender, EventArgs e)
        {
            if (textBoxWPN.Text != "")
                selectedData.WPN = double.Parse(textBoxWPN.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxWQP_TextChanged(object sender, EventArgs e)
        {
            if (textBoxWQP.Text != "")
                selectedData.WQP = double.Parse(textBoxWQP.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void textBoxWQN_TextChanged(object sender, EventArgs e)
        {
            if (textBoxWQN.Text != "")
                selectedData.WQN = double.Parse(textBoxWQN.Text);
            Calculate(NodeTreeCtr.Nodes[0], false);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            Calculate(NodeTreeCtr.Nodes[0], false);
            NodesInfo node = new NodesInfo("NodesInfo.xml");
            foreach (ElectricityOriginalData electricityOriginalData in data)
            {
                node.RecordNodesInfo(electricityOriginalData);
            }
            node.Save();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            selectedData.WPPIncre = Double.Parse(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            selectedData.WPNIncre = Double.Parse(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            selectedData.WQPIncre = Double.Parse(textBox3.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            selectedData.WQNIncre = Double.Parse(textBox4.Text);
        }
    }
}