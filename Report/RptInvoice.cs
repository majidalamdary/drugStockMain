using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace DrugStockWeb.Report
{
    public partial class RptInvoice : DevExpress.XtraReports.UI.XtraReport
    {
        public RptInvoice()
        {
            InitializeComponent();
        }

        private void RptInvoice_BeforePrint(object sender, CancelEventArgs e)
        {
            
        }
    }
}
