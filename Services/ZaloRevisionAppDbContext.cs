using backend.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public partial class ZaloRevisionAppDbContext : DbContext
{
    public ZaloRevisionAppDbContext()
    {
    }

    public ZaloRevisionAppDbContext(DbContextOptions<ZaloRevisionAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamAttempt> ExamAttempts { get; set; }

    public virtual DbSet<ExamAttemptAnswer> ExamAttemptAnswers { get; set; }

    public virtual DbSet<ExamPart> ExamParts { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<GroupQuestion> GroupQuestions { get; set; }

    public virtual DbSet<ManualResponseQuestion> ManualResponseQuestions { get; set; }

    public virtual DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }

    public virtual DbSet<PdfExamAttempt> PdfExamAttempts { get; set; }

    public virtual DbSet<PdfExamCode> PdfExamCodes { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<ShortAnswerQuestion> ShortAnswerQuestions { get; set; }

    public virtual DbSet<SortingQuestion> SortingQuestions { get; set; }

    public virtual DbSet<StudentProcess> StudentProcesses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<TrueFalseQuestion> TrueFalseQuestions { get; set; }

    public virtual DbSet<TrueFalseTHPTQuestion> TrueFalseTHPTQuestions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("achievements");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("admins");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.Id)
                .HasConstraintName("admins_ibfk_1");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comments");

            entity.HasIndex(e => e.ExamId, "exam_id");

            entity.HasIndex(e => e.ReplyTo, "reply_to");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.ReplyTo).HasColumnName("reply_to");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Exam).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("comments_ibfk_1");

            entity.HasOne(d => d.ReplyToNavigation).WithMany(p => p.InverseReplyToNavigation)
                .HasForeignKey(d => d.ReplyTo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("comments_ibfk_3");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("comments_ibfk_2");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("exams");

            entity.HasIndex(e => e.ApprovedBy, "approved_by");

            entity.HasIndex(e => e.SubjectId, "subject_id");

            entity.HasIndex(e => e.TeacherId, "teacher_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AllowAnswerSwap)
                .HasDefaultValueSql("'0'")
                .HasColumnName("allow_answer_swap");
            entity.Property(e => e.AllowPartSwap)
                .HasDefaultValueSql("'0'")
                .HasColumnName("allow_part_swap");
            entity.Property(e => e.AllowQuestionSwap)
                .HasDefaultValueSql("'0'")
                .HasColumnName("allow_question_swap");
            entity.Property(e => e.AllowShowScore)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("allow_show_score");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayType)
                .HasDefaultValueSql("'normal'")
                .HasColumnType("enum('normal','pdf')")
                .HasColumnName("display_type");
            entity.Property(e => e.EarlyTurnIn).HasColumnName("early_turn_in");
            entity.Property(e => e.ExamType)
                .HasDefaultValueSql("'regular'")
                .HasColumnType("enum('regular','midterm','final','other')")
                .HasColumnName("exam_type");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.State)
                .HasDefaultValueSql("'1'")
                .HasColumnName("state");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(4)
                .HasColumnName("subject_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.TimeLimit).HasColumnName("time_limit");
            entity.Property(e => e.Title)
                .HasMaxLength(256)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.ExamApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("exams_ibfk_3");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("exams_ibfk_1");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ExamTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("exams_ibfk_2");
        });

        modelBuilder.Entity<ExamAttempt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("exam_attempts");

            entity.HasIndex(e => e.ExamId, "exam_id");

            entity.HasIndex(e => e.StudentId, "student_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.IsPractice)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_practice");
            entity.Property(e => e.PartOrder)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<ulong>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("part_order");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.Score)
                .HasColumnType("decimal(4,2) unsigned")
                .HasColumnName("score");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("submitted_at");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamAttempts)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("exam_attempts_ibfk_1");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamAttempts)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("exam_attempts_ibfk_2");
        });

        modelBuilder.Entity<ExamAttemptAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("exam_attempt_answers");

            entity.HasIndex(e => e.ExamAttemptId, "exam_attempt_id");

            entity.HasIndex(e => e.ExamQuestionId, "exam_question_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerOrder)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<string>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("answer_order");
            entity.Property(e => e.AnswerType)
                .HasDefaultValueSql("'string'")
                .HasColumnType("enum('array','string')")
                .HasColumnName("answer_type");
            entity.Property(e => e.ExamAttemptId).HasColumnName("exam_attempt_id");
            entity.Property(e => e.ExamQuestionId).HasColumnName("exam_question_id");
            entity.Property(e => e.IsCorrect)
                .HasColumnType("text")
                .HasColumnName("is_correct");
            entity.Property(e => e.StudentAnswer)
                .HasColumnType("text")
                .HasColumnName("student_answer");

            entity.HasOne(d => d.ExamAttempt).WithMany(p => p.ExamAttemptAnswers)
                .HasForeignKey(d => d.ExamAttemptId)
                .HasConstraintName("exam_attempt_answers_ibfk_1");

            entity.HasOne(d => d.ExamQuestion).WithMany(p => p.ExamAttemptAnswers)
                .HasForeignKey(d => d.ExamQuestionId)
                .HasConstraintName("exam_attempt_answers_ibfk_2");
        });

        modelBuilder.Entity<ExamPart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("exam_parts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.PartIndex).HasColumnName("part_index");
            entity.Property(e => e.PartTitle)
                .HasMaxLength(255)
                .HasColumnName("part_title");
            entity.Property(e => e.QuestionTypes)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<string>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("question_types");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("exam_questions");

            entity.HasIndex(e => e.ExamPartId, "exam_part_id");

            entity.HasIndex(e => e.QuestionId, "question_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExamPartId).HasColumnName("exam_part_id");
            entity.Property(e => e.OrderIndex).HasColumnName("order_index");
            entity.Property(e => e.Point)
                .HasColumnType("decimal(4,2) unsigned")
                .HasColumnName("points");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.ExamPart).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.ExamPartId)
                .HasConstraintName("exam_questions_ibfk_1");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("exam_questions_ibfk_2");
        });

        modelBuilder.Entity<GroupQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("group_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PassageAuthor)
                .HasColumnType("text")
                .HasColumnName("passage-author");
            entity.Property(e => e.PassageContent)
                .HasColumnType("mediumtext")
                .HasColumnName("passage-content");
            entity.Property(e => e.PassageTitle)
                .HasMaxLength(255)
                .HasColumnName("passage-title");
            entity.Property(e => e.Questions)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<ulong>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("questions");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.GroupQuestion)
                .HasForeignKey<GroupQuestion>(d => d.Id)
                .HasConstraintName("group_questions_ibfk_1");
        });

        modelBuilder.Entity<ManualResponseQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("manual_response_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AllowEnter).HasColumnName("allow_enter");
            entity.Property(e => e.AllowTakePhoto).HasColumnName("allow_take_photo");
            entity.Property(e => e.AnswerKeys)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<string>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("answer_keys");
            entity.Property(e => e.MarkAsWrong).HasColumnName("mark_as_wrong");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ManualResponseQuestion)
                .HasForeignKey<ManualResponseQuestion>(d => d.Id)
                .HasConstraintName("manual_response_questions_ibfk_1");
        });

        modelBuilder.Entity<MultipleChoiceQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("multiple_choice_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectAnswer)
                .HasColumnType("text")
                .HasColumnName("correct_answer");
            entity.Property(e => e.WrongAnswer1)
                .HasColumnType("text")
                .HasColumnName("wrong_answer_1");
            entity.Property(e => e.WrongAnswer2)
                .HasColumnType("text")
                .HasColumnName("wrong_answer_2");
            entity.Property(e => e.WrongAnswer3)
                .HasColumnType("text")
                .HasColumnName("wrong_answer_3");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.MultipleChoiceQuestion)
                .HasForeignKey<MultipleChoiceQuestion>(d => d.Id)
                .HasConstraintName("multiple_choice_questions_ibfk_1");
        });

        modelBuilder.Entity<PdfExamAttempt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pdf_exam_attempts");

            entity.HasIndex(e => e.CodeId, "code_id");

            entity.HasIndex(e => e.ExamId, "exam_id");

            entity.HasIndex(e => e.StudentId, "student_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CodeId).HasColumnName("code_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.CorrectBoard)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<bool>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("correct_board");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.Score)
                .HasColumnType("decimal(4,2) unsigned")
                .HasColumnName("score");
            entity.Property(e => e.StudentAnswer)
                .HasColumnType("json")
                .HasColumnName("student_answer");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("submitted_at");

            entity.HasOne(d => d.Code).WithMany(p => p.PdfExamAttempts)
                .HasForeignKey(d => d.CodeId)
                .HasConstraintName("pdf_exam_attempts_ibfk_3");

            entity.HasOne(d => d.Exam).WithMany(p => p.PdfExamAttempts)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("pdf_exam_attempts_ibfk_1");

            entity.HasOne(d => d.Student).WithMany(p => p.PdfExamAttempts)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("pdf_exam_attempts_ibfk_2");
        });

        modelBuilder.Entity<PdfExamCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pdf_exam_codes");

            entity.HasIndex(e => e.ExamId, "exam_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerKey)
                .HasColumnType("json")
                .HasColumnName("answer_keys");
            entity.Property(e => e.Code)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("code");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");

            entity.HasOne(d => d.Exam).WithMany(p => p.PdfExamCodes)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("pdf_exam_codes_ibfk_1");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("questions");

            entity.HasIndex(e => e.SubjectId, "subject_id");

            entity.HasIndex(e => e.TopicId, "topic_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.Explanation)
                .HasColumnType("text")
                .HasColumnName("explanation");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(4)
                .HasColumnName("subject_id");
            entity.Property(e => e.Title)
                .HasColumnType("text")
                .HasColumnName("title");
            entity.Property(e => e.TopicId)
                .HasMaxLength(10)
                .HasColumnName("topic_id");
            entity.Property(e => e.Type)
                .HasColumnType("enum('multiple-choice','true-false','short-answer','fill-in-the-blank','constructed-response','sorting','group','true-false-thpt')")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Subject).WithMany(p => p.Questions)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("questions_ibfk_1");

            entity.HasOne(d => d.Topic).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("questions_ibfk_2");
        });

        modelBuilder.Entity<ShortAnswerQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("short_answer_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerKey)
                .HasMaxLength(4)
                .HasColumnName("answer_key");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ShortAnswerQuestion)
                .HasForeignKey<ShortAnswerQuestion>(d => d.Id)
                .HasConstraintName("short_answer_questions_ibfk_1");
        });

        modelBuilder.Entity<SortingQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sorting_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectOrder)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<string>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("correct_order");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SortingQuestion)
                .HasForeignKey<SortingQuestion>(d => d.Id)
                .HasConstraintName("sorting_questions_ibfk_1");
        });

        modelBuilder.Entity<StudentProcess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("student_process");

            entity.HasIndex(e => e.StudentId, "student_id");

            entity.HasIndex(e => e.SubjectId, "subject_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvgScore)
                .HasPrecision(4)
                .HasColumnName("avg_score");
            entity.Property(e => e.CorrectRate)
                .HasPrecision(5)
                .HasColumnName("correct_rate");
            entity.Property(e => e.LastActivity)
                .HasColumnType("datetime")
                .HasColumnName("last_activity");
            entity.Property(e => e.StreakDays).HasColumnName("streak_days");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(4)
                .HasColumnName("subject_id");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentProcesses)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("student_process_ibfk_2");

            entity.HasOne(d => d.Subject).WithMany(p => p.StudentProcesses)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("student_process_ibfk_1");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("subjects");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(4)
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Grades)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<byte>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("grades");
            entity.Property(e => e.IsVisible)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_visible");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.QuestionDS).HasColumnName("questionDS");
            entity.Property(e => e.QuestionDVCT).HasColumnName("questionDVCT");
            entity.Property(e => e.QuestionSX).HasColumnName("questionSX");
            entity.Property(e => e.QuestionTL).HasColumnName("questionTL");
            entity.Property(e => e.QuestionTLN).HasColumnName("questionTLN");
            entity.Property(e => e.QuestionTN).HasColumnName("questionTN");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("teachers");

            entity.HasIndex(e => e.SubjectId, "subject_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Grades)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<byte>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("grades");
            entity.Property(e => e.Introduction)
                .HasColumnType("text")
                .HasColumnName("introduction");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(4)
                .HasColumnName("subject_id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.Id)
                .HasConstraintName("teachers_ibfk_2");

            entity.HasOne(d => d.Subject).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("teachers_ibfk_1");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("topics");

            entity.HasIndex(e => e.SubjectId, "subject_id");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Grades)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<byte>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("grades");
            entity.Property(e => e.IsVisible)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_visible");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(4)
                .HasColumnName("subject_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Subject).WithMany(p => p.Topics)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("topics_ibfk_1");
        });

        modelBuilder.Entity<TrueFalseQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("true_false_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerKey).HasColumnName("answer_key");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.TrueFalseQuestion)
                .HasForeignKey<TrueFalseQuestion>(d => d.Id)
                .HasConstraintName("true_false_questions_ibfk_1");
        });

        modelBuilder.Entity<TrueFalseTHPTQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("true_false_thpt_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerKeys)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<bool>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("answer_keys");
            entity.Property(e => e.PassageAuthor)
                .HasColumnType("text")
                .HasColumnName("passage-author");
            entity.Property(e => e.PassageContent)
                .HasColumnType("mediumtext")
                .HasColumnName("passage-content");
            entity.Property(e => e.PassageTitle)
                .HasMaxLength(255)
                .HasColumnName("passage-title");
            entity.Property(e => e.Statements)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<string>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("statements");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.TrueFalseTHPTQuestion)
                .HasForeignKey<TrueFalseTHPTQuestion>(d => d.Id)
                .HasConstraintName("true_false_thpt_questions_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasColumnType("text")
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'student'")
                .HasColumnType("enum('student','teacher','hos','admin')")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.ZaloId)
                .HasColumnType("text")
                .HasColumnName("zalo_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
