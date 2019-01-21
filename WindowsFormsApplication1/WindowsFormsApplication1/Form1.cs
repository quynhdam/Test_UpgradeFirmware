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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        SerialPort P = new SerialPort();

        string InputData = String.Empty;

        private delegate void SetTextDeleg(string data);

        delegate void SetTextCallback(string text);
        Process ps = new Process();
        int second1 = 0;
        int second2 = 0;
        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            //
            P.ReadTimeout = 1000;
            P.DataReceived += new SerialDataReceivedEventHandler(DataReceive);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            txtkq.Multiline = true;
            txtkq.WordWrap = true;
            txtkq.ScrollBars = ScrollBars.Vertical;
            txtkq.BackColor = Color.Black;
            txtkq.ForeColor = Color.White;
            label5.Hide();
            label2.Hide();
            label3.Hide();
            label4.Hide();
            
        }
        private void Connect()
        {
            P.BaudRate = 115200;
            P.PortName = "COM1";
            P.Parity = Parity.None;
            P.DataBits = 8;
            P.DtrEnable = true;
            P.StopBits = StopBits.One;
            P.Open();
        }
        private void DataReceive(object sender, SerialDataReceivedEventArgs e)
        {
            InputData = P.ReadExisting();
            if (InputData != String.Empty)
            {
                SetText(InputData);

            }
        }


        private void SetText(string text)
        {
            if (this.txtkq.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else this.txtkq.Text += text;
        }

        private void txtkq_TextChanged(object sender, EventArgs e)
        {
            txtkq.SelectionStart = txtkq.TextLength;
            txtkq.ScrollToCaret();
            string[] lines = txtkq.Lines;

            foreach (string line in lines)
            {
                textBox1.Text = line;
                label5.Text = line;
            }
        }
        private void Process_Cmd()
        {
            ps.StartInfo.FileName = "cmd.exe";
            //ps.StartInfo.Arguments = @"/K ping 172.172.1.1";
            ps.StartInfo.Arguments = @"/K cd C:\Users\QuynhDam\Documents\Visual Studio 2015\Firmware && C:\Users\QuynhDam\Downloads\tftp -i 192.168.1.1 put tclinux.bin";
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.UseShellExecute = false;
            ps.Start();
            string output = ps.StandardOutput.ReadToEnd();
            ps.WaitForExit();
        }
        private void GhiFile()
        {
            txtkq.Clear();

            string command = textBox1.Text.Trim();
            string fileLPath = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\CoverApplication\\Log.txt";
            string text = "prolinecmd serialnum display";
            if (P.IsOpen && command == "#")
            {
                Thread.Sleep(1000);
                P.WriteLine("");
                P.WriteLine(text);
                //P.WriteLine("OK");
                txtkq.Text = P.ReadExisting();
                string[] lines = txtkq.Lines;
                File.AppendAllLines(fileLPath, txtkq.Lines);

                //using (StreamWriter sw = new StreamWriter(fileLPath))
                //{
                //    sw.WriteLine(txtkq.Text);
                //}
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            second1++;
            label4.Hide();
            label4.Text = second1.ToString();
            txtkq.Clear();

            string command = textBox1.Text.Trim();
            string text = "prolinecmd serialnum display";
            if (second1 == 10)
            {
                // Compare();
                P.WriteLine("");
                P.WriteLine("ambit");
                P.WriteLine("ambitdebug");
                P.WriteLine("retsh foxconn168!");

                P.WriteLine("");
                Thread.Sleep(10);
                P.WriteLine(text);
                P.WriteLine("");
                txtkq.Text = P.ReadExisting();
                string[] lines = txtkq.Lines.ToArray();
                //string n = txtkq.Text.Trim();

                //File.WriteAllText(fileLPath,txtkq.Text);
                using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\WindowsFormsApplication1", "WriteLines.txt")))
                {
                    foreach (string n in lines)
                    {
                        outputFile.WriteLine(n);
                    }
                }
                //using (StreamWriter sw = new StreamWriter(fileLPath))
                //{
                //    sw.WriteLine(txtkq.Text);
                //}
                timer2.Tick -= timer2_Tick;
                SoSanh();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            second2++;
            //label3.Hide();
            label3.Text = second2.ToString();

            if (second2 == 20)
            {
                EndTask("cmd");

                var currentProc = System.Diagnostics.Process.GetCurrentProcess();
                string name = currentProc.ProcessName;
                txtSend.Text = name.ToString();
                var data = new byte[] { 13, (byte)'g', (byte)'o', 13 };
                P.Write(data, 0, data.Length);

            }
            if (second2 == 100)
            {
                var data = new byte[] { 13, 13 };
                P.Write(data, 0, data.Length);

                timer2.Enabled = true;
                timer2.Start();
                timer2_Tick(sender, e);
                timer3.Tick -= timer3_Tick;
            }

        }
        public void EndTask(string taskname)
        {
            string processName = taskname.Replace(".exe", "");

            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }

        private void Compare()
        {
            P.WriteLine("");
            P.WriteLine("ambit");
            P.WriteLine("ambitdebug");
            P.WriteLine("retsh foxconn168!");
        }
        int second4 = 0;
        int minute4 = 0;
        int hour4 = 0;
        private void timer4_Tick(object sender, EventArgs e)
        {
            second4++;
            label1.Text = hour4.ToString() + ":" + minute4.ToString() + ":" + second4.ToString();
            if (second4 == 59)
            {
                minute4++;
                second4 = 0;
            }
            if (minute4 == 59)
            {
                hour4++;
                minute4 = 0;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            txtkq.Clear();

            string command = textBox1.Text.Trim();

            if (P.IsOpen && command == "#")
            {
                P.WriteLine("");
                P.WriteLine("prolinecmd serialnum display");
                txtkq.Text = P.ReadExisting();
                //string[] lines = txtkq.Lines;
                string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                File.AppendAllLines(Path.Combine(mydocpath, "WriteFile.txt"), txtkq.Lines);


            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Connect();
            if (P.IsOpen)
            {
                lblStatus.Text = "Connected";
                lblStatus.ForeColor = Color.Lime;
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                MessageBox.Show("Không kết nối được.", "Thử lại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            timer4.Enabled = true;
            timer4.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            P.Close();
            timer4.Stop();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            
        }

        private void label5_TextChanged(object sender, EventArgs e)
        {
            if (label5.Text == ".")
            {
                P.WriteLine("");
                P.WriteLine("");
                Process_Cmd();
                timer3.Enabled = true;
                timer3.Start();
                timer3_Tick(sender, e);
                label5.TextChanged -= label5_TextChanged;
            }
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string fileLPath = @"C:/Users/QuynhDam/Documents/Visual Studio 2015/Projects/WindowsFormsApp/Log.txt";

        //    string[] lines = textBox1.Lines;
        //    int count = textBox1.Lines.Length;

        //    using (StreamWriter sw = new StreamWriter(fileLPath))
        //    {


        //            sw.WriteLine(this.textBox1.Text);


        //        sw.Close();

        //    }

        //    button1.Text = count.ToString();
        //    FileStream file = new FileStream(@"C:/Users/QuynhDam/Documents/Visual Studio 2015/Projects/WindowsFormsApp/Log.txt", FileMode.Open);
        //    StreamReader read = new StreamReader(file);
        //    if (read.ReadToEnd() != null)
        //    {
        //        MessageBox.Show("OK", "Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Not OK", "Notification", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        //    }
        //    read.Close();
        //}
        private void SoSanh()
        {
            string file1 = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\CoverApplication\\WriteLines.txt";
            string file2 = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\WindowsFormsApplication1\\WriteLines.txt";
            if ((!File.Exists(file1)) || (!File.Exists(file2)))
            {
                MessageBox.Show("File1 va File2 khong ton tai!!!");
                return;
            }
            using (StreamReader li = new StreamReader(file1))
            using (StreamReader li2 = new StreamReader(file2))
            {
                
                 while (true)
                    {
                        if (li.EndOfStream || li2.EndOfStream)
                            break;
                        string liTxt = li.ReadLine();
                        string li2Txt = li2.ReadLine();
                        if (!liTxt.Equals(li2Txt))
                        {
                            MessageBox.Show("Upgrade NOT OK");
                        }
                        else
                        {
                            MessageBox.Show("Upgrade OK");
                        }
                    }
                
                
            }
        }
    }
}
