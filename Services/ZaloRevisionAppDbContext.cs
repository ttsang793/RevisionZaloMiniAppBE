using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using backend.Models;

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

    public virtual DbSet<ConstructedResponseQuestion> ConstructedResponseQuestions { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<FillInTheBlankQuestion> FillInTheBlankQuestions { get; set; }

    public virtual DbSet<GroupQuestion> GroupQuestions { get; set; }

    public virtual DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<ShortAnswerQuestion> ShortAnswerQuestions { get; set; }

    public virtual DbSet<SortingQuestion> SortingQuestions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<TrueFalseQuestion> TrueFalseQuestions { get; set; }

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
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

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
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExamId).HasColumnName("exam_id");
            entity.Property(e => e.ReplyTo).HasColumnName("reply_to");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Exam).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ExamId)
                .HasConstraintName("comments_ibfk_1");

            entity.HasOne(d => d.ReplyToNavigation).WithMany(p => p.CommentReplyToNavigations)
                .HasForeignKey(d => d.ReplyTo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("comments_ibfk_3");

            entity.HasOne(d => d.User).WithMany(p => p.CommentUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("comments_ibfk_2");
        });

        modelBuilder.Entity<ConstructedResponseQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("constructed_response_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AllowEnter).HasColumnName("allow_enter");
            entity.Property(e => e.AllowTakePhoto).HasColumnName("allow_take_photo");
            entity.Property(e => e.AnswerKeys)
                .HasColumnType("json")                
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<string>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("answer_key");
            entity.Property(e => e.MarkAsWrong).HasColumnName("mark_as_wrong");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ConstructedResponseQuestion)
                .HasForeignKey<ConstructedResponseQuestion>(d => d.Id)
                .HasConstraintName("constructed_response_questions_ibfk_1");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("exams");

            entity.HasIndex(e => e.ApprovedBy, "approved_by");

            entity.HasIndex(e => e.SubjectId, "subject_id");

            entity.HasIndex(e => e.TeacherId, "teacher_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AllowAnswerSwap).HasColumnName("allow_answer_swap");
            entity.Property(e => e.AllowPartSwap).HasColumnName("allow_part_swap");
            entity.Property(e => e.AllowQuestionSwap).HasColumnName("allow_question_swap");
            entity.Property(e => e.AllowShowScore).HasColumnName("allow_show_score");
            entity.Property(e => e.AllowTurnInTime).HasColumnName("allow_turn_in_time");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayType)
                .HasColumnType("enum('NORMAL','PDF')")
                .HasColumnName("display_type");
            entity.Property(e => e.ExamType).HasColumnName("exam_type");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(4)
                .HasColumnName("subject_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.TimeLimit).HasColumnName("time_limit");
            entity.Property(e => e.Title)
                .HasMaxLength(256)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.ExamApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("exams_ibfk_3");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("exams_ibfk_1");

            entity.HasOne(d => d.Teacher).WithMany(p => p.ExamTeachers)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("exams_ibfk_2");
        });

        modelBuilder.Entity<FillInTheBlankQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fill_in_the_blank_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerKeys)
                .HasColumnType("json")                
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<string>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("answer_key");
            entity.Property(e => e.MarkAsWrong).HasColumnName("mark_as_wrong");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.FillInTheBlankQuestion)
                .HasForeignKey<FillInTheBlankQuestion>(d => d.Id)
                .HasConstraintName("fill_in_the_blank_questions_ibfk_1");
        });

        modelBuilder.Entity<GroupQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("group_questions");

            entity.Property(e => e.Id).HasColumnName("id");
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

        modelBuilder.Entity<MultipleChoiceQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("multiple_choice_questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerA)
                .HasColumnType("text")
                .HasColumnName("answer_a");
            entity.Property(e => e.AnswerB)
                .HasColumnType("text")
                .HasColumnName("answer_b");
            entity.Property(e => e.AnswerC)
                .HasColumnType("text")
                .HasColumnName("answer_c");
            entity.Property(e => e.AnswerD)
                .HasColumnType("text")
                .HasColumnName("answer_d");
            entity.Property(e => e.AnswerKey)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("answer_key");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.MultipleChoiceQuestion)
                .HasForeignKey<MultipleChoiceQuestion>(d => d.Id)
                .HasConstraintName("multiple_choice_questions_ibfk_1");
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
            entity.Property(e => e.Explaination)
                .HasColumnType("text")
                .HasColumnName("explaination");
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
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
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
                .HasPrecision(4)
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

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("students");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RealName)
                .HasMaxLength(255)
                .HasColumnName("real_name");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.Id)
                .HasConstraintName("students_ibfk_1");
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
            entity.Property(e => e.Role)
                .HasColumnType("enum('teacher','hos')")
                .HasColumnName("role");
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
            entity.Property(e => e.UpdatedAt)
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
