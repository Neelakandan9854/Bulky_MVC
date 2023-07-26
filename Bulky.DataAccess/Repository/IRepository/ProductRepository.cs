using Bulky.DataAccess.Data;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public class ProductRepository : Repository<Product>, IProductRepository 
    {

        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db):base(db) { 
        
               _db= db;
        }

        public void Update(Product obj)
        {
            var objfromdb= _db.Products.FirstOrDefault(u=> u.id==obj.id);
            if (objfromdb != null)
            {
                objfromdb.Tittle = obj.Tittle;
                objfromdb.Description = obj.Description;
                objfromdb.ISBN= obj.ISBN;
                objfromdb.Author = obj.Author;
                objfromdb.price = obj.price;
                objfromdb.Listprice = obj.Listprice;
                objfromdb.price50 = obj.price50;
                objfromdb.price100 = obj.price100;
                objfromdb.CategoryId = obj.CategoryId;
                if(obj.ImageUrl!= null)
                {
                    objfromdb.ImageUrl = obj.ImageUrl;
                }


            }
        }
    }
}
