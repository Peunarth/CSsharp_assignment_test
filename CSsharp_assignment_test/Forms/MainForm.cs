using CSharp_assignment_test.Models; // ត្រូវប្រាកដថា namespace នេះត្រឹមត្រូវ
using CSsharp_assignment_test; // ត្រូវប្រាកដថា namespace នេះត្រឹមត្រូវ
using CSsharp_assignment_test.Reports; // ត្រូវប្រាកដថា namespace នេះត្រឹមត្រូវ
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CSharp_assignment_test
{
    public partial class MainForm : Form
    {
        private SaleService _saleService;
        public MainForm()
        {
            InitializeComponent();
            _saleService = new SaleService();
        }
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtpStartDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date;
            string productNameFilter = txtProductNameFilter.Text.Trim();

            try
            {
                SaleReport report = new SaleReport();
                report.Parameters["paramStartDate"].Value = startDate;
                report.Parameters["paramEndDate"].Value = endDate;

                if (!string.IsNullOrEmpty(productNameFilter))
                {
                    report.Parameters["paramProductNameFilter"].Value = productNameFilter;
                }
                else
                {
                    report.Parameters["paramProductNameFilter"].Value = DBNull.Value;
                }
                if (report.FindControl("xrLabelStartDate", true) is XRLabel startDateLabel)
                {
                    startDateLabel.Text = $"From : {startDate.ToShortDateString()}";
                }
                if (report.FindControl("xrLabelEndDate", true) is XRLabel endDateLabel)
                {
                    endDateLabel.Text = $"To : {endDate.ToShortDateString()}";
                }
                ReportPrintTool printTool = new ReportPrintTool(report);
                printTool.PreviewForm.WindowState = FormWindowState.Maximized;
                printTool.ShowRibbonPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while generating the report: {ex.Message}\nCheck logs/errors.txt for details if available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            LoadProdut();
        }
        void LoadProdut()
        {
            DateTime startDate = dtpStartDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date;
            string productNameFilter = txtProductNameFilter.Text.Trim();
            try
            {
                List<SaleDto> salesData = _saleService.GetProductSales(startDate, endDate, productNameFilter);
                if (salesData != null && salesData.Any())
                {
                    dgvProduct.DataSource = salesData;
                    if (dgvProduct.Columns.Contains("Column5"))
                    {
                        dgvProduct.Columns["Column5"].DefaultCellStyle.Format = "c2";
                        dgvProduct.Columns["Column5"].DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                    }
                    if (dgvProduct.Columns.Contains("Column4"))
                    {
                        dgvProduct.Columns["Column4"].DefaultCellStyle.Format = "c2";
                        dgvProduct.Columns["Column4"].DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                    }
                }
                else
                {
                    dgvProduct.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"An unexpected error occurred while loading data:\n\n";
                errorMessage += $"Message: {ex.Message}\n";
                errorMessage += $"Source: {ex.Source}\n";
                errorMessage += $"Target Method: {ex.TargetSite?.Name}\n";
                errorMessage += $"Stack Trace:\n{ex.StackTrace}";
                MessageBox.Show(errorMessage, "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    string logDirectory = "logs";
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    string logFilePath = Path.Combine(logDirectory, "errors.txt");                    
                    File.AppendAllText(logFilePath, $"[{DateTime.Now}] {errorMessage}\n\n-----\n\n");
                }
                catch (Exception logEx)
                {                    
                    MessageBox.Show($"Could not write to log file: {logEx.Message}\n\nOriginal error: {ex.Message}", "Log Error Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                dgvProduct.DataSource = null;
            }
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProdut();
        }
        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtpStartDate.Value.Date;
            var endDate = dtpEndDate.Value.Date;
            string productNameFilter = txtProductNameFilter.Text.Trim();
            try
            {
                SaleReport report = new SaleReport();
                report.Parameters["paramStartDate"].Value = startDate;
                report.Parameters["paramEndDate"].Value = endDate;
                if (!string.IsNullOrEmpty(productNameFilter))
                {
                    report.Parameters["paramProductNameFilter"].Value = productNameFilter;
                }
                else
                {
                    report.Parameters["paramProductNameFilter"].Value = DBNull.Value;
                }
                if (report.FindControl("xrLabelStartDate", true) is XRLabel startDateLabel)
                {
                    startDateLabel.Text = $"Report Period: {startDate}";
                }
                if (report.FindControl("xrLabelEndDate", true) is XRLabel endDateLabel)
                {
                    endDateLabel.Text = $"To {endDate}";
                }
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                    saveFileDialog.FileName = $"ProductSalesReport_{DateTime.Now:yyyyMMdd}.pdf";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        report.ExportToPdf(saveFileDialog.FileName);

                        MessageBox.Show("Report exported to PDF successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
