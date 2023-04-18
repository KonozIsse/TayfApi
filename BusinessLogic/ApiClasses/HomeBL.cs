using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Entities.ViewModel;
using AutoMapper;
using Contracts;
using Entities.Exception;
using Entities.RequestFeatures;
using System.Reflection;
using Newtonsoft.Json;

namespace BusinessLogic.ApiClasses
{
    public class HomeBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        protected readonly NewsBL _newsBL;
        private readonly LocService _locService;
        private readonly ImageBL _imageBL; 
        private readonly ProductBL _productBL;
        private readonly UserBL _userBL;
        public HomeBL(IRepositoryManager repositoryManager, IMapper mapper, NewsBL newsBL, LocService locService, ImageBL imageBL, ProductBL productBL, UserBL userBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _newsBL = newsBL;
            _locService = locService;
            _imageBL = imageBL;
            _productBL = productBL;
            _userBL = userBL;
        }
        public async Task<HomeVM> GetHome(int customerId, int currencyId, string lang )
        {
            var languge = await _repositoryManager.Language.GetCodeLanguage(lang, false);
            var model = new HomeVM
            {
                Sliders = await GetAllSliders(lang),
                Services = await GetAllServices(lang),
                Banner = await GetBanner(languge.Id),
                Blogs = await _newsBL.GetNews(lang)??null,
                ProductsPopular = await _productBL.PopularsPage(),
                ProductsBest = await _productBL.BestPage(),
                ProductsLatest = await _productBL.LatestPage(),
                ProductsSpecial = await _productBL.SpecialsPage(),
                ProductsTopRated = await _productBL.TopRatedPage(),
                ProductsDailyDeal = await _productBL.DailyDeals(),
                Products = await _productBL.GetAllActiveAcceptProducts(customerId, lang),
                FlashProducts = await _productBL.GetFlashProds(customerId,lang),
                SpecialProducts = await _productBL.GetSpecialsProd(customerId, lang),
                Stores = await _userBL.GetStores()
            };
            return model;
        }
        public async Task<HomeCPVM> GetHomeCP(int userId)
        {
            var orders = await _repositoryManager.Order.GetOrders(false); 
            var panding = await _repositoryManager.Order.GetAllPandingOrders(false);
            var cansal = await _repositoryManager.Order.GetAllCansalOrders(false);
            var complete = await _repositoryManager.Order.GetAllCompleteOrders(false);
            var products = await _repositoryManager.Product.GetAllProducts();
            var stocks = await _repositoryManager.Inventory.GetAllOutStock();
            var ordersTransaction = orders.Where(c => c.Transaction != null);
            var countCustomers = await _repositoryManager.User.GetCustomers(false);
            var carts = await _repositoryManager.Cart.GetAllCarts(false);

            var store = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                orders = orders.Where(c => c.StoreId == userId).ToList();
                panding = panding.Where(c => c.StoreId == userId).ToList();
                cansal = cansal.Where(c => c.StoreId == userId).ToList();
                complete = complete.Where(c => c.StoreId == userId).ToList();
                products = products.Where(c => c.StoreId == userId).ToList();
                stocks = stocks.Where(c => c.VendorId == userId).ToList();
                carts = carts.Where(c => c.StoreId == userId).ToList();
            }

            var model = new HomeCPVM
            {
                TotalOrders = orders.Count() ,
                TotalProducts = products.Count(),
                TotalOutStock = stocks.Count(),
                TotalPurchased = ordersTransaction.Sum(c => c.OrderPrice) == 0 ? 0 : ordersTransaction.Sum(c => c.OrderPrice),
                TotalTransactions = orders.Sum(c => c.OrderPrice) == 0?0: orders.Sum(c => c.OrderPrice),
                TotalCustomerRegistrations = countCustomers.Count(),
                PendingOrders = panding.Count(),
                PendingPercentage = double.IsNaN(Math.Round((((double)panding.Count() / orders.Count()) * 100), 2)) ? 0 : Math.Round((((double)panding.Count() / orders.Count()) * 100), 2),
                CansalOrders = cansal.Count(),
                CansalPercentage = double.IsNaN(Math.Round((((double)cansal.Count() / orders.Count()) * 100), 2)) ? 0 : Math.Round((((double)cansal.Count() / orders.Count()) * 100), 2),
                CompleteOrders = complete.Count(),
                CompletePercentage = double.IsNaN(Math.Round((((double)complete.Count() / orders.Count()) * 100), 2)) ? 0 : Math.Round((((double)complete.Count() / orders.Count()) * 100), 2),
                TotalCarts = carts.Count(),
                CartsPercentage = double.IsNaN(Math.Round((((double)carts.Count() / products.Count()) * 100), 2)) ? 0: Math.Round((((double)carts.Count() / products.Count()) * 100), 2)
            };
          
            return model;
        }
        public async Task<AdminDto> GetCurrentUser(int id)
        {
            var user = await _repositoryManager.User.GetUserId(id, false);
            var userDto = _mapper.Map<AdminDto>(user);
            
            return userDto;
        }
        public async Task<List<CustomerDto>> GetNewCustomers()
        {
            var customers = await _repositoryManager.User.GetCustomers(false);
            var customersDto = _mapper.Map<List<CustomerDto>>(customers);
            return customersDto.Take(4).ToList();
        }
        public async Task<List<ProductDto>> GetRecentProducts(int storeId, string lang)
        {
            var products = await _repositoryManager.Product.GetProductsCP("",null);
            var store = await _repositoryManager.User.GetUserId(storeId, false);
            if (store.UserType == UserType.Store)
            {
                products = products.Where(c => c.StoreId == storeId).ToList();
            }
            var productsDto = products.Select(c =>
            {
                var productDto = _mapper.Map<ProductDto>(c);
                productDto.ProductName = lang == "en" ? c.ProductName : c.ProductNameAr;
                productDto.ImageProduct = _imageBL.GetImageOriginal(c.Images.First().ImageId);
                return productDto;
            }).Take(15).ToList();
            return productsDto;
        }
        public async Task<List<OrderDto>> GetOrders(int storeId)
        {
            var orders = await _repositoryManager.Order.GetOrders(false);
            var user = await _repositoryManager.User.GetUserId(storeId, false);
            if (user.UserType == UserType.Store)
            {
                orders = orders.Where(c => c.StoreId == storeId).ToList();
            }
            var productsDto = orders.Select(order=>new OrderDto
            {
                Id = order.Id,
                OrderPrice = Convert.ToDecimal(order.OrderPrice),
                CustomerName = order.Customer.FullName,
                OrderStatusName = order.OrderStatus.StatusName,
                OrderStatusEnum = order.OrderStatus.OrderStatusEnum
            }).Take(15).ToList();
            return productsDto;
        }
        public async Task<List<StoreDto>> GetStores()
        {
            var stores = await _repositoryManager.User.GetAllStores(false);
            var storesDto = stores.Select(store =>
            {
                var bookDto = _mapper.Map<StoreDto>(store);
                bookDto.Image = _imageBL.GetImageMedium(Convert.ToInt32(store.ImageId));
                bookDto.CreatedAt = store.CreatedAt.ToString("G");
                return bookDto;
            }).ToList();
            return storesDto;
        }
        public async Task<string> GetLogo()
        {
            var logo = await _repositoryManager.Setting.GetSettingByValue("website_logo",false);
            return _imageBL.GetImageOriginal(Convert.ToInt32(logo.Value));
        }
        //Banner------------------------------------------------
        public async Task<BannerDto> GetBanner(int langId)
        {
            var banner = await _repositoryManager.Banner.GetBannerByType(langId,"category", false);
            var bannerDto = _mapper.Map<BannerDto>(banner);
            bannerDto.LangName = banner.Language.Code == "en" ? banner.Language.Name : banner.Language.NameAr;
            bannerDto.Img = _imageBL.GetImageOriginal(banner.ImgId);
            return bannerDto;
        }
        public async Task<PagedList<BannerDto>> GetBanners(string search,string filter, PostsParameters postsParameters)
        {
            var banners = await _repositoryManager.Banner.GetAllBanner(search,filter, false);
            var bannersDto = banners.Select(c =>
            {
                var bannerDto = _mapper.Map<BannerDto>(c);
                bannerDto.LangName = c.Language.Name == "English" ? _locService.GetLocalizedStringValue("English") : _locService.GetLocalizedStringValue("Arabic");
                bannerDto.Img = _imageBL.GetImageOriginal(c.ImgId);
                bannerDto.CreatedAt = c.CreatedAt.ToString("G");
                return bannerDto;
            }).ToList();
            return PagedList<BannerDto>.ToPagedList(bannersDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> UpdateBanner(UpdateBannerDto updateDto)
        {
            var banner = await _repositoryManager.Banner.GetBannerId(updateDto.Id, true);
            if (banner == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _mapper.Map(updateDto, banner);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(banner, _locService.GetLocalizedStringValue("successSave"));
        }
        //Sliders------------------------------------------------
        public async Task<PagedList<SliderDto>> GetSliderMobile(string lang,string search, PostsParameters postsParameters)
        {
            var sliders = await _repositoryManager.Slider.GetSlidersForMobile(search);
            var slidersDto = sliders.Select(c => 
            {
                 var sliderDto  = _mapper.Map<SliderDto>(c);
                sliderDto.Title = lang == "en" ? c.Title : c.TitleAr;
                sliderDto.Decription = lang == "en" ? c.Decription : c.DecriptionAr;
                sliderDto.Image = _imageBL.GetImageMedium(c.ImgId);
                sliderDto.CreatedAt = c.CreatedAt.ToString("G");
                sliderDto.UpdatedAt = c.UpdatedAt == null ? null : c.UpdatedAt.Value.ToString("G");
                return sliderDto;
                }).ToList();
            return PagedList<SliderDto>.ToPagedList(slidersDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<PagedList<SliderDto>> GetSliderWeb(string search, string filter, string lang, PostsParameters postsParameters)
        {
            var sliders = await _repositoryManager.Slider.GetSlidersForWeb(search, filter);
                var slidersDto = sliders.Select(c => 
                {
                   var sliderDto  = _mapper.Map<SliderDto>(c);
                    sliderDto.Title = lang == "en" ? c.Title : c.TitleAr;
                    sliderDto.Decription = lang == "en" ? c.Decription : c.DecriptionAr;
                    sliderDto.Image = _imageBL.GetImageOriginal(c.ImgId);
                    sliderDto.CreatedAt = c.CreatedAt.ToString("G");
                    sliderDto.UpdatedAt = c.UpdatedAt == null ? null : c.UpdatedAt.Value.ToString("G");
                    return sliderDto;
                }).ToList();
            return PagedList<SliderDto>.ToPagedList(slidersDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<SliderDto>> GetAllSliders(string lang)
        {
            var sliders = await _repositoryManager.Slider.GetSliders();
               var slidersDto =  sliders.Select(c =>
                {
                   var sliderDto = _mapper.Map<SliderDto>(c);
                    sliderDto.Title = lang == "en" ? c.Title : c.TitleAr;
                    sliderDto.Decription = lang == "en" ? c.Decription : c.DecriptionAr;
                    sliderDto.Image = _imageBL.GetImageOriginal(c.ImgId);
                    sliderDto.CreatedAt = c.CreatedAt.ToString("G");
                    sliderDto.UpdatedAt = c.UpdatedAt == null ? null : c.UpdatedAt.Value.ToString("G");
                    return sliderDto;
                }).ToList();
            return slidersDto;
        }
        public async Task<BussnessResultModel> AddSliderWeb(int storeId, CreateSliderDto createSliderDto)
        {
            var slider = _mapper.Map<Sliders>(createSliderDto);
            slider.Type = SlidersImageType.Web;
            var store = await _repositoryManager.User.GetActiveUserId(storeId, false);
            if (store.UserType == UserType.Store)
            {
                slider.VendorId = storeId;
            }
            if (createSliderDto.ImageId == 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            slider.ImgId = createSliderDto.ImageId;
            _repositoryManager.Slider.AddSlider(slider);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(slider, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> AddSliderMobile(int storeId, CreateSliderDto createSliderDto)
        {
            var slider = _mapper.Map<Sliders>(createSliderDto);
            if (createSliderDto.ImageId == 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            slider.ImgId = createSliderDto.ImageId;
            slider.LanguageId = createSliderDto.LangId;
            slider.Type = SlidersImageType.Mobile;
            slider.VendorId = storeId;
            _repositoryManager.Slider.AddSlider(slider);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(slider, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> UpdateSlider(int storeId, UpdateSliderDto updateDto)
        {
            var slider = await _repositoryManager.Slider.GetSlideById(updateDto.Id, true);
            if (slider == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            slider.VendorId = storeId;
            slider.ImgId = updateDto.ImageId;
            _mapper.Map(updateDto, slider);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(slider, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> DeleteSlide(int id)
        {
            var slider = await _repositoryManager.Slider.GetSlideById(id, false);
            if (slider == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.Slider.DeleteSlider(slider);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(slider, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<UpdateSliderDto> GetSliderId(int id)
        {
            var slider = await _repositoryManager.Slider.GetSlideById(id, false);
            var sliderDto = _mapper.Map<UpdateSliderDto>(slider);
            
            var image = _repositoryManager.Image.GetImageId(slider.ImgId);
            sliderDto.ImageId = slider.ImgId;
            sliderDto.Image  = $"/media_files/medium/{image.Name}";
            return sliderDto;
        }
        //Services------------------------------------------------
        public async Task<List<ServiceDto>> GetAllServices(string lang)
        {
            var services = await _repositoryManager.Services.GetAllServices("", false);
               var servicesDto = services.Select(c => new ServiceDto
                {
                    Title = lang == "en" ? c.Title : c.TitleAr,
                    Description = lang == "en" ? c.Description : c.DescriptionAr,
                    Image = _imageBL.GetImageOriginal(c.ImgId.Value),
                    CreatedAt = c.CreatedAt.ToString("G"),
                    UpdatedAt = c.UpdatedAt == null ? null : c.UpdatedAt.Value.ToString("G"),
                }).ToList();
            return servicesDto;
        } 
        public async Task<PagedList<ServiceDto>> GetServices(string search, string lang, PostsParameters postsParameters)
        {
            var services = await _repositoryManager.Services.GetAllServices(search, false);
                 var servicesDto = services.Select(c => new ServiceDto
                 {
                    Title = lang == "en" ? c.Title : c.TitleAr,
                    Description = lang == "en" ? c.Description : c.DescriptionAr,
                    Image = _imageBL.GetImageOriginal(c.ImgId.Value),
                    CreatedAt = c.CreatedAt.ToString("G"),
                    UpdatedAt = c.UpdatedAt == null ? null : c.UpdatedAt.Value.ToString("G"),
                }).ToList();
            return PagedList<ServiceDto>.ToPagedList(servicesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> UpdateService(UpdateServiceDto update)
        {
            var service = await _repositoryManager.Services.GetServiceById(update.Id, true, false);
            if (service == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _mapper.Map(update, service);
            await _repositoryManager.SaveAsync();

            return new BussnessResultModel(service, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> DeleteService(int id)
        {
            var service = await _repositoryManager.Services.GetServiceById(id, false, false);
            if (service == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }

            _repositoryManager.Services.DeleteService(service);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(service, _locService.GetLocalizedStringValue("successDelete"));
        }
        //Contact------------------------------------------------
        public async Task<ContactVM> GetContactSetting()
        {
            var setting = await _repositoryManager.Setting.GetAllSettings(false);
            return new ContactVM
            {
                phone_no = setting.Where(r => r.Key == "phone_no").First().Value,
                address = setting.Where(r => r.Key == "address").First().Value,
                country = setting.Where(r => r.Key == "country").First().Value,
                city = setting.Where(r => r.Key == "city").First().Value,
                open_time = setting.Where(r => r.Key == "open_time").First().Value,
                close_time = setting.Where(r => r.Key == "close_time").First().Value
            };
        }
        public async Task<PagedList<ContactDto>> GetAllContacts(string search ,string filter, PostsParameters postsParameters)
        {
            var contacts = await _repositoryManager.Contact.GetContacts(search, filter);
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);
            return PagedList<ContactDto>.ToPagedList(contactsDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> AddContact(CreateContactDto createContactDto)
        {
            var contact = _mapper.Map<Contact>(createContactDto);
            contact.IsStatus = Status.NotActive;
            _repositoryManager.Contact.CreateContact(contact);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(contact, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> DeleteContact(int id)
        {
            var contact = await _repositoryManager.Contact.GetContactById(id, false);
            if (contact == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.Contact.DeleteContact(contact);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(contact, _locService.GetLocalizedStringValue("successDelete"));
        }
        //template------------------------------------------------
        public async Task<List<MessageTemplateDto>> GetAllMessageTemplates()
        {
            var templetes = await _repositoryManager.MessageTemplate.GetEmailTemplatesList(false);
            var templetsDto = templetes.Select(templete =>
            {
                var templeteDto = _mapper.Map<MessageTemplateDto>(templete);
                templeteDto.Name = templete.NameTemplate.ToString();    
                return templeteDto;
            }).ToList();
            return templetsDto;
        }
        public async Task<UpdateTemplateDto> GetTemplateId(int id)
        {
            var template = await _repositoryManager.MessageTemplate.GetTemplateById(id, false);
           var templateDto =  _mapper.Map<UpdateTemplateDto>(template);
            return templateDto;
        }
        public async Task<BussnessResultModel> UpdateTemplate(UpdateTemplateDto updateDto)
        {
            var template = await _repositoryManager.MessageTemplate.GetTemplateById(updateDto.Id, true);
            if (template == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _mapper.Map(updateDto, template);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(template, _locService.GetLocalizedStringValue("successSave"));
        }
        //Email------------------------------------------------
        public async Task<PagedList<MailListDto>> GetMailLists(string search, PostsParameters postsParameters)
        {
            var emails = await _repositoryManager.MailList.GetMailListEmail(search);
            var emailsDto = _mapper.Map<List<MailListDto>>(emails);
            return PagedList<MailListDto>.ToPagedList(emailsDto, postsParameters.PageNumber, postsParameters.PageSize);
        } 
        public async Task<List<MailListDto>> GetMailListstest(int userID)
        {
            var emails = await _repositoryManager.MailList.GetMailListEmail("");
            var emailsDto = _mapper.Map<List<MailListDto>>(emails);
            return emailsDto;
        }
        public async Task SendUserEmail(string email)
        {
            var exsitEmail = await _repositoryManager.MailList.GetEmail(email);
            if (exsitEmail == null)
            {
                var mailList = new MailList { Email = email };
                _repositoryManager.MailList.SendUserEmail(mailList);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task<BussnessResultModel> RemoveMailList(int id)
        {
            var email = await _repositoryManager.MailList.GetMailListById(id, false);
            if (email == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _repositoryManager.MailList.RemoveMailList(email);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(email, _locService.GetLocalizedStringValue("successDelete"));
        }
        //Language------------------------------------------------
        public async Task<PagedList<LanguageDto>> GetAllLanguages(string search,string filter, string lang, PostsParameters postsParameters)
        {
            var languages = await _repositoryManager.Language.GetAllLanguage(search , filter);
            var languagesDto = languages.Select(language => 
            {
                var languageDto = _mapper.Map<LanguageDto>(language);
                languageDto.Name = lang == "en" ? language.Name : language.NameAr;
                languageDto.Image = _imageBL.GetImageOriginal(language.ImgId);
                languageDto.IsDefault = language.IsStatus == Status.Active ? true : false;
                return languageDto;
            }).ToList();
            return PagedList<LanguageDto>.ToPagedList(languagesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<LanguageDto>> GetLanguages(string lang)
        {
            var languages = await _repositoryManager.Language.GetListLanguage(false);
            var languagesDto = languages.Select(language =>
            {
                var languageDto = _mapper.Map<LanguageDto>(language);
                languageDto.Name = lang == "en" ? language.Name : language.NameAr;
                return languageDto;
            }).ToList();
            return languagesDto;
        }
        public async Task<BussnessResultModel> DeleteLanguage(int id)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(id, false);
            if (language == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _repositoryManager.Language.DeleteLanguage(language);
            await _repositoryManager.SaveAsync();

            return new BussnessResultModel(language, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> UpdateLanguage(int id ,UpdateLanguageDto updateDto)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(id, true);
            if (language == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _mapper.Map(updateDto, language);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(language, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task ChangeDefaultLanugage(int id)
        {
            var languages = await _repositoryManager.Language.GetListLanguage(true);
            foreach (var item in languages)
            {
                if (item.Id == id)
                {
                    item.IsStatus = Status.Active;
                }
                else
                {
                    item.IsStatus = Status.NotActive; 
                }
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task<UpdateLanguageDto> GetLanugage(int id)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(id, false);
            var langDto = _mapper.Map<UpdateLanguageDto>(language);
            return langDto;
        }
        //Currency------------------------------------------------
        public async Task<PagedList<CurrencyDto>> GetAllCurrencies(string lang, PostsParameters postsParameters)
        {
            var currencies = await _repositoryManager.Currency.GetAllCurrencies(false);
            var currenciesDto = currencies.Select(currency => 
            {
                var currencyDto= _mapper.Map<CurrencyDto>(currency);
                currencyDto.Name = lang == "en" ? currency.Name : currency.NameAr;
                currencyDto.Position = currency.Position == "Left" ? _locService.GetLocalizedStringValue("left") : _locService.GetLocalizedStringValue("right");
                currencyDto.IsStatus = currency.IsStatus == Status.Active ? _locService.GetLocalizedStringValue("active") : _locService.GetLocalizedStringValue("notActive");
                return currencyDto;
            }).ToList();
            return PagedList<CurrencyDto>.ToPagedList(currenciesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<UpdateCurrencyDto> GetCurrency(int id)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(id,false);
            var currencyDto = _mapper.Map<UpdateCurrencyDto>(currency);
             return currencyDto;
        }
        public async Task ChangeDefaultCurrency(int id)
        {
            var currencies = await _repositoryManager.Currency.GetAllCurrencies(true);
            foreach (var item in currencies)
            {
                if (item.Id == id)
                {
                    item.IsDefault = 1;
                }
                else
                {
                    item.IsDefault = 0;
                }
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task<BussnessResultModel> AddCurrency(CreateCurrencyDto createDto)
        {
            var IsExists = _repositoryManager.Currency.ExistCurrency(createDto.Symbol);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
            }
            else
            {
                var currency = _mapper.Map<Currency>(createDto);
                currency.IsDefault = 0;
               
                _repositoryManager.Currency.AddCurrency(currency);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(currency, _locService.GetLocalizedStringValue("successAdd"));
            }
        }
        public async Task<BussnessResultModel> UpdatCurrency(UpdateCurrencyDto updateDto)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(updateDto.Id, true);
            if (currency != null)
            {
                _mapper.Map(updateDto, currency);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(currency, _locService.GetLocalizedStringValue("successAdd)"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
        }
        public async Task<BussnessResultModel> DeleteCurrency(int id)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(id, false);
            if (currency == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.Currency.DeleteCurrency(currency);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(currency, _locService.GetLocalizedStringValue("successDelete"));
        }
        //Notification------------------------------------------------
        public async Task<BussnessResultModel> CreateNotification(CreateNotificationDto create)
        {

            var users = await _repositoryManager.User.GetCustomers(false);
            var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.GeneralNotfication);
            if (create.IdUsers.First() == 0)
            {
                foreach (var u in users)
                {
                    var notification1 = _mapper.Map<Notification>(create);
                    notification1.UserId = u.Id;
                    notification1.IsRead = false;
                    notification1.NotificationActionId = action.Id;
                    notification1.Status = NotificationStatus.New;
                    _repositoryManager.Notification.CreateNotification(notification1);
                }
            }
            if (create.IdUsers.First() != 0 && users != null)
            {
                foreach (var f in create.IdUsers)
                {
                    var user = await _repositoryManager.User.GetActiveUserId(f, false); ;
                    if (user != null)
                    {
                        var notification = _mapper.Map<Notification>(create);
                        notification.UserId = f;
                        notification.IsRead = false;
                        notification.NotificationActionId = action.Id;
                        notification.Status = NotificationStatus.New;
                        _repositoryManager.Notification.CreateNotification(notification);
                    }
                }
            }
            await _repositoryManager.SaveAsync();

            return new BussnessResultModel(users, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> DeleteNotification(int id)
        {
            var notification = await _repositoryManager.Notification.FindNotificationId(id, false);
            if (notification == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.Notification.DeleteNotification(notification);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(notification, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<PagedList<NotificationDto>> GetNotifications(string lang ,PostsParameters postsParameters)
        {
            var notfis = await _repositoryManager.Notification.GetAllNotifications(false);
            var notfisDto = notfis.Select(item=>
                {
                    var dto = _mapper.Map<NotificationDto>(item);
                    dto.Name = item.User.FullName;
                    dto.Subject = lang == "en" ? item.NotificationAction.Subject : item.NotificationAction.SubjectAr;
                    dto.NotificationKey = item.NotificationAction.NotificationKey ;
                    return dto;
                }).ToList();
            return PagedList<NotificationDto>.ToPagedList(notfisDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        //Pages---------------------------------------------------------
        public async Task<PageDto> GetTypePage(PageType type, string lang)
        {
            var page = await _repositoryManager.StaticPages.GetTypePage(type, false);
            var pageDto = _mapper.Map<PageDto>(page);
            if (page.Names.ContainsKey(lang))
            {
                pageDto.Title = page.Names[lang];
            }
            if (page.Descriptions.ContainsKey(lang))
            {
                pageDto.Description = page.Descriptions[lang];
            }
            return pageDto;
        }
        public async Task<PagedList<PageDto>> GetAllPages(string search,string filter, string lang, PostsParameters postsParameters)
        {
            var pages = await _repositoryManager.StaticPages.GetAllPages(search, filter, false);
            var pagesDto = pages.Select(page => 
            {
                var pageDto = _mapper.Map<PageDto>(page);
                if (page.Names.ContainsKey(lang))
                {
                    pageDto.Title =  page.Names[lang];
                }
                if (page.Descriptions.ContainsKey(lang))
                {
                    pageDto.Description = page.Descriptions[lang];
                }
                return pageDto;
            }).ToList();
            return PagedList<PageDto>.ToPagedList(pagesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> EditPage(EditPageDto update)
        {
            var page = await _repositoryManager.StaticPages.GetPage(update.Id, true);
            if (page == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            page.Id = update.Id;
            page.Names = update.Names;
            page.Descriptions = update.Descriptions;
            //_mapper.Map(update, page);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(page, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<EditPageDto> GetPageId(int id)
        {
            var page = await _repositoryManager.StaticPages.GetPage(id, false);
            var pageDto = _mapper.Map<EditPageDto>(page);
            return pageDto;
        }
        //setting------------------------------------------------------
        public async Task<IEnumerable<SettingDto>> GetAllSettings()
        {
            var settings = await _repositoryManager.Setting.GetAllSettings(false);
            var settingsDto = _mapper.Map<List<SettingDto>>(settings);
            return settingsDto;
        }
        public async Task<BussnessResultModel> EditSetting(SettingVM update)
        {
            PropertyInfo[] properties = update.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var itemDB = await _repositoryManager.Setting.GetSettingByValue(property.Name, true);
                itemDB.Value = property.GetValue(update)?.ToString();
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(properties, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> EditSettingStore(SettingStoreVM update)
        {
            PropertyInfo[] properties = update.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var itemDB = await _repositoryManager.Setting.GetSettingByValue(property.Name, true);
                itemDB.Value = property.GetValue(update)?.ToString();
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(properties, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<SocialSettingVM> GetSocialSetting()
        {
            var settingList = await _repositoryManager.Setting.GetAllSettings(false);
            return new SocialSettingVM
            {
                facebook_url = settingList.Where(r => r.Key == "facebook_url").First().Value,
                twitter_url = settingList.Where(r => r.Key == "twitter_url").First().Value,
                youtube_link = settingList.Where(r => r.Key == "youtube_link").First().Value,
                instagram_url = settingList.Where(r => r.Key == "instagram_url").First().Value,
                press_link = settingList.Where(r => r.Key == "press_link").First().Value,
                android_app_link = settingList.Where(r => r.Key == "android_app_link").First().Value,
                ios_app_link = settingList.Where(r => r.Key == "ios_app_link").First().Value
            };
        }
        public async Task<SettingStoreVM> GetSettingStore()
        {
            var settingList = await _repositoryManager.Setting.GetAllSettings(false);
            return new SettingStoreVM
            {
                google_map_api = settingList.Where(r => r.Key == "google_map_api").First().Value,
                contact_us_email = settingList.Where(r => r.Key == "contact_us_email").First().Value,
                order_email = settingList.Where(r => r.Key == "order_email").First().Value,
                hide_price = settingList.Where(r => r.Key == "hide_price").First().Value,
            };
        }
    }
}