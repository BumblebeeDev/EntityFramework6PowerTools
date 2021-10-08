﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlServerCe;
using System.Linq;
using Xunit;

namespace SqlProviderSmokeTest
{
    public class Test1
    {
        [Fact]
        public void SmokeTest1()
        {
            var connectionString = "Data Source=Tester.sdf;";

            using (var ctx = new SchoolContext(new SqlCeConnection(connectionString)))
            {
                ctx.Database.CreateIfNotExists();

                ctx.Database.ExecuteSqlCommand("SELECT 1");

                var students = ctx.Students.ToList();

                Assert.True(ctx.Database.Connection as SqlCeConnection != null);

                var stud = new Student() { StudentName = "Bill" };

                ctx.Students.Add(stud);
                ctx.SaveChanges();
            }

            using (var ctx = new SchoolContext(connectionString))
            {
                var students = ctx.Students.ToList();

                Assert.True(students.Count > 0);

                Assert.True(ctx.Database.Connection as SqlCeConnection != null);
            }
        }
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public byte[] Photo { get; set; }
        public decimal Height { get; set; }
        public float Weight { get; set; }

        public Grade Grade { get; set; }
    }

    public class Grade
    {
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public string Section { get; set; }

        public ICollection<Student> Students { get; set; }
    }

    [DbConfigurationType(typeof(System.Data.Entity.SqlServerCompact.SqlCeDbConfiguration))]
    public class SchoolContext : DbContext
    {
        public SchoolContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<SchoolContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public SchoolContext(SqlCeConnection connection) : base(connection, true)
        { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
    }
}
