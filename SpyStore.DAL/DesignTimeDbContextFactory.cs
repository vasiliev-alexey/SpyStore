using System.IO;
 
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
           
            var builder = new DbContextOptionsBuilder<StoreContext>();
            builder.UseSqlite("Filename=SpyStoreDAL.sqlite");
            return new StoreContext(builder.Options);
        }
    }
}