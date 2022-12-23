using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Repository
{
    public class TaxRateRepository : RepositoryBase<TaxRate>, ITaxRateRepository
    {
        public TaxRateRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<TaxRate>> GetTaxRates(string seach)
         => await FindByCondition(c=>c.Zone.ZoneName.Contains(seach),false).OrderByDescending(x => x.CreatedAt).ToListAsync();
        public async Task<TaxRate> GetTaxRateId(int id , bool trackChanges)
         => await FindByCondition(r => r.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<TaxRate> GetTaxRateIdByZoneId (int zoneId, bool trackChanges)
        => await FindByCondition(r => r.ZoneId == zoneId, trackChanges).FirstOrDefaultAsync();
        public void AddTaxRate(TaxRate taxRate) => Create(taxRate);
        public void DeleteTaxRate(TaxRate taxRate) => Delete(taxRate);
        public bool ExistTaxRates(int zoneId)
         => FindByCondition(c => c.ZoneId == zoneId, false).Count() > 0;
    }
}
