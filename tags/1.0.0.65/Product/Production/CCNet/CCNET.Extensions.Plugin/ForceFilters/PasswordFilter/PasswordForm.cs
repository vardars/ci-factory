using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CCNET.Extensions.Plugin.ForceFilters.PasswordFilter
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();
        }

        public string Password
        {
            get
            {
                return this.textBox1.Text;
            }
        }
    }
}