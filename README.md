markdown
# CsvProcessor

WebAPI для загрузки и обработки CSV файлов с сохранением данных в PostgreSQL.

---

## Как запустить

### 1. Установить PostgreSQL

Скачать: https://www.postgresql.org/download/

При установке запомни пароль (например, 123).

### 2. Создать базу данных

В графическом интерфейсе pgAdmin либо выполнить в командной строке:

```sql
CREATE DATABASE csvdb;
```
3. Настроить подключение
Открыть файл appsettings.json и изменить пароль на тот, который был введен при установке PostgreSQL:

json
"DefaultConnection": "Host=localhost;Database=csvdb;Username=postgres;Password=ВАШ_ПАРОЛЬ"
4. Запустить проект
bash
dotnet restore
dotnet run
5. Открыть Swagger
Перейти по адресу:

text
https://localhost:5001/swagger
API Методы
Метод	URL	Описание
POST	/api/Csv/upload	Загружает CSV файл, валидирует и сохраняет в БД
GET	/api/Csv/results?fileName=test&minDate=2026-01-01	Фильтрация по имени файла, дате, среднему значению и времени
GET	/api/Csv/values/{fileName}/last	Возвращает последние 10 записей для указанного файла
Формат CSV
csv
Date;ExecutionTime;Value
2026-01-01T10-00-00.1234Z;5.5;100.5
2026-01-01T10-00-05.1234Z;3.2;200.3
Тестовый файл
Создать файл test.csv:

csv
2026-01-01T10-00-00.1234Z;5.5;100.5
2026-01-01T10-00-05.1234Z;3.2;200.3
2026-01-01T10-00-10.1234Z;4.1;150.7
Загрузить его через Swagger.

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
text
