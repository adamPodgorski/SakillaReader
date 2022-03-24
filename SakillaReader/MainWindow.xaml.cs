using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SakillaReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            tConnectionString.Text = "server=127.0.0.1;uid=root;pwd=adam12;database=sakila";
        }

        private void ConnectionString_TextChanged(object sender, TextChangedEventArgs e)
        {
            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            var tableList = binder.GetTablesList(tConnectionString.Text);
            cTableList.DataContext = tableList;

        }

        private void cTableList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (cTableList.SelectedItem.ToString() != string.Empty)
            {
                var db = new SakilaConnector(tConnectionString.Text);
                var results = db.GetDataFromTable(cTableList.SelectedItem.ToString(), tConnectionString.Text);
                explorer.DataContext = results.DefaultView;
            }
        }

        private void bDbExists_Click(object sender, RoutedEventArgs e)
        {
            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);

            if (binder.checkDatabaseExists("sakila",tConnectionString.Text))
            {
                MessageBox.Show("Sakila DB exists on server 127.0.0.1");
            }
            else
            {
                MessageBox.Show("Sakila DB not exists on server 127.0.0.1");
            }
        }

        private void bLoadData_Click(object sender, RoutedEventArgs e)
        {
            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            if (binder.checkDatabaseExists("sakila", tConnectionString.Text))
            {
                MessageBox.Show("DB already exists");
            }
            else
            {
                binder.CreateSakilaDb(lServerPath.Content.ToString());
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            if (!binder.checkDatabaseExists("sakila", tConnectionString.Text))
            {
                MessageBox.Show("DB not exists");
            }
            else
            {
                binder.InitialLoadSakilaDB(lServerPath.Content.ToString());
            }
        }

        private void convertToReport(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tFilePath.Text) && Directory.Exists(tFilePath.Text))
            {
                SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
                binder.SaveToCSV(((DataView)explorer.ItemsSource).ToTable(), ';', @$"{tFilePath.Text}\exportfile.csv");
            }
            else
            {
                MessageBox.Show("Please provide valid file path");
            }
        }

        private void getActiveRentals(object sender, RoutedEventArgs e)
        {
            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            explorer.DataContext = binder.GetActiveCustomers(tConnectionString.Text).DefaultView;
            
        }

        private void getRentedTitles(object sender, RoutedEventArgs e)
        {
            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            explorer.DataContext = binder.GetRentedTitles(tConnectionString.Text).DefaultView;
        }

        private void getNewRetals(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Warning!!!! \nThis will return only rentals after 20.03.2022");
            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            explorer.DataContext = binder.GetNewRentals(tConnectionString.Text).DefaultView;
        }

        private void getAllCustomersWithRentals(object sender, RoutedEventArgs e)
        {
            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            explorer.DataContext = binder.GetAllCustomersWithRentals(tConnectionString.Text).DefaultView;
        }

        private void getOverdueRentals(object sender, RoutedEventArgs e)
        {

            SakilaBinder binder = new SakilaBinder(tConnectionString.Text);
            explorer.DataContext = binder.GetOverdueTitles(tConnectionString.Text).DefaultView;
        }
    }
}
