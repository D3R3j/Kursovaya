using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
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

namespace Kursovaya
{
    /// <summary>
    /// Логика взаимодействия для RegAuthPage.xaml
    /// </summary>

        public partial class RegAuthPage : Page
        {
            private string connectionString = "Data Source=users.db;Version=3;";

            public RegAuthPage()
            {
                InitializeComponent();
                LoadRoles();
            }

            // Загрузка ролей в ComboBox
            private void LoadRoles()
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = "SELECT Name FROM Roles";
                        SQLiteCommand command = new SQLiteCommand(query, connection);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RoleComboBox.Items.Add(reader["Name"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}");
                }
            }

            // Хэширование пароля
            private string HashPassword(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    StringBuilder builder = new StringBuilder();
                    foreach (byte b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }
                    return builder.ToString();
                }
            }

            // Авторизация
            private void LoginButton_Click(object sender, RoutedEventArgs e)
            {
                string login = LoginTextBox.Text;
                string password = PasswordBox.Password;

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Введите логин и пароль.");
                    return;
                }

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                        SELECT u.ID, r.Name AS Role 
                        FROM Users u 
                        JOIN Roles r ON u.RoleID = r.ID 
                        WHERE u.Login = @Login AND u.PasswordHash = @PasswordHash;
                    ";
                        SQLiteCommand command = new SQLiteCommand(query, connection);
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["ID"]);
                                string role = reader["Role"].ToString();

                                LogAction(userId, $"Вход в систему ({role})");

                                MessageBox.Show($"Добро пожаловать, {login}! Ваша роль: {role}");

                                
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка авторизации: {ex.Message}");
                }
            }

            // Регистрация
            private void RegisterButton_Click(object sender, RoutedEventArgs e)
            {
                string login = RegisterLoginTextBox.Text;
                string password = RegisterPasswordBox.Password;
                string role = RoleComboBox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
                {
                    MessageBox.Show("Заполните все поля.");
                    return;
                }

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();

                        // Получение RoleID по названию роли
                        string roleIdQuery = "SELECT ID FROM Roles WHERE Name = @RoleName";
                        SQLiteCommand roleIdCommand = new SQLiteCommand(roleIdQuery, connection);
                        roleIdCommand.Parameters.AddWithValue("@RoleName", role);
                        int roleId = Convert.ToInt32(roleIdCommand.ExecuteScalar());

                        // Добавление пользователя
                        string insertQuery = @"
                        INSERT INTO Users (Login, PasswordHash, RoleID) 
                        VALUES (@Login, @PasswordHash, @RoleID);
                    ";
                        SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);
                        insertCommand.Parameters.AddWithValue("@Login", login);
                        insertCommand.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
                        insertCommand.Parameters.AddWithValue("@RoleID", roleId);

                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show("Пользователь успешно зарегистрирован!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка регистрации: {ex.Message}");
                }
            }

            // Логирование действий
            private void LogAction(int userId, string action)
            {
                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        connection.Open();
                        string query = @"
                        INSERT INTO ActionLog (UserID, Action) 
                        VALUES (@UserID, @Action);
                    ";
                        SQLiteCommand command = new SQLiteCommand(query, connection);
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@Action", action);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка логирования: {ex.Message}");
                }
            }
        }
    }

