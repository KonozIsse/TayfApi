using AutoMapper;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BusinessLogic.Helpers;

namespace BusinessLogic
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
            //MapLanguage
            CreateMap<LanguageDto, Language>().ReverseMap();
            CreateMap<UpdateLanguageDto, Language>().ReverseMap();
            //MapRole
            CreateMap<RoleLinksDto, Permission>().ReverseMap();
            //MapNotification
            CreateMap<NotificationDto, Notification>().ReverseMap();
            CreateMap<CreateNotificationDto, Notification>().ReverseMap();
            //MapPermission
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
            CreateMap<ProductPageDto, Product>().ReverseMap();
            CreateMap<CategoriesProductDto, ProductCategory>().ReverseMap();
            CreateMap<ProductCategoryDto, ProductCategory>().ReverseMap();
            //MapProductSales
            CreateMap<SaleDto, ProductSales>().ReverseMap();
            CreateMap<CreateSaleDto, ProductSales>().ReverseMap();
            //MapSpecialProducts
            CreateMap<SpecialDto, SpecialProducts>().ReverseMap();
            CreateMap<CreateSpecialDto, SpecialProducts>().ReverseMap();
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
            CreateMap<ReviewDto, Review>().ReverseMap();
            CreateMap<CreateReviewDto, Review>().ReverseMap(); 
            CreateMap<UpdateReviewDto, Review>().ReverseMap();
            //MapWishList
            CreateMap<CreateLikeDto, WishList>().ReverseMap();
            CreateMap<WishListDto, WishList>().ReverseMap();
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
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<CustomerDto, User>().ReverseMap();
            CreateMap<UpdateCustomerDto, User>().ReverseMap();
            CreateMap<UserForRegistrationDto, User>().ReverseMap(); 
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
            CreateMap<SettingMedieDto, Setting>().ReverseMap();
            CreateMap<SettingDto, Setting>().ReverseMap(); 
            CreateMap<UpdateSettingDto, Setting>().ReverseMap();
            //MapService
            CreateMap<ServiceDto, Service>().ReverseMap(); 
            CreateMap<UpdateServiceDto, Service>().ReverseMap();
            //MapSliders
            CreateMap<SliderDto, Sliders>().ReverseMap(); 
            CreateMap<CreateSliderDto, Sliders>().ReverseMap();
            CreateMap<UpdateSliderDto, Sliders>().ReverseMap(); 
            CreateMap<PageDto, StaticPages>().ReverseMap();
            //MapBanner
            CreateMap<BannerDto, Banner>().ReverseMap(); 
            CreateMap<UpdateBannerDto, Banner>().ReverseMap();
            //MapImage
            CreateMap<ImageDto, Image>().ReverseMap();
            CreateMap<CreateImageDto, Image>().ReverseMap();
            CreateMap<CreateImageProductDto, Image>().ReverseMap();
            CreateMap<ImageProductDto, Image>().ReverseMap();
            //MapImageSetting
            CreateMap<ImageSettingDto, ImageSetting>().ReverseMap();
            CreateMap<CreateImageSettingDto, ImageSetting>().ReverseMap(); 
            CreateMap<UpdateImageSettingDto, ImageSetting>().ReverseMap();
            //MapDeliveryTime
            CreateMap<DeliveryTimeDto, DeliveryTime>().ReverseMap();
            //MapDevice
            CreateMap<DeviceDto, Device>().ReverseMap();
            CreateMap<CreateDeviceDto, Device>().ReverseMap();
            CreateMap<UpdateDeviceDto, Device>().ReverseMap();
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
