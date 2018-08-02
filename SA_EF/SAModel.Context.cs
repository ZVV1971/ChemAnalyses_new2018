﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace SA_EF
{
    public partial class ChemicalAnalysesEntities : DbContext
    {
        //public static bool IsLoginPwdSet { get; set; } = false;
        //private static string _connectionString = "name=CAEntities";
        //public static string connectionString
        //{
        //    get
        //    {
        //        if (_connectionString.StartsWith("name=")) return _connectionString;
        //        string pattern = @"^(.+MSSQLLocalDB)(;attachdbfilename=)([^;]+)(.+integrated security=)([^;]+)(;initial catalog=[^;]+)(.+)";
        //        RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
        //        Regex regex = new Regex(pattern, options);
        //        string replacement = "$1$4False;User ID= " + UserName + "Password=" + Password + "$6$7";
        //        return regex.Replace(_connectionString, replacement);
        //    }
        //    set
        //    {
        //        _connectionString = value;
        //    }
        //} 
        public static string UserName { get; set; } = "CAAdmin";
        public static string Password { get; set; } = "CAAdminPassword";

        public static string connectionString { get; set; }= "name=CAEntities";

        public ChemicalAnalysesEntities() :base(connectionString) {}

        public void s()
        {
            this.Database.Connection.StateChange += (sender, args) =>
             {
                 if (args.CurrentState == ConnectionState.Open)
                 {
                //     SqlCommand cmd = new SqlCommand();

                //     cmd.CommandText = "StoredProcedureName";
                //     cmd.CommandType = CommandType.StoredProcedure;
                //     cmd.Parameters.AddRange(new SqlParameter[]
                //     {
                //new SqlParameter("rolename", UserName),
                //new SqlParameter("password", Password)
                //     });
                //     //((IObjectContextAdapter)this).ObjectContext.Connection
                //     cmd.ExecuteScalar();
                 }
             };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LinearCalibration> LineaCalibrations { get; set; }
        public virtual DbSet<DataPoint> DataPoints { get; set; }
        public virtual DbSet<SaltAnalysisData> SaltAnalysisDatas { get; set; }
        public virtual DbSet<CalibrationType> CalibrationType { get; set; }
        public virtual DbSet<Sample> Samples { get; set; }
    
        public virtual int GetSamplesByMultipleIDs()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetSamplesByMultipleIDs");
        }
    
        public virtual int UpdateCalibrationData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateCalibrationData");
        }
    }
}