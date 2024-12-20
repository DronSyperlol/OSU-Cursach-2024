﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20241218003254_Session")]
    partial class Session
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Database.Entities.Session", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<ulong>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DiedAt")
                        .HasColumnType("datetime");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<long>("ToUserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ToUserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Database.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("LastName")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)");

                    b.Property<string>("Username")
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Database.Entities.Session", b =>
                {
                    b.HasOne("Database.Entities.User", "ToUser")
                        .WithMany()
                        .HasForeignKey("ToUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ToUser");
                });
#pragma warning restore 612, 618
        }
    }
}