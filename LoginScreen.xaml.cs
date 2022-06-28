using System;
using System.Collections.Generic;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(local); Initial Catalog = Barber; Integrated Security = True;");
            try
            {
                if (sqlCon.State == System.Data.ConnectionState.Closed)
                    sqlCon.Open();
                String userCheck = 
                    "SELECT COUNT(1) FROM Klienci WHERE Nickname = @Username AND Haslo = @Password";
                String adminCheck = 
                    "SELECT COUNT(1) FROM Pracownicy WHERE Nickname = @Username AND Haslo = @Password";

                SqlCommand userLogin = new SqlCommand(userCheck, sqlCon);
                userLogin.CommandType = System.Data.CommandType.Text;
                userLogin.Parameters.AddWithValue("@Username", Username.Text);
                userLogin.Parameters.AddWithValue("@Password", Password.Password);

                SqlCommand adminLogin = new SqlCommand(adminCheck, sqlCon);
                adminLogin.CommandType = System.Data.CommandType.Text;
                adminLogin.Parameters.AddWithValue("@Username", Username.Text);
                adminLogin.Parameters.AddWithValue("@Password", Password.Password);


                int uCount = Convert.ToInt32(userLogin.ExecuteScalar());
                int aCount = Convert.ToInt32(adminLogin.ExecuteScalar());
                if(uCount == 1)
                {
                    MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    this.Close();
                }
                else if(aCount == 1)
                {
                    MainWindow dashboard = new MainWindow();
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
    }
}
