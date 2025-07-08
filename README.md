# CitizenAppealsApp

Клиент-серверное приложение для работы с обращениями граждан.

## Архитектура
- **Server**: ASP.NET Core Web API (.NET 7), Entity Framework Core, JWT, AutoMapper.
  - Controllers: `AuthController` (аутентификация), `AppealsController` (управление обращениями).
  - Models: модели базы данных (EF Core SQL Server).
- **Client**: WPF-приложение (.NET 7), MVVM.
  - Views: `LoginWindow` (вход), `MainWindow` (список обращений), `ReportDialog` (выбор дат для отчета).
  - ViewModel: `LoginViewModel`, `MainViewModel`, `ReportDialogViewModel` с привязками и командами.
- **Shared**:
  - DTO (`AppealDto`, `LoginDto`, `TokenDto`, `UpdateCheckResultDto`, `ReportRequestDto`).
- **База данных**: Ms SQL Server с таблицами `Roles`, `Users`, `Citizens`, `Executors`, `Appeals`, `AppealExecutors`.

## Настройка и запуск

### Требования
- .NET 7 SDK
- SQL Server
- Ide (Rider, Visual Studio)
- Git

### 1. Клонирование репозитория
```bash
git clone https://github.com/nxtvrtur/CitizenAppeals.git
```
### 2. Создание Базы Данных
  1. Создайте базу данных CitizenAppeals
  2. Выполните скрипт db.sql, выбрав созданную БД.
  3. В файле CitizenAppeals.Server/appsettings.json замените строку подключения:
  ```bash
  "DefaultConnection": "Server=DESKTOP-OD5ORN5\\SQLEXPRESS;Database=CitizenAppeals;User Id=sa;Password=123;Encrypt=False;"
  ```
  На свою: 
  ```bash
  "Server=[Имя_сервера];Database=CitizenAppeals;User Id=[Имя_пользователя];Password=[Пароль];Encrypt=False;"
  ```
### 3. Запуск и тестирование
  1. Если некоторых зависимостей нет, выполните команду:
```bash
dotnet restore
``` 
  2. Запустите сервер
 ```bash
cd CitizenAppeals.Server
dotnet run
```
Также сервер можно запустить через IDE, выбрав CitizenAppeals.Server (https) как запускаемый проект.
Также можно протестировать отдельно сервер, используя Swagger или Postman.
Для этого в файле `CitizenAppeals.Server/Properties/launchSettings.json` измените параметр "launchBrowser" на `true` и запустите сервер.
3. Запустите клиент аналогичным спобом или через IDE
4. Протестируйте функционал

## Тестовые данные
1. Учетная запись с ролью admin
- Логин: `admin`
- Пароль: `admin`
3. Учетная запись с ролью user
- Логин: `user`
- Пароль: `user`
