using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace Kursovaya
{
    /// <summary>
    /// Логика взаимодействия для AddProductWindow.xaml
    /// </summary>
        public partial class AddProductWindow : Window
        {
            private string connectionString = "Data Source=C:\\Users\\iluxa\\source\\repos\\Kursovaya\\DataBase.db;Version=3;";

            public AddProductWindow()
            {
                InitializeComponent();
            }

            private void SaveProduct_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Products (Name, CategoryID, Quantity, Price, ExpiryDate) VALUES (@Name, @CategoryID, @Quantity, @Price, @ExpiryDate)";
                        SQLiteCommand command = new SQLiteCommand(query, connection);
                        command.Parameters.AddWithValue("@Name", NameTextBox.Text);
                        command.Parameters.AddWithValue("@CategoryID", int.Parse(CategoryIDTextBox.Text));
                        command.Parameters.AddWithValue("@Quantity", int.Parse(QuantityTextBox.Text));
                        command.Parameters.AddWithValue("@Price", double.Parse(PriceTextBox.Text));
                        command.Parameters.AddWithValue("@ExpiryDate", ExpiryDateTextBox.Text);

                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Товар успешно добавлен!");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления товара: {ex.Message}");
                }
            }
        }
    }

