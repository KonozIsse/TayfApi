using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models.Enums;
using Status = Entities.Models.Enums.Status;
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
        private readonly Language lang ;

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
        public List<string> GetListStatus()
        {
            return Enum.GetNames(typeof(Status)).ToList();
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
            var language = await _repositoryManager.Language.GetCodeLanguage(lang.Code, false);
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
                flash = await _productBL.GetFlashProds(),
                specialProducts = await _productBL.GetSpecialsProd(),
                stores = await _userBL.GetStores()
            };
            return model;
        }
        public async Task<string> GetLogo()
        {
            var logo = await _repositoryManager.Setting.GetSettingByValue("website_logo");
            return await _imageBL.GetImageOriginal(logo.Value);
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
        public List<SliderDto> GetSliderMobile()
        {
            var sliders = _repositoryManager.Slider.GetSlidersForMobile()
                .Select(c => new SliderDto
                {
                    Title = lang.Code == "en" ? c.Title : c.TitleAr,
                    Decription = lang.Code == "en" ? c.Decription : c.DecriptionAr,
                    ImageId = Convert.ToInt32(_imageBL.GetImageMedium(c.ImgId.ToString()))
                }).ToList();
            return sliders;
        }
        public List<SliderDto> GetSliderWeb()
        {
            var sliders = _repositoryManager.Slider.GetSlidersForWeb()
                .Select(c => new SliderDto
                {
                    Title = lang.Code == "en" ? c.Title : c.TitleAr,
                    Decription = lang.Code == "en" ? c.Decription : c.DecriptionAr,
                    Url = c.Url,
                    ImageId = Convert.ToInt32(_imageBL.GetImageOriginal(c.ImgId.ToString()))
                }).ToList();
            return sliders;
        }
        public async Task AddSliderWeb(CreateSliderDto createSliderDto)
        {
            var slider = _mapper.Map<Sliders>(createSliderDto);
            slider.Type = SlidersImageType.Web;
            slider.VendorId =  1;
            _repositoryManager.Slider.AddSlider(slider);
            await _repositoryManager.SaveAsync();
        } 
        public async Task AddSliderMobile(CreateSliderDto createSliderDto)
        {
            var slider = _mapper.Map<Sliders>(createSliderDto);
            slider.Type = SlidersImageType.Mobile;
            slider.VendorId =  1;
            _repositoryManager.Slider.AddSlider(slider);
            await _repositoryManager.SaveAsync();
        }
        public async Task<BussnessResultModel> UpdateSlider( UpdateSliderDto updateDto)
        {
            var slider = await _repositoryManager.Slider.GetSlideById(updateDto.Id, true);
            if (slider == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
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
        public List<ServiceDto> GetServices()
        {
            var services = _repositoryManager.Services.GetAllServices(false)
                .Select(c => new ServiceDto
                {
                    Title = lang.Code == "en" ? c.Title : c.TitleAr,
                    Description = lang.Code == "en" ? c.Description : c.DescriptionAr,
                    // ImgId =  Convert.ToInt32(urlImg + x.Image.ImageSettings.FirstOrDefault(i => i.ImageType == ImageType.ACTUAL).Path)
                }).ToList();
            return services;
        }
        public async Task UpdateService( UpdateServiceDto update)
        {
            var service = await _repositoryManager.Services.GetServiceById(update.Id, true, false);
            _mapper.Map(update, service);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteService(int id)
        {
            var slider = await _repositoryManager.Services.GetServiceById(id,false, false);
            _repositoryManager.Services.DeleteService(slider);
            await _repositoryManager.SaveAsync();
        }
        //Contact------------------------------------------------
        public async Task<Contact> GetContact(int id)
        {
            return await _repositoryManager.Contact.GetContactById(id, false);
        }
        public async Task<List<ContactDto>> GetAllContacts(int rows, int pageId = 1)
        {
            var contacts = await _repositoryManager.Contact.GetContacts(rows ,pageId);
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
        public async Task UpdateTemplate( UpdateTemplateDto updateDto)
        {
            var template = await _repositoryManager.MessageTemplate.GetTemplateById(updateDto.Id, true);
            _mapper.Map(updateDto, template);
            await _repositoryManager.SaveAsync();
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
        public async Task<List<MailListDto>> GetMailLists(string search)
        {
            var emails = await _repositoryManager.MailList.GetMailListEmail(search);
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
        //public async Task<BussnessResultModel<List<LanguageDto>>> GetAllLanguagesk()
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
        public async Task<Language> GetLanguageCode(string code)
        {
           return await _repositoryManager.Language.GetCodeLanguage(code , false);
        } 
        public async Task<Language> GetLanguageId(int id)
        {
            return await _repositoryManager.Language.GetCodeLanguageId(id , false);
        }
        public async Task DeleteLanguage(int id)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(id, false);
            _repositoryManager.Language.DeleteLanguage(language);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateLanguage(UpdateLanguageDto updateDto)
        {
            var language = await _repositoryManager.Language.GetCodeLanguageId(updateDto.Id, true);
            _mapper.Map(updateDto, language);
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
            var currency = currencies.First();
            var currencyDto = currenciesDto.First();
            currencyDto.Name = lang.Code == "en" ? currency.Name : currency.NameAr;
            return currenciesDto;
        } 
        public async Task<CurrencyDto> GetCurrency(int id)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(id,false);
            var currencyDto = _mapper.Map<CurrencyDto>(currency);
            currencyDto.Name = lang.Code == "en" ? currency.Name : currency.NameAr;
            return currencyDto;
        } 
        public bool ExistCurrency(string code)
        {
           return _repositoryManager.Currency.ExistCurrency(code);
        }
        public async Task AddCurrency(CreateCurrencyDto createDto)
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
        }
        public async Task UpdatCurrency(UpdateCurrencyDto updateDto)
        {
            var currency = await _repositoryManager.Currency.GetCurrency(updateDto.Id, true);
            if(currency == null)
            {
                _mapper.Map(updateDto, currency);
                await _repositoryManager.SaveAsync();
            }
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
            if(notification == null)
            {
                _locService.GetLocalizedStringValue("correctLink");
            }
            _repositoryManager.Notification.DeleteNotification(notification);
            await _repositoryManager.SaveAsync();
        }
        public async Task AddNotification(Notification createNotificationDto)
        {
            //var notification = _mapper.Map<Notification>(createNotificationDto);
            //notification.IsRead = false;
            //notification.UserId = ;
            _repositoryManager.Notification.CreateNotification(createNotificationDto);
            await _repositoryManager.SaveAsync();
        }
        //setting------------------------------------------------------

        public async Task<Setting> GetSettingKey(string name)
        {
            return await _repositoryManager.Setting.GetSettingByValue(name);
        }
        public async Task<IEnumerable<Setting>> GetAllSettings()
        {
            return await _repositoryManager.Setting.GetAllSettings(false);
        }
        public async Task<IEnumerable<PageDto>> GetAllPages()
        {
            var pages = await _repositoryManager.StaticPages.GetAllPages(false);
            var pagesDto = _mapper.Map<List<PageDto>>(pages);
            var page = pages.First();
            var pageDto = pagesDto.First();
            pageDto.Title = lang.Code == "en" ? page.Title : page.TitleAr;
            pageDto.Description = lang.Code == "en" ? page.Description : page.DescriptionAr;
            return pagesDto;
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
        public async Task<PageDto> GetPage(int id)
        {
            var page = await _repositoryManager.StaticPages.GetPage(id,false);
            var pageDto = _mapper.Map<PageDto>(page);
            pageDto.Title = lang.Code == "en" ? page.Title : page.TitleAr;
            pageDto.Description = lang.Code == "en" ? page.Description : page.DescriptionAr;
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

    }
}