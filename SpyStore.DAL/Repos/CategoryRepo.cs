using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpyStore.DAL.EF;
using SpyStore.DAL.Repos.Base;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.Repos
{
    internal class CategoryRepo : RepoBase<Category>
    {
        protected CategoryRepo()
        {
        }

        protected CategoryRepo(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public override IEnumerable<Category> GetAll()
        {
            return Table.OrderBy(x => x.CategoryName);
        }

        public override IEnumerable<Category> GetRange(int skip, int take)
        {
            return GetRange(Table.OrderBy(x => x.CategoryName), skip, take);
        }
    }
}