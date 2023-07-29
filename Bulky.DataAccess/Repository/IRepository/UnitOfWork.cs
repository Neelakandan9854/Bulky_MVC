using Bulky.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository category { get;private set; }
        public IProductRepository product { get; private set; }

        public ICompanyRepository company { get; private set; }

        public ICartRepository carts { get; private set; }    

        public IApplicationUserRepository applicationUser { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        
        public UnitOfWork(ApplicationDbContext Db)
        {
           _db= Db;
            applicationUser = new ApplicationUserRepository(_db);
            carts = new CartRepository(_db);
            category = new CategoryRepository(_db);
            product = new ProductRepository(_db);
            company = new CompanyRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
