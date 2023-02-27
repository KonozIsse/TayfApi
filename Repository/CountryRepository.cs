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
        public async Task<List<Country>> GetCountries()
        => await FindAll(false).Include(x => x.Zones).Include(c=>c.Image).ToListAsync();
        public async Task<List<Country>> GetCountriesImage(int ImageId)
       => await FindByCondition(c=>c.ImgId== ImageId,false).ToListAsync();
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
        public void AddCountry(Country country) => Create(country);
        public void DeleteCountry(Country country) => Delete(country);
    }
    public class ZoneRepository : RepositoryBase<Zone>, IZoneRepository
    {
        public ZoneRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
       
        public async Task<List<Zone>> GetZonesByCountryId(int countryId)
        => await FindByCondition(c => c.CountryId == countryId, false).ToListAsync();
        public async Task<Zone> GetZoneId (int id, bool trackChanges)
         => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
       public async Task<List<Zone>> GetAllZones(string search)
        {
            var zones = FindAll(false);
            if (!string.IsNullOrEmpty(search))
            {
                zones.Where(c => c.ZoneName.Contains(search) || c.ZoneCode.Contains(search) || c.Country.CountryName.Contains(search));
            }
           return await zones.Include(c => c.Country).ToListAsync();
        }

        public void AddZone(int countryId, Zone zone) 
        { 
            zone.CountryId = countryId;
            Create(zone); 
        }
        public void DeleteZone(Zone zone) => Delete(zone);

        public bool ExistZone(string zoneName, string code)
       => FindByCondition(x => x.ZoneName == zoneName && x.ZoneCode == code, false).Count() > 0;
    }
}
