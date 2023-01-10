using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        private IUserRepository _userRepository;
        private IBannerRepository _bannerRepository;
        private INotificationRepository _notificationRepository;
        private INotificationActionRepository _notificationActionRepository;
        private IAddressRepository _addressRepository;
        private IProductAttributRepository _attributesProductRepository;
        private ICategoriesRepository _categoriesRepository; 
        private IContactRepository _contactRepository;
        private ICountryRepository _countryRepository;
        private ICurrencyRepository _currencyRepository;
        private ICouponRepository _couponRepository;
        private IImageRepository _imageRepository;
        private IInventoryRepository _inventoryRepository;
        private ILanguageRepository _languageRepository;
        private INewsRepository _newsRepository;  
        private IOrderRepository _orderRepository;
        private IOrderAttributeProductRepository _orderAttributesProductsRepository;
        private ISettingRepository _settingRepository;
        private IProductRepository _productRepository; 
        private IProductTypeRepository _productTypeRepository;
        private IProductOptionRepository _productOptionRepository;
        private IProductOptionValueRepository _productOptionValueRepository;
        private ICartRepository _cartRepository;
        private ICartAttributeProductRepository _cartAttributeProductRepository;
        private IMailListRepository _mailListRepository;
        private IDeliveryTimeRepository _deliveryTimeRepository;
        private IMessageTemplateRepository _messageTemplateRepository;
        private IStaticPagesRepository _staticPagesRepository;
        private IServicesRepository _servicesRepository;
        private ISliderRepository _sliderRepository;
        private ISpecialProductsRepository _specialProductsRepository;
        private IOrderStatusRepository _orderStatusRepository;
        private ICommentNewsRepository _commentNewsRepository;
        private ITaxClassRepository _taxClassRepository ;
        private ITaxRateRepository _taxRateRepository ;
        private ISalesRepository _salesRepository;
        private IPermissionRepository _permissionRepository;
        private IPaymentMethodsRepository _paymentMethodsRepository;
        private ILinkRepository _linkRepository;
        private IOrderProductsRepository _orderProductsRepository;
        private IImageSettingRepository _imageSettingRepository;
        private IWishListRepository _wishListRepository;
        private IDeviceRepository _deviceRepository;
        private IReviewRepository _reviewRepository;
        private IZoneRepository _zoneRepository; 
        private IUnitRepository _unitRepository;    
        private IRoleRepository _roleRepository;
        private IProductCategoryRepository _productCategoryRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }
        public IUserRepository User
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(_repositoryContext);
                return _userRepository;
            }
        }
        public IBannerRepository Banner
        {
            get
            {
                if (_bannerRepository == null)
                    _bannerRepository = new BannerRepository(_repositoryContext);
                return _bannerRepository;
            }
        }
        public INotificationRepository Notification
        {
            get
            {
                if (_notificationRepository == null)
                    _notificationRepository = new NotificationRepository(_repositoryContext);
                return _notificationRepository;
            }
        }
        public INotificationActionRepository NotificationAction
        {
            get
            {
                if (_notificationActionRepository == null)
                    _notificationActionRepository = new NotificationActionRepository(_repositoryContext);
                return _notificationActionRepository;
            }
        }
        public IAddressRepository Address
        {
            get
            {
                if (_addressRepository == null)
                    _addressRepository = new AddressRepository(_repositoryContext);
                return _addressRepository;
            }
        }
        public IProductAttributRepository Attribute
        {
            get
            {
                if (_attributesProductRepository == null)
                    _attributesProductRepository = new ProductAttributRepository(_repositoryContext);
                return _attributesProductRepository;
            }
        }
        public ICategoriesRepository Categories
        {
            get
            {
                if (_categoriesRepository == null)
                    _categoriesRepository = new CategoriesRepository(_repositoryContext);
                return _categoriesRepository;
            }
        }
        public IContactRepository Contact
        {
            get
            {
                if (_contactRepository == null)
                    _contactRepository = new ContactRepository(_repositoryContext);
                return _contactRepository;
            }
        }
        public ICountryRepository Country
        {
            get
            {
                if (_countryRepository == null)
                    _countryRepository = new CountryRepository(_repositoryContext);
                return _countryRepository;
            }
        }
        public ICurrencyRepository Currency
        {
            get
            {
                if (_currencyRepository == null)
                    _currencyRepository = new CurrencyRepository(_repositoryContext);
                return _currencyRepository;
            }
        }
        public ICouponRepository Coupon
        {
            get
            {
                if (_couponRepository == null)
                    _couponRepository = new CouponRepository(_repositoryContext);
                return _couponRepository;
            }
        }
        public ICartRepository Cart
        {
            get
            {
                if (_cartRepository == null)
                    _cartRepository = new CartRepository(_repositoryContext);
                return _cartRepository;
            }
        }
        public ICartAttributeProductRepository CartAttributeProduct
        {
            get
            {
                if (_cartAttributeProductRepository == null)
                    _cartAttributeProductRepository = new CartAttributeProductRepository(_repositoryContext);
                return _cartAttributeProductRepository;
            }
        }
        public IImageRepository Image
        {
            get
            {
                if (_imageRepository == null)
                    _imageRepository = new ImageRepository(_repositoryContext);
                return _imageRepository;
            }
        }
        public IInventoryRepository Inventory
        {
            get
            {
                if (_inventoryRepository == null)
                    _inventoryRepository = new InventoryRepository(_repositoryContext);
                return _inventoryRepository;
            }
        }
        public ILanguageRepository Language
        {
            get
            {
                if (_languageRepository == null)
                    _languageRepository = new LanguageRepository(_repositoryContext);
                return _languageRepository;
            }
        }
        public INewsRepository News
        {
            get
            {
                if (_newsRepository == null)
                    _newsRepository = new NewsRepository(_repositoryContext);
                return _newsRepository;
            }
        }
        public IOrderRepository Order
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new OrderRepository(_repositoryContext);
                return _orderRepository;
            }
        }
        public IOrderAttributeProductRepository OrderAttributesProducts
        {
            get
            {
                if (_orderAttributesProductsRepository == null)
                    _orderAttributesProductsRepository = new OrderAttributeProductRepository(_repositoryContext);
                return _orderAttributesProductsRepository;
            }
        }
        public ISettingRepository Setting
        {
            get
            {
                if (_settingRepository == null)
                    _settingRepository = new SettingRepository(_repositoryContext);
                return _settingRepository;
            }
        }
        public IProductRepository Product
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new ProductRepository(_repositoryContext);
                return _productRepository;
            }
        }
        public IMailListRepository MailList
        {
            get
            {
                if (_mailListRepository == null)
                    _mailListRepository = new MailListRepository(_repositoryContext);
                return _mailListRepository;
            }
        }
        public IDeliveryTimeRepository DeliveryTime
        {
            get
            {
                if (_deliveryTimeRepository == null)
                    _deliveryTimeRepository = new DeliveryTimeRepository(_repositoryContext);
                return _deliveryTimeRepository;
            }
        }
        public IMessageTemplateRepository MessageTemplate
        {
            get
            {
                if (_messageTemplateRepository == null)
                    _messageTemplateRepository = new MessageTemplateRepository(_repositoryContext);
                return _messageTemplateRepository;
            }
        }
        public IStaticPagesRepository StaticPages
        {
            get
            {
                if (_staticPagesRepository == null)
                    _staticPagesRepository = new StaticPagesRepository(_repositoryContext);
                return _staticPagesRepository;
            }
        }
        public IServicesRepository Services
        {
            get
            {
                if (_servicesRepository == null)
                    _servicesRepository = new ServicesRepository(_repositoryContext);
                return _servicesRepository;
            }
        }
        public ISliderRepository Slider
        {
            get
            {
                if (_sliderRepository == null)
                    _sliderRepository = new SliderRepository(_repositoryContext);
                return _sliderRepository;
            }
        }
        public ISpecialProductsRepository SpecialProducts
        {
            get
            {
                if (_specialProductsRepository == null)
                    _specialProductsRepository = new SpecialProductsRepository(_repositoryContext);
                return _specialProductsRepository;
            }
        }
        public IOrderStatusRepository OrderStatus
        {
            get
            {
                if (_orderStatusRepository == null)
                    _orderStatusRepository = new OrderStatusRepository(_repositoryContext);
                return _orderStatusRepository;
            }
        }
        public ICommentNewsRepository CommentNews
        { 
            get
            {
                if (_commentNewsRepository == null)
                    _commentNewsRepository = new CommentNewsRepository(_repositoryContext);
                return _commentNewsRepository;
            }
        }
       
        public ITaxClassRepository TaxClass
        {
            get
            {
                if (_taxClassRepository == null)
                    _taxClassRepository = new TaxClassRepository(_repositoryContext);
                return _taxClassRepository;
            }
        }
        public ITaxRateRepository TaxRate
        {
            get
            {
                if (_taxRateRepository == null)
                    _taxRateRepository = new TaxRateRepository(_repositoryContext);
                return _taxRateRepository;
            }
        }
        public ISalesRepository Sales
        {
            get
            {
                if (_salesRepository == null)
                    _salesRepository = new SalesRepository(_repositoryContext);
                return _salesRepository;
            }
        }
        public IPermissionRepository Permission
        {
            get
            {
                if (_permissionRepository == null)
                    _permissionRepository = new PermissionRepository(_repositoryContext);
                return _permissionRepository;
            }
        }
        public IPaymentMethodsRepository PaymentMethods
        {
            get
            {
                if (_paymentMethodsRepository == null)
                    _paymentMethodsRepository = new PaymentMethodsRepository(_repositoryContext);
                return _paymentMethodsRepository;
            }
        }
        public ILinkRepository Link
        {
            get
            {
                if (_linkRepository == null)
                    _linkRepository = new LinkRepository(_repositoryContext);
                return _linkRepository;
            }
        }
        public IImageSettingRepository ImageSetting
        {
            get
            {
                if (_imageSettingRepository == null)
                    _imageSettingRepository = new ImageSettingRepository(_repositoryContext);
                return _imageSettingRepository;
            }
        }
        public IOrderProductsRepository OrderProducts
        {
            get
            {
                if (_orderProductsRepository == null)
                    _orderProductsRepository = new OrderProductsRepository(_repositoryContext);
                return _orderProductsRepository;
            }
        }
        public IWishListRepository WishList
        {
            get
            {
                if (_wishListRepository == null)
                    _wishListRepository = new WishListRepository(_repositoryContext);
                return _wishListRepository;
            }
        }
        public IDeviceRepository Device
        {
            get
            {
                if (_deviceRepository == null)
                    _deviceRepository = new DeviceRepository(_repositoryContext);
                return _deviceRepository;
            }
        }
        public IReviewRepository Review
        {
            get
            {
                if (_reviewRepository == null)
                    _reviewRepository = new ReviewRepository(_repositoryContext);
                return _reviewRepository;
            }
        }
        public IZoneRepository Zone
        {
            get
            {
                if (_zoneRepository == null)
                    _zoneRepository = new ZoneRepository(_repositoryContext);
                return _zoneRepository;
            }
        }
        public IProductOptionRepository Option
        {
            get
            {
                if (_productOptionRepository == null)
                    _productOptionRepository = new ProductOptionRepository(_repositoryContext);
                return _productOptionRepository;
            }
        }
        public IProductOptionValueRepository Value
        {
            get
            {
                if (_productOptionValueRepository == null)
                    _productOptionValueRepository = new ProductOptionValueRepository(_repositoryContext);
                return _productOptionValueRepository;
            }
        }
        public IUnitRepository Unit
        {
            get
            {
                if (_unitRepository == null)
                    _unitRepository = new UnitRepository(_repositoryContext);
                return _unitRepository;
            }
        }
        public IProductTypeRepository ProductType

        {
            get
            {
                if (_productTypeRepository == null)
                    _productTypeRepository = new ProductTypeRepository(_repositoryContext);
                return _productTypeRepository;
            }
        } 

        public IRoleRepository Role 
        {
            get
            {
                if (_roleRepository == null)
                    _roleRepository = new RoleRepository(_repositoryContext);
                return _roleRepository;
            }
        }

        public IProductCategoryRepository ProductCategory
        {
            get
            {
                if (_productCategoryRepository == null)
                    _productCategoryRepository = new ProductCategoryRepository(_repositoryContext);
                return _productCategoryRepository;
            }
        }

        public Task SaveAsync() => _repositoryContext.SaveChangesAsync();
    }
}
