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

namespace Client
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
        

        private void btnConnect_Click(object sender, EventArgs e)
        {

            client = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(txtIP.Text), int.Parse(txtPort.Text));

            try
            {
                client.Connect(IpEnd);

                if (client.Connected)
                {
                    txtLogs.AppendText("Connected to server" + "\n");
                    STW = new StreamWriter(client.GetStream());
                    STR = new StreamReader(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                    txtLogs.Text += "Client Connected\n";
                    lblStatus.Text = "connected";

                }
                else
                {
                    txtLogs.Text += "Client did not connect\n";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error";
                txtLogs.Text += "Client could not connect\n>>>" + ex.Message.ToString() + "\n";
            }
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
                    txtLogs.Text += "client could not connect\n>>>" + ex.Message.ToString() + "\n";
                }
            }
        }
    }
}
