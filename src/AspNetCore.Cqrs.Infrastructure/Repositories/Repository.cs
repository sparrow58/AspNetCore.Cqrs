﻿using AspNetCore.Cqrs.Core.Abstractions.Entities;
using AspNetCore.Cqrs.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Cqrs.Infrastructure.Repositories
{
    internal class Repository<T> : IRepository<T> where T : AggregateRoot
    {
        private readonly WeatherContext _context;
        private readonly DbSet<T> _entitySet;

        public Repository(WeatherContext context)
        {
            _context = context;
            _entitySet = _context.Set<T>();
        }

        public IQueryable<T> GetAll(bool noTracking = true)
        {
            var set = _entitySet;
            if (noTracking)
            {
                return set.AsNoTracking();
            }
            return set;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await GetAll(false)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public void Insert(T entity)
        {
            _entitySet.Add(entity);
        }

        public void Insert(List<T> entities)
        {
            _entitySet.AddRange(entities);
        }

        public void Delete(T entity)
        {
            _entitySet.Remove(entity);
        }

        public void Remove(IEnumerable<T> entitiesToRemove)
        {
            _entitySet.RemoveRange(entitiesToRemove);
        }
    }
}