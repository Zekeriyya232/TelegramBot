﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TelegramBot.Entities;

#nullable disable

namespace TelegramBot.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240912142758_TaskMemberAndTaskUpdate")]
    partial class TaskMemberAndTaskUpdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TelegramBot.Entities.ChatMembersDB", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("firstName")
                        .HasColumnType("text");

                    b.Property<string>("lastName")
                        .HasColumnType("text");

                    b.Property<long>("telegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("userName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ChatMember");
                });

            modelBuilder.Entity("TelegramBot.Entities.MembersDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("startingJob")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long?>("telegramId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("TelegramBot.Entities.TaskDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("creationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("endingTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("startingTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("taskDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("taskGiver")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("taskName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("TelegramBot.Entities.TaskMember", b =>
                {
                    b.Property<int>("TaskId")
                        .HasColumnType("integer");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("progress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("progressPercent")
                        .HasColumnType("integer");

                    b.HasKey("TaskId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("taskMembers");
                });

            modelBuilder.Entity("TelegramBot.Entities.UserDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("phone")
                        .HasColumnType("text");

                    b.Property<string>("role")
                        .HasColumnType("text");

                    b.Property<string>("userEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("userName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("userSurname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TelegramBot.Entities.TaskMember", b =>
                {
                    b.HasOne("TelegramBot.Entities.MembersDB", "Member")
                        .WithMany("TaskMembers")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TelegramBot.Entities.TaskDB", "Task")
                        .WithMany("TaskMembers")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("TelegramBot.Entities.MembersDB", b =>
                {
                    b.Navigation("TaskMembers");
                });

            modelBuilder.Entity("TelegramBot.Entities.TaskDB", b =>
                {
                    b.Navigation("TaskMembers");
                });
#pragma warning restore 612, 618
        }
    }
}
