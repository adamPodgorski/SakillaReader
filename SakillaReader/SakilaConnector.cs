using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace SakillaReader
{
    class SakilaConnector
    {
        private MySqlConnection connection;

        public SakilaConnector(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
        }

        public DataTable GetDataFromTable(string tableName, string connectionString)
        {
            return RunSqlQuery($"Select * from {tableName}", connectionString);
        }

        public DataTable RunSqlQuery(string query, string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            return dt;
        }

        public bool CheckDbExists(string dbName, string cString)
        {
            connection = new MySqlConnection(cString);
            var result = true;
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{dbName}'", connection);
                result = cmd.ExecuteScalar().Equals(dbName);
            }
            catch(Exception e)
            {
                result = false;
            }
            return result;
        }

        public DataTable GetTableList()
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand($"show tables", connection);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            return dt;
        }

        public bool CheckMySqlConnection(string ConnectionString)
        {
            bool failed = false;
            connection = null;
            try
            {
                using (connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ExecuteScript(string connectionString, string script)
        {
            connection = new MySqlConnection(connectionString);
            var result = true;
            try
            {
                MySqlScript msScript = new MySqlScript(connection, script);
                msScript.Delimiter = "$$";
                msScript.Execute();
            }
            catch(Exception e)
             {
                result = false;
            }
            return result;
        }
    }
}
