﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomReservation.Data;

#nullable disable

namespace RoomReservation.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.9");

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "d83cc7ab-bb04-44aa-bf91-1e17c1b45fcb",
                            ConcurrencyStamp = "b2815680-271e-4f7c-8218-510cc8da9e44",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "0941aa3e-2d65-4f57-ab42-3aab9678f00e",
                            ConcurrencyStamp = "a622cdc3-1c77-4303-b736-92514b3d8f37",
                            Name = "Member",
                            NormalizedName = "MEMBER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "2f697355-9779-47ee-b278-c97cbee59020",
                            RoleId = "d83cc7ab-bb04-44aa-bf91-1e17c1b45fcb"
                        },
                        new
                        {
                            UserId = "5532c45c-5f99-4e10-a214-8e59abc7c28d",
                            RoleId = "0941aa3e-2d65-4f57-ab42-3aab9678f00e"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("RoomReservation.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ICalGuid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("LockedOut")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "2f697355-9779-47ee-b278-c97cbee59020",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "7b749551-c048-4a79-8f85-ff4293077a0e",
                            Email = "Super@Admin.de",
                            EmailConfirmed = true,
                            FirstName = "Super",
                            ICalGuid = "0085c919-2cd3-44aa-ac20-e637e20d448a",
                            LastName = "Admin",
                            LockedOut = false,
                            LockoutEnabled = false,
                            NormalizedEmail = "SUPER@ADMIN.DE",
                            NormalizedUserName = "ADMIN, SUPER",
                            PasswordHash = "AQAAAAEAACcQAAAAEKokcUBZfplMtk1W+qxSpEX+h/EJ5RjJOHTQ551uypFyHNE7x1/StibzWDOiGZMOeg==",
                            PhoneNumberConfirmed = false,
                            Role = "d83cc7ab-bb04-44aa-bf91-1e17c1b45fcb",
                            SecurityStamp = "9f3f2872-e0e6-4387-a844-05ef7ad78aef",
                            TwoFactorEnabled = false,
                            UserName = "Admin, Super"
                        },
                        new
                        {
                            Id = "5532c45c-5f99-4e10-a214-8e59abc7c28d",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "fe28466d-509c-4973-a4ab-99a49081af8b",
                            Email = "Default@Member.de",
                            EmailConfirmed = false,
                            FirstName = "Default",
                            ICalGuid = "db46240b-9f97-4ac7-add2-79c53f3c9321",
                            LastName = "Member",
                            LockedOut = false,
                            LockoutEnabled = true,
                            NormalizedEmail = "DEFAULT@MEMBER.DE",
                            NormalizedUserName = "MEMBER, Default",
                            PasswordHash = "AQAAAAEAACcQAAAAEGms6yuc3ZqqOrubuz+kpGzE2etXzFAROSdIF9eFZw2Lnr8nCQ+J34bvJbdBOXcTPQ==",
                            PhoneNumberConfirmed = false,
                            Role = "0941aa3e-2d65-4f57-ab42-3aab9678f00e",
                            SecurityStamp = "ceaac5e7-b576-47e9-8cd1-f417be6d0e7b",
                            TwoFactorEnabled = false,
                            UserName = "Member, Default"
                        },
                        new
                        {
                            Id = "0",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "4c7f20f0-94aa-4313-a58b-72a38adefb53",
                            Email = "Dummy@Dum.my",
                            EmailConfirmed = false,
                            FirstName = "Dummy",
                            ICalGuid = "a1e584a3-06fe-432e-b2d1-7316e2fd3a69",
                            LastName = "User",
                            LockedOut = false,
                            LockoutEnabled = false,
                            NormalizedEmail = "DUMMY@DUM.MY",
                            NormalizedUserName = " ",
                            PasswordHash = "AQAAAAEAACcQAAAAEIgk0KnNLax+6Mr6/29dC7paDPFIhaZrBc+MEpWGUcuD/Q9riN1VxlPl6OOllW+KfA==",
                            PhoneNumberConfirmed = false,
                            Role = "0",
                            SecurityStamp = "668b5ae7-8f9a-46f4-b7ef-2492b37520e9",
                            TwoFactorEnabled = false,
                            UserName = " "
                        });
                });

            modelBuilder.Entity("RoomReservation.Models.Reservation", b =>
                {
                    b.Property<DateTime>("Start")
                        .HasColumnType("Date");

                    b.Property<string>("ResourceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Start", "ResourceId");

                    b.HasIndex("ResourceId");

                    b.HasIndex("UserId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("RoomReservation.Models.Room", b =>
                {
                    b.Property<int>("RoomId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Limit")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RoomId");

                    b.ToTable("Rooms");

                    b.HasData(
                        new
                        {
                            RoomId = 1,
                            Limit = 1,
                            Name = "101"
                        },
                        new
                        {
                            RoomId = 2,
                            Limit = 2,
                            Name = "102"
                        });
                });

            modelBuilder.Entity("RoomReservation.Models.Seat", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExtraInfo")
                        .HasColumnType("TEXT");

                    b.Property<int>("RoomId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.HasIndex("RoomId");

                    b.ToTable("Seats");

                    b.HasData(
                        new
                        {
                            id = "101-1-1",
                            RoomId = 1,
                            title = "AP 1"
                        },
                        new
                        {
                            id = "101-1-2",
                            RoomId = 1,
                            title = "AP 2"
                        },
                        new
                        {
                            id = "102-2-1",
                            RoomId = 2,
                            title = "AP 1"
                        },
                        new
                        {
                            id = "102-2-2",
                            RoomId = 2,
                            title = "AP 2"
                        },
                        new
                        {
                            id = "102-2-3",
                            RoomId = 2,
                            title = "AP 3"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("RoomReservation.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("RoomReservation.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RoomReservation.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("RoomReservation.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RoomReservation.Models.Reservation", b =>
                {
                    b.HasOne("RoomReservation.Models.Seat", "Seat")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RoomReservation.Models.AppUser", "AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("Seat");
                });

            modelBuilder.Entity("RoomReservation.Models.Seat", b =>
                {
                    b.HasOne("RoomReservation.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });
#pragma warning restore 612, 618
        }
    }
}
