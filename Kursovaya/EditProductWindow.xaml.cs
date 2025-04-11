using System;
using System.Collections.Generic;
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
using System.Data.SQLite;

namespace Kursovaya
{
    /// <summary>
    /// Логика взаимодействия для EditProductWindow.xaml
    /// </summary>
  
        public partial class EditProductWindow : Window
        {
            private string connectionString = "Data Source=C:\\Users\\iluxa\\source\\repos\\Kursovaya\\DataBase.db;Version=3;";
            private int productId;

            public EditProductWindow(int id)
            {
                InitializeComponent();
                productId = id;
                LoadProductData();
            }

            private void LoadProductData()
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT * FROM Products WHERE ID = @ID";
                        SQLiteCommand command = new SQLiteCommand(query, connection);
                        command.Parameters.AddWithValue("@ID", productId);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                NameTextBox.Text = reader["Name"].ToString();
                                CategoryIDTextBox.Text = reader["CategoryID"].ToString();
                                QuantityTextBox.Text = reader["Quantity"].ToString();
                                PriceTextBox.Text = reader["Price"].ToString();
                                ExpiryDateTextBox.Text = reader["ExpiryDate"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных товара: {ex.Message}");
                }
            }

            private void SaveChanges_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                        UPDATE Products 
                        SET Name = @Name, CategoryID = @CategoryID, Quantity = @Quantity, Price = @Price, ExpiryDate = @ExpiryDate 
                        WHERE ID = @ID";
                        SQLiteCommand command = new SQLiteCommand(query, connection);
                        command.Parameters.AddWithValue("@Name", NameTextBox.Text);
                        command.Parameters.AddWithValue("@CategoryID", int.Parse(CategoryIDTextBox.Text));
                        command.Parameters.AddWithValue("@Quantity", int.Parse(QuantityTextBox.Text));
                        command.Parameters.AddWithValue("@Price", double.Parse(PriceTextBox.Text));
                        command.Parameters.AddWithValue("@ExpiryDate", ExpiryDateTextBox.Text);
                        command.Parameters.AddWithValue("@ID", productId);

                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Товар успешно обновлен!");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления товара: {ex.Message}");
                }
            }
        }
    }

