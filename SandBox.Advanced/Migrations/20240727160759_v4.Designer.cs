﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SandBox.Advanced.Database;

#nullable disable

namespace SandBox.Advanced.Migrations
{
    [DbContext(typeof(SandBoxContext))]
    [Migration("20240727160759_v4")]
    partial class v4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("SandBox.Models.Blackbox.BlackWord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("BlackWords");
                });

            modelBuilder.Entity("SandBox.Models.Blackbox.Captcha", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte>("AttemptsRemain")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTimeExpired")
                        .HasColumnType("TEXT");

                    b.Property<long?>("IdTelegram")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("IdTelegram");

                    b.ToTable("Captchas");
                });

            modelBuilder.Entity("SandBox.Models.Common.Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("TEXT");

                    b.Property<long?>("IdTelegram")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MessageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("IdTelegram");

                    b.ToTable("Events");

                    b.HasDiscriminator().HasValue("Event");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("SandBox.Models.Telegram.Account", b =>
                {
                    b.Property<long>("IdTelegram")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTimeJoined")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAprroved")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsManagerThisBot")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsNeedToVerifyByCaptcha")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsSpamer")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastActivity")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("IdTelegram");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SandBox.Models.Telegram.ChatProps", b =>
                {
                    b.Property<long>("IdChat")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("CountNormalMessageToBeAprroved")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<float>("PercentageToDetectSpamFromMl")
                        .HasColumnType("REAL");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("IdChat");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("SandBox.Models.Telegram.MemberInChat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("CountMessage")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CountSpam")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTimeJoined")
                        .HasColumnType("TEXT");

                    b.Property<long?>("IdChat")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("IdTelegram")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRestricted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastActivity")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("IdChat");

                    b.HasIndex("IdTelegram");

                    b.ToTable("MembersInChats");
                });

            modelBuilder.Entity("SandBox.Models.Telegram.Question", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Quest")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("SandBox.Models.Events.EventContent", b =>
                {
                    b.HasBaseType("SandBox.Models.Common.Event");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSpam")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("EventContent");
                });

            modelBuilder.Entity("SandBox.Models.Events.EventJoined", b =>
                {
                    b.HasBaseType("SandBox.Models.Common.Event");

                    b.HasDiscriminator().HasValue("EventJoined");
                });

            modelBuilder.Entity("SandBox.Models.Blackbox.Captcha", b =>
                {
                    b.HasOne("SandBox.Models.Telegram.Account", "Account")
                        .WithMany()
                        .HasForeignKey("IdTelegram");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SandBox.Models.Common.Event", b =>
                {
                    b.HasOne("SandBox.Models.Telegram.ChatProps", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId");

                    b.HasOne("SandBox.Models.Telegram.Account", "Account")
                        .WithMany()
                        .HasForeignKey("IdTelegram");

                    b.Navigation("Account");

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("SandBox.Models.Telegram.MemberInChat", b =>
                {
                    b.HasOne("SandBox.Models.Telegram.ChatProps", "Chat")
                        .WithMany()
                        .HasForeignKey("IdChat");

                    b.HasOne("SandBox.Models.Telegram.Account", "Account")
                        .WithMany()
                        .HasForeignKey("IdTelegram");

                    b.Navigation("Account");

                    b.Navigation("Chat");
                });
#pragma warning restore 612, 618
        }
    }
}
