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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoginHashPassword
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection myConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LoginHashPasswordDB;Integrated Security=True");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (usernameTXT.Text == string.Empty || passwordTXT.Password == string.Empty)
                {
                    MessageBox.Show("Fill in all the fields");
                }
                else if (CheckForUser(usernameTXT.Text))
                {
                    myConnection.Open();

                    string query = "SELECT password, salt FROM [User] Where username = @username";

                    SqlCommand selectCommand = new SqlCommand(query, myConnection);
                    selectCommand.Parameters.AddWithValue("@username", usernameTXT.Text);
                    selectCommand.ExecuteNonQuery();

                    SqlDataReader sReader;

                    sReader = selectCommand.ExecuteReader();

                    while (sReader.Read())
                    {
                        if (sReader["password"].ToString() == Convert.ToBase64String(Hash.HashPasswordWithSalt(passwordTXT.Password, Convert.FromBase64String(sReader["salt"].ToString()))))
                        {
                            MessageBox.Show("Login successful!");
                        }
                        else
                        {
                            MessageBox.Show("Wrong password");
                        }
                    }

                    myConnection.Close();
                }
                else
                {
                    MessageBox.Show("User doesn't exists!");
                }

            }
            catch (Exception error)
            {
                MessageBox.Show("Login error" + error);
            }
        }

        private void RegisterBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (usernameTXT.Text == string.Empty || passwordTXT.Password == string.Empty)
                {
                    MessageBox.Show("Fill in all the fields");
                }
                else if (!CheckForUser(usernameTXT.Text))
                {
                    byte[] salt = Hash.GenerateSalt();

                    byte[] password = Hash.HashPasswordWithSalt(passwordTXT.Password, salt);

                    myConnection.Open();

                    string query = "INSERT INTO [User] VALUES(@username, @password, @salt);";

                    SqlCommand selectCommand = new SqlCommand(query, myConnection);
                    selectCommand.Parameters.AddWithValue("@username", usernameTXT.Text);
                    selectCommand.Parameters.AddWithValue("@password", Convert.ToBase64String(password));
                    selectCommand.Parameters.AddWithValue("@salt", Convert.ToBase64String(salt));
                    selectCommand.ExecuteNonQuery();

                    myConnection.Close();

                    MessageBox.Show("User is made");
                }
                else
                {
                    MessageBox.Show("User already exists!");
                }

            }
            catch (Exception error)
            {
                MessageBox.Show("Errors during creation" + error);
            }
        }

        private bool CheckForUser(string username)
        {
            myConnection.Open();

            string query = "SELECT username FROM [User];";

            SqlCommand selectCommand = new SqlCommand(query, myConnection);
            selectCommand.ExecuteNonQuery();

            SqlDataReader sReader;

            sReader = selectCommand.ExecuteReader();

            while (sReader.Read())
            {
                if (sReader["username"].ToString() == username)
                {
                    myConnection.Close();
                    return true;
                }
            }

            myConnection.Close();

            return false;
        }
    }
}
