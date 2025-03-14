﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDishOfProj.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }
        public string? Image {  get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
