using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Veterinary_Clinic_System
{
    public class DBConnection
    {
        private string connectionString = "server=127.0.0.1;port=3307;database=vet_db;uid=root;pwd=@Sqlroot123!;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}