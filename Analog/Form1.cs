using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Analog
{
    public partial class Form1 : Form
    {
        private int i = 1;

        public Form1()
        {
            InitializeComponent();
            InitTimer();
            InitTextBoxs();
            comboBoxTimeTick.SelectedIndex = 3;
        }


        private void InitTextBoxs()
        {
            textBoxUA.Text = "224";
            textBoxUB.Text = "224";
            textBoxUC.Text = "224";
            textBoxUAB.Text = "385";
            textBoxUBC.Text = "386";
            textBoxUCA.Text = "387";
            textBoxIA.Text = "2";
            textBoxIB.Text = "3";
            textBoxIC.Text = "4";
            textBoxPA.Text = "400";
            textBoxPB.Text = "401";
            textBoxPC.Text = "402";
            textBoxPS.Text = "1200";
            textBoxQA.Text = "400";
            textBoxQB.Text = "400";
            textBoxQC.Text = "400";
            textBoxQS.Text = "1200";
            textBoxSA.Text = "300";
            textBoxSB.Text = "300";
            textBoxSC.Text = "300";
            textBoxSS.Text = "900";
            textBoxPFA.Text = "500";
            textBoxPFB.Text = "500";
            textBoxPFC.Text = "500";
            textBoxPFS.Text = "1500";
            textBoxFR.Text = "50";
            textBoxWPP.Text = "700";
            textBoxWPN.Text = "701";
            textBoxWQP.Text = "702";
            textBoxWQN.Text = "703";
            textBoxIStatus.Text = "FFFF";
//            textBoxDatetime.Text = DateTime.Now.ToLongTimeString();
        }

        public void InitTimer()
        {
            timer.Interval = 10*60*1000;
            timer.Tick += timer_Tick;
            timer.Start();

            timer1.Interval = 1000;
            timer1.Tick += timer_Tick1;
            timer1.Start();
        }

        private string getFormatString()
        {
            string message = "[,T,1,12345,1," +
                             textBoxUA.Text + "," +
                             textBoxUB.Text + "," +
                             textBoxUC.Text + "," +
                             textBoxUAB.Text + "," +
                             textBoxUBC.Text + "," +
                             textBoxUCA.Text + "," +
                             textBoxIA.Text + "," +
                             textBoxIB.Text + "," +
                             textBoxIC.Text + "," +
                             textBoxPA.Text + "," +
                             textBoxPB.Text + "," +
                             textBoxPC.Text + "," +
                             textBoxPS.Text + "," +
                             textBoxQA.Text + "," +
                             textBoxQB.Text + "," +
                             textBoxQC.Text + "," +
                             textBoxQS.Text + "," +
                             textBoxSA.Text + "," +
                             textBoxSB.Text + "," +
                             textBoxSC.Text + "," +
                             textBoxSS.Text + "," +
                             textBoxPFA.Text + "," +
                             textBoxPFB.Text + "," +
                             textBoxPFC.Text + "," +
                             textBoxPFS.Text + "," +
                             textBoxFR.Text + "," +
                             (Convert.ToInt32(textBoxWPP.Text) + i) + "," +
                             (Convert.ToInt32(textBoxWPN.Text) + i) + "," +
                             (Convert.ToInt32(textBoxWQP.Text) + i) + "," +
                             (Convert.ToInt32(textBoxWQN.Text) + i) + "," +
                             textBoxIStatus.Text + "," +
                             "]";
            return message;
        }

        private void timer_Tick1(object sender, EventArgs e)
        {
            richTextBoxStringFormat.Text = getFormatString();
            textBoxDatetime.Text = DateTime.Now.ToString();
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(new IPEndPoint(new IPAddress(new byte[] {121, 41, 109, 137}), 5000));
                while (!tcpClient.Connected) ;
                {
                    richTextBoxMessage.Text += "\n已连接主机" + DateTime.Now.ToLongTimeString();

                    i += 10;
                    NetworkStream stream = tcpClient.GetStream();
                    string message = getFormatString();
                    byte[] dataBytes = Encoding.ASCII.GetBytes(message);
                    IAsyncResult result = stream.BeginWrite(dataBytes, 0, dataBytes.Length,
                        new AsyncCallback(SendCallback), stream); //异步发送数据
                    while (!result.IsCompleted) ;
                    richTextBoxMessage.Text += "\n成功发送：" + message + "\n";
                }
            }
            catch (Exception ee)
            {
                richTextBoxMessage.Text += "\n连接失败";
            }
        }

        private Timer timer = new Timer();
        private Timer timer1 = new Timer();


        private static void SendCallback(IAsyncResult ar)
        {
        }

        private void comboBoxTimeTick_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxTimeTick.SelectedIndex)
            {
                case 0:
                    timer.Interval = 100;
                    break;
                case 1:
                    timer.Interval = 1000;
                    break;
                case 2:
                    timer.Interval = 1000*60;
                    break;
                case 3:
                    timer.Interval = 1000*60*10;
                    break;
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            richTextBoxMessage.Text = "";
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (buttonStop.Text != "暂停")
            {
                buttonStop.Text = "暂停";
                timer.Start();
            }
            else
            {
                buttonStop.Text = "继续";
                timer.Stop();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)  //判断是否最小化
            {
              //  this.ShowInTaskbar = false;  //不显示在系统任务栏
                notifyIcon1.Visible = true;  //托盘图标可见
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;  //显示在系统任务栏
                this.WindowState = FormWindowState.Normal;  //还原窗体
             //   notifyIcon1.Visible = false;  //托盘图标隐藏
            }
            else this.WindowState = FormWindowState.Minimized;
        }
    }
}