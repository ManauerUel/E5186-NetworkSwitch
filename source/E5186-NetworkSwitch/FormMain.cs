using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Threading;

namespace E5186_NetworkSwitch
{

    public partial class FormMain : Form
    {

        private E5186API api;
        private IPAddress ipAddress;
        private String password;
        private E5186API.NET_MODE? curNetMode;

        private System.Timers.Timer netModeRefresher;
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        public FormMain()
        {
            InitializeComponent();

            ipAddress = IPAddress.Parse((String)Properties.Settings.Default["ipAddress"]);
            password = (String)Properties.Settings.Default["password"];

            netModeRefresher = new System.Timers.Timer();
            netModeRefresher.Interval = 3000;
            netModeRefresher.Elapsed += delegate (object source, ElapsedEventArgs e)
            {
                dispatcher.Invoke(refreshCurrentNetworkMode);
            };


            api = new E5186API(ipAddress, password);
            api.messageHandler += delegate (String message, MessageType type)
            {
                dispatcher.Invoke(delegate() {
                    statusLabel.Text = message;
                    if (type == MessageType.ERROR)
                        statusLabel.ForeColor = Color.DarkRed;
                    if (type == MessageType.MESSAGE)
                        statusLabel.ForeColor = Color.Black;
                });
            };
            api.connectionHandler += delegate (ConnectionType type)
            {
                try {
                    if (type == ConnectionType.CONNECTED)
                    {
                        dispatcher.Invoke(refreshCurrentNetworkMode);
                        netModeRefresher.Start();
                    }
                    if (type == ConnectionType.DISCONNECTED)
                    {
                        netModeRefresher.Stop();
                        dispatcher.Invoke(refreshCurrentNetworkMode);
                    }
                } catch(Exception ex)
                {
                    // TaskCanceledException at exit
                }


            };
            api.connectThreaded();
        }


        private void refreshCurrentNetworkMode()
        {
            if (api == null || api.Connected == ConnectionType.DISCONNECTED)
            {
                buttonHSDPA.Enabled = false;
                buttonLTE.Enabled = false;
            }
            else { 
                curNetMode = api.getNetMode();
                statusLabel.Text = "Momentaner Netzwerkmodus: " + curNetMode.ToString();

                if (curNetMode == E5186API.NET_MODE.HSDPA || curNetMode == E5186API.NET_MODE.HSDPA_PREFFERED)
                {
                    buttonHSDPA.Enabled = false;
                    buttonLTE.Enabled = true;
                }
                else if (curNetMode == E5186API.NET_MODE.LTE || curNetMode == E5186API.NET_MODE.LTE_PREFFERED)
                {
                    buttonHSDPA.Enabled = true;
                    buttonLTE.Enabled = false;
                }
                else if (curNetMode == E5186API.NET_MODE.AUTO || curNetMode == E5186API.NET_MODE.EDGE)
                {
                    buttonHSDPA.Enabled = true;
                    buttonLTE.Enabled = true;
                }
                else
                {
                    buttonHSDPA.Enabled = false;
                    buttonLTE.Enabled = false;
                }
            }

        }

        private void buttonLTE_Click(object sender, EventArgs e)
        {
            api.setNetMode(E5186API.NET_MODE.LTE);
        }

        private void buttonHSDPA_Click(object sender, EventArgs e)
        {
            api.setNetMode(E5186API.NET_MODE.HSDPA);
        }

        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            FormSettings settingsDialog = new FormSettings(ipAddress, password);
            settingsDialog.FormClosing += delegate (object senderDialog, FormClosingEventArgs args)
            {
                ipAddress = IPAddress.Parse(settingsDialog.textBoxIpAddress.Text);
                password = settingsDialog.textBoxPassword.Text;

                Properties.Settings.Default["ipAddress"] = ipAddress.ToString();
                Properties.Settings.Default["password"] = password;
                Properties.Settings.Default.Save();

                api.disconnect();
                api.Password = password;
                api.IpAddress = ipAddress;
                api.connectThreaded();

                settingsDialog.Dispose();
            };

            showSmoothDialog(settingsDialog);
        }


        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            FormAbout aboutDialog = new FormAbout();
            showSmoothDialog(aboutDialog);
        }

        private void showSmoothDialog(Form dialogToShow)
        {
            // Doing a little smooth Wiggle Wiggle
            dialogToShow.StartPosition = FormStartPosition.Manual;
            dialogToShow.Visible = true;
            int currX = this.Location.X + this.Width - dialogToShow.Width / 3;
            for (int i = currX; i <= this.Location.X + this.Width + 3; i += 3)
            {
                dialogToShow.Location = new Point(i, this.Location.Y);
                dialogToShow.Refresh();
                Thread.Sleep(1);
            }
            dialogToShow.Visible = false;
            dialogToShow.ShowDialog(this);
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
