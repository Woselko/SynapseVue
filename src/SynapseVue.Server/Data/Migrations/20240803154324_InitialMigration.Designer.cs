﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SynapseVue.Server.Data;

#nullable disable

namespace SynapseVue.Server.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240803154324_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Xml")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens", (string)null);
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Categories.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Color = "#FFCD56",
                            Name = "DHT22"
                        },
                        new
                        {
                            Id = 2,
                            Color = "#441C4D",
                            Name = "DHT11"
                        },
                        new
                        {
                            Id = 3,
                            Color = "#FF6384",
                            Name = "PIR"
                        },
                        new
                        {
                            Id = 4,
                            Color = "#07E80B",
                            Name = "LED"
                        },
                        new
                        {
                            Id = 5,
                            Color = "#4BC0C0",
                            Name = "LCD-DISPLAY"
                        },
                        new
                        {
                            Id = 6,
                            Color = "#FF9124",
                            Name = "BUZZ"
                        },
                        new
                        {
                            Id = 7,
                            Color = "#2B88D8",
                            Name = "CAMERA"
                        },
                        new
                        {
                            Id = 8,
                            Color = "#DB650B",
                            Name = "RFID"
                        },
                        new
                        {
                            Id = 9,
                            Color = "#07E8B0",
                            Name = "BH1750"
                        },
                        new
                        {
                            Id = 10,
                            Color = "#BD040D",
                            Name = "MQ-9"
                        },
                        new
                        {
                            Id = 11,
                            Color = "#DE0064",
                            Name = "MQ-3"
                        });
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Identity.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Identity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("BirthDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<long?>("ConfirmationEmailRequestedOn")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FullName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Gender")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("LockoutEnd")
                        .HasColumnType("INTEGER");

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

                    b.Property<string>("ProfileImageName")
                        .HasColumnType("TEXT");

                    b.Property<long?>("ResetPasswordEmailRequestedOn")
                        .HasColumnType("INTEGER");

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

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AccessFailedCount = 0,
                            BirthDate = 1306790461440000060L,
                            ConcurrencyStamp = "315e1a26-5b3a-4544-8e91-2760cd28e231",
                            Email = "test@bitplatform.dev",
                            EmailConfirmed = true,
                            FullName = "SynapseVue test account",
                            Gender = 2,
                            LockoutEnabled = true,
                            NormalizedEmail = "TEST@BITPLATFORM.DEV",
                            NormalizedUserName = "TEST@BITPLATFORM.DEV",
                            PasswordHash = "AQAAAAIAAYagAAAAEP0v3wxkdWtMkHA3Pp5/JfS+42/Qto9G05p2mta6dncSK37hPxEHa3PGE4aqN30Aag==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "959ff4a9-4b07-4cc1-8141-c5fc033daf83",
                            TwoFactorEnabled = false,
                            UserName = "test@bitplatform.dev"
                        });
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Media.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CreatedAt")
                        .HasMaxLength(512)
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Photos");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = 1307708817408000120L,
                            Data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                            Description = "MyPhoto1",
                            Name = "photo1"
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = 1307708817408000120L,
                            Data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                            Description = "MyPhoto2",
                            Name = "photo2"
                        });
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Media.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CreatedAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<string>("DetectedObjects")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<long>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPersonDetected")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsProcessed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Videos");

                    b.HasData(
                        new
                        {
                            Id = 2,
                            CreatedAt = 1307708817408000120L,
                            Description = "MyVideo 2",
                            DetectedObjects = "nothing",
                            FilePath = "C:\\C_Sources\\00SynapseVue\\src\\SynapseVue.Server\\wwwroot\\videos\\test1.avi",
                            FileSize = 34124L,
                            IsPersonDetected = false,
                            IsProcessed = false,
                            Name = "test1.avi"
                        },
                        new
                        {
                            Id = 3,
                            CreatedAt = 1307708817408000120L,
                            Description = "MyVideo 3 mp3",
                            DetectedObjects = "nothing",
                            FilePath = "C:\\C_Sources\\00SynapseVue\\src\\SynapseVue.Server\\wwwroot\\videos\\film.mp4",
                            FileSize = 34124L,
                            IsPersonDetected = false,
                            IsProcessed = true,
                            Name = "film.mp4"
                        });
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Products.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CreatedOn")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastReadValue")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<long?>("LastSuccessActivity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int>("PIN")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 3,
                            CreatedOn = 1307708817408000120L,
                            Description = "Passive Infra Red motion detector",
                            LastReadValue = "",
                            LastSuccessActivity = 1307708817408000120L,
                            Name = "PIRSensor-main",
                            PIN = 18
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 4,
                            CreatedOn = 1307708817408000120L,
                            Description = "Led diode",
                            LastReadValue = "",
                            LastSuccessActivity = 1307708817408000120L,
                            Name = "LED-main",
                            PIN = 17
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 6,
                            CreatedOn = 1307673427968000120L,
                            Description = "Buzzer for sound generating",
                            LastReadValue = "",
                            LastSuccessActivity = 1307708817408000120L,
                            Name = "Buzzer-main",
                            PIN = 23
                        },
                        new
                        {
                            Id = 4,
                            CategoryId = 1,
                            CreatedOn = 1307708817408000120L,
                            Description = "DHT Temperature and humidity reader",
                            LastReadValue = "",
                            LastSuccessActivity = 1307708817408000120L,
                            Name = "DHT22Sensor-main",
                            PIN = 27
                        },
                        new
                        {
                            Id = 5,
                            CategoryId = 5,
                            CreatedOn = 1307708817408000120L,
                            Description = "Display, no pin needed",
                            LastReadValue = "",
                            LastSuccessActivity = 1307708817408000120L,
                            Name = "Display-main",
                            PIN = 0
                        },
                        new
                        {
                            Id = 6,
                            CategoryId = 7,
                            CreatedOn = 1307675197440000120L,
                            Description = "Camera connected to PCI slot 0, no pin needed",
                            LastReadValue = "",
                            LastSuccessActivity = 1307708817408000120L,
                            Name = "Camera0-AI-main",
                            PIN = 0
                        },
                        new
                        {
                            Id = 7,
                            CategoryId = 7,
                            CreatedOn = 1307691122688000120L,
                            Description = "Camera connected to PCI slot 1, no pin needed",
                            LastReadValue = "",
                            LastSuccessActivity = 1307708817408000120L,
                            Name = "Camera1-SERVER-main",
                            PIN = 0
                        });
                });

            modelBuilder.Entity("SynapseVue.Server.Models.System.SystemState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Mode")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SystemStates");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Mode = "Home"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("SynapseVue.Server.Models.Identity.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("SynapseVue.Server.Models.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("SynapseVue.Server.Models.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("SynapseVue.Server.Models.Identity.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SynapseVue.Server.Models.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("SynapseVue.Server.Models.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Products.Product", b =>
                {
                    b.HasOne("SynapseVue.Server.Models.Categories.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("SynapseVue.Server.Models.Categories.Category", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}