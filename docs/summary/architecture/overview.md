# 🏗️ Architecture Summary (TaskManagerWpf)

## Контекст
- Desktop app на **WPF**.
- Архитектура: **MVVM**.
- Цель: простая менеджерилка задач с JSON-хранилищем, фильтрами/поиском/сортировкой и модальным редактором.

## Проекты решения
- `TaskManagerWpf.App`: основное приложение (WPF).
- `TaskManagerWpf.Tests`: тесты xUnit (также `UseWPF=true` для совместимости с WPF инфраструктурой).

## Слои и ответственность
- **Models** (`TaskManagerWpf.App/Models`): DTO/сущности (например, `TaskItem`), без UI/IO.
- **ViewModels** (`TaskManagerWpf.App/ViewModels`):
  - состояние экрана + команды (`RelayCommand`)
  - фильтрация/поиск/сортировка через `ICollectionView`
  - валидация (в `TaskEditorViewModel`)
- **Views** (`*.xaml`): UI, биндинги, стили; минимум code-behind.
- **Services** (`TaskManagerWpf.App/Services`): всё, что взаимодействует с внешним миром (файл, диалоги, message box) — за интерфейсами.

## Инъекция зависимостей (паттерн)
ViewModel принимает интерфейсы:
- `ITaskStore` — загрузка/сохранение задач
- `IDialogService` — открыть модальный редактор задачи
- `IMessageService` — показать ошибки/подтверждения

Это даёт:
- тестируемость без UI/IO (через `TestDoubles`)
- изоляцию WPF от бизнес-логики

## Важные паттерны в `MainViewModel`
- **Hook/Unhook**: подписка на `PropertyChanged` у элементов коллекции для автосохранения и refresh.
- **Debounce save**: сохранение откладывается на ~350ms и отменяется при новых изменениях.
- **Snapshot перед сохранением**: сохраняем `TaskItem` (модели), а не VM.

## Правила изменений (для промптов)
- Не добавляй “умные” зависимости без причины; предпочитай встроенное .NET.
- Изменение поведения = обнови/добавь тест.
- UI не должен знать про файловую систему; это обязанность `ITaskStore`.
