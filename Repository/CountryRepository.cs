using Contracts;
using Entities;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CountryRepository : RepositoryBase<Country>, ICountryRepository
    {
        public CountryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Country>> GetCountriesCountZones(bool trackChanges)
        => await FindByCondition(c => c.Zones.Count() > 0, trackChanges).Include(x => x.Zones).ToListAsync();
        public async Task<List<Country>> GetCountries()
        => await FindAll(false).Include(x => x.Zones).Include(c=>c.Image).ToListAsync();
        public async Task<IEnumerable<Country>> GetAllCountries(string search)
        { 
            var countries = FindAll(false);
            if (!String.IsNullOrEmpty(search))
            {
                countries.Where(c=>c.CountryName.Contains(search) || c.CountryCode2.Contains(search)
                || c.CountryCode3.Contains(search)).ToList();
            }
            return await countries.Include(x => x.Zones).Include(c => c.Image).ToListAsync();
        }
        public async Task<Country> GetcountryById(int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id , trackChanges).FirstOrDefaultAsync();
        public bool ExistCountry(string countryName, string code)
         => FindByCondition(x => x.CountryName == countryName && x.MobileCode == code,false).Count() > 0;
        public async Task<Country> GetCountryByCode(string code, bool trackChanges)
         => await FindByCondition(c => c.MobileCode == code, trackChanges).FirstOrDefaultAsync();
        public void AddCountry(Country country) => Create(country);
        public void DeleteCountry(Country country) => Delete(country);
    }
    public class ZoneRepository : RepositoryBase<Zone>, IZoneRepository
    {
        public ZoneRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public bool ExistZone(string name, string code)
        => FindByCondition(x => x.ZoneName == name && x.ZoneCode == code, false).Count() > 0;
        public async Task<List<Zone>> GetZonesByCountryId(int countryId)
        => await FindByCondition(c => c.CountryId == countryId, false).ToListAsync();
        public async Task<Zone> GetZoneId (int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<Zone> GetZoneIdCountryId(int id,int countryId , bool trackChanges)
       => await FindByCondition(c => c.Id == id&& c.CountryId == countryId, trackChanges).FirstOrDefaultAsync();
        public async Task<List<Zone>> GetAllZones() => await FindAll(false).Include(c=>c.Country).ToListAsync();

        public void AddZone(int countryId, Zone zone) 
        { 
            zone.CountryId = countryId;
            Create(zone); 
        }
        public void DeleteZone(Zone zone) => Delete(zone);
    }
}
