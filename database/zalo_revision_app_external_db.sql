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
	`role` ENUM('HS', "GV", "AD") NOT NULL DEFAULT "HS",
	`notification` JSON, -- boolean[]
	`created_at` DATETIME DEFAULT NOW(),
	`updated_at` DATETIME DEFAULT NOW() ON UPDATE NOW()
);

CREATE TABLE `students` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`favorites` JSON, -- bigint[]
	`achievements` JSON, -- smallint[]
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

-- CREATE TABLE `group_questions` (
-- 	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
-- 	`passage_title` VARCHAR(255),
-- 	`passage_content` MEDIUMTEXT,
-- 	`passage_author` TEXT
-- );

CREATE TABLE `questions` (
	`id` BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
	`title` TEXT NOT NULL,
	`image_url` TEXT,
	`grade` TINYINT UNSIGNED,
	`type` ENUM('multiple-choice', 'true-false', 'short-answer', 'gap-fill', 'constructed-response', 'sorting', 'true-false-thpt') NOT NULL,
	`difficulty` TINYINT UNSIGNED,
	`topic_id` VARCHAR(10),
	`teacher_id` BIGINT UNSIGNED NOT NULL,
	-- `group_id` BIGINT UNSIGNED,
	`explanation` TEXT,
	`created_at` DATETIME DEFAULT NOW(),
	`updated_at` DATETIME DEFAULT NOW() ON UPDATE NOW(),
	FOREIGN KEY (`topic_id`) REFERENCES `topics`(`id`) ON DELETE CASCADE,
	FOREIGN KEY (`teacher_id`) REFERENCES `teachers`(`id`) ON DELETE CASCADE
	-- FOREIGN KEY (`group_id`) REFERENCES `group_questions`(`id`) ON DELETE CASCADE
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
	`exam_type` ENUM('regular', 'midterm', 'final', 'other') NOT NULL DEFAULT 'regular',
	`display_type` ENUM('normal', 'pdf') NOT NULL DEFAULT 'normal',
	`title` VARCHAR(255) NOT NULL,
	`grade` TINYINT UNSIGNED NOT NULL,
	`time_limit` SMALLINT UNSIGNED NOT NULL,
	`early_turn_in` SMALLINT UNSIGNED,
	`allow_show_score` BOOLEAN DEFAULT TRUE NOT NULL,
	`allow_part_swap` BOOLEAN DEFAULT FALSE,
	`allow_question_swap` BOOLEAN DEFAULT FALSE,
	`allow_answer_swap` BOOLEAN DEFAULT FALSE,
	`teacher_id` BIGINT UNSIGNED NOT NULL,
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
	`type` ENUM('multiple-choice', 'true-false', 'short-answer', 'gap-fill', 'constructed-response', 'sorting', 'true-false-thpt') NOT NULL,
	`part_index` SMALLINT UNSIGNED DEFAULT 0,
	`question_index` SMALLINT UNSIGNED DEFAULT 0,
	`answer_key` TEXT,
	`point` DECIMAL(5,2) UNSIGNED CHECK (`point` >= 0 AND `point` <= 10),
	FOREIGN KEY (`pdf_exam_code_id`) REFERENCES `pdf_exam_codes`(`id`) ON DELETE CASCADE
);

CREATE TABLE `pdf_exam_attempts` (
	`id` BIGINT UNSIGNED PRIMARY KEY,
	`pdf_exam_code_id` BIGINT UNSIGNED NOT NULL,
	`student_answer` JSON NOT NULL,
	`correct_board` JSON NOT NULL, -- tinyint[]
	`point_board` JSON NOT NULL, -- decimal[]
	FOREIGN KEY (`id`) REFERENCES `exam_attempts`(`id`) ON DELETE CASCADE,
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
	FOREIGN KEY (`student_id`) REFERENCES `students`(`id`),
	FOREIGN KEY (`exam_id`) REFERENCES `exams`(`id`)
);

/* Insert data */
INSERT INTO `subjects` (`id`, `name`, `grades`, `questionMC`, `questionTF`, `questionSA`, `questionGF`, `questionST`, `is_visible`) VALUES
('VAN', 'Ngữ văn', '[6,7,8,9,10,11,12]', FALSE, FALSE, FALSE, TRUE, FALSE, TRUE),
('TOAN', 'Toán', '[6,7,8,9,10,11,12]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
('ANH', 'Tiếng Anh', '[6,7,8,9,10,11,12]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
('KHTN', 'Khoa học tự nhiên', '[6,7,8,9]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
('LSDL', 'Lịch sử & Địa lý', '[6,7,8,9]', TRUE, TRUE, TRUE, TRUE, TRUE, TRUE),
('GDCD', 'Giáo dục công dân', '[6,7,8,9]', TRUE, TRUE, FALSE, TRUE, FALSE, FALSE),
('CN', 'Công nghệ', '[6,7,8,9]', TRUE, TRUE, FALSE, TRUE, FALSE, TRUE),
('TIN', 'Tin học', '[6,7,8,9]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
('CNCN', 'Công nghệ (Công nghiệp)', '[10,11,12]', TRUE, TRUE, FALSE, TRUE, FALSE, TRUE),
('CNNN', 'Công nghệ (Nông nghiệp)', '[10,11,12]',  TRUE, TRUE, FALSE, TRUE, FALSE, TRUE),
('KTPL', 'Giáo dục Kinh tế – Pháp luật', '[10,11,12]', TRUE, TRUE, FALSE, TRUE, FALSE, FALSE),
('LY', 'Vật lý', '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
('HOA', 'Hóa học', '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
('SINH', 'Sinh học', '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
('SU', 'Lịch sử', '[10,11,12]', TRUE, TRUE, FALSE, TRUE, TRUE, TRUE),
('DIA', 'Địa lý', '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
('ICT', 'Tin học (Ứng dụng)', '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE),
('KHMT', 'Tin học (KHMT)', '[10,11,12]', TRUE, TRUE, TRUE, TRUE, FALSE, TRUE);

INSERT INTO `topics` (`id`, `name`, `grades`, `subject_id`, `is_visible`) VALUES
('ANH1', 'Thì hiện tại đơn', '[6, 7, 8, 9, 10, 11, 12]', 'ANH', TRUE),
('ANH2', 'Thì hiện tại hoàn thành', '[10, 11, 12]', 'ANH', TRUE),
('TIN1', 'Hệ điều hành và phần mềm ứng dụng', '[7]', 'TIN', TRUE),
('TIN2', 'Lập trình trực quan', '[8]', 'TIN', FALSE),
('TOAN1', 'Số nguyên', '[6]', 'TOAN', TRUE),
('TOAN2', 'Tính đơn điệu của hàm số', '[12]', 'TOAN', TRUE),
('TOAN3', 'Số tự nhiên', '[6]', 'TOAN', TRUE);

INSERT INTO `users` (`id`, `zalo_id`, `name`, `avatar`, `email`, `role`, `notification`, `created_at`, `updated_at`) VALUES
(1, NULL, 'Admin', '/avatar/default.jpg', NULL, 'AD', NULL, '2025-11-25 17:31:33', '2025-11-25 17:31:33'),
(2, NULL, 'Thảo Lê', 'https://h5.zdn.vn/static/images/avatar.png', 'softwaretran@gmail.com', 'GV', '[false, false, true]', '2025-11-25 17:31:33', '2025-12-04 22:03:55');

INSERT INTO `teachers` (`id`, `subject_id`, `grades`, `introduction`) VALUES
(2, 'TOAN', '[6, 7, 8, 9]', 'Chào các em! Hi vọng các em có những giờ học thật vui nhé!');

INSERT INTO `admins` (`id`, `password`) VALUES
(1, 'AQAAAAIAAYagAAAAEJNR4TJiaNCauvkIJ1qy8f7vvuS4cyGwdxsGDQ44g7w5Ljd6eKHQ0i65M+/6Ntq/vA==');

INSERT INTO `questions` (`title`, `image_url`, `grade`, `type`, `difficulty`, `topic_id`, `teacher_id`, `explanation`) VALUES
('Cho tập A = {1; 2; 3; 4; 5}. Cho biết phần tử nào không thuộc tập A?', 'https://res.cloudinary.com/dqxhmt5sp/image/upload/v1764832923/tj6rfzrfnisigykwk5qf.png', 6, 'multiple-choice', 1, 'TOAN3', 2, 'Tập hợp A có 5 phần tử: 1, 2, 3, 4, 5. Nhưng không có phần tử 0, nên 0 không thuộc tập A.'),
('Cho tập B = {2; 4; 6; 8; 10}. Đâu là mệnh đề đúng?', 'https://res.cloudinary.com/dqxhmt5sp/image/upload/v1764833037/d5miosuvjjahaebiaihv.png', 6, 'multiple-choice', 1, 'TOAN3', 2, ''),
('Kí hiệu của tập hợp số tự nhiên là:', NULL, 6, 'multiple-choice', 1, 'TOAN3', 2, ''),
('Số tự nhiên nhỏ nhất là:', NULL, 6, 'multiple-choice', 2, 'TOAN3', 2, ''),
('Số nguyên tố chẵn duy nhất là:', NULL, 6, 'multiple-choice', 2, 'TOAN3', 2, 'Số nguyên tố là số tự nhiên lớn hơn 1, chỉ có hai ước số dương là 1 và chính nó. Số 2 thỏa mãn khái niệm, và mọi số chẵn đều chia hết cho 2, nên 2 là số nguyên tố chẵn duy nhất.'),
('Hợp số nhỏ nhất là:', NULL, 6, 'multiple-choice', 2, 'TOAN3', 2, 'Hợp số là một số tự nhiên lớn hơn 1 và có nhiều hơn hai ước số (bao gồm 1 và chính nó). Dựa trên khái niệm, số nhỏ nhất thỏa mãn là 4 (do 2 và 3 là các số nguyên tố).'),
('Số nào chia hết cho 3?', NULL, 6, 'multiple-choice', 3, 'TOAN3', 2, '4 + 7 + 2 + 8 = 21 chia hết cho 3 => 4728 chia hết cho 3\\n\n1 + 2 + 3 + 5 = 11 không chia hết cho 3 => 1235 không chia hết cho 3\\n\n3 + 7 + 3 + 7 = 20 không chia hết cho 3 => 3737 không chia hết cho 3\\n\n5 + 7 + 6 + 1 = 19 không chia hết cho 3 => 5761 không chia hết cho 3'),
('Các quả banh màu xanh đều là hợp số.', 'https://res.cloudinary.com/dqxhmt5sp/image/upload/gxiqgb4a4zh6ixhnudum.png', 6, 'true-false', 1, 'TOAN1', 2, ''),
('Cho a, b, 100, c là 4 số tự nhiên liên tiếp giảm dần. Xét tính đúng sai của các mệnh đề sau:', NULL, 6, 'true-false-thpt', 4, 'TOAN3', 2, 'a, b, 100, c là 4 số tự nhiên liên tiếp giảm dần => a = 102; b = 101; c = 99\\n\n- MĐ1: b = 101 > 100 => Đúng.\\n\n- MĐ2: c là số nhất => Đúng (102 > 101 > 100 > 99).\\n\n- MĐ3: b - c = 101 - 99 = 2 ≠ 1 => Sai.\\n\n- MĐ4: a + b + c = 302, lớn hơn hoặc bằng 302 => Đúng.'),
('Tìm ước chung lớn nhất của 20 và 28.', NULL, 6, 'short-answer', 3, 'TOAN3', 2, ''),
('Tìm số tự nhiên n = 5b2d, biết n chia hết cho cả 2, 5 và 9?', NULL, 6, 'short-answer', 4, 'TOAN3', 2, 'n chia hết cho 2 và 5 => d = 0\n5 + b + 2 phải chia hết cho 9 => b = 2 => Số n = 5220'),
('Số 0 là bội của ___ khác 0', NULL, 6, 'gap-fill', 1, 'TOAN3', 2, ''),
('Mọi số tự nhiên lớn hơn 1 luôn có ít nhất 2 ước là ___.', NULL, 6, 'gap-fill', 1, 'TOAN3', 2, ''),
('37 là ___ của 37.', NULL, 6, 'gap-fill', 1, 'TOAN3', 2, "37 vừa là bội, vừa là ước của 37, nên các em có thể điền 'ước và bội', hoặc 'bội và ước' đêu được."),
('Sắp xếp thứ tự thực hiện phép tính đối với biểu thức không có dấu ngoặc:', NULL, 6, 'sorting', 1, 'TOAN3', 2, ''),
('Trình bày lời giải cho bài toán sau: Chị Hòa có 1 số bông sen. Nếu chị bó thành các bó gồm 3 bông, 5 bông, hay 7 bông thì đều vừa hết. Hỏi chị Hòa có bao nhiêu bông sen, biết rằng chị Hòa có khoảng từ 200 đến 300 bông?', NULL, 6, 'constructed-response', 3, 'TOAN3', 2, 'Gọi x là số bông sen của chị Hòa (200 < x < 300).\\n\n“Nếu chị bó thành các bó gồm 3 bông, 5 bông, hay 7 bông thì đều vừa hết” => x ∈ BC(3, 5, 7).\\n\nTa có BC(3, 5, 7) = {105, 210, 315…} => x = 210 (bông).\\n\nVậy: Chị Hòa có 210 bông sen.'),
('Năm nhuận đầu tiên của thế kỉ XXI là?', NULL, 6, 'short-answer', 1, 'TOAN3', 2, 'Vì năm 2000 thuộc thế kỉ XX, nên năm đầu tiên thỏa mãn là 2004 (vì thuộc thế kỉ XXI, và 2004 chia hết cho 4).');

INSERT INTO `multiple_choice_questions` (`id`, `correct_answer`, `wrong_answer`) VALUES
(1, '0', '["2", "3", "5"]'),
(2, '2 ∈ B', '["1 ∈ B", "3 ∉ B", "4 ∉ B"]'),
(3, 'N', '["Z", "Q", "R"]'),
(4, '0', '["1", "2", "3"]'),
(5, '2', '["0", "4", "8"]'),
(6, '4', '["0", "1", "3"]'),
(7, '4728', '["1235", "3737", "5761"]');

INSERT INTO `true_false_questions` (`id`, `answer_key`) VALUES (8, 1);

INSERT INTO `short_answer_questions` (`id`, `answer_key`) VALUES
(10, '4'),
(11, '5220'),
(17, '2004');

INSERT INTO `manual_response_questions` (`id`, `answer_keys`, `allow_take_photo`, `allow_enter`, `mark_as_wrong`) VALUES
(12, '["tất cả các số tự nhiên", "các số tự nhiên"]', NULL, NULL, 1),
(13, '["1 và chính nó"]', NULL, NULL, 1),
(14, '["ước và bội", "bội và ước"]', NULL, NULL, 1),
(16, '[]', 1, 1, 0);

INSERT INTO `sorting_questions` (`id`, `correct_order`) VALUES
(15, '["Lũy thừa", "Nhân và chia", "Cộng và trừ"]');

INSERT INTO `true_false_thpt_questions` (`id`, `passage_title`, `passage_content`, `passage_author`, `statements`, `answer_keys`) VALUES
(9, NULL, NULL, NULL, '["b > 100", "c là số nhỏ nhất.", "b – c = 1", "a + b + c ≥ 302"]', '[true, true, false, true]');

INSERT INTO `exams` (`id`, `exam_type`, `display_type`, `title`, `grade`, `time_limit`, `early_turn_in`, `allow_show_score`, `allow_part_swap`, `allow_question_swap`, `allow_answer_swap`, `teacher_id`, `subject_id`, `created_at`, `updated_at`, `published_at`, `status`) VALUES
(1, 'regular', 'normal', 'Kiểm tra thường xuyên (đợt 1)', 6, 600, 300, 1, 1, 1, 1, 2, 'TOAN', '2025-11-25 17:31:33', '2025-11-30 13:31:38', '2025-11-25 21:22:27', 3),
(2, 'regular', 'pdf', 'Kiểm tra thường xuyên (đợt 2)', 6, 600, 300, 1, 0, NULL, NULL, 2, 'TOAN', '2025-11-25 17:31:33', '2025-12-04 15:31:29', NULL, 1),
(3, 'regular', 'pdf', 'Kiểm tra thường xuyên (đợt 3)', 6, 900, 600, 1, 0, NULL, NULL, 2, 'TOAN', '2025-12-01 01:03:04', '2025-12-04 15:32:17', NULL, 0);

INSERT INTO `exam_parts` (`id`, `exam_id`, `part_title`, `question_types`) VALUES
(1, 1, 'Trắc nghiệm khách quan', '["multiple-choice", "multiple-choice", "multiple-choice", "multiple-choice"]'),
(2, 1, 'Trắc nghiệm Đúng - Sai', '["true-false", "true-false-thpt"]'),
(3, 1, 'Sắp xếp', '["sorting"]'),
(4, 1, 'Điền vào chỗ trống', '["gap-fill", "gap-fill", "gap-fill"]'),
(5, 1, 'Tự luận', '["short-answer", "constructed-response"]');

INSERT INTO `exam_questions` (`id`, `exam_part_id`, `question_id`, `order_index`, `point`) VALUES
(1, 1, 1, 1, 0.50),
(2, 1, 2, 1, 0.50),
(3, 1, 3, 2, 0.50),
(4, 1, 4, 2, 0.50),
(5, 1, 5, 3, 0.50),
(6, 1, 6, 3, 0.50),
(7, 1, 7, 4, 0.50),
(8, 2, 8, 1, 1.00),
(9, 2, 9, 2, 1.00),
(10, 3, 17, 1, 1.50),
(11, 4, 13, 1, 0.50),
(12, 4, 14, 2, 0.50),
(13, 4, 15, 3, 0.50),
(14, 5, 10, 1, 1.00),
(15, 5, 11, 1, 1.00),
(16, 5, 16, 2, 2.00);

INSERT INTO `pdf_exam_codes` (`exam_id`, `code`, `task_pdf`, `answer_pdf`, `num_part`) VALUES
(2, '101', '/pdf/sample.pdf', '/pdf/sample.pdf', 3),
(2, '793', '/pdf/sample.pdf', '/pdf/sample.pdf', 3),
(3, '123', 'https://res.cloudinary.com/dqxhmt5sp/raw/upload/emigmoyuamd7l5nloeru.pdf', 'https://res.cloudinary.com/dqxhmt5sp/raw/upload/v1764845978/szjybcjvlaclsplpti4d.pdf', 3),
(3, '231', 'https://res.cloudinary.com/dqxhmt5sp/raw/upload/v1764845980/ec5jqlapbvpwi4drslu2.pdf', 'https://res.cloudinary.com/dqxhmt5sp/raw/upload/v1764845980/h2yemgodguruck1kffot.pdf', 3);

INSERT INTO `pdf_exam_codes_questions` (`id`, `pdf_exam_code_id`, `type`, `part_index`, `question_index`, `answer_key`, `point`) VALUES
(1, 1, 'multiple-choice', 1, 1, 'A', 0.50),
(2, 1, 'multiple-choice', 1, 2, 'B', 0.50),
(3, 1, 'multiple-choice', 1, 3, 'C', 0.50),
(4, 1, 'multiple-choice', 1, 4, 'D', 0.50),
(5, 1, 'multiple-choice', 1, 5, 'B', 0.50),
(6, 1, 'multiple-choice', 1, 6, 'C', 0.50),
(7, 1, 'multiple-choice', 1, 7, 'A', 0.50),
(8, 1, 'true-false', 2, 1, 'S', 0.50),
(9, 1, 'true-false-thpt', 2, 2, 'SSSĐ', 1.00),
(10, 1, 'true-false-thpt', 2, 3, 'ĐSĐĐ', 1.00),
(11, 1, 'short-answer', 3, 1, '2025', 1.00),
(12, 1, 'gap-fill', 3, 2, '', 1.00),
(13, 1, 'constructed-response', 3, 3, '', 2.00),
(14, 2, 'true-false', 1, 1, 'Đ', 0.50),
(15, 2, 'true-false-thpt', 1, 2, 'SĐSĐ', 1.00),
(16, 2, 'true-false-thpt', 1, 3, 'ĐSĐS', 1.00),
(17, 2, 'multiple-choice', 2, 1, 'D', 0.50),
(18, 2, 'multiple-choice', 2, 2, 'C', 0.50),
(19, 2, 'multiple-choice', 2, 3, 'B', 0.50),
(20, 2, 'multiple-choice', 2, 4, 'A', 0.50),
(21, 2, 'multiple-choice', 2, 5, 'D', 0.50),
(22, 2, 'multiple-choice', 2, 6, 'A', 0.50),
(23, 2, 'multiple-choice', 2, 7, 'C', 0.50),
(24, 2, 'gap-fill', 3, 1, '', 1.00),
(25, 2, 'short-answer', 3, 2, '2025', 1.00),
(26, 2, 'constructed-response', 3, 3, '', 2.00),
(27, 3, 'multiple-choice', 1, 1, 'A', 1.00),
(28, 3, 'multiple-choice', 1, 2, 'B', 1.00),
(29, 3, 'multiple-choice', 1, 3, 'C', 1.00),
(30, 3, 'multiple-choice', 1, 4, 'A', 1.00),
(31, 3, 'multiple-choice', 1, 5, 'B', 1.00),
(32, 3, 'true-false', 2, 1, 'Đ', 0.50),
(33, 3, 'true-false', 2, 2, 'S', 0.50),
(34, 3, 'true-false-thpt', 2, 3, 'ĐSĐS', 1.00),
(35, 3, 'constructed-response', 3, 1, '', 3.00),
(36, 4, 'multiple-choice', 1, 1, 'D', 1.00),
(37, 4, 'multiple-choice', 1, 2, 'C', 1.00),
(38, 4, 'multiple-choice', 1, 3, 'A', 1.00),
(39, 4, 'multiple-choice', 1, 4, 'C', 1.00),
(40, 4, 'multiple-choice', 1, 5, 'D', 1.00),
(41, 4, 'true-false-thpt', 2, 1, 'SĐSS', 1.00),
(42, 4, 'true-false', 2, 2, 'Đ', 0.50),
(43, 4, 'true-false', 2, 3, 'Đ', 0.50),
(44, 4, 'constructed-response', 3, 1, '', 3.00);