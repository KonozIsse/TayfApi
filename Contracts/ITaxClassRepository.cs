using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITaxClassRepository
    {
        Task<IEnumerable<TaxClass>> GetTaxClasses(string search);
        bool ExistTax(string name);
        Task<TaxClass> GetTaxClassId(int id, bool trackChanges);
        void AddTaxClass(TaxClass taxClass);
        void DeleteTaxClass(TaxClass taxClass);
    }
}
