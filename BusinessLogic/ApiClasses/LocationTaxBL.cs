using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Exception;
using Entities.Models;
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
        public async Task<BussnessResultModel> DeleteAddress(int id)
        {
            var address = await _repositoryManager.Address.GetAddress(id, false);
            if (address == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.Address.DeleteAddress(address);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(address, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> DeleteAddressCustomer(int id, int customerId)
        {
            var address = await _repositoryManager.Address.GetAddressIdByCustomerId(id, customerId, false);
            if (address == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            var user = await _repositoryManager.User.GetUserId(customerId, true);
            if (user != null && address.IsDefault == true)
            {
                user.DefaultAddressId = 0;
            }
            _repositoryManager.Address.DeleteAddress(address);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(address, _locService.GetLocalizedStringValue("successDelete"));
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
        public async Task<BussnessResultModel> EditAddress(int userId, UpdateAddressDto update)
        {
            var address = await _repositoryManager.Address.GetAddressIdByCustomerId(update.Id, userId, true);
            if (address == null)
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
            if (address == null)
            {
                return null;
            }
            var addressDto = _mapper.Map<AddressDto>(address);
            return addressDto;
        }
        public async Task<AddressDto> GetAddressCustomer(int customerId)
        {
            var address = await _repositoryManager.Address.GetAddressCustomer(customerId);
            if (address == null)
            {
                address = null;
            }
            var addressDto = _mapper.Map<AddressDto>(address);
            return addressDto;
        }
        public async Task<PagedList<AddressDto>> GetAddressesCustomerId(int customerId, PostsParameters postsParameters)
        {
            var addresses = await _repositoryManager.Address.GetAllAddressesByCustomerId(customerId);
            //var addressDto = _mapper.Map<List<AddressDto>>(addresses);
            var addressDto = new List<AddressDto>();
            foreach (var address in addresses)
            {
                addressDto.Add(new AddressDto
                {
                    Id = address.Id,
                    CustomerName = address.User.FullName,
                    AddressTitle = address.AddressTitle,
                    Address1 = address.Address1,
                    CityName = address.CityName,
                    Street = address.Street,
                });
            }
            return PagedList<AddressDto>.ToPagedList(addressDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<AddressDto>> GetAddressesCustomer(int customerId)
        {
            var addresses = await _repositoryManager.Address.GetAllAddressesByCustomerId(customerId);
            //var addressDto = _mapper.Map<List<AddressDto>>(addresses);
            var addressDto = new List<AddressDto>();
            foreach (var address in addresses)
            {
                addressDto.Add(new AddressDto
                {
                    Id = address.Id,
                    CustomerName = address.User.FullName,
                    AddressTitle = address.AddressTitle,
                    Address1 = address.Address1,
                    CityName = address.CityName,
                    Street = address.Street,
                });
            }
            return addressDto;
        }
        public async Task<AddressDto> GetAddressIdCustomerId(int id, int customerId)
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
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
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
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
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
            if (country == null)
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
        public async Task<PagedList<CountryDto>> GetAllCountries(string lang, string search, PostsParameters postsParameters)
        {
            var countries = await _repositoryManager.Country.GetAllCountries(search);
            var countriesDto = countries.Select(c=>
            {
                var countryDto = _mapper.Map<CountryDto>(c);
                countryDto.CountryName = lang == "en" ? c.CountryName : c.CountryNameAr;
                return countryDto;
            }).ToList();
            return PagedList<CountryDto>.ToPagedList(countriesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<CountryDto>> GetAllCountries()
        {
            var countries = await _repositoryManager.Country.GetCountries();
            var countriesDto = _mapper.Map<List<CountryDto>>(countries);
            return countriesDto;
        }
        public async Task<CountryDto> GetCountry(int id, string lang)
        {
            var country = await _repositoryManager.Country.GetcountryById(id, false);
            var countryDto = _mapper.Map<CountryDto>(country);
            countryDto.CountryName = lang == "en" ? country.CountryName : country.CountryNameAr;
            return countryDto;
        }
        //Zone------------------------------------------------
        public async Task<PagedList<ZoneDto>> GetAllZones(string search, string lang, PostsParameters postsParameters)
        {
            var zones = await _repositoryManager.Zone.GetAllZones(search);
            var zonesDto = zones.Select(item => new ZoneDto
            {
                Id = item.Id,
                ZoneName = lang == "en" ? item.ZoneName : item.ZoneNameAr,
                ZoneCode = item.ZoneCode,
                CountryName = lang == "en" ? item.Country.CountryName : item.Country.CountryNameAr,
            }).ToList();

            return PagedList<ZoneDto>.ToPagedList(zonesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<ZoneDto>> GetZonesByCountryId(int countryId, string lang)
        {
            var zones = await _repositoryManager.Zone.GetZonesByCountryId(countryId);
            var zonesDto = new List<ZoneDto>();
            foreach (var item in zones)
            {
                zonesDto.Add(new ZoneDto
                {
                    Id = item.Id,
                    ZoneName = lang == "en" ? item.ZoneName : item.ZoneNameAr,
                    ZoneCode = item.ZoneCode,
                    CountryName = lang == "en" ? item.Country.CountryName : item.Country.CountryNameAr,
                });
            }
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
            if (String.IsNullOrEmpty(create.ZoneName) || create.ZoneName.Contains(" "))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
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
        public async Task<BussnessResultModel> EditZone(UpdateZoneDto updateDto)
        {
            var zone = await _repositoryManager.Zone.GetZoneId(updateDto.Id, true);
            if (zone == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            if (String.IsNullOrEmpty(updateDto.ZoneName) || updateDto.ZoneName.Contains("    "))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            _mapper.Map(updateDto, zone);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(zone, _locService.GetLocalizedStringValue("successSave"));
        }
        //TaxClass------------------------------------------------

        public async Task<PagedList<TaxClassDto>> GetTaxes(string search, string lang, PostsParameters postsParameters)
        {
            var taxes = await _repositoryManager.TaxClass.GetTaxClasses(search);
            var taxesDto = taxes.Select(item => new TaxClassDto
            {
                Id = item.Id,
                Title = lang == "en" ? item.Title : item.TitleAr,
                Description = lang == "en" ? item.Description : item.DescriptionAr,
                CreateAt = item.CreatedAt
            });

            return PagedList<TaxClassDto>.ToPagedList(taxesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> AddTaxClass(int storeId, CreateTaxClassDto create)
        {
            if (String.IsNullOrEmpty(create.Title) || (create.Title.Contains("")))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            var IsExists = _repositoryManager.TaxClass.ExistTax(create.Title);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
            }
            var taxClass = _mapper.Map<TaxClass>(create);
            taxClass.StoreId = storeId == 0 ? null : storeId;
            _repositoryManager.TaxClass.AddTaxClass(taxClass);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(taxClass, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> EditTaxClass(int storeId, UpdateTaxClassDto updateDto)
        {
            if (String.IsNullOrEmpty(updateDto.Title) || (updateDto.Title.Contains("")))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
            }
            var taxClass = await _repositoryManager.TaxClass.GetTaxClassId(updateDto.Id, true);
            if (taxClass == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            taxClass.StoreId = storeId == 0 ? null : storeId;
            _mapper.Map(updateDto, taxClass);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(taxClass, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> DeleteTaxClass(int id)
        {
            var taxClass = await _repositoryManager.TaxClass.GetTaxClassId(id, false);
            if (taxClass == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.TaxClass.DeleteTaxClass(taxClass);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(taxClass, _locService.GetLocalizedStringValue("successDelete"));
        }
        //TaxRate------------------------------------------------
        public async Task<PagedList<TaxRateDto>> GetTaxeRates(string seach, string lang, PostsParameters postsParameters)
        {
            var taxes = await _repositoryManager.TaxRate.GetTaxRates(seach);
            var taxesDto = taxes.Select(item => new TaxRateDto
            {
                Id = item.Id,
                Tax_Rate = item.Tax_Rate,
                ZoneName = lang == "en" ? item.Zone.ZoneName : item.Zone.ZoneNameAr,
                TaxClassTitle = lang == "en" ? item.TaxClass.Title : item.TaxClass.TitleAr,
                CreatedAt = item.CreatedAt
            });

            return PagedList<TaxRateDto>.ToPagedList(taxesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> AddTaxRate(int storeId, CreateTaxRateDto create)
        {
            var IsExists = _repositoryManager.TaxRate.ExistTaxRates(create.ZoneId);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);

            }
            var taxRate = _mapper.Map<TaxRate>(create);
            taxRate.StoreId = storeId == 0 ? null : storeId;
            _repositoryManager.TaxRate.AddTaxRate(taxRate);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(taxRate, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> EditTaxRate(int storeId, UpdateTaxRateDto updateDto)
        {
            var taxRate = await _repositoryManager.TaxRate.GetTaxRateId(updateDto.Id, true);
            if (taxRate != null)
            {
                taxRate.StoreId = storeId == 0 ? null : storeId;
                _mapper.Map(updateDto, taxRate);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(taxRate, _locService.GetLocalizedStringValue("successSave"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"), false);
            }
        }
        public async Task<BussnessResultModel> DeleteTaxRate(int id)
        {
            var taxRate = await _repositoryManager.TaxRate.GetTaxRateId(id, false);
            if (taxRate != null)
            {
                _repositoryManager.TaxRate.DeleteTaxRate(taxRate);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(taxRate, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
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
            if (customer != null && customer.DefaultAddressId != null)
            {
                var defaultAddress = await _repositoryManager.Address.GetAddressIdByCustomerId(customer.DefaultAddressId.Value, customerId, false);
                if (defaultAddress != null)
                {
                    tax = await CalculateTaxToZoneId(defaultAddress.ZoneId);
                }
            }
            return tax;
        }
    }
}
