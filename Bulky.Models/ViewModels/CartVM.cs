﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class CartVM
    {
        public IEnumerable<Carts> Listcarts { get; set; }
         public OrderHead OrderHead { get; set; }
        
    }
}
