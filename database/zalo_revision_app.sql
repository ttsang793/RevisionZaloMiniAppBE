-- DROP DATABASE `zalo_revision_app_db`;

-- CREATE DATABASE `zalo_revision_app_db`;

CREATE TABLE `subjects` (
	`id` VARCHAR(4) PRIMARY KEY,
	`name` VARCHAR(100) UNIQUE NOT NULL,
	`grades` JSON NOT NULL, -- byte[]
	`questionMC` BOOLEAN,
	`questionTF` BOOLEAN,
	`questionSA` BOOLEAN,
	`questionGF` BOOLEAN,
	`questionST` BOOLEAN,
	`created_at` DATETIME DEFAULT NOW(),
	`updated_at` DATETIME DEFAULT NOW() ON UPDATE NOW(),
	`is_visible` BOOLEAN DEFAULT TRUE
);

CREATE TABLE `topics` (
	`id` VARCHAR(10) PRIMARY KEY,
	`name` VARCHAR(200) NOT NULL,
	`grades` JSON NOT NULL, -- byte[]
	`subject_id` VARCHAR(4) NOT NULL,
	`created_at` DATETIME DEFAULT NOW(),
	`updated_at` DATETIME DEFAULT NOW() ON UPDATE NOW(),
	`is_visible` BOOLEAN DEFAULT TRUE,
	FOREIGN KEY (`subject_id`) REFERENCES `subjects`(`id`) ON DELETE CASCADE
);

CREATE TABLE `users` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`zalo_id` TEXT,
	`name` VARCHAR(255) NOT NULL,
	`avatar` TEXT,
	`email` TEXT,
	`role` ENUM("HS", "GV", "AD") NOT NULL DEFAULT "HS",
	`notification` JSON, -- boolean[]
	`created_at` DATETIME DEFAULT NOW(),
	`updated_at` DATETIME DEFAULT NOW() ON UPDATE NOW()
);

CREATE TABLE `students` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`favorites` JSON, -- bigint[]
	`achivements` JSON, -- smallint[]
	`grade` TINYINT UNSIGNED,
	`attendance` JSON, -- datetime[]
	`streak` SMALLINT UNSIGNED DEFAULT 0,
	`allow_save_history` BOOLEAN DEFAULT TRUE,
	FOREIGN KEY (`id`) REFERENCES `users`(`id`) ON DELETE CASCADE
);

CREATE TABLE `teachers` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`subject_id` VARCHAR(4),
	`grades` JSON NOT NULL, -- byte[]
	`introduction` TEXT,
	FOREIGN KEY (`subject_id`) REFERENCES `subjects`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`id`) REFERENCES `users`(`id`) ON DELETE CASCADE
);

CREATE TABLE `followers` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`student_id` BIGINT UNSIGNED NOT NULL,
	`teacher_id` BIGINT UNSIGNED NOT NULL,
	`is_visible` BOOLEAN DEFAULT TRUE,
	FOREIGN KEY (`student_id`) REFERENCES `students`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`teacher_id`) REFERENCES `teachers`(`id`) ON DELETE CASCADE
);

CREATE TABLE `admins` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`password` VARCHAR(255) NOT NULL,
	FOREIGN KEY (`id`) REFERENCES `users`(`id`) ON DELETE CASCADE
);

CREATE TABLE `questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`title` TEXT,
	`image_url` TEXT,
	`grade` TINYINT UNSIGNED,
	`type` ENUM("multiple-choice", "true-false", "short-answer", "gap-fill", "constructed-response", "sorting", "true-false-thpt") NOT NULL,
	`difficulty` TINYINT UNSIGNED,
	`topic_id` VARCHAR(10) NOT NULL,
	`teacher_id` BIGINT UNSIGNED NOT NULL,
	`explanation` TEXT,
	`created_at` DATETIME DEFAULT NOW(),
	`updated_at` DATETIME DEFAULT NOW() ON UPDATE NOW(),
	FOREIGN KEY (`topic_id`) REFERENCES `topics`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`teacher_id`) REFERENCES `teachers`(`id`) ON DELETE CASCADE
);

CREATE TABLE `multiple_choice_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`correct_answer` TEXT NOT NULL,
	`wrong_answer` JSON NOT NULL, -- text[]
	FOREIGN KEY (`id`) REFERENCES `questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `true_false_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`answer_key` BOOLEAN NOT NULL,
	FOREIGN KEY (`id`) REFERENCES `questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `short_answer_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`answer_key` VARCHAR(4),
	FOREIGN KEY (`id`) REFERENCES `questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `manual_response_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`answer_keys` JSON, -- text[]
	`allow_take_photo` BOOLEAN,
	`allow_enter` BOOLEAN,
	`mark_as_wrong` BOOLEAN NOT NULL,
	FOREIGN KEY (`id`) REFERENCES `questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `sorting_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`correct_order` JSON NOT NULL, -- text[]
	FOREIGN KEY (`id`) REFERENCES `questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `true_false_THPT_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`passage_title` VARCHAR(255),
	`passage_content` MEDIUMTEXT,
	`passage_author` TEXT,
	`statements` JSON NOT NULL, -- text[]
	`answer_keys` JSON NOT NULL, -- text[]
	FOREIGN KEY (`id`) REFERENCES `questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `exams` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`exam_type` ENUM("regular", "midterm", "final", "other") NOT NULL DEFAULT "regular",
	`display_type` ENUM("normal", "pdf") NOT NULL DEFAULT "normal",
	`title` VARCHAR(255) NOT NULL,
	`grade` TINYINT UNSIGNED NOT NULL,
	`time_limit` SMALLINT UNSIGNED NOT NULL,
	`early_turn_in` SMALLINT UNSIGNED,
	`allow_show_score` BOOLEAN DEFAULT TRUE NOT NULL,
	`allow_part_swap` BOOLEAN DEFAULT FALSE,
	`allow_question_swap` BOOLEAN DEFAULT FALSE,
	`allow_answer_swap` BOOLEAN DEFAULT FALSE,
	`teacher_id` BIGINT UNSIGNED,
	`subject_id` VARCHAR(4) NOT NULL,
	`created_at` DATETIME DEFAULT NOW(),
	`updated_at` DATETIME DEFAULT NOW() ON UPDATE NOW(),
	`published_at` DATETIME,
	`status` TINYINT UNSIGNED DEFAULT 0,
	FOREIGN KEY (`teacher_id`) REFERENCES `teachers`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`subject_id`) REFERENCES `subjects`(`id`) ON DELETE CASCADE
);

CREATE TABLE `exam_parts` (	
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`exam_id` BIGINT UNSIGNED NOT NULL,
	`part_title` VARCHAR(255),
	`question_types` JSON -- text[]
);

CREATE TABLE `exam_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`exam_part_id` BIGINT UNSIGNED NOT NULL,
	`question_id` BIGINT UNSIGNED NOT NULL,
	`order_index` TINYINT UNSIGNED NOT NULL,
	`point` DECIMAL(5,2) UNSIGNED CHECK (`point` >= 0 AND `point` <= 10),
	FOREIGN KEY (`exam_part_id`) REFERENCES `exam_parts`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`question_id`) REFERENCES `questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `exam_attempts` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`exam_id` BIGINT UNSIGNED NOT NULL,
	`student_id` BIGINT UNSIGNED NOT NULL,
	`total_point` DECIMAL(5,2) UNSIGNED CHECK (`total_point` >= 0 AND `total_point` <= 10),
	`started_at` DATETIME,
	`duration` SMALLINT UNSIGNED,
	`submitted_at` DATETIME DEFAULT NOW(),
	`marked_at` DATETIME,
	`comment` TEXT,
	`is_practice` BOOLEAN DEFAULT FALSE,
	`part_order` JSON, -- bigint[]
	FOREIGN KEY (`exam_id`) REFERENCES `exams`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`student_id`) REFERENCES `students`(`id`) ON DELETE CASCADE
);

CREATE TABLE `exam_attempt_answers` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`exam_attempt_id` BIGINT UNSIGNED NOT NULL,
	`exam_question_id` BIGINT UNSIGNED NOT NULL,
	`answer_order` JSON, -- text[],
	`student_answer` JSON, -- text[],
	`correct` JSON, -- tinyint[],
	`point` DECIMAL(5,2),
	FOREIGN KEY (`exam_attempt_id`) REFERENCES `exam_attempts`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`exam_question_id`) REFERENCES `exam_questions`(`id`) ON DELETE CASCADE
);

CREATE TABLE `pdf_exam_codes` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`exam_id` BIGINT UNSIGNED NOT NULL,
	`code` CHAR(3) NOT NULL,
	`task_pdf` LONGTEXT,
	`answer_pdf` LONGTEXT,
	`num_part` TINYINT,
	FOREIGN KEY (`exam_id`) REFERENCES `exams`(`id`) ON DELETE CASCADE
);

CREATE TABLE `pdf_exam_codes_questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`pdf_exam_code_id` BIGINT UNSIGNED,
	`type` ENUM("multiple-choice", "true-false", "short-answer", "gap-fill", "constructed-response", "sorting", "true-false-thpt") NOT NULL,
	`part_index` SMALLINT UNSIGNED DEFAULT 0,
	`question_index` SMALLINT UNSIGNED DEFAULT 0,
	`answer_key` TEXT,
	`point` DECIMAL(5,2) UNSIGNED CHECK (`point` >= 0 AND `point` <= 10),
	FOREIGN KEY (`pdf_exam_code_id`) REFERENCES `pdf_exam_codes`(`id`) ON DELETE CASCADE
);

CREATE TABLE `pdf_exam_attempts` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`pdf_exam_code_id` BIGINT UNSIGNED NOT NULL,
	`student_answer` JSON NOT NULL,
	`correct_board` JSON NOT NULL, -- tinyint[]
	`point_board` JSON NOT NULL, -- decimal[]
	FOREIGN KEY (`pdf_exam_code_id`) REFERENCES `pdf_exam_codes`(`id`) ON DELETE CASCADE
);

CREATE TABLE `comments` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`exam_id` BIGINT UNSIGNED NOT NULL,
	`user_id` BIGINT UNSIGNED NOT NULL,
	`reply_to` BIGINT UNSIGNED,
	`content` MEDIUMTEXT NOT NULL,
	`created_at` DATETIME DEFAULT NOW(),
	FOREIGN KEY (`exam_id`) REFERENCES `exams`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`reply_to`) REFERENCES `comments`(`id`) ON DELETE CASCADE
);

CREATE TABLE `student_histories` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`student_id` BIGINT UNSIGNED,
	`exam_id` BIGINT UNSIGNED,
	`time` TIMESTAMP DEFAULT NOW() ON UPDATE NOW(),
	FOREIGN KEY (`student_id`) REFERENCES `students`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`exam_id`) REFERENCES `exams`(`id`)
);

CREATE TABLE `student_reminders` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`student_id` BIGINT UNSIGNED,
	`hour` TIME DEFAULT '12:00:00',
	`date` JSON, -- boolean[]
	`is_active` BOOLEAN DEFAULT TRUE,
	FOREIGN KEY (`student_id`) REFERENCES `students`(`id`) ON DELETE CASCADE,

)

/* Insert data */
INSERT INTO `subjects` (`id`, `name`, `grades`, `questionMC`, `questionTF`, `questionSA`, `questionGF`, `questionST`, `is_visible`) VALUES
("VAN", "Ngữ văn", '[6,7,8,9,10,11,12]', FALSE, FALSE, FALSE, TRUE, FALSE, TRUE),
("TOAN", "Toán", '[6,7,8,9,10,11,12]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
("ANH", "Tiếng Anh", '[6,7,8,9,10,11,12]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
("KHTN", "Khoa học tự nhiên", '[6,7,8,9]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
("LSDL", "Lịch sử & Địa lý", '[6,7,8,9]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
("GDCD", "Giáo dục công dân", '[6,7,8,9]', TRUE, TRUE, FALSE, TRUE, FALSE, FALSE),
("CN", "Công nghệ", '[6,7,8,9]', TRUE, TRUE, FALSE, TRUE, FALSE, TRUE),
("TIN", "Tin học", '[6,7,8,9]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
("CNCN", "Công nghệ (Công nghiệp)", '[10,11,12]', TRUE, TRUE, FALSE, TRUE, FALSE, TRUE),
("CNNN", "Công nghệ (Nông nghiệp)", '[10,11,12]',  TRUE, TRUE, FALSE, TRUE, FALSE, TRUE),
("KTPL", "Giáo dục Kinh tế – Pháp luật", '[10,11,12]', TRUE, TRUE, FALSE, TRUE, FALSE, FALSE),
("LY", "Vật lý", '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
("HOA", "Hóa học", '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
("SINH", "Sinh học", '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
("SU", "Lịch sử", '[10,11,12]', TRUE, TRUE, FALSE, TRUE, TRUE, TRUE),
("DIA", "Địa lý", '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
("ICT", "Tin học (Ứng dụng)", '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
("KHMT", "Tin học (KHMT)", '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE);

INSERT INTO `users` (`id`, `zalo_id`, `name`, `avatar`, `email`, `role`) VALUES
(1, NULL, 'Admin', '/avatar/default.jpg', NULL, 'AD');

INSERT INTO `admins` (`id`, `password`) VALUES
(1, 'AQAAAAIAAYagAAAAEJNR4TJiaNCauvkIJ1qy8f7vvuS4cyGwdxsGDQ44g7w5Ljd6eKHQ0i65M+/6Ntq/vA==');