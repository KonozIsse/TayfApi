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
        public async Task<IEnumerable<TaxClass>> GetTaxClasses(string search, string filter)
        {
            var taxes = FindAll(false);
            if (!string.IsNullOrEmpty(search))
            {
                if(filter == "0")
                {
                    taxes = taxes.Where(c => c.Title.Contains(search));
                }
                else if(filter == "1")
                {
                    taxes = taxes.Where(c => c.Description.Contains(search));
                }
                else
                {
                    taxes = taxes.Where(c => c.Title.Contains(search) || c.Description.Contains(search));
                }
               
            }
            return await taxes.ToListAsync();
        }
        public bool ExistTax(string name)
          => FindByCondition(c=>c.Title == name ,false).Count() > 0;
        public async Task<TaxClass> GetTaxClassId(int id, bool trackChanges)
         => await FindByCondition(r => r.Id == id, trackChanges).FirstOrDefaultAsync();
        public void AddTaxClass(TaxClass taxClass) => Create(taxClass);
        public void DeleteTaxClass(TaxClass taxClass) => Delete(taxClass);
    }
}
