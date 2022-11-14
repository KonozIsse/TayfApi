using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICurrencyRepository
    {
        Task<List<Currency>> GetActiveCurrencies(bool trackChanges);
        Task<Currency> GetDefaultCurrency(bool trackChanges);
        Task<List<Currency>> GetAllCurrencies(bool trackChanges);
        Task<Currency> GetCurrency(int id, bool trackChanges);
        bool ExistCurrency(string code);
        void DeleteCurrency(Currency currency);
        void AddCurrency(Currency currency);
    }
}
