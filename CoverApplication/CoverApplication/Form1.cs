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

namespace CoverApplication
{
    public partial class Form1 : Form
    {
        //initialization serial port
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
            //get name port and put them to array
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
            label6.Hide();


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
        private void btKetNoi_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    P.DtrEnable = true;
            //    P.Open();
            //    btNgat.Enabled = true;
            //    btKetNoi.Enabled = false;
            //    lblStatus.Text = "Connected";
            //    lblStatus.ForeColor = Color.Lime;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Không kết nối được.", "Thử lại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void btNgat_Click(object sender, EventArgs e)
        {
            //    P.Close();
            //    btKetNoi.Enabled = true;
            //    btNgat.Enabled = false;
            //    lblStatus.Text = "Waiting";
            //    lblStatus.ForeColor = Color.Red;
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            //txtkq.Text = "";

        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            // Application.Exit();
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
                label6.Text = line;
            }
        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{


        //    second++;
        //    label2.Text = minute.ToString() + ":" + second.ToString();
        //    //label2.Hide();
        //    if (second == 5)
        //    {
        //        P.WriteLine("");
        //        P.WriteLine("");
        //        //string text = @"/K cd C:\Users\QuynhDam\Documents\Visual Studio 2015\Firmware && C:\Users\QuynhDam\Downloads\tftp -i 192.168.1.1 put tclinux.bin";
        //        //System.Diagnostics.Process.Start("cmd.exe", text);

        //        ps.StartInfo.FileName = "cmd.exe";
        //        ps.StartInfo.Arguments = @"/K cd C:\Users\QuynhDam\Documents\Visual Studio 2015\Firmware";
        //        //ps.StartInfo.Arguments = @"/K cd C:\Users\QuynhDam\Documents\Visual Studio 2015\Firmware && C:\Users\QuynhDam\Downloads\tftp -i 192.168.1.1 put tclinux.bin";
        //        ps.StartInfo.RedirectStandardOutput = true;
        //        ps.StartInfo.UseShellExecute = false;
        //        ps.Start();
        //        string output = ps.StandardOutput.ReadToEnd();
        //        txtHidden.Text = output.ToString();
        //        ps.WaitForExit();
        //        timer1.Tick -= timer1_Tick;
        //        timer3.Enabled = true;
        //        timer3.Start();
        //        timer3_Tick(sender, e);
        //    }


        // }
        private void Process_Cmd()
        {
            string firmware = "tclinux.bin";
            string ip = "192.168.1.1";
            ps.StartInfo.FileName = "cmd.exe";
            //ps.StartInfo.Arguments = @"/K ping 172.172.1.1";
            ps.StartInfo.Arguments = @"/K cd C:\Users\QuynhDam\Documents\Visual Studio 2015\Firmware && C:\Users\QuynhDam\Downloads\tftp -i "+ ip+" put "+firmware+"";
            ps.StartInfo.RedirectStandardOutput = true;
            ps.StartInfo.UseShellExecute = false;
            ps.Start();
            string output = ps.StandardOutput.ReadToEnd().Trim();
            ps.WaitForExit();
            txtSend.Text = output.ToString();
            string[] output2 =txtSend.Lines;
           // MessageBox.Show(output2[6]);
            
            //string[] stringArray = new string[] { output1 };
            //string[] output1 = "WinAgents TFTP Client version 2.0b Copyright (c) 2004-2011 by Tandem Systems, Ltd. http://www.winagents.com - Software for network administrators Transfering file tclinux.bin to server in octet mode... Transferring data from 193.168.1.1... Error occurred during the file transfer(Error code = 0): Timeout expired. Retries expired. C: \\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Firmware > ";
            if (Array.IndexOf(output2, "Timeout expired. Retries expired.") > -1 == true || Array.IndexOf(output2, "Error 10054.Please check whether the TFTP server is available.") > -1 == true ) {
                MessageBox.Show("Re-Check IP or connect LAN ");
                Application.Exit();
            }
            
        }
        private void Tab2()
        {
            System.Diagnostics.Process.Start(@"C:\Users\QuynhDam\Documents\Visual Studio 2015\Projects\WindowsFormsApplication1\WindowsFormsApplication1\bin\Debug\WindowsFormsApplication1.exe");

        }
        private void txtSend_TextChanged(object sender, EventArgs e)
        {

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
            }
        }

        private void btSend_Click(object sender, EventArgs e)
        {
            txtkq.Clear();

            string command = textBox1.Text.Trim();
            string fileLPath = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\CoverApplication\\Log.txt";
            string text = "prolinecmd serialnum display";
            if (P.IsOpen && command == "#")
            {
                Thread.Sleep(1000);
                P.WriteLine(text);
                P.WriteLine("OK");
                txtkq.Text = P.ReadExisting();
                string[] lines = txtkq.Lines;

                using (StreamWriter sw = new StreamWriter(fileLPath))
                {
                    sw.WriteLine(txtkq.Text);
                }

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
                //Compare();
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
                using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\CoverApplication", "WriteLines.txt")))
                {
                    foreach(string n in lines)
                    {
                        outputFile.WriteLine(n);
                    }
                    
                }
                //using (StreamWriter sw = new StreamWriter(fileLPath))
                //{
                //    sw.WriteLine(txtkq.Text);
                //}

                Thread.Sleep(2000);
                System.Diagnostics.Process.Start(@"C:\Users\QuynhDam\Documents\Visual Studio 2015\Projects\WindowsFormsApplication1\WindowsFormsApplication1\bin\Debug\WindowsFormsApplication1.exe");
                Application.Exit();
                // P.WriteLine("reboot");
                //label6.TextChanged += label6_TextChanged;
                timer2.Tick -= timer2_Tick;


            }
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            second2++;
            //label3.Hide();
            label3.Text = second2.ToString();

            if (second2 == 20)
            {
                EndTask("cmd");
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
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

        private void label4_Click(object sender, EventArgs e)
        {

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

        //private void label6_TextChanged(object sender, EventArgs e)
        //{

        //    if (label6.Text == ".")
        //    {
        //        P.WriteLine("");
        //        P.WriteLine("");
        //        Process_Cmd();
        //        timer5.Enabled = true;
        //        timer5.Start();
        //        timer5_Tick(sender, e);
        //        label6.TextChanged -= label6_TextChanged;

        //    }
        //}
        //int second5 = 0;
        //private void timer5_Tick(object sender, EventArgs e)
        //{

        //    second5++;

        //    label7.Text = second5.ToString();

        //    if (second5 == 20)
        //    {
        //        EndTask("cmd");
        //        Thread.Sleep(1000);
        //        var data = new byte[] { 13, (byte)'g', (byte)'o', 13 };
        //        P.Write(data, 0, data.Length);

        //    }
        //    if (second5 == 100)
        //    {
        //        var data = new byte[] { 13, 13 };
        //        P.Write(data, 0, data.Length);

        //        timer6.Enabled = true;
        //        timer6.Start();
        //        timer6_Tick(sender, e);
        //        timer5.Tick -= timer5_Tick;
        //    }

        //}
        //int second6 = 0;
        //private void timer6_Tick(object sender, EventArgs e)
        //{
        //    second6++;

        //    txtkq.Clear();

        //    string command = textBox1.Text.Trim();
        //    string text = "prolinecmd serialnum display";
        //    if (second6 == 10)
        //    {
        //        //Compare();
        //        P.WriteLine("");
        //        P.WriteLine("ambit");
        //        P.WriteLine("ambitdebug");
        //        P.WriteLine("retsh foxconn168!");

        //        P.WriteLine("");
        //        Thread.Sleep(10);
        //        P.WriteLine(text);
        //        P.WriteLine("");
        //        txtkq.Text = P.ReadExisting();
        //        string[] lines = txtkq.Lines;
        //        using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\CoverApplication", "WriteLines2.txt")))
        //        {
        //            outputFile.WriteLine(txtkq.Text);
        //        }
        //        SoSanh();
        //        timer6.Tick -= timer6_Tick;
        //    }
        //}
        private void SoSanh()
        {
            bool equal = true;
            FileStream myFile1;
            byte[] dataFile1;
            FileStream myFile2;
            byte[] dataFile2;
            string nameFile1 = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\CoverApplication\\WriteLines.txt";
            string nameFile2 = "C:\\Users\\QuynhDam\\Documents\\Visual Studio 2015\\Projects\\CoverApplication\\WriteLines2.txt";


            if ((!File.Exists(nameFile1)) || (!File.Exists(nameFile2)))
            {
                MessageBox.Show("File1 va File2 khong ton tai!!!");
                return;
            }

            try
            {
                myFile1 = File.OpenRead(nameFile1);
                dataFile1 = new byte[myFile1.Length];
                myFile1.Read(dataFile1, 0, (int)myFile1.Length);

                myFile2 = File.OpenRead(nameFile2);
                dataFile2 = new byte[myFile2.Length];
                myFile2.Read(dataFile2, 0, (int)myFile2.Length);

                if (myFile1.Length == myFile2.Length)
                {
                    for (int i = 0; i < dataFile1.Length; i++)
                    {
                        if (dataFile1[i] != dataFile2[i])
                        {
                            equal = false;
                        }
                        else
                        {
                            equal = true;
                        }
                    }
                }
                if (equal)
                {
                    MessageBox.Show("Upgrade OK");
                }
                else
                {
                    MessageBox.Show("Upgrade NOT OK");
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
