
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Exception;
using Entities.Models;
using Entities.Models.Enums;
using Entities.ViewModel;
using MailKit;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Web.Mvc;

namespace BusinessLogic.ApiClasses
{
    public class CartBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly LocService _locService;
        private readonly LocationTaxBL _locationTaxBL;
        private readonly Util _util;
        private readonly ImageBL _imageBL;
        private readonly ProductBL _productBL;
        public CartBL( IRepositoryManager repositoryManager, IMapper mapper, LocService locService, LocationTaxBL locationTaxBL, Util util,ImageBL imageBL, ProductBL productBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _locService = locService;
            _locationTaxBL = locationTaxBL;
            _util = util;
            _imageBL = imageBL;
            _productBL = productBL;
        }
        //Cart------------------------------------------------
      
        public async Task<decimal> GetTotalOrder (int customerId, int orderId )
        {
            decimal total = 0;
            if (orderId != 0)
            {
                var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(orderId);
                total = orderProducts.Sum(r => r.FinalPrice);
            }
            else
            {
                var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
                total = carts.Sum(r => r.FinalPrice);
            }
            return total;
        }
        public async Task<decimal> GetTotalCartsCustomer(int customerId)
        {
            decimal total = 0;
            var carts = await _repositoryManager.Cart.CartsNotActiveCustomer(customerId);
            if (carts.Count() > 0)
            {
                total = Convert.ToDecimal(carts.Sum(t => t.FinalPrice));
            }
            return total;
        }
        public async Task<decimal> GetTotalCartsStore (int storeId)
        {
            decimal total = 0;
            var carts = await _repositoryManager.Cart.GetCartsToStoreId(storeId);
            if (carts.Count() > 0)
            {
                total = Convert.ToDecimal(carts.Sum(t => t.FinalPrice));
            }
            return total;
        
        }
        public async Task<List<StoreDto>> GetAllStoresInCartsToCustomer(int userId)
        {
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(userId);
            var cartsStores = carts.GroupBy(x => x.StoreId).Select(x => x.First()).ToList();
            var model = new List<StoreDto>();
            foreach (var cart in cartsStores)
            {
                var store = await _repositoryManager.User.GetStoreId(cart.StoreId);
                model.Add(new StoreDto
                {
                    Id = store.Id,
                    FirstName = store.FirstName,
                    Image = _imageBL.GetImageMedium(store.ImageId.Value),
                    AdressInfo = store.AdressInfo,
                    CountCart = carts.Where(r => r.StoreId == store.Id).Count(),
                    TotalPrice = carts.Where(r => r.StoreId == store.Id).Count() > 0 ? carts.Where(r => r.StoreId == store.Id).Sum(r => r.FinalPrice) : 0
                });
            }
                //var grouped = carts.GroupBy(c => c.Store).Select(x => new
                //{
                //    x.Key.Id,
                //    x.Key.FirstName,
                //    x.Key.AdressInfo,
                //    ImageStore = _imageBL.GetImageMedium(x.Key.ImageId.Value),
                //    TotalPrice =  x.Sum(c => c.FinalPrice),
                //    CountCarts = x.Count(),
                //});
                //var storeDto = grouped.Select(c => new StoreDto
                //{
                //    Id = c.Id,
                //    FirstName = c.FirstName,
                //    Image = c.ImageStore,
                //    AdressInfo = c.AdressInfo,
                //    CountCart = c.CountCarts,
                //    TotalPrice = c.TotalPrice,
                //}).DistinctBy(c=>c.Id).ToList();

                return model;
        }
        public async Task ChangeActiveStatusCart(int id)
        {
            var cart = await _repositoryManager.Cart.GetCartId(id, true);
            cart.IsStatus = Status.Active;
            await _repositoryManager.SaveAsync();
        }
        public async Task<BussnessResultModel> AddedToCart(int customerId ,CreateCartDto createDto)
        {
            var product = await _repositoryManager.Product.GetActiveProductById(createDto.ProductId, true);
            if (product == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("NoProducts"), false);
            }
            var availableProduct = _productBL.AvailabilityProducts(createDto.ProductId);
            if (availableProduct <= createDto.Qty)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("notAv"), false);
            }
            var productPrice = product.Price;
            var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(createDto.ProductId);
            if (special != null)
            {
                productPrice = special.SpecialPrice;
            }
            var flash = await _repositoryManager.Sales.GetFlashProductId(createDto.ProductId);
            if (flash != null)
            {
                productPrice = flash.DiscountPrice;
            }
            var cart = await _repositoryManager.Cart.GetCartCustomerProduct(createDto.ProductId,customerId, true);
            if (cart != null && (availableProduct) < (cart.Qty + createDto.Qty))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("notAv"), false);
            }
            var attributes = await _repositoryManager.Attribute.GetAttributesProductId(createDto.ProductId);
            if (attributes != null)
            {
                if (createDto.CartAttributeProducts != null)
                {
                    if (createDto.CartAttributeProducts.Count() != attributes.Count())
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("plzchooseoption"), false);
                    }
                    else
                    {
                        foreach (var cartAttributDto in createDto.CartAttributeProducts)
                        {
                            var attribut = await _repositoryManager.Attribute.GetAttributeId(cartAttributDto.AttributesProductId, false);
                            var availableAttribute = _productBL.AvailabilityProductOption(createDto.ProductId, attribut.Id);
                            if (availableAttribute <= createDto.Qty)
                            {
                                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("notAvOp"), false);
                            }
                            if (attribut != null && attribut.AttributePrice != 0)
                            {
                                if (attribut.PricePrefix == "+")
                                {
                                    productPrice += attribut.AttributePrice;
                                }
                                if (attribut.PricePrefix == "-" && productPrice != 0)
                                {
                                    productPrice -= attribut.AttributePrice;
                                }
                            }
                        }
                    }
                }
                else
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("plzchooseoption"), false);
                }
            }
            if (cart == null)
            {
                var addCart = _mapper.Map<Cart>(createDto);
                addCart.IsStatus = Status.NotActive;
                addCart.CustomerId = customerId;
                addCart.ProdId = createDto.ProductId;
                addCart.StoreId = product.StoreId;
                addCart.FinalPrice = Convert.ToDecimal(productPrice * createDto.Qty);
                _repositoryManager.Cart.AddCart(addCart);
            }
            else
            {
                cart.StoreId =  product.StoreId;
                cart.ProdId = createDto.ProductId;
                createDto.Qty = cart.Qty + createDto.Qty;
                cart.FinalPrice = Convert.ToDecimal(productPrice * createDto.Qty);
               
                _mapper.Map(createDto, cart);
            }
          
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(cart, _locService.GetLocalizedStringValue("AddedToCart"));
        }
        public async Task<decimal> UpdateTotalCart(int customerId,int cartId,int qty)
        {
            var cart = await _repositoryManager.Cart.GetCartId(cartId, true);
            var product = await _repositoryManager.Product.GetActiveProductById(cart.ProdId, true);
            var productPrice = product.Price;
            var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(cart.ProdId);
            if (special != null)
            {
                productPrice = special.SpecialPrice;
            }
            var flash = await _repositoryManager.Sales.GetFlashProductId(cart.ProdId);
            if (flash != null)
            {
                productPrice = flash.DiscountPrice;
            }
            var cartAttributes = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cartId, true);
            if (cartAttributes != null)
            {
                foreach (var cartAttributDto in cartAttributes)
                {
                    var attributes = await _repositoryManager.Attribute.GetAttributesProductId(cart.ProdId);
                    var attribut = attributes.Where(c => c.Id == cartAttributDto.AttributesProductId).FirstOrDefault();
                    if (attribut != null && attribut.AttributePrice != 0)
                    {
                        if (attribut.PricePrefix == "+")
                        {
                            productPrice += attribut.AttributePrice;
                        }
                        if (attribut.PricePrefix == "-" && productPrice != 0)
                        {
                            productPrice -= attribut.AttributePrice;
                        }
                    }
                }
            }
           
            if (cart != null)
            {
                cart.CustomerId = customerId;
                cart.StoreId =  product.StoreId;
                cart.ProdId = product.Id;
                cart.Qty = cart.Qty + qty;
                cart.FinalPrice = Convert.ToDecimal(productPrice * cart.Qty);
                await _repositoryManager.SaveAsync();
            }
            return cart.FinalPrice;
        }
        public async Task<decimal> GetTotalCart(int storeId, int customerId, string code)
        {
            decimal total = 0;
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            var cart = carts.First();
            if (storeId != 0)
            {
                carts.Where(c => c.StoreId == storeId).ToList();
            }
            if (carts.Count() > 0)
            {
                foreach (var item in carts)
                {
                    total = total + Convert.ToDecimal(item.FinalPrice);
                }
            }
            if (code != null)
            {
                var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(code);
                if (coupon != null)
                {
                    if (coupon.DiscountType == DiscountType.CartDiscount)
                    {
                        if (total > coupon.CouponAmount)
                        {
                            total = total - Convert.ToDecimal(coupon.CouponAmount);
                        }
                    }
                    else if (coupon.DiscountType == DiscountType.CartPercentDiscount)
                    {
                        if (total > 0)
                        {
                            total = total - (total * Convert.ToDecimal(Convert.ToDecimal(coupon.CouponAmount) / 100));
                        }
                    }
                    else if (coupon.DiscountType == DiscountType.ProductDiscount)
                    {
                        total = 0;
                        foreach (var item in carts)
                        {
                            foreach(var ProductCoupon in coupon.ProductsCoupons)
                            {
                                var product = await _repositoryManager.Product.GetProductById(item.ProdId, false);
                                if (ProductCoupon.ProductId == product.Id)
                                {
                                    var newTotal = Convert.ToDecimal(product.Price) - Convert.ToDecimal(coupon.CouponAmount);
                                    total = total + newTotal;
                                }
                                else
                                {
                                    total = total + Convert.ToDecimal(product.Price);
                                }
                            }
                           
                        }
                    }
                    else if (coupon.DiscountType == DiscountType.ProductPercentDiscount)
                    {
                        total = 0;
                        foreach (var item in carts)
                        {
                            foreach (var ProductCoupon in coupon.ProductsCoupons)
                            {
                                var product = await _repositoryManager.Product.GetProductById(item.ProdId, false);
                                if (ProductCoupon.ProductId == product.Id)
                                {
                                    decimal newval = Convert.ToDecimal(product.Price) * Convert.ToDecimal(Convert.ToDecimal(coupon.CouponAmount) / 100);
                                    total = total + newval;
                                }
                                else
                                {
                                    total = total + Convert.ToDecimal(product.Price);
                                 }
                            }
                           
                        }
                    }
                }
            }
            decimal tax = await _locationTaxBL.GetTax(customerId);
            if (tax != 0)
            {
                total = total + ((total * tax) / 100);
            }
            return total;
        }
        public async Task<string> GetValueCodeCoupon(int customerId, string code, Currency currency)
        {
            string tot = "0";
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(code);
            if (coupon == null)
            {
                return tot;
            }
            else
            {
                if (coupon.DiscountType == DiscountType.CartDiscount)
                {
                    tot = currency.Symbol + " " + Convert.ToDecimal(coupon.CouponAmount);
                    return tot;
                }
                else if (coupon.DiscountType == DiscountType.CartPercentDiscount)
                {
                    tot = "%" + coupon.CouponAmount;
                    return tot;
                }
                else if (coupon.DiscountType == DiscountType.ProductDiscount)
                {
                    bool isEx = false;
                    foreach (var cart in carts)
                    {
                        foreach(var product in coupon.ProductsCoupons)
                        {
                            if (product.ProductId == cart.ProdId)
                            {
                                isEx = true;
                            }
                        }
                    }

                    if (isEx)
                    {
                        tot = currency.Symbol + " " + Convert.ToDecimal(coupon.CouponAmount);
                        return tot;
                    }
                    else
                    {

                    }
                }
                else if (coupon.DiscountType == DiscountType.ProductPercentDiscount)
                {
                    bool isEx = false;
                    foreach (var cart in carts)
                    {
                        foreach (var product in coupon.ProductsCoupons)
                        {
                            if (product.ProductId == cart.ProdId)
                            {
                                isEx = true;
                            }
                        }
                    }

                    if (isEx)
                    {
                        tot = "%" + coupon.CouponAmount;
                        return tot;
                    }
                }
            }
            return tot;
        }
        public async Task<decimal> AvailableAmountForCart(int cartId, int customerId)
        {
            int availableInventory = 0;
            var instock = 0;
            var outstock = 0;
            var cart = await _repositoryManager.Cart.GetCartId(cartId, false);
            cart.Id = cartId;
            cart.CustomerId = customerId;
            var cartAttributeProducts = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cartId,false);
            if (cartAttributeProducts != null)
            {
                var productAttribut = cartAttributeProducts.First().AttributesProductId;
                var attribute = await _repositoryManager.Attribute.GetAttributeIdProductId(productAttribut ,cart.ProdId);
                if (attribute != null)
                {
                    var inventories = await _repositoryManager.Inventory.GetAllInventoryByProductIdOption(cart.ProdId, attribute.Id);
                    foreach (var inventory in inventories)
                    {
                        if (inventory.StockType == "in")
                        {
                            instock = instock + inventory.Stock;
                        }
                        if (inventory.StockType == "out")
                        {
                            outstock = outstock + inventory.Stock;
                        }
                    }
                    availableInventory = instock - outstock;
                    if (cart != null && availableInventory < cart.Qty)
                    {
                        return -1;
                    }
                    var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(cart.StoreId, customerId);
                    if (order != null)
                    {
                        var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(cart.ProdId, order.Id , true);
                        if (orderProduct != null)
                        {
                            if (orderProduct.Qty > availableInventory)
                            {
                                return -1;
                            }
                        }
                    }
                   
                } 
                return availableInventory;
            }
            
            else
            {
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(cart.ProdId);
                if (inventories != null && inventories.Count() > 0)
                {
                    foreach (var inventory in inventories)
                    {
                        if (inventory.StockType == "in")
                        {
                            instock = instock + inventory.Stock;
                        }
                        if (inventory.StockType == "out")
                        {
                            outstock = outstock + inventory.Stock;
                        }
                    }
                    availableInventory = instock - outstock;
                    if (cart != null && availableInventory < cart.Qty)
                    {
                        return -1;
                    }
                    var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(cart.StoreId, customerId);
                    if (order != null)
                    {
                        var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(cart.ProdId, order.Id,true);
                        if (orderProduct != null)
                        {
                            if (orderProduct.Qty > availableInventory)
                            {
                                return -1;
                            }
                        }
                    }
                    return availableInventory;
                }
                else
                {
                    return -1;
                }
            }
        }
        public async Task<BussnessResultModel> DeleteCartCustomerStore(int customerId , int storeId)
        {
            var carts = await _repositoryManager.Cart.GetCartsToStoreCustomer(storeId, customerId);
            if (carts == null)
            {
                return new BussnessResultModel(null, "Please make sure the link is correct", false);
            }
            foreach(var cart in carts)
            {
                var cartAttributes = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cart.Id, false);
                if (cartAttributes != null)
                {
                    foreach (var cartAttribut in cartAttributes)
                    {
                        _repositoryManager.CartAttributeProduct.DeleteCartAttributeProduct(cartAttribut);
                    }
                }
                _repositoryManager.Cart.DeleteCart(cart);
                await _repositoryManager.SaveAsync();
            }
          
            return new BussnessResultModel(carts , _locService.GetLocalizedStringValue("successDelete"));
        } 
        public async Task<BussnessResultModel> DeleteCart(int cartId)
        {
            var cart = await _repositoryManager.Cart.GetCartId(cartId, false);
            if (cart == null)
            {
                return new BussnessResultModel(null, "Please make sure the link is correct", false);
            }
            var cartAttributes = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cartId,false);
            if (cartAttributes != null)
            {
                foreach (var cartAttribut in cartAttributes)
                {
                    _repositoryManager.CartAttributeProduct.DeleteCartAttributeProduct(cartAttribut);
                }
            }
            _repositoryManager.Cart.DeleteCart(cart);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(cart , _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<List<CartVM>> GetCarts (int userId)
        {
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(userId);
            var cartVM = new List<CartVM>();
            foreach (var cart in carts)
            {
                var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(cart.ProdId);
                var flash = await _repositoryManager.Sales.GetFlashProductId(cart.ProdId);
                var images = await _repositoryManager.ImageProduct.GetAllImagesProduct(cart.ProdId,false,true);
                    cartVM.Add(new CartVM
                    {
                        Id = cart.Id,
                        Qty = cart.Qty,
                        StoreId = cart.StoreId,
                        FinalPrice = cart.FinalPrice,
                        Attributes = null?? _productBL.GetAttributsProducts(cart.ProdId).Result,
                        ShareLink = _util.url1 + "/share.html?id=" + cart.ProdId,
                        ProductId = cart.ProdId,
                        ProductName = cart.Product.ProductName,
                        ProductImage = _imageBL.GetImageOriginal(images.First().ImageId),
                        IsFeature = cart.Product.IsFeature,
                        SpecialPrice = special == null ? 0 : special.SpecialPrice,
                        StoreName = cart.Store.FirstName,
                        IsSpecial = (special == null ? false : true),
                        ProductDescription = cart.Product.Description,
                        CreatedAt = cart.Product.CreatedAt.ToString(),
                        UpdatedAt = cart.Product.UpdatedAt.ToString() ?? null,
                        ProductModel = cart.Product.ProductModel,
                        ProductPrice = (flash != null ? flash.DiscountPrice : cart.Product.Price),
                        ProductStatus = Convert.ToInt16(cart.Product.IsStatus),
                        TotaLTax =   _locationTaxBL.GetTax(userId).Result
                    }) ;
                    
                }
            
            return cartVM;
        }
        public async Task<CheckoutVM> CheckoutCart(int storeId, int CustomerId, Currency code, string coupon = null)
        {
            var usr = await _repositoryManager.User.GetActiveUserId(CustomerId, false);
            var model = new CheckoutVM
            {
                FirstName = usr.FirstName,
                LastName = usr.LastName,
                Phone = usr.PhoneNumber,
                DisCount = GetValueCodeCoupon(CustomerId, coupon, code).Result ?? null,
                Payment = await _repositoryManager.PaymentMethods.GetPaymentMethods("") ?? null,
                Countries = await _locationTaxBL.GetAllCountries() ?? null,
                Cart = await GetCarts(CustomerId) ?? null,
                Tax = await _locationTaxBL.GetTax(CustomerId) == 0 ? 0 : await _locationTaxBL.GetTax(CustomerId),
                Times = await _repositoryManager.DeliveryTime.GetAllDeliveryTimes() ?? null,
                Coupon = coupon ?? null,
                Total = await GetTotalCart(storeId, CustomerId, coupon) == 0 ? 0 : await GetTotalCart(storeId, CustomerId, coupon),
                Address = await _locationTaxBL.GetDefaultAddress(CustomerId),
            };
            return model;
        }
        
    }
}
