using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IAddressRepository Address { get; }
        IBannerRepository Banner { get; }
        ICartRepository Cart { get; }
        ICartProductRepository CartProduct { get; }
        ICartAttributeProductRepository CartAttributeProduct { get; }
        ICategoriesRepository Categories { get; }
        IContactRepository Contact { get; }
        ICountryRepository Country { get; }
        ICouponRepository Coupon { get; }
        ICurrencyRepository Currency { get; }
        ICommentNewsRepository CommentNews { get; }
        ICustomerStoresRepository CustomerStore { get; }
        ICustomerProductRepository CustomerProduct { get; }
        IImageRepository Image { get; }
        IInventoryRepository Inventory { get; }
        ILanguageRepository Language { get; }
        ILinkRepository Link { get; }
        IImageSettingRepository ImageSetting { get; }
        INewsRepository News { get; }
        INotificationRepository Notification { get; }
        INotificationActionRepository NotificationAction { get; }
        IMailListRepository MailList { get; }
        IMessageTemplateRepository MessageTemplate { get; }
        IDeliveryTimeRepository DeliveryTime { get; }
        IDeviceRepository Device { get; }
        ISalesRepository Sales { get; }
        ISettingRepository Setting { get; }
        IStaticPagesRepository StaticPages { get; }
        IServicesRepository Services { get; }
        ISpecialProductsRepository SpecialProducts { get; }
        ISliderRepository Slider { get; }
        IOrderRepository Order { get; }
        IOrderStatusRepository OrderStatus { get; }
        IOrderProductsRepository OrderProducts { get; }
        IOrderAttributeProductRepository OrderAttributesProducts { get; }
        ICustomerAttributesProductRepository CustomerAttributesProduct { get; }
        ITaxClassRepository TaxClass { get; }
        ITaxRateRepository TaxRate { get; }
        IProductRepository Product { get; }
        IProductTypeRepository ProductType { get; }
        IProductAttributRepository Attribute { get; }
        IProductOptionRepository Option { get; }
        IProductOptionValueRepository Value { get; }
        IPermissionRepository Permission { get; }
        IPaymentMethodsRepository PaymentMethods { get; }
        IWishListRepository WishList { get; }
        IReviewRepository Review { get; }
        IUserRepository User { get; }
        IUnitRepository Unit { get; }
        IZoneRepository Zone { get; }
        Task SaveAsync();
    }
}
