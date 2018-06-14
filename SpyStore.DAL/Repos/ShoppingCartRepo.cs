using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SpyStore.DAL.EF;
using SpyStore.DAL.Repos.Base;
using SpyStore.DAL.Repos.Interfaces;
using SpyStore.Models.Entities;
using SpyStore.Models.ViewModels;
//using System.Data.SqlClient;

namespace SpyStore.DAL.Repos
{
    public class ShoppingCartRepo : RepoBase<ShoppingCartRecord>, IShoppingCartRepo
    {
        private readonly IProductRepo _productRepo;

        public ShoppingCartRepo(DbContextOptions<StoreContext> options,
            IProductRepo productRepo) : base(options)
        {
            _productRepo = productRepo;
        }

        public ShoppingCartRepo(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        public override IEnumerable<ShoppingCartRecord> GetAll()
        {
            return Table.OrderByDescending(x => x.DateCreated);
        }

        public override IEnumerable<ShoppingCartRecord> GetRange(int skip, int take)
        {
            return GetRange(Table.OrderByDescending(x => x.DateCreated), skip, take);
        }

        public ShoppingCartRecord Find(int customerId, int productId)
        {
            return Table.FirstOrDefault(
                x => x.CustomerId == customerId && x.ProductId == productId);
        }

        public override int Update(ShoppingCartRecord entity, bool persist = true)
        {
            return Update(entity, _productRepo.Find(entity.ProductId)?.UnitsInStock, persist);
        }

        public int Update(ShoppingCartRecord entity, int? quantityInStock, bool persist = true)
        {
            if (entity.Quantity <= 0) return Delete(entity, persist);
            if (entity.Quantity > quantityInStock)
                throw new InvalidQuantityException("Can't add more product than available in stock");
            return base.Update(entity, persist);
        }

        public override int Add(ShoppingCartRecord entity, bool persist = true)
        {
            return Add(entity, _productRepo.Find(entity.ProductId)?.UnitsInStock, persist);
        }

        public int Add(ShoppingCartRecord entity, int? quantityInStock, bool persist = true)
        {
            var item = Find(entity.CustomerId, entity.ProductId);
            if (item == null)
            {
                if (quantityInStock != null && entity.Quantity > quantityInStock.Value)
                    throw new InvalidQuantityException("Can't add more product than available in stock");
                return base.Add(entity, persist);
            }

            item.Quantity += entity.Quantity;
            return item.Quantity <= 0 ? Delete(item, persist) : Update(item, quantityInStock, persist);
        }

        public CartRecordWithProductInfo GetShoppingCartRecord(int customerId, int productId)
        {
            return Table
                .Where(x => x.CustomerId == customerId && x.ProductId == productId)
                .Include(x => x.Product)
                .ThenInclude(p => p.Category)
                .Select(x => GetRecord(customerId, x, x.Product, x.Product.Category))
                .FirstOrDefault();
        }

        public IEnumerable<CartRecordWithProductInfo> GetShoppingCartRecords(int customerId)
        {
            return Table
                .Where(x => x.CustomerId == customerId)
                .Include(x => x.Product)
                .ThenInclude(p => p.Category)
                .Select(x => GetRecord(customerId, x, x.Product, x.Product.Category))
                .OrderBy(x => x.ModelName);
        }

        public int Purchase(int customerId)
        {
            var customerIdParam = new NpgsqlParameter("@customerId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Input,
                Value = customerId
            };
            var orderIdParam = new NpgsqlParameter("@orderId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            using (var conn = Context.Database.GetDbConnection())
            {
                var command = conn.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(customerIdParam);
                command.Parameters.Add(orderIdParam);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return -1;
                }
            }

            return (int) orderIdParam.Value;
        }

        private static CartRecordWithProductInfo GetRecord(
            int customerId, ShoppingCartRecord scr, Product p, Category c)
        {
            return new CartRecordWithProductInfo
            {
                Id = scr.Id,
                DateCreated = scr.DateCreated,
                CustomerId = customerId,
                Quantity = scr.Quantity,
                ProductId = scr.ProductId,
                Description = p.Description,
                ModelName = p.ModelName,
                ModelNumber = p.ModelNumber,
                ProductImage = p.ProductImage,
                ProductImageLarge = p.ProductImageLarge,
                ProductImageThumb = p.ProductImageThumb,
                CurrentPrice = p.CurrentPrice,
                UnitsInStock = p.UnitsInStock,
                CategoryName = c.CategoryName,
                LineItemTotal = scr.Quantity * p.CurrentPrice
                // TimeStamp = scr.TimeStamp
            };
        }
    }
}