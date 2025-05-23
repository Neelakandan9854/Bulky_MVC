﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
         
        public string StreetAdress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostelCode { get; set; }
        public string PhoneNumber { get; set; }

    }
}
