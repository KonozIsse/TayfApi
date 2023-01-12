using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Exception;
using Entities.Models;
using Zone = Entities.Models.Zone;
using BussnessResultModel = Entities.Exception.BussnessResultModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using Entities.RequestFeatures;

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
       
        public async Task<BussnessResultModel> CreateAddress(int userId, CreateAddressDto create)
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
            return new BussnessResultModel(address, _locService.GetLocalizedStringValue("successAdd"));
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
        public async Task EditDefultAddress(int defaultAddressId, int userId)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true);
            if (user != null)
            {
                user.DefaultAddressId = defaultAddressId;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task<BussnessResultModel> EditAddress(int userId ,UpdateAddressDto update)
        {
            var address = await _repositoryManager.Address.GetAddressIdByCustomerId(update.Id, userId, true);
            if(address == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            address.UserId = userId;
            _mapper.Map(update, address);
            if (update.IsDefault == true)
            {
                await AddDefultAddress(update.Id, userId);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(address, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<AddressDto> GetDefaultAddress(int customerId)
        {
            var address = await _repositoryManager.Address.GetDefaultAddressCustomer(customerId);
            if(address == null)
            {
                return null;
            }
            var addressDto = _mapper.Map<AddressDto>(address);
            return addressDto;
        } 
        public async Task<AddressDto> GetAddressCustomer (int customerId)
        {
            var address = await _repositoryManager.Address.GetAddressCustomer(customerId);
            if(address == null)
            {
                address =  null;
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
        public async Task<AddressDto> GetAddressIdCustomerId (int id ,int customerId)
        {
            var addresses = await _repositoryManager.Address.GetAddressIdByCustomerId(id, customerId, false);
            if (addresses == null)
            {
                return null;
            }
            var addressDto = _mapper.Map<AddressDto>(addresses);
            return addressDto;
        }
        //Country------------------------------------------------

        public async Task<BussnessResultModel> AddCountry(CreateCountryDto create)
        {
            
            var IsExists = _repositoryManager.Country.ExistCountry(create.CountryName, create.MobileCode);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"),false); 
            }
            var country = _mapper.Map<Country>(create);
            if (create.CountryName == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            if (create.ImageId == 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            _repositoryManager.Country.AddCountry(country);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(country, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> DeleteCountry(int id)
        {
            var country = await _repositoryManager.Country.GetcountryById(id, false);
            if (country == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"),false);
            }
            var zones = await _repositoryManager.Zone.GetZonesByCountryId(id);
            if (zones != null)
            {
                foreach (var zone in zones)
                {
                    _repositoryManager.Zone.DeleteZone(zone);
                }
            }
            _repositoryManager.Country.DeleteCountry(country);

            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(country, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> EditCountry(UpdateCountryDto updateDto)
        {
            var country = await _repositoryManager.Country.GetcountryById(updateDto.Id, true);
            if(country == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            if (updateDto.CountryNameAr == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            _mapper.Map(updateDto, country);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(country, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<Country> GetCountryCode(string code)
        {
            return await _repositoryManager.Country.GetCountryByCode(code, false);
        }
        public async Task<PagedList<CountryDto>> GetAllCountries(string lang, string search, PostsParameters postsParameters)
        {
            var countries = await _repositoryManager.Country.GetAllCountries(search);
            //var countriesDto = _mapper.Map<List<CountryDto>>(countries);
            var countriesDto = new List<CountryDto>();
            foreach (var category in countries)
            {
                countriesDto.Add(new CountryDto
                {
                    Id = category.Id,
                    CountryName = lang == "en" ? category.CountryName : category.CountryNameAr,
                    IsStatus = category.IsStatus,
                    CountryCode3 = category.CountryCode3
                });
            }
            return PagedList<CountryDto>.ToPagedList(countriesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<CountryDto>> GetCountriesForWeb(string lang = "en")
        {
            var countries = await _repositoryManager.Country.GetCountries();
            var countriesDto = _mapper.Map<List<CountryDto>>(countries);

            //var country = countries.First();
            //var countryDto = countriesDto.First();
            //countryDto.ImageId = Convert.ToInt32(await _imageBL.GetImageOriginal(country.ImgId.ToString()));
            //countryDto.CountryName = lang == "en" ? country.CountryName : country.CountryNameAr;
            return countriesDto;
        } 
        public async Task<CountryDto> GetCountry(int id, string lang = "en")
        {
            var country = await _repositoryManager.Country.GetcountryById(id , false);
            var countryDto = _mapper.Map<CountryDto>(country);
            countryDto.CountryName = lang == "en" ? country.CountryName : country.CountryNameAr;
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
        public async Task<List<ZoneDto>> GetZonesByCountryId(int countryId, string lang = "en")
        {
            var zones = await _repositoryManager.Zone.GetZonesByCountryId(countryId);
            var zonesDto = _mapper.Map<List<ZoneDto>>(zones);
            var zone = zones.First();
            var zoneDto = zonesDto.First();
            zoneDto.ZoneName = lang == "en" ? zone.ZoneName : zone.ZoneNameAr;
            return zonesDto;
        }
        public async Task<BussnessResultModel> AddZone(CreateZoneDto create)
        {
            var IsExists = _repositoryManager.Zone.ExistZone(create.ZoneName, create.ZoneCode);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false); 
            }

            var zone = _mapper.Map<Zone>(create);
            if (String.IsNullOrEmpty(create.ZoneName) || create.ZoneName.Contains("    "))
            {
                return new BussnessResultModel(zone, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            _repositoryManager.Zone.AddZone(create.CountryId, zone);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(zone, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> DeleteZone(int id)
        {
            var zone = await _repositoryManager.Zone.GetZoneId(id, false);
            if (zone == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _repositoryManager.Zone.DeleteZone(zone);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(zone, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> EditZone (UpdateZoneDto updateDto)
        {
            var zone = await _repositoryManager.Zone.GetZoneId(updateDto.Id, true);
            if (zone == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            if (String.IsNullOrEmpty(updateDto.ZoneName) || updateDto.ZoneName.Contains("    "))
            {
                return new BussnessResultModel(zone, _locService.GetLocalizedStringValue("enterallfiled"), false); 
            }
            _mapper.Map(updateDto, zone);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(zone, _locService.GetLocalizedStringValue("successSave"));
        }
        //TaxClass------------------------------------------------
        public bool CheckTaxesExist(string name)
        {
            return _repositoryManager.TaxClass.ExistTax(name);
        }
        public async Task<List<TaxClassDto>> GetTaxes(string lang = "en")
        {
            var taxes = await _repositoryManager.TaxClass.GetTaxClasses();
            var taxesDto = _mapper.Map<List<TaxClassDto>>(taxes);
            var taxe = taxes.First();
            var taxeDto = taxesDto.First();
            taxeDto.Title = lang == "en" ? taxe.Title : taxe.TitleAr;
            taxeDto.Description = lang == "en" ? taxe.Description : taxe.DescriptionAr;
            return taxesDto;
        }
        public async Task AddTaxClass(int storeId ,CreateTaxClassDto createTaxClassDto)
        {
            var taxClass = _mapper.Map<TaxClass>(createTaxClassDto);
            taxClass.StoreId = storeId == 0 ? null : storeId;
            _repositoryManager.TaxClass.AddTaxClass(taxClass);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditTaxClass(int storeId, UpdateTaxClassDto updateDto)
        {
            var taxClass = await _repositoryManager.TaxClass.GetTaxClassId(updateDto.Id, true);
            taxClass.StoreId = storeId == 0 ? null : storeId;
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
        public async Task AddTaxRate(int storeId, CreateTaxRateDto createTaxRateDto)
        {
            var taxRate = _mapper.Map<TaxRate>(createTaxRateDto);
            taxRate.StoreId = storeId == 0 ? null : storeId;
            _repositoryManager.TaxRate.AddTaxRate(taxRate);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditTaxRate(int storeId, UpdateTaxRateDto updateDto)
        {
            var taxRate = await _repositoryManager.TaxRate.GetTaxRateId(updateDto.Id, true);
            if (taxRate != null)
            {
                taxRate.StoreId = storeId == 0 ? null : storeId;
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
        public async Task EditSetting(UpdateSettingDto updateSettingDto, int storeId = 0, int admin = 0)
        {
            var settings = await _repositoryManager.Setting.GetAllSettings(true);
            foreach(var setting in settings)
            {
                setting.VendorId = storeId == 0 ? null : storeId;
                setting.AdminId = admin == 0 ? null : admin;
                _mapper.Map(updateSettingDto, setting);
            }
            await _repositoryManager.SaveAsync();
        }
    }
}
