using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITaxRateRepository
    {
        Task<IEnumerable<TaxRate>> GetTaxRates();
        Task<TaxRate> GetTaxRateId(int id, bool trackChanges);
        Task<TaxRate> GetTaxRateIdByZoneId(int zoneId, bool trackChanges);
        void AddTaxRate(TaxRate taxClass);
        void DeleteTaxRate(TaxRate taxClass);
    }
}
