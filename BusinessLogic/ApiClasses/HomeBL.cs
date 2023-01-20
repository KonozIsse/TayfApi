using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Status = Entities.Models.Enums.Status;
using Entities.ViewModel;
using AutoMapper;
using Contracts;
using BussnessResultModel = Entities.Exception.BussnessResultModel;
using Entities.Exception;
using Entities.RequestFeatures;
using Entities.Models.Enum;
using MailKit.Search;
using Org.BouncyCastle.Asn1.X509;
using System.Numerics;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogic.ApiClasses
{
    public class HomeBL 
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper; 
        protected readonly NewsBL _newsBL;
        private readonly LocService _locService;
        private readonly ProductBL _productBL;
        private readonly ImageBL _imageBL;
        private readonly UserBL _userBL;
        private readonly Util _util;
        public HomeBL(IRepositoryManager repositoryManager, IMapper mapper,  NewsBL newsBL, LocService locService , ProductBL productBL, ImageBL imageBL , UserBL userBL,Util util)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _newsBL = newsBL;
            _locService = locService;
            _productBL = productBL;
            _imageBL = imageBL;
            _userBL = userBL;
            _util = util;
        }
        public List<string> GetListStatus()
        {
            return Enum.GetNames(typeof(Status)).ToList();
        }
      
        public async Task<NavbarVM> GetNavbar()
        {
            var navbarDto = new NavbarVM
            {
                DefaultLanguage = await GetDefaultLanguage(),
                Languages = await GetAllLanguages(),
                Currencies = await GetCurrencies()
            };
            return navbarDto;
        }
        public async Task<HomeVM> GetHome(int customerId, Currency cod, string code = "en")
        {
            var language = await _repositoryManager.Language.GetCodeLanguage(code, false);
            int langId = language.Id;
            var model = new HomeVM
            {
                //sliders = GetSliderWeb(code),
                Banner = await GetBanner(langId),
                services = GetServices(code),
                blog = await _newsBL.GetNews(code),
                ProductsPopular = await _productBL.PopularsPage(),
                ProductsBest = await _productBL.BestPage(),
                ProductsLatest = await _productBL.LatestPage(),
                ProductsSpecial = await _productBL.SpecialsPage(),
                ProductsTopRated = await _productBL.TopRatedPage(),
                ProductsDailyDeal = await _productBL.DailyDeals(),
                products = await _productBL.GetProducts(customerId, code),
                flash = await _productBL.GetFlashProds(),
                specialProducts = await _productBL.GetSpecialsProd(),
                stores = await _userBL.GetStores()
            };
            return model;
        }
        public async Task<HomeCPVM> GetHomeCP(int userId)
        {
            var orders = await _repositoryManager.Order.GetAllOrders();
            var products = await _repositoryManager.Product.GetAllProducts();
            var stocks = await _repositoryManager.Inventory.GetAllOutStock();
            var ordersTransaction = orders.Where(c => c.TransactionId != null);
            var countCustomers = await _repositoryManager.User.GetCustomers(false);

            var store = await _repositoryManager.User.GetUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                orders = orders.Where(c => c.StoreId == userId).ToList();
                products = products.Where(c => c.StoreId == userId).ToList();
                stocks = stocks.Where(c => c.VendorId == userId).ToList();
            }

            var model = new HomeCPVM
            {
                TotalOrders = orders.Count(),
                TotalProducts = products.Count(),
                TotalOutStock = stocks.Count(),
                TotalPurchased = ordersTransaction.Sum(c => c.OrderPrice),
                TotalTransactions = orders.Sum(c => c.OrderPrice),
                TotalCustomerRegistrations = countCustomers.Count(),
            };
            return model;
        }
        public async Task<List<CustomerDto>> GetNewCustomers()
        {
            var customers = await _repositoryManager.User.GetCustomers(false);
            var customersDto = _mapper.Map<List<CustomerDto>>(customers);
            return customersDto.Take(4).ToList();
        } 
        public async Task<List<RecentProductDto>> GetRecentProducts(int? storeId)
        {
            var products = await _repositoryManager.Product.GetProductsCP("");
            var productsDto = _mapper.Map<List<RecentProductDto>>(products);
            return productsDto.Take(15).ToList();
        }
        public async Task<List<OrderDto>> GetOrders(int? storeId)
        {
            var orders = await _repositoryManager.Order.GetAllOrders();
            if(storeId != 0)
            {
                orders = orders.Where(c => c.StoreId == storeId).ToList();
            }
            var productsDto = new List<OrderDto>();
            foreach(var order in orders)
            {
                productsDto.Add(new OrderDto
                {
                    Id = order.Id,
                    OrderPrice = Convert.ToDecimal(order.OrderPrice),
                    CustomerName = order.Customer.FullName,
                    OrderStatusName = order.OrderStatus.StatusName
                });
            }
           
            return productsDto.Take(15).ToList();
        }
        // Math.Round((((double)x.Count() / AllCount.Count()) * 100), 2)
        //public async Task<> GetGoalCompletion(int? storeId)
        //{
        //    var orders = await _repositoryManager.Order.GetAllOrders();
        //    if (storeId != 0)
        //    {
        //        orders = orders.Where(c => c.StoreId == storeId).ToList();
        //    }
        //    var ordersPanding = orders.Where(c => c.OrderStatusId == 1);
        //    var ordersCompleting = orders.Where(c => c.OrderStatusId == 2);
        //    var ordersCanceled = orders.Where(c => c.OrderStatusId == 3);

        //    var products =  await _repositoryManager.Product.GetProductsCP(storeId, "");

        //    var carts = await _repositoryManager.Cart.GetCarts();
        //    var grouped = carts.GroupBy(c => c.Product).Select(x => new
        //    {
        //        VotesCount = x.Count()  / products.Count(),
        //        VotePercentage = Math.Round((((double)x.Count() / products.Count()) * 100), 2)
        //    });
        //    var groupedorders = orders.GroupBy(c => c.OrderStatus).Select(x => new
        //    {
        //        VotesCount = x.Count()  ,
        //    });



        //    return model;
        //}
        public async Task<string> GetLogo()
        {
            var logo = await _repositoryManager.Setting.GetSettingByValue("website_logo", false);
            return await _imageBL.GetImageOriginal(logo.Value);
        }
        //Banner------------------------------------------------
        public async Task<BannerDto> GetBanner(int langId)
        {
            var banner = await _repositoryManager.Banner.GetBannerByType(langId, "category" , false);
            if(banner == null)
            {
                return null;
            }
            return new BannerDto
            {
                Url = banner.Url,
               // ImgId = Convert.ToInt32( urlImg + banner.Image.ImageSettings.FirstOrDefault(i => i.ImageType == ImageType.ACTUAL).Path)
            };
        }
        public async Task<List<BannerDto>> GetBanners()
        {
            var banners = await _repositoryManager.Banner.GetAllBanner(false);
            var bannersDto = _mapper.Map<List<BannerDto>>(banners);
            return bannersDto;
        }
        public async Task UpdateBanner(UpdateBannerDto updateDto)
        {
            var banner = await _repositoryManager.Banner.GetBannerId(updateDto.Id, true);
            _mapper.Map(updateDto, banner);
            await _repositoryManager.SaveAsync();
        }
        //Sliders------------------------------------------------
        public PagedList<SliderDto> GetSliderMobile(string lang , PostsParameters postsParameters)
        {
            var sliders = _repositoryManager.Slider.GetSlidersForMobile()
                .Select(c => new SliderDto
                {
                    Title = lang == "en" ? c.Title : c.TitleAr,
                    Decription = lang == "en" ? c.Decription : c.DecriptionAr,
                    ImageId = Convert.ToInt32(_imageBL.GetImageMedium(c.ImgId.ToString()))
                }).ToList();
            return PagedList<SliderDto>.ToPagedList(sliders, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public PagedList<SliderDto> GetSliderWeb(string search, string lang, PostsParameters postsParameters)
        {
            var sliders = _repositoryManager.Slider.GetSlidersForWeb(search)
                .Select(c => new SliderDto
                {
                    Title = lang == "en" ? c.Title : c.TitleAr,
                    Decription = lang == "en" ? c.Decription : c.DecriptionAr,
                    Url = c.Url,
                    ImageId = Convert.ToInt32(_imageBL.GetImageOriginal(c.ImgId.ToString()))
                }).ToList();
            return PagedList<SliderDto>.ToPagedList(sliders, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task AddSliderWeb(int storeId , CreateSliderDto createSliderDto)
        {
            var slider = _mapper.Map<Sliders>(createSliderDto);
            slider.Type = SlidersImageType.Web;
            slider.VendorId = storeId;
            _repositoryManager.Slider.AddSlider(slider);
            await _repositoryManager.SaveAsync();
        } 
        public async Task<BussnessResultModel> AddSliderMobile(int storeId, CreateSliderDto createSliderDto)
        {
            var slider = _mapper.Map<Sliders>(createSliderDto);
            if(createSliderDto.ImageId == 0)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctImage"), false);
            }
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
            _mapper.Map(updateDto, slider);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(slider, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> DeleteSlide (int id)
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
        //Services------------------------------------------------
        public List<ServiceDto> GetServices(string code)
        {
            var services = _repositoryManager.Services.GetAllServices(false)
                .Select(c => new ServiceDto
                {
                    Title = code == "en" ? c.Title : c.TitleAr,
                    Description = code == "en" ? c.Description : c.DescriptionAr,
                    // ImgId =  Convert.ToInt32(urlImg + x.Image.ImageSettings.FirstOrDefault(i => i.ImageType == ImageType.ACTUAL).Path)
                }).ToList();
            return services;
        }
        public async Task<BussnessResultModel> UpdateService( UpdateServiceDto update)
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
            var service = await _repositoryManager.Services.GetServiceById(id,false, false);
            if (service == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            
            _repositoryManager.Services.DeleteService(service);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(service, _locService.GetLocalizedStringValue("successDelete"));
        }
        //Contact------------------------------------------------
        public async Task<ContactVM>  GetContactSetting()
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
        public async Task<Contact> GetContact(int id)
        {
            return await _repositoryManager.Contact.GetContactById(id, false);
        }
        public async Task<List<ContactDto>> GetAllContacts(string search, int rows, int pageId = 1)
        {
            var contacts = await _repositoryManager.Contact.GetContacts(search,rows, pageId);
            if(contacts == null)
            {
                return null;
            }
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);
            return contactsDto;
        }
        public async Task<BussnessResultModel> AddContact(CreateContactDto createContactDto)
        {
            var contact = _mapper.Map<Contact>(createContactDto);
            contact.IsRead = false;
            contact.IsStatus = Status.NotActive;
            _repositoryManager.Contact.CreateContact(contact);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(contact ,_locService.GetLocalizedStringValue("successAdd"));
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
        public async Task<MessageTemplate> GetEmailMessage(int id)
        {
            return await _repositoryManager.MessageTemplate.GetTemplateById(id, false);
        }
        public async Task<List<MessageTemplateDto>> GetAllMessageTemplates()
        {
            var templets = await _repositoryManager.MessageTemplate.GetEmailTemplatesList(false);
            var templetsDto = _mapper.Map<List<MessageTemplateDto>>(templets);
            return templetsDto;
        }
        public async Task<BussnessResultModel> UpdateTemplate( UpdateTemplateDto updateDto)
        {
            var template = await _repositoryManager.MessageTemplate.GetTemplateById(updateDto.Id, true);
            if(template == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            _mapper.Map(updateDto, template);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(template, _locService.GetLocalizedStringValue("successSave"));
        }
        //Email------------------------------------------------
        public async Task<MailList> GetMailListId(int id)
        {
            return await _repositoryManager.MailList.GetMailListById(id, false);
        }
        public async Task<List<MailListDto>> GetAllMailLists()
        {
            var emails = await _repositoryManager.MailList.GetMailLists();
            var emailsDto = _mapper.Map<List<MailListDto>>(emails);
            return emailsDto;
        }  
        public async Task<PagedList<MailListDto>> GetMailLists(string search, PostsParameters postsParameters)
        {
            var emails = await _repositoryManager.MailList.GetMailListEmail(search);
            var emailsDto = _mapper.Map<List<MailListDto>>(emails);
            return PagedList<MailListDto>.ToPagedList(emailsDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task SendUserEmail( string email)
        {
            var exsitEmail = await _repositoryManager.MailList.GetEmail(email);
            if(exsitEmail == null)
            {
                var send = new SendMailListDto
                {
                    Email = email
                };
                var mailList = _mapper.Map<MailList>(send);
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
       
        public async Task<PagedList<LanguageDto>> GetLanguages(string search , string lang , PostsParameters postsParameters)
        {
            var languages = await _repositoryManager.Language.GetAllLanguage(search);
            //var languagesDto = _mapper.Map<List<LanguageDto>>(languages);
            var languagesDto = new List<LanguageDto>();
            foreach(var language in languages)
            {
                languagesDto.Add(new LanguageDto
                {
                    Id = language.Id,
                    Name = lang == "en" ? language.Name : language.NameAr,
                });
            }
            return PagedList<LanguageDto>.ToPagedList(languagesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<LanguageDto>> GetAllLanguages()
        {
            var languages = await _repositoryManager.Language.ListLanguage(false);
            var languagesDto = _mapper.Map<List<LanguageDto>>(languages);
            return languagesDto;
        }
        public async Task<LanguageDto> GetDefaultLanguage()
        {
            var language = await _repositoryManager.Language.GetDefaultLanguage( false);
            var languagesDto = _mapper.Map<LanguageDto>(language);
          //  languagesDto.ImgId = Convert.ToInt32(await _imageApi.GetImageOriginal(language.ImgId.ToString()));
            return languagesDto;
        }  
        public async Task<Language> GetLanguageCode(string code)
        {
           return await _repositoryManager.Language.GetCodeLanguage(code , false);
        } 
        public async Task<Language> GetLanguageId(int id)
        {
            return await _repositoryManager.Language.GetCodeLanguageId(id , false);
        }
        public async Task<BussnessResultModel> DeleteLanguage(int id)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(id, false);
            if(language == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _repositoryManager.Language.DeleteLanguage(language);
            await _repositoryManager.SaveAsync();

            return new BussnessResultModel(language, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> UpdateLanguage(UpdateLanguageDto updateDto)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(updateDto.Id, true);
            if (language == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _mapper.Map(updateDto, language);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(language, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task ChangeLanugage(int id)
        {
          var language = await _repositoryManager.Language.GetCodeLanguageId(id, true);
            if (language != null)
            {
                language.IsDefault = 1;
                await _repositoryManager.SaveAsync();
            }
        }
       
        //Currency------------------------------------------------
        public async Task<PagedList<CurrencyDto>> GetAllCurrencies(string lang , PostsParameters postsParameters)
        {
            var currencies = await _repositoryManager.Currency.GetAllCurrencies(false);
            var currenciesDto = _mapper.Map<List<CurrencyDto>>(currencies);
            var currency = currencies.First();
            var currencyDto = currenciesDto.First();
            currencyDto.Name = lang == "en" ? currency.Name : currency.NameAr;
            return PagedList<CurrencyDto>.ToPagedList(currenciesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<CurrencyDto>> GetCurrencies(string lang = "")
        {
            var currencies = await _repositoryManager.Currency.GetAllCurrencies(false);
            var currenciesDto = _mapper.Map<List<CurrencyDto>>(currencies);
            var currency = currencies.First();
            var currencyDto = currenciesDto.First();
            currencyDto.Name = lang == "en" ? currency.Name : currency.NameAr;
            return currenciesDto;
        } 
        public async Task<CurrencyDto> GetCurrency(int id ,string lang = "en")
        {
            var currency = await _repositoryManager.Currency.GetCurrency(id,false);
            var currencyDto = _mapper.Map<CurrencyDto>(currency);
            currencyDto.Name = lang == "en" ? currency.Name : currency.NameAr;
            return currencyDto;
        } 
        public async Task<BussnessResultModel> AddCurrency(CreateCurrencyDto createDto)
        {
            var IsExists = _repositoryManager.Currency.ExistCurrency(createDto.Symbol);
            if (IsExists)
            {
                return new BussnessResultModel(null,_locService.GetLocalizedStringValue("ExistItem"),false) ;
            }
            else
            {
                var currency = _mapper.Map<Currency>(createDto);
                currency.IsDefault = 0;
                if (createDto.Position == "0")
                {
                    currency.Position = "Left";
                }
                if (createDto.Position == "1")
                {
                    currency.Position = "Right";
                }

                _repositoryManager.Currency.AddCurrency(currency);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(currency, _locService.GetLocalizedStringValue("successAdd"));
            }
        }
        public async Task<BussnessResultModel> UpdatCurrency(UpdateCurrencyDto updateDto)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(updateDto.Id, true);
            if(currency != null)
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
            if(currency == null)
            {
                return new BussnessResultModel(null,_locService.GetLocalizedStringValue("correctLink"), false);
            }
            _repositoryManager.Currency.DeleteCurrency(currency); 
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(currency, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task ChangeCurrency(int id)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(id, true);
            if (currency != null)
            {
                currency.IsDefault = 1;
                await _repositoryManager.SaveAsync();
            }
        }
        //Notification------------------------------------------------

        public async Task<BussnessResultModel> CreateNotification(CreateNotificationDto create)
        {
           
            var users =  await _repositoryManager.User.GetCustomers(false);
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
                    var user = await _repositoryManager.User.GetActiveUserId(f, false);;
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
            if(notification == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"),false) ;
            }
            _repositoryManager.Notification.DeleteNotification(notification);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(notification, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<List<NotificationDto>> GetNotifications(int PageId)
        {
            var notfis = await _repositoryManager.Notification.GetNotificationsPage(PageId,15);
            //var dto = _mapper.Map<List<NotificationDto>>(notfis);
            var dto =  new List<NotificationDto>();
            foreach(var item in notfis)
            {
                dto.Add(new NotificationDto
                {
                    Id = item.Id,
                    Name = item.User.FullName,
                    Subject = item.Subject,
                    Body = item.Body,
                    NotificationKey = item.NotificationAction.NotificationKey,
                    CreatedAt = item.CreatedAt
                });
            }
            return dto;
        }
        //setting------------------------------------------------------

        public async Task<Setting> GetSettingKey(string name)
        {
            return await _repositoryManager.Setting.GetSettingByValue(name,false);
        }
        public async Task<IEnumerable<Setting>> GetAllSettings()
        {
            return await _repositoryManager.Setting.GetAllSettings(false);
        }
        public async Task<PagedList<PageDto>> GetAllPages(string search ,string lang , PostsParameters postsParameters)
        {
            var pages = await _repositoryManager.StaticPages.GetAllPages(search ,false);
            var pagesDto = _mapper.Map<List<PageDto>>(pages);
            var page = pages.First();
            var pageDto = pagesDto.First();
            pageDto.Title = lang == "en" ? page.Title : page.TitleAr;
            pageDto.Description = lang == "en" ? page.Description : page.DescriptionAr;
            return PagedList<PageDto>.ToPagedList(pagesDto, postsParameters.PageNumber, postsParameters.PageSize);
        } 
        public async Task EditSettingVM (SettingVM update , string value)
        {
            var itemDB = await GetSettingKey("cp_logo");
            var itemDB2 = await GetSettingKey("website_logo");
            if (value != null)
            {
                itemDB.Value = value;
                itemDB2.Value = value;
            }
            await _repositoryManager.SaveAsync();

        }
        public async Task<PageDto> GetPage(int id , string lang = "en")
        {
            var page = await _repositoryManager.StaticPages.GetPage(id,false);
            var pageDto = _mapper.Map<PageDto>(page);
            pageDto.Description = lang == "en" ? page.Description : page.DescriptionAr;
            return pageDto;
        }
        public async Task<BussnessResultModel> EditPage(EditPageDto update)
        {
            var page = await _repositoryManager.StaticPages.GetPage(update.Id,true);
            if(page == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink") , false);
            } 
            _mapper.Map(update, page);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(page, _locService.GetLocalizedStringValue("successSave"));
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
        //Address------------------------------------------------------
        public async Task<List<AddressDto>> GetAddresses(int user)
        {
            var addresses = await _repositoryManager.Address.GetAllAddressesByCustomerId(user);
            var addressesDto = _mapper.Map<List<AddressDto>>(addresses);
            return addressesDto;
        }
        public async Task<User> GetCustomer(int custId)
        {
            return await _repositoryManager.User.GetActiveUserId(custId,false);
        }
        public async Task<List<CartVM>> GetHomeCart(int user, Currency currency)
        {
            var carts = await _repositoryManager.Cart.CartsNotActiveCustomer(user);
            if (carts == null)
            {
                // return new ExceptionModel<List<CartVM>>(null, _locService.GetLocalizedStringValue(""), false);
            }
            var cartVM = new List<CartVM>();
            foreach (var cart in carts)
            {
                if (cart.StoreId != 0)
                {
                    var product = await _repositoryManager.Product.GetProductById(cart.ProdId, false);
                    var store = await _repositoryManager.User.GetStore(cart.StoreId, false);
                    var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(cart.ProdId);
                    var attr = await _productBL.GetOptions(cart.ProdId);
                    var flash = await _repositoryManager.Sales.GetFlashProductId(cart.ProdId);
                    if (product != null)
                    {
                        var offer_price = special == null ? 0 : special.SpecialPrice;
                        cartVM.Add(new CartVM
                        {
                            Id = cart.Id,
                            Qty = cart.Qty,
                            StoreId = cart.StoreId,
                            FinalPrice = cart.FinalPrice,
                            Attributes = attr ?? null,
                            ShareLink = _util.url1 + "/share.html?id=" + cart.ProdId,
                            ProductId = cart.ProdId,
                            ProductName = product.ProductName,
                            //ProductImage = await _imageBL.GetImageOriginal(product.Images.First().ToString()),
                            IsFeature = product.IsFeature,
                            SpecialPrice = offer_price,
                            StoreName = store.FirstName,
                            IsSpecial = (special == null ? false : true),
                            ProductDescription = product.Description,
                            CreatedAt = product.CreatedAt.ToString(),
                            UpdatedAt = product.UpdatedAt.ToString() ?? null,
                            ProductModel = product.ProductModel,
                            ProductPrice = (flash != null ? flash.DiscountPrice : product.Price),
                            ProductStatus = Convert.ToInt16(product.IsStatus)
                        });
                    }
                }
            }
            return/* new ExceptionModel<List<CartVM>>(cartVM)*/ cartVM;
        }
    }
}