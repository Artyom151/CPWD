using Terminal.Gui;
using NStack;
using System;

namespace CPWD
{
    public class SettingsWindow : Window
    {
        private readonly WSLManager wslManager;
        private bool isDisposed;

        public SettingsWindow() : base("Настройки - CPWD")
        {
            isDisposed = false;

            try
            {
                wslManager = new WSLManager();

                X = 0;
                Y = 0;
                Width = Dim.Fill();
                Height = Dim.Fill();

                var settingsTab = new TabView()
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill() - 2
                };

                // Вкладка общих настроек
                var generalTab = new TabView.Tab("Общие", CreateGeneralView());
                
                // Вкладка сетевых настроек
                var networkTab = new TabView.Tab("Сеть", CreateNetworkView());
                
                // Вкладка интеграции
                var integrationTab = new TabView.Tab("Интеграция", CreateIntegrationView());

                settingsTab.AddTab(generalTab, true);
                settingsTab.AddTab(networkTab, false);
                settingsTab.AddTab(integrationTab, false);

                var saveButton = new Button(ustring.Make("Сохранить"))
                {
                    X = Pos.Center() - 15,
                    Y = Pos.Bottom(settingsTab)
                };

                var cancelButton = new Button(ustring.Make("Отмена"))
                {
                    X = Pos.Center() + 5,
                    Y = Pos.Bottom(settingsTab)
                };

                saveButton.Clicked += () =>
                {
                    try
                    {
                        MessageBox.Query("Настройки", "Настройки сохранены", "OK");
                        Application.RequestStop();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Ошибка", $"Не удалось сохранить настройки:\n{ex.Message}", "OK");
                    }
                };

                cancelButton.Clicked += () =>
                {
                    Application.RequestStop();
                };

                Add(settingsTab);
                Add(saveButton);
                Add(cancelButton);
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Ошибка", $"Не удалось инициализировать окно настроек:\n{ex.Message}", "OK");
                Application.RequestStop();
            }
        }

        private View CreateGeneralView()
        {
            var frame = new FrameView("Общие настройки")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            var defaultVersionLabel = new Label("Версия WSL по умолчанию:")
            {
                X = 1,
                Y = 1
            };

            var defaultVersionRadio = new RadioGroup(new ustring[] { "WSL 1", "WSL 2" })
            {
                X = Pos.Right(defaultVersionLabel) + 1,
                Y = 1
            };

            defaultVersionRadio.SelectedItem = 1; // WSL 2 по умолчанию
            defaultVersionRadio.SelectedItemChanged += (args) =>
            {
                try
                {
                    var version = defaultVersionRadio.SelectedItem + 1;
                    Task.Run(async () =>
                    {
                        try
                        {
                            await wslManager.SetDefaultVersionAsync(version);
                            Application.MainLoop.Invoke(() =>
                            {
                                MessageBox.Query("Успех", $"Версия WSL по умолчанию изменена на WSL {version}", "OK");
                            });
                        }
                        catch (Exception ex)
                        {
                            Application.MainLoop.Invoke(() =>
                            {
                                MessageBox.ErrorQuery("Ошибка", $"Не удалось изменить версию WSL:\n{ex.Message}", "OK");
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Ошибка", $"Ошибка при обработке изменения версии:\n{ex.Message}", "OK");
                }
            };

            frame.Add(defaultVersionLabel);
            frame.Add(defaultVersionRadio);

            return frame;
        }

        private View CreateNetworkView()
        {
            var frame = new FrameView("Сетевые настройки")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            var portForwardingCheck = new CheckBox("Включить проброс портов")
            {
                X = 1,
                Y = 1
            };

            var networkIntegrationCheck = new CheckBox("Интеграция с сетью Windows")
            {
                X = 1,
                Y = 3
            };

            frame.Add(portForwardingCheck);
            frame.Add(networkIntegrationCheck);

            return frame;
        }

        private View CreateIntegrationView()
        {
            var frame = new FrameView("Настройки интеграции")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            var windowsIntegrationCheck = new CheckBox("Интеграция с Windows")
            {
                X = 1,
                Y = 1
            };

            var autoMountCheck = new CheckBox("Автоматическое монтирование дисков")
            {
                X = 1,
                Y = 3
            };

            frame.Add(windowsIntegrationCheck);
            frame.Add(autoMountCheck);

            return frame;
        }

        public new void Dispose()
        {
            if (!isDisposed)
            {
                try
                {
                    wslManager?.Dispose();
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, но не прерываем процесс закрытия
                    Console.WriteLine($"Ошибка при освобождении ресурсов: {ex.Message}");
                }
                finally
                {
                    isDisposed = true;
                    base.Dispose();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                try
                {
                    wslManager?.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при освобождении ресурсов: {ex.Message}");
                }
                finally
                {
                    isDisposed = true;
                }
            }
            base.Dispose(disposing);
        }
    }
} 