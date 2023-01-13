using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICountryRepository
    {
        Task<List<Country>> GetCountriesCountZones(bool trackChanges);
        Task<List<Country>> GetCountries();
        Task<Country> GetcountryById(int id, bool trackChanges);
        bool ExistCountry(string countryName, string code);
        Task<Country> GetCountryByCode(string code, bool trackChanges);
        Task<IEnumerable<Country>> GetAllCountries(string search);
        void AddCountry(Country country);
        void DeleteCountry(Country country);
    }
    public interface IZoneRepository
    {
        bool ExistZone(string name, string code);
        Task<List<Zone>> GetZonesByCountryId(int countryId);
        Task<Zone> GetZoneIdCountryId(int id, int countryId, bool trackChanges);
        Task<Zone> GetZoneId(int id, bool trackChanges);
        Task<List<Zone>> GetAllZones(string search);
        void AddZone(int countryId ,Zone zone);
        void DeleteZone(Zone zone);
    }
}
