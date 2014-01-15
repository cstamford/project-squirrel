// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;
using System.Net;
using System.Windows.Forms;

namespace Squirrel.Client.Interface
{
    public partial class ConnectDialog : Form
    {
        public ConnectDialog()
        {
            InitializeComponent();
        }

        private void ButtonConnect_Click(object sender, System.EventArgs e)
        {
            IPAddress address;
            int port;
            string name;

            try
            {
                address = IPAddress.Parse(TextBoxIP.Text);
                port = int.Parse(TextBoxPort.Text);
                name = TextBoxNickname.Text;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                return;
            }

            try
            {
                if (((Interface)Owner).connect(address, port, name))
                {
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to connect!");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }
    }
}
