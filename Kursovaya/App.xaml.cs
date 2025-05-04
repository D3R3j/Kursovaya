using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Kursovaya
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем экземпляр окна авторизации
            RegAuthPage regAuthPage = new RegAuthPage();

            // Показываем окно авторизации
            if (regAuthPage.ShowDialog() == true)
            {
                // Если авторизация успешна, показываем главное окно
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                // Если пользователь закрыл окно авторизации, завершаем приложение
                Shutdown();
            }
        }
    }
}
