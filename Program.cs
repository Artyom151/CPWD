using Terminal.Gui;
using System.Management.Automation;
using System.Text;
using System.Runtime.InteropServices;

namespace CPWD
{
    public class Program
    {
        private static Window mainWindow = null!;
        private static MenuBar menuBar = null!;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageID);

        [STAThread]
        public static void Main()
        {
            try
            {
                // Устанавливаем кодировку для консоли
                SetConsoleOutputCP(65001); // UTF-8
                SetConsoleCP(65001);
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;

                // Устанавливаем заголовок консоли
                Console.Title = "CPWD - Управление WSL";

                // Инициализируем Terminal.Gui
                Application.Init();
                
                var top = Application.Top;
                mainWindow = new Window("CPWD - Управление WSL")
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill()
                };
                
                top.Add(mainWindow);

                menuBar = new MenuBar(new MenuBarItem[] {
                    new MenuBarItem("_Файл", new MenuItem[] {
                        new MenuItem("_Выход", "", () => Application.RequestStop())
                    }),
                    new MenuBarItem("_WSL", new MenuItem[] {
                        new MenuItem("_Дистрибутивы", "", () => ShowDistributions()),
                        new MenuItem("_Настройки", "", () => ShowSettings())
                    }),
                    new MenuBarItem("_Справка", new MenuItem[] {
                        new MenuItem("_О программе", "", () => ShowAbout()),
                        new MenuItem("_Помощь", "", () => ShowHelp())
                    })
                });

                mainWindow.Add(menuBar);

                var welcomeText = new Label(NStack.ustring.Make("Добро пожаловать в CPWD (C# Package WSL Mod)"))
                {
                    X = Pos.Center(),
                    Y = Pos.Center() - 2,
                };

                var instructionText = new Label(NStack.ustring.Make("Нажмите F1 для помощи или F2 для открытия меню"))
                {
                    X = Pos.Center(),
                    Y = Pos.Center(),
                };

                mainWindow.Add(welcomeText);
                mainWindow.Add(instructionText);

                var statusBar = new StatusBar(new StatusItem[] {
                    new StatusItem(Key.F1, "~F1~ Помощь", () => ShowHelp()),
                    new StatusItem(Key.F2, "~F2~ Меню", () => ActivateMenu()),
                    new StatusItem(Key.F10, "~F10~ Выход", () => Application.RequestStop())
                });

                // Добавляем обработчик клавиш для всего приложения
                Application.Top.KeyPress += (e) =>
                {
                    if (e.KeyEvent.Key == Key.F2)
                    {
                        ActivateMenu();
                        e.Handled = true;
                    }
                };

                top.Add(statusBar);

                Application.Run();
            }
            finally
            {
                Application.Shutdown();
            }
        }

        private static void ActivateMenu()
        {
            menuBar.SetFocus();
            if (menuBar.Menus.Length > 0)
            {
                menuBar.OpenMenu();
            }
        }

        private static void ShowDistributions()
        {
            try
            {
                var distributionsWindow = new DistributionsWindow();
                Application.Run(distributionsWindow);
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Ошибка", $"Не удалось открыть окно дистрибутивов:\n{ex.Message}", "OK");
            }
        }

        private static void ShowSettings()
        {
            try
            {
                var settingsWindow = new SettingsWindow();
                Application.Run(settingsWindow);
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Ошибка", $"Не удалось открыть окно настроек:\n{ex.Message}", "OK");
            }
        }

        private static void ShowHelp()
        {
            try
            {
                var helpWindow = new HelpWindow();
                Application.Run(helpWindow);
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Ошибка", $"Не удалось открыть окно помощи:\n{ex.Message}", "OK");
            }
        }

        private static void ShowAbout()
        {
            MessageBox.Query(
                "О программе",
                "CPWD (C# Package WSL Mod)\nВерсия 1.0.0\n\nУтилита для управления и настройки WSL\nРазработано с использованием Terminal.Gui",
                "OK"
            );
        }
    }
} 