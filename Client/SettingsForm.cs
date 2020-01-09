using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string host = "";
            string user = "";
            string password = "";
            if(!string.IsNullOrEmpty(hostTextBox.Text))
            {
                host = hostTextBox.Text;
            }
            else
            {
                MessageBox.Show("Host is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
            if (!string.IsNullOrEmpty(userTextBox.Text))
            {
                user = userTextBox.Text;
            }
            else
            {
                MessageBox.Show("User is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
            if (!string.IsNullOrEmpty(passwordTextBox.Text))
            {
                password = passwordTextBox.Text;
            }
            else
            {
                MessageBox.Show("Password is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }                        
            MyGlobalClass.Host = host;
            MyGlobalClass.User = user;
            MyGlobalClass.Password = password;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            hostTextBox.Text = MyGlobalClass.Host;
            userTextBox.Text = MyGlobalClass.User;
            passwordTextBox.Text = MyGlobalClass.Password;
        }
    }
}
