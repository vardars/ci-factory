using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public partial class LoginForm : Form
    {
        private string _proxyServerUrl;

        public LoginForm(string proxyServerUrl)
        {
            this._proxyServerUrl = proxyServerUrl;
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            WebRequest request = WebRequest.Create(_proxyServerUrl + "/authenticate");
            NetworkCredential netCredential = new NetworkCredential(this.userName.Text, this.password.Text);
            request.Credentials = netCredential;
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            WebResponse webResponse = null;
            try
            {
                webResponse = request.GetResponse();
                StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
                string response = streamReader.ReadToEnd();
                webResponse.Close();

                if (response.Contains("AUTHENTICATION SUCCESSFUL"))
                {
                    Credentials.UserName = this.userName.Text;
                    Credentials.Password = this.password.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Unable to authenticate with the proxy server. Check your username and password and try again.");
                }
            }
            catch
            {
                MessageBox.Show("Unable to authenticate with the proxy server. Check your username and password and try again.");
            }            
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }
}
