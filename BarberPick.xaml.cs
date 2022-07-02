using System;
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
    /// BarberPick Window opens after user provides
    /// correct information about new reservation;
    /// used to check additional info, inspecting the
    /// database for duplicates and completes the reservation process
    /// </summary>
    public partial class BarberPick : Window
    {
        public BarberPick()
        {
            InitializeComponent();
            bindBarber();

        }

        public static string barber;
        /// <summary>
        /// GetBarber Method stores barber shop worker info
        /// selected by the user to complete the reservation
        /// </summary>
        private void GetBarber()
        {
            if (BarberSelect.SelectedValue != null)
            {
                barber = BarberSelect.SelectedValue.ToString();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// SubmitBarber_Click Method is pinned to Submit Barber button;
        /// checks if user selected a barber shop worker, then sends
        /// query to the database to check if there's no other reservation
        /// for date/time/place/worker; afterward finishes the reservation
        /// and inserts it into the database
        /// </summary>
        private void SubmitBarber_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(local); Initial Catalog = Barber; Integrated Security = True;");
            try
            {

                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                GetBarber();
                if (BarberSelect.SelectedValue != null)
                {
                    string Username = LoginScreen.name;
                    string Worker = barber;
                    string Address = ClientPanel.address;
                    string Date = ClientPanel.date;
                    string Time = ClientPanel.time;
                    String resAdd =
                        @"  INSERT INTO Rezerwacje VALUES (
                        (SELECT PESEL FROM Klienci WHERE Nickname = @Username),
                        (SELECT PESEL FROM Pracownicy WHERE CONCAT(Imie, ' ', Nazwisko) = @Barber),
                        (SELECT ID FROM Placowki WHERE CONCAT(Ulica, ' ', Budynek, ', ', Kod) = @Address),
                        CONCAT(@Date, ' ', @Time)); ";

                    String resCheck = @"SELECT COUNT(1) FROM Rezerwacje AS Rez
                                    JOIN Pracownicy AS Prac
                                    ON Rez.PracownikID = Prac.PESEL
                                    JOIN Placowki AS Plac
                                    ON Rez.PlacowkaID = Plac.ID
                                    WHERE CONCAT(Prac.Imie, ' ', Prac.Nazwisko) = @Barber AND 
                                    CONCAT(Plac.Ulica, ' ', Plac.Budynek, ', ', Plac.Kod) = @Address AND
                                    Rez.DataCzas = CONCAT(@Date, ' ', @Time);";

                    SqlCommand NewReservation = new SqlCommand(resAdd, sqlCon);
                    NewReservation.CommandType = CommandType.Text;
                    NewReservation.Parameters.AddWithValue("@Username", Username);
                    NewReservation.Parameters.AddWithValue("@Barber", Worker);
                    NewReservation.Parameters.AddWithValue("@Address", Address);
                    NewReservation.Parameters.AddWithValue("@Date", Date);
                    NewReservation.Parameters.AddWithValue("@Time", Time);

                    SqlCommand ReservationCheck = new SqlCommand(resCheck, sqlCon);
                    ReservationCheck.CommandType = CommandType.Text;
                    ReservationCheck.Parameters.AddWithValue("@Barber", Worker);
                    ReservationCheck.Parameters.AddWithValue("@Address", Address);
                    ReservationCheck.Parameters.AddWithValue("@Date", Date);
                    ReservationCheck.Parameters.AddWithValue("@Time", Time);



                    int count = Convert.ToInt32(ReservationCheck.ExecuteScalar());

                    if (count == 0 && BarberSelect.SelectedValue != null)
                    {
                        NewReservation.ExecuteNonQuery();
                        MessageBox.Show("Reservation went successfully");
                        ClientPanel window = new ClientPanel();
                        window.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("There's another reservation set on this time");
                    }
                }
                else
                {
                    MessageBox.Show("You didn't pick the barber.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }
        /// <summary>
        /// bindBarber Method is used to check, which barbers
        /// work in the selected shop, sending a query to the database,
        /// then displays their First and Last names to the user using
        /// a Combobox
        /// </summary>
        private void bindBarber()
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(local); Initial Catalog = Barber; Integrated Security = True;");
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                string Query = @"SELECT CONCAT(Prac.Imie,' ', Prac.Nazwisko) FROM Pracownicy AS Prac
                                    INNER JOIN Placowki AS Plac ON Prac.PlacowkaID = Plac.ID
                                    WHERE CONCAT(Plac.Ulica, ' ', Plac.Budynek, ', ', Plac.Kod)= '" + ClientPanel.address + "';";
                SqlCommand cmd = new SqlCommand(Query, sqlCon);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string barber = dr.GetString(0);
                    BarberSelect.Items.Add(barber);
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
        /// Return_Click Method is pinned to the Return button,
        /// allows user to return to the Client Panel window;
        /// </summary>
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            ClientPanel window = new ClientPanel();
            window.Show();
            this.Close();
        }
    }
}