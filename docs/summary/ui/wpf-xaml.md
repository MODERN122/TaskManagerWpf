# 🎨 UI Summary (WPF/XAML)

## Базовые принципы
- UI строится через **Binding** (двусторонние поля формы, команды кнопок).
- Логику держим в ViewModel; в XAML — декларативное описание.

## Команды и взаимодействия
- Используем `[RelayCommand]` в VM и биндимся к `Command`.
- `CanExecute` логика в VM; после смены `SelectedTask` и т.п. вызываем `NotifyCanExecuteChanged()`.

## Фильтр/поиск/сортировка
- `MainViewModel` использует `ICollectionView` (`TasksView`):
  - `Filter` → поиск и статусный фильтр
  - `SortDescriptions` → сортировка (CreatedAt/Priority/DueDate)
- Для “живого” поиска иногда требуется `TasksView.Refresh()` на смене текста.

## Converters
- Converters должны быть **чистыми** (без IO, без сервисов, без зависимостей).
- Для оформления приоритета/просрочки используем отдельные converters.

## Code-behind границы
Допустимо:
- `InitializeComponent()`, wiring окна/диалога, focus/UX helpers.
Не допускается:
- бизнес-логика, работа с файлами, изменения моделей напрямую.

## Threading
- Любые объекты WPF/`CollectionView` ожидают UI thread.
- Тесты, использующие `CollectionView`, запускаем в STA (`StaTest.Run`).
