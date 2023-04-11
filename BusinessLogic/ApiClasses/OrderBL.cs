
using AutoMapper;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Exception;
using Entities.Models;
using Entities.Models.Enums;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.ApiClasses
{
    public class OrderBL 
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper; 
        protected readonly LocationTaxBL _locationTaxBL;
        protected readonly ImageBL _imageBL;
        protected readonly ProductBL _productBL;
        protected readonly IEmailSender _emailSender; 
        protected readonly LocService _locService;
        protected readonly SignInManager<User> _signInManager; 
       // protected readonly LoggerManager _logger;
        public OrderBL(IRepositoryManager repositoryManager, IMapper mapper, LocationTaxBL locationTaxBL
            , ImageBL imageBL, ProductBL productBL, IEmailSender emailSender,  LocService locService , SignInManager<User> signInManager/*, LoggerManager logger*/ )
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _locationTaxBL = locationTaxBL;
            _imageBL = imageBL;
            _productBL = productBL;
            _emailSender = emailSender;
            _locService = locService;
            _signInManager = signInManager;
           // _logger = logger;
        }

        //Order------------------------------------------------
        public async Task<OrderDto> GetOrder(int id, int currencyId)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, false, true);
            var orderDto = _mapper.Map<OrderDto>(order);
            var store = await _repositoryManager.User.GetStoreId(order.StoreId);
            if (store != null)
            {
                decimal subTotal = 0;
                var products = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(id);
                var time = await _repositoryManager.DeliveryTime.GetDeliveryTimeById(order.DeliveryTimeId, false);
                var currency = await _repositoryManager.Currency.GetCurrency(currencyId, false);
                var states = await _repositoryManager.OrderStatus.GetOrderStatusById(order.OrderStatusId, false);
                var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
                var address = await _repositoryManager.Address.GetDefaultAddressCustomer(order.CustomerId);

                var orderProducts = new List<OrderProductDto>();
                foreach (var c in products)
                {
                    var attributes = await _repositoryManager.OrderAttributesProducts.GetAttributesOrderProduct(id, c.ProductId);

                    var attributesOrder = attributes.Select(c => new OrderAttributProductDto
                    {
                        Option = c.ProductAttribut.ProductOption.OptionName,
                        OptionType = c.ProductAttribut.ProductOption.OptionType,
                        Value = c.ProductAttribut.ProductOptionValue.OptionValueName,
                    }).ToList();
                    var image = await _repositoryManager.ImageProduct.GetAllImagesProductId(c.ProductId, false, true);
                    orderProducts.Add(new OrderProductDto
                    {
                        Qty = c.Qty,
                        ProductName = c.Product.ProductName,
                        ProductModel = c.Product.ProductModel,
                        ProductPrice = c.Product.Price,
                        ProductImage = _imageBL.GetImageOriginal(image.First().ImageId),
                        OrderAttributesProducts = attributesOrder ?? null
                    });
                    subTotal = subTotal + c.Qty * c.Product.Price;
                }
                orderDto.CustomerName = customer.FullName;
                orderDto.CustomerEmail = customer.Email;
                orderDto.CustomerPhone = customer.PhoneNumber;
                orderDto.AddressName = address != null ? address.AddressTitle : null;
                orderDto.AddressDetail = address != null ? $" {address.Address1} , {address.CityName} , {address.Street} ,  {address.Flat} " : null;
                orderDto.Currency = currency.Symbol;
                orderDto.StoreName = store.FirstName;
                orderDto.StoreEmail = store.Email;
                orderDto.StorePhone = store.PhoneNumber;
                orderDto.DeliveryTimeName = time.Time ?? null;
                orderDto.OrderStatusName = states.StatusName ?? null;
                orderDto.OrderProducts = orderProducts ?? null;
                orderDto.OrderPrice = subTotal;
                orderDto.TotalTax = order.TotalTax;
                orderDto.CouponAmount = order.Coupon == null ? 0 : order.Coupon.CouponAmount;
                orderDto.CouponCode = order.Coupon == null ? "" : order.Coupon.CouponCode;
                orderDto.Total = order.OrderPrice; 
                orderDto.OrderStatusEnum = states.OrderStatusEnum;
            }
            return orderDto;
        }
        public async Task<BussnessResultModel> AddOrder(int customerId, CreateOrderDto createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);
            decimal totalOrder = order.OrderPrice;
            if (createOrderDto != null)
            {
                var tax = await _locationTaxBL.GetTax(customerId);
                order.CustomerId = customerId;
                var status = await _repositoryManager.OrderStatus.GetOrderStatusEnum(OrderStatusEnum.OrderPending,false);
                order.OrderStatusId = status.Id;
                order.IsStatus = Status.NotActive;
                order.TotalTax = tax;
               
                if (createOrderDto.AddressId == 0)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("EnterField"), false);
                }
                if (createOrderDto.DeliveryTimeId == 0)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ChooseTime"), false);
                }
                var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
                if (carts.Count == 0 && carts == null)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("cartIsEmpty"), false);
                }
                else
                {
                    var orderProducts = new List<OrderProduct>();
                    foreach (var cart in carts)
                    {
                        decimal total = cart.FinalPrice;
                        var orderAttributs = new List<OrderAttributProduct>();
                        order.StoreId = cart.StoreId;

                        var inventory = await _repositoryManager.Inventory.GetStockProduct(cart.ProdId);
                        if (inventory == null)
                        {
                            return new BussnessResultModel(null, _locService.GetLocalizedStringValue("notAvOp"), false);
                        }

                        if (createOrderDto.CouponCode != null)
                        {
                            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(createOrderDto.CouponCode);
                            order.CouponId = coupon.Id;
                            if (coupon.CouponAmount > 0 && coupon.CouponAmount > total)
                            {
                                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CodeGrater"), false);
                            }
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
                                    foreach (var item in carts)
                                    {
                                        foreach (var productsCoupons in coupon.ProductsCoupons)
                                        {
                                            if (productsCoupons.ProductId == item.ProdId)
                                            {
                                                var newTotal = Convert.ToDecimal(item.Product.Price) - Convert.ToDecimal(coupon.CouponAmount);
                                                total -= newTotal;
                                            }
                                            else
                                            {
                                                total -= Convert.ToDecimal(item.Product.Price);
                                            }
                                        }
                                    }
                                }
                                else if (coupon.DiscountType == DiscountType.ProductPercentDiscount)
                                {
                                    var productsCoupons = await _repositoryManager.ProductsCoupon.GetAllProductsCouponId(coupon.Id, false);
                                    foreach (var productCoupon in productsCoupons)
                                    {
                                        if (productCoupon.ProductId == cart.ProdId)
                                        {
                                            total = total - (total * coupon.CouponAmount / 100);
                                        }
                                        else
                                        {
                                            total = total;
                                        }
                                    }
                                }
                            }
                        }
                        var cartAttributeProducts = await _repositoryManager.CartAttributeProduct.CartAttributeProductsCartId(cart.Id, false);
                        if (cartAttributeProducts.Count > 0)
                        {
                            foreach (var cartAttribute in cartAttributeProducts)
                            {
                                var inventoryAttribut = await _repositoryManager.Inventory.GetStockProductAttribut(cart.ProdId, cartAttribute.AttributesProductId);
                                if (inventoryAttribut == null)
                                {
                                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("notAvOp"), false);
                                }
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
                            FinalPrice = total,
                            OrderAttributesProducts = orderAttributs
                        });
                        totalOrder = orderProducts.Sum(c => c.FinalPrice);
                    }
                    order.OrderProducts = orderProducts;

                   
                }
                if (tax != 0)
                {
                    totalOrder = totalOrder + ((totalOrder * tax) / 100);
                }
                order.OrderPrice = totalOrder;
                _repositoryManager.Order.CreateOrder(order);
                await _repositoryManager.SaveAsync();
            }
            await DeleteByStore(order.StoreId, customerId);
            return new BussnessResultModel(totalOrder, _locService.GetLocalizedStringValue("PendOrdMsg"));
        }
        public async Task<BussnessResultModel> DeleteByStore(int storeId, int userId)
        {
            var carts = await _repositoryManager.Cart.GetCartsToStoreCustomer(storeId, userId);
            if (carts == null)
            {
                return new BussnessResultModel(null, "Please make sure the link", false);
            }
            else
            {
                foreach (var cart in carts)
                {
                    _repositoryManager.Cart.DeleteCart(cart);
                    await _repositoryManager.SaveAsync();
                }
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("successDelete"), false);
            }
        }
        public async Task<PagedList<OrderDto>> GetHistoryOrder(int customerId , int currencyId,PostsParameters postsParameters )
        {
            var orders = await _repositoryManager.Order.GetOrdersToCustomer(customerId);
            var currency = await _repositoryManager.Currency.GetCurrency(currencyId,false);
            var ordersDto = orders.Select(order => 
            {
                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.CountProduct = order.OrderProducts.Count();
                orderDto.CustomerName = order.Customer.FullName;
                orderDto.OrderStatusName = order.OrderStatus.StatusName;
                orderDto.CreatedAt = order.CreatedAt.ToString("MM/dd/yyyy hh:mm tt");
                return orderDto;
            });
            return PagedList<OrderDto>.ToPagedList(ordersDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<OrderProductDto>> GetOrderProducts( int orderId,  string lang)
        {
            var order = await _repositoryManager.Order.GetOrderId(orderId,false,true);
            if (order != null)
            {
                var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(orderId);
                if (orderProducts.Count() > 0)
                {
                    var orderProductsDto = orderProducts.Select(orderProduc =>
                    {
                        var orderProductDto = _mapper.Map<OrderProductDto>(orderProduc);
                        var product = _repositoryManager.Product.GetAcceptAdminActiveProduct(orderProduc.ProductId).Result;

                        orderProductDto.ProductName = lang == "en" ? product.ProductName : product.ProductNameAr;
                        orderProductDto.ProductModel = product.ProductModel;
                        orderProductDto.ProductImage = _imageBL.GetImageMedium(product.Images.First().ImageId);
                        orderProductDto.ProductPrice = product.Price;
                        return orderProductDto;
                    }).ToList();
                    return orderProductsDto;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        public async Task UpdateTotalOrderPrice(int id,int customerId, decimal totalPrice)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true,false);
            if (order == null)
            {
                // return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            order.OrderPrice = totalPrice;
            order.CustomerId = customerId;
            await _repositoryManager.SaveAsync();
        }
        public async Task<BussnessResultModel> OrderPending(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true ,false);
            if(order == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }

            var status = await _repositoryManager.OrderStatus.GetOrderStatusEnum(OrderStatusEnum.OrderPending, false);
            order.OrderStatusId = status.Id;
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(order, _locService.GetLocalizedStringValue("ChangeToPending"));
        }
        public async Task<BussnessResultModel> OrderComplete(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true, false);
            if (order == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }

            if (order.OrderStatus.OrderStatusEnum == OrderStatusEnum.OrderCompleted)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("AlreadyComp"), false);
            }
            if (order.OrderStatus.OrderStatusEnum != OrderStatusEnum.OrderShipped)
            {
                return new BussnessResultModel(null,  _locService.GetLocalizedStringValue("OrderNottPaid"), false);
            }
            var period = _repositoryManager.Setting.GetPeriod();
            if (order.OrderStatus.OrderStatusEnum == OrderStatusEnum.OrderShipped && order.DatePurchased.Value.AddDays(Convert.ToInt32(period)) <= DateTime.UtcNow)
            {
                var status = await _repositoryManager.OrderStatus.GetOrderStatusEnum(OrderStatusEnum.OrderCompleted, false);
                order.OrderStatusId = status.Id;
            }
            var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(id);
            foreach (var OrdPro in orderProducts)
            {
                var date = DateTime.UtcNow;
                var attributs = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(OrdPro.Id, false);
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
            string msgEm1 = await InvoiceOrder(id);
            var temp = await _repositoryManager.MessageTemplate.GetNameTemplate(NameTemplate.OrderCompleted); 
            var msgem = msgEm1 + temp.Message + "<br><br> The E-Tayf account team <br> Thank You";
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
            return new BussnessResultModel(order, _locService.GetLocalizedStringValue("OrdeCompleted"));
        }
        public async Task OrderCancal(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true, false);
            var status = await _repositoryManager.OrderStatus.GetOrderStatusEnum(OrderStatusEnum.OrderCanceled, false);
            order.OrderStatusId = status.Id;
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
            var order = await _repositoryManager.Order.GetOrderId(id, true, false);
            if (order == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            var status = await _repositoryManager.OrderStatus.GetOrderStatusById(order.OrderStatusId, false);
            if (status.OrderStatusEnum == OrderStatusEnum.OrderPending)
            {
                var status1 = await _repositoryManager.OrderStatus.GetOrderStatusEnum(OrderStatusEnum.OrderRejcted, false);
                order.OrderStatusId = status1.Id;
            }
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
            //var temp = await _repositoryManager.MessageTemplate.GetNameTemplate(NameTemplate.OrderRejected);
            //var msgem = msgEm1 + "<br><br> The E-Tayf account team <br> Thank You";
            //var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
            //try
            //{
            //    var message = new Message(new string[] { customer.Email }, "Order Details", msgem);
            //    _emailSender.SendEmail(message);
            //}
            //catch (Exception exp)
            //{
            //    //  _logger.LogError("could not send a rejection email to customer, order id =" + id, exp);
            //}
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(order, _locService.GetLocalizedStringValue("OrderRejected"));
        }
        public async Task OrderReceived(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true, false);
            if (order == null)
            {
               // return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            var status = await _repositoryManager.OrderStatus.GetOrderStatusEnum(OrderStatusEnum.OrderReceived, false);
            order.OrderStatusId = status.Id;

            var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(id);
            foreach (var OrdPro in orderProducts)
            {
                var date = DateTime.UtcNow;
                var attributs = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(OrdPro.Id, false);
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

            //send email 
            string msgEm1 = await InvoiceOrder(id);
            var temp = await _repositoryManager.MessageTemplate.GetNameTemplate(NameTemplate.OrderRecieved); //recived email
            var msgem = msgEm1 + temp.Message + "<br><br> The E-Tayf account team <br> Thank You";
            var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
            var message = new Message(new string[] { customer.Email }, "Order Details", msgem);
            _emailSender.SendEmail(message);
        }
        public async Task<BussnessResultModel> ShippedOrder(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true, false);
            if (order == null)
            {
                return new BussnessResultModel(null,_locService.GetLocalizedStringValue("correctLink") , false);
            }
            if (order.OrderStatus.OrderStatusEnum != OrderStatusEnum.OrderReceived || String.IsNullOrEmpty(order.Transaction))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("notPaid"), false);
            }
            if (order.OrderStatus.OrderStatusEnum == OrderStatusEnum.OrderReceived)
            {
                var status = await _repositoryManager.OrderStatus.GetOrderStatusEnum(OrderStatusEnum.OrderShipped, false);
                order.OrderStatusId = status.Id;
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
             string msgEm1 = await InvoiceOrder(id);

            //send email
            var temp = await _repositoryManager.MessageTemplate.GetNameTemplate(NameTemplate.OrderShipped);
            var msgem = msgEm1 + "<br><br> The E-Tayf account team <br> Thank You";
            var customer = await _repositoryManager.User.GetCustomerId(order.CustomerId, false);
            try
            {
                var message = new Message(new string[] { customer.Email }, "Order Details", msgem);
                _emailSender.SendEmail(message);
            }
            catch (Exception exp)
            {
                 //_logger.LogError($"COULD NOT SEND EMAIL: " + exp.Message, exp);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(order,  _locService.GetLocalizedStringValue("Toshipped"));
        }
        public async Task GetFailOrder(int orderId)
        {
            var order = await _repositoryManager.Order.GetOrderId(orderId, false, false);
            if (order != null)
            {
                var user = await _repositoryManager.User.GetUserId(order.CustomerId,false);
                if (user != null)
                {
                     await _signInManager.PasswordSignInAsync(user.Email, user.PasswordHash, true,false);
                }
            }
        }
        public async Task<BussnessResultModel> DeleteOrder(int orderId)
        {
            var order = await _repositoryManager.Order.GetOrderId(orderId, false, false);
            if (order.OrderStatusId != 1)
            {
                return new BussnessResultModel(null, "Cann't Delete This Order", false);
            }
            if (order != null)
            {
                var orderProducts = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(orderId);
                foreach (var orderProduct in orderProducts)
                {
                    _repositoryManager.OrderProducts.DeleteOrderProduct(orderProduct);
                    var attributes = await _repositoryManager.OrderAttributesProducts.GetAllOrderAttributesProducts(orderProduct.Id, false);
                    if (attributes != null)
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
            return new BussnessResultModel(order, "successDelete");
        }
       
        public async Task<List<OrderDto>> GetAllSalesOrders(int userId , string search, int customerId, int storeId, int statusId, DateTime? dateFrom, DateTime? dateTo)
        {
            var orders = await _repositoryManager.Order.GetAllOrders(search, customerId, storeId, statusId, dateFrom, dateTo);
          
            var store = await _repositoryManager.User.GetActiveUserId(userId,false);
            if(store.UserType == UserType.Store)
            {
                orders = orders.Where(c => c.StoreId == userId).ToList();
            }
            var ordersDto = orders.Select(order=> 
            {
                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.CustomerName = order.Customer.FullName;
                orderDto.CodeMobileCountry = order.Customer.CodeMobileCountry;
                orderDto.Currency = "$";
                orderDto.CustomerPhone = order.Customer.PhoneNumber;
                orderDto.StoreName = order.Store.FullName;
                orderDto.StorePhone = order.Store.PhoneNumber;
                orderDto.CreatedAt = order.CreatedAt.ToString("MM/dd/yyyy hh:mm tt");
                orderDto.OrderStatusName = order.OrderStatus.StatusName ?? null;
                orderDto.Total = orders.Sum(c => c.OrderPrice);
                return orderDto;
           }).Take(100).ToList();
            return ordersDto;
        } 
        public async Task<PagedList<OrderDto>> GetAllOrders(int userId  ,int customerId  ,int storeId ,int statusId  ,string search ,PostsParameters postsParameters)
        {
            var orders = await _repositoryManager.Order.GetAllOrders(search, customerId,storeId, statusId, null,null);
            var store = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                orders = orders.Where(c => c.StoreId == userId).ToList();
            }
            var ordersDto = orders.Select(order =>
            {
                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.CustomerName = order.Customer.FullName;
                orderDto.Currency = "$";
                orderDto.CustomerEmail = order.Customer.Email;
                orderDto.CustomerPhone = order.Customer.PhoneNumber;
                orderDto.StoreName = order.Store.FirstName;
                orderDto.StoreEmail = order.Store.Email;
                orderDto.StorePhone = order.Store.PhoneNumber;
                orderDto.CreatedAt = order.CreatedAt.ToString("MM/dd/yyyy hh:mm tt");
                orderDto.DatePurchased = (!String.IsNullOrEmpty(order.DatePurchased.ToString()) ? order.DatePurchased.Value.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss tt") : "");
                orderDto.DeliveryTimeName = order.DeliveryTime.Time ?? null;
                orderDto.OrderStatusName = order.OrderStatus.StatusName ?? null;
                orderDto.OrderStatusEnum = order.OrderStatus.OrderStatusEnum; 
                orderDto.CountProduct = order.OrderProducts.Count();
                return orderDto;
            }).ToList();
            return PagedList<OrderDto>.ToPagedList(ordersDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<string> InvoiceOrder(int id)
        {
            decimal sub = 0;
            var newOrder = await _repositoryManager.Order.GetOrderId(id, false, false);
            var MyOrders = await _repositoryManager.OrderProducts.GetAllProductsToOrderId(id);

            var user = await _repositoryManager.User.GetUserId(newOrder.CustomerId, false);
            var time = await _repositoryManager.DeliveryTime.GetDeliveryTimeById(newOrder.DeliveryTimeId, false);

            string sts = "";
            var state = await _repositoryManager.OrderStatus.GetOrderStatusById(newOrder.OrderStatusId, false);
            if (state != null)
            {
                sts = state.StatusName;
            }
            string address1 = "";
            string address2 = "";
            string city1 = "";
            string street = "";
            var c2 = await _repositoryManager.Address.GetAddressIdByCustomerId(newOrder.AddressId, newOrder.CustomerId, false);
            if (c2 != null)
            {
                street = c2.Street;
                address1 = c2.Address1;
                address2 = c2.Address2;
                //decimal tax = 0;
                var city = await _repositoryManager.Zone.GetZoneId(c2.ZoneId, false);
                if (city != null)
                {
                    city1 = city.ZoneName;
                }
            }
            var setting = await _repositoryManager.Setting.GetSettingByValue("website_logo",false);
            var logo = _imageBL.GetImageOriginal(Convert.ToInt32(setting.Value));
            var all = "";
            var bb3 = "";
            var m3 = "";
            var img = "<div style='text-align: center;'><img src=" + logo +"style='width: 25%;/></div><hr>";//
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

            foreach (var item in MyOrders)
            {
                string menuTxt = "";
                var menu = await _repositoryManager.Product.GetProductById(item.ProductId, false);
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
            var cop = " ";
            var copoun = await _repositoryManager.Coupon.GetCouponIdNotFinished(Convert.ToInt32(newOrder.CouponId));
            if (copoun != null)
            {
                cop = copoun.CouponCode;
            }
            var t = "</tbody><br><br><tfoot style='width: 100%;display: inherit; '><tr><td colspan = '3' style='color: #492f91;font-size: 20px;'>SubTotal</td><td style = 'color: #000;font-size: 17px;'> QAR "
                + sub.ToString("0.00") + " </td></tr><tr><td colspan = '3' style='color: #492f91;font-size: 20px;'>Discount </td><td style = 'color: #000;font-size: 17px;' >"
                + cop + "</td></tr><tr><td colspan = '3' style='color: #492f91;font-size: 20px;'>Total</td><td style = 'color: #000;font-size: 17px;' > QAR "
                + newOrder.OrderPrice.ToString("0.00") + " </td></tr></tfoot></table>";

            all = img + bdy1 + m3 + t;
            return all;
        }
        //Coupon------------------------------------------------
        public async Task<List<ProductDto>> GetAllPrductsToCoupon()
        {
            var products = await _repositoryManager.Product.GetAllProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return productsDto;
        }
        public async Task<BussnessResultModel> AddCoupon(CreateCouponDto createDto, int userId)
        {
            var IsExists = _repositoryManager.Coupon.CheckExistCoupon(createDto.CouponCode);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
            }
            var coupon = _mapper.Map<Coupon>(createDto);

            var user = await _repositoryManager.User.GetUserId(userId, false);
            if (user.UserType == UserType.Admin)
            {
                coupon.AdminId = userId;
            }
            else
            {
                coupon.StoreId = userId;
            }
            if ((createDto.DiscountType == DiscountType.ProductDiscount || createDto.DiscountType == DiscountType.ProductPercentDiscount) && (createDto.ProductsCoupons == null || createDto.ProductsCoupons.Count() == 0))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue(" Please Choose Products"),false);
            }
            if (createDto.DiscountType == DiscountType.ProductDiscount || createDto.DiscountType == DiscountType.ProductPercentDiscount && (createDto.ProductsCoupons != null || createDto.ProductsCoupons.Count() > 0))
            {
              //  coupon.ProductsCoupons.AddRange(_mapper.Map<List<ProductsCoupon>>(createDto.ProductsCoupons));
            }
            _repositoryManager.Coupon.AddCoupon(coupon);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(coupon, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> DeleteCoupon(int id)
        {
            var coupon = await _repositoryManager.Coupon.GetCouponId(id, false);
            if (coupon == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            var products = await _repositoryManager.ProductsCoupon.GetAllProductsCouponId(id, false);
            if(products != null)
            {
                foreach(var item in products)
                {
                    _repositoryManager.ProductsCoupon.DeleteProductsCoupon(item);
                }
            }
            _repositoryManager.Coupon.DeleteCoupon(coupon);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(coupon, _locService.GetLocalizedStringValue("successDelete"));
        }
        public async Task<BussnessResultModel> UpdateCoupon(UpdateCouponDto updateDto, int userId )
        {
            var coupon = await _repositoryManager.Coupon.GetCouponId(updateDto.Id, true);
            if(coupon == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"),false);
            }
            var user = await _repositoryManager.User.GetUserId(userId, false);
            if (user.UserType == UserType.Admin)
            {
                coupon.AdminId = userId;
            }
            else
            {
                coupon.StoreId = userId;
            }
            _mapper.Map(updateDto, coupon);
            await _repositoryManager.SaveAsync();
            if (updateDto.DiscountType == DiscountType.ProductDiscount || updateDto.DiscountType == DiscountType.ProductPercentDiscount)
            {
                  var products = await _repositoryManager.ProductsCoupon.GetAllProductsCouponId(updateDto.Id, false);
                var Ids = products.Select(x => x.Id);
                var IdsDto = updateDto.ProductsCoupons.Select(x => x.Id);
                var listToDelete = Ids.Except(IdsDto).ToList();

                await _repositoryManager.ProductsCoupon.DeleteRowRange(listToDelete);

                var listToAdd = updateDto.ProductsCoupons.Where(x => x.Id == 0);

                var entity = _mapper.Map<List<ProductsCoupon>>(listToAdd);
                foreach (var item in entity)
                {
                    item.ProductId = updateDto.Id;
                }
                _repositoryManager.ProductsCoupon.CreatProductsCouponRange(entity);

                var listToUpdate = Ids.Intersect(IdsDto);

                foreach (var item in listToUpdate)
                {
                    var itemEntity = await _repositoryManager.ProductsCoupon.GetAllProductsCouponId(item, true);
                    var dtoEntity = updateDto.ProductsCoupons.First(x => x.Id == item);
                    _mapper.Map(dtoEntity, itemEntity);
                }
                await _repositoryManager.SaveAsync();
            }
           
            return new BussnessResultModel(coupon, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<PagedList<CouponDto>> GetAllCoupons(string search , PostsParameters postsParameters)
        {
            var coupons = await _repositoryManager.Coupon.GetCoupons(search);
            var couponsDto = coupons.Select(coupon =>
            {
                var couponDto = _mapper.Map<CouponDto>(coupon);
                if (coupon.DiscountType == DiscountType.CartDiscount)
                {
                    couponDto.DiscountType = _locService.GetLocalizedStringValue("CartDiscount");
                }
                else if (coupon.DiscountType == DiscountType.CartPercentDiscount)
                {
                    couponDto.DiscountType = _locService.GetLocalizedStringValue("Cart2Discount");
                }
                else if (coupon.DiscountType == DiscountType.ProductDiscount)
                {
                    couponDto.DiscountType = _locService.GetLocalizedStringValue("ProductDiscount");
                }
                else
                {
                    couponDto.DiscountType = _locService.GetLocalizedStringValue("Product2Discount");
                }
                return couponDto;
            }).ToList();
            return PagedList<CouponDto>.ToPagedList(couponsDto, postsParameters.PageNumber, postsParameters.PageSize);
        } 
       
        //OrderStatus------------------------------------------------
        public async Task<PagedList<OrderStatusDto>> GetOrderStatus(string lang  , PostsParameters postsParameters)
        {
            var orderStatuses = await _repositoryManager.OrderStatus.GetOrderStatusesList(false);
            var model = orderStatuses.Select(status=>
                {
                    var statusDto = _mapper.Map<OrderStatusDto>(status);
                    statusDto.StatusName = lang == "en" ? status.StatusName : status.StatusNameAr;
                    statusDto.Option = lang == "en" ? (status.IsStatus == Status.Active ? "Yes" : "No") : (status.IsStatus == Status.Active ? "نعم" : "لا");
                    return statusDto;
                }).ToList();
            return PagedList<OrderStatusDto>.ToPagedList(model, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<OrderStatusDto>> GetAllOrderStatus(string lang)
        {
            var orderStatuses = await _repositoryManager.OrderStatus.GetOrderStatusesList(false);
            var model = orderStatuses.Select(status =>
            {
                var statusDto = _mapper.Map<OrderStatusDto>(status);
                statusDto.StatusName = lang == "en" ? status.StatusName : status.StatusNameAr;
                statusDto.Option = status.IsStatus == Status.Active ? _locService.GetLocalizedStringValue("Yes") : _locService.GetLocalizedStringValue("No");
                return statusDto;
            }).ToList();
            return model;
        }
        public async Task<BussnessResultModel> EditOrderStatus(UpdateOrderStatusDto update)
        {
            var status =  await _repositoryManager.OrderStatus.GetOrderStatusById(update.Id , true);
            if(status == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink") , false); 
            }
            _mapper.Map(update, status);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(status, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<UpdateOrderStatusDto> GetUpdateOrderStatus(int id)
        {
            var status = await _repositoryManager.OrderStatus.GetOrderStatusById(id, false);
            var statusDto = _mapper.Map<UpdateOrderStatusDto>(status);
            return statusDto;
        }
        //Payment------------------------------------------------
        public async Task<PagedList<PaymentDto>> GetAllPayments(string search, PostsParameters postsParameters)
        {
            var payments =  await _repositoryManager.PaymentMethods.GetPaymentMethods(search);
            var paymentsDto = _mapper.Map<List<PaymentDto>>(payments);
            return PagedList<PaymentDto>.ToPagedList(paymentsDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        //DeliveryTime------------------------------------------------
        public async Task<DeliveryTimeDto> GetTimeById(int timeId)
        {
            var time = await _repositoryManager.DeliveryTime.GetDeliveryTimeById(timeId , false);
            var timeDto = _mapper.Map<DeliveryTimeDto>(time);
            return timeDto;
        }
        
    }
}
