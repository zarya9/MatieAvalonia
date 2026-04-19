# Диаграммы (блок 1 ТЗ)

Исходный формат — [Mermaid](https://mermaid.js.org/). Рендер: GitHub, VS Code (расширение Mermaid), Typora и др.

---

## 1. Activity — основной сценарий «клиент: запись на услугу»

```mermaid
flowchart TD
    A([Начало]) --> B[Открыть приложение]
    B --> C{Авторизован?}
    C -->|Нет| D[Ввод логина/пароля]
    D --> E{Успешный вход?}
    E -->|Нет| D
    E -->|Да| F[Главное окно]
    C -->|Да| F
    F --> G[Раздел «Запись»]
    G --> H[Выбор услуги и мастера]
    H --> I[Дата и время]
    I --> J{Слот свободен?}
    J -->|Нет| I
    J -->|Да| K[Сохранить запись]
    K --> L[Список «Мои записи»]
    L --> Z([Конец])
```

---

## 2. Use Case

Акторы снаружи, внутри рамки **ИС Матье** — прецеденты. Стиль как у Activity: сначала объявления узлов, затем связи. Вставка: [mermaid.live](https://mermaid.live) или draw.io → **Insert → Mermaid**.

```mermaid
flowchart TD
    A1((Клиент))
    A2((Модератор))
    A3((Администратор))
    A4((Мастер))
    subgraph SYS["ИС Матье"]
        U1[Вход и регистрация]
        U2[Каталог услуг]
        U3[Запись на услугу]
        U4[Баланс и карта]
        U5[Отзыв]
        U6[Услуги: добавление и изменение]
        U7[Привязка услуги к мастеру]
        U8[Квалификации мастеров]
        U9[Пользователи: просмотр и правка]
        U10[Сотрудники]
        U11[Записи клиентов на мастера]
        U12[Заявка на повышение квалификации]
    end
    A1 --> U1
    A1 --> U2
    A1 --> U3
    A1 --> U4
    A1 --> U5
    A2 --> U6
    A2 --> U7
    A2 --> U8
    A3 --> U9
    A3 --> U10
    A4 --> U11
    A4 --> U12
```

Тот же смысл, **латиница** (если рендер ругается на кириллицу):

```mermaid
flowchart TD
    A1((Client))
    A2((Moderator))
    A3((Admin))
    A4((Master))
    subgraph SYS["IS Matie"]
        U1[Login register]
        U2[Catalog]
        U3[Booking]
        U4[Balance card]
        U5[Review]
        U6[Services CRUD]
        U7[Service master bind]
        U8[Qualifications]
        U9[Users]
        U10[Staff]
        U11[Master clients]
        U12[Qualification request]
    end
    A1 --> U1
    A1 --> U2
    A1 --> U3
    A1 --> U4
    A1 --> U5
    A2 --> U6
    A2 --> U7
    A2 --> U8
    A3 --> U9
    A3 --> U10
    A4 --> U11
    A4 --> U12
```

---

## 3. ER-диаграмма (основные сущности)

Стиль как у Activity / Use Case: сначала узлы сущностей, затем связи (направление: от зависимой строки к справочнику / «родителю»). Вставка: [mermaid.live](https://mermaid.live) или draw.io → **Insert → Mermaid**.

```mermaid
flowchart TD
    USER[USER]
    ROLE[ROLE]
    SERVICE[SERVICE]
    COLLECTION[COLLECTION]
    BOOKING[BOOKING]
    BOOKINGSTATUS[BOOKINGSTATUS]
    CARDUSER[CARDUSER]
    REVIEW[REVIEW]
    MASTER[MASTER]
    QUALIFICATION[QUALIFICATION]
    REQUEST[REQUEST]
    REQUESTSTATUS[REQUESTSTATUS]
    USER -->|N:1| ROLE
    BOOKING -->|клиент N:1| USER
    BOOKING -->|мастер N:1| MASTER
    CARDUSER -->|N:1| USER
    SERVICE -->|N:1 коллекция| COLLECTION
    BOOKING -->|N:1 услуга| SERVICE
    BOOKING -->|N:1 статус| BOOKINGSTATUS
    REVIEW -->|N:1 автор| USER
    REVIEW -->|N:1 услуга| SERVICE
    REVIEW -->|N:1 мастер| MASTER
    MASTER -->|N:1 профиль| USER
    MASTER -->|N:1| QUALIFICATION
    REQUEST -->|N:1 заявитель| USER
    REQUEST -->|N:1 квалификация| QUALIFICATION
    REQUEST -->|N:1 статус| REQUESTSTATUS
```

Тот же смысл, **латиница**:

```mermaid
flowchart TD
    USER[USER]
    ROLE[ROLE]
    SERVICE[SERVICE]
    COLLECTION[COLLECTION]
    BOOKING[BOOKING]
    BOOKINGSTATUS[BOOKINGSTATUS]
    CARDUSER[CARDUSER]
    REVIEW[REVIEW]
    MASTER[MASTER]
    QUALIFICATION[QUALIFICATION]
    REQUEST[REQUEST]
    REQUESTSTATUS[REQUESTSTATUS]
    USER -->|N:1| ROLE
    BOOKING -->|client N:1| USER
    BOOKING -->|master N:1| MASTER
    CARDUSER -->|N:1| USER
    SERVICE -->|N:1 collection| COLLECTION
    BOOKING -->|N:1 service| SERVICE
    BOOKING -->|N:1 status| BOOKINGSTATUS
    REVIEW -->|N:1 author| USER
    REVIEW -->|N:1 service| SERVICE
    REVIEW -->|N:1 master| MASTER
    MASTER -->|N:1 profile| USER
    MASTER -->|N:1| QUALIFICATION
    REQUEST -->|N:1 applicant| USER
    REQUEST -->|N:1 qualification| QUALIFICATION
    REQUEST -->|N:1 status| REQUESTSTATUS
```

**Альтернатива** — встроенная нотация Mermaid `erDiagram` (кардинальность `||--o{` и т.д.):

```mermaid
erDiagram
    USER ||--o{ BOOKING : "клиент"
    MASTER ||--o{ BOOKING : "мастер"
    USER ||--o{ CARDUSER : "карты"
    USER }o--|| ROLE : "роль"
    SERVICE }o--|| COLLECTION : "коллекция"
    BOOKING }o--|| SERVICE : "услуга"
    BOOKING }o--|| BOOKINGSTATUS : "статус"
    REVIEW }o--|| USER : "автор"
    REVIEW }o--|| SERVICE : "услуга"
    REVIEW }o--|| MASTER : "мастер"
    MASTER }o--|| USER : "профиль"
    MASTER }o--|| QUALIFICATION : "квалификация"
    REQUEST }o--|| USER : "заявитель"
    REQUEST }o--|| QUALIFICATION : "квалификация"
    REQUEST }o--|| REQUESTSTATUS : "статус"
```

---

## 4. DFD (уровень 0 и фрагмент уровня 1)

Стиль как у Use Case: `flowchart TD`, сначала узлы, затем связи. Вставка: [mermaid.live](https://mermaid.live) или draw.io → **Insert → Mermaid**.

**DFD уровень 0 (контекст):**

```mermaid
flowchart TD
    P0[0. ИС Матье]
    A1((Клиент))
    A2((Модератор))
    A3((Администратор))
    A4((Мастер))
    DB[(D1. PostgreSQL)]
    A1 <-->|запросы UI| P0
    A2 <-->|запросы UI| P0
    A3 <-->|запросы UI| P0
    A4 <-->|запросы UI| P0
    P0 <-->|SQL| DB
```

**DFD уровень 0, латиница:**

```mermaid
flowchart TD
    P0[0. IS Matie]
    A1((Client))
    A2((Moderator))
    A3((Admin))
    A4((Master))
    DB[(D1. PostgreSQL)]
    A1 <-->|UI| P0
    A2 <-->|UI| P0
    A3 <-->|UI| P0
    A4 <-->|UI| P0
    P0 <-->|SQL| DB
```

**Фрагмент DFD уровня 1 (процессы и хранилище):**

```mermaid
flowchart TD
    P1[1. Авторизация]
    P2[2. Справочники и каталог]
    P3[3. Записи и баланс]
    D[(D1. PostgreSQL)]
    P1 -->|SQL| D
    P2 -->|SQL| D
    P3 -->|SQL| D
```

**Тот же фрагмент, латиница:**

```mermaid
flowchart TD
    P1[1. Auth]
    P2[2. Catalog]
    P3[3. Bookings balance]
    D[(D1. PostgreSQL)]
    P1 -->|SQL| D
    P2 -->|SQL| D
    P3 -->|SQL| D
```

---

## 5. Диаграмма классов

Полная UML-структура в нотации Mermaid `classDiagram`: **имена классов по-русски**, поля и методы — **на английском** (как в учебном примере). Обобщение ролей (`<|--`) — логическая модель; в БД все роли хранятся в одной таблице `User` с полем `roleId`. Агрегация — `o--` (ромб у владельца). Вставка: [mermaid.live](https://mermaid.live) или draw.io → **Insert → Mermaid**.

### 5.1. Роли пользователей и связи с сущностями предметной области

```mermaid
classDiagram
    direction TB

    class Пользователь {
        <<abstract>>
        +idUser: int
        +fname: string
        +name: string
        +patronymic: string
        +login: string
        +password: string
        +updatedAt: datetime
        +verifyLogin(): bool
    }

    class Роль {
        +idRole: int
        +name: string
    }

    class Клиент {
        +balance: decimal
        +register(): void
        +login(): void
        +bookService(): void
        +leaveReview(): void
        +topUpBalance(): void
    }

    class Модератор {
        +editService(): void
        +bindMasterToService(): void
        +editQualifications(): void
    }

    class Администратор {
        +listUsers(): void
        +editUser(): void
        +manageStaff(): void
    }

    class Мастер {
        +viewClientBookings(): void
        +requestQualificationUpgrade(): void
    }

    class ПрофильМастера {
        +idMaster: int
        +userId: int
        +qualifId: int
    }

    class Квалификация {
        +idQualif: int
        +name: string
        +index: int
    }

    class КоллекцияУслуг {
        +idCollection: int
        +name: string
    }

    class Услуга {
        +idService: int
        +collectionId: int
        +name: string
        +description: string
        +price: decimal
        +updatedAt: datetime
        +imgPath: string
    }

    class Запись {
        +idBooking: int
        +userId: int
        +masterId: int
        +serviceId: int
        +dateTime: datetime
        +statusId: int
        +updatedAt: datetime
        +typeId: int
    }

    class СтатусЗаписи {
        +idBookingStatus: int
        +name: string
    }

    class Отзыв {
        +idReview: int
        +userId: int
        +serviceId: int
        +masterId: int
        +rating: int
        +comment: string
        +createdAt: datetime
    }

    class БанковскаяКарта {
        +idCard: int
        +userId: int
        +numberCard: string
        +dateCard: datetime
        +cvv: string
        +isPriority: bool
    }

    class Заявка {
        +idRequest: int
        +userId: int
        +quialifId: int
        +statusId: int
        +reviewedAt: datetime
        +createdAt: datetime
    }

    class СтатусЗаявки {
        +idStatus: int
        +name: string
    }

    Пользователь <|-- Клиент
    Пользователь <|-- Модератор
    Пользователь <|-- Администратор
    Пользователь <|-- Мастер
    Пользователь "n" --> "1" Роль : roleId

    Мастер "1" o-- "1" ПрофильМастера : профиль
    ПрофильМастера "n" --> "1" Квалификация

    Клиент "1" o-- "*" Запись : клиент
    ПрофильМастера "1" o-- "*" Запись : мастер
    Услуга "1" o-- "*" Запись
    Запись "n" --> "1" СтатусЗаписи

    КоллекцияУслуг "1" o-- "*" Услуга

    Клиент "1" o-- "*" БанковскаяКарта

    Клиент "1" o-- "*" Отзыв : автор
    Услуга "1" o-- "*" Отзыв
    ПрофильМастера "1" o-- "*" Отзыв

    Пользователь "1" o-- "*" Заявка
    Заявка "n" --> "1" Квалификация
    Заявка "n" --> "1" СтатусЗаявки
```

### 5.2. Доступ к данным, утилита и окно приложения

```mermaid
classDiagram
    direction TB

    class AppDbContext {
        +Bookings: DbSet
        +BookingStatuses: DbSet
        +CardUsers: DbSet
        +Collections: DbSet
        +Masters: DbSet
        +Qualifications: DbSet
        +Requests: DbSet
        +RequestStatuses: DbSet
        +Reviews: DbSet
        +Roles: DbSet
        +Services: DbSet
        +Users: DbSet
        +SaveChanges(): int
    }

    class DbTimestamp {
        <<utility>>
        +Now: datetime
        +LocalClock: Func
        +UseSystemLocalClock(): void
    }

    class MainWindow {
        +NavigateToCatalog(): void
        +NavigateTo(page): void
    }

    class ServicesCatalogPage {
        +loadServices(): void
    }

    class BookingCreatePage {
        +saveBooking(): void
    }

    MainWindow --> ServicesCatalogPage : навигация
    MainWindow --> BookingCreatePage : навигация
    ServicesCatalogPage ..> AppDbContext : EF запросы
    BookingCreatePage ..> AppDbContext : EF запросы
    BookingCreatePage ..> DbTimestamp : метка времени
```

*Примечание: для защиты экспортируйте PNG из Mermaid или перерисуйте в Visio / Draw.io. Если draw.io «режет» большую `classDiagram`, вставляйте блоки 5.1 и 5.2 по отдельности.*
