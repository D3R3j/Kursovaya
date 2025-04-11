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
            StatusComboBox.ItemsSource = new List<string> { "Active", "Inactive", "Pending" };
        }

        private void SaveStatus_Click(object sender, RoutedEventArgs e)
        {
            string status = StatusComboBox.SelectedItem.ToString();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Inventory SET Status = @Status WHERE ID = @ID";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@ID", productId);

                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Статус товара успешно изменен!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения статуса: {ex.Message}");
            }
        }
    }
}
