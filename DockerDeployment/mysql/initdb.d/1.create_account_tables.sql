CREATE DATABASE IF NOT EXISTS hsst_account_db DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT ENCRYPTION='N';
USE hsst_account_db;

CREATE TABLE IF NOT EXISTS user_accounts
(
	user_id BIGINT AUTO_INCREMENT PRIMARY KEY,
	user_email VARCHAR(50) UNIQUE NOT NULL,
	user_nickname VARCHAR(50) UNIQUE NOT NULL,
	salt BINARY(16) NOT NULL,
	hashed_password BINARY(32) NOT NULL
);