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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;


namespace Kursovaya
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=C:\\Users\\iluxa\\source\\repos\\Kursovaya\\DataBase.db;Version=3;";
        private int selectedProductId = -1;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }


        private void LoadData()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            p.ID AS ID,
                            p.Name AS Name,
                            c.Name AS CategoryName,
                            p.Quantity,
                            p.Price,
                            p.ExpiryDate,
                            i.Status
                        FROM Products p
                        LEFT JOIN Inventory i ON p.ID = i.ResponsibleID 
                        LEFT JOIN Categories c ON p.CategoryID = c.ID;
                    ";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);

                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);
                    ProductsDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow();
            addProductWindow.ShowDialog();
            LoadData();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Выберите товар для редактирования.");
                return;
            }

            EditProductWindow editProductWindow = new EditProductWindow(selectedProductId);
            editProductWindow.ShowDialog();
            LoadData();
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Выберите товар для удаления.");
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Products WHERE ID = @ID";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", selectedProductId);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Товар успешно удален.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления товара: {ex.Message}");
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show("Выберите товар для изменения состояния.");
                return;
            }

            ChangeStatusWindow changeStatusWindow = new ChangeStatusWindow(selectedProductId);
            changeStatusWindow.ShowDialog();
            LoadData();
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem != null)
            {
                var selectedItem = ProductsDataGrid.SelectedItem as System.Data.DataRowView;
                if (selectedItem != null)
                {
                    selectedProductId = Convert.ToInt32(selectedItem["ProductID"]);
                }
            }
        }
    }
}
