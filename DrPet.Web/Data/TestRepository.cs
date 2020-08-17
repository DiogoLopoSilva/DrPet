using DrPet.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data
{
    public class TestRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly DataContext context;

        public TestRepository(DataContext context)
        {
            this.context = context;
        }

        public Task CreateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {   
            var a = new Animal
            {
                Name = "TEST",
                Color="TEST",
                Breed="TEST",
                Species="TEST",
                Sex="TEST",
                DateOfBirth=DateTime.Now,
                User = this.context.Users.Find("diogo.lopo.silva@formandos.cinel.pt")
            };

            var animal = context.Animals.FirstOrDefault(an => an.Name == a.Name);

            if (animal==null)
            {
                this.context.Animals.Add(a);
                this.context.SaveChanges();
            }

            return this.context.Set<T>().AsNoTracking();
        }

        public Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
