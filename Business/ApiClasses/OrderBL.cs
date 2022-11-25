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
        public async Task UpdateTotalOrderPrice(int id, decimal totalPrice)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderPrice = totalPrice;
            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}", order.Id, order.OrderPrice.ToString("0.00")));
            await _repositoryManager.SaveAsync();
        }
        //public async Task AddOrder(CreateOrderDto createOrderDto)
        //{
        //    var order = _mapper.Map<Order>(createOrderDto);
        //    order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}", order.Id, order.OrderPrice.ToString("0.00")));
        //    order.OrderStatusId = 2;
        //    order.CustomerId = 2;
        //    order.CurrencyId = 1;
        //    order.StoreId = 1;
        //    order.PaymentMethodsId = 7;
        //    order.IsSeen = 0;
        //    var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(createOrderDto.CouponCode);
        //    if (coupon != null)
        //    {
        //        coupon.CouponCode = createOrderDto.CouponCode;
        //    }
        //    _repositoryManager.Order.CreateOrder(order);
        //    try
        //    {
        //        await _repositoryManager.SaveAsync();
        //    }
        //    catch (Exception) { }
        //}
        public async Task OrderPending(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 2;
            await _repositoryManager.SaveAsync();
        }

        public async Task OrderComplete(int id)
        {
            var order = await _repositoryManager.Order.GetOrderId(id, true);
            order.OrderStatusId = 3;
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
            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(updateOderDto.CouponCode);
            if (coupon != null)
            {
                order.Coupon.CouponCode = code;
                order.Coupon.CouponAmount = amount;
               // order.OrderPrice = updateOderDto.OrderPrice;
            }
            else
            {
                order.Coupon.CouponCode = "";
                order.Coupon.CouponAmount = 0;
             //   order.OrderPrice = totalBeforCode + (totalBeforCode * (updateOderDto.TotalTax / 100));
            }
            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}",order.Id, order.OrderPrice.ToString("0.00")));

            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateStatusOrder(/*UpdateOderDto updateOderDto, */ int id, int vendorId, int adminId)
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
                        FirstName = customer.FullName,
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

        public async Task AddOrder(CreateOrderDto createOrderDto)
        {
            var storeId = 1;
            var customerId = 2;
            var order = _mapper.Map<Order>(createOrderDto);
            order.StoreId = storeId;
            order.CustomerId = customerId;
            order.OrderStatusId = 2;
            order.CurrencyId = 1;
            order.PaymentMethodsId = 7;
            order.IsSeen = 0;
            order.TotalTax = await _locationTaxBL.GetTax(customerId);
            order.HashedCtpAndPayment = ComputeSha256Hash(string.Format("CSP={0};Amount={1}", order.Id, order.OrderPrice.ToString("0.00")));

            //if(createOrderDto.OrderProducts != null)
            //{
            //    var carts = await _repositoryManager.Cart.GetCartsToCustomerId(customerId);
            //    foreach(var cart in carts)
            //    {
            //        cart.CartAttributeProducts.Where(c => c.CartId == cart.Id);
            //    }
            //}
           
            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(createOrderDto.CouponCode);
            if (coupon != null)
            {
                coupon.CouponCode = createOrderDto.CouponCode;
            }
            _repositoryManager.Order.CreateOrder(order);
            try
            {
                await _repositoryManager.SaveAsync();
            }
            catch (Exception) { }
        }

        //public async Task<decimal> AddOrder2(CreateOrderDto createOrderDto)
        //{
        //    int oid = 0; decimal tot = 0; decimal all = 0;
        //    if (createOrderDto != null)
        //    {
        //        decimal cop_amount = 0; var discount_type = ""; var coponCode = "";
        //        if (createOrderDto.CouponCode != null)
        //        {
        //            var coupon = await _repositoryManager.Coupon.GetCouponCodeNotFinished(createOrderDto.CouponCode);
        //            if (coupon != null)
        //            {
        //                coupon.CouponCode = createOrderDto.CouponCode;
        //            }
        //        }
        //        int CustomerId = 2;

        //        decimal prev_tot = 0;
        //        var prods = await _repositoryManager.Cart.GetCartsToCustomerId(CustomerId);
        //        foreach (var p in prods)
        //        {
        //            var prd = p.Product;
        //            if (prd != null)
        //            {
        //                prev_tot += (prd.Price * p.Qty);
        //            }
        //        }

        //        decimal tax = await _locationTaxBL.GetTax(CustomerId);


        //        //foreach (var b in obj.stores)
        //        //{
        //            var all_prods = new List<OrderProduct>();
        //            foreach (var p in createOrderDto.OrderProducts)
        //            {
        //                var cartItem = _cartBL.getCartById(p.prod_id);
        //                var prd = _cartBL.getProduct(Convert.ToInt32(cartItem.products_id));   // Find(p.prod_id);
        //                if (cartItem != null)
        //                {
        //                    var attr_prod = db.products_attributes.Where(r => r.products_id == prd.products_id);
        //                    var det = db.customers_basket_attributes.Where(r => r.customers_basket_id == p.prod_id).ToList();
        //                    decimal prod_price = prd.products_price;
        //                    var flash = _cartBL.getFlashById(prd.products_id);
        //                    if (flash != null)
        //                    {
        //                        prod_price = flash.flash_sale_products_price;
        //                    }

        //                    var special = _cartBL.getSpecialById(prd.products_id);
        //                    if (special != null)
        //                    {
        //                        prod_price = special.specials_new_products_price;
        //                    }
        //                    var orde_det = new List<orders_products_attributes>();

        //                    if (det.Count() > 0)
        //                    {
        //                        foreach (var d in det)
        //                        {
        //                            var opattr = attr_prod.Where(r => r.products_id == prd.products_id && r.options_id == d.products_options_id && r.options_values_id == d.products_options_values_id).FirstOrDefault();
        //                            if (opattr != null)
        //                            {
        //                                if (opattr.price_prefix == "+")
        //                                {
        //                                    prod_price += opattr.options_values_price;
        //                                }
        //                                if (opattr.price_prefix == "-")
        //                                {
        //                                    if (prod_price != 0)
        //                                    {
        //                                        prod_price -= opattr.options_values_price;
        //                                    }
        //                                }
        //                                var getop = db.products_options.Where(op => op.products_options_id == opattr.options_id).FirstOrDefault();
        //                                var getopVal = db.products_options_values.Where(t => t.products_options_values_id == opattr.options_values_id).FirstOrDefault();
        //                                orde_det.Add(new orders_products_attributes
        //                                {
        //                                    products_id = prd.products_id,
        //                                    options_attribute_id = opattr.products_attributes_id,
        //                                    price_prefix = opattr.price_prefix,
        //                                    options_id = opattr.options_id,
        //                                    options_value_id = opattr.options_values_id,
        //                                    options_values_price = opattr.options_values_price,
        //                                    products_options = (getop != null ? (getop.OptionsNames != null ? getop.OptionsNames[lang] : "") : ""),
        //                                    products_options_values = (getopVal != null ? (getopVal.OptionsValuesNames != null ? getopVal.OptionsValuesNames[lang] : "") : "")
        //                                });

        //                            }

        //                        }
        //                    }

        //                    if (prd != null)
        //                    {

        //                        all_prods.Add(new orders_products
        //                        {
        //                            products_quantity = p.qty,
        //                            products_model = prd.products_model,
        //                            products_name = prd.products_name,
        //                            products_id = prd.products_id,
        //                            products_tax = 0,
        //                            vendor_id = prd.vendorId,
        //                            products_price = prod_price,
        //                            final_price = p.qty * prod_price,
        //                            orders_products_attributes = orde_det
        //                        });


        //                    }

        //                }
        //            }

        //            decimal sub_tot = all_prods.Sum(r => r.final_price);
        //            var stor = new order
        //            {
        //                total_tax = tax,
        //                shipping_cost = 0,
        //                order_status = 1,
        //                customers_id = Convert.ToInt32(CustomerID),
        //                is_delete = false,
        //                shipping_method = "",
        //                payment_method = "Qatar Charity",// obj.paym;
        //                order_information = obj.notes,
        //                address_id = obj.address_id,
        //                currency = "QAR",
        //                currency_value = 1,
        //                times = obj.times,
        //                is_seen = 0,
        //                coupon_code = coponCode,
        //                orders_products = all_prods,
        //                coupon_amount = cop_amount,
        //                created_at = DateTime.UtcNow,
        //                ordered_source = 1, // 1 mobile , 2 web
        //                product_ids = "",
        //                order_price = 0,
        //                free_shipping = 1,
        //                vendor_id = b.store_id,
        //                billing_phone = ""
        //            };
        //            var strObj = _cartBL.AddOrderByItem(stor);
        //            oid = strObj.orders_id;
        //            try
        //            {
        //                Log.Info(string.Format("{0}  order added by Website with id ", oid));
        //            }
        //            catch (Exception e) { }
        //            int Customerid = Convert.ToInt32(CustomerID);

        //            if (discount_type == "fixed_cart")
        //            {
        //                sub_tot -= cop_amount;
        //            }
        //            else if (discount_type == "percent")
        //            {
        //                sub_tot = (sub_tot - (sub_tot * (cop_amount / 100)));
        //            }
        //            else if (discount_type == "fixed_product")
        //            {
        //                var copon = _cartBL.GetCouponById(obj.coupon);

        //                if (copon != null)
        //                {
        //                    sub_tot = 0;
        //                    List<string> SelectedValues = new List<string>();
        //                    string[] split = copon.product_ids.Split(',');
        //                    foreach (var t in split)
        //                    {
        //                        if (!String.IsNullOrEmpty(t))
        //                        {
        //                            SelectedValues.Add(t.ToString());
        //                        }
        //                    }
        //                    ViewBag.SelectedValues = SelectedValues;
        //                    foreach (var t in b.prods)
        //                    {
        //                        var bsk = _cartBL.getProductCartByProduct(t.prod_id, Convert.ToInt32(CustomerID));
        //                        if (bsk != null)
        //                        {
        //                            if (SelectedValues.Contains(t.prod_id.ToString()))
        //                            {
        //                                //iscode = true;
        //                                decimal newval = Convert.ToDecimal(bsk.final_price) - cop_amount;
        //                                sub_tot += newval;
        //                            }
        //                            else
        //                            {
        //                                sub_tot += Convert.ToDecimal(bsk.final_price);
        //                            }
        //                        }

        //                    }
        //                }
        //            }
        //            else if (discount_type == "percent_product")
        //            {
        //                var copon = _cartBL.GetCouponById(obj.coupon);
        //                if (copon != null)
        //                {
        //                    sub_tot = 0;
        //                    List<string> SelectedValues = new List<string>();
        //                    string[] split = copon.product_ids.Split(',');
        //                    foreach (var t in split)
        //                    {
        //                        if (!String.IsNullOrEmpty(t))
        //                        {
        //                            SelectedValues.Add(t.ToString());
        //                        }
        //                    }
        //                    ViewBag.SelectedValues = SelectedValues;
        //                    foreach (var t in b.prods)
        //                    {

        //                        var bsk = _cartBL.getProductCartByProduct(t.prod_id, Convert.ToInt32(CustomerID));
        //                        if (bsk != null)
        //                        {
        //                            if (SelectedValues.Contains(t.prod_id.ToString()))
        //                            {
        //                                decimal newval = Convert.ToDecimal(bsk.final_price) * (cop_amount / 100);
        //                                sub_tot += newval;
        //                            }
        //                            else
        //                            {
        //                                sub_tot += Convert.ToDecimal(bsk.final_price);
        //                            }
        //                        }
        //                    }

        //                }
        //            }


        //            if (tax > 0)
        //            {
        //                sub_tot = (sub_tot + (sub_tot * (tax / 100)));
        //            }
        //            tot = sub_tot;
        //            _cartBL.updateTotal(stor.orders_id, tot);
        //            oid = stor.orders_id;
        //            all += tot;

        //        }

        //    }

        //    _cartBL.deleteOrderItems(Convert.ToInt32(CustomerID), obj);
        //    return Decimal.Round(all, 2);

        //}
    }
}
