using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BarberShop
{
    /// <summary>
    /// Interaction logic for AdministrationPanel.xaml
    /// </summary>
    public partial class AdministrationPanel : Window
    {
        public AdministrationPanel()
        {
            InitializeComponent();
            bindVisitsGrid();
        }
        private void bindVisitsGrid()
        {
            SqlConnection sqlCon = new SqlConnection();
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["Reservations"].ConnectionString;
            sqlCon.Open();
            string username = LoginScreen.name;
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @"SELECT CONCAT(K.Imie,' ',K.Nazwisko) AS ImieNazwisko, DataCzas from Rezerwacje AS Res
                                INNER JOIN Pracownicy AS Prac
                                ON Prac.PESEL = Res.PracownikID
                                INNER JOIN Klienci AS K
                                ON K.PESEL = Res.KlientID
                                WHERE Prac.Nickname = 'test' AND DataCzas > CURRENT_TIMESTAMP;";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Connection = sqlCon;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("Rezerwacje");
            adapter.Fill(dt);
            UpcomingVis.ItemsSource = dt.DefaultView;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
