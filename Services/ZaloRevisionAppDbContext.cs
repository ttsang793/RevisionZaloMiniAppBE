using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;

using backend.Models;
using MySql.EntityFrameworkCore.Extensions;

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

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<Achievement> Achivements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("subjects");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(4)
                .HasColumnName("id");
            entity.Property(e => e.Classes)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<int>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("classes");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
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
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("update_at");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("topics");

            entity.HasIndex(e => e.SubjectId, "subject_id");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("id");
            entity.Property(e => e.Classes)
                .HasColumnType("json")
                .HasConversion(
                      v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                      v => JsonSerializer.Deserialize<List<int>>(v!, new JsonSerializerOptions())
                  )
                .HasColumnName("classes");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.IsVisible)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_visible");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(4)
                .HasColumnName("subject_id");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Subject).WithMany(p => p.Topics)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("topics_ibfk_1");
        });

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("achievements");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");

            entity.Property(e => e.Description)
                .HasMaxLength(256)
                .HasColumnName("description");

            entity.Property(e => e.Image)
                .HasColumnName("image");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
