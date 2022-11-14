using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
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
        public async Task<List<AddressDto>> GetAddresses(int userId)
        {
            var addresses = await _repositoryManager.Address.GetAllAddressesByCustomerId(userId);
            var addressesDto = _mapper.Map<List<AddressDto>>(addresses);
            var addressDto = addressesDto.First();
            foreach (var address in addresses)
            {
                var defaultAddress = await _repositoryManager.User.GetUserDefaultAddress(userId, address.Id);
                if (defaultAddress != null)
                {
                    addressDto.IsDefault = true;
                }
                else
                {
                    addressDto.IsDefault = false;
                }
            }
            return addressesDto;
        }
        public async Task CreateAddress(int userId, CreateAddressDto createAddressDto)
        {
            var address = _mapper.Map<Address>(createAddressDto);
            address.UserId = userId;
            _repositoryManager.Address.AddAddress(address);
            if (address.User.DefaultAddressId == address.Id)
            {
                await AddDefultAddress(address.Id, userId);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteAddressById(int id, int customerId)
        {
            var address = await _repositoryManager.Address.GetAddressIdByCustomerId(id, customerId, false);
            _repositoryManager.Address.DeleteAddress(address);
            await _repositoryManager.SaveAsync();
        }
        public async Task AddDefultAddress(int defaultAddressId, int userId)
        {
            var user = await _repositoryManager.User.GetUserId(userId, false);
            if (user != null)
            {
                user.DefaultAddressId = defaultAddressId;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task EditAddres(UpdateAddressDto updateAddressDto, int id, bool isDefault)
        {
            var address = await _repositoryManager.Address.GetAddressIdByCustomerId(id, updateAddressDto.UserId, true);
            address.UserId = updateAddressDto.UserId;
            _mapper.Map(updateAddressDto, address);

            if (isDefault == true)
            {
                await AddDefultAddress(id, updateAddressDto.UserId);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<AddressDto> DefaultAddress(int customerId)
        {
            var address = await _repositoryManager.Address.GetAddressCustomer(customerId);
            var addressDto = new AddressDto();
            if (address != null)
            {
                addressDto = _mapper.Map<AddressDto>(address);
                addressDto.UserId = customerId;
                addressDto.CityName = (address.Country == null ? "" :
                        (address.Country.Zones.Where(r => r.Id == address.ZoneId).FirstOrDefault() == null ? "" :
                        address.Country.Zones.Where(r => r.Id == address.ZoneId).FirstOrDefault().ZoneName));
                addressDto.IsDefault = await _repositoryManager.User.GetUserDefaultAddress(customerId, address.Id) != null;
            }
            else
            {
                addressDto = null;
            }
            return addressDto;
        }
        //Country------------------------------------------------
        public async Task<List<CountryDto>> GetCountries()
        {
            var countries = await _repositoryManager.Country.GetCountries();
            // var iso = _imgCatRepo.getImgOrg(x1.image.ToString());
            var countryDto = _mapper.Map<List<CountryDto>>(countries);
            return countryDto;
        }
        public async Task DeleteCountry(int id)
        {
            var country = await _repositoryManager.Country.GetcountryById(id, false);
            _repositoryManager.Country.DeleteCountry(country);
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
            var country = countries.First();
            var countriesDto = _mapper.Map<List<CountryDto>>(countries);
            var countryDto = countriesDto.First();
            countryDto.ImageId = Convert.ToInt32(await _imageBL.GetImageOriginal(country.ImgId.ToString()));
            return countriesDto;
        }
        //Zone------------------------------------------------
        public async Task<List<Zone>> GetZones()
        {
            return await _repositoryManager.Zone.GetAllZones();
        }
        public async Task<List<ZoneDto>> GetZonesByCountryId(int countryId)
        {
            var zones = await _repositoryManager.Zone.GetZonesByCountryId(countryId);
            var zonesDto = _mapper.Map<List<ZoneDto>>(zones);
            return zonesDto;
        }
        public async Task AddZone(int countryId, CreateZoneDto createZoneDto)
        {
            var zone = _mapper.Map<Zone>(createZoneDto);
            _repositoryManager.Zone.AddZone(countryId, zone);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteZone(int id, int countryId)
        {
            var zone = await _repositoryManager.Zone.GetZoneIdCountryId(id, countryId, false);
            _repositoryManager.Zone.DeleteZone(zone);
            await _repositoryManager.SaveAsync();
        }
        //TaxRate------------------------------------------------
        public async Task AddTaxRate(CreateTaxRateDto createTaxRateDto)
        {
            var taxRate = _mapper.Map<TaxRate>(createTaxRateDto);
            _repositoryManager.TaxRate.AddTaxRate(taxRate);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditTaxRate(int id, UpdateTaxRateDto updateTaxRateDto)
        {
            var taxRate = await _repositoryManager.TaxRate.GetTaxRateId(id, true);
            _mapper.Map(updateTaxRateDto, taxRate);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteTaxRate(int id)
        {
            var taxRate = await _repositoryManager.TaxRate.GetTaxRateId(id, false);
            _repositoryManager.TaxRate.DeleteTaxRate(taxRate);
            await _repositoryManager.SaveAsync();
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
       
        //TaxClass------------------------------------------------
        public async Task AddTaxClass (CreateTaxClassDto createTaxClassDto)
        {
            var taxClass = _mapper.Map<TaxClass>(createTaxClassDto);
            _repositoryManager.TaxClass.AddTaxClass(taxClass);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditTaxClass( int id , UpdateTaxClassDto updateTaxClassDto)
        {
            var taxClass = await _repositoryManager.TaxClass.GetTaxClassId(id , true);
            _mapper.Map(updateTaxClassDto , taxClass);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteTaxClass(int id)
        {
            var taxClass = await _repositoryManager.TaxClass.GetTaxClassId(id, false);
            _repositoryManager.TaxClass.DeleteTaxClass(taxClass);
            await _repositoryManager.SaveAsync();
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
