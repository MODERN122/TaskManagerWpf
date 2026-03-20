# 📚 Documentation Summaries (WPF/.NET)

Этот каталог содержит тематически организованные summary по проекту **TaskManagerWpf** для удобного использования в промптах.

## 🗂️ Структура документации

### [🏗️ Architecture](./architecture/)

MVVM, слои приложения, зависимости между проектами, правила изменений.

- **Файлы:** структура, границы ответственности, паттерны, naming
- **Использование:** `"учитывай summary по architecture"`

### [🎨 UI (WPF/XAML)](./ui/)

Привязки, команды, converters, ресурсы/стили, code-behind границы.

- **Файлы:** XAML-паттерны, биндинги, threading (UI thread), visual states
- **Использование:** `"учитывай summary по ui"`

### [💾 Storage (JSON)](./storage/)

Хранение задач в JSON, автосохранение, обработка ошибок, кодировки.

- **Файлы:** `JsonTaskStore`, формат данных, seed/инициализация
- **Использование:** `"учитывай summary по storage"`

### [🧪 Testing](./testing/)

Как и что тестируем (xUnit), STA-обёртка, test doubles.

- **Файлы:** тестовая стратегия, `StaTest`, `MockServices`, примеры тестов
- **Использование:** `"учитывай summary по testing"`

### [📦 Dependencies](./dependencies/)

Пакеты, для чего они, как добавлять новые зависимости.

- **Файлы:** `CommunityToolkit.Mvvm`, `MahApps.Metro.IconPacks`, пакеты тестов
- **Использование:** `"учитывай summary по dependencies"`

## 🎯 Быстрые ссылки для промптов

### **Архитектура**

```
"учитывай summary по architecture"
"следуй MVVM границам из architecture summary"
```

### **UI / XAML**

```
"учитывай summary по ui"
"добавь XAML/Binding, следуй паттернам из ui summary"
```

### **Хранилище JSON**

```
"учитывай summary по storage"
"добавь/измени сохранение JSON, следуй storage summary"
```

### **Тесты**

```
"учитывай summary по testing"
"добавь тесты xUnit, используй StaTest и MockServices"
```

### **Зависимости**

```
"учитывай summary по dependencies"
"добавь пакет и объясни зачем, следуя dependencies summary"
```

## 🚀 Как использовать

1. Выберите тему (architecture/ui/storage/testing/dependencies)
2. Укажите в промпте соответствующую фразу `"учитывай summary по ..."`
3. AI подтянет релевантные паттерны, файлы и ограничения проекта

---

**Всего документов:** 5 разделов (папок) + summary-файлы внутри  
**Последнее обновление:** Март 2026 — первичная настройка summary под WPF/.NET
