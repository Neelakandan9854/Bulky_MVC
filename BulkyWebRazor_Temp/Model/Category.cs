﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [DisplayName(" Display Order")]
        [Range(1, 100)]
        public int DisplayOrder { get; set; }
    }
}
