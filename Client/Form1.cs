using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private ChatClient Client;

        public Form1()
        {
            InitializeComponent();
            this.Client = new ChatClient();
            this.Client.ChatUpdate += new ChatClient.ChatboxUpdate(listBox1_Update);
        }

        private void listBox1_Update(string str)
        {
            if (textBox2.InvokeRequired)
            {
                textBox2.BeginInvoke(
                    new Action(() =>
                    {
                        textBox2.Text += str;
                        textBox2.Text += Environment.NewLine;
                    }));
                
            }
            else
            {
                textBox2.Text += str;
                textBox2.Text += Environment.NewLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Send();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Send();
        }

        private void Send()
        {
            if (InputBox.Text == string.Empty)
            {
                MessageBox.Show("입력안했는데요", "잠깐");
                return;
            }
            this.Client.Send(InputBox.Text);
            InputBox.Clear();
        }

        #region InputBox Placeholder Function
        private void InputBox_Enter(object sender, EventArgs e)
        {
            InputBox.Text = string.Empty;
            InputBox.ForeColor = Color.Black;
        }

        private void InputBox_Leave(object sender, EventArgs e)
        {
            if (InputBox.Text == string.Empty)
            {
                InputBox.Text = "Type here!";
                InputBox.ForeColor = Color.Silver;
            }
                
        }
        #endregion
    }
}
