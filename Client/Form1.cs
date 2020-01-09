using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            bool answer = loadSettings();
            if (answer)
            {
                ChangeStatus();
            }
        }

        private bool loadSettings()
        {
            bool answer = false;
            string filename = KEY_VALUES.SETTINGSFILE;
            try
            {
                if (File.Exists(filename))
                {
                    StreamReader reader = new StreamReader(filename);
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] lines = line.Split(KEY_VALUES.SPLIT_CHAR);
                        if (lines[0].Equals(KEY_VALUES.HOST))
                        {
                            MyGlobalClass.Host = lines[1];
                        }
                        if (lines[0].Equals(KEY_VALUES.USER))
                        {
                            MyGlobalClass.User = lines[1];
                        }
                        if (lines[0].Equals(KEY_VALUES.PASSWORD))
                        {
                            MyGlobalClass.Password = lines[1];
                        }
                    }
                    reader.Close();
                }
                else
                {
                    MyGlobalClass.Host = KEY_VALUES.DEFAULT_HOST;
                    MyGlobalClass.User = KEY_VALUES.DEFAULT_USER;
                    MyGlobalClass.Password = KEY_VALUES.DEFAULT_PASSWORD;
                }
                answer = true;
            }
            catch (Exception)
            {
                answer = false;
            }
            return answer;
        }

        private void ChangeStatus()
        {
            userLabel.Text = "User:'" + MyGlobalClass.User + "'";
            passwordLabel.Text = "Password:'" + MyGlobalClass.Password + "'";
            hostLabel.Text = "Host:'" + MyGlobalClass.FullHost + "'";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var window = MessageBox.Show("Are you sure to you want close this application ?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (window == DialogResult.Yes)
            {
                SaveSettings();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void SaveSettings()
        {
            //if(File.Exists(KEY_VALUES.SETTINGSFILE))
            //{
            //    File.Delete(KEY_VALUES.SETTINGSFILE);
            //}
            StreamWriter writer = new StreamWriter(KEY_VALUES.SETTINGSFILE);
            writer.WriteLine(KEY_VALUES.HOST + KEY_VALUES.SPLIT_CHAR + MyGlobalClass.Host);
            writer.WriteLine(KEY_VALUES.USER + KEY_VALUES.SPLIT_CHAR + MyGlobalClass.User);
            writer.WriteLine(KEY_VALUES.PASSWORD + KEY_VALUES.SPLIT_CHAR + MyGlobalClass.Password);
            writer.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }





        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm f = new SettingsForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                ChangeStatus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = "";
            string name = "";
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                filename = textBox1.Text;
                FileInfo fileinfo = new FileInfo(filename);
                name = fileinfo.Name;
            }
            else
            {
                MessageBox.Show("File Address is empty, please choose the file after send", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SendFileAsync(filename, name);
        }

        private async void SendFileAsync(string localFileName, string remoteFileName)
        {
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            statusLabel.Text = "File is sending...";

            Task<string> task = Task.Run(() => SendFile(localFileName, remoteFileName));
            string answer = await task;
            if (!answer.Equals(KEY_VALUES.ANSWER))
            {
                MessageBox.Show("File is not sended, Error:" + answer, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                progressBar1.Value = 100;
                MessageBox.Show("File succesfull sended", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            statusLabel.Text = "Ready";
            progressBar1.Visible = false;
        }

        private string SendFile(string localFileName, string remoteFileName)
        {
            string answer = KEY_VALUES.ANSWER;
            try
            {
                /* Create an FTP Request */
                int bufferSize = 1024;
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(MyGlobalClass.FullHost + remoteFileName);
                /* Log in to the FTP Server with the User Name and Password Provided */
                ftpRequest.Credentials = new NetworkCredential(MyGlobalClass.User, MyGlobalClass.Password);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Establish Return Communication with the FTP Server */
                Stream ftpStream = ftpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */
                FileStream localFileStream = new FileStream(localFileName, FileMode.Open);

                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                long summaBytes = 0;
                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                while (bytesSent != 0)
                {
                    ftpStream.Write(byteBuffer, 0, bytesSent);
                    bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                    summaBytes += bytesSent;
                    this.Invoke((System.Windows.Forms.MethodInvoker)delegate { ProgressValueChange(localFileStream.Length, summaBytes); });

                }
                /* Resource Cleanup */
                localFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;

            }
            catch (Exception ex)
            {
                answer = ex.Message;
            }
            return answer;
        }

        private void ProgressValueChange(long totalBytes, long summaBytes)
        {
            try
            {
                progressBar1.Value = Convert.ToInt32((summaBytes * 100) / totalBytes);
            }
            catch (Exception) { }

        }
    }
}
