-- SEQUENCES
CREATE SEQUENCE category_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE article_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE tag_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- TABLES
CREATE TABLE categories (
    category_id bigint NOT NULL DEFAULT nextval('category_id_seq'::regclass),
    parent_id bigint,
    created_at timestamp without time zone NOT NULL,
    updated_at timestamp without time zone NOT NULL,
    title character varying(240) NOT NULL
);

CREATE TABLE articles (
    article_id bigint NOT NULL DEFAULT nextval('article_id_seq'::regclass),
    category_id bigint NOT NULL,
    author_id bigint,
    date_at timestamp without time zone NOT NULL,
    created_at timestamp without time zone NOT NULL,
    updated_at timestamp without time zone NOT NULL,
    title character varying(255) NOT NULL,
    content text NOT NULL,
    status integer DEFAULT 0 NOT NULL
);

CREATE TABLE tags (
    tag_id bigint NOT NULL DEFAULT nextval('tag_id_seq'::regclass),
    slug character varying(120) NOT NULL,
    title character varying(120) NOT NULL
);

CREATE TABLE article_tags (
    article_id bigint NOT NULL,
    tag_id bigint NOT NULL
);

CREATE TABLE article_roles (
    article_id bigint NOT NULL,
    slug character varying(80) NOT NULL,
    name character varying(80) NOT NULL
);

-- SET SEQUENCES TO COLUMNS
ALTER SEQUENCE category_id_seq OWNED BY categories.category_id;
ALTER SEQUENCE article_id_seq OWNED BY articles.article_id;
ALTER SEQUENCE tag_id_seq OWNED BY tags.tag_id;

-- PRIMARY KEYS
ALTER TABLE ONLY categories ADD CONSTRAINT categories_pkey PRIMARY KEY (category_id);
ALTER TABLE ONLY articles ADD CONSTRAINT articles_pkey PRIMARY KEY (article_id);
ALTER TABLE ONLY tags ADD CONSTRAINT tags_pkey PRIMARY KEY (tag_id);
ALTER TABLE ONLY article_tags ADD CONSTRAINT article_tags_pkey PRIMARY KEY (article_id, tag_id);
ALTER TABLE ONLY article_roles ADD CONSTRAINT article_roles_pkey PRIMARY KEY (article_id, slug);

-- FOREIGN KEYS
ALTER TABLE ONLY categories ADD CONSTRAINT fk_category_parent FOREIGN KEY (parent_id) REFERENCES categories(category_id);
ALTER TABLE ONLY articles ADD CONSTRAINT fk_article_category FOREIGN KEY (category_id) REFERENCES categories(category_id);
ALTER TABLE ONLY article_tags ADD CONSTRAINT fk_article_tag_article FOREIGN KEY (article_id) REFERENCES articles(article_id) ON DELETE CASCADE;
ALTER TABLE ONLY article_tags ADD CONSTRAINT fk_article_tag_tag FOREIGN KEY (tag_id) REFERENCES tags(tag_id);
ALTER TABLE ONLY article_roles ADD CONSTRAINT fk_article_role_article FOREIGN KEY (article_id) REFERENCES articles(article_id) ON DELETE CASCADE;

-- INITIALIZE SEQUENCES
SELECT pg_catalog.setval('article_id_seq', 1, true);
SELECT pg_catalog.setval('category_id_seq', 1, true);
SELECT pg_catalog.setval('tag_id_seq', 1, true);
