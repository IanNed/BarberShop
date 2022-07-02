using System;
using System.Configuration;
using System.Collections.Generic;
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
    /// Client Panel opens after user accesses
    /// client's account, used to check upcoming
    /// visits, as well as creating new reservations;
    /// </summary>
    public partial class ClientPanel : Window
    {
        public ClientPanel()
        {
            InitializeComponent();
            bindDataGrid();
            bindShop();


        }
        public static string date;
        /// <summary>
        /// GetDate() Method for storing reservation date
        /// information provided by user
        /// </summary>
        private void GetDate()
        {
            if (DaySelect.SelectedDate != null)
            {
                date = DaySelect.SelectedDate.Value.Date.ToShortDateString();
            }
            else
            {
                MessageBox.Show("Please select the date for the reservation");
                return;
            }
        }
        public static string time;
        /// <summary>
        /// GetTime() Method for storing reservation time
        /// information provided by user
        /// </summary>
        private void GetTime()
        {
            if (TimeSelect.SelectedValue != null)
            {
                time = TimeSelect.Text;
            }
            else
            {
                MessageBox.Show("Please select the time for the reservation");
                return;
            }
        }
        public static string address;
        /// <summary>
        /// GetAddress() Method for storing reservation address
        /// information provided by user
        /// </summary>
        private void GetAddress()
        {
            if (PlaceSelect.SelectedValue != null)
            {
                address = PlaceSelect.SelectedValue.ToString();
            }
            else
            {
                MessageBox.Show("Please select the address for the reservation");
                return;
            }
        }
        public List<Pracownicy> Barber { get; set; }
        /// <summary>
        /// bindShop() Method for formatting and displaying
        /// database columns in order to show user all available
        /// barber shop locations
        /// </summary>
        private void bindShop()
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(local); Initial Catalog = Barber; Integrated Security = True;");
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                string Query = "SELECT CONCAT(Ulica, ' ', Budynek, ', ', Kod) AS Adres FROM Placowki;";
                SqlCommand cmd = new SqlCommand(Query, sqlCon);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string place = dr.GetString(0);
                    PlaceSelect.Items.Add(place);
                }
                sqlCon.Close();
            }
            catch
            {
                Exception ex = new Exception();
            }
            finally
            {
                sqlCon.Close();
            }
        }
        /// <summary>
        /// bindDataGrid Method is used to check Rezerwacje table in database
        /// and display data related to upcoming visites of a logged in
        /// using DataGrid table
        /// </summary>
        private void bindDataGrid()
        {
            SqlConnection sqlCon = new SqlConnection();
            sqlCon.ConnectionString = ConfigurationManager.ConnectionStrings["Reservations"].ConnectionString;
            sqlCon.Open();
            string username = LoginScreen.name;
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @"SELECT CONCAT(Prac.Imie,' ',Prac.Nazwisko) AS ImieNazwisko, CONCAT(Plac.Ulica,' ',Plac.Budynek, ', ', Plac.Kod) AS Adres, DataCzas from Rezerwacje AS Res
                                INNER JOIN Pracownicy AS Prac
                                ON Prac.PESEL = Res.PracownikID
                                INNER JOIN Placowki AS Plac
                                ON Plac.ID = Res.PlacowkaID
                                INNER JOIN Klienci AS K
                                ON K.PESEL = Res.KlientID
                                WHERE K.Nickname = @username AND DataCzas > CURRENT_TIMESTAMP;";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Connection = sqlCon;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("Rezerwacje");
            adapter.Fill(dt);
            UpcomingRes.ItemsSource = dt.DefaultView;
        }
        /// <summary>
        /// SubmitVisit Method is connected to the "Submit" button;
        /// Checks the information provided by user, then allows
        /// user to check available barbers if the provided information
        /// is correct
        /// </summary>
        private void SubmitVisit(object sender, RoutedEventArgs e)
        {
            if (TimeSelect.SelectedValue != null && DaySelect.SelectedDate != null
                && PlaceSelect.SelectedValue != null && 
                DaySelect.SelectedDate.Value > DateTime.Now) { 
                GetDate();
                GetTime();
                GetAddress();

                BarberPick window = new BarberPick();
                window.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter correct value in every field");
            }
            
        }
        /// <summary>
        /// Logout_Click Method is pinned to "Logout" button,
        /// used to exit account and close an application
        /// </summary>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
