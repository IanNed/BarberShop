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
    /// Login Screen; Used to access both user accounts
    /// as well as worker accounts, contains Log in and
    /// Sign Up options; The last one transfers the user
    /// to the sign up page
    /// </summary>
    public partial class LoginScreen : Window
    {
        public static string name;

        private void GetUsername()
        {
            name = Username.Text;
        }

        /// <summary>
        /// Submit_Click method is connected to Log In button
        /// Checks if username and password are correct using
        /// the information in database; Depending on this,
        /// either allows user to access the account or asks
        /// to provide correct data
        /// </summary>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(local); Initial Catalog = Barber; Integrated Security = True;");
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                String userCheck = 
                    "SELECT COUNT(1) FROM Klienci WHERE Nickname = @Username AND Haslo = @Password";
                String adminCheck = 
                    "SELECT COUNT(1) FROM Pracownicy WHERE Nickname = @Username AND Haslo = @Password";

                SqlCommand userLogin = new SqlCommand(userCheck, sqlCon);
                userLogin.CommandType = CommandType.Text;
                userLogin.Parameters.AddWithValue("@Username", Username.Text);
                userLogin.Parameters.AddWithValue("@Password", Password.Password);

                SqlCommand adminLogin = new SqlCommand(adminCheck, sqlCon);
                adminLogin.CommandType = CommandType.Text;
                adminLogin.Parameters.AddWithValue("@Username", Username.Text);
                GetUsername();
                adminLogin.Parameters.AddWithValue("@Password", Password.Password);


                int uCount = Convert.ToInt32(userLogin.ExecuteScalar());
                int aCount = Convert.ToInt32(adminLogin.ExecuteScalar());
                if(uCount == 1)
                {
                    ClientPanel dashboard = new ClientPanel();
                    dashboard.Show();
                    this.Close();
                }
                else if(aCount == 1)
                {
                    AdministrationPanel dashboard = new AdministrationPanel();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Incorrect username or password.");
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
        /// Registration_Click method transfers new user to
        /// the registration window
        /// </summary>
        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            Registration window = new Registration();
            window.Show();
            
        }
    }
}
