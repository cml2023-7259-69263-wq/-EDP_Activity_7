using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

using Excel = Microsoft.Office.Interop.Excel;

namespace Veterinary_Clinic_System
{
    public partial class ReportGenerator : Form
    {


        public ReportGenerator()
        {
            InitializeComponent();
            this.Load += ReportGenerator_Load;
        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label66_Click(object sender, EventArgs e)
        {

        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel74_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void ReportGenerator_Load(object sender, EventArgs e)
        {
            cmbReportType.Items.Clear();

            cmbReportType.Items.Add("Appointment Report");
            cmbReportType.Items.Add("Payment Sales Report");
            cmbReportType.Items.Add("Medical Treatment Report");

            cmbReportType.SelectedIndex = 0;

            dtpFrom.Value = DateTime.Today.AddMonths(-1);
            dtpTo.Value = DateTime.Today;

            dvgReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedItem == null)
            {
                MessageBox.Show("Please select a report type.");
                return;
            }

            string reportType = cmbReportType.SelectedItem.ToString();
            DateTime from = dtpFrom.Value.Date;
            DateTime to = dtpTo.Value.Date;

            LoadReport(reportType, from, to);
        }

        private void LoadReport(string reportType, DateTime from, DateTime to)
        {
            string query = "";

            if (reportType == "Appointment Report")
            {
                query = @"
            SELECT 
                a.appointment_id AS 'Appointment ID',
                CONCAT(po.first_name, ' ', po.last_name) AS 'Owner Name',
                p.pet_name AS 'Pet Name',
                p.pet_species AS 'Species',
                CONCAT(v.first_name, ' ', v.last_name) AS 'Veterinarian',
                s.service_name AS 'Service',
                a.appointment_date AS 'Appointment Date',
                a.reason AS 'Reason',
                a.status AS 'Status'
            FROM appointment a
            INNER JOIN pet p ON a.pet_id = p.pet_id
            INNER JOIN pet_owner po ON p.owner_id = po.owner_id
            INNER JOIN veterinarian v ON a.vet_id = v.vet_id
            INNER JOIN service s ON a.service_id = s.service_id
            WHERE DATE(a.appointment_date) BETWEEN @from AND @to
            ORDER BY a.appointment_date ASC";
            }
            else if (reportType == "Payment Sales Report")
            {
                query = @"
            SELECT
                pay.payment_id AS 'Payment ID',
                pay.appointment_id AS 'Appointment ID',
                CONCAT(po.first_name, ' ', po.last_name) AS 'Owner Name',
                p.pet_name AS 'Pet Name',
                s.service_name AS 'Service',
                pay.amount AS 'Amount',
                pay.payment_date AS 'Payment Date',
                pay.payment_method AS 'Payment Method'
            FROM payment pay
            INNER JOIN appointment a ON pay.appointment_id = a.appointment_id
            INNER JOIN pet p ON a.pet_id = p.pet_id
            INNER JOIN pet_owner po ON p.owner_id = po.owner_id
            INNER JOIN service s ON a.service_id = s.service_id
            WHERE DATE(pay.payment_date) BETWEEN @from AND @to
            ORDER BY pay.payment_date ASC";
            }
            else if (reportType == "Medical Treatment Report")
            {
                query = @"
            SELECT
                pr.record_id AS 'Record ID',
                a.appointment_id AS 'Appointment ID',
                CONCAT(po.first_name, ' ', po.last_name) AS 'Owner Name',
                p.pet_name AS 'Pet Name',
                CONCAT(v.first_name, ' ', v.last_name) AS 'Veterinarian',
                pr.diagnosis AS 'Diagnosis',
                pr.treatment AS 'Treatment',
                pres.medicine_name AS 'Medicine',
                pres.dosage AS 'Dosage',
                pres.duration AS 'Duration',
                pr.notes AS 'Notes'
            FROM pet_record pr
            INNER JOIN appointment a ON pr.appointment_id = a.appointment_id
            INNER JOIN pet p ON a.pet_id = p.pet_id
            INNER JOIN pet_owner po ON p.owner_id = po.owner_id
            INNER JOIN veterinarian v ON a.vet_id = v.vet_id
            LEFT JOIN prescription pres ON pr.record_id = pres.record_id
            WHERE DATE(a.appointment_date) BETWEEN @from AND @to
            ORDER BY a.appointment_date ASC";
            }

            try
            {
                DBConnection db = new DBConnection();
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@from", from);
                        cmd.Parameters.AddWithValue("@to", to);

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        dvgReport.DataSource = table;
                        FormatReportGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message);
            }
        }

        private void FormatReportGrid()
        {
            dvgReport.RowHeadersVisible = false;
            dvgReport.AllowUserToAddRows = false;
            dvgReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dvgReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dvgReport.MultiSelect = false;
            dvgReport.ReadOnly = true;

            dvgReport.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dvgReport.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dvgReport.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dvgReport.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dvgReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            if (dvgReport.Rows.Count == 0)
            {
                MessageBox.Show("Please generate a report first.");
                return;
            }

            ExportToExcel();
        }

        private void ExportToExcel()
        {
            try
            {
                string reportType = cmbReportType.SelectedItem.ToString();

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Add();

                Excel.Worksheet sheet1 = workbook.Sheets[1];
                sheet1.Name = "Report Data";

                Excel.Worksheet sheet2 = workbook.Sheets.Add(After: sheet1);
                sheet2.Name = "Graph";

                sheet1.Rows["1:3"].RowHeight = 24;
                sheet1.Columns["A"].ColumnWidth = 10;


                // Logo area: A1:A3
                Excel.Range logoArea = sheet1.Range["A1", "A4"];
                logoArea.Merge();
                
                // Company name: B1:I1
                Excel.Range companyRange = sheet1.Range["B1", "I1"];
                companyRange.Merge();
                companyRange.Value = "iCurePet Veterinary Clinic";
                companyRange.Font.Size = 18;
                companyRange.Font.Bold = true;
                companyRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                companyRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                // Report title: B2:I2
                Excel.Range reportRange = sheet1.Range["B2", "I2"];
                reportRange.Merge();
                reportRange.Value = reportType;
                reportRange.Font.Size = 13;
                reportRange.Font.Bold = true;
                reportRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                reportRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                // Date range: B3:E3
                Excel.Range dateRange = sheet1.Range["B3", "E3"];
                dateRange.Merge();
                dateRange.Value = "Date Range: " + dtpFrom.Value.ToShortDateString() + " - " + dtpTo.Value.ToShortDateString();
                dateRange.Font.Size = 10;
                dateRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                // Generated date: F3:I3
                Excel.Range generatedRange = sheet1.Range["F3", "I3"];
                generatedRange.Merge();
                generatedRange.Value = "Generated: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt");
                generatedRange.Font.Size = 10;
                generatedRange.Font.Italic = true;
                generatedRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                // Header background and border: A1:I3
                Excel.Range headerTextRange = sheet1.Range["B1", "I3"];
                headerTextRange.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(222, 235, 247));
                headerTextRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                headerTextRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                // Logo
                string logoPath = @"C:\Users\Ayis\Pictures\logo.png";
                if (System.IO.File.Exists(logoPath))
                {
                    Excel.Shape logo = sheet1.Shapes.AddPicture(
                        logoPath,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        Microsoft.Office.Core.MsoTriState.msoCTrue,
                        0,
                        0,
                        -1,
                        -1
                    );

                    logo.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;

                    // Set width only so it does not spill into B
                    logo.Width = 55;

                    // Center inside A1:A3
                    logo.Left = (float)logoArea.Left + ((float)logoArea.Width - (float)logo.Width) / 2;
                    logo.Top = (float)logoArea.Top + ((float)logoArea.Height - (float)logo.Height) / 2;

                    logo.Placement = Excel.XlPlacement.xlMoveAndSize;
                }

                int startRow = 5;


                // COLUMN HEADERS
                for (int i = 0; i < dvgReport.Columns.Count; i++)
                {
                    sheet1.Cells[startRow, i + 1] = dvgReport.Columns[i].HeaderText;
                    sheet1.Cells[startRow, i + 1].Font.Bold = true;
                }

                // DATA ROWS
                for (int i = 0; i < dvgReport.Rows.Count; i++)
                {
                    for (int j = 0; j < dvgReport.Columns.Count; j++)
                    {
                        sheet1.Cells[i + startRow + 1, j + 1] =
                            dvgReport.Rows[i].Cells[j].Value?.ToString();
                    }
                }

                int lastRow = startRow + dvgReport.Rows.Count + 2;

                // SIGNATURE PLACEHOLDER
                sheet1.Cells[lastRow + 2, 1] = "Prepared By:";
                sheet1.Cells[lastRow + 5, 1] = "____________________________";
                sheet1.Cells[lastRow + 6, 1] = "Signature over Printed Name";

                sheet1.Columns.AutoFit();

                // SHEET 2 GRAPH DATA
                sheet2.Cells[1, 1] = "Category";
                sheet2.Cells[1, 2] = "Count";

                DataTable summary = GetSummaryForGraph(reportType);

                for (int i = 0; i < summary.Rows.Count; i++)
                {
                    sheet2.Cells[i + 2, 1] = summary.Rows[i]["Category"].ToString();
                    sheet2.Cells[i + 2, 2] = Convert.ToInt32(summary.Rows[i]["Count"]);
                }

                Excel.Range chartRange = sheet2.Range["A1", "B" + (summary.Rows.Count + 1)];

                Excel.ChartObjects chartObjects = (Excel.ChartObjects)sheet2.ChartObjects();
                Excel.ChartObject chartObject = chartObjects.Add(300, 40, 500, 300);
                Excel.Chart chart = chartObject.Chart;

                chart.SetSourceData(chartRange);
                chart.ChartType = Excel.XlChartType.xlColumnClustered;
                chart.HasTitle = true;
                chart.ChartTitle.Text = reportType + " Graph";

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Workbook|*.xlsx";
                saveDialog.FileName = reportType.Replace(" ", "_") + ".xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveDialog.FileName);
                    MessageBox.Show("Report exported successfully.");
                }

                workbook.Close();
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message);
            }
        }

        private DataTable GetSummaryForGraph(string reportType)
        {
            DataTable table = new DataTable();
            string query = "";

            if (reportType == "Appointment Report")
            {
                query = @"
            SELECT status AS Category, COUNT(*) AS Count
            FROM appointment
            WHERE DATE(appointment_date) BETWEEN @from AND @to
            GROUP BY status";
            }
            else if (reportType == "Payment Sales Report")
            {
                query = @"
            SELECT payment_method AS Category, COUNT(*) AS Count
            FROM payment
            WHERE DATE(payment_date) BETWEEN @from AND @to
            GROUP BY payment_method";
            }
            else if (reportType == "Medical Treatment Report")
            {
                query = @"
            SELECT diagnosis AS Category, COUNT(*) AS Count
            FROM pet_record pr
            INNER JOIN appointment a ON pr.appointment_id = a.appointment_id
            WHERE DATE(a.appointment_date) BETWEEN @from AND @to
            GROUP BY diagnosis";
            }

            DBConnection db = new DBConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@from", dtpFrom.Value.Date);
                    cmd.Parameters.AddWithValue("@to", dtpTo.Value.Date);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(table);
                }
            }

            return table;
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {

        }
    }
 }

