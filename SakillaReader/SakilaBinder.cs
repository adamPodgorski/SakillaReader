using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace SakillaReader
{
    class SakilaBinder
    {
        string connectionString;
        SakilaConnector connector;
        public SakilaBinder(string connectionString)
        {
             connector = new SakilaConnector(connectionString);
            
        }

        public List<string> GetTablesList(string connectionString)
        {
            var connectionOk = connector.CheckMySqlConnection(connectionString);
            if (connectionOk)
            {
                var tables = connector.GetTableList();
                var tableList = tables.AsEnumerable().Select(x => x[0].ToString()).ToList();
                return tableList;
            }
            return new List<string>();
        }

        public bool checkDatabaseExists(string dbName, string connectionString)
        {
            return connector.CheckDbExists(dbName, connectionString);  
        }

        public bool CreateSakilaDb(string connectionString)
        {
            var result = true;
            try
            {
                connector.ExecuteScript(connectionString,File.ReadAllText("SakilaCreate.sql"));
            }
            catch(Exception e)
            {
                return false;
            }

            return result;
        }

        public bool InitialLoadSakilaDB(string connectionString)
        {
            var result = true;
            try
            {
                connector.ExecuteScript(connectionString, File.ReadAllText("SakilaInsert.sql"));
            }
            catch (Exception e)
            {
                return false;
            }

            return result;
        }

        public void SaveToCSV(DataTable dt, char csvDelimiter, string filename)
        {
            try
            {

                int columnCount = dt.Columns.Count;
                string columnNames = "";
                string[] output = new string[dt.Rows.Count + 1];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += dt.Columns[i].ToString() + csvDelimiter;
                }
                output[0] += columnNames;

                for (int i = 1; (i - 1) < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        output[i] += dt.Rows[i - 1][j].ToString() + csvDelimiter;
                    }
                }

                System.IO.File.WriteAllLines(filename, output, System.Text.Encoding.UTF8);
            }
            catch (Exception e) { }
         }

        public DataTable GetActiveCustomers(string connectionString)
        {
            return connector.GetDataFromTable("customer", connectionString).Select("active=1").CopyToDataTable(); 
        }

        public DataTable GetRentedTitles(string connectionString)
        {
            var cmd = @"select f.title, r.rental_date, CONCAT(c.first_name, ' ', c.last_name) as rented_by   from sakila.rental r 
                    join sakila.inventory i on r.inventory_id = i.inventory_id
                    join sakila.film f on i.film_id = f.film_id
                    join sakila.customer c on r.customer_id = c.customer_id
                    where r.return_date is null; ";
            return connector.RunSqlQuery(cmd, connectionString);
        }

        public DataTable GetOverdueTitles(string connectionString)
        {
            var cmd = @$"select f.title, r.rental_date, CONCAT(c.first_name, ' ', c.last_name) as rented_by   from sakila.rental r 
                    join sakila.inventory i on r.inventory_id = i.inventory_id
                    join sakila.film f on i.film_id = f.film_id
                    join sakila.customer c on r.customer_id = c.customer_id
                    where r.return_date is null and rental_date<'{DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd")}'; ";
            return connector.RunSqlQuery(cmd, connectionString);
        }

        public DataTable GetAllCustomersWithRentals(string connectionString)
        {
            var cmd = @"select f.title, r.rental_date, r.return_date, CONCAT(c.first_name, ' ', c.last_name) as rented_by   from sakila.rental r 
                    join sakila.inventory i on r.inventory_id = i.inventory_id
                    join sakila.film f on i.film_id = f.film_id
                    join sakila.customer c on r.customer_id = c.customer_id;";
            return connector.RunSqlQuery(cmd, connectionString);
        }

        public DataTable GetNewRentals(string connectionString)
        {
            var cmd = @"select * from sakila.customer c where c.create_date>'2022-03-20 15:16:03'; ";
            return connector.RunSqlQuery(cmd, connectionString);
        }
    }
}
