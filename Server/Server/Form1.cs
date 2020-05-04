using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public String TextToSend;
        public Form1()
        {
            InitializeComponent();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    txtIP.Text = address.ToString();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void btnStart_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "starting";
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(txtPort.Text));
                listener.Start();
                client = await listener.AcceptTcpClientAsync();
                STR = new StreamReader(client.GetStream());
                STW = new StreamWriter(client.GetStream());
                STW.AutoFlush = true;

                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
                lblStatus.Text = "connected";
                txtLogs.Text += "Server Connected\n";
            }
            catch(Exception ex)
            {
                lblStatus.Text = "Error";
                txtLogs.Text += "Server could not connect\n>>>"+ex.Message.ToString()+"\n";

            }

            
        }

        


        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(txtPort.Text));
                listener.Stop();
                client = listener.AcceptTcpClient();
                STR = new StreamReader(client.GetStream());
                STW = new StreamWriter(client.GetStream());
                STW.AutoFlush = false;

                backgroundWorker1.CancelAsync();
                backgroundWorker2.WorkerSupportsCancellation = false;
                txtLogs.Text += "Server disConnected\n";
            }
            catch(Exception ex)
            {
                lblStatus.Text = "Error";
                txtLogs.Text += "Server could not disconnect\n>>>"+ex.Message.ToString()+"\n";

            }
}

        private void timer1_Tick(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
            //backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            if (txtMSG.Text != "")
            {
                TextToSend = txtMSG.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            txtMSG.Text = "";
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                this.txtLogs.Invoke(new MethodInvoker(delegate ()
                {
                    txtLogs.AppendText("Me:" + TextToSend + "\n");
                    lblStatus.Text = "connected - sent";
                }));
            }
            backgroundWorker2.CancelAsync();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.txtLogs.Invoke(new MethodInvoker(delegate ()
                    {
                        txtLogs.AppendText("You:" + recieve + "\n");
                        lblStatus.Text = "connected";
                    }));
                    recieve = "";
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Error";
                    txtLogs.Text += "Server could not connect\n>>>" + ex.Message.ToString() + "\n";
                }
            }
        }
    }
}
