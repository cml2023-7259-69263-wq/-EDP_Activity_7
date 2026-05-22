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
    public partial class AddAccount : Form
    {
        private int selectedUserId = 0;
        private bool isEditMode = false;

        public AddAccount(int userId)
        {
            InitializeComponent();

            selectedUserId = userId;
            isEditMode = true;
        }

        public AddAccount()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtFirstName.Text.Trim() == "" ||
       txtLastName.Text.Trim() == "" ||
       txtEmail.Text.Trim() == "" ||
       txtUsername.Text.Trim() == "" ||
       txtPassword.Text.Trim() == "" ||
       cmbRole.Text == "" ||
       cmbStatus.Text == "")
            {
                MessageBox.Show("Please complete all required fields.");
                return;
            }

            if (isEditMode)
            {
                UpdateAccount();
            }
            else
            {
                AddNewAccount();
            }
        }

       

        private void AddNewAccount()
        {
            DBConnection db = new DBConnection();

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    string query = @"
                        INSERT INTO users
                        (first_name, last_name, email, contact_number, username, password, role, status)
                        VALUES
                        (@first_name, @last_name, @email, @contact_number, @username, @password, @role, @status)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@last_name", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact_number", txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                        cmd.Parameters.AddWithValue("@role", cmbRole.Text);
                        cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Account added successfully.");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        private void UpdateAccount()
        {
            DBConnection db = new DBConnection();

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    string query = @"
                UPDATE users SET
                    first_name = @first_name,
                    last_name = @last_name,
                    email = @email,
                    contact_number = @contact_number,
                    username = @username,
                    password = @password,
                    role = @role,
                    status = @status
                WHERE user_id = @user_id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@last_name", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact_number", txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                        cmd.Parameters.AddWithValue("@role", cmbRole.Text);
                        cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                        cmd.Parameters.AddWithValue("@user_id", selectedUserId);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Account updated successfully.");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        private void LoadAccountDetails()
        {
            DBConnection db = new DBConnection();

            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    string query = "SELECT * FROM users WHERE user_id = @user_id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", selectedUserId);

                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFirstName.Text = reader["first_name"].ToString();
                                txtLastName.Text = reader["last_name"].ToString();
                                txtEmail.Text = reader["email"].ToString();
                                txtContact.Text = reader["contact_number"].ToString();
                                txtUsername.Text = reader["username"].ToString();
                                txtPassword.Text = reader["password"].ToString();
                                cmbRole.Text = reader["role"].ToString();
                                cmbStatus.Text = reader["status"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }


        private void AddAccount_Load(object sender, EventArgs e)
        {
            cmbRole.Items.Clear();
            cmbRole.Items.Add("Admin");
            cmbRole.Items.Add("Veterinarian");
            cmbRole.Items.Add("Staff");

            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Active");
            cmbStatus.Items.Add("Inactive");

            cmbRole.SelectedIndex = 2;   // Staff
            cmbStatus.SelectedIndex = 0; // Active

            txtPassword.UseSystemPasswordChar = true;

            if (isEditMode)
            {
                this.Text = "Edit Account";
                btnSave.Text = "Update";
                LoadAccountDetails();
            }
            else
            {
                this.Text = "Add Account";
                btnSave.Text = "Save";
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
