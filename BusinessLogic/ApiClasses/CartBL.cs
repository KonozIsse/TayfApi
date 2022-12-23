
using AutoMapper;
using BusinessLogic.ViewModel;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cms;
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
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly LocService _locService;
        private readonly LocationTaxBL _locationTaxBL;
        public CartBL( IRepositoryManager repositoryManager, IMapper mapper, LocService locService, LocationTaxBL locationTaxBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _locService = locService;
            _locationTaxBL = locationTaxBL;
        }
        //Cart------------------------------------------------
        public int CartCount()
        {
            return _repositoryManager.Cart.CartCount();
        }
        public async Task<decimal> GetTotalOrder (int customerId, int orderId = 0)
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
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
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
        public async Task ChangeActiveStatusCart(int id)
        {
            var cart = await _repositoryManager.Cart.GetCartId(id, true);
            cart.IsStatus = Status.Active;
            await _repositoryManager.SaveAsync();
        }
        public async Task AddCart(int customerId ,CreateCartDto createDto)
        {
            var product = await _repositoryManager.Product.GetActiveProductById(createDto.ProductId, true);
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
            var cartAttributesDto = createDto.CartAttributeProducts;
            if (cartAttributesDto != null)
            {
                foreach (var cartAttributDto in cartAttributesDto)
                {
                    var attributes = await _repositoryManager.Attribute.GetAttributesProductId(createDto.ProductId);
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
            var cart = await _repositoryManager.Cart.GetCartId(createDto.Id, true);
            if (cart == null)
            {
                var addCart = _mapper.Map<Cart>(createDto);
                addCart.CustomerId = customerId;
                addCart.ProdId = createDto.ProductId;
                addCart.StoreId = product.StoreId == 0 ? 0 : product.StoreId.Value;
                addCart.FinalPrice = Convert.ToDecimal(productPrice * createDto.Qty);
                _repositoryManager.Cart.AddCart(addCart);
            }
            else
            {
                cart.StoreId = product.StoreId == 0 ? 0 : product.StoreId.Value;
                createDto.Qty = cart.Qty + createDto.Qty;
                cart.FinalPrice = Convert.ToDecimal(productPrice * createDto.Qty);
                _mapper.Map(createDto, cart);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<string> AddProductToCart(int customerId, CreateCartDto createDto)
        {
            var prod = await _repositoryManager.Product.GetProductById(createDto.ProductId, true);
            var storeId = prod.StoreId == 0 ? 0 : prod.StoreId;
            if (prod == null)
            {
                return _locService.GetLocalizedStringValue("noproducts");
            }
            var cart = await _repositoryManager.Cart.GetCustomerProduct(createDto.ProductId, customerId, true);

            ////Availability
            var instock = 0;
            var outstock = 0;

            if (createDto.CartAttributeProducts != null)
            {
                var attributeDto = createDto.CartAttributeProducts.First();
                var instock2 = 0; var outstock2 = 0;
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByProductIdOption(createDto.ProductId, attributeDto.AttributesProductId);
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
                if ((inv0All) >= createDto.Qty) { } else { return _locService.GetLocalizedStringValue("notAvOp"); }

                if (cart != null && (inv0All) < (cart.Qty + createDto.Qty))
                {
                    return _locService.GetLocalizedStringValue("notAvOp");
                }
                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId.Value, customerId);
                if (order != null)
                {
                    var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(createDto.ProductId, order.Id, true);
                    if (orderProduct != null)
                    {
                        if ((orderProduct.Qty + createDto.Qty) > (inv0All))
                        {
                            return _locService.GetLocalizedStringValue("notAvOp");
                        }
                    }
                }
            }
            else
            {
                int invall;
                var inventories = await _repositoryManager.Inventory.GetAllInventoryByPrductId(createDto.ProductId);
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
                    if (invall >= createDto.Qty) { }
                    else { return _locService.GetLocalizedStringValue("notAv"); }
                }
                else
                {
                    return _locService.GetLocalizedStringValue("notAv");
                }
                if (cart != null && invall < (cart.Qty + createDto.Qty))
                {
                    return _locService.GetLocalizedStringValue("notAvOp");
                }

                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId.Value, customerId);
                if (order != null)
                {
                    var orderProduct = await _repositoryManager.OrderProducts.GetOrderProductsId(createDto.ProductId, order.Id, true);
                    if (orderProduct != null)
                    {
                        if ((orderProduct.Qty + createDto.Qty) > (invall))
                        {
                            return _locService.GetLocalizedStringValue("notAvOp");
                        }
                    }
                }
            }
            await AddCart(customerId, createDto);

            return _locService.GetLocalizedStringValue("addedtoCart");
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
                    if (coupon.DiscountType == "fixed_cart")
                    {
                        if (total > coupon.CouponAmount)
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
                    }
                    else if (coupon.DiscountType == "fixed_product")
                    {
                        total = 0;
                        foreach (var item in carts)
                        {
                            var product = await _repositoryManager.Product.GetProductById(item.ProdId, false);
                            if (coupon.Product.Contains(item.ProdId.ToString()))
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
                    else if (coupon.DiscountType == "percent_product")
                    {
                        total = 0;
                        foreach (var item in carts)
                        {
                            var product = await _repositoryManager.Product.GetProductById(item.ProdId, false);
                            if (coupon.Product.Contains(item.ProdId.ToString()))
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
                        string pid = cart.ProdId.ToString();
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
                        string pid = cart.ProdId.ToString();
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
        public async Task<decimal> AvailableAmountForCart(int cartId, int customerId)
        {
            int availableInventory = 0;
            var instock = 0;
            var outstock = 0;
            var cart = await _repositoryManager.Cart.GetCartId(cartId, false);
            cart.Id = cartId;
            cart.CustomerId = customerId;
            var cartAttributeProducts = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cartId);
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
        public async Task DeleteCart(int cartId)
        {
            var cart = await _repositoryManager.Cart.GetCartId(cartId, false);
            if (cart != null)
            {
                var cartAttributes = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cartId);
                if (cartAttributes != null)
                {
                    foreach (var cartAttribut in cartAttributes)
                    {
                        _repositoryManager.CartAttributeProduct.DeleteCartAttributeProduct(cartAttribut);
                    }
                }
                _repositoryManager.Cart.DeleteCart(cart);
            }
            await _repositoryManager.SaveAsync();
        }
    }
}
