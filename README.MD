# Moviezator

Moviezator - небольшой overhead-проект для ведения списка фильмов. Домен простой, но проект намеренно разнесён по слоям
и инфраструктуре: API, application layer, contracts, persistence, миграционный сервис, Aspire AppHost и отдельный
Angular frontend.

## Компоненты

- `backend/src/Host` - ASP.NET Core host приложения.
- `backend/src/Api` - контроллеры и HTTP-слой.
- `backend/src/Application` - CQRS handlers, validation, сценарии приложения.
- `backend/src/Core` - доменные сущности, value objects и enum-ы.
- `backend/src/Core.Abstractions` - абстракции репозиториев.
- `backend/src/Contracts` - request/response DTO.
- `backend/src/Persistence` - EF Core `DbContext`, конфигурации, репозитории и миграции.
- `backend/src/MigrationService` - отдельный сервис применения EF Core миграций.
- `backend/src/AspireHost` - Aspire orchestration для Postgres, миграций, API и frontend.
- `frontend` - простой Angular UI в виде редактируемой таблицы.

## Запуск

Запуск через Aspire:

```powershell
dotnet run --project backend/src/AspireHost/AspireHost.csproj
```

Aspire поднимает:

- PostgreSQL;
- `MigrationService`, который применяет миграции и завершает работу;
- backend `Host`, который ждёт успешного завершения миграций;
- Angular frontend на `http://127.0.0.1:5001/`.

Frontend проксирует запросы `/api` в backend. При запуске через Aspire адрес backend передаётся через `API_TARGET`. При
ручном запуске frontend использует fallback `http://localhost:5000`.

## Ручной Запуск

Backend:

```powershell
dotnet run --project backend/src/Host/Host.csproj
```

Frontend:

```powershell
cd frontend
npm install
npm start
```

По умолчанию frontend запускается на `http://127.0.0.1:5001/`.

## Миграции

Миграции лежат в `backend/src/Persistence/Migrations`.

Создание миграции:

```powershell
dotnet ef migrations add <Name> --project backend/src/Persistence --startup-project backend/src/Host --context AppDbContext
```

Ручное применение миграций:

```powershell
dotnet ef database update --project backend/src/Persistence --startup-project backend/src/Host --context AppDbContext
```

В обычном Aspire-сценарии миграции применяет `MigrationService`, а не основной web host.

## Проверка

Сборка backend:

```powershell
dotnet build Moviezator.slnx
```

Сборка frontend:

```powershell
cd frontend
npm run build
```
