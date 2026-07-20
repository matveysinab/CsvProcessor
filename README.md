# CsvProcessor
WebAPI для загрузки и обработки CSV файлов с сохранением данных в PostgreSQL.

## как запустить:

### 1. Установить PostgreSQL
- Скачать: https://www.postgresql.org/download/
- При установке "запомни пароль" (например, `123`)

### 2. Создать базу данных
в графическом интерфейсе pgAdmin либо 
```sql
CREATE DATABASE csvdb;
```
### 3. Не забыть изменить пароль на введенный!
"DefaultConnection": "Host=localhost;Database=csvdb;Username=postgres;Password=ВАШ_ПАРОЛЬ"

### API Методы
1. Загрузка CSV
text
POST /api/Csv/upload
Загружает CSV файл, валидирует и сохраняет в БД.

2. Получение результатов
text
GET /api/Csv/results?fileName=test&minDate=2026-01-01
Фильтрация по имени файла, дате, среднему значению и времени.

3. Последние 10 значений
text
GET /api/Csv/values/{fileName}/last
Возвращает последние 10 записей для указанного файла.

Формат CSV
csv
Date;ExecutionTime;Value
2026-01-01T10-00-00.1234Z;5.5;100.5
2026-01-01T10-00-05.1234Z;3.2;200.3

Тестовый файл
Создать test.csv:

csv
2026-01-01T10-00-00.1234Z;5.5;100.5
2026-01-01T10-00-05.1234Z;3.2;200.3
2026-01-01T10-00-10.1234Z;4.1;150.7
Загрузить его через Swagger

Используемые технологии
.NET 8

ASP.NET Core WebAPI

Entity Framework Core

PostgreSQL

Swagger/OpenAPI

Зависимости
bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
