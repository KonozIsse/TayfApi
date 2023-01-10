using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using AutoMapper;
using Contracts;
using Entities;
using System.Net;
using System.Web.Http;
using System.Net.Mail;
using Entities.Models.Enum;
using Entities.Exception;
using BusinessLogic.Services;
using System.Web.Helpers;

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
        protected readonly ISMSService _sms;
        public UserBL(IRepositoryManager repositoryManager, IMapper mapper , Util util, UserManager<User> userManager,  IEmailSender emailSender
            , LocService locService , RoleManager<Role> roleManager , LocationTaxBL locationTaxBL , SignInManager<User> signInManager, IAuthenticationManager authManager , ILoggerManager logger ,ISMSService sms)
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
            _sms = sms;
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
            if (permissions != null)
            {
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
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"),false);
            }
           
        }
        public async Task<BussnessResultModel> AddRole(CreateRoleDto create)
        {
            var IsExists = _repositoryManager.Role.IsExistRole(create.Name);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
            }
            if (String.IsNullOrEmpty(create.Name) || create.Name.Contains(" "))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterallfiled"), false);
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
                if (String.IsNullOrEmpty(create.Name) || create.Name.Contains(" "))
                {
                    return new BussnessResultModel(role, _locService.GetLocalizedStringValue("enterallfiled"), false);
                }
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
            store.LastName = "Store";
            store.RoleId = 3;
            store.PhoneNumber = create.PhoneNumber;
            store.UserName = create.Email;
            store.VerifiedCode = 1234;
            store.IsMobileVerified = false;
            store.Status = Status.Active;
            store.TypeRegister = TypeRegister.Normal;
            var result = await _userManager.CreateAsync(store, create.Password);
            if (create.Avater == null)
            {
                return new BussnessResultModel(store, _locService.GetLocalizedStringValue("correctImage"),false);
            }

            MailAddress addr = new MailAddress(create.Email);
            if (create.Email != addr.ToString())
            {
                 return new BussnessResultModel(store, _locService.GetLocalizedStringValue("EnterValidEmailAddress"), false);
            }
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
        public async Task<BussnessResultModel> EditSubscribeletter(string newsletter, int CustomerId)
        {
            var customer = await _repositoryManager.User.GetCustomerId(CustomerId,true);
            if(customer == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"),false);
            }
            customer.IsSubscribe = newsletter == "0" ? false : true;
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<User> GetUserById(int id)
        {
            return await _repositoryManager.User.GetActiveUserId(id, false);
        }
        public async Task<IEnumerable<User>> GetAdminsStors()
        {
            return await _repositoryManager.User.GetAdminsStors(false);
        }
        public async Task<List<User>> GetCustomers()
        {
            return await _repositoryManager.User.GetCustomers(false);
        }
        public async Task<List<UserTotal>> GetCustomerTotal(string search)
        {
            var customers = await _repositoryManager.User.GetCustomerTotal(search, false);
            
            var customerTotal = new List<UserTotal>();
            foreach (var x in customers)
            {
                x.CustomerOrders = await _repositoryManager.Order.GetsAllTransactionOrders();
                var order = x.CustomerOrders.Where(c => c.CustomerId == x.Id);

                customerTotal.Add(new UserTotal
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    CreatedAt = x.CreatedAt,
                    Total = Convert.ToInt32(order.Sum(c => c.OrderPrice)),
                });
            }
            var descTotal = customerTotal.OrderByDescending(c => c.Total).ToList();
            return descTotal;
        }
        public async Task<List<UserTotal>> GetVendorTotal(string search)
        {
            var stores = await _repositoryManager.User.GetVendorTotal(search, false);
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
                var result = await _signInManager.PasswordSignInAsync(isVerify.Email, isVerify.PasswordHash, true, false);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(isVerify);
            }
            else
            {
                return new BussnessResultModel(null , _locService.GetLocalizedStringValue("errorCode"), false);
            }
        }
        public async Task<User> ReSendCode(string email)
        {
            var customer = await _repositoryManager.User.GetCustomerEmail(email,false);
            if (customer != null)
            {
                var temp = await _repositoryManager.MessageTemplate.GetTemplateById(2, false);
                var msgem = "Hello " + customer.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + customer.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";
                try
                {
                    var message = new Message(new string[] { email }, temp.Subject, msgem);
                    _emailSender.SendEmail(message);
                }
                catch (Exception exp)
                {
                    _logger.LogError("", exp);
                }
                try
                {
                   await _sms.SendSMS(customer.PhoneNumber,Convert.ToInt32(customer.VerifiedCode),customer.CodeMobileCountry);
                }
                catch (Exception exp)
                {
                    _logger.LogError("",exp);
                }

            }
            return customer;
        }

        public async Task<BussnessResultModel> ChangePassword (int UserId, string OldPassword, string NewPassword)
        {
            var user = await _repositoryManager.User.GetUserId(UserId, true);
            var result = await _userManager.ChangePasswordAsync(user , OldPassword, NewPassword);
            if (!result.Succeeded)
            {
                return new BussnessResultModel(null, " ", false);
            }
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(result);
        }
        public async Task<BussnessResultModel> ChangePasswordCustomer (int UserId, string OldPassword, string NewPassword, string ConfirmedPassword)
        {
            var user = await _repositoryManager.User.GetUserId(UserId, true);
            if (!String.IsNullOrEmpty(ConfirmedPassword) && !String.IsNullOrEmpty(NewPassword))
            {
                if (ConfirmedPassword == NewPassword)
                {
                    if (OldPassword != user.PasswordHash)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("passwnotequal"), false);
                    }

                    var result2 = await _userManager.ChangePasswordAsync(user, OldPassword, NewPassword);
                    if (result2.Succeeded)
                    {
                        user.PasswordHash = NewPassword;
                        await _repositoryManager.SaveAsync();
                        return new BussnessResultModel(user, _locService.GetLocalizedStringValue("PasswordChangedSuccessfully"));
                    }
                    else
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ConfirmPassAtLeast"), false);
                    }
                }
                else
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("passwnotequal"), false);
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterPassword"), false);
            }
          
        }
        public async Task<BussnessResultModel> ResetPasswordCustomer(string email, string NewPassword, string ConfirmedPassword)
        {
            var user = await _repositoryManager.User.GetCustomerEmail(email, true);
            if (ConfirmedPassword == NewPassword)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, NewPassword);
                if (result.Succeeded)
                {
                    user.ResetPasswordCode = null;
                    user.PasswordHash = NewPassword;
                    var devices = await _repositoryManager.Device.GetDevicesUserId(user.Id, true);
                    if (devices != null)
                    {
                        foreach (var device in devices)
                        {
                            device.DeviceToken = user.PasswordHash;
                        }
                    }
                    await _repositoryManager.SaveAsync();
                    return new BussnessResultModel(user);
                }
                else
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"), false);
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("passwnotequal"), false);
            }
        }

        public async Task<BussnessResultModel> UpdateCustomerCP(UpdateCustomerDto updateDto)
        {
            var user = await _repositoryManager.User.GetUserId(updateDto.Id, true);
            if (user != null)
            {
                var devices = await _repositoryManager.Device.GetDevicesUserId(updateDto.Id, true);
                if (user.PasswordHash != updateDto.Password && devices != null)
                {
                    foreach (var device in devices)
                    {
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
        public async Task<BussnessResultModel> ForgotPassword(string email)
        {
            var user = await _repositoryManager.User.GetCustomerEmail(email, true);
            if (user == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"), false);
            }
            if (user.TypeRegister != TypeRegister.Normal)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("SocialAccountCanNotResetPassword"), false);
            }
            var devices = await _repositoryManager.Device.GetDevicesUserId(user.Id, true);
            if (devices != null)
            {
                foreach (var device in devices)
                {
                    device.DeviceToken = user.PasswordHash;
                }
            }
            user.ResetPasswordCode = Convert.ToInt32(_util.GenerateRandomNo()) + Convert.ToInt32(_util.GenerateRandomNo2());
            var content = "This code" + user.ResetPasswordCode;
            var message = new Message(new string[] {email }, "Forgot Password Confirmation", content);
            _emailSender.SendEmail(message);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(user);
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
        public async Task<BussnessResultModel> RegisterCustomer(CreateCustomerDto userRegister,string lang = "en")
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
            var user = _mapper.Map<User>(userRegister);
            if (userRegister.Agree == 1)
            {
                if (userRegister.Password == userRegister.ConfirmPassword)
                {
                    if (userRegister.PhoneNumber.Length > 14)
                    {
                        if (lang == "en")
                        {
                            return new BussnessResultModel(null, "Please Verify Mobile Number , Mobile must be at most 14 numbers", false);
                        }
                        else
                        {
                            return new BussnessResultModel(null, "e: يرجى التحقق من رقم الهاتف المحمول ، يجب أن يكون رقم الجوال 14 رقمًا كحد أقصى", false);
                        }
                    }

                    user.PhoneNumber = userRegister.PhoneNumber.StartsWith("0") ? userRegister.PhoneNumber.Substring(1) : userRegister.PhoneNumber;
                    user.TypeRegister = TypeRegister.Normal;
                    user.IsSubscribe = userRegister.IsSubscribe == "0" ? false : true;
                    user.Lang = lang;
                    user.IsMobileVerified = false;
                    user.RoleId = 2;
                    user.UserName = userRegister.FirstName + userRegister.LastName;
                    //var regex = new Regex("1P([A-Z0-9]{4})");
                    user.VerifiedCode = Convert.ToInt32(_util.GenerateRandomNo()) + Convert.ToInt32(_util.GenerateRandomNo2());
                    var country = await _repositoryManager.Country.GetcountryById(userRegister.CountryId.Value, false);
                    user.CodeMobileCountry = country.MobileCode == null ? null : country.MobileCode;
                    MailAddress addr = new MailAddress(userRegister.Email);
                    if (userRegister.Email != addr.ToString())
                    {
                        if (lang == "en")
                        {
                            return new BussnessResultModel(null, "Please Verify Your Email ", false);
                        }
                        else
                        {
                            return new BussnessResultModel(null, " يرجى التحقق من البريد الالكتروني", false);
                        }
                    }
                    var result = await _userManager.CreateAsync(user, userRegister.Password);
                    if (result.Succeeded)
                    {
                        var role = await _repositoryManager.Role.GetRoleId(user.RoleId, false);
                        await _userManager.AddClaimAsync(user, new Claim(role.Name, user.FirstName));
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


                        var temp = await _repositoryManager.MessageTemplate.GetTemplateById(2, false);
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
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("passwnotequal"), false);
                }
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("plzAgree"), false);
            }
        } 
        public async Task<BussnessResultModel> EditAdmin(UpdateAdminDto update )
        {
            var user = await _repositoryManager.User.GetUserId(update.Id, true);
            if(user == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            if (!string.IsNullOrEmpty(update.Password))
            {
                if (String.IsNullOrEmpty(update.OldPassword))
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("enterPassword"), false);
                }
                if (!update.OldPassword.Equals(user.PasswordHash))
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("errorOldPassword"), false);
                }
                var change = await _userManager.ChangePasswordAsync(user, update.OldPassword, update.Password);
                if (!change.Succeeded)
                {
                    return new BussnessResultModel(change, _locService.GetLocalizedStringValue("ConfirmPassAtLeast"), false);
                }
            }
            _mapper.Map(update, user);
            await _repositoryManager.SaveAsync();

            return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successSave"));
        } 
        public async Task<BussnessResultModel> EditCustomer(UpdateCustomerDto update )
        {
            var user = await _repositoryManager.User.GetActiveCustomerId(update.Id, true);
            if(user == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            
            if (!String.IsNullOrEmpty(update.ConfirmedPassword) && !String.IsNullOrEmpty(update.NewPassword))
            {
                if (update.ConfirmedPassword == update.NewPassword)
                {
                    if (update.Password != user.PasswordHash)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("InvalidLogin"), false);
                    }
                   
                    var result2 = await _userManager.ChangePasswordAsync(user, update.Password, update.NewPassword);
                    if (result2.Succeeded)
                    {
                        return new BussnessResultModel(user, _locService.GetLocalizedStringValue("PasswordChangedSuccessfully"));
                    }
                    else
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("InvalidLogin"), false);
                    }
                }
                else
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("passwnotequal"), false);
                }
            }
            if (!String.IsNullOrEmpty(update.Email) && user.Email != update.Email)
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                     user.Email = update.Email;
                }
                else
                {
                    return new BussnessResultModel(null,_locService.GetLocalizedStringValue("EmailExist"),false);
                }

            }
            _mapper.Map(update, user);
            await _repositoryManager.SaveAsync();

            return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successSave"));
        } 
        public async Task<BussnessResultModel> RegisterUser(CreateAdminDto userRegister ,int zoneId , string street , string zip  , string lang  = "en")
        {
            var IsExists = _repositoryManager.User.GetEmail(userRegister.Email);
            if (IsExists)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("uniqUser"), false);
            }
            MailAddress addr = new MailAddress(userRegister.Email);
            if (userRegister.Email != addr.ToString())
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("EnterValidEmailAddress"), false); 
            }
            else
            {
                var user = _mapper.Map<User>(userRegister);
                user.PhoneNumber = userRegister.PhoneNumber;
                user.UserName = userRegister.Email;
                user.Lang = lang;
                user.IsMobileVerified = false;
                user.TypeRegister = TypeRegister.Normal;
                user.PasswordHash = userRegister.Password;
                user.VerifiedCode = Convert.ToInt32(_util.GenerateRandomNo()) + Convert.ToInt32(_util.GenerateRandomNo2());
                var country = await _repositoryManager.Country.GetcountryById(userRegister.CountryId.Value, false);
                user.CodeMobileCountry = country.MobileCode == null ? null : country.MobileCode;

                var result = await _userManager.CreateAsync(user, userRegister.Password);
                if (result.Succeeded)
                {
                    var existAddress = await _repositoryManager.Address.GetDefaultAddressCustomer(user.Id);
                    if (existAddress == null)
                    {
                        var address = new Address();
                        address.UserId = user.Id;
                        address.CountryId = user.CountryId.Value;
                        address.Street = street;
                        address.ZoneId = zoneId;
                        address.Post_Code = zip;
                        _repositoryManager.Address.AddAddress(address);
                    }

                    var role = await _repositoryManager.Role.GetRoleId(user.RoleId, false);
                    await _userManager.AddClaimAsync(user, new Claim(/*ClaimTypes.Name*/role.Name, user.Email));

                    var temp = await _repositoryManager.MessageTemplate.GetTemplateById(2, false); //verify emasil
                    var msgem = "Hello " + userRegister.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + user.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";

                    var message = new Message(new string[] { user.Email }, temp.Subject, msgem);
                    _emailSender.SendEmail(message);

                    await _repositoryManager.SaveAsync();
                    return new BussnessResultModel(user, "successAdd");
                }
                else
                {
                    if (lang == "en")
                    {
                        return new BussnessResultModel(null, " Error Occurs", false);
                    }
                    else
                    {
                        return new BussnessResultModel(null, "e: حدث حطأ يرجى التأكد من بيانات الدخول", false); 
                    }
                }
            } 
            
        }
        public async Task<BussnessResultModel> DeactiveCustomer(int id)
        {
            var customer = await _repositoryManager.User.GetCustomerId(id, true);
            if (customer == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
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

            var message = new Message(new string[] { customer.Email }, action.Subject, action.Template);
            _emailSender.SendEmail(message);

            await _repositoryManager.SaveAsync();

            var devices = await _repositoryManager.Device.GetDevicesUserId(id, false);
            foreach (var device in devices)
            {
                _repositoryManager.Device.DeleteDevice(device);
                await _repositoryManager.SaveAsync();
            }
            return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successDeactive"));
        }
        public async Task<BussnessResultModel> ActiveCustomer(int id)
        {
            var customer = await _repositoryManager.User.GetCustomerId(id, true);
            if (customer != null)
            {
                customer.Status = Status.Active;
                await _userManager.RemoveClaimAsync(customer, new Claim("DeActivated", "true"));
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successDeactive"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
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
       
        public async Task<User> GetAdminAndStoreEmail(string email)
        {
            return await _repositoryManager.User.GetAdminAndStoreEmail(email);
        } 

        //Device------------------------------------------------
        public async Task AddDevice(CreateDeviceDto createDto)
        {
            var user = await _repositoryManager.User.GetUserId(createDto.UserId, true);
            var device = _mapper.Map<Device>(createDto);
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
        public string GetTokenDevice(int userId)
        {
            return  _repositoryManager.Device.GetTokenUser(userId);
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
