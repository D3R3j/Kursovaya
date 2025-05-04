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
    /// Логика взаимодействия для ChangeStatusWindow.xaml
    /// </summary>
    public partial class ChangeStatusWindow : Window
    {
        private string connectionString = "Data Source=C:\\Users\\iluxa\\source\\repos\\Kursovaya\\DataBase.db;Version=3;";
        private int productId;

        public ChangeStatusWindow(int id)
        {
            InitializeComponent();
            productId = id;
            StatusComboBox.ItemsSource = new List<string> { "Заказано", "На складе", "Доставляют" };
        }

        private void SaveStatus_Click(object sender, RoutedEventArgs e)
        {
            string status = StatusComboBox.SelectedItem.ToString();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Проверяем, существует ли запись в таблице Inventory для данного товара
                    string checkQuery = "SELECT COUNT(*) FROM Inventory WHERE ID = @ID";
                    SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@ID", productId);

                    int recordCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (recordCount > 0)
                    {
                        // Если запись существует, обновляем статус
                        string updateQuery = "UPDATE Inventory SET Status = @Status WHERE ID = @ID";
                        SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@Status", status);
                        updateCommand.Parameters.AddWithValue("@ID", productId);

                        updateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // Если записи нет, добавляем новую
                        string insertQuery = @"
                INSERT INTO Inventory (ResponsibleID, Status, Date) 
                VALUES (@ID, @Status, DATE('now'));
            ";
                        SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@ID", productId);
                        insertCommand.Parameters.AddWithValue("@Status", status);
                        insertCommand.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Статус товара успешно изменен/добавлен!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения/добавления статуса: {ex.Message}");
            }
        }
    }
}
