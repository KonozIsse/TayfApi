using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AutoMapper;
using BusinessLogic.ViewModel;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enum;
using Entities.Models.Enums;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
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
        protected readonly ImageBL _imageBL;
        protected readonly ProductBL _productBL;
        protected readonly IEmailSender _emailSender;
        public OrderBL(IRepositoryManager repositoryManager, IMapper mapper, CartBL cartBL, LocationTaxBL locationTaxBL, ImageBL imageBL, ProductBL productBL, IEmailSender emailSender)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _cartBL = cartBL;
            _locationTaxBL = locationTaxBL;
            _imageBL = imageBL;
            _productBL = productBL;
            _emailSender = emailSender;
        }
        //Order------------------------------------------------

        public async Task<List<Order>> GetPendOrders()
        {
            return await _repositoryManager.Order.GetPendingOrders();
        }
        public async Task<List<Order>> GetsAllOrders()
        {
            return await _repositoryManager.Order.GetsAllTransactionOrders();
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
        public async Task<Order> OrderByIdAndStatus(int orderId, int statusId)
        {
            return await _repositoryManager.Order.GetOrderIdAndStatusId(orderId , statusId);
        }
        public async Task AddOrder(int customerId ,CreateOrderDto createOrderDto)
        {
            decimal total = 0;
           
            var tax = await _locationTaxBL.GetTax(customerId);

            var order = _mapper.Map<Order>(createOrderDto);
            
            order.CustomerId = customerId;
            order.OrderStatusId = 1;
            order.CurrencyId = 5;
            order.PaymentMethodsId = 6;
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
                    order.StoreId = cart.StoreId;
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
                        ProductId = cart.ProdId,
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
                                if (coupon.Product.Contains(item.ProdId.ToString()))
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
                                if (coupon.Product.Contains(item.ProdId.ToString()))
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
            order.OrderStatusId = 1;
            await _repositoryManager.SaveAsync();
        }
        public async Task<BussnessResultModel> OrderComplete(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            if(order == null)
            {
                return new BussnessResultModel(null, "correctLink", false);
            }
            if(order.OrderStatusId == 2)
            {
                return new BussnessResultModel(null, "AlreadyComp", false);
            }
            if ( order.OrderStatusId != 6)
            {
                return new BussnessResultModel(null, ""/* _locService.GetLocalizedStringValue("OrderNottPaid")*/, false);
            }
            if (order.OrderStatusId == 6)
            {
                order.OrderStatusId = 2;
            }
            var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.CompleteOrder);
            var store = await _repositoryManager.User.GetStore(order.StoreId, false);
            action.Template = action.Template.Replace("{userName}", store.FirstName);
            Notification notification = new()
            {
                Body = action.Template,
                UserId = order.CustomerId,
                NotificationActionId = action.Id,
                Status = NotificationStatus.New,
                Subject = action.Subject,
                IsRead = false
            };
            _repositoryManager.Notification.CreateNotification(notification);
            //send email 
           // string msgEm1 = await InvoiceOrder(id);
            var temp = await _repositoryManager.MessageTemplate.GetTemplateById(4, false); //shipped email
            var msgem = /*msgEm1 +*/ temp.Message+ "<br><br> The E-Tayf account team <br> Thank You";
            var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
            try
            {
                var message = new Message(new string[] { customer.Email }, "Order Details", msgem);
                _emailSender.SendEmail(message);
            }
            catch (Exception exp)
            {
              //  _logger.LogError($"COULD NOT SEND EMAIL: " + exp.Message, exp);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(order, "OrdeCompleted");
        }
        public async Task OrderCancal(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 5;
            var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.CancelOrder);
            var store = await _repositoryManager.User.GetStore(order.StoreId, false);
            action.Template = action.Template.Replace("{userName}", store.FirstName);
            Notification notification = new()
            {
                Body = action.Template,
                UserId = order.CustomerId,
                NotificationActionId = action.Id,
                Status = NotificationStatus.New,
                Subject = action.Subject,
                IsRead = false
            };
            _repositoryManager.Notification.CreateNotification(notification);
            await _repositoryManager.SaveAsync();

        }
        public async Task<BussnessResultModel> OrderReject(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            if(order == null)
            {
                return new BussnessResultModel(null, "AlreadyCancel", false);
            }
            if(order.OrderStatusId == 3)
            {
                return new BussnessResultModel(null, "AlreadyCancel", false);
            }
            if (order.OrderStatusId != 1)
            {
                var period =  _repositoryManager.Setting.GetPeriod();
                //if order shipped or recieved and allowed period for refund ended
                if ((order.OrderStatusId == 5 || order.OrderStatusId == 6) && (order.DatePurchased.Value.AddDays(Convert.ToInt32(period)) < EasternTime))
                {
                    return new BussnessResultModel(null, "e: You Cann't Reject Order After period about " + period + " days from order", false);
                }
            }
            order.OrderStatusId = 4;
            var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.RejectOrder);
            var store = await _repositoryManager.User.GetStore(order.StoreId, false);
            action.Template = action.Template.Replace("{userName}", store.FirstName);
            Notification notification = new()
            {
                Body = action.Template,
                UserId = order.CustomerId,
                NotificationActionId = action.Id,
                Status = NotificationStatus.New,
                Subject = action.Subject,
                IsRead = false
            };
            _repositoryManager.Notification.CreateNotification(notification);
            //string msgEm1 = await InvoiceOrder(id);
             var temp = await _repositoryManager.MessageTemplate.GetTemplateById(5,false); 
            var msgem = temp.Message + "<br><br> The E-Tayf account team <br> Thank You";
            var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
            try
            {
                var message = new Message(new string[] { customer.Email }, "Order Details", msgem);
                 _emailSender.SendEmail(message);
            }
            catch (Exception exp)
            {
                //  _logger.LogError("could not send a rejection email to customer, order id =" + id, exp);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(order, "OrderRejected");
        } 
        public async Task OrderReceived(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 5;
            var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.ReceiveOrder);
            var store = await _repositoryManager.User.GetStore(order.StoreId, false);
            action.Template = action.Template.Replace("{userName}", store.FirstName);
            Notification notification = new()
            {
                Body = action.Template,
                UserId = order.CustomerId,
                NotificationActionId = action.Id,
                Status = NotificationStatus.New,
                Subject = action.Subject,
                IsRead = false
            };
            _repositoryManager.Notification.CreateNotification(notification);
            await _repositoryManager.SaveAsync();
        } 
        public async Task<BussnessResultModel> ShippedOrder(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            if (order == null)
            {
                return new BussnessResultModel(null , "correctLink", false);
            }
            if (order.OrderStatusId != 5 || String.IsNullOrEmpty(order.TransactionId))
            {
                return new BussnessResultModel(null, "notPaid", false);
            }
            if (order.OrderStatusId == 5)
            {
                order.OrderStatusId = 6;
            }
            var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(id);
            foreach (var OrdPro in orderProducts)
            {
                var date = DateTime.UtcNow;
                var attributs = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(OrdPro.Id , false);
                if (attributs != null)
                {
                    foreach (var attr in attributs)
                    {
                       
                        var inventory = new Inventory
                        {
                            StockType = "out",
                            Stock = OrdPro.Qty,
                            TotalPurchasedPrice = OrdPro.FinalPrice,
                            PurchaseCode = id.ToString(),
                            ProductId = OrdPro.ProductId,
                            AttributesProductId = attr.ProductAttributId,
                            AddedDate = date.Millisecond,
                            VendorId = order.StoreId
                        };
                        _repositoryManager.Inventory.AddInventory(inventory);
                    }
                }
                else
                {
                    var inventory1 = new Inventory
                    {
                        StockType = "out",
                        Stock = OrdPro.Qty,
                        TotalPurchasedPrice = OrdPro.FinalPrice,
                        PurchaseCode = id.ToString(),
                        ProductId = OrdPro.ProductId,
                        AttributesProductId = null,
                        AddedDate = date.Millisecond,
                        VendorId = order.StoreId
                    };
                    _repositoryManager.Inventory.AddInventory(inventory1);
                }
            }
           
            var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.ShippedOrder);
            var store = await _repositoryManager.User.GetStore(order.StoreId, false);
            action.Template = action.Template.Replace("{userName}", store.FirstName);
            Notification notification = new()
            {
                Body = action.Template,
                UserId = order.CustomerId,
                NotificationActionId = action.Id,
                Status = NotificationStatus.New,
                Subject = action.Subject,
                IsRead = false
            };
            _repositoryManager.Notification.CreateNotification(notification);
           // string msgEm1 = await InvoiceOrder(id);

            //send email
            var temp = await _repositoryManager.MessageTemplate.GetTemplateById(3, false);
            var msgem = temp.Message + "<br><br> The E-Tayf account team <br> Thank You";
            var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
            try
            {
                var message = new Message(new string[] { customer.Email }, "Order Details", msgem);
                _emailSender.SendEmail(message);
            }
            catch (Exception exp)
            {
               // _logger.LogError($"COULD NOT SEND EMAIL: " + exp.Message, exp);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(order, "Toshipped");
        }
        public async Task UpdateOrderAfterPay(int id, string PaymentsId)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            if (order.OrderStatusId == 1)
            {
                order.OrderStatusId = 5; //recieved order
                order.DatePurchased = EasternTime;
                order.TransactionId = PaymentsId;
                order.UpdatedAt = EasternTime;
                order.PaymentMethodsId = 6;// "Qatar Charity";
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task<Order> GetOrderId(int orderId)
        {
            return await _repositoryManager.Order.GetOrderId(orderId, false);
        }
        public async Task<BussnessResultModel> DeleteOrder(int orderId)
        {
            var order = await GetOrderId(orderId);
            if (order.OrderStatusId != 1)
            {
                return new BussnessResultModel(null, "Cann't Delete This Order", false);
            }
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
            else
            {
               return new BussnessResultModel(null, "correctLink", false);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(order , "successDelete");
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
        public async Task<List<OrderFilterDto>> GetOrdersByFilter(int customerId,List<Order> orders , string lange)
        {
            var products = new List<ProductDto>();
            var ordersDto = new List<OrderFilterDto>();
            foreach (var order in orders)
            {
                order.CustomerId = customerId;
                var store = await _repositoryManager.User.GetStoreId(order.StoreId);
                if (store != null)
                {
                    var productsStore = products.Where(r => r.StoreId == store.Id).ToList();
                    int CountproductsStore = productsStore.Count();

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
                    var customer = await _repositoryManager.User.GetCustomerId(customerId, false);

                    ordersDto.Add(new OrderFilterDto
                    {
                    Id = order.Id,
                    CustomerId = customerId,
                    CustomerName = customer.FullName,
                    Currency = order.Currency.Symbol,
                    CustomerEmail = (customer != null ? customer.Email : ""),
                    CustomerPhone = (customer != null ? customer.PhoneNumber : ""),
                    StoreId = store.Id,
                    StoreName = (store != null ? store.FullName : ""),
                    StoreEmail = store.Email,
                    StorePhone = store.PhoneNumber,
                    AddressId = order.AddressId,
                    CreatedAt = Convert.ToDateTime(order.CreatedAt),
                    DatePurchased = (!String.IsNullOrEmpty(order.DatePurchased.ToString()) ? order.DatePurchased.Value.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss tt") : ""),
                    DeliveryTimeName = nameTime,
                    DeliveryTimeId = order.DeliveryTimeId,
                    TotalTax = order.TotalTax,
                    OrderStatusId = order.OrderStatusId,
                    OrderStatusName = stat,
                    Notes = order.Notes,
                    CountProduct = CountproductsStore,
                    OrderPrice = order.OrderPrice
                });
             }
            }
            return ordersDto;
        }
        public async Task<OrderFilterDto> GetOrdersById(int customerId, int orderId, string lange)
        {
            var products = new List<ProductDto>();
            var ordersDto = new OrderFilterDto();
            var order = await GetOrderId(orderId);
                order.CustomerId = customerId;
                var store = await _repositoryManager.User.GetStoreId(order.StoreId);
                if (store != null)
                {
                    var productsStore = products.Where(r => r.StoreId == store.Id).ToList();
                    int CountproductsStore = productsStore.Count();

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
                    var customer = await _repositoryManager.User.GetCustomerId(customerId, false);

                    ordersDto = new OrderFilterDto
                    {
                        Id = order.Id,
                        CustomerId = customerId,
                        CustomerName = customer.FullName,
                        Currency = order.Currency.Symbol,
                        CustomerEmail = (customer != null ? customer.Email : ""),
                        CustomerPhone = (customer != null ? customer.PhoneNumber : ""),
                        StoreId = store.Id,
                        StoreName = (store != null ? store.FullName : ""),
                        StoreEmail = store.Email,
                        StorePhone = store.PhoneNumber,
                        AddressId = order.AddressId,
                        CreatedAt = Convert.ToDateTime(order.CreatedAt),
                        DatePurchased = (!String.IsNullOrEmpty(order.DatePurchased.ToString()) ? order.DatePurchased.Value.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss tt") : ""),
                        DeliveryTimeName = nameTime,
                        DeliveryTimeId = order.DeliveryTimeId,
                        TotalTax = order.TotalTax,
                        OrderStatusId = order.OrderStatusId,
                        OrderStatusName = stat,
                        Notes = order.Notes,
                        CountProduct = CountproductsStore,
                        OrderPrice = order.OrderPrice
                    };
                }
            return ordersDto;
        }
        public async Task<List<OrderDto>> GetOrderCustomer(int CustomerId, string lange)
        {
            var orders = await _repositoryManager.Order.GetOrdersToCustomer(CustomerId);
            var ordersDto = _mapper.Map<List<OrderDto>>(orders);
           return ordersDto;
        }
        public async Task<string> InvoiceOrder(int id)
        {
            decimal sub = 0;
            var newOrder = await _repositoryManager.Order.GetOrderId(id , false);
         

            var user = await _repositoryManager.User.GetUserId(newOrder.CustomerId , false);
            var time = await _repositoryManager.DeliveryTime.GetDeliveryTimeById(newOrder.DeliveryTimeId , false);

            string sts = "";
            var state = newOrder.OrderStatus;
            if (state != null)
            {
                sts = state.StatusName;
            }
            string address1 = "";
            string address2 = "";
            string city1 = "";
            string street = "";
            var c2 = await _repositoryManager.Address.GetAddressIdByCustomerId(newOrder.AddressId, newOrder.CustomerId , false);
            if (c2 != null)
            {
                street = c2.Street;
                address1 = c2.Address1;
                address2 = c2.Address2;
                //decimal tax = 0;
                var city = await _repositoryManager.Zone.GetZoneId(c2.ZoneId , false);
                if (city != null)
                {
                    city1 = city.ZoneName;
                }
            }
            //var l = await _repositoryManager.Setting.GetSettingByValue("website_logo");
            //var logo =  await _imageBL.GetImageOriginal(l.Value);
            var all = "";
            var bb3 = "";
            var m3 = "";
            var img = "<div style='text-align: center;'><img style='width: 25%;/></div><hr>";//
            var bdy1 = "<table style='width: -webkit-fill-available;'><tbody style='display: contents;'>" +
                           "<tr><td style='color: #212121;line-height: 1.8;font-size: 20px;font-weight: 500;'>Name</td><td><span style='color: #471267 ;font-size: 20px;'> " + user.FirstName + " " + user.LastName + " </span><br /></td><td style='color: #212121;line-height: 1.8;font-size: 20px;font-weight: 500;'>Order date</td><td style='padding-right: 5%;'>" + "<span style='color: #471267 ;font-size: 20px;' > " + newOrder.CreatedAt.AddHours(3).ToString("MM/dd/yyyy") + " </span><br />" +
                           "<span style='color: #471267 ;font-size: 10px;'> " + newOrder.CreatedAt.AddHours(3).ToString("hh:mm tt") + " </span></td></tr><tr>" +
                           "<td style='color: #212121;line-height: 1.8;font-size: 20px;font-weight: 500;'> Email</td>" +
                            "<td style='color: #471267;font-size: 20px;padding-right: 5%; '>" + user.Email + "</td>" +
                                "<td style='color: #212121;line-height: 1.8;font-size: 20px;font-weight: 500;'>Mobile</td>" +
                            "<td style='color: #471267;font-size: 20px;padding-right: 5%; '>" + user.PhoneNumber + "</td>" +
                               "<td style='color: #212121;line-height: 1.8;font-size: 20px;font-weight: 500;'>Order Status</td>" +
                               "<td style='color: #471267;font-size: 20px;padding-right: 5%; '>" + sts + "</td>" +
                               "</tr><tr style='width: 100 %; display: inline-block;'><td style='color: #212121;line-height: 1.8;font-size: 20px;font-weight: 500;'>Order number</td><td style='color: #471267;font-size: 20px;padding-right: 5%; '> " + newOrder.Id + " </td><td style='color: #212121;line-height: 1.8;font-size: 20px;font-weight: 500;'>Order Time(Needed)</td><td style='color: #471267;font-size: 20px;padding-right: 5%; '> " + (time != null ? time.Time : "") + " </td></tr></table>"

                               + "<table style='text-align: left !important;width: 100% !important;max-width: 100% !important;' class='invoice-table shop_table shop_table_responsive cart woocommerce-cart-form__contents'><thead><tr><th style='color: #471267;font-size: 22px;font-weight: normal;'>Address</th>" +
                           "<td style ='color: #1a1a1b;font-size: 17px;font-weight: normal;'> " + address1 + " " + address2 + "<br/>" + city1 + "<br/>" + street + "</td>" +
                           "</tr ><tr style='display: inherit;'><th style='color: #471267;font-size: 20px;font-size: 22px;font-weight: normal;width: 50%;'><br>Order Notes</th><td style ='color: #1a1a1b;font-size: 15px;font-weight: normal;width: 50%;'> " + newOrder.Notes + "</tr></ table ><br><br>"
                            + "<div class='container' style='width: 530px;text-align:left;'></div>"
                           + "<br><hr><table style='text-align: left !important;width: 100% !important;max-width: 100% !important;' class='invoice-table shop_table shop_table_responsive cart woocommerce-cart-form__contents'><thead><tr><th style = 'color: #492f91;font-size: 20px;'>Description</th>" +
                          "<th style = 'color: #492f91;font-size: 20px;'>Price</th>" +
                          "<th style = 'color: #492f91;font-size: 20px;'>Qty.</th><th style = 'color: #492f91;font-size: 20px;'>Total</th></tr></thead><tbody>";
             var MyOrders = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(id);
            foreach (var item in MyOrders)
            {
                string menuTxt = "";
                var menu = item.Product;
                if (menu != null)
                {
                    menuTxt = menu.ProductName;
                }
                bb3 = "<tr><td style = 'color: #000;font-size: 17px;width: 40%;' >" + menuTxt + "</td>" +
                "<td style = 'color: #000;font-size: 17px;width:30%;' > " + item.FinalPrice.ToString("0.00") + " QAR</td>" +
                  "<td style = 'color: #000;font-size: 17px;width:30%;' >" + item.Qty + "</td>" +
                  "<td style='color: #000;font-size: 17px;width:30%;'>  QAR " + (item.FinalPrice * item.Qty).ToString("0.00") + "</td></tr>";
                sub = sub + (item.FinalPrice * item.Qty);
                m3 = m3 + bb3;
            }


            var t = "</tbody><br><br><tfoot style='width: 100%;display: inherit; '><tr><td colspan = '3' style='color: #492f91;font-size: 20px;'>SubTotal</td><td style = 'color: #000;font-size: 17px;'> QAR "
                + sub.ToString("0.00") + " </td></tr><tr><td colspan = '3' style='color: #492f91;font-size: 20px;'>Discount </td><td style = 'color: #000;font-size: 17px;' >"
                + newOrder.Coupon.CouponAmount + "</td></tr><tr><td colspan = '3' style='color: #492f91;font-size: 20px;'>Total</td><td style = 'color: #000;font-size: 17px;' > QAR "
                + newOrder.OrderPrice.ToString("0.00") + " </td></tr></tfoot></table>";

            all = img + bdy1 + m3 + t;
            return all;
        }

       //Coupon------------------------------------------------
        public async Task AddCoupon(CreateCouponDto createDto)
        {
            var coupon = _mapper.Map<Coupon>(createDto);
            //coupon.StoreId = 1;
            if ((createDto.DiscountType == "fixed_product" || createDto.DiscountType == "percent_product"))
            {
                if (createDto.Products != null && createDto.Products.Count() > 0)
                {
                    coupon.Product = createDto.Products == null ? null : string.Join(",", createDto.Products);
                }
            }
            _repositoryManager.Coupon.AddCoupon(coupon);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteCoupon(int id)
        {
            var cart = await _repositoryManager.Coupon.GetCouponId(id, false);
            _repositoryManager.Coupon.DeleteCoupon(cart);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateCoupon(UpdateCouponDto updateDto)
        {
            var coupon = await _repositoryManager.Coupon.GetCouponId(updateDto.Id, true);
            //coupon.StoreId = 1;
            if ((updateDto.DiscountType == "fixed_product" || updateDto.DiscountType == "percent_product"))
            {
                if (updateDto.Products != null && updateDto.Products.Count() > 0)
                {
                    coupon.Product = updateDto.Products == null ? null : string.Join(",", updateDto.Products);
                }
            }
            _mapper.Map(updateDto, coupon);
            await _repositoryManager.SaveAsync();
        }

        public bool CheckCodeCoupon(string code)
        {
            return _repositoryManager.Coupon.CheckExistCoupon(code);
        }
        public async Task<IEnumerable<Coupon>> GetCoupons()
        {
            return await _repositoryManager.Coupon.GetCoupons();
        } 
        public async Task<Coupon> GetCoupon(int id)
        {
            return await _repositoryManager.Coupon.GetCouponId(id , false);
        }
        //OrderStatus------------------------------------------------
        public async Task<List<OrderStatus>> GetOrderStatus()
        {
            return await _repositoryManager.OrderStatus.GetOrderStatusesList(false);
        }
        public async Task EditOrderStatus(UpdateOrderStatusDto update)
        {
            var status =  await _repositoryManager.OrderStatus.GetOrderStatusById(update.Id , true);
            _mapper.Map(update, status);
            await _repositoryManager.SaveAsync();
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
