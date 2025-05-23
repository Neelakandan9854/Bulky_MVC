﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string Tittle { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }

        [Required]
        [Display (Name = "List price")]
        [Range(1, 1000)]
        public double Listprice { get; set; }

        [Required]
        [Display(Name = "price for 1-50")]
        [Range(1, 1000)]
        public double price { get; set; }


        [Required]
        [Display(Name = "price for 50-100")]
        [Range(1, 1000)]
        public double price50 { get; set; }


        [Required]
        [Display(Name = "price for 100+")]
        [Range(1, 1000)]
        public double price100 { get; set; }

      
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
    }
}
