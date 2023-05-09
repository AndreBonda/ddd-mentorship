﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ShareBook.Infrastructure;

#nullable disable

namespace ShareBook.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230509065635_Adding foreign key to LoanRequest")]
    partial class AddingforeignkeytoLoanRequest
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ShareBook.Domain.Books.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Author")
                        .HasColumnType("text")
                        .HasColumnName("author");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt");

                    b.Property<string>("Labels")
                        .HasColumnType("text")
                        .HasColumnName("labels");

                    b.Property<string>("Owner")
                        .HasColumnType("text")
                        .HasColumnName("owner");

                    b.Property<int>("Pages")
                        .HasColumnType("integer")
                        .HasColumnName("pages");

                    b.Property<bool>("SharedByOwner")
                        .HasColumnType("boolean")
                        .HasColumnName("sharedByOwner");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pK_books");

                    b.ToTable("books", (string)null);
                });

            modelBuilder.Entity("ShareBook.Domain.Books.Book", b =>
                {
                    b.OwnsOne("ShareBook.Domain.Books.LoanRequest", "LoanRequest", b1 =>
                        {
                            b1.Property<Guid>("BookId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("loanRequest_CreatedAt");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uuid")
                                .HasColumnName("loanRequest_Id");

                            b1.Property<string>("RequestingUser")
                                .HasColumnType("text")
                                .HasColumnName("loanRequest_RequestingUser");

                            b1.Property<int>("Status")
                                .HasColumnType("integer")
                                .HasColumnName("loanRequest_Status");

                            b1.HasKey("BookId");

                            b1.ToTable("books");

                            b1.WithOwner()
                                .HasForeignKey("BookId")
                                .HasConstraintName("fK_books_books_id");
                        });

                    b.Navigation("LoanRequest");
                });
#pragma warning restore 612, 618
        }
    }
}