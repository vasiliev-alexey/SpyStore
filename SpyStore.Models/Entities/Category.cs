using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SpyStore.Models.Entities.Base;

namespace SpyStore.Models.Entities
{
    [Table("categories", Schema = "store")]
    public class Category : EntityBase
    {
        [DataType(DataType.Text), MaxLength(50)]
        public string CategoryName { get; set; }

        [InverseProperty(nameof(Product.Category))]
        public List<Product> Products { get; set; } = new List<Product>();


    }
}
