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

namespace Veterinary_Clinic_System
{
    public partial class UserManagement : Form
    {
        private void LoadUsers(string keyword = "")
        {
            DBConnection db = new DBConnection();

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    string query = @"
                SELECT 
                    user_id AS ID,
                    CONCAT(first_name, ' ', last_name) AS Name,
                    email AS Email,
                    role AS Role,
                    status AS Status
                FROM users
                WHERE first_name LIKE @keyword
                   OR last_name LIKE @keyword
                   OR email LIKE @keyword
                   OR username LIKE @keyword
                   OR role LIKE @keyword
                   OR status LIKE @keyword";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            accountList.DataSource = table;

                            accountList.RowHeadersVisible = false;
                            accountList.AllowUserToAddRows = false;
                            accountList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            accountList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                            accountList.MultiSelect = false;
                            accountList.ReadOnly = true;

                            accountList.Columns["ID"].FillWeight = 10;
                            accountList.Columns["Name"].FillWeight = 25;
                            accountList.Columns["Email"].FillWeight = 35;
                            accountList.Columns["Role"].FillWeight = 20;
                            accountList.Columns["Status"].FillWeight = 15;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        public UserManagement()
        {
            InitializeComponent();
        }

        private void accountList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void UserManagement_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (accountList.CurrentRow == null)
            {
                MessageBox.Show("Please select an account first.");
                return;
            }

            int userId = Convert.ToInt32(accountList.CurrentRow.Cells["ID"].Value);

            AddAccount editForm = new AddAccount(userId);
            editForm.ShowDialog();

            LoadUsers();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text.Trim());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddAccount addAccount = new AddAccount();
            addAccount.ShowDialog();

            LoadUsers();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
            this.Hide();
        }
    }
}
