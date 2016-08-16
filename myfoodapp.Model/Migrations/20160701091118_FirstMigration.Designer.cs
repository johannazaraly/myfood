using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using myfoodapp.Model;

namespace myfoodapp.Migrations
{
    [DbContext(typeof(LocalDataContext))]
    [Migration("20160701091118_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("myfoodapp.Model.Measure", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("captureDate");

                    b.Property<int?>("sensorId");

                    b.Property<decimal>("value");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("myfoodapp.Model.Message", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("content")
                        .IsRequired();

                    b.Property<DateTime>("date");

                    b.Property<string>("info");

                    b.Property<int?>("messageTypeId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("myfoodapp.Model.MessageType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("description");

                    b.Property<string>("name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("myfoodapp.Model.SensorType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("description");

                    b.Property<DateTime?>("lastCalibration");

                    b.Property<string>("name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("myfoodapp.Model.Measure", b =>
                {
                    b.HasOne("myfoodapp.Model.SensorType")
                        .WithMany()
                        .HasForeignKey("sensorId");
                });

            modelBuilder.Entity("myfoodapp.Model.Message", b =>
                {
                    b.HasOne("myfoodapp.Model.MessageType")
                        .WithMany()
                        .HasForeignKey("messageTypeId");
                });
        }
    }
}
