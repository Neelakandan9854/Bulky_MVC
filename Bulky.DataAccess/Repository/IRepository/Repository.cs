﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository.IRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> DbSet;
        public Repository(ApplicationDbContext Db) 
        {
              _db= Db;
            this.DbSet=_db.Set<T>();
            _db.Products.Include(u =>u. Category).Include(u =>u.CategoryId);

        }
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeproperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = DbSet;
                
            }
            else
            {
                 query = DbSet.AsNoTracking();
            }
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeproperties))
            {
                foreach (var property in includeproperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeproperties=null)
        {
            IQueryable<T> query = DbSet;
            if(filter != null)
            {
                query = query.Where(filter);
            }
            
            if (!string.IsNullOrEmpty(includeproperties))
            {
                foreach(var property in includeproperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)) 
                { 
                   query= query.Include(property);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            DbSet.RemoveRange(entity);
        }
    }
}
