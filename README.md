# Task Manager (WPF + MVVM)

Десктоп-приложение для управления задачами на WPF с использованием MVVM.

## Требования

- Windows
- .NET SDK 9.0+ (подойдёт и 8.0, но проект создан под `net9.0-windows`)
- Visual Studio 2022 / JetBrains Rider / VS Code + C# Dev Kit (по желанию)

## Запуск

### Вариант 1: через Visual Studio

1. Откройте `TaskManagerWpf/TaskManagerWpf.sln`
2. Установите проект `TaskManagerWpf.App` как Startup Project
3. Запустите (`F5`)

### Вариант 2: через командную строку

Из папки `TaskManagerWpf`:

```bash
dotnet restore
dotnet run --project .\TaskManagerWpf.App\TaskManagerWpf.App.csproj
```

## Где хранится JSON

Файл создаётся и обновляется автоматически:

- `%LocalAppData%\TaskManagerWpf\tasks.json`

## Функционал

- CRUD задач
  - добавление/редактирование в модальном диалоге
  - удаление с подтверждением
- смена статуса Active/Completed
- фильтр: Все / Активные / Завершённые
- поиск (в реальном времени) по Title и Description
- сортировка: CreatedAt / Priority / DueDate
- визуальное выделение просроченных задач (Active + DueDate < Today)
- валидация:
  - Title обязателен
  - при создании DueDate не может быть в прошлом
- обработка ошибок чтения/записи JSON (сообщение пользователю)

## Архитектура (MVVM)

- **Model**: `TaskManagerWpf.App/Models/TaskItem.cs`
- **ViewModels**:
  - `MainViewModel` (коллекция, `ICollectionView`, команды, автосохранение)
  - `TaskEditorViewModel` (валидация + Save/Cancel)
- **Views**:
  - `MainWindow.xaml`
  - `Views/TaskEditorWindow.xaml` (модальный диалог)
- **Services**:
  - `JsonTaskStore` (async load/save)
  - `DialogService` (открытие диалога из VM)
  - `MessageBoxMessageService` (подтверждения/ошибки)
- **Converters** (минимум 2):
  - `PriorityToBrushConverter`
  - `NullableDateToStringConverter`
  - `OverdueToBrushConverter`

## Тесты

Проект тестов: `TaskManagerWpf.Tests` (xUnit).

### Запуск тестов

Из корня репозитория:

```bash
dotnet test
```

Или точечно:

```bash
dotnet test .\TaskManagerWpf.Tests\TaskManagerWpf.Tests.csproj
```

### Что покрыто тестами

- **`TaskEditorViewModel`**: валидация и нормализация данных (обязательный `Title`, правила для `DueDate`, `BuildResult()`).
- **`MainViewModel`**: поиск/фильтрация/сортировка через `ICollectionView`.
  - Часть тестов запускается через `StaTest.Run(...)`, т.к. WPF-коллекции/`CollectionView` ожидают STA-thread.
- **`JsonTaskStore`**: seed-сценарий сохранения/загрузки всех статусов/приоритетов и проверка, что кириллица пишется в JSON без `\uXXXX`-экранирования.

### Test doubles

`TaskManagerWpf.Tests/TestDoubles` — тестовые реализации интерфейсов приложения, чтобы тесты были быстрыми и детерминированными:

- `InMemoryTaskStore`: `ITaskStore` в памяти (без файловой системы).
- `StubDialogService`: `IDialogService` с настраиваемым ответом (возврат результата редактирования/создания).
- `RecordingMessageService`: `IMessageService`, который записывает показанные сообщения/ошибки и управляет ответом `Confirm(...)`.

