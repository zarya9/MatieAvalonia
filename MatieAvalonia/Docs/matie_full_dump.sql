--
-- PostgreSQL database dump
--

\restrict jbqQ7Oep4xiw4A13h7sAZS2L1TFMOLKROyAqnxlB5I5S1D45vFnU4Xauhdy7hWb

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Booking; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Booking" (
    "ID_Booking" integer NOT NULL,
    "User_id" integer,
    "Master_id" integer,
    "Service_id" integer,
    "DateTime" timestamp without time zone,
    "Status_id" integer,
    "Updated_At" timestamp without time zone,
    "Type_id" integer
);


ALTER TABLE public."Booking" OWNER TO postgres;

--
-- Name: BookingStatus; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BookingStatus" (
    "ID_BookingStatus" integer NOT NULL,
    "Name " character varying(50)
);


ALTER TABLE public."BookingStatus" OWNER TO postgres;

--
-- Name: BookingStatus_ID_BookingStatus_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."BookingStatus_ID_BookingStatus_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."BookingStatus_ID_BookingStatus_seq" OWNER TO postgres;

--
-- Name: BookingStatus_ID_BookingStatus_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."BookingStatus_ID_BookingStatus_seq" OWNED BY public."BookingStatus"."ID_BookingStatus";


--
-- Name: Booking_ID_Booking_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Booking_ID_Booking_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Booking_ID_Booking_seq" OWNER TO postgres;

--
-- Name: Booking_ID_Booking_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Booking_ID_Booking_seq" OWNED BY public."Booking"."ID_Booking";


--
-- Name: CardUser; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CardUser" (
    "ID_Card" integer NOT NULL,
    "User_id" integer,
    "NumberCard" character varying(20),
    "DateCard" timestamp without time zone,
    "CVV" character varying(3),
    "IsPriority" boolean
);


ALTER TABLE public."CardUser" OWNER TO postgres;

--
-- Name: CardUser_ID_Card_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."CardUser_ID_Card_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."CardUser_ID_Card_seq" OWNER TO postgres;

--
-- Name: CardUser_ID_Card_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."CardUser_ID_Card_seq" OWNED BY public."CardUser"."ID_Card";


--
-- Name: Collection; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Collection" (
    "ID_Collection" integer NOT NULL,
    "Name" character varying(100)
);


ALTER TABLE public."Collection" OWNER TO postgres;

--
-- Name: Collection_ID_Collection_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Collection_ID_Collection_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Collection_ID_Collection_seq" OWNER TO postgres;

--
-- Name: Collection_ID_Collection_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Collection_ID_Collection_seq" OWNED BY public."Collection"."ID_Collection";


--
-- Name: Master; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Master" (
    "ID_Master" integer NOT NULL,
    "User_id" integer,
    "Qualif_id" integer
);


ALTER TABLE public."Master" OWNER TO postgres;

--
-- Name: Master_ID_Master_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Master_ID_Master_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Master_ID_Master_seq" OWNER TO postgres;

--
-- Name: Master_ID_Master_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Master_ID_Master_seq" OWNED BY public."Master"."ID_Master";


--
-- Name: Qualification; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Qualification" (
    "ID_Qualif" integer NOT NULL,
    "Name" character varying,
    "Index" integer
);


ALTER TABLE public."Qualification" OWNER TO postgres;

--
-- Name: Qualification_ID_Qualif_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Qualification_ID_Qualif_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Qualification_ID_Qualif_seq" OWNER TO postgres;

--
-- Name: Qualification_ID_Qualif_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Qualification_ID_Qualif_seq" OWNED BY public."Qualification"."ID_Qualif";


--
-- Name: Request; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Request" (
    "ID_Request" integer NOT NULL,
    "User_id" integer,
    "Quialif_id" integer,
    "Status_id" integer,
    "Reviewed_At" timestamp without time zone,
    "Created_At" timestamp without time zone
);


ALTER TABLE public."Request" OWNER TO postgres;

--
-- Name: RequestStatuses; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestStatuses" (
    "ID_Status" integer NOT NULL,
    "Name" character varying(50)
);


ALTER TABLE public."RequestStatuses" OWNER TO postgres;

--
-- Name: RequestStatuses_ID_Status_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RequestStatuses_ID_Status_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RequestStatuses_ID_Status_seq" OWNER TO postgres;

--
-- Name: RequestStatuses_ID_Status_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RequestStatuses_ID_Status_seq" OWNED BY public."RequestStatuses"."ID_Status";


--
-- Name: Request_ID_Request_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Request_ID_Request_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Request_ID_Request_seq" OWNER TO postgres;

--
-- Name: Request_ID_Request_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Request_ID_Request_seq" OWNED BY public."Request"."ID_Request";


--
-- Name: Reviews; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Reviews" (
    "ID_Review" integer NOT NULL,
    "User_id" integer,
    "Service_id" integer,
    "Master_id" integer,
    "Rating" integer,
    "Comment" character varying(255),
    "Created_At" timestamp without time zone
);


ALTER TABLE public."Reviews" OWNER TO postgres;

--
-- Name: Reviews_ID_Review_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Reviews_ID_Review_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Reviews_ID_Review_seq" OWNER TO postgres;

--
-- Name: Reviews_ID_Review_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Reviews_ID_Review_seq" OWNED BY public."Reviews"."ID_Review";


--
-- Name: Role; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Role" (
    "ID_Role" integer NOT NULL,
    "Name" character varying(50)
);


ALTER TABLE public."Role" OWNER TO postgres;

--
-- Name: Role_ID_Role_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Role_ID_Role_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Role_ID_Role_seq" OWNER TO postgres;

--
-- Name: Role_ID_Role_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Role_ID_Role_seq" OWNED BY public."Role"."ID_Role";


--
-- Name: Services; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Services" (
    "ID_Service" integer NOT NULL,
    "Collection_id" integer,
    "Name" character varying(50),
    "Description" character varying(50),
    "Price" numeric(7,2),
    "Updated_at" timestamp without time zone,
    "ImgPath" character varying(255)
);


ALTER TABLE public."Services" OWNER TO postgres;

--
-- Name: Services_ID_Service_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Services_ID_Service_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Services_ID_Service_seq" OWNER TO postgres;

--
-- Name: Services_ID_Service_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Services_ID_Service_seq" OWNED BY public."Services"."ID_Service";


--
-- Name: User; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."User" (
    "ID_User" integer NOT NULL,
    "Fname" character varying(50),
    "Name" character varying(50),
    "Patronymic" character varying(50),
    "Login" character varying(50),
    "Password" character varying(50),
    "Role_id" integer,
    "Updated_At" timestamp without time zone,
    "Balance" numeric(20,2)
);


ALTER TABLE public."User" OWNER TO postgres;

--
-- Name: User_ID_User_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."User_ID_User_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."User_ID_User_seq" OWNER TO postgres;

--
-- Name: User_ID_User_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."User_ID_User_seq" OWNED BY public."User"."ID_User";


--
-- Name: Booking ID_Booking; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Booking" ALTER COLUMN "ID_Booking" SET DEFAULT nextval('public."Booking_ID_Booking_seq"'::regclass);


--
-- Name: BookingStatus ID_BookingStatus; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingStatus" ALTER COLUMN "ID_BookingStatus" SET DEFAULT nextval('public."BookingStatus_ID_BookingStatus_seq"'::regclass);


--
-- Name: CardUser ID_Card; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CardUser" ALTER COLUMN "ID_Card" SET DEFAULT nextval('public."CardUser_ID_Card_seq"'::regclass);


--
-- Name: Collection ID_Collection; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Collection" ALTER COLUMN "ID_Collection" SET DEFAULT nextval('public."Collection_ID_Collection_seq"'::regclass);


--
-- Name: Master ID_Master; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Master" ALTER COLUMN "ID_Master" SET DEFAULT nextval('public."Master_ID_Master_seq"'::regclass);


--
-- Name: Qualification ID_Qualif; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Qualification" ALTER COLUMN "ID_Qualif" SET DEFAULT nextval('public."Qualification_ID_Qualif_seq"'::regclass);


--
-- Name: Request ID_Request; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request" ALTER COLUMN "ID_Request" SET DEFAULT nextval('public."Request_ID_Request_seq"'::regclass);


--
-- Name: RequestStatuses ID_Status; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatuses" ALTER COLUMN "ID_Status" SET DEFAULT nextval('public."RequestStatuses_ID_Status_seq"'::regclass);


--
-- Name: Reviews ID_Review; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Reviews" ALTER COLUMN "ID_Review" SET DEFAULT nextval('public."Reviews_ID_Review_seq"'::regclass);


--
-- Name: Role ID_Role; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Role" ALTER COLUMN "ID_Role" SET DEFAULT nextval('public."Role_ID_Role_seq"'::regclass);


--
-- Name: Services ID_Service; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Services" ALTER COLUMN "ID_Service" SET DEFAULT nextval('public."Services_ID_Service_seq"'::regclass);


--
-- Name: User ID_User; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User" ALTER COLUMN "ID_User" SET DEFAULT nextval('public."User_ID_User_seq"'::regclass);


--
-- Data for Name: Booking; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Booking" ("ID_Booking", "User_id", "Master_id", "Service_id", "DateTime", "Status_id", "Updated_At", "Type_id") FROM stdin;
1	6	3	1	2026-04-19 11:17:33.880487	1	2026-04-18 11:17:33.880487	1
2	7	4	2	2026-04-20 11:17:33.880487	2	2026-04-18 11:17:33.880487	1
3	8	5	3	2026-04-21 11:17:33.880487	3	2026-04-18 11:17:33.880487	2
4	9	3	4	2026-04-22 11:17:33.880487	4	2026-04-18 11:17:33.880487	2
5	10	4	5	2026-04-23 11:17:33.880487	5	2026-04-18 11:17:33.880487	3
6	6	5	6	2026-04-24 11:17:33.880487	6	2026-04-18 11:17:33.880487	3
7	7	3	7	2026-04-25 11:17:33.880487	7	2026-04-18 11:17:33.880487	1
8	8	4	8	2026-04-26 11:17:33.880487	8	2026-04-18 11:17:33.880487	2
9	9	5	9	2026-04-27 11:17:33.880487	9	2026-04-18 11:17:33.880487	3
10	10	3	10	2026-04-28 11:17:33.880487	10	2026-04-18 11:17:33.880487	1
11	6	2	8	2026-04-20 10:00:00	1	2026-04-19 00:11:04.184715	\N
12	6	2	8	2026-04-20 10:00:00	1	2026-04-19 00:37:20.341331	\N
\.


--
-- Data for Name: BookingStatus; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."BookingStatus" ("ID_BookingStatus", "Name ") FROM stdin;
1	Создана
2	Подтверждена
3	В работе
4	Выполнена
5	Отменена клиентом
6	Отменена мастером
7	Просрочена
8	Перенесена
9	Ожидает оплаты
10	Ожидает подтверждения
\.


--
-- Data for Name: CardUser; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."CardUser" ("ID_Card", "User_id", "NumberCard", "DateCard", "CVV", "IsPriority") FROM stdin;
1	6	2200123412341234	2027-12-01 00:00:00	111	f
2	7	2200123412341235	2028-03-01 00:00:00	112	t
3	8	2200123412341236	2029-05-01 00:00:00	113	f
4	9	2200123412341237	2028-09-01 00:00:00	114	f
5	10	2200123412341238	2030-01-01 00:00:00	115	t
6	3	2200123412341239	2027-07-01 00:00:00	116	f
7	4	2200123412341240	2027-08-01 00:00:00	117	f
8	5	2200123412341241	2027-10-01 00:00:00	118	t
9	2	2200123412341242	2029-11-01 00:00:00	119	f
10	1	2200123412341243	2031-02-01 00:00:00	120	t
11	6	1111111111111111	2030-04-30 23:59:59	777	f
\.


--
-- Data for Name: Collection; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Collection" ("ID_Collection", "Name") FROM stdin;
1	Аниме
2	Новый год
3	Хэллоуин
4	Киберпанк
5	Нуар
6	Кастом
7	Косплей
8	Весна
9	Фэнтези
10	Минимализм
\.


--
-- Data for Name: Master; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Master" ("ID_Master", "User_id", "Qualif_id") FROM stdin;
1	3	3
2	4	4
3	5	5
4	1	7
5	2	5
6	6	1
7	7	2
8	8	2
9	9	3
10	10	2
\.


--
-- Data for Name: Qualification; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Qualification" ("ID_Qualif", "Name", "Index") FROM stdin;
1	Стажёр	1
2	Ученик	2
3	Мастер (базовый)	3
4	Мастер	4
5	Ведущий мастер	5
6	Старший мастер	6
7	Главный мастер лавки	7
8	Эксперт по косплею	8
9	Эксперт по кастому	9
10	Наставник / методист	10
\.


--
-- Data for Name: Request; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Request" ("ID_Request", "User_id", "Quialif_id", "Status_id", "Reviewed_At", "Created_At") FROM stdin;
1	3	4	1	\N	2026-04-06 11:17:33.880487
2	4	5	2	\N	2026-04-08 11:17:33.880487
3	5	6	3	2026-04-13 11:17:33.880487	2026-04-09 11:17:33.880487
4	6	2	4	2026-04-12 11:17:33.880487	2026-04-10 11:17:33.880487
5	7	3	5	2026-04-14 11:17:33.880487	2026-04-11 11:17:33.880487
6	8	4	6	\N	2026-04-12 11:17:33.880487
7	9	5	7	\N	2026-04-13 11:17:33.880487
8	10	6	8	\N	2026-04-14 11:17:33.880487
9	2	7	9	2026-04-16 11:17:33.880487	2026-04-15 11:17:33.880487
10	1	8	10	2026-04-17 11:17:33.880487	2026-04-16 11:17:33.880487
\.


--
-- Data for Name: RequestStatuses; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."RequestStatuses" ("ID_Status", "Name") FROM stdin;
1	Создана
2	На рассмотрении
3	Одобрена
4	Отклонена
5	Требует правок
6	Назначено собеседование
7	Проверка портфолио
8	Ожидает документов
9	Архив
10	Закрыта
\.


--
-- Data for Name: Reviews; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Reviews" ("ID_Review", "User_id", "Service_id", "Master_id", "Rating", "Comment", "Created_At") FROM stdin;
1	6	1	1	5	Очень понравилась кастомизация, быстро и аккуратно.	2026-04-09 11:17:33.880487
2	7	2	2	4	Хорошая работа, хотелось чуть ярче по цвету.	2026-04-10 11:17:33.880487
3	8	3	3	5	Образ получился как в референсе.	2026-04-11 11:17:33.880487
4	9	4	1	4	Качество отличное, немного задержали срок.	2026-04-12 11:17:33.880487
5	10	5	2	5	Идеально под фестиваль, спасибо мастеру.	2026-04-13 11:17:33.880487
6	3	6	3	4	Праздничный образ понравился клиенту.	2026-04-14 11:17:33.880487
7	4	7	1	5	Грим держался весь вечер.	2026-04-15 11:17:33.880487
8	5	8	2	5	Киберпанк-эффект получился отличным.	2026-04-16 11:17:33.880487
9	2	9	3	4	Стиль выдержан, хорошая детализация.	2026-04-17 11:17:33.880487
10	1	10	1	5	Экспресс-услуга без потери качества.	2026-04-18 11:17:33.880487
11	6	3	6	3	павапр	2026-04-19 00:39:21.280909
\.


--
-- Data for Name: Role; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Role" ("ID_Role", "Name") FROM stdin;
1	Администратор
2	Модератор
3	Мастер
4	Пользователь
10	Гость
\.


--
-- Data for Name: Services; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Services" ("ID_Service", "Collection_id", "Name", "Description", "Price", "Updated_at", "ImgPath") FROM stdin;
1	6	Кастом аксессуара	Индивидуальная кастомизация	1500.00	2026-04-18 11:17:33.880487	Resources/Pr1.jpg
2	6	Покраска реквизита	Покраска с грунтом и лаком	2200.00	2026-04-18 11:17:33.880487	Resources/Pr2.jpg
4	7	Пошив косплей-элемента	Изготовление одного элемента	4500.00	2026-04-18 11:17:33.880487	Resources/KL2.jpg
5	1	Стайлинг аниме	Образ под аниме-персонажа	2800.00	2026-04-18 11:17:33.880487	Resources/KL3.jpg
6	2	Новогодний образ	Праздничный макияж и аксессуары	2600.00	2026-04-18 11:17:33.880487	Resources/Pr3.jpg
7	3	Хэллоуин грим	Тематический грим	3200.00	2026-04-18 11:17:33.880487	Resources/Pr4.jpg
8	4	Киберпанк-детализация	Неон, металл, акцентные элементы	3900.00	2026-04-18 11:17:33.880487	Resources/Pr5.jpg
9	5	Нуар-стилизация	Драматичная ч/б подача	3400.00	2026-04-18 11:17:33.880487	Resources/KL4.jpg
10	6	Экспресс кастом	Быстрая кастомизация за 1 день	1800.00	2026-04-18 11:17:33.880487	Resources/Pr6.jpg
3	7	Базовый косплей-образ!!!	Подбор и адаптация образа	3000.00	2026-04-19 00:01:20.04654	Resources/KL1.jpg
\.


--
-- Data for Name: User; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."User" ("ID_User", "Fname", "Name", "Patronymic", "Login", "Password", "Role_id", "Updated_At", "Balance") FROM stdin;
1	Иванов	Иван	Иванович	admin	admin123	1	2026-04-18 11:17:33.880487	\N
2	Петрова	Мария	Сергеевна	moder	moder123	2	2026-04-18 11:17:33.880487	\N
3	Смирнов	Алексей	Павлович	master1	master123	3	2026-04-18 11:17:33.880487	\N
4	Кузнецова	Анна	Игоревна	master2	master123	3	2026-04-18 11:17:33.880487	\N
5	Соколов	Дмитрий	Олегович	master3	master123	3	2026-04-18 11:17:33.880487	\N
7	Новиков	Максим	Юрьевич	user2	user123	4	2026-04-18 11:17:33.880487	\N
8	Фёдорова	Ольга	Романовна	user3	user123	4	2026-04-18 11:17:33.880487	\N
9	Морозов	Никита	Владимирович	user4	user123	4	2026-04-18 11:17:33.880487	\N
10	Павлова	София	Денисовна	user5	user123	4	2026-04-18 11:17:33.880487	\N
6	Васильева	Елена	Андреевна	user1	user123	4	2026-04-19 00:37:06.751361	8508.00
\.


--
-- Name: BookingStatus_ID_BookingStatus_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."BookingStatus_ID_BookingStatus_seq"', 10, true);


--
-- Name: Booking_ID_Booking_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Booking_ID_Booking_seq"', 12, true);


--
-- Name: CardUser_ID_Card_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."CardUser_ID_Card_seq"', 11, true);


--
-- Name: Collection_ID_Collection_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Collection_ID_Collection_seq"', 10, true);


--
-- Name: Master_ID_Master_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Master_ID_Master_seq"', 10, true);


--
-- Name: Qualification_ID_Qualif_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Qualification_ID_Qualif_seq"', 10, true);


--
-- Name: RequestStatuses_ID_Status_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."RequestStatuses_ID_Status_seq"', 10, true);


--
-- Name: Request_ID_Request_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Request_ID_Request_seq"', 10, true);


--
-- Name: Reviews_ID_Review_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Reviews_ID_Review_seq"', 11, true);


--
-- Name: Role_ID_Role_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Role_ID_Role_seq"', 10, true);


--
-- Name: Services_ID_Service_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Services_ID_Service_seq"', 10, true);


--
-- Name: User_ID_User_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."User_ID_User_seq"', 10, true);


--
-- Name: BookingStatus BookingStatus_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BookingStatus"
    ADD CONSTRAINT "BookingStatus_pkey" PRIMARY KEY ("ID_BookingStatus");


--
-- Name: Booking Booking_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Booking"
    ADD CONSTRAINT "Booking_pkey" PRIMARY KEY ("ID_Booking");


--
-- Name: CardUser CardUser_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CardUser"
    ADD CONSTRAINT "CardUser_pkey" PRIMARY KEY ("ID_Card");


--
-- Name: Collection Collection_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Collection"
    ADD CONSTRAINT "Collection_pkey" PRIMARY KEY ("ID_Collection");


--
-- Name: Master Master_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Master"
    ADD CONSTRAINT "Master_pkey" PRIMARY KEY ("ID_Master");


--
-- Name: Qualification Qualification_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Qualification"
    ADD CONSTRAINT "Qualification_pkey" PRIMARY KEY ("ID_Qualif");


--
-- Name: RequestStatuses RequestStatuses_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestStatuses"
    ADD CONSTRAINT "RequestStatuses_pkey" PRIMARY KEY ("ID_Status");


--
-- Name: Request Request_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT "Request_pkey" PRIMARY KEY ("ID_Request");


--
-- Name: Reviews Reviews_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Reviews"
    ADD CONSTRAINT "Reviews_pkey" PRIMARY KEY ("ID_Review");


--
-- Name: Role Role_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Role"
    ADD CONSTRAINT "Role_pkey" PRIMARY KEY ("ID_Role");


--
-- Name: Services Services_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Services"
    ADD CONSTRAINT "Services_pkey" PRIMARY KEY ("ID_Service");


--
-- Name: User User_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_pkey" PRIMARY KEY ("ID_User");


--
-- Name: Booking Booking_Status; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Booking"
    ADD CONSTRAINT "Booking_Status" FOREIGN KEY ("Status_id") REFERENCES public."BookingStatus"("ID_BookingStatus") NOT VALID;


--
-- Name: Services Collection_Services; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Services"
    ADD CONSTRAINT "Collection_Services" FOREIGN KEY ("Collection_id") REFERENCES public."Collection"("ID_Collection") NOT VALID;


--
-- Name: Booking Master_Booking; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Booking"
    ADD CONSTRAINT "Master_Booking" FOREIGN KEY ("Master_id") REFERENCES public."User"("ID_User");


--
-- Name: Master Master_Qualif; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Master"
    ADD CONSTRAINT "Master_Qualif" FOREIGN KEY ("Qualif_id") REFERENCES public."Qualification"("ID_Qualif");


--
-- Name: Reviews Master_Review; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Reviews"
    ADD CONSTRAINT "Master_Review" FOREIGN KEY ("Master_id") REFERENCES public."Master"("ID_Master");


--
-- Name: Request Qualif_Request; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT "Qualif_Request" FOREIGN KEY ("Quialif_id") REFERENCES public."Qualification"("ID_Qualif");


--
-- Name: Request Request_Status; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT "Request_Status" FOREIGN KEY ("Status_id") REFERENCES public."RequestStatuses"("ID_Status") NOT VALID;


--
-- Name: Booking Service_Booking; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Booking"
    ADD CONSTRAINT "Service_Booking" FOREIGN KEY ("Service_id") REFERENCES public."Services"("ID_Service") NOT VALID;


--
-- Name: Reviews Service_Review; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Reviews"
    ADD CONSTRAINT "Service_Review" FOREIGN KEY ("Service_id") REFERENCES public."Services"("ID_Service");


--
-- Name: Booking User_Booking; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Booking"
    ADD CONSTRAINT "User_Booking" FOREIGN KEY ("User_id") REFERENCES public."User"("ID_User");


--
-- Name: CardUser User_Card; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CardUser"
    ADD CONSTRAINT "User_Card" FOREIGN KEY ("User_id") REFERENCES public."User"("ID_User");


--
-- Name: Master User_Master; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Master"
    ADD CONSTRAINT "User_Master" FOREIGN KEY ("User_id") REFERENCES public."User"("ID_User");


--
-- Name: Reviews User_Rating; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Reviews"
    ADD CONSTRAINT "User_Rating" FOREIGN KEY ("User_id") REFERENCES public."User"("ID_User");


--
-- Name: Request User_Request; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Request"
    ADD CONSTRAINT "User_Request" FOREIGN KEY ("User_id") REFERENCES public."User"("ID_User");


--
-- Name: User User_Role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."User"
    ADD CONSTRAINT "User_Role" FOREIGN KEY ("Role_id") REFERENCES public."Role"("ID_Role") NOT VALID;


--
-- PostgreSQL database dump complete
--

\unrestrict jbqQ7Oep4xiw4A13h7sAZS2L1TFMOLKROyAqnxlB5I5S1D45vFnU4Xauhdy7hWb

