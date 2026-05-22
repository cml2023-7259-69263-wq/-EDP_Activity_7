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
    public partial class PasswordRecovery : Form
    {
        private string recoveryCode = "";
        private string recoveryEmail = "";
        public PasswordRecovery()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void sendCode_Click(object sender, EventArgs e)
        {
            recoveryEmail = email.Text.Trim();

            if (recoveryEmail == "")
            {
                MessageBox.Show("Please enter your email address!");
                return;
            }

            recoveryCode = new Random().Next(100000, 999999).ToString();

            MessageBox.Show("Your code is: " + recoveryCode);
        }

        private void resetPass_Click(object sender, EventArgs e)
        {
            string enteredCode = code.Text.Trim();
            string newPassword = newPass.Text.Trim();
            string confirmPassword = confirmPass.Text.Trim();

            if (enteredCode == "" || newPassword == "" || confirmPassword == "")
            {
                MessageBox.Show("All fields required.");
                return;
            }

            if (enteredCode != recoveryCode)
            {
                MessageBox.Show("Invalid recovery code.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Password do not match.");
                return;
            }

            DBConnection db = new DBConnection();
            
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    string query = "UPDATE users SET password = @password WHERE email = @email";

                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@password", newPassword);
                        cmd.Parameters.AddWithValue("@email", recoveryEmail);

                        conn.Open();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password reset successful.");

                            Login login = new Login();
                            login.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Email address not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }
    }
}
