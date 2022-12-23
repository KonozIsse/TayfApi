using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Formats.Asn1;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Zone = Entities.Models.Zone;

namespace BusinessLogic.ApiClasses
{
    public class LocationTaxBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly ImageBL _imageBL;
        private readonly LocService _locService;
        private readonly Language _language ;
        
        public LocationTaxBL(IRepositoryManager repositoryManager, IMapper mapper, ImageBL imageBL, LocService locService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageBL = imageBL;
            _locService = locService;
        }
        //Address------------------------------------------------
        public async Task<Address> GetAddressId(int id)
        {
            return await _repositoryManager.Address.GetAddress(id, false);
        }
        public async Task<Address> GetAddressUser(int id, int user)
        {
            return await _repositoryManager.Address.GetAddressIdByCustomerId(id, user, false);
        }
        public async Task CreateAddress(int userId, CreateAddressDto create)
        {
            var address = _mapper.Map<Address>(create);
            address.UserId = userId;
            address.User.FirstName = create.FirstName;
            address.User.LastName = create.LastName;
            address.CityName = address.Zone.ZoneName;
            _repositoryManager.Address.AddAddress(address);
            await _repositoryManager.SaveAsync();
            if (create.IsDefault == true)
            {
                await AddDefultAddress(address.Id, userId);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<BussnessResultModel> DeleteAddress(int id, int customerId)
        {
            var address = await _repositoryManager.Address.GetAddressIdByCustomerId(id, customerId, false);
            if(address == null)
            {
                return new BussnessResultModel(null ,_locService.GetLocalizedStringValue("correctLink"), false);
            }
            var user = await _repositoryManager.User.GetUserId(customerId, true);
            if (user != null && address.IsDefault == true)
            {
                user.DefaultAddressId = 0;
            }
            _repositoryManager.Address.DeleteAddress(address);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(address , _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task AddDefultAddress(int defaultAddressId, int userId)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true);
            if (user != null)
            {
                user.DefaultAddressId = defaultAddressId;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task EditAddress(int userId ,UpdateAddressDto update)
        {
            var address = await _repositoryManager.Address.GetAddressIdByCustomerId(update.Id, userId, true);
            address.UserId = userId;
            _mapper.Map(update, address);
            if (update.IsDefault == true)
            {
                await AddDefultAddress(update.Id, userId);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<AddressDto> DefaultAddress(int customerId)
        {
            var address = await _repositoryManager.Address.GetAddressCustomer(customerId);
            if(address == null)
            {
                return null;
            }
            var addressDto = _mapper.Map<AddressDto>(address);
            return addressDto;
        }
        public async Task<List<AddressDto>> GetAddressesCustomerId (int customerId)
        {
            var addresses = await _repositoryManager.Address.GetAllAddressesByCustomerId(customerId);
            if (addresses == null)
            {
                return null;
            }
            var addressDto = _mapper.Map<List<AddressDto>>(addresses);
            var address = addresses.First();
            addressDto.Select(c => c.CustomerName = address.User.FullName);
            return addressDto;
        }
        //Country------------------------------------------------

        public async Task AddCountry(CreateCountryDto createCountryDto)
        {
            var country = _mapper.Map<Country>(createCountryDto);
            _repositoryManager.Country.AddCountry(country);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteCountry(int id)
        {
            var country = await _repositoryManager.Country.GetcountryById(id, false);
            if(country != null)
            {
                var zones = await _repositoryManager.Zone.GetZonesByCountryId(id);
                if(zones != null)
                {
                    foreach (var zone in zones)
                    {
                        _repositoryManager.Zone.DeleteZone(zone);
                    }
                }
                _repositoryManager.Country.DeleteCountry(country);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task EditCountry(UpdateCountryDto updateDto)
        {
            var country = await _repositoryManager.Country.GetcountryById(updateDto.Id, true);
            if(country != null)
            {
                _mapper.Map(updateDto, country);
            }
            await _repositoryManager.SaveAsync();
        }
        public bool CheckCountryExist(string countryName, int countryCode)
        {
            return _repositoryManager.Country.ExistCountry(countryName, countryCode);
        }
        public async Task<Country> GetCountryCode(int code)
        {
            return await _repositoryManager.Country.GetCountryByCode(code, false);
        }
        public async Task<List<CountryDto>> GetCountriesForWeb()
        {
            var countries = await _repositoryManager.Country.GetCountries();
            var countriesDto = _mapper.Map<List<CountryDto>>(countries);

            var country = countries.First();
            var countryDto = countriesDto.First();
            countryDto.ImageId = Convert.ToInt32(await _imageBL.GetImageOriginal(country.ImgId.ToString()));
            countryDto.CountryName = _language.Code == "en" ? country.CountryName : country.CountryNameAr;
            return countriesDto;
        } 
        public async Task<CountryDto> GetCountry(int id)
        {
            var country = await _repositoryManager.Country.GetcountryById(id , false);
            var countryDto = _mapper.Map<CountryDto>(country);
            countryDto.CountryName = _language.Code == "en" ? country.CountryName : country.CountryNameAr;
            return countryDto;
        }
        //Zone------------------------------------------------
        public bool CheckZoneExist(string name, string code)
        {
            return _repositoryManager.Zone.ExistZone(name, code);
        }
        public async Task<List<Zone>> GetZones()
        {
            return await _repositoryManager.Zone.GetAllZones();
        }
        public async Task<List<ZoneDto>> GetZonesByCountryId(int countryId)
        {
            var zones = await _repositoryManager.Zone.GetZonesByCountryId(countryId);
            var zonesDto = _mapper.Map<List<ZoneDto>>(zones);
            var zone = zones.First();
            var zoneDto = zonesDto.First();
            zoneDto.ZoneName = _language.Code == "en" ? zone.ZoneName : zone.ZoneNameAr;
            return zonesDto;
        }
        public async Task AddZone(CreateZoneDto createZoneDto)
        {
            var zone = _mapper.Map<Zone>(createZoneDto);
            _repositoryManager.Zone.AddZone(createZoneDto.CountryId, zone);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteZone(int id)
        {
            var zone = await _repositoryManager.Zone.GetZoneId(id, false);
            _repositoryManager.Zone.DeleteZone(zone);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditZone (UpdateZoneDto updateDto)
        {
            var zone = await _repositoryManager.Zone.GetZoneId(updateDto.Id, true);
            _mapper.Map(updateDto, zone);
            await _repositoryManager.SaveAsync();
        }
        //TaxClass------------------------------------------------
        public bool CheckTaxesExist(string name)
        {
            return _repositoryManager.TaxClass.ExistTax(name);
        }
        public async Task<List<TaxClassDto>> GetTaxes()
        {
            var taxes = await _repositoryManager.TaxClass.GetTaxClasses();
            var taxesDto = _mapper.Map<List<TaxClassDto>>(taxes);
            var taxe = taxes.First();
            var taxeDto = taxesDto.First();
            taxeDto.Title = _language.Code == "en" ? taxe.Title : taxe.TitleAr;
            taxeDto.Description = _language.Code == "en" ? taxe.Description : taxe.DescriptionAr;
            return taxesDto;
        }
        public async Task AddTaxClass(CreateTaxClassDto createTaxClassDto)
        {
            var taxClass = _mapper.Map<TaxClass>(createTaxClassDto);
            //if(storeId != 0)
            //{
            //    taxClass.StoreId = storeId;
            //}
            _repositoryManager.TaxClass.AddTaxClass(taxClass);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditTaxClass( UpdateTaxClassDto updateDto)
        {
            var taxClass = await _repositoryManager.TaxClass.GetTaxClassId(updateDto.Id, true);
            _mapper.Map(updateDto, taxClass);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteTaxClass(int id)
        {
            var taxClass = await _repositoryManager.TaxClass.GetTaxClassId(id, false);
            _repositoryManager.TaxClass.DeleteTaxClass(taxClass);
            await _repositoryManager.SaveAsync();
        }
        //TaxRate------------------------------------------------
        public bool CheckTaxRatesExist(int zoneId)
        {
            return _repositoryManager.TaxRate.ExistTaxRates(zoneId);
        }
        public async Task<IEnumerable<TaxRate>> GetTaxeRates(string seach)
        {
            //var taxes =
            return await _repositoryManager.TaxRate.GetTaxRates(seach);
            //var taxesDto = _mapper.Map<List<TaxRateDto>>(taxes);
            //return taxesDto;
        } 
        public async Task<TaxRate> GetTaxeRate(int id)
        {
          return await _repositoryManager.TaxRate.GetTaxRateId(id, false);
        }
        public async Task AddTaxRate(CreateTaxRateDto createTaxRateDto)
        {
            var taxRate = _mapper.Map<TaxRate>(createTaxRateDto);
            _repositoryManager.TaxRate.AddTaxRate(taxRate);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditTaxRate(UpdateTaxRateDto updateDto)
        {
            var taxRate = await _repositoryManager.TaxRate.GetTaxRateId(updateDto.Id, true);
            if (taxRate != null)
            {
                _mapper.Map(updateDto, taxRate);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task DeleteTaxRate(int id)
        {
            var taxRate = await _repositoryManager.TaxRate.GetTaxRateId(id, false);
            if (taxRate != null)
            {
                _repositoryManager.TaxRate.DeleteTaxRate(taxRate);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task<decimal> CalculateTaxToZoneId(int zoneId)
        {
            decimal tax = 0;
            var zone = await _repositoryManager.TaxRate.GetTaxRateIdByZoneId(zoneId, true);
            if (zone != null)
            {
                tax = zone.Tax_Rate;
            }
            return tax;
        }
        public async Task<decimal> GetTax(int customerId)
        {
            decimal tax = 0;
            var customer = await _repositoryManager.User.GetCustomerId(customerId, false);
            if (customer != null && customer.DefaultAddressId != 0)
            {
                var defaultAddress = await _repositoryManager.Address.GetAddressIdByCustomerId(customer.DefaultAddressId.Value, customerId, false);
                if (defaultAddress != null)
                {
                    tax = await CalculateTaxToZoneId(defaultAddress.ZoneId);
                }
            }
            return tax;
        }
        //settings------------------------------------------------
        public async Task<List<SettingDto>> Settings(string lang)
        {
            var settings = await _repositoryManager.Setting.GetAllSettings(false);
            var settingDtos = _mapper.Map<List<SettingDto>>(settings);
            return settingDtos;
        }
        public async Task EditSetting(UpdateSettingDto updateSettingDto)
        {
            var settings = await _repositoryManager.Setting.GetAllSettings(true);
            foreach(var setting in settings)
            {
                _mapper.Map(updateSettingDto, setting);
            }
            await _repositoryManager.SaveAsync();
        }
    }
}
