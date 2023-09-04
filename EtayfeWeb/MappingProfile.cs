using AutoMapper;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using EtayfeWeb.Helpers;
using System.Linq.Expressions;

namespace EtayfeWeb
{
    public class MappingProfile : Profile
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public MappingProfile(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            //MapContact
            CreateMap<ContactDto, Contact>().ReverseMap();
            CreateMap<CreateContactDto, Contact>().ReverseMap();
            //MapLink
            CreateMap<LinkDto, Link>().ReverseMap();
            //MapLanguage
            CreateMap<Language, LanguageDto>().ReverseMap();
            CreateMap<UpdateLanguageDto, Language>().ReverseMap();
            //MapPaymentMethods
            CreateMap<PaymentDto, PaymentMethods>().ReverseMap();
            //MapRole
            CreateMap<RoleLinksDto, Permission>().ReverseMap();
            //MapNotification
            CreateMap<NotificationDto, Notification>().ReverseMap();
            CreateMap<CreateNotificationDto, Notification>().ReverseMap();
            //MapRole
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<CreateRoleDto, Role>().ReverseMap(); 
            CreateMap<UpdateRoleDto, Role>().ReverseMap();
            //MapCurrency
            CreateMap<CurrencyDto, Currency>().ReverseMap();
            CreateMap<CreateCurrencyDto, Currency>().ReverseMap();
            CreateMap<UpdateCurrencyDto, Currency>().ReverseMap();
            //MapCart
            CreateMap<CartDto, Cart>().ReverseMap();
            CreateMap<CreateCartDto, Cart>().ReverseMap();
            //MapCartAttributeProduct
            CreateMap<CartAttributeProductDto, CartAttributeProduct>().ReverseMap();
            //MapCoupon
            CreateMap<CreateCouponDto, Coupon>().ReverseMap();
            CreateMap<CouponDto, Coupon>().ReverseMap(); 
            CreateMap<UpdateCouponDto, Coupon>().ReverseMap();
            //ProductsCoupon
            CreateMap<ProductsCouponDto, ProductsCoupon>().ReverseMap();
            //MapCategory
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<MainCategoryDto, Category>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();
            //MapCountry
            CreateMap<CountryDto, Country>().ReverseMap();
            CreateMap<CreateCountryDto, Country>().ReverseMap(); 
            CreateMap<UpdateCountryDto, Country>().ReverseMap();
            //MapZone
            CreateMap<ZoneDto, Zone>().ReverseMap();
            CreateMap<CreateZoneDto, Zone>().ReverseMap();
            CreateMap<UpdateZoneDto, Zone>().ReverseMap();
            CreateMap<UpdateZoneDto, ZoneDto>().ReverseMap();
            //MapNews
            CreateMap<NewsDto, News>().ReverseMap();
            CreateMap<CreateNewsDto, News>().ReverseMap();
            CreateMap<UpdateNewsDto, News>().ReverseMap();
            //MapCommentNews
            CreateMap<CreateCommentDto, CommentNews>().ReverseMap();
            CreateMap<CommentsDto, CommentNews>().ReverseMap();
            //MapProduct
            CreateMap<ProductDto, Product>().ReverseMap();
            CreateMap<CreateProductDto, Product>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();
            //MapProductCategory
            CreateMap<CreateProductCategoryDto, ProductCategory>().ReverseMap();
            CreateMap<ProductCategoryDto, ProductCategory>().ReverseMap();
            //MapProductSales
            CreateMap<SaleDto, ProductSales>().ReverseMap();
            //MapSpecialProducts
            CreateMap<SpecialDto, SpecialProducts>().ReverseMap();
            //MapAttribute
            CreateMap<AttributeDto, ProductAttribut>().ReverseMap();
            CreateMap<CreateAttributeDto, ProductAttribut>().ReverseMap();
            CreateMap<UpdateAttributeDto, ProductAttribut>().ReverseMap();
            //MapOption
            CreateMap<OptionDto, ProductOption>().ReverseMap();
            CreateMap<CreateOptionDto, ProductOption>().ReverseMap(); 
            CreateMap<UpdateOptionDto, ProductOption>().ReverseMap();
            //MapValue
            CreateMap<ValueDto, ProductOptionValue>().ReverseMap();
            CreateMap<CreateValueDto, ProductOptionValue>().ReverseMap();
            CreateMap<UpdateValueDto, ProductOptionValue>().ReverseMap();
            //MapAddress
            CreateMap<AddressDto, Address>().ReverseMap();
            CreateMap<CreateAddressDto, Address>().ReverseMap();
            CreateMap<UpdateAddressDto, Address>().ReverseMap();
            //MapReview
            CreateMap<Review, ReviewDto>().AfterMap((s, d) => {
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
                 d.CustomerImage = baseUrl + "/media_files/avatars/" + s.Customer.Avater;
            });
            CreateMap<CreateReviewDto, Review>().ReverseMap(); 
           //MapOrder
            CreateMap<CreateOrderDto, Order>().ReverseMap();
            CreateMap<UpdateOderDto, Order>().ReverseMap();
            CreateMap<OrderDto, Order>().ReverseMap();
            //MapOrderProduct
            CreateMap<OrderProductDto, OrderProduct>().ReverseMap();
            //MapStore
            CreateMap<StoreDto, User>().ReverseMap();
            CreateMap<CreateStoreDto, User>().ReverseMap();
            CreateMap<UpdateStoreDto, User>().ReverseMap();  
            CreateMap<ResetPasswordDto, User>().ReverseMap(); 
            //MapCustomer
            CreateMap<CreateCustomerDto, User>().ReverseMap(); 
            CreateMap<CreateCustomerCPDto, User>().ReverseMap();
            CreateMap<User, CustomerDto>().AfterMap((s, d) => {
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
                d.Avater = baseUrl + "/media_files/avatars/" + s.Avater;
            });
            CreateMap<UpdateCustomerDto, User>().ReverseMap();
            //AdminMap
            CreateMap<AdminDto, User>().ReverseMap();
            CreateMap<CreateAdminDto, User>().ReverseMap();
            CreateMap<UpdateAdminDto, User>().ReverseMap();
            //MapTaxRate
            CreateMap<TaxRateDto, TaxRate>().ReverseMap();
            CreateMap<CreateTaxRateDto, TaxRate>().ReverseMap();
            CreateMap<UpdateTaxRateDto, TaxRate>().ReverseMap();
            //MapTaxClass
            CreateMap<TaxClassDto, TaxClass>().ReverseMap();
            CreateMap<CreateTaxClassDto, TaxClass>().ReverseMap();
            CreateMap<UpdateTaxClassDto, TaxClass>().ReverseMap();
            //MapSetting
            CreateMap<SettingDto, Setting>().ReverseMap(); 
            CreateMap<UpdateSettingDto, Setting>().ReverseMap();
            //MapService
            CreateMap<ServiceDto, Service>().ReverseMap(); 
            CreateMap<UpdateServiceDto, Service>().ReverseMap();
            //MapSliders
            CreateMap<SliderDto, Sliders>().ReverseMap(); 
            CreateMap<CreateSliderDto, Sliders>().ReverseMap();
            CreateMap<UpdateSliderDto, Sliders>().ReverseMap();
            //Page
            CreateMap<PageDto, StaticPages>().ReverseMap();
            //MapBanner
            CreateMap<BannerDto, Banner>().ReverseMap(); 
            CreateMap<UpdateBannerDto, Banner>().ReverseMap();
            //MapImage
            CreateMap<Image, ImageDto>().AfterMap((s, d) => {
                d.Name = "/media_files/original/" + s.Name;
            });
            CreateMap<CreateImageDto, Image>().ReverseMap();
            //ImageProduct
            CreateMap<CreateImageProductDto, ProductImage>().ReverseMap();
            CreateMap<ProductImagesDto, ProductImage>().ReverseMap();
            //MapImageSetting
            CreateMap<ImageSetting, ImageSettingDto>().AfterMap((s, d) => {
                d.Path = "/media_files" + s.Path;
            });
            CreateMap<CreateImageSettingDto, ImageSetting>().ReverseMap();
            CreateMap<UpdateImageSettingDto, ImageSetting>().ReverseMap();
            //MapDeliveryTime
            CreateMap<DeliveryTimeDto, DeliveryTime>().ReverseMap();
            //MapMailList
            CreateMap<MailListDto, MailList>().ReverseMap();
            CreateMap<SendMailListDto, MailList>().ReverseMap();
            //MapMessageTemplate
            CreateMap<MessageTemplateDto, MessageTemplate>().ReverseMap();
            CreateMap<UpdateTemplateDto, MessageTemplate>().ReverseMap();
            //MapInventory
            CreateMap<InventoryDto, Inventory>().ReverseMap();
            CreateMap<CreateInventoryDto, Inventory>().ReverseMap();
            //MapOrderStatus
            CreateMap<UpdateOrderStatusDto, OrderStatus>().ReverseMap();
            CreateMap<OrderStatusDto, OrderStatus>().ReverseMap();


            ForAllMaps((typeMap, m) =>
            {
                Expression<Action<object, object>> afterFunction = (sourceObj, destObj) => FillEnumDesc(destObj);
                typeMap.AddAfterMapAction(afterFunction);
            });
        }
      
        private void FillEnumDesc(object destObj)
        {
            var hasEnum = destObj.GetType().GetProperties().Any(x => x.IsDefined(typeof(EnumBindAttribute), false));
            if (hasEnum)
            {
                EnumHelper.FillEnumDesc(destObj);
            }
        }
    }
}
