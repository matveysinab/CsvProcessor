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
