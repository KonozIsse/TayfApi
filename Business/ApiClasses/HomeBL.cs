using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models.Enums;
using Status = Entities.Models.Enums.Status;
using Zone = Entities.Models.Zone;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using BusinessLogic.ViewModel;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Contracts;

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
         string lang ;

        public HomeBL(IRepositoryManager repositoryManager, IMapper mapper,  NewsBL newsBL, LocService locService , ProductBL productBL, ImageBL imageBL , UserBL userBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _newsBL = newsBL;
            _locService = locService;
            _productBL = productBL;
            _imageBL = imageBL;
            _userBL = userBL;
        }
        public async Task<NavbarVM> GetNavbar()
        {
            var navbarDto = new NavbarVM
            {
                DefaultLanguage = await GetLanguage(),
                Languages = await GetAllLanguages(),
                Currencies = await GetCurrencies()
            };
            return navbarDto;
        }
        public async Task<HomeVM> GetHome(int customerId, Currency cod)
        {
            var language = await _repositoryManager.Language.GetCodeLanguage(lang, false);
            int langId = language.Id;
            var model = new HomeVM
            {
                sliders = GetSliderWeb(),
                Banner = await GetBanner(langId),
                services = GetServices(),
                blog = await _newsBL.GetNews(),
                ProductsPopular = await _productBL.PopularsPage(),
                ProductsBest = await _productBL.BestPage(),
                ProductsLatest = await _productBL.LatestPage(),
                ProductsSpecial = await _productBL.SpecialsPage(),
                ProductsTopRated = await _productBL.TopRatedPage(),
                ProductsDailyDeal = await _productBL.DailyDeals(),
                //products = await _productBL.GetProductsCatId(0, customerId),
                flash = await _productBL.GetFlashProds(customerId),
                specialProducts = await _productBL.GetSpecialsProd(customerId),
                stores = await _userBL.GetStores()
            };
            return model;
        }
        public async Task<string> GetLogo()
        {
            return await _imageBL.GetImageOriginal(_repositoryManager.Setting.GetSettingByValue("website_logo").Value);
        }
        //Banner------------------------------------------------
        public async Task<BannerDto> GetBanner(int langId)
        {
            var banner = await _repositoryManager.Banner.GetBannerByType(langId, "category" , false);
            return banner == null ? null : new BannerDto
            {
                Url = banner.Url,
               // ImgId = Convert.ToInt32( urlImg + banner.Image.ImageSettings.FirstOrDefault(i => i.ImageType == ImageType.ACTUAL).Path)
            };
        }
        public async Task UpdateBanner(int id, UpdateBannerDto updateDto)
        {
            var banner = await _repositoryManager.Banner.GetBannerId(id, true);
            _mapper.Map(updateDto, banner);
            await _repositoryManager.SaveAsync();
        }
        //Sliders------------------------------------------------
        public List<SliderDto> GetSliderMobile(string lange)
        {
            var sliders = _repositoryManager.Slider.GetSlidersForMobile()
                .Select(c => new SliderDto
                {
                    Title = (c.Title != null ? c.Title = _locService.GetLocalizedStringValue(lange) : ""),
                    Decription = (c.Decription != null ? c.Decription = _locService.GetLocalizedStringValue(lange) : ""),
                    ImageId = Convert.ToInt32(_imageBL.GetImageMedium(c.ImgId.ToString()))
                }).ToList();
            return sliders;
        }
        public List<SliderDto> GetSliderWeb()
        {
            var sliders = _repositoryManager.Slider.GetSlidersForWeb()
                .Select(x => new SliderDto
                {
                    Title = x.Title,
                    Decription = x.Decription,
                    Url = x.Url,
                    ImageId = Convert.ToInt32(_imageBL.GetImageOriginal(x.ImgId.ToString()))
                }).ToList();
            return sliders;
        }
        public async Task AddSlider(CreateSliderDto createSliderDto)
        {
            var slider = _mapper.Map<Sliders>(createSliderDto);
            _repositoryManager.Slider.AddSlider(slider);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateSlider(int id, UpdateSliderDto updateDto)
        {
            var slider = await _repositoryManager.Slider.GetSlideById(id, true);
            _mapper.Map(updateDto, slider);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteSlide (int id)
        {
            var slider = await _repositoryManager.Slider.GetSlideById(id, false);
            _repositoryManager.Slider.DeleteSlider(slider);
            await _repositoryManager.SaveAsync();
        }
        //Services------------------------------------------------
        public List<ServiceDto> GetServices()
        {
            var services = _repositoryManager.Services.GetAllServices(false)
                .Select(x => new ServiceDto
                {
                    Title = x.Title,
                    Description = x.Description,
                   // ImgId =  Convert.ToInt32(urlImg + x.Image.ImageSettings.FirstOrDefault(i => i.ImageType == ImageType.ACTUAL).Path)
                }).ToList();
            return services;
        }
        public async Task UpdateService(int id , UpdateServiceDto updateServiceDto)
        {
            var service = await _repositoryManager.Services.GetServiceById(id, true, false);
            _mapper.Map(updateServiceDto , service);
            await _repositoryManager.SaveAsync();
        }
        //Contact------------------------------------------------
        public async Task<Contact> GetContact(int id)
        {
            return await _repositoryManager.Contact.GetContactById(id, false);
        }
        public async Task<List<ContactDto>> GetAllContacts()
        {
            var contacts = await _repositoryManager.Contact.GetContacts(false);
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts);
            return contactsDto;
        }
        public async Task AddContact(CreateContactDto createContactDto)
        {
            var contact = _mapper.Map<Contact>(createContactDto);
            contact.IsRead = false;
            contact.IsStatus = Status.NotActive;
            _repositoryManager.Contact.CreateContact(contact);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteContact(int id)
        {
            var contact = await _repositoryManager.Contact.GetContactById(id, false);
            _repositoryManager.Contact.DeleteContact(contact);
            await _repositoryManager.SaveAsync();
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
        public async Task UpdateTemplate(int id, UpdateTemplateDto updateDto)
        {
            var template = await _repositoryManager.MessageTemplate.GetTemplateById(id, true);
            _mapper.Map(updateDto, template);
            await _repositoryManager.SaveAsync();
        }
        //Email------------------------------------------------
        public async Task<List<MailListDto>> GetAllMailLists()
        {
            var emails = await _repositoryManager.MailList.GetMailLists();
            var emailsDto = _mapper.Map<List<MailListDto>>(emails);
            return emailsDto;
        }
        public async Task SendUserEmail(SendMailListDto sendMailListDto)
        {
            var mailList = _mapper.Map<MailList>(sendMailListDto);
            _repositoryManager.MailList.SendUserEmail(mailList);
            await _repositoryManager.SaveAsync();
        }
        public async Task RemoveMailList(int id)
        {
            var email = await _repositoryManager.MailList.GetMailListById(id, false);
            _repositoryManager.MailList.RemoveMailList(email);
            await _repositoryManager.SaveAsync();
        }
        //Language------------------------------------------------
        //public async Task<BussnessResultModel<List<LanguageDto>>> GetAllLanguages()
        //{
        //    //var hanNoPermission = false;
        //    //if (!hanNoPermission)
        //    //{
        //    //    return new BussnessResultModel<List<LanguageDto>>(null, "no permission", false);
        //    //}

        //    var languages = await _repositoryManager.Language.ListLanguage(false);
        //    if (languages == null)
        //    {
        //        return new BussnessResultModel<List<LanguageDto>>(null, _locService.GetLocalizedStringValue("NoLanguage"), false);
        //    }
        //    var languagesDto = _mapper.Map<List<LanguageDto>>(languages);
        //    return new BussnessResultModel<List<LanguageDto>>(languagesDto);
        //}
        public async Task<List<LanguageDto>> GetAllLanguages()
        {
            var languages = await _repositoryManager.Language.ListLanguage(false);
            var languagesDto = _mapper.Map<List<LanguageDto>>(languages);
            return languagesDto;
        }
        public async Task<LanguageDto> GetLanguage()
        {
            var language = await _repositoryManager.Language.GetDefaultLanguage( false);
            var languagesDto = _mapper.Map<LanguageDto>(language);
          //  languagesDto.ImgId = Convert.ToInt32(await _imageApi.GetImageOriginal(language.ImgId.ToString()));
            return languagesDto;
        }
        public async Task DeleteLanguage(int id)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(id, false);
            _repositoryManager.Language.DeleteLanguage(language);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateLanguage(int id, UpdateLanguageDto updateLanguageDto)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(id, true);
            _mapper.Map(updateLanguageDto, language);
            await _repositoryManager.SaveAsync();
        }
        public async Task ChangeLanugage(int id)
        {
          var language = await _repositoryManager.Language.GetCodeLanguageId(id, true);
            if (language != null)
            {
                language.IsDefault = 1;
            }
            else
            {
                language.IsDefault = 0;
            }
            
            await _repositoryManager.SaveAsync();
        }
        //Currency------------------------------------------------
        public async Task<List<CurrencyDto>> GetCurrencies()
        {
            var currencies = await _repositoryManager.Currency.GetAllCurrencies(false);
            var currenciesDto = _mapper.Map<List<CurrencyDto>>(currencies);
            return currenciesDto;
        }
        public async Task AddCurrency(CreateCurrencyDto createDto)
        {
            var currency = _mapper.Map<Currency>(createDto);
            _repositoryManager.Currency.AddCurrency(currency);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdatCurrency(int id, UpdateCurrencyDto updateDto)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(id, true);
            _mapper.Map(updateDto, currency);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteCurrency(int id)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(id, false);
            _repositoryManager.Currency.DeleteCurrency(currency);
            await _repositoryManager.SaveAsync();
        }
        //Notification------------------------------------------------
        public int GetCountNotify(int userId)
        {
            return _repositoryManager.Notification.GetNotificationCountUser(userId);
        }
        public async Task ReadNotification(int userId)
        {
            var notifications = await _repositoryManager.Notification.GetNotificationsToUserId(userId, true);
            for (int i = 0; i < notifications.Count(); i++)
            {
                notifications[i].IsRead = true;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task DeleteNotification(int id)
        {
            var notification = await _repositoryManager.Notification.FindNotificationId(id, false);
            _repositoryManager.Notification.DeleteNotification(notification);
            await _repositoryManager.SaveAsync();
        }
        public async Task AddNotification(CreateNotificationDto createNotificationDto)
        {
            var notification = _mapper.Map<Notification>(createNotificationDto);
            _repositoryManager.Notification.CreateNotification(notification);
            await _repositoryManager.SaveAsync();
        }
        //thing------------------------------------------------
        //public async Task<List<OrderDto>> GetOrders(int customerId)
        //{
        //    var orderProductDto = new List<OrderProductDto>();
        //    var orders = await _repositoryManager.Order.GetOrdersToCustomer(customerId);
        //    var ordersDto = _mapper.Map<List<OrderDto>>(orders);
        //    var orderDto = ordersDto.First();
        //    foreach (var order in orders)
        //    {
        //        if (order.StoreId != 0)
        //        {
        //            var productsOrder = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(order.Id);
        //            if (productsOrder.Count() > 0)
        //            {
        //                productsOrder.ForEach(c => orderProductDto.Add(new OrderProductDto
        //                { 
        //                }));
        //             }
        //        }
        //        var couponDto = new CouponDto();
        //        if (!String.IsNullOrEmpty(order.Coupon.CouponCode))
        //        {
        //            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(order.Coupon.CouponCode);
        //            if (coupon != null)
        //            {
        //                List<int> SelectedValues = new List<int>();
        //                if (coupon.Product != null && coupon.Product != "")
        //                {
        //                    string[] split = coupon.Product.Split(',');
        //                    foreach (var item in split)
        //                    {
        //                        if (!String.IsNullOrEmpty(item))
        //                        {
        //                            SelectedValues.Add(Convert.ToInt32(item));
        //                        }
        //                    }
        //                }
        //                couponDto = new CouponDto
        //                {

        //                    ExpiryDate = Convert.ToDateTime(!String.IsNullOrEmpty(coupon.ExpiryDate.ToString()) ? coupon.ExpiryDate.Value.Date.ToString("dd/MM/yyyy") : ""),
        //                    ProductIds = SelectedValues
        //                };
        //            }
        //            else
        //            {
        //                couponDto = null;
        //            }
        //        }
        //        else
        //        {
        //            couponDto = null;
        //        }
        //        var addressDto = new AddressDto();
        //        var address = await _repositoryManager.Address.GetAddress(order.AddressId,false);

        //        if ( address != null)
        //        {
        //            addressDto = new AddressDto
        //            {
        //                CityName = (address.Country == null ? "" :
        //                (address.Country.Zones.Where(r => r.Id == address.ZoneId).FirstOrDefault() == null ? "" :
        //                address.Country.Zones.Where(r => r.Id == address.ZoneId).FirstOrDefault().ZoneName)),
        //                //Tax = _settRepo.calculateTax(addrs.entry_zone_id),
        //                IsDefault = await _repositoryManager.User.GetUserDefaultAddress(customerId, address.Id) != null
        //            };
        //        }
        //        var CategoryDtos = new List<CategoryDto>();
        //        var store = await _repositoryManager.User.GetStoreId(order.StoreId);
        //        if (store != null)
        //        {
        //            var categories = await _repositoryManager.Categories.GetAllCategories(false);
        //            if (categories.Count() > 0)
        //            {
        //                foreach (var category in categories)
        //                {
        //                    CategoryDtos.Add(new CategoryDto
        //                    {
        //                        ImageId = Convert.ToInt32(await _imageApi.GetImageMedium(category.ImgId.ToString())),
        //                        //CountProduct = ordersDto.Where(r => r. == c3.id && r.category_id == ct.categories_id).Count(),
        //                        //Products = pop.Where(r => r.store_id == c3.id && (r.category_id == ct.categories_id)).ToList(),
        //                        //Total = pop.Where(r => r.store_id == c3.id && r.category_id == ct.categories_id).Sum(x => x.final_price),
        //                    });
        //                }
        //            }
        //            var ordersStore = ordersDto.Where(r => r.VendorId == store.Id).ToList();
        //            int count = ordersStore.Count();
        //            var nameTime = "";
        //            if (order.DeliveryTimeId != 0)
        //            {
        //                var time = await _repositoryManager.DeliveryTime.GetDeliveryTimeById(order.DeliveryTimeId, false);
        //                if (time != null)
        //                {
        //                    nameTime = time.Time;
        //                }
        //            }
        //            string stat = "";
        //            var states = await _repositoryManager.OrderStatus.GetOrderStatusById(order.OrderStatusId, false);
        //            if (states != null)
        //            {
        //                stat = states.StatusName;
        //            }
        //            var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
        //            string image = url + "/" + filesRootPath + await _imageApi.GetImageOriginal(store.Avater);
        //            string imgcltThumbStore = url + await _imageApi.GetImageThumbnail(store.Avater);
        //            string imgcltMidStore = url + await _imageApi.GetImageMedium(store.Avater);
        //            var pay = new PaymentSetting { };
        //            ordersDto.Add(new OrderDto
        //            {
        //                DatePurchased = Convert.ToDateTime(!String.IsNullOrEmpty(order.DatePurchased.ToString()) ?
        //                order.DatePurchased.Value.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss tt") : ""),
        //                //countProduct = productsCount;
        //                //image_medium = imgcltMidStore,
        //                //image_thumb = imgcltThumbStore,

        //                //times = tm,

        //                 //CouponId = couponDto,  
        //                //order_status_name = stat,

        //                //categories = allcat.Where(x => x.products.Where(r => r.store_id == c3.id).Count() > 0).ToList(),
        //                //item_counts = cnt,
        //                // = ord.order_price,
        //                //payment = pay

        //                //total = prod2.Where(r=>r.vendor_id == c3.id).Sum(x => x.order_price)

        //            });
        //        }
        //    }
        //    return ordersDto;
        //}
    }
}