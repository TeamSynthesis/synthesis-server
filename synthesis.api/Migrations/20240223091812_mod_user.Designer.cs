﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using synthesis.api.Data.Repository;

#nullable disable

namespace synthesis.api.Migrations
{
    [DbContext(typeof(RepositoryContext))]
    [Migration("20240223091812_mod_user")]
    partial class mod_user
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "hstore");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("synthesis.api.Data.Models.FeatureModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("FeatureId");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Features");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.MemberModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("MemberId");

                    b.Property<List<string>>("Roles")
                        .HasColumnType("text[]");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.HasIndex("UserId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.ProjectModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("ProjectId");

                    b.Property<string>("IconUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.TaskToDoModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("TaskId");

                    b.Property<string>("Activity")
                        .HasColumnType("text");

                    b.Property<DateTime?>("AssignedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("FeatureId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("MemberId")
                        .HasColumnType("uuid");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.HasIndex("MemberId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.TeamModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("TeamId");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.UserModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("UserId");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("GitHubId")
                        .HasColumnType("text");

                    b.Property<int>("OnBoardingProgress")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.HasIndex("GitHubId");

                    b.HasIndex("UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.UserSessionModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IpAddress")
                        .HasColumnType("text");

                    b.Property<string>("UserAgent")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.FeatureModel", b =>
                {
                    b.HasOne("synthesis.api.Data.Models.ProjectModel", "Project")
                        .WithMany("Features")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.MemberModel", b =>
                {
                    b.HasOne("synthesis.api.Data.Models.TeamModel", "Team")
                        .WithMany("Members")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("synthesis.api.Data.Models.UserModel", "User")
                        .WithMany("MemberProfiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.ProjectModel", b =>
                {
                    b.HasOne("synthesis.api.Data.Models.TeamModel", "Team")
                        .WithMany("Projects")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("synthesis.api.Data.Models.ProjectMetadata", "ProjectMetadata", b1 =>
                        {
                            b1.Property<Guid>("ProjectModelId")
                                .HasColumnType("uuid");

                            b1.HasKey("ProjectModelId");

                            b1.ToTable("Projects");

                            b1.ToJson("ProjectMetadata");

                            b1.WithOwner()
                                .HasForeignKey("ProjectModelId");

                            b1.OwnsOne("synthesis.api.Data.Models.Branding", "Branding", b2 =>
                                {
                                    b2.Property<Guid>("ProjectMetadataProjectModelId")
                                        .HasColumnType("uuid");

                                    b2.Property<string>("Slogan")
                                        .HasColumnType("text");

                                    b2.HasKey("ProjectMetadataProjectModelId");

                                    b2.ToTable("Projects");

                                    b2.WithOwner()
                                        .HasForeignKey("ProjectMetadataProjectModelId");

                                    b2.OwnsOne("synthesis.api.Data.Models.Image", "Icon", b3 =>
                                        {
                                            b3.Property<Guid>("BrandingProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<string>("Description")
                                                .HasColumnType("text");

                                            b3.Property<string>("ImgUrl")
                                                .HasColumnType("text");

                                            b3.HasKey("BrandingProjectMetadataProjectModelId");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("BrandingProjectMetadataProjectModelId");
                                        });

                                    b2.OwnsMany("synthesis.api.Data.Models.Image", "MoodBoards", b3 =>
                                        {
                                            b3.Property<Guid>("BrandingProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("integer");

                                            b3.Property<string>("Description")
                                                .HasColumnType("text");

                                            b3.Property<string>("ImgUrl")
                                                .HasColumnType("text");

                                            b3.HasKey("BrandingProjectMetadataProjectModelId", "Id");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("BrandingProjectMetadataProjectModelId");
                                        });

                                    b2.OwnsOne("synthesis.api.Data.Models.ColorPalette", "Palette", b3 =>
                                        {
                                            b3.Property<Guid>("BrandingProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<Dictionary<string, string>>("Neutral")
                                                .HasColumnType("hstore");

                                            b3.Property<string>("PreviewUrl")
                                                .HasColumnType("text");

                                            b3.Property<Dictionary<string, string>>("Primary")
                                                .HasColumnType("hstore");

                                            b3.Property<string>("Reason")
                                                .HasColumnType("text");

                                            b3.Property<Dictionary<string, string>>("Secondary")
                                                .HasColumnType("hstore");

                                            b3.HasKey("BrandingProjectMetadataProjectModelId");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("BrandingProjectMetadataProjectModelId");
                                        });

                                    b2.OwnsOne("synthesis.api.Data.Models.Typography", "Typography", b3 =>
                                        {
                                            b3.Property<Guid>("BrandingProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<string>("Font")
                                                .HasColumnType("text");

                                            b3.Property<string>("Reason")
                                                .HasColumnType("text");

                                            b3.HasKey("BrandingProjectMetadataProjectModelId");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("BrandingProjectMetadataProjectModelId");
                                        });

                                    b2.OwnsMany("synthesis.api.Data.Models.Wireframe", "Wireframes", b3 =>
                                        {
                                            b3.Property<Guid>("BrandingProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("integer");

                                            b3.Property<string>("Screen")
                                                .HasColumnType("text");

                                            b3.HasKey("BrandingProjectMetadataProjectModelId", "Id");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("BrandingProjectMetadataProjectModelId");

                                            b3.OwnsOne("synthesis.api.Data.Models.Image", "Image", b4 =>
                                                {
                                                    b4.Property<Guid>("WireframeBrandingProjectMetadataProjectModelId")
                                                        .HasColumnType("uuid");

                                                    b4.Property<int>("WireframeId")
                                                        .HasColumnType("integer");

                                                    b4.Property<string>("Description")
                                                        .HasColumnType("text");

                                                    b4.Property<string>("ImgUrl")
                                                        .HasColumnType("text");

                                                    b4.HasKey("WireframeBrandingProjectMetadataProjectModelId", "WireframeId");

                                                    b4.ToTable("Projects");

                                                    b4.WithOwner()
                                                        .HasForeignKey("WireframeBrandingProjectMetadataProjectModelId", "WireframeId");
                                                });

                                            b3.Navigation("Image");
                                        });

                                    b2.Navigation("Icon");

                                    b2.Navigation("MoodBoards");

                                    b2.Navigation("Palette");

                                    b2.Navigation("Typography");

                                    b2.Navigation("Wireframes");
                                });

                            b1.OwnsOne("synthesis.api.Data.Models.CompetitiveAnalysis", "CompetitiveAnalysis", b2 =>
                                {
                                    b2.Property<Guid>("ProjectMetadataProjectModelId")
                                        .HasColumnType("uuid");

                                    b2.HasKey("ProjectMetadataProjectModelId");

                                    b2.ToTable("Projects");

                                    b2.WithOwner()
                                        .HasForeignKey("ProjectMetadataProjectModelId");

                                    b2.OwnsMany("synthesis.api.Data.Models.Competitor", "Competitors", b3 =>
                                        {
                                            b3.Property<Guid>("CompetitiveAnalysisProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("integer");

                                            b3.Property<string>("Description")
                                                .HasColumnType("text");

                                            b3.Property<List<string>>("Features")
                                                .HasColumnType("text[]");

                                            b3.Property<string>("LogoUrl")
                                                .HasColumnType("text");

                                            b3.Property<string>("Name")
                                                .HasColumnType("text");

                                            b3.Property<string>("PricingModel")
                                                .HasColumnType("text");

                                            b3.Property<double>("ReviewSentiment")
                                                .HasColumnType("double precision");

                                            b3.Property<string>("Size")
                                                .HasColumnType("text");

                                            b3.Property<string>("Url")
                                                .HasColumnType("text");

                                            b3.HasKey("CompetitiveAnalysisProjectMetadataProjectModelId", "Id");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("CompetitiveAnalysisProjectMetadataProjectModelId");
                                        });

                                    b2.OwnsOne("synthesis.api.Data.Models.Swot", "Swot", b3 =>
                                        {
                                            b3.Property<Guid>("CompetitiveAnalysisProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<List<string>>("Opportunities")
                                                .HasColumnType("text[]");

                                            b3.Property<List<string>>("Strengths")
                                                .HasColumnType("text[]");

                                            b3.Property<List<string>>("Threats")
                                                .HasColumnType("text[]");

                                            b3.Property<List<string>>("Weaknesses")
                                                .HasColumnType("text[]");

                                            b3.HasKey("CompetitiveAnalysisProjectMetadataProjectModelId");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("CompetitiveAnalysisProjectMetadataProjectModelId");
                                        });

                                    b2.OwnsOne("synthesis.api.Data.Models.TargetAudience", "TargetAudience", b3 =>
                                        {
                                            b3.Property<Guid>("CompetitiveAnalysisProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.HasKey("CompetitiveAnalysisProjectMetadataProjectModelId");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("CompetitiveAnalysisProjectMetadataProjectModelId");

                                            b3.OwnsOne("synthesis.api.Data.Models.Demographics", "Demographics", b4 =>
                                                {
                                                    b4.Property<Guid>("TargetAudienceCompetitiveAnalysisProjectMetadataProjectModelId")
                                                        .HasColumnType("uuid");

                                                    b4.Property<string>("Age")
                                                        .HasColumnType("text");

                                                    b4.HasKey("TargetAudienceCompetitiveAnalysisProjectMetadataProjectModelId");

                                                    b4.ToTable("Projects");

                                                    b4.WithOwner()
                                                        .HasForeignKey("TargetAudienceCompetitiveAnalysisProjectMetadataProjectModelId");
                                                });

                                            b3.Navigation("Demographics");
                                        });

                                    b2.Navigation("Competitors");

                                    b2.Navigation("Swot");

                                    b2.Navigation("TargetAudience");
                                });

                            b1.OwnsOne("synthesis.api.Data.Models.Overview", "Overview", b2 =>
                                {
                                    b2.Property<Guid>("ProjectMetadataProjectModelId")
                                        .HasColumnType("uuid");

                                    b2.Property<string>("Description")
                                        .HasColumnType("text");

                                    b2.HasKey("ProjectMetadataProjectModelId");

                                    b2.ToTable("Projects");

                                    b2.WithOwner()
                                        .HasForeignKey("ProjectMetadataProjectModelId");

                                    b2.OwnsMany("synthesis.api.Data.Models.SuggestedDomain", "SuggestedDomains", b3 =>
                                        {
                                            b3.Property<Guid>("OverviewProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("integer");

                                            b3.Property<string>("Name")
                                                .HasColumnType("text");

                                            b3.Property<string>("Reason")
                                                .HasColumnType("text");

                                            b3.HasKey("OverviewProjectMetadataProjectModelId", "Id");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("OverviewProjectMetadataProjectModelId");
                                        });

                                    b2.OwnsMany("synthesis.api.Data.Models.SuggestedName", "SuggestedNames", b3 =>
                                        {
                                            b3.Property<Guid>("OverviewProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("integer");

                                            b3.Property<string>("Name")
                                                .HasColumnType("text");

                                            b3.Property<string>("Reason")
                                                .HasColumnType("text");

                                            b3.HasKey("OverviewProjectMetadataProjectModelId", "Id");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("OverviewProjectMetadataProjectModelId");
                                        });

                                    b2.Navigation("SuggestedDomains");

                                    b2.Navigation("SuggestedNames");
                                });

                            b1.OwnsOne("synthesis.api.Data.Models.Technology", "Technology", b2 =>
                                {
                                    b2.Property<Guid>("ProjectMetadataProjectModelId")
                                        .HasColumnType("uuid");

                                    b2.HasKey("ProjectMetadataProjectModelId");

                                    b2.ToTable("Projects");

                                    b2.WithOwner()
                                        .HasForeignKey("ProjectMetadataProjectModelId");

                                    b2.OwnsMany("synthesis.api.Data.Models.TechStack", "TechStacks", b3 =>
                                        {
                                            b3.Property<Guid>("TechnologyProjectMetadataProjectModelId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("integer");

                                            b3.Property<string>("Description")
                                                .HasColumnType("text");

                                            b3.Property<string>("LogoUrl")
                                                .HasColumnType("text");

                                            b3.Property<string>("Name")
                                                .HasColumnType("text");

                                            b3.Property<string>("Reason")
                                                .HasColumnType("text");

                                            b3.HasKey("TechnologyProjectMetadataProjectModelId", "Id");

                                            b3.ToTable("Projects");

                                            b3.WithOwner()
                                                .HasForeignKey("TechnologyProjectMetadataProjectModelId");
                                        });

                                    b2.Navigation("TechStacks");
                                });

                            b1.Navigation("Branding");

                            b1.Navigation("CompetitiveAnalysis");

                            b1.Navigation("Overview");

                            b1.Navigation("Technology");
                        });

                    b.Navigation("ProjectMetadata");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.TaskToDoModel", b =>
                {
                    b.HasOne("synthesis.api.Data.Models.FeatureModel", "Feature")
                        .WithMany("Tasks")
                        .HasForeignKey("FeatureId");

                    b.HasOne("synthesis.api.Data.Models.MemberModel", "Member")
                        .WithMany("Tasks")
                        .HasForeignKey("MemberId");

                    b.HasOne("synthesis.api.Data.Models.ProjectModel", "Project")
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feature");

                    b.Navigation("Member");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.UserModel", b =>
                {
                    b.OwnsMany("System.Collections.Generic.Dictionary<string, int>", "Skills", b1 =>
                        {
                            b1.Property<Guid>("UserModelId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            b1.HasKey("UserModelId", "Id");

                            b1.ToTable("Users");

                            b1.ToJson("Skills");

                            b1.WithOwner()
                                .HasForeignKey("UserModelId");
                        });

                    b.Navigation("Skills");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.UserSessionModel", b =>
                {
                    b.HasOne("synthesis.api.Data.Models.UserModel", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.FeatureModel", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.MemberModel", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.ProjectModel", b =>
                {
                    b.Navigation("Features");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.TeamModel", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("Projects");
                });

            modelBuilder.Entity("synthesis.api.Data.Models.UserModel", b =>
                {
                    b.Navigation("MemberProfiles");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
