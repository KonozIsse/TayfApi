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
        public OrderBL(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
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
        public async Task UpdateTotalOrderPrice(int id, decimal totalPrice)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderPrice = totalPrice;
            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}", order.Id, order.OrderPrice.ToString("0.00")));
            await _repositoryManager.SaveAsync();
        }
        public async Task AddOrder(CreateOrderDto createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);
            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}", order.Id, order.OrderPrice.ToString("0.00")));
            order.OrderStatusId = 1;
            _repositoryManager.Order.CreateOrder(order);
            try
            {
                await _repositoryManager.SaveAsync();
            }
            catch (Exception) { }
        }
        public async Task OrderPending(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 1;
            await _repositoryManager.SaveAsync();
        }

        public async Task OrderComplete(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 2;
            await _repositoryManager.SaveAsync();
        }
        public async Task OrderCancal(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 3;
            await _repositoryManager.SaveAsync();
        }
        public async Task OrderReject(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 4;
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateOrderAfterPay(int id, string PaymentsId, string payment)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            if (order.OrderStatusId == 1)
            {
                order.OrderStatusId = 5; //recieved order
                order.DatePurchased = EasternTime;
                order.TransactionId = PaymentsId;
                order.UpdatedAt = EasternTime;
                order.PaymentMethods.PaymentMethod = payment;// "Qatar Charity";
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task DeleteOrder(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            _repositoryManager.Order.DeleteOrder(order);
            await _repositoryManager.SaveAsync();
        }
        public async Task EditOrder(UpdateOderDto updateOderDto, decimal totalBeforCode,  string code, decimal amount)
        {
            var order = await _repositoryManager.Order.GetActiveOrderId(updateOderDto.Id, true);
            _mapper.Map(updateOderDto, order);
            var coupon = await _repositoryManager.Coupon.GetCouponIdNotFinished(updateOderDto.CouponId.Value);
            if (updateOderDto.CouponId.Value != 0)
            {
                order.Coupon.CouponCode = code;
                order.Coupon.CouponAmount = amount;
                order.OrderPrice = updateOderDto.OrderPrice;
            }
            else
            {
                order.Coupon.CouponCode = "";
                order.Coupon.CouponAmount = 0;
                order.OrderPrice = totalBeforCode + (totalBeforCode * (updateOderDto.TotalTax / 100));
            }
            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}",order.Id, order.OrderPrice.ToString("0.00")));

            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateStatusOrder(/*UpdateOderDto updateOderDto, */int id, int vendorId, int adminId)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            // _mapper.Map(updateOderDto, order);
            await _repositoryManager.SaveAsync();

            //out stock 
            var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(order.Id);
            foreach (var item in orderProducts)
            {
                string attribute = "";
                var orderAttributesProducts = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(item.Id, true);
                if (orderAttributesProducts != null)
                {
                    foreach (var option in orderAttributesProducts)
                    {
                        attribute += option.ProductAttributId + ",";
                    }
                }
                var inventory = new Inventory();
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
                inventory.AttributesProductId = Convert.ToInt32(attribute);
                _repositoryManager.Inventory.AddInventory(inventory);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task DeleteOrderProduct(int productId, int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, false);
            var orderProductList = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(id);
            if (orderProductList.Count() > 0)
            {
                if (orderProductList.Count() == 1)
                {
                    order.IsDeleted = true;
                    await _repositoryManager.SaveAsync();
                }
                else
                {
                    var orderProduct = orderProductList.Where(r => r.ProductId == productId).FirstOrDefault();
                    if (orderProduct != null)
                    {
                        var orderAttributProducts = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(orderProduct.Id,false);
                        if (orderAttributProducts != null)
                        {
                            foreach (var orderAttributProduct in orderAttributProducts)
                            {
                                _repositoryManager.OrderAttributesProducts.DeleteOrderAttributProduct(orderAttributProduct);
                                await _repositoryManager.SaveAsync();
                            }
                        }
                        _repositoryManager.OrderProducts.DeleteOrderProduct(orderProduct);
                        await _repositoryManager.SaveAsync();
                    }
                }
            }
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
        public async Task<List<HistoryOrderDto>> getProductsOrders(int customerId, Currency currency, List<Order> Orders)
        {
            var model = new List<HistoryOrderDto>();
            foreach (var order in Orders)
            {
                if (order.StoreId != 0)
                {
                    var orderProducts = order.OrderProducts.ToList();
                    int count = orderProducts.Count();
                    var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId , false);

                    string state = "";
                    var orderStatus = order.OrderStatus;
                    if (orderStatus != null)
                    {
                        state = orderStatus.StatusName;
                    }

                    model.Add(new HistoryOrderDto
                    {
                        Id = order.Id,
                        OrderPrice = Convert.ToDecimal(order.OrderPrice),
                        Symbol = currency.Symbol,
                        pcount = count,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        OrderStatusId = order.OrderStatusId,
                        StatusName = state
                    });
                }
            }

            return model;
        }
        //Coupon------------------------------------------------
        public async Task AddCoupon(CreateCouponDto createCouponDto)
        {
            var coupon = _mapper.Map<Coupon>(createCouponDto);
           // coupon.StoreId = GetCurrentUserId();
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
        protected DateTime EasternTime
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
