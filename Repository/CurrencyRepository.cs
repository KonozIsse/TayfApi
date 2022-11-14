using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Models.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CurrencyRepository : RepositoryBase<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Currency>> GetActiveCurrencies(bool trackChanges)
        => await FindByCondition(c => c.IsStatus == Status.Active, trackChanges).ToListAsync();
        public async Task<Currency> GetDefaultCurrency(bool trackChanges)
        => await FindByCondition(c => c.IsDefault == 1 && c.IsStatus == Status.Active, trackChanges).SingleOrDefaultAsync();
        public async Task<List<Currency>> GetAllCurrencies(bool trackChanges)
         => await FindAll(trackChanges).ToListAsync();
        public async Task<Currency> GetCurrency(int id , bool trackChanges)
        => await FindByCondition(c => c.Id == id, trackChanges).SingleOrDefaultAsync();
        public bool ExistCurrency(string code) => FindByCondition(x => x.Symbol == code ,false).Count() > 0;
        public void DeleteCurrency(Currency currency) => Delete(currency);
        public void AddCurrency(Currency currency) => Create(currency);
    }
}
