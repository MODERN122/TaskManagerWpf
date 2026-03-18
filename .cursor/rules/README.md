# Cursor Rules (TaskManagerWpf)

Правила лежат в `.cursor/rules/*.mdc` и используются Cursor как “постоянные инструкции” для агента.

## Что внутри

- `core-standards.mdc`: базовые принципы проекта (всегда применяется)
- `csharp-style.mdc`: стиль C# / nullability / ошибки
- `wpf-xaml.mdc`: WPF/XAML паттерны и границы code-behind
- `mvvm-toolkit.mdc`: как использовать `CommunityToolkit.Mvvm`
- `tests.mdc`: xUnit/STA/test doubles

## Рекомендуемый способ просить AI

Примеры:

```
"сделай рефакторинг, следуй cursor rules"
"добавь фичу, не нарушая MVVM границы"
"добавь тесты, учитывай tests.mdc (STA + doubles)"
```
