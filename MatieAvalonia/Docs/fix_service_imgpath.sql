-- Нормализация путей картинок услуг для приложения «Матье» (PostgreSQL).
-- В БД должно быть: Resources/Pr1.jpg (приложение понимает и «/», и «\»).
-- Старые значения вида «2 квалик/Ресурсы/pr/Кастом/Pr1.jpg» → Resources/Pr1.jpg
-- (имя файла = хвост после последнего / или \; с начала снимается префикс «цифры + пробел»).
--
-- Перед выполнением: бэкап. Файлы с такими именами должны лежать в MatieAvalonia/Resources/
-- и копироваться в bin/.../Resources при сборке.
--
-- Просмотр результата до UPDATE:
-- SELECT "ID_Service", "ImgPath" AS old,
--        CONCAT('Resources/', regexp_replace(
--          regexp_replace(trim("ImgPath"), '^\d+\s+', ''),
--          E'^.*[/\\\\]',
--          ''
--        )) AS new
-- FROM "Services"
-- WHERE "ImgPath" IS NOT NULL AND btrim("ImgPath") <> ''
--   AND "ImgPath" !~* '^[Rr]esources[/\\\\]';

BEGIN;

UPDATE "Services"
SET "ImgPath" = CONCAT(
  'Resources/',
  regexp_replace(
    regexp_replace(trim("ImgPath"), '^\d+\s+', ''),
    E'^.*[/\\\\]',
    ''
  )
)
WHERE "ImgPath" IS NOT NULL
  AND btrim("ImgPath") <> ''
  AND "ImgPath" !~* E'^[Rr]esources[/\\\\]'
  AND length(regexp_replace(
    regexp_replace(trim("ImgPath"), '^\d+\s+', ''),
    E'^.*[/\\\\]',
    ''
  )) > 0;

COMMIT;
