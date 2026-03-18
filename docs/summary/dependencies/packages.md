# 📦 Dependencies Summary

## App (`TaskManagerWpf.App`)

- **`CommunityToolkit.Mvvm` (8.4.0)**
  - `[ObservableProperty]`, `[RelayCommand]`, базовые MVVM-утилиты
  - цель: меньше бойлерплейта, предсказуемые VM

- **`MahApps.Metro.IconPacks` (6.2.1)**
  - набор иконок для WPF
  - цель: быстрый UI без ручного импорта SVG/PNG

## Tests (`TaskManagerWpf.Tests`)

- **`xunit` / `xunit.runner.visualstudio`**
  - тестовый фреймворк и интеграция с VS/Rider
- **`Microsoft.NET.Test.Sdk`**
  - инфраструктура запуска тестов
- **`coverlet.collector`**
  - сбор покрытия при запуске тестов

## Гайдлайны добавления зависимостей
- Сначала подумай: можно ли сделать это на чистом .NET/WPF?
- Если нужен пакет:
  - добавляй только в тот проект, где он реально нужен
  - фиксируй причину в описании PR/коммита (зачем пакет)
  - предпочтительно популярные/поддерживаемые библиотеки
