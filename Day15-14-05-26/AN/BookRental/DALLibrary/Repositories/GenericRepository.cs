using DALLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using DALLibrary.Interfaces;
using DALLibrary.Exceptions;
using System;

namespace DALLibrary.Repositories
{
    public abstract class GenericRepository<T>
        : IGenericRepository<T>
        where T : class
    {
        protected readonly BookRentalContext _context;

        protected readonly DbSet<T> _dbSet;

        protected GenericRepository(BookRentalContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}