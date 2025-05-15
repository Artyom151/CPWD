using Terminal.Gui;

namespace CPWD
{
    public class HelpWindow : Window
    {
        public HelpWindow() : base("Помощь - CPWD")
        {
            X = 0;
            Y = 0;
            Width = Dim.Fill();
            Height = Dim.Fill();

            var helpText = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                Text = @"CPWD (C# Package WSL Mod) - Утилита для управления WSL

Горячие клавиши:
F1 - Показать это окно помощи
F2 - Открыть главное меню
F10 - Выход из программы

Основные функции:
1. Управление дистрибутивами WSL:
   - Просмотр списка установленных дистрибутивов
   - Запуск и остановка дистрибутивов
   - Просмотр состояния и версии дистрибутивов

2. Настройки:
   - Настройка параметров WSL
   - Управление сетевыми настройками
   - Настройка интеграции с Windows

Для навигации используйте:
- Стрелки ↑↓ для перемещения по спискам
- Enter для выбора пункта меню
- Tab для переключения между элементами
- Esc для закрытия текущего окна"
            };

            var closeButton = new Button("Закрыть")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(helpText) - 1
            };
            closeButton.Clicked += () => Application.RequestStop();

            Add(helpText);
            Add(closeButton);
        }
    }
} 