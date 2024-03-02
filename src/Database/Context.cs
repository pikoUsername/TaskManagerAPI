﻿using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using TaskManager.Database.Models; 

namespace TaskManager.Database; 

public class TaskManagerContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<TaskTag> TaskTags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<DayTimetable> DayTimetables { get; set; }
    public DbSet<WorkVisit> WorkVisits { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<TaskType> TaskTypes { get; set; }
    public DbSet<FileModel> FileModels { get; set; }

    public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        builder.Entity<User>()
            .HasIndex(u => u.FullName)
            .IsUnique(); 
    }
} 