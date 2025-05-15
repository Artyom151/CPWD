using Terminal.Gui;
using NStack;

namespace CPWD
{
    public class DistributionsWindow : Window
    {
        private readonly WSLManager wslManager;
        private readonly ListView distributionList;
        private List<WSLDistribution> distributions;
        private readonly Button startButton;
        private readonly Button stopButton;

        public DistributionsWindow() : base("Дистрибутивы WSL")
        {
            wslManager = new WSLManager();
            distributions = new List<WSLDistribution>();

            // Создаем список дистрибутивов
            distributionList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                AllowsMarking = false,
                CanFocus = true
            };

            // Добавляем кнопки управления
            startButton = new Button(ustring.Make("Запустить"))
            {
                X = 0,
                Y = Pos.Bottom(distributionList),
                Visible = false,
                CanFocus = true
            };

            stopButton = new Button(ustring.Make("Остановить"))
            {
                X = Pos.Right(startButton) + 2,
                Y = Pos.Bottom(distributionList),
                Visible = false,
                CanFocus = true
            };

            var refreshButton = new Button(ustring.Make("Обновить"))
            {
                X = Pos.Right(stopButton) + 2,
                Y = Pos.Bottom(distributionList),
                CanFocus = true
            };

            // Добавляем обработчики событий
            distributionList.SelectedItemChanged += (args) =>
            {
                bool hasSelection = distributionList.SelectedItem >= 0;
                startButton.Visible = hasSelection;
                stopButton.Visible = hasSelection;
            };

            startButton.Clicked += () =>
            {
                if (distributionList.SelectedItem >= 0)
                {
                    var selected = distributions[distributionList.SelectedItem];
                    Task.Run(async () =>
                    {
                        try
                        {
                            await wslManager.StartDistributionAsync(selected.Name);
                            await RefreshDistributions();
                        }
                        catch (Exception ex)
                        {
                            Application.MainLoop.Invoke(() =>
                            {
                                MessageBox.ErrorQuery("Ошибка", $"Не удалось запустить дистрибутив:\n{ex.Message}", "OK");
                            });
                        }
                    });
                }
            };

            stopButton.Clicked += () =>
            {
                if (distributionList.SelectedItem >= 0)
                {
                    var selected = distributions[distributionList.SelectedItem];
                    Task.Run(async () =>
                    {
                        try
                        {
                            await wslManager.StopDistributionAsync(selected.Name);
                            await RefreshDistributions();
                        }
                        catch (Exception ex)
                        {
                            Application.MainLoop.Invoke(() =>
                            {
                                MessageBox.ErrorQuery("Ошибка", $"Не удалось остановить дистрибутив:\n{ex.Message}", "OK");
                            });
                        }
                    });
                }
            };

            refreshButton.Clicked += () =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await RefreshDistributions();
                    }
                    catch (Exception ex)
                    {
                        Application.MainLoop.Invoke(() =>
                        {
                            MessageBox.ErrorQuery("Ошибка", $"Не удалось обновить список дистрибутивов:\n{ex.Message}", "OK");
                        });
                    }
                });
            };

            // Добавляем элементы управления в окно
            Add(distributionList);
            Add(startButton);
            Add(stopButton);
            Add(refreshButton);

            // Загружаем начальные данные
            Task.Run(async () =>
            {
                try
                {
                    await RefreshDistributions();
                }
                catch (Exception ex)
                {
                    Application.MainLoop.Invoke(() =>
                    {
                        MessageBox.ErrorQuery("Ошибка", $"Не удалось загрузить список дистрибутивов:\n{ex.Message}", "OK");
                    });
                }
            });
        }

        private async Task RefreshDistributions()
        {
            distributions = await wslManager.GetDistributionsAsync();
            Application.MainLoop.Invoke(() =>
            {
                var items = distributions.Select(d => ustring.Make($"{d.Name} - {d.State} (версия {d.Version})")).ToList();
                distributionList.SetSource(items);
            });
        }

        public new void Dispose()
        {
            wslManager.Dispose();
            base.Dispose();
        }
    }
} 