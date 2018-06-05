using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SpyStore.DAL.EF;

namespace SpyStore.DAL
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StoreContext>
    {
        public StoreContext CreateDbContext(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("app.json", false)
                .Build();


            var configuration = configBuilder.Build();


            var builder = new DbContextOptionsBuilder<StoreContext>();
            var useDbType = configuration["use_db_type"];

            switch (useDbType)
            {
                case "pg":

                {
                    Console.WriteLine("*****    PG - construct*****");
                        var connString =
                        $"Host = {configuration["pg_host"]}; Port =  {configuration["pg_port"]}; Database =  {configuration["pg_db"]}; Username =  {configuration["pg_user"]}; Password =  {configuration["pg_pass"]}";

                    builder.UseNpgsql(connString );
                    Console.WriteLine("*****PG - constructed *****");
                    }

                    break;

                case "sql_lite":
                    builder.UseSqlite("Filename=SpyStoreDAL.sqlite");
                    break;
                default:
                    throw new ArgumentException();
            }


         
            return new StoreContext(builder.Options);
        }
    }
}