
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
            decimal totalPrice = 0;
            var cartProductDto = createDto.CartProducts.First();
            var product = await _repositoryManager.Product.GetActiveProductById(cartProductDto.ProductId, true);
            var special = await _repositoryManager.SpecialProducts.GetSpecialProductId(cartProductDto.ProductId);
            var flash = await _repositoryManager.Sales.GetFlashProductId(cartProductDto.ProductId);
            if (special != null)
            {
                totalPrice = special.SpecialPrice;
            }
            else if (flash != null)
            {
                totalPrice = flash.DiscountPrice;
            }
            else
            {
                totalPrice = product.Price;
            }
            var cartAttributesDto = cartProductDto.CartAttributeProducts;
            if (cartAttributesDto != null)
            {
                foreach (var cartAttributDto in cartAttributesDto)
                {
                    var attributes = await _repositoryManager.Attribute.GetAttributesProductId(cartProductDto.ProductId);
                    var attribut = attributes.Where(c => c.Id == cartAttributDto.AttributesProductId).FirstOrDefault();
                    if (attribut != null && attribut.AttributePrice != 0)
                    {
                        if (attribut.PricePrefix == "+")
                        {
                            totalPrice += attribut.AttributePrice;
                        }
                        if (attribut.PricePrefix == "-" && totalPrice != 0)
                        {
                            totalPrice -= attribut.AttributePrice;
                        }
                    } 
                }
            }
            //var store = await _repositoryManager.User.GetUserId(product.StoreId.Value , false);
            var cart = await _repositoryManager.Cart.GetCartId(createDto.Id, true);
            if (cart == null)
            {
                var addCart = _mapper.Map<Cart>(createDto);
                addCart.CustomerId = customerId;
                addCart.CartProducts.First().StoreId = product.StoreId.Value;    
                addCart.FinalPrice = Convert.ToDecimal(totalPrice * cartProductDto.Qty);
                _repositoryManager.Cart.AddCart(addCart);
            }
            else
            {
                var cartProduct = await _repositoryManager.CartProduct.GetCartProductId(cartProductDto.Id, false);
                cartProduct.StoreId = product.StoreId.Value;
                cartProductDto.Qty = cartProduct.Qty + cartProductDto.Qty;
                cart.FinalPrice = Convert.ToDecimal(totalPrice * cartProductDto.Qty);
                _mapper.Map(createDto, cart);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<string> AddProductToCart(int productId, int customerId, int Quantity, int?[] option = null)
        {
            var prod = await _repositoryManager.Product.GetProductById(productId, true);
            var storeId = prod.StoreId;
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
                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId.Value, customerId);
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

                var order = await _repositoryManager.Order.GetCustomerNewOlderByStore(storeId.Value, customerId);
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
            await AddCustomerProduct(productId , customerId, createDto);

            return _locService.GetLocalizedStringValue("addedtoCart");
        }
        public async Task DeleteCartAttributeProduct(int id, int cartAttributeProductId)
        {
            var cartProduct = await _repositoryManager.CartProduct.GetCartProductId(id, false);
            if(cartProduct != null)
            {
                var cartAttributeProduct = await _repositoryManager.CartAttributeProduct.CartAttributeProducts(cartAttributeProductId);
                if (cartAttributeProduct != null)
                {
                    var cartAttributeProductList = cartAttributeProduct.Select(c => c.Id).ToList();
                    await _repositoryManager.CartAttributeProduct.DeleteCartAttributeProductList(cartAttributeProductList);
                }
                _repositoryManager.CartProduct.DeleteCartProduct(cartProduct);
            }
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
                if (cart != null && availableInventory < cart.CartProducts.First().Qty)
                {
                    return -1;
                }
                var storeId = cart.CartProducts.First().StoreId;
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
                    if (cart != null && availableInventory < cart.CartProducts.First().Qty)
                    {
                        return -1;
                    }
                    var storeId = cart.CartProducts.First().StoreId;
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
            cart.Id = cartId;
            cart.CustomerId = customerId;
           
            var cartProduct = await _repositoryManager.CartProduct.GetCartIdProductId(cart.CartProducts.First().ProductId, cartId);
            int productId = cart.CartProducts.First().ProductId;
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
                if (cart != null && availableInventory < cart.CartProducts.First().Qty)
                {
                    return -1;
                }
                var storeId = cart.CartProducts.First().StoreId;
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
                    if (cart != null && availableInventory < cart.CartProducts.First().Qty)
                    {
                        return -1;
                    }
                    var storeId = cart.CartProducts.First().StoreId;
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
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            var cart = carts.First();
            var cartProduct = await _repositoryManager.CartProduct.GetAllCartProductToCatId(cart.Id);
            var cartPro = cartProduct.First();
            if (storeId != 0)
            {
                 cartProduct.Where(c => c.StoreId == storeId).ToList();
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
                if(coupon != null)
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
                            foreach(var item2 in cartProduct)
                            {
                                var product = await _repositoryManager.Product.GetProductById(item2.ProductId, false);
                                if (coupon.Product.Contains(item2.ProductId.ToString()))
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
                    else if (coupon.DiscountType == "percent_product")
                    {
                        total = 0;
                        foreach (var item in carts)
                        {
                            foreach (var item2 in cartProduct)
                            {
                                var product = await _repositoryManager.Product.GetProductById(item2.ProductId, false);
                                if (coupon.Product.Contains(item2.ProductId.ToString()))
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
        public async Task AddCustomerProduct(int productId,int customerId, CreateCustomerProductDto updateDto)
        {
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
                if(updateDto.CustomerAttributesProducts != null)
                {
                    var customerAttributesProducts = await _repositoryManager.CustomerAttributesProduct.GetAllAttributesCustomerProduct(customerProduct.Id);
                    if (customerAttributesProducts != null)
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
                }
                var entity = _mapper.Map<CustomerProduct>(updateDto);
                entity.FinalPrice = updateDto.FinalPrice * updateDto.Quantity;
                _repositoryManager.CustomerProduct.AddCustomerProduct(entity);
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
       
    }
}
