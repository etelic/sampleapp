using GeneratorBase.MVC.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace GeneratorBase.MVC.Report
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        //change code for Reports
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string reportName = Request.QueryString["Report"];
                string ID = Request.QueryString["ID"];
                LoadReport(reportName, ID);
            }
        }
        private void LoadReport(string reportName, string ID)
        {
            if (!string.IsNullOrEmpty(reportName))
            {
                MyReportViewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
                MyReportViewer.ServerReport.ReportServerCredentials = new ReportServerCredentials(System.Configuration.ConfigurationManager.AppSettings["ReportUser"], System.Configuration.ConfigurationManager.AppSettings["ReportPass"], System.Configuration.ConfigurationManager.AppSettings["ReportPath"]);
                MyReportViewer.ServerReport.ReportServerUrl = new Uri(System.Configuration.ConfigurationManager.AppSettings["ReportPath"]);
                MyReportViewer.ServerReport.ReportPath = reportName;
                if (!string.IsNullOrEmpty(ID))
                {
                    var reportParameter = new ReportParameter("empId");
                    reportParameter.Values.Add(ID);
                    MyReportViewer.ServerReport.SetParameters(new[] { reportParameter });
                }

                MyReportViewer.ServerReport.Refresh();
            }
        }
        //
    }
    public class ReportServerCredentials : IReportServerCredentials
    {
        private string reportServerUserName;
        private string reportServerPassword;
        private string reportServerDomain;
        public ReportServerCredentials(string userName, string password, string domain)
        {
            reportServerUserName = userName;
            reportServerPassword = password;
            reportServerDomain = domain;
        }
        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use default identity.
                return null;
            }
        }
        public ICredentials NetworkCredentials
        {
            get
            {
                // Use default identity.
                return new NetworkCredential(reportServerUserName, reportServerPassword, reportServerDomain);
            }
        }
        public void New(string userName, string password, string domain)
        {
            reportServerUserName = userName;
            reportServerPassword = password;
            reportServerDomain = domain;
        }
        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            // Do not use forms credentials to authenticate.
            authCookie = null;
            user = null;
            password = null;
            authority = null;
            return false;
        }
    }
}