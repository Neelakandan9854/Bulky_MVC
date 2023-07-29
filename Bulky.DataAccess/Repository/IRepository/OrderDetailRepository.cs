﻿using Bulky.DataAccess.Data;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository 
    {

        private readonly ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db):base(db) { 
        
               _db= db;
        }

        public void Update(OrderDetail obj)
        {
            _db.Update(obj);
        }
    }
}
