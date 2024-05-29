using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext dbContext;

        //private Dictionary<string, GenericRepository<BaseEntity>> repositories;

        private Hashtable repositories;

        public UnitOfWork(StoreContext dbContext)
        {
            this.dbContext = dbContext;
            //repositories = new Dictionary<string, GenericRepository<BaseEntity>>();
            repositories = new Hashtable();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).Name;

            if(!repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<TEntity>(dbContext);

                repositories.Add(key, repository);
            }

            return repositories[key] as IGenericRepository<TEntity>;
        }


        public async Task<int> CompleteAsync()
            => await dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await dbContext.DisposeAsync();

       
    }
}
