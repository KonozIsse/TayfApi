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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ApiClasses
{
    public class OrderBL 
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        protected readonly CartBL _cartBL;  
        protected readonly LocationTaxBL _locationTaxBL;
        public OrderBL(IRepositoryManager repositoryManager, IMapper mapper, CartBL cartBL, LocationTaxBL locationTaxBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _cartBL = cartBL;
            _locationTaxBL = locationTaxBL;
        }
        //Order------------------------------------------------
        public async Task<List<Order>> GetPendOrders()
        {
            return await _repositoryManager.Order.GetPendingOrders();
        }
        public async Task<List<Order>> GetPendOrdersByVendorId(int vendorId)
        {
            return await _repositoryManager.Order.GetPendOrdersByStore(vendorId);
        }
        public async Task<List<Order>> GetCompOrders()
        {
            return await _repositoryManager.Order.GetCompleteOrders();
        }
        public async Task<List<Order>> GetCompOrdersByVendorId(int vendorId)
        {
            return await _repositoryManager.Order.GetCompleteOrdersByStore(vendorId);
        }
        public async Task<List<Order>> GetCancelOrders()
        {
            return await _repositoryManager.Order.GetCancelOrders();
        }
        public async Task<List<Order>> GetCancelOrdersByVendorId(int vendorId)
        {
            return await _repositoryManager.Order.GetCancelOrdersByStore(vendorId);
        }
        public int GetAllOrdersCount()
        {
            return  _repositoryManager.Order.GetAllOrdersCount();
        }
        public int GetVendorOrdersCount(int vendorId)
        {
            return _repositoryManager.Order.GetOrdersByVendor(vendorId);
        }
        public async Task<List<Order>> GetOrders()
        {
            return await _repositoryManager.Order.GetAllOrders();
        }
        public async Task AddOrder(CreateOrderDto createOrderDto)
        {
            decimal total = 0;
            var storeId = 1;
            var customerId = 3;
            var tax = await _locationTaxBL.GetTax(customerId);

            var order = _mapper.Map<Order>(createOrderDto);
            order.StoreId = storeId;
            order.CustomerId = customerId;
            order.OrderStatusId = 2;
            order.CurrencyId = 1;
            order.PaymentMethodsId = 7;
            order.IsSeen = 0;
            order.TotalTax = tax;
            order.CodeCoupon = createOrderDto.CouponCode;

            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}", order.Id, order.OrderPrice.ToString("0.00")));
            var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            if (carts != null)
            {
                var orderProducts = new List<OrderProduct>();
                foreach (var cart in carts)
                {
                    var orderAttributs = new List<OrderAttributProduct>();
                    var cartAttributeProducts = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cart.Id);
                    if (cartAttributeProducts != null)
                    {
                        foreach (var cartAttribute in cartAttributeProducts)
                        {
                            orderAttributs.Add(new OrderAttributProduct
                            {
                                ProductAttributId = cartAttribute.AttributesProductId
                            });
                        }
                    }
                    orderProducts.Add(new OrderProduct
                    {
                        ProductId = cart.ProductId,
                        Qty = cart.Qty,
                        FinalPrice = cart.FinalPrice,
                        OrderAttributesProducts = orderAttributs
                    });
                    total += orderProducts.First().FinalPrice;
                }
                order.OrderProducts = orderProducts;

                if (createOrderDto.CouponCode != null)
                {
                    var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(createOrderDto.CouponCode);
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
                                if (coupon.Product.Contains(item.ProductId.ToString()))
                                {
                                    var newTotal = Convert.ToDecimal(item.FinalPrice) - Convert.ToDecimal(coupon.CouponAmount);
                                    total += newTotal;
                                }
                                else
                                {
                                    total += Convert.ToDecimal(item.FinalPrice);
                                }
                            }
                        }
                        else if (coupon.DiscountType == "percent_product")
                        {
                            total = 0;
                            foreach (var item in carts)
                            {
                                if (coupon.Product.Contains(item.ProductId.ToString()))
                                {
                                    decimal newval = Convert.ToDecimal(item.FinalPrice) * Convert.ToDecimal(Convert.ToDecimal(coupon.CouponAmount) / 100);
                                    total = total + newval;
                                }
                                else
                                {
                                    total = total + Convert.ToDecimal(item.FinalPrice);
                                }
                            }
                        }
                    }
                }
            }
            if (tax != 0)
            {
                total = total + ((total * tax) / 100);
            }
            order.OrderPrice = total;
            _repositoryManager.Order.CreateOrder(order);
            try
            {
                await _repositoryManager.SaveAsync();
            }
            catch (Exception) { }
        }
        public async Task<List<HistoryOrderDto>> GetHistoryOrder(int customerId)
        {
            var model = new List<HistoryOrderDto>();
            var orders = await _repositoryManager.Order.GetOrdersToCustomer(customerId);
            foreach (var order in orders)
            {
                if (order.StoreId != 0)
                {
                    var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(order.Id);
                    int count = orderProducts.Count();
                    var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);

                    string state = "";
                    var orderStatus = await _repositoryManager.OrderStatus.GetOrderStatusById(order.OrderStatusId, false);
                    if (orderStatus != null)
                    {
                        state = orderStatus.StatusName;
                    }
                    model.Add(new HistoryOrderDto
                    {
                        Id = order.Id,
                        IsStatus = order.IsStatus,
                        OrderPrice = Convert.ToDecimal(order.OrderPrice),
                        Symbol = "QAR", //currency.Symbol,
                        pcount = count,
                        FullName = customer.FullName,
                        OrderStatusId = order.OrderStatusId,
                        StatusName = state
                    });
                }
            }
            return model;
        }
        public async Task UpdateTotalOrderPrice(int id, decimal totalPrice)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderPrice = totalPrice;
            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}", order.Id, order.OrderPrice.ToString("0.00")));
            await _repositoryManager.SaveAsync();
        }
        public async Task OrderPending(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 2;
            await _repositoryManager.SaveAsync();
        }
        public async Task OrderComplete(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 4;
            await _repositoryManager.SaveAsync();
        }
        public async Task OrderCancal(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 5;
            await _repositoryManager.SaveAsync();
        }
        public async Task OrderReject(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 6;
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateOrderAfterPay(int id, string PaymentsId, int paymentMethodsId)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            if (order.OrderStatusId == 1)
            {
                order.OrderStatusId = 7; //recieved order
                order.DatePurchased = EasternTime;
                order.TransactionId = PaymentsId;
                order.UpdatedAt = EasternTime;
                order.PaymentMethodsId = paymentMethodsId;// "Qatar Charity";
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task DeleteOrder(int orderId)
        {
            var order = await _repositoryManager.Order.GetOrderId(orderId, false);
            if(order != null)
            {
                var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(orderId);
                foreach(var orderProduct in orderProducts)
                {
                    _repositoryManager.OrderProducts.DeleteOrderProduct(orderProduct);
                    var attributes = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(orderProduct.Id, false);
                    if(attributes != null)
                    {
                        foreach (var attribut in attributes)
                        {
                            _repositoryManager.OrderAttributesProducts.DeleteOrderAttributProduct(attribut);
                        }
                    }
                }

                _repositoryManager.Order.DeleteOrder(order);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateStatusOrder(int id, int adminId, int vendorId = 0)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            //out stock 
            var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(order.Id);
            foreach (var item in orderProducts)
            {
                var inventory = new Inventory();
                int attribute = 0;
                var orderAttributesProducts = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(item.Id, true);
                if (orderAttributesProducts != null)
                {
                    foreach (var option in orderAttributesProducts)
                    {
                        attribute = option.ProductAttributId.Value ;
                        inventory.AttributesProductId = attribute == 0 ? 0 : attribute;
                    }
                }
              
                inventory.AddedDate = EasternTime.Millisecond;
                inventory.AdminId = adminId;
                if (vendorId != 0)
                {
                    inventory.VendorId = vendorId;
                }
                inventory.Stock = item.Qty;
                inventory.ProductId = item.ProductId;
                inventory.TotalPurchasedPrice = item.FinalPrice;
                inventory.PurchaseCode = item.OrderId.ToString();
                inventory.StockType = "out";
               // inventory.AttributesProductId = attribute == 0 ? 0 : attribute;
                _repositoryManager.Inventory.AddInventory(inventory);
                
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task<List<OrderDto>> GetOrdersByFilter(int customerId)
        {
            var orders = await _repositoryManager.Order.GetOrdersToCustomer(customerId);
            var ordersDto = _mapper.Map<List<OrderDto>>(orders);
            foreach (var order in orders)
            {
                order.CustomerId = customerId;
                var store = await _repositoryManager.User.GetStoreId(order.StoreId);
                if (store != null)
                {
                    var products = await _repositoryManager.Product.GetProductsTOStoreId(order.StoreId);
                    int productsCount = products.Count();
                    var nameTime = "";
                    if (order.DeliveryTimeId != 0)
                    {
                        var time = await _repositoryManager.DeliveryTime.GetDeliveryTimeById(order.DeliveryTimeId, false);
                        if (time != null)
                        {
                            nameTime = time.Time;
                        }
                    }
                    string stat = "";
                    var states = await _repositoryManager.OrderStatus.GetOrderStatusById(order.OrderStatusId, false);
                    if (states != null)
                    {
                        stat = states.StatusName;
                    }
                    var orderDto = ordersDto.First();
                    orderDto.CreatedAt = Convert.ToDateTime(!String.IsNullOrEmpty(order.DatePurchased.ToString()) ? order.CreatedAt.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss tt") : "");
                    orderDto.DatePurchased = Convert.ToDateTime(!String.IsNullOrEmpty(order.DatePurchased.ToString()) ? order.DatePurchased.Value.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss tt") : "");
                    orderDto.countProduct = productsCount;
                }
            }
            return ordersDto;
        }
        //Coupon------------------------------------------------
        public async Task AddCoupon(CreateCouponDto createCouponDto)
        {
            var coupon = _mapper.Map<Coupon>(createCouponDto);
            coupon.StoreId = 1;
            coupon.Product = createCouponDto.Products == null ? null : string.Join(",", createCouponDto.Products);
            _repositoryManager.Coupon.AddCoupon(coupon);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteCoupon(int id)
        {
            var cart = await _repositoryManager.Coupon.GetCouponId(id, false);
            _repositoryManager.Coupon.DeleteCoupon(cart);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateCoupon(int id, UpdateCouponDto updateCouponDto)
        {
            var coupon = await _repositoryManager.Coupon.GetCouponId(id, true);
            coupon.StoreId = 1;
            coupon.Product = updateCouponDto.Products == null ? null : string.Join(",", updateCouponDto.Products);
            _mapper.Map(updateCouponDto, coupon);
            await _repositoryManager.SaveAsync();
        }
        public bool CheckCodeCoupon(string code)
        {
            return _repositoryManager.Coupon.CheckExistCoupon(code);
        }
        //OrderStatus------------------------------------------------
        public async Task<List<OrderStatus>> GetOrderStatus()
        {
            return await _repositoryManager.OrderStatus.GetOrderStatusesList(false);
        }
        //Payment------------------------------------------------
        public async Task<List<PaymentMethods>> GetPayments()
        {
            return await _repositoryManager.PaymentMethods.GetPaymentMethods();
        }
        public async Task<List<PaymentMethods>> GetPaymentsByVendor(int vendorId)
        {
            return await _repositoryManager.PaymentMethods.GetPaymentsByVendor(vendorId);
        }
        //DeliveryTime------------------------------------------------
        public async Task<DeliveryTimeDto> GetTimeById(int timeId)
        {
            var time = await _repositoryManager.DeliveryTime.GetDeliveryTimeById(timeId , false);
            var timeDto = _mapper.Map<DeliveryTimeDto>(time);
            return timeDto;
        }
        public async Task<List<DeliveryTime>> GetTimes()
        {
            return await _repositoryManager.DeliveryTime.GetAllDeliveryTimes();
        }
        //Unit------------------------------------------------
        public async Task<List<Unit>> GetUnit()
        {
            return await _repositoryManager.Unit.GetAlActivelUnit();
        }
        public async Task<List<Unit>> GetUnitsByVendor(int vendorId)
        {
            return await _repositoryManager.Unit.GetUnitsByVendor(vendorId);
        }
        //-----------------------------------------------
        public DateTime EasternTime
        {
            get
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            }
        }
        protected static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 Sha256Hash = SHA256.Create())
            {
                byte[] bytes = Sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
