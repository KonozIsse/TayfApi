
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ApiClasses
{
    public class CartBL
    {
        private ProductBL _productBl;
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly LocService _locService;
        private readonly LocationTaxBL _locationTaxBL;

        public CartBL( ProductBL productBl, IRepositoryManager repositoryManager, IMapper mapper, LocService locService, LocationTaxBL locationTaxBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _productBl = productBl;
            _locService = locService;
            _locationTaxBL = locationTaxBL;
        }
        //Cart------------------------------------------------
        public async Task<List<CartDto>> GetCartByStore(int customerId)
        {
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            var cartsDto = _mapper.Map<List<CartDto>>(carts);
            var cartDto = cartsDto.First();
            foreach (var cart in carts)
                if (cart.CartStores != null)
                {
                    var voteGrouped = cart.CartStores.GroupBy(x => x.Store)
                       .Select(x => new
                       {
                           x.Key.FirstName,
                           PriceCount = x.Sum(c => c.FinalPrice)
                       });
                    cartDto.StoreGrouped = voteGrouped;
                }
            return cartsDto;
        }
        public int CartProductInCartCount()
        {
            return _repositoryManager.CartProduct.CartCount();
        }
        public async Task<List<Cart>> CartCount(int customerId)
        {
            return await _repositoryManager.Cart.CartsNotActiveCustomer(customerId);
        }
        public async Task<List<CustomerProduct>> GetProductsCustomerId(int customerId)
        {
            return await _repositoryManager.CustomerProduct.GetProductsCustomerId(customerId);
        }

        public async Task ChangeActiveStatusCart(int id)
        {
            var cart = await _repositoryManager.Cart.GetCartId(id, true);
            cart.IsStatus = Status.Active;
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateCart(int id, UpdateCartDto updateCartDto)
        {
            var cart = await _repositoryManager.Cart.GetCartId(id, true);
            _mapper.Map(updateCartDto, cart);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteCartAttributeProduct(int id, int cartAttributeProductId)
        {
            var cartProduct = await _repositoryManager.CartProduct.GetCartProductId(id, false);

            var cartAttributeProduct = await _repositoryManager.CartAttributeProduct.CartAttributeProducts(cartAttributeProductId);
            var cartAttributeProductList = cartAttributeProduct.Select(c => c.Id).ToList();
            await _repositoryManager.CartAttributeProduct.DeleteCartAttributeProductList(cartAttributeProductList);

            _repositoryManager.CartProduct.DeleteCartProduct(cartProduct);
            await _repositoryManager.SaveAsync();
        }
        public async Task<decimal> AvailableAmountForCart(int cartId, int customerId)
        {
            int availableInventory = 0;
            var cart = await _repositoryManager.Cart.GetCartId(cartId, false);

            int productId = cart.CartProducts.Select(c => c.ProductId).First();
            int customerProductId = cart.Customer.CustomerProducts.First().Id;
            List<string> option = new List<string>();
            var customerProducts = await _repositoryManager.CustomerAttributesProduct.GetAllAttributesCustomerProduct(customerProductId);
            var attributId = customerProducts.Select(c => c.AttributesProduct).First().Id;
            if (customerProducts.Count() > 0)
            {
                foreach (var customerProduct in customerProducts)
                {
                    int optionId = customerProduct.AttributesProduct.OptionId;
                    int valueId = customerProduct.AttributesProduct.ValueId;
                    var productOption = await _repositoryManager.Option.GetOptionId(optionId, false);
                    if (productOption != null)
                    {
                        var attribut = await _repositoryManager.Attribute.GetProductOptionValue(productId, optionId, valueId);
                        if (attribut != null)
                        {
                            option.Add(attribut.Id.ToString());
                        }
                    }
                }
            }
            var instock = 0;
            var outstock = 0;
            if (option != null)
            {
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByProductIdOption(productId, attributId);
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
                var storeId = cart.CartStores.First().StoreId;
                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId, customerId);
                if (order != null)
                {
                    var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, order.Id);
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
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
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
                    var storeId = cart.CartStores.First().StoreId;
                    var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId, customerId);
                    if (order != null)
                    {
                        var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, order.Id);
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
        public async Task<decimal> AvailableAmountForCarttest(int cartId, int customerId)
        {
            int availableInventory = 0;
            var instock = 0;
            var outstock = 0;
            var cart = await _repositoryManager.Cart.GetCartId(cartId, false);
            int productId = cart.CartProducts.Select(c => c.ProductId).First();

            var cartProduct = await _repositoryManager.CartProduct.GetCartIdProductId(productId, cartId);
            int optionId = cartProduct.CartAttributeProducts.Select(c => c.AttributesProduct).First().OptionId;
            int valueId = cartProduct.CartAttributeProducts.Select(c => c.AttributesProduct).First().ValueId;
            var attribute = await _repositoryManager.Attribute.GetProductOptionValue(productId, optionId, valueId);
            if (attribute != null)
            {
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByProductIdOption(productId, attribute.Id);
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
                var storeId = cart.CartStores.First().StoreId;
                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId, customerId);
                if (order != null)
                {
                    var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, order.Id);
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
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
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
                    var storeId = cart.CartStores.First().StoreId;
                    var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId, customerId);
                    if (order != null)
                    {
                        var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, order.Id);
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
        public async Task<decimal> GetTotalCart(int storeId, int customerId, string code)
        {
            decimal total = 0;
            var customerCarts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            if (storeId != 0)
            {
                customerCarts = customerCarts.Where(c => c.CartStores.Any(c => c.StoreId == storeId)).ToList();
            }
            if (customerCarts.Count() > 0)
            {
                foreach (var cart in customerCarts)
                {
                    total = total + Convert.ToDecimal(cart.FinalPrice);
                }
            }
            if (code != null)
            {
                total = await GetCartWithCoupon(storeId, customerId, code);
            }
            decimal tax = await _locationTaxBL.GetTax(customerId);
            if (tax != 0)
            {
                total = total + ((total * tax) / 100);
            }
            return total;
        }

        public async Task<decimal> GetCartWithCoupon(int storeId, int customerId, string code)
        {
            decimal total = 0;
            var customerCarts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            if (storeId != 0)
            {
                customerCarts = customerCarts.Where(c => c.CartStores.Any(c => c.StoreId == storeId)).ToList();
            }
            if (customerCarts.Count() > 0)
            {
                foreach (var cart in customerCarts)
                {
                    total = total + Convert.ToDecimal(cart.FinalPrice);
                }
            }

            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(code);
            if (coupon == null)
            {
                return total;
            }
            else
            {
                if (coupon.DiscountType == "fixed_cart")
                {
                    if (total < coupon.CouponAmount)
                    {
                    }
                    else
                    {
                        total = total - Convert.ToDecimal(coupon.CouponAmount);
                    }

                }
                else if (coupon.DiscountType == "percent")
                {
                    if (total > 0)
                    {
                        total = total - (total * Convert.ToDecimal(Convert.ToDecimal(coupon.CouponAmount) / 100));
                    }
                    else
                    {
                    }
                }
                else if (coupon.DiscountType == "fixed_product")
                {
                    total = 0;
                    foreach (var cart in customerCarts)
                    {
                        string productId = cart.CartProducts.First().ProductId.ToString();
                        if (coupon.Product.Contains(productId))
                        {
                            decimal newTotal = Convert.ToDecimal(cart.FinalPrice) - Convert.ToDecimal(coupon.CouponAmount);
                            total = total + newTotal;
                        }
                        else
                        {
                            total = total + Convert.ToDecimal(cart.FinalPrice);
                        }
                    }
                }
                else if (coupon.DiscountType == "percent_product")
                {
                    total = 0;
                    foreach (var cart in customerCarts)
                    {
                        string productId = cart.CartProducts.First().ProductId.ToString();
                        if (coupon.Product.Contains(productId))
                        {
                            decimal newval = Convert.ToDecimal(cart.FinalPrice) * Convert.ToDecimal(Convert.ToDecimal(coupon.CouponAmount) / 100);
                            total = total + newval;
                        }
                        else
                        {
                            total = total + Convert.ToDecimal(cart.FinalPrice);
                        }
                    }
                }
            }
            decimal tax = await _locationTaxBL.GetTax(customerId);
            total = total + ((total * tax) / 100);
            return total;
        }
        public async Task<string> GetValueCodeCoupon(int customerId, string code, Currency currency)
        {
            string tot = "0";
            var carts = await _repositoryManager.CustomerProduct.GetProductsCustomerId(customerId);
            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(code);
            if (coupon == null)
            {
                return tot;
            }
            else
            {
                if (coupon.DiscountType == "fixed_cart")
                {
                    tot = currency.Symbol + " " + Convert.ToDecimal(coupon.CouponAmount);
                    return tot;
                }
                else if (coupon.DiscountType == "percent")
                {
                    tot = "%" + coupon.CouponAmount;
                    return tot;
                }
                else if (coupon.DiscountType == "fixed_product")
                {
                    bool isEx = false;
                    foreach (var cart in carts)
                    {
                        string pid = cart.ProductId.ToString();
                        if (coupon.Product.Contains(pid))
                        {
                            isEx = true;
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
                else if (coupon.DiscountType == "percent_product")
                {
                    bool isEx = false;
                    foreach (var cart in carts)
                    {
                        string pid = cart.ProductId.ToString();
                        if (coupon.Product.Contains(pid))
                        {
                            isEx = true;
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
        public async Task AddCustomerProduct(int productId, CreateCustomerProductDto updateDto)
        {
            var customerId = 0 /* GetCurrentUserId()*/;
            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerIdProduct(productId, customerId);
            var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(productId);
            var flash = await _repositoryManager.Sales.GetFlashProductId(productId);
            if (special != null)
            {
                updateDto.FinalPrice = special.SpecialPrice;
            }
            else if (flash != null)
            {
                updateDto.FinalPrice = flash.DiscountPrice;
            }
            else
            {
                updateDto.FinalPrice = customerProduct.FinalPrice.Value;
            }
            if (customerProduct == null)
            {
                var customerAttributesProducts = await _repositoryManager.CustomerAttributesProduct.GetAllAttributesCustomerProduct(customerProduct.Id);
                if (customerAttributesProducts != null && customerAttributesProducts.Count() > 0)
                {
                    foreach (var item in customerAttributesProducts)
                    {
                        var attributs = await _repositoryManager.Attribute.GetAttributesProductId(productId);

                        var attribut = attributs.Where(r => r.Id == item.AttributesProductId).FirstOrDefault();
                        if (attribut != null)
                        {
                            if (attribut != null && attribut.AttributePrice != 0)
                            {
                                if (attribut.PricePrefix == "+")
                                {
                                    updateDto.FinalPrice += attribut.AttributePrice;
                                }
                                if (attribut.PricePrefix == "-")
                                {
                                    if (updateDto.FinalPrice != 0)
                                    {
                                        updateDto.FinalPrice -= attribut.AttributePrice;
                                    }
                                }
                            }

                            _repositoryManager.CustomerAttributesProduct.AddAttributeCustomerProduct(item);
                            await _repositoryManager.SaveAsync();
                        }
                        customerProduct.FinalPrice = updateDto.FinalPrice * updateDto.Quantity;
                        await _repositoryManager.SaveAsync();

                    }
                }
                customerProduct.FinalPrice = updateDto.FinalPrice * updateDto.Quantity;
                _repositoryManager.CustomerProduct.AddCustomerProduct(customerProduct);
                await _repositoryManager.SaveAsync();
            }
            else
            {
                customerProduct.Quantity += updateDto.Quantity;
                customerProduct.FinalPrice = updateDto.FinalPrice * customerProduct.Quantity;
                _mapper.Map(updateDto, customerProduct);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<string> AddProductToCart(int productId, int customerId, int Quantity, int?[] option = null)
        {
            var prod = await _repositoryManager.Product.GetProductById(productId, true);
            var storeId = prod.ProductsStores.First().VendorId;
            if (prod == null)
            {
                return _locService.GetLocalizedStringValue("noproducts");
            }
            var customerProduct = await _repositoryManager.CustomerProduct.GetCustomerIdProduct(productId, customerId);
            var availabalProduct = _productBl.AvailabilityProducts(productId);

            var distinct = _repositoryManager.Attribute.GetDistinctProdCart(productId);

            var attributs = await _repositoryManager.Attribute.GetAttributesProductId(productId);

            if (attributs.First() != null)
            {
                if (option != null)
                {
                    if (option.Count() != distinct) { return _locService.GetLocalizedStringValue("plzchooseoption"); } else { }
                }
                else
                {
                    return _locService.GetLocalizedStringValue("plzchooseoption");
                }
            }
            else { }

            //Availability
            var instock = 0;
            var outstock = 0;
            var optStr = "";
            if (option != null)
            {
                foreach (var p in option)
                {
                    optStr = optStr + p.Value + ",";
                }
            }
            if (option != null)
            {
                var instock2 = 0; var outstock2 = 0;
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByProductIdOption(productId, Convert.ToInt32(optStr));
                foreach (var inventory in inventories)
                {
                    if (inventory.StockType == "in")
                    {
                        instock2 += inventory.Stock;
                    }
                    if (inventory.StockType == "out")
                    {
                        outstock2 += inventory.Stock;
                    }
                }
                int inv0All = instock2 - outstock2;
                if ((inv0All) >= Quantity) { } else { return _locService.GetLocalizedStringValue("notAvOp"); }

                if (customerProduct != null && (inv0All) < (customerProduct.Quantity + Quantity))
                {
                    return _locService.GetLocalizedStringValue("notAvOp");
                }
                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId, customerId);
                if (order != null)
                {
                    var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, order.Id);
                    if (orderProduct != null)
                    {
                        if ((orderProduct.Qty + Quantity) > (inv0All))
                        {
                            return _locService.GetLocalizedStringValue("notAvOp");
                        }
                    }
                }
            }
            else
            {
                int invall;
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(productId);
                if (inventories != null && inventories.Count() > 0)
                {
                    foreach (var inventory in inventories)
                    {
                        if (inventory.StockType == "in")
                        {
                            instock += inventory.Stock;
                        }
                        if (inventory.StockType == "out")
                        {
                            outstock += inventory.Stock;
                        }
                    }
                    invall = instock - outstock;
                    if (invall >= Quantity) { }
                    else { return _locService.GetLocalizedStringValue("notAv"); }
                }
                else
                {
                    return _locService.GetLocalizedStringValue("notAv");
                }
                if (customerProduct != null && invall < (customerProduct.Quantity + Quantity))
                {
                    return _locService.GetLocalizedStringValue("notAvOp");
                }

                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId, customerId);
                if (order != null)
                {
                    var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(productId, order.Id);
                    if (orderProduct != null)
                    {
                        if ((orderProduct.Qty + Quantity) > (invall))
                        {
                            return _locService.GetLocalizedStringValue("notAvOp");
                        }
                    }
                }
            }
            var createDto = _mapper.Map<CreateCustomerProductDto>(customerProduct);
            await AddCustomerProduct(productId, createDto);

            return _locService.GetLocalizedStringValue("addedtoCart");
        }





        /*
        public async Task<string> AddProductToCart(int productId, UpdateCustomerProductDto updateDto)
        {
            var customerId = GetCurrentUserId();
            if (productId == 0)
            {
                return _locService.GetLocalizedStringValue("NoProducts");
            }
            var product = await _repositoryManager.Product.GetProductById(productId, true);
            if (product == null)
            {
                return _locService.GetLocalizedStringValue("NoProducts");
            }
            product.CustomerProducts.Select(c => c.CustomerId == customerId);
            // await AddCartProduct(productId, updateDto);
            return _locService.GetLocalizedStringValue("AddedToCart");
        }
        public async Task AddCustomerProductCart3(CreateCustomerProductDto createDto, int productId, int customerId)
        {
            var customerProduct = _mapper.Map<CustomerProduct>(createDto);
            var product = await _repositoryManager.Product.GetProductById(productId, true);
            var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(productId);
            var flash = await _repositoryManager.Sales.GetFlashProductId(productId);
            if (product != null)
            {
                if (special != null)
                {
                    createDto.FinalPrice = special.SpecialPrice;
                }
                else if (flash != null)
                {
                    createDto.FinalPrice = flash.DiscountPrice;
                }
                else
                {
                    createDto.FinalPrice = product.Price;
                }
               
                var attributes = customerProduct.CustomerAttributesProducts.Select(c=>c.AttributesProduct);
                var option = createDto.CustomerAttributesProducts;
                if (option != null && option.Count() > 0)
                {
                    foreach (var opt in option)
                    {
                        var attribute = attributes.Where(r => r.Id == opt.AttributesProductId).FirstOrDefault();
                        if (attribute != null && attribute.AttributePrice != 0)
                        {
                            if (attribute.PricePrefix == "+")
                            {
                                createDto.FinalPrice += attribute.AttributePrice;
                            }
                            if (attribute.PricePrefix == "-")
                            {
                                if (createDto.FinalPrice != 0)
                                {
                                    createDto.FinalPrice -= attribute.AttributePrice;
                                }
                            }
                        }
                    }
                }
            }
            if (customerProduct == null)
            {
                customerProduct.FinalPrice = createDto.FinalPrice * createDto.Quantity;
                await _productApi.AddCustomerProduct(createDto, customerId, productId);
            }
            else 
            {
                customerProduct.Quantity = customerProduct.Quantity + createDto.Quantity;
                customerProduct.FinalPrice = customerProduct.Quantity * createDto.FinalPrice;
                _mapper.Map(createDto, customerProduct);
            }

            await _repositoryManager.SaveAsync();
        }
        */

        //ggg------------------------------------------------
        //public async Task<List<CartDto>> GetCart(int storeId, int userId, Currency curr)
        //{
        //    var cart = await _repositoryManager.Cart.GetCartsToCustomerId(userId);

        //    if (storeId != 0)
        //    {
        //        cart.Where(r => r.CartStores.Any(c=>c.StoreId == storeId)).ToList();
        //    }
        //    return await getCartModel();
        //}
        //public async Task<List<CartDto>> getCartModel()
        //{
        //    var carts = await _repositoryManager.Cart.GetCarts();
        //    var cartsDto = _mapper.Map<List<CartDto>>(carts);
        //    var cartDto = cartsDto.First();
        //    if (carts.Count() > 0)
        //    {
        //        foreach (var cart in carts)
        //        {
        //            var storeId = cart.CartStores.First().StoreId;
        //            var productId = cart.CartProducts.First().ProductId;
        //            if (storeId != 0)
        //            {
        //                var product = await _repositoryManager.Product.GetProductById(productId , false);
        //                if (product != null)
        //                {
        //                    var store = _userApi.GetStore(storeId);
        //                    var category = await _repositoryManager.Categories.GetCategoryToPrductId(productId);
        //                    var special = await _productApi.IsOffer(productId);
        //                    var flash = await _repositoryManager.Sales.GetFlashProductId(productId);
        //                    var offerPrice = special.Id == 0 ? 0 : special.SpecialPrice;

        //                    //-------------------
        //                    //  cartDto. = optionCart(product_id, t.customers_basket_id),
        //                    cartDto.CartStores.First().StoreId = storeId;
        //                }
        //            }
        //        }
        //    }
        //    return cartsDto;
        //}
    }
}
