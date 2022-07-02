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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
           
            SqlConnection sqlCon = new SqlConnection(@"Data Source=(local); Initial Catalog = Barber; Integrated Security = True;");

            try
            {

                var isNumeric = ulong.TryParse(Pesel.Text, out ulong n);


                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                String userAdd = "INSERT INTO Klienci VALUES (@pesel,@Username,@Password,@Firstname,@Lastname);";
                SqlCommand userReg = new SqlCommand(userAdd, sqlCon);

                    userReg.CommandType = CommandType.Text;

                if (Firstname.Text.Length < 2)
                    MessageBox.Show("Name is too short");
                else
                    userReg.Parameters.AddWithValue("@Firstname", Firstname.Text);

                if (Lastname.Text.Length < 2)
                    MessageBox.Show("Last name is too short");
                else
                    userReg.Parameters.AddWithValue("@Lastname", Lastname.Text);

                if (Pesel.Text.Length != 11 || !isNumeric)
                    MessageBox.Show("PESEL is incorrect");
                else
                    userReg.Parameters.AddWithValue("@pesel", Pesel.Text);

                if (Username.Text.Length < 3)
                    MessageBox.Show("Username is too short");
                else
                    userReg.Parameters.AddWithValue("@Username", Username.Text);

                if (Password.Password.Length < 5)
                    MessageBox.Show("Password is too weak");
                else
                    userReg.Parameters.AddWithValue("@Password", Password.Password);


                int count = userReg.ExecuteNonQuery();
                

                if (count == 1)
                {
                    MessageBox.Show("Registration was successful!\nYou can log in your account now.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show(count.ToString());
                }


            }
            catch(Exception)
            {
                MessageBox.Show("Please check all the fields and try again.");
            }
            finally
            {
                sqlCon.Close();
            }

        }
    }
}
