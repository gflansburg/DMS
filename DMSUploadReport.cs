namespace Gafware.Modules.DMS
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for DMSUploadReport.
    /// </summary>
    public partial class DMSUploadReport : Telerik.Reporting.Report
    {
        public DMSUploadReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            this.ReportParameters["StartDate"].Value = DateTime.Now.AddDays(-30);
            this.ReportParameters["EndDate"].Value = DateTime.Now;
        }

        public string ConnectionString
        {
            get
            {
                return this.DNN.ConnectionString;
            }
            set
            {
                this.DNN.ConnectionString = value;
            }
        }
    }
}