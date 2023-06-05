-- CREATE DATABASE IF NOT EXISTS df_game_db DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT ENCRYPTION='N';
-- USE df_game_db;
CREATE DATABASE IF NOT EXISTS hsst_game_db DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT ENCRYPTION='N';
USE hsst_game_db;

CREATE TABLE IF NOT EXISTS user_accounts
(
	user_id BIGINT AUTO_INCREMENT PRIMARY KEY,
	email VARCHAR(50) UNIQUE NOT NULL,
	nickname VARCHAR(50) UNIQUE NOT NULL,
	salt BINARY(16) NOT NULL,
	hashed_password BINARY(32) NOT NULL
);

CREATE TABLE IF NOT EXISTS user_attendences
(
	user_id BIGINT NOT NULL UNIQUE COMMENT '유저의 고유 넘버, Account db의 값과 일치',
	consecutive_login_count INT NOT NULL DEFAULT 0 COMMENT '연속접속 횟수',
	last_login_date DATETIME NOT NULL COMMENT '마지막 접속'
);

CREATE TABLE IF NOT EXISTS user_collections
(
	collection_id BIGINT PRIMARY KEY NOT NULL AUTO_INCREMENT COMMENT '수집품 아이디',
	user_id BIGINT NOT NULL COMMENT '소유자',
	collection_code BIGINT NOT NULL COMMENT '수집품 코드',
	collection_count INT NOT NULL COMMENT '수집품 개수',
);

CREATE TABLE IF NOT EXISTS mailbox
(
	mail_id BIGINT PRIMARY KEY NOT NULL AUTO_INCREMENT COMMENT '우편 아이디',
	user_id BIGINT NOT NULL COMMENT '유저 아이디',
	collection_code BIGINT DEFAULT -1 COMMENT '0번 아이템 종류',
	collection_count INT DEFAULT -1 COMMENT '0번 아이템 개수',
	mail_title TEXT NOT NULL COMMENT '우편 타이틀',
	mail_body TEXT COMMENT '메세지',
	read_date DATETIME NOT NULL DEFAULT '9999-12-31 23:59:59' COMMENT '읽은 날짜',
	recieve_date DATETIME NOT NULL COMMENT '수령일',
	expiration_date DATETIME NOT NULL DEFAULT '9999-12-31 23:59:59' COMMENT '만료일',
	is_deleted TINYINT NOT NULL DEFAULT 0 COMMENT '삭제여부'
);

-- CREATE TABLE IF NOT EXISTS user_achievement
-- (
-- 	user_id BIGINT PRIMARY KEY NOT NULL COMMENT '유저 아이디',
-- 	user_level INT NOT NULL DEFAULT 1 COMMENT '유저 레벨',
-- 	user_exp BIGINT NOT NULL DEFAULT 0 COMMENT '유저가 가진 경험치',
-- 	highest_cleared_stage_id BIGINT NOT NULL DEFAULT 0 COMMENT '유저가 최대로 클리어한 던전'
-- );