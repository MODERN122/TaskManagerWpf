# 🧪 Testing Summary (xUnit + WPF)

## Набор тестов
- `TaskEditorViewModelTests`: валидация и нормализация данных редактора.
- `MainViewModelQueryTests`: поиск/фильтр/сортировка через `ICollectionView`.
- `JsonTaskStoreSeedTests`: сохранение/загрузка JSON и проверка UTF-8 (кириллица без `\uXXXX`).

## STA helper
Если тест трогает WPF view-инфраструктуру (`CollectionView`/`ICollectionView`), он запускается в STA:
- `StaTest.Run(() => { ... })`

Причина: некоторые WPF-компоненты завязаны на apartment state и UI thread.

## Test doubles
Используем `TaskManagerWpf.Tests/MockServices`:
- `InMemoryTaskStore` вместо `JsonTaskStore`
- `StubDialogService` вместо реального окна
- `RecordingMessageService` вместо `MessageBox`

## Команды запуска
Из корня:

```bash
dotnet test
```

Покрытие (coverlet collector) включено как зависимость тестового проекта.
