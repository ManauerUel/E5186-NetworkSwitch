using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace E5186_NetworkSwitch
{
    public partial class FormSettings : Form
    {
        public FormSettings(IPAddress ipAddress, String password)
        {
            InitializeComponent();
            textBoxIpAddress.Text = ipAddress.ToString();
            textBoxPassword.Text = password;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((Form.ModifierKeys == Keys.None && keyData == Keys.Escape) || keyData == Keys.Enter)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

    }
}
