using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using AutoMapper;
using Contracts;
using Entities;
using System.Web.Http.ModelBinding;
using System.Net;
using System.Web.Http;
using System.Data;
using MailKit.Security;
using MailKit.Net.Smtp;
using EmailService;
using Microsoft.Extensions.Hosting;
using Entities.Models.Enum;

namespace BusinessLogic.ApiClasses
{
    public class UserBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper; 
        private readonly Util _util;
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly LocService _locService;
        protected readonly IEmailSender _emailSender;
        protected readonly RoleManager<Role> _roleManager;  
        protected readonly LocationTaxBL _locationTaxBL;
        protected readonly IAuthenticationManager _authManager;
        protected readonly ILoggerManager _logger;

        public UserBL(IRepositoryManager repositoryManager, IMapper mapper , Util util, UserManager<User> userManager,  IEmailSender emailSender
            , LocService locService , RoleManager<Role> roleManager , LocationTaxBL locationTaxBL , SignInManager<User> signInManager,
            IAuthenticationManager authManager , ILoggerManager logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _util = util;
            _userManager = userManager;
            _emailSender = emailSender;
            _locService = locService;
            _roleManager = roleManager;
            _locationTaxBL = locationTaxBL;
            _signInManager = signInManager;
            _authManager = authManager;
            _logger = logger;
           // _sMSSender = sMSSender;
        }
        //Role------------------------------------------------
        public async Task<List<Role>> GetTypesStoreAdmin()
        {
            return await _repositoryManager.Role.GetRolesAdminStore();
        } 
        public async Task<Role> GetRoleId(int roleId)
        {
            return await _repositoryManager.Role.GetRoleId(roleId , false);
        }
        public async Task<BussnessResultModel> SaveRole(int roleId, int[] linkIds)
        {
            var permissions = await _repositoryManager.Permission.GetPermissionsRole(roleId, true);
            foreach (var permission in permissions)
            {
                _repositoryManager.Permission.DeletePermission(permission);
            }

            foreach (int linkId in linkIds)
            {
                _repositoryManager.Permission.AddPermission(new Permission { RoleId = roleId, LinkId = linkId });
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(permissions, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> AddRole(CreateRoleDto create)
        {
            var IsExists = _repositoryManager.Role.IsExistRole(create.Name);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
            }
            var role = await _roleManager.CreateAsync(new Role
            {
                Name = create.Name,
                NormalizedName = create.Name.ToUpper(),
                IsStatus = create.IsStatus,
                IsVendorLink = create.IsVendorLink == null ? false : true,
            }) ;
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(role, _locService.GetLocalizedStringValue("successAdd"));
        } 
        public async Task<BussnessResultModel> EditRole(UpdateRoleDto create)
        {
            var role = await _repositoryManager.Role.GetRoleId(create.Id, true);
            if (role != null)
            {
                _mapper.Map(create, role);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(role, _locService.GetLocalizedStringValue("successSave"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
        }
        public async Task<BussnessResultModel> DeleteRole(int id)
        {
            var role = await _repositoryManager.Role.GetRoleId(id, true);
            if (role != null)
            {
                role.IsStatus = Status.NotActive;
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(role, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        }
        //Store------------------------------------------------
        public async Task<List<StoreDto>> GetStores()
        {
            var stores = await _repositoryManager.User.GetAllStores(false);
            var storesDto = _mapper.Map<List<StoreDto>>(stores);
           // var storeDto = storesDto.First();
            //foreach (var store in stores)
            //{
            //    storeDto.Avater = await _imageBL.GetImageThumbnail(store.Avater);
            //}
            return storesDto;
        } 
        public async Task<IEnumerable<User>> GetStoreList(int PageId = 1, int rows = 10)
        {
           return await _repositoryManager.User.GetStoreList(PageId , rows);
        } 
        public int GetStoresCount()
        {
           return  _repositoryManager.User.GetStoresCount();
        }
        public async Task<User> GetStoreId(int storeId)
        {
           return await _repositoryManager.User.GetStore(storeId , false);
        }
        public async Task<List<StoreDto>> GetSomeStores()
        {
            var stores = await _repositoryManager.User.Get10Stores();
            var storesDto = _mapper.Map<List<StoreDto>>(stores);
           
            return storesDto;
        }
        public async Task<StoreDto> GetStore(int id)
        {
            var store = await _repositoryManager.User.GetStoreId(id);
            var storeDto = _mapper.Map<StoreDto>(store);
           // storeDto.Avater = await _imageBL.GetImageMedium(store.Avater);
            return storeDto;
        }
        public async Task<BussnessResultModel> AddStore(CreateStoreDto create)
        {
            var store = _mapper.Map<User>(create);
            //store.FirstName = create.NameStore;
            store.LastName = "Store";
            store.RoleId = 3;
            store.PhoneNumber = create.PhoneNumber;
            store.UserName = create.Email;
            store.VerifiedCode = 1234;
            store.IsMobileVerified = false;
            store.Status = Status.Active;
            store.TypeRegister = TypeRegister.Normal;
            var result = await _userManager.CreateAsync(store, create.Password);
            if (!result.Succeeded)
            {
                string errors = "";
                foreach (var x in result.Errors)
                     errors += x + ", ";
                if (store.Lang == "en")
                {
                    return new BussnessResultModel(null, errors, false) ;
                }
                else
                {
                    return new BussnessResultModel(null, "e: حدث خطأ يرجى التأكد من البيانات", false) ;
                }
            }
            return new BussnessResultModel(store, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> UpdateStore(UpdateStoreDto update)
        {
            var store = await _repositoryManager.User.GetStore(update.Id, true);
            if(store == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            store.FirstName = update.NameStore;
            store.LastName = "Store";
            _mapper.Map(update, store);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(store, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> DeleteStore(int id)
        {
            var store = await _repositoryManager.User.GetStore(id, false);
            if (store != null)
            {
                var orders = await _repositoryManager.Order.GetOrdersToStore(id);
                var products = await _repositoryManager.Product.GetProductsTOStoreId(id);
                if(orders != null || products != null)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("CannotDeleteStoreBescouseHaveOrdersAndProducts") , false);
                }
                _repositoryManager.User.DeleteUser(store);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(store, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        }
        //user------------------------------------------------
        public async Task<User> GetUserById(int id)
        {
            return await _repositoryManager.User.GetUserId(id, false);
        }
        public async Task<IEnumerable<User>> GetAdminsStors()
        {
            return await _repositoryManager.User.GetAdminsStors(false);
        }
        public async Task<User> VerifiedCodeUser(int UserId , int code )
        {
            return await _repositoryManager.User.VerifiedCodeUser(UserId, code, false);
        }
        public async Task<List<User>> GetCustomers()
        {
            return await _repositoryManager.User.GetCustomers(false);
        }
        public async Task EditEmail(int userId, string email)
        {
            var user = await _repositoryManager.User.GetCustomerId(userId, true);
            user.Email = email;
            await _repositoryManager.SaveAsync();
        }
        public async Task<BussnessResultModel> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (model.Password == model.ConfirmPassword)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (!result.Succeeded)
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"), false);
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("passwnotequal"), false);
            }
            return new BussnessResultModel(user);
        }
        public async Task<BussnessResultModel> AddVerifyUser(int userId, int code)
        {
            var isVerify = await _repositoryManager.User.VerifiedCodeUser(userId, code, true);
            if (isVerify != null)
            {
                isVerify.IsMobileVerified = true;
                var user = new User { UserName = isVerify.Email, Email = isVerify.Email };
                var dec = _util.decr(isVerify.PasswordHash);
                var result = await _signInManager.PasswordSignInAsync(isVerify.Email, dec, true, false);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(isVerify);
            }
            else
            {
                return new BussnessResultModel(null , _locService.GetLocalizedStringValue("errorCode"), false);
            }
        }
        public async Task EditeUserSubscribe(bool newsletter, int userId)
        {
            var user = await _repositoryManager.User.GetCustomerId(userId, true);
            user.IsSubscribe = newsletter;
            await _repositoryManager.SaveAsync();
        }

        public async Task<BussnessResultModel> ChangePassword (int UserId, string OldPassword, string NewPassword)
        {
            var user = await _repositoryManager.User.GetUserId(UserId, true);
            var result = await _userManager.ChangePasswordAsync(user , OldPassword, NewPassword);
            if (!result.Succeeded)
            {
                return new BussnessResultModel(null, " fail", false);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(result);
        }
        public async Task UpdateAdmin(UpdateAdminDto updateDto)
        {
            var user = await _repositoryManager.User.GetUserId(updateDto.Id, true);
            if (user != null)
            {
                user.UserName = updateDto.FirstName + updateDto.LastName;
                var devices = await _repositoryManager.Device.GetDevicesUserId(updateDto.Id, true);
                if (user.PasswordHash != updateDto.Password && devices != null)
                {
                    foreach (var device in devices)
                    {
                        user.PasswordHash = updateDto.Password;
                        device.DeviceToken = user.PasswordHash;
                    }
                }
                _mapper.Map(updateDto, user);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task<BussnessResultModel> UpdateCustomer(UpdateCustomerDto updateDto)
        {
            var user = await _repositoryManager.User.GetUserId(updateDto.Id, true);
            if (user != null)
            {
                
                var devices = await _repositoryManager.Device.GetDevicesUserId(updateDto.Id, true);
                if (user.PasswordHash != updateDto.Password && devices != null)
                {
                    foreach (var device in devices)
                    {
                        user.PasswordHash = updateDto.Password;
                        device.DeviceToken = user.PasswordHash;
                    }
                }
                _mapper.Map(updateDto, user);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successSave"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
        }
        public async Task<BussnessResultModel> RemoveUserData(int id)
        {
            var user = await _repositoryManager.User.GetUserId(id, false);
            if(user != null)
            {
                var addresses = await _repositoryManager.Address.GetAllAddressesByCustomerId(id);
                if (addresses != null)
                {
                    foreach (var address in addresses)
                    {
                        _repositoryManager.Address.DeleteAddress(address);
                    }
                }
                var orders = await _repositoryManager.Order.GetOrdersToCustomer(id);
                if (orders != null)
                {
                    foreach (var order in orders)
                    {
                        _repositoryManager.Order.DeleteOrder(order);
                    }
                }
                var devices = await _repositoryManager.Device.GetDevicesUserId(id, false);
                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        _repositoryManager.Device.DeleteDevice(device);
                    }
                }
                var notifications = await _repositoryManager.Notification.GetNotificationsToUserId(id, false);
                if (notifications != null)
                {
                    foreach (var notification in notifications)
                    {
                        _repositoryManager.Notification.DeleteNotification(notification);
                    }
                }
                _repositoryManager.User.DeleteUser(user);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successDelete") );
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        }
        public async Task<BussnessResultModel> RegisterCustomer(CreateCustomerDto userRegister)
        {
            var user = _mapper.Map<User>(userRegister);
            if (userRegister.PhoneNumber.Length >= 8 && userRegister.PhoneNumber.Length < 11)
            {
                //if (userRegister.TypeRegister == TypeRegister.Facebook)
                //{
                //    user.TypeRegister = TypeRegister.Facebook;
                //    user.Avater = userRegister.SocialImage;
                //}
                //else if (user.TypeRegister == TypeRegister.Google)
                //{
                //    user.TypeRegister = TypeRegister.Google;
                //    user.Avater = userRegister.SocialImage;
                //}
                //else if (user.TypeRegister == TypeRegister.Apple)
                //{
                //    user.TypeRegister = TypeRegister.Apple;
                //    user.Avater = userRegister.SocialImage;
                //}
                //else
                //{
                //    user.TypeRegister = TypeRegister.Normal;
                //}
                user.TypeRegister = TypeRegister.Normal;
                var code =  "en";
                user.Lang = code;
                var country = await _repositoryManager.Country.GetcountryById(userRegister.CountryId.Value, false);
                user.RoleId = 2;
                user.UserName = userRegister.Email;
               
                user.VerifiedCode = Convert.ToInt32(_util.GenerateRandomNo()) + Convert.ToInt32(_util.GenerateRandomNo2());
                user.CodeMobileCountry = country.MobileCode == 0 ? 0 : country.MobileCode;
                var result = await _userManager.CreateAsync(user, userRegister.Password);
                if (result.Succeeded)
                {
                    var role = await _repositoryManager.Role.GetRoleId(user.RoleId, false);
                    await _userManager.AddClaimAsync(user, new Claim(role.Name, user.FullName));
                    var validat = new UserForAuthenticationDto
                    {
                        UserName = userRegister.Email,
                        Password = userRegister.Password
                    };
                    if (!await _authManager.ValidateUser(validat))
                    {
                        _logger.LogWarn($" Authentication failed. Wrong user name or password.");
                    }
                    var token = await _authManager.CreateToken();
                    var device = new Device
                    {
                        DeviceType = "Web",
                        UserId = user.Id,
                        DeviceModel = "Web",
                        OperatingSystem = "Windows",
                        DeviceToken = token,
                        IsStatus = Status.Active,
                    };
                    _repositoryManager.Device.AddDevice(device);
                   

                    var temp = await _repositoryManager.MessageTemplate.GetTemplateById(2 , false); 
                    var msgem = "Hello " + userRegister.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + user.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";

                    var message = new Message(new string[] { user.Email }, temp.Subject, msgem);
                    _emailSender.SendEmail(message);

                    await _repositoryManager.SaveAsync();
                    return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successAdd"));
                }
                else
                {
                    string errors = "";
                    foreach (var x in result.Errors)
                        errors += x + ", ";
                    if (user.Lang == "en")
                    {
                        return new BussnessResultModel(null, errors, false);
                    }
                    else
                    {
                        return new BussnessResultModel(null, "e: حدث خطأ يرجى التأكد من البيانات", false);
                    }
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("MobileVal"), false);
            }
        } 
        public async Task<BussnessResultModel> EditAdmin(UpdateAdminDto update )
        {
            var user = await _repositoryManager.User.GetUserId(update.Id, true);
            var change = await _userManager.ChangePasswordAsync(user, update.OldPassword, update.Password);
            if (!change.Succeeded)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ConfirmPassAtLeast"), false);
            }
           
            var role = await _repositoryManager.Role.GetRoleId(update.RoleId, false);
            await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Name, user.Email));
            await _userManager.AddClaimAsync(user, new Claim(role.Name, update.Email));
            _mapper.Map(update, user);
            await _repositoryManager.SaveAsync();

            return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successSave"));
        } 
        public async Task<BussnessResultModel> RegisterUser(CreateAdminDto userRegister ,int zoneId , string street , string zip )
        {
            var user = _mapper.Map<User>(userRegister);
            user.PhoneNumber = userRegister.PhoneNumber;
            user.UserName = userRegister.Email;
            user.TypeRegister = TypeRegister.Normal;
            user.PasswordHash = userRegister.Password;
            user.VerifiedCode = Convert.ToInt32( _util.GenerateRandomNo()) + Convert.ToInt32(_util.GenerateRandomNo2());
            user.IsMobileVerified = false;
            
            var result = await _userManager.CreateAsync(user, userRegister.Password);
            if (result.Succeeded)
            {
                var existAddress = await _repositoryManager.Address.GetAddressCustomer(user.Id);
                if (existAddress == null)
                {
                    var address = new Address();
                    address.UserId = user.Id;
                    address.CountryId = user.CountryId;
                    address.Street = street;
                    address.ZoneId = zoneId;
                    address.Post_Code = zip;
                    _repositoryManager.Address.AddAddress(address);
                }
                var role = await _repositoryManager.Role.GetRoleId(user.RoleId, false);
                await _userManager.AddClaimAsync(user, new Claim(/*ClaimTypes.Name*/role.Name, user.Email));
                var temp = await _repositoryManager.MessageTemplate.GetTemplateById(2, false); 
                var msgem = "Hello " + userRegister.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + user.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";

                //var message = new Message(new string[] { user.Email }, temp.Subject, msgem);
                //_emailSender.SendEmail(message);

                string strUrl = "https://connectsms.vodafone.com.qa/SMSConnect/SendServlet?application=http_gw1157&password=bdeyc5h3"
                + "&content=your code is " + user.VerifiedCode + "&destination=" + user.PhoneNumber + "&source=97433&mask=ETayf";

                WebRequest request = HttpWebRequest.Create(strUrl);
                // Get the response back  
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream s = (Stream)response.GetResponseStream();
                StreamReader readStream = new StreamReader(s);
                string dataString = readStream.ReadToEnd();
                response.Close();
                s.Close();
                readStream.Close();

                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(user, "successAdd");
            }
            else
            {
                return new BussnessResultModel(null, " Error Occurs", false);
            }
        }
        public async Task<BussnessResultModel> DeactiveCustomer( int id)
        {
            var customer = await _repositoryManager.User.GetCustomerId(id, true);
            if (customer != null)
            {
                customer.Status = Status.NotActive;
                await _userManager.AddClaimAsync(customer, new Claim("DeActivated", "true"));
                
                var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.DeactiveAccount);
                Notification notification = new()
                {
                    Body = action.Template,
                    UserId = id,
                    NotificationActionId = action.Id,
                    Status = NotificationStatus.New,
                    Subject = action.Subject,
                    IsRead = false
                };
                _repositoryManager.Notification.CreateNotification(notification);
                await _repositoryManager.SaveAsync();

                var devices = await _repositoryManager.Device.GetDevicesUserId(id, false);
                foreach(var device in devices)
                {
                    _repositoryManager.Device.DeleteDevice(device);
                    await _repositoryManager.SaveAsync();
                }

                return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successDeactive"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        } 
        //public async Task<BussnessResultModel> ActiveCustomer( int id)
        //{
        //    var customer = await _repositoryManager.User.GetCustomerId(id, true);
        //    if (customer != null)
        //    {
        //        customer.Status = Status.Active;
        //        var claim = _repositoryManager.Claim.FirstOrDefault(c => c.UserId == id && c.ClaimType == "DeActivated");
        //        if (claim != null)
        //        {
        //            _repositoryManager.Claim.DeleteClaim(claim); 
        //        }
        //        await _repositoryManager.SaveAsync();
        //        return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successDeactive"));
        //    }
        //    else
        //    {
        //        return new BussnessResultModel(null, "correctLink", false);
        //    }
        //}
        public async Task<User> FacebookUser(string socialId)
        {
            return await _repositoryManager.User.GetFacebookRegisterUser(socialId);
        }
        public async Task<string> ValidateUser(UserForAuthenticationDto user)
        {
            if (!await _authManager.ValidateUser(user))
            {
                try
                {
                    _logger.LogInfo(string.Format("{0} provided invalid password to generate token", user.UserName));
                }
                catch (Exception) { }
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            try
            {
                _logger.LogInfo(string.Format("{0} successfully generate token", user.UserName));
            }
            catch (Exception) { }
            return await _authManager.CreateToken();
        }
        public async Task<User> GoogleUser(string socialId)
        {
            return await _repositoryManager.User.GetGoogleRegisterUser(socialId);
        } 
        public async Task<User> GetAdminAndStoreEmail(string email)
        {
            return await _repositoryManager.User.GetAdminAndStoreEmail(email);
        } 
        public bool GetExsitEmail(string email)
        {
            return  _repositoryManager.User.GetCustomerEmail(email);
        }
        public async Task<User> AppleUser(string socialId)
        {
            return await _repositoryManager.User.GetAppleRegisterUser(socialId);
        }
        public async Task<List<UserTotal>> GetCustomerTotal(string search)
        {
            var customers = await _repositoryManager.User.GetCustomerTotal(search, false);
            if(customers == null)
            {
                return null;
            }
            var customerTotal = new List<UserTotal>();
            foreach (var x in customers)
            {
                x.CustomerOrders = await _repositoryManager.Order.GetsAllTransactionOrders();
                var order = x.CustomerOrders.Where(c=>c.CustomerId == x.Id);

                customerTotal.Add(new UserTotal
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss tt"),
                    Total = Convert.ToInt32(order.Sum(c => c.OrderPrice)),
                });
            }
            var descTotal = customerTotal.OrderByDescending(c => c.Total).ToList();
            return descTotal;
        }
        public async Task<List<UserTotal>> GetVendorTotal(string search)
        {
            var stores = await _repositoryManager.User.GetVendorTotal(search, false);
            if (stores == null)
            {
                return null;
            }
            var storesTotal = new List<UserTotal>();
            foreach (var x in stores)
            {
                x.StoreOrders = await _repositoryManager.Order.GetsAllTransactionOrders();
                var order = x.StoreOrders.Where(c => c.StoreId == x.Id);

                storesTotal.Add(new UserTotal
                {
                    FirstName = x.FirstName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Total = Convert.ToInt32(order.Sum(c => c.OrderPrice)),
                });
            }
            var descTotal = storesTotal.OrderByDescending(c => c.Total).ToList();
            return descTotal;
        }

        //Device------------------------------------------------
        public async Task AddDevice(CreateDeviceDto createDto)
        {
            var user = await _repositoryManager.User.GetUserId(createDto.UserId, true);
            var device = _mapper.Map<Device>(createDto);
             //device.UserId = GetCurrentUserId() ;
            device.IsStatus = Status.Active;
             device.DeviceToken= user.PasswordHash;
            _repositoryManager.Device.AddDevice(device);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateDevice(string deviceId , UpdateDeviceDto updateDeviceDto)
        {
            var user = await _repositoryManager.User.GetUserId(updateDeviceDto.UserId, true);
            var device = await _repositoryManager.Device.GetDeviceUser(Convert.ToInt32(deviceId),updateDeviceDto.UserId, true);
            if (device == null )
            {
                try {await AddDevice(updateDeviceDto);} catch { }
            }
            else
            {
                _mapper.Map(updateDeviceDto, device);
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task UpdateFcm( int deviceId,int userId, string fcmToken)
        {
            var device = await _repositoryManager.Device.GetDeviceUser(deviceId,userId,  true); 
            if (device != null)
            {
                device.FcmToken = fcmToken;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task ResendToken(int userId)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true); 
            string token = _repositoryManager.Device.GetTokenUser(userId);
            if (token != null && token == user.PasswordHash)
            {
                var code = Convert.ToInt32(_util.GenerateRandomNo());
                user.VerifiedCode = code;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task LogOutDevice(int userId)
        {
            var devicesUser = await _repositoryManager.Device.GetDevicesUserId(userId, false);
            if (devicesUser != null)
            {
                foreach (var device in devicesUser)
                {
                    _repositoryManager.Device.DeleteDevice(device);
                    await _repositoryManager.SaveAsync();
                }
            }
        }
        public async Task<Device> CheckDeviceToken(string token)
        {
            return await _repositoryManager.Device.CheckDevice(token, false);
        }
        public async Task<List<Device>> GetDevices(int userId)
        {
            return await _repositoryManager.Device.GetDevicesUserId(userId, false);
        }
        //Link------------------------------------------------
        public async Task<IEnumerable<Link>> GetLinks()
        {
            var links = await _repositoryManager.Link.GetLinks();
            return links;
        } 
        public async Task<IEnumerable<Permission>> GetPermissionsRoleId(int roleId)
        {
            return await _repositoryManager.Permission.GetPermissionsShowRole(roleId);
        }
    }
}
