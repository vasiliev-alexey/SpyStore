using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EF;
using SpyStore.Models.Entities;
using Xunit;

namespace SpyStore.DAL.Tests
{
    [Collection("SpyStore.DAL")]
    public class CategoryTests : IDisposable
    {
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

        private readonly StoreContext _db;


        private void CleanDatabase()
        {
            _db.Database.ExecuteSqlCommand("truncate  \"Categories\"  restart identity cascade");

         
        }

        [Fact]
        public void FirstTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldAddACategoryWithDbSet()
        {
            var category = new Category {CategoryName = "Foo"};
            _db.Categories.Add(category);
            Assert.Equal(EntityState.Added, _db.Entry(category).State);
            Assert.True(category.Id < 0);
            Assert.Null(category.TimeStamp);
            var rs = _db.SaveChanges();

            Debug.Write(rs);

            Assert.Equal(EntityState.Unchanged, _db.Entry(category).State);
            Assert.Equal(1, category.Id);
            //   Assert.NotNull(category.TimeStamp);
            Assert.Equal(1, _db.Categories.Count());
        }

        [Fact]
        public void ShouldGetAllCategoriesOrderedByName()
        {
            _db.Categories.Add(new Category {CategoryName = "Foo"});
            _db.Categories.Add(new Category {CategoryName = "Bar"});
            _db.SaveChanges();
            var categories = _db.Categories.OrderBy(c => c.CategoryName).ToList();
            Assert.Equal(2, _db.Categories.Count());
            Assert.Equal("Bar", categories[0].CategoryName);
            Assert.Equal("Foo", categories[1].CategoryName);
        }


        [Fact]
        public void ShouldNotUpdateANonAttachedCategory()
        {
            var category = new Category {CategoryName = "Foo"};
            _db.Categories.Add(category);
            category.CategoryName = "Bar";
            Assert.Throws<InvalidOperationException>(() => _db.Categories.Update(category));
        }

        [Fact]
        public void ShouldUpdateACategory()
        {
            var category = new Category {CategoryName = "Foo"};
            _db.Categories.Add(category);
            _db.SaveChanges();
            category.CategoryName = "Bar";
            _db.Categories.Update(category);
            Assert.Equal(EntityState.Modified, _db.Entry(category).State);
            _db.SaveChanges();
            Assert.Equal(EntityState.Unchanged, _db.Entry(category).State);
            //  StoreContext context;
            using (var context = new StoreContext())
            {
                Assert.Equal("Bar", context.Categories.First().CategoryName);
            }
        }


        [Fact]
        public void ShouldDeleteACategory()
        {
            var category = new Category {CategoryName = "Foo"};
            _db.Categories.Add(category);
            _db.SaveChanges();
            Assert.Equal(1, _db.Categories.Count());
            _db.Categories.Remove(category);
            Assert.Equal(EntityState.Deleted, _db.Entry(category).State);
            _db.SaveChanges();
            Assert.Equal(EntityState.Detached, _db.Entry(category).State);
            Assert.Equal(0, _db.Categories.Count());
        }
    }
}