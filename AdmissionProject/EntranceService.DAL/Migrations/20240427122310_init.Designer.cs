﻿// <auto-generated />
using System;
using EntranceService.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EntranceService.DAL.Migrations
{
    [DbContext(typeof(EntranceDbContext))]
    [Migration("20240427122310_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EntranceService.DAL.Entity.Application", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ApplicationStatus")
                        .HasColumnType("integer");

                    b.Property<string>("Citizenship")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastChangeDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ManagerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("EntranceService.DAL.Entity.ApplicationPrograms", b =>
                {
                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProgramId")
                        .HasColumnType("uuid");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.HasKey("ApplicationId", "ProgramId");

                    b.ToTable("ApplicationsPrograms");
                });

            modelBuilder.Entity("EntranceService.DAL.Entity.EducationDocumentData", b =>
                {
                    b.Property<Guid>("ownerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("EducationDocumentId")
                        .HasColumnType("uuid");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ownerId", "EducationDocumentId");

                    b.ToTable("EducationDocumentsData");
                });

            modelBuilder.Entity("EntranceService.DAL.Entity.PassportData", b =>
                {
                    b.Property<Guid>("ownerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("birthPlace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("issueDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("issuePlace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("number")
                        .HasColumnType("integer");

                    b.Property<int>("series")
                        .HasColumnType("integer");

                    b.HasKey("ownerId");

                    b.ToTable("PassportsData");
                });
#pragma warning restore 612, 618
        }
    }
}