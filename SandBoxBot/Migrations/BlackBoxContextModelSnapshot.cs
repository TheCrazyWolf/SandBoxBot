﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SandBoxBot.Database;

#nullable disable

namespace SandBoxBot.Migrations
{
    [DbContext(typeof(SandBoxContext))]
    partial class BlackBoxContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.5");

            modelBuilder.Entity("SandBoxBot.Models.Account", b =>
                {
                    b.Property<long>("IdAccountTelegram")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTimeJoined")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastActivity")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("IdAccountTelegram");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SandBoxBot.Models.BlackWord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("BlackWords");
                });

            modelBuilder.Entity("SandBoxBot.Models.Incident", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<long?>("IdAccountTelegram")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSpam")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IdAccountTelegram");

                    b.ToTable("Sentences");
                });

            modelBuilder.Entity("SandBoxBot.Models.Incident", b =>
                {
                    b.HasOne("SandBoxBot.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("IdAccountTelegram");

                    b.Navigation("Account");
                });
#pragma warning restore 612, 618
        }
    }
}
