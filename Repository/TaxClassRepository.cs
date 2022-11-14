using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Asn1;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TaxClassRepository : RepositoryBase<TaxClass>, ITaxClassRepository
    {
        public TaxClassRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<IEnumerable<TaxClass>> GetTaxClasses()
          => await FindAll(false).ToListAsync();
        public async Task<TaxClass> GetTaxClassId(int id, bool trackChanges)
         => await FindByCondition(r => r.Id == id, trackChanges).FirstOrDefaultAsync();
        public void AddTaxClass(TaxClass taxClass) => Create(taxClass);
        public void DeleteTaxClass(TaxClass taxClass) => Delete(taxClass);
    }
}
