# Лавка «Матье» — клиент Avalonia + PostgreSQL

Десктоп-приложение на **.NET 8** и **Avalonia 11** с доступом к базе **PostgreSQL** через Entity Framework Core.

## Требования

- .NET SDK 8.0+
- PostgreSQL (строка подключения в `MatieAvalonia/Data/AppDbContext.cs` или через конфигурацию при доработке)

## Сборка и запуск

```bash
dotnet build MatieAvalonia.sln
dotnet run --project MatieAvalonia/MatieAvalonia.csproj
```

## Тесты (блок 5 ТЗ)

```bash
dotnet test MatieAvalonia.sln
```

Проект `MatieAvalonia.Tests`: unit-тесты для метки времени последнего изменения (`DbTimestamp`) и вспомогательных правил расписания (`BookingScheduleRules`).

## Документация по курсовой (блоки 1, 3, 5, 6)

| Файл | Содержание |
|------|------------|
| [Docs/DIAGRAMS.md](MatieAvalonia/Docs/DIAGRAMS.md) | Activity, Use Case, ER, DFD, диаграмма классов (Mermaid) |
| [Docs/DEMO_TABULAR_DATA.md](MatieAvalonia/Docs/DEMO_TABULAR_DATA.md) | Пример табличных данных (по 10 строк на таблицу) |
| [Docs/TEST_CASES.md](MatieAvalonia/Docs/TEST_CASES.md) | Шаблон тест-кейсов для ручного тестирования |
| [Docs/MANUAL_AND_SPEC.md](MatieAvalonia/Docs/MANUAL_AND_SPEC.md) | Руководство пользователя и спецификация (единый документ) |

Скрипты SQL: папка `MatieAvalonia/Docs/` (`seed_demo.sql`, `fix_service_imgpath.sql` и др.).

## Фиксация в репозитории (блок 7)

```bash
git init
git add .
git commit -m "Курсовой проект: Лавка Матье"
git remote add origin <URL-вашего-репозитория>
git push -u origin main
```

Убедитесь, что **не коммитите** пароли БД в открытый репозиторий: вынесите строку подключения в `appsettings.json` + `.gitignore` для секретов.

## Функционал приложения (блок 4)

- Списки с **постраничным выводом** и **сортировкой по алфавиту**: каталог услуг, коллекции, пользователи, записи, услуги модератора и др.
- **Добавление и редактирование** записей (услуги, пользователи, запись на приём, карты, отзывы и т.д.).
- **Время последнего изменения** (`Updated_At` / `UpdatedAt`) для пользователей, услуг, записей — через `DbTimestamp.Now`.
