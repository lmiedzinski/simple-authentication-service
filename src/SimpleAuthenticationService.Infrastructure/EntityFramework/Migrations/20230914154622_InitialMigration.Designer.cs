﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SimpleAuthenticationService.Infrastructure.EntityFramework;

#nullable disable

namespace SimpleAuthenticationService.Infrastructure.EntityFramework.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230914154622_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SimpleAuthenticationService.Domain.UserAccounts.UserAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("login");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("password_hash");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.ToTable("user_accounts", (string)null);
                });

            modelBuilder.Entity("SimpleAuthenticationService.Infrastructure.OutboxPattern.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ProcessedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("outbox_messages", (string)null);
                });

            modelBuilder.Entity("SimpleAuthenticationService.Domain.UserAccounts.UserAccount", b =>
                {
                    b.OwnsOne("SimpleAuthenticationService.Domain.UserAccounts.RefreshToken", "RefreshToken", b1 =>
                        {
                            b1.Property<Guid>("UserAccountId")
                                .HasColumnType("uuid");

                            b1.Property<DateTime>("ExpirationDateUtc")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("refresh_token_expiration_date_utc");

                            b1.Property<bool>("IsActive")
                                .HasColumnType("boolean")
                                .HasColumnName("refresh_token_is_active");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(1024)
                                .HasColumnType("character varying(1024)")
                                .HasColumnName("refresh_token_value");

                            b1.HasKey("UserAccountId");

                            b1.ToTable("user_accounts");

                            b1.WithOwner()
                                .HasForeignKey("UserAccountId");
                        });

                    b.OwnsMany("SimpleAuthenticationService.Domain.UserAccounts.Claim", "_claims", b1 =>
                        {
                            b1.Property<Guid>("id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uuid");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)")
                                .HasColumnName("type");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(512)
                                .HasColumnType("character varying(512)")
                                .HasColumnName("value");

                            b1.Property<Guid>("user_account_id")
                                .HasColumnType("uuid");

                            b1.HasKey("id");

                            b1.HasIndex("user_account_id");

                            b1.ToTable("user_account_claims", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("user_account_id");
                        });

                    b.Navigation("RefreshToken");

                    b.Navigation("_claims");
                });
#pragma warning restore 612, 618
        }
    }
}
