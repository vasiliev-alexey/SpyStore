using System;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EF;
using Xunit;

namespace SpyStore.DAL.Tests
{

    [Collection("SpyStore.DAL")]
    public class CategoryTests :    IDisposable
    {

        private readonly StoreContext _db;

        public CategoryTests()
        {
            _db = new StoreContext();
            CleanDatabase();
        }

        public void Dispose()
        {
            CleanDatabase();
            _db.Dispose();
        }


        private void CleanDatabase()
        {
            _db.Database.ExecuteSqlCommand("Delete from Store.Categories");
          
        }

        [Fact]
        public void FirstTest()
        {
            Assert.True(true);
        }


    }
}