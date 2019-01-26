using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_FirmwareUpgrade
{
    public partial class Form1 : Form
    {
        string InputData = String.Empty;

        private delegate void SetTextDeleg(string data);

        delegate void SetTextCallback(string text);
        public Form1()
        {
            InitializeComponent();
            serialPort.ReadTimeout = 1000;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            label2.TextChanged -= label2_TextChanged_1;
        }
        
        int second = 0;
        int minute = 0;
        private void LoadTime()
        {
            
            labTime.Text = minute.ToString() + " : " + second.ToString();
            second++;
            if (second == 59)
            {
                minute++;
                second = 0;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Load time 
            timerStatus.Enabled = true;
            timerStatus.Start();
           
        }
        private int n = 0;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(n==0)
            {
                serialPort.BaudRate = 115200;
                serialPort.PortName = "COM1";
                serialPort.Parity = Parity.None;
                serialPort.DataBits = 8;
                serialPort.DtrEnable = true;
                serialPort.StopBits = StopBits.One;
                serialPort.Open();
                lblStatus.Text = "Connect";
                lblStatus.ForeColor = Color.Blue;
                timerEnter.Enabled = true;
                timerEnter.Start();
            }
            

        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            serialPort.Close();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            InputData = serialPort.ReadExisting();
            if (InputData != String.Empty)
            {
                SetText(InputData);

            }
        }

        private void SetText(string text)
        {
            if (this.txtAllLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else this.txtAllLog.Text += text;
        }

        private void txtAllLog_TextChanged(object sender, EventArgs e)
        {
            txtAllLog.SelectionStart = txtAllLog.TextLength;
            txtAllLog.ScrollToCaret();
            string[] lines = txtAllLog.Lines;

            foreach (string line in lines)
            {
                txtLog.Text = line;
                label1.Text = line;
                label2.Text = line;
            }
        }

        private void timerStatus_Tick(object sender, EventArgs e)
        {
            LoadTime();
        }
        private int i = 0;
        private void timerEnter_Tick(object sender, EventArgs e)
        {
            if(i==0)
            {
                serialPort.WriteLine("");
                
            }
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            if (txtLog.Text.Trim() == "Please press Enter to activate this console")
            {
                serialPort.WriteLine("");
            }
            if (txtLog.Text.Trim() == "login:")
            {

                serialPort.WriteLine("ambit");
                serialPort.WriteLine("ambitdebug");
                serialPort.WriteLine("retsh foxconn168!");
                txtAllLog.Clear();
                timerEnter.Stop();
                Thread.Sleep(2000);
                string text = "prolinecmd serialnum display";
                serialPort.WriteLine("");
                Thread.Sleep(1000);
                serialPort.WriteLine(text);
                serialPort.WriteLine("");
                txtAllLog.Text = serialPort.ReadExisting();
                string[] lines = txtAllLog.Lines.ToArray();
                Thread.Sleep(1000);
                using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\Test_FirmwareUpgrade", "WriteLines1.txt")))
                {
                    foreach (string n in lines)
                    {
                         outputFile.WriteLine(n);
                    }

                }
                Thread.Sleep(2000);
                
                Second();             
                txtLog.TextChanged -= txtLog_TextChanged;
                }
        }
        
        Process ps = new Process();
        private void Process_Cmd()
        {
            ps.StartInfo.FileName = "cmd.exe";
            //ps.StartInfo.Arguments = @"/K ping 172.172.1.1";
            ps.StartInfo.Arguments = @"/K cd C:\Users\QuynhDam\Documents\Visual Studio 2015\Firmware && C:\Users\QuynhDam\Downloads\tftp -i 192.168.1.1 put tclinux.bin";
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.UseShellExecute = false;
            ps.Start();
            string output = ps.StandardOutput.ReadToEnd().Trim();
            ps.WaitForExit();
            string[] text = new[] { output };
            int n = output.IndexOf("Timeout expired. Retries expired.");
            int m = output.IndexOf("Error 10054.Please check whether the TFTP server is available.");
            if (n > -1 || m > -1)
            {
                MessageBox.Show("Re-Check IP or connect LAN ");
                Application.Exit();
            }
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
            if (label1.Text == ".")
            {
                serialPort.WriteLine("");
                serialPort.WriteLine("");
                label1.Text = "";
                Process_Cmd();
                
                timerGo.Enabled = true;
                timerGo.Start();
                label1.TextChanged -= label1_TextChanged;
            }

        }

        private int j = 0;
        
        private void timerGo_Tick(object sender, EventArgs e)
        {
            j++;
            if(j==13)
            {
                var data = new byte[] { 13, (byte)'g', (byte)'o', 13 };
                serialPort.Write(data, 0, data.Length);
                j = 0;
                timerGo.Stop();
                
            }
            
           
        }

        private void timerLog_Tick(object sender, EventArgs e)
        {
            int count = 0;
            count++;
            if(count==50 && i!=2)

            {
                MessageBox.Show("GPON False");
            }
        }

        private void label2_TextChanged_1(object sender, EventArgs e)
        {
            if (txtLog.Text.Trim() == "login:")
            {
                serialPort.WriteLine("ambit");
                serialPort.WriteLine("ambitdebug");
                serialPort.WriteLine("retsh foxconn168!");
                txtAllLog.Clear();
                timerEnter.Stop();
                Thread.Sleep(2000);
                string text = "prolinecmd serialnum display";
                serialPort.WriteLine("");
                Thread.Sleep(1000);
                serialPort.WriteLine(text);
                serialPort.WriteLine("");
                txtAllLog.Text = serialPort.ReadExisting();
                string[] lines = txtAllLog.Lines.ToArray();
                Thread.Sleep(1000);
                using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\Test_FirmwareUpgrade", "WriteLines2.txt")))
                {
                    foreach (string n in lines)
                    {
                        outputFile.WriteLine(n);
                    }

                }
                Thread.Sleep(2000);
                //string file1 = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\Test_FirmwareUpgrade\\WriteLines1.txt";
                //string file2 = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\Test_FirmwareUpgrade\\WriteLines2.txt";

                //using (StreamReader li = new StreamReader(file1))
                //using (StreamReader li2 = new StreamReader(file2))
                //{

                //    while (true)
                //    {
                //        if (li.EndOfStream || li2.EndOfStream)
                //            break;
                //        string liTxt = li.ReadLine();
                //        string li2Txt = li2.ReadLine();
                //        if (!liTxt.Equals(li2Txt))
                //        {
                //            MessageBox.Show("Upgrade NOT OK");
                //            Application.Exit();
                //        }
                //        else
                //        {
                //            MessageBox.Show("Upgrade OK");
                //            Application.Exit();

                //        }
                //    }


                //}
                string[] lines1 = File.ReadAllLines("C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\Test_FirmwareUpgrade\\WriteLines1.txt");
                string text1 = string.Concat(lines1);
                string value1 = String.Empty;

                List<string> keyValuePairs1 = text1.Split('.').ToList();

                foreach (var keyValuePair1 in keyValuePairs1)
                {
                    string key1 = keyValuePair1.Split(':')[0].Trim();
                    if (key1 == "SerialNum")
                    {
                        value1 = keyValuePair1.Split(':')[1];
                    }
                }
                string[] lines2 = File.ReadAllLines("C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\Test_FirmwareUpgrade\\WriteLines2.txt");
                string text2 = string.Concat(lines1);
                string value2 = String.Empty;

                List<string> keyValuePairs2 = text1.Split('.').ToList();

                foreach (var keyValuePair2 in keyValuePairs2)
                {
                    string key2 = keyValuePair2.Split(':')[0].Trim();
                    if (key2 == "SerialNum")
                    {
                        value2 = keyValuePair2.Split(':')[1];
                    }
                }
                if (value1 == value2)
                {
                    MessageBox.Show("OK");
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("Not OK");
                    Application.Exit();
                }
                serialPort.Close();
            }

        }

        private void Second()
        {
            serialPort.WriteLine("reboot");
            label1.TextChanged += label1_TextChanged;
            label2.TextChanged += label2_TextChanged_1;
            timerEnter.Start();

        }
   

        
    }
}
