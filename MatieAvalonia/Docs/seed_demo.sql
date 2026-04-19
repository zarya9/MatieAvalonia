-- Демо-наполнение БД для «Матье» (PostgreSQL). Имена таблиц/колонок как в модели EF (Npgsql).
-- Выполняйте на копии базы. При расхождении имён таблиц проверьте схему (часто public."Services", public."Collection").

BEGIN;

INSERT INTO "Collection" ("Name")
SELECT v FROM (VALUES ('Кастом открытый'), ('Косплей лаб')) AS t(v)
WHERE NOT EXISTS (SELECT 1 FROM "Collection" c WHERE c."Name" = t.v);

INSERT INTO "Services" ("Name", "Description", "Price", "Collection_id", "Updated_at", "ImgPath")
SELECT 'Демо-стрижка', 'Коротко о услуге', 1500.00, c."ID_Collection", NOW(), NULL
FROM "Collection" c
WHERE c."Name" ILIKE '%кастом%'
  AND NOT EXISTS (SELECT 1 FROM "Services" s WHERE s."Name" = 'Демо-стрижка')
LIMIT 1;

COMMIT;

-- Пользователей, роли, мастеров, записи и отзывы проще создать через UI (регистрация, админ, модератор).
