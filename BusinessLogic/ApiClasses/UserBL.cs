using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Contracts;
using Entities;
using Entities.Exception;
using BusinessLogic.Services;
using Entities.RequestFeatures;
using System.Data;
using System.Text.RegularExpressions;

namespace BusinessLogic.ApiClasses
{
    public class UserBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper; 
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly LocService _locService;
        protected readonly IEmailSender _emailSender;
        protected readonly RoleManager<Role> _roleManager;  
        protected readonly LocationTaxBL _locationTaxBL;
        protected readonly IAuthenticationManager _authManager;
        protected readonly ILoggerManager _logger;
        protected readonly ISMSService _sms;
        protected readonly ImageBL _imageBL;
        protected readonly ImageUploadServices _imageUploadServices;
        public UserBL(IRepositoryManager repositoryManager, IMapper mapper ,UserManager<User> userManager,  IEmailSender emailSender
            , LocService locService , RoleManager<Role> roleManager , LocationTaxBL locationTaxBL , SignInManager<User> signInManager, IAuthenticationManager authManager 
            , ILoggerManager logger ,ISMSService sms, ImageUploadServices imageUploadServices , ImageBL imageBL)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _userManager = userManager;
            _emailSender = emailSender;
            _locService = locService;
            _roleManager = roleManager;
            _locationTaxBL = locationTaxBL;
            _signInManager = signInManager;
            _authManager = authManager;
            _logger = logger;
            _sms = sms;
            _imageUploadServices = imageUploadServices;
            _imageBL = imageBL;
        }
        //Role------------------------------------------------
        public async Task<List<RoleDto>> GetTypesStoreAdmin()
        {
            var roles = await _repositoryManager.Role.GetRolesAdminStore();
            var rolesDto = _mapper.Map<List<RoleDto>>(roles);
            return rolesDto;
        }
        public async Task<List<LinkDto>> GetLinks()
        {
            var links = await _repositoryManager.Link.GetLinks();
            var linksDto = links.Select(link =>
            {
              var linkDto = _mapper.Map<LinkDto>(link);
                linkDto.Show = link.IsStatus == Status.Active ? true : false;
                return linkDto;
            }).ToList();
            return linksDto;
        }
        public async Task<List<LinkDto>> GetLinksRole(int roleId)
        {
            var links = await _repositoryManager.Link.GetLinks();
            links = links.Where(c=>c.Permissions.Any(x=>x.RoleId == roleId)).ToList();   
            var linksDto = links.Select(link =>
            {
              var linkDto = _mapper.Map<LinkDto>(link);
                linkDto.Show = link.IsStatus == Status.Active ? true : false;
                return linkDto;
            }).ToList();
            return linksDto;
        }
        public async Task<List<Permission>> GetPermissionsRole(int roleId)
        {
            var permissions = await _repositoryManager.Permission.GetPermissionsRole(roleId, false);
            return (List<Permission>)permissions;
        }
        public async Task<BussnessResultModel> AddPermission(int roleId, List<RoleLinksDto> RoleLinksDto)
        {
            var permissions = await _repositoryManager.Permission.GetPermissionsRole(roleId, true);
            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    _repositoryManager.Permission.DeletePermission(permission);
                }

                foreach (var item in RoleLinksDto)
                {
                    _repositoryManager.Permission.AddPermission(new Permission { RoleId = roleId, LinkId = item.LinkId });
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
            var IsExists = await _repositoryManager.Role.IsExistRole(create.Name,false);
            if (IsExists != null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ExistItem"), false);
            }
            var role = _mapper.Map<Role>(create);
            role.NormalizedName = create.Name.ToUpper();
            await _roleManager.CreateAsync(role);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(role, _locService.GetLocalizedStringValue("successAdd"));
        } 
        public async Task<BussnessResultModel> EditRole(UpdateRoleDto create)
        {
            var role = await _repositoryManager.Role.GetRoleId(create.Id, true);
            if (role != null)
            {
                role.NormalizedName = create.Name.ToUpper();
                _mapper.Map(create, role);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(role, _locService.GetLocalizedStringValue("successSave"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
        }
        public async Task<UpdateRoleDto> GetMapRoleId(int id)
        {
            var role = await _repositoryManager.Role.GetRoleId(id, false);
            var roleDto = _mapper.Map<UpdateRoleDto>(role);
            return roleDto;
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
        public async Task<PagedList<StoreDto>> GetAllStores(string search, PostsParameters postsParameters)
        {
            var stores = await _repositoryManager.User.GetSearchStores(search);
            var storesDto = stores.Select(store =>
            {
                var storeDto = _mapper.Map<StoreDto>(store);
                storeDto.Image = _imageBL.GetTypeImage(Convert.ToInt32(store.ImageId), ImageType.MEDIUM);
                storeDto.CreatedAt = store.CreatedAt.ToString("G");
                return storeDto;
            }).ToList();
            return PagedList<StoreDto>.ToPagedList(storesDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<StoreDto>> GetStores()
        {
            var stores = await _repositoryManager.User.GetAllStores(false);
            var storesDto = stores.Select(store =>
            {
                var bookDto = _mapper.Map<StoreDto>(store);
                bookDto.Image = _imageBL.GetTypeImage(Convert.ToInt32(store.ImageId), ImageType.MEDIUM);
                bookDto.CreatedAt = store.CreatedAt.ToString("G");
                return bookDto;
            }).ToList();

            return storesDto;
        }
        public async Task<UpdateStoreDto> GetMapStore(int id)
        {
            var store = await _repositoryManager.User.GetStoreId(id);
           var storeDto = _mapper.Map<UpdateStoreDto>(store);
            return storeDto;
        }
        public async Task<PagedList<StoreDto>> GetAllActiveStores(PostsParameters postsParameters , int? sort)
        {
            var stores = await _repositoryManager.User.GetAllStores(false);
            if (sort == 1)
            {
                stores = stores.OrderBy(x => x.FirstName).ToList();
            }
            else if (sort == 2)
            {
                stores = stores.OrderByDescending(x => x.FirstName).ToList();
            }
            var storesDto = stores.Select(store =>
            {
                var storeDto = _mapper.Map<StoreDto>(store);
                storeDto.Status = store.Status == Status.Active ? _locService.GetLocalizedStringValue("active")
                  : _locService.GetLocalizedStringValue("notActive");
                storeDto.Image = _imageBL.GetTypeImage(Convert.ToInt32(store.ImageId), ImageType.MEDIUM);
                storeDto.CreatedAt = store.CreatedAt.ToString("G");
                return storeDto;
            }).ToList();
            return PagedList<StoreDto>.ToPagedList(storesDto, postsParameters.PageNumber, postsParameters.PageSize);
        } 
        public async Task<PagedList<StoreDto>> GetVendorTotal(int userId ,string search, PostsParameters postsParameters)
        {
            
            var stores = await _repositoryManager.User.GetVendorTotal(search, false);
            var orders = await _repositoryManager.Order.GetsAllTransactionOrders();
           
            var storesTotal = stores.Select(x =>
            {
                var storeDto = _mapper.Map<StoreDto>(x);
                var order = orders.Where(c => c.StoreId == x.Id);
                storeDto.TotalPrice = Convert.ToInt32(order.Sum(c => c.OrderPrice));
                return storeDto;
            }).OrderByDescending(c => c.TotalPrice).ToList();
            var store = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                storesTotal = storesTotal.Where(c => c.Id == userId).ToList();
            }
            return PagedList<StoreDto>.ToPagedList(storesTotal, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<BussnessResultModel> AddStore(CreateStoreDto create)
        {
            var store = _mapper.Map<User>(create);
            var name = await _userManager.FindByNameAsync(create.FirstName);
            if(name != null)
            {
                return new BussnessResultModel(store, "StoreAlreadyExists", false);
            }
            if (create.ImageId == 0)
            {
                return new BussnessResultModel(store, _locService.GetLocalizedStringValue("correctImage"), false);
            }
            string emailRules = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            if (!Regex.IsMatch(create.Email, emailRules))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("EnterValidEmailAddress"), false);
            }
            store.LastName = "Store";
            var role = await _repositoryManager.Role.IsExistRole("Store",false);
            store.RoleId = role.Id;
            store.PhoneNumber = create.PhoneNumber;
            store.UserName = create.Email;
            store.VerifiedCode = 1234;
            store.PhoneNumberConfirmed = true;
            store.Status = Status.Active;
            store.TypeRegister = TypeRegister.Normal;
            store.UserType = UserType.Store;
            var result = await _userManager.CreateAsync(store, create.Password);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    return new BussnessResultModel(null, error.Code + error.Description , false);
                    /* _locService.GetLocalizedStringValue("ErrorOccurs")*/
                }
            }
            return new BussnessResultModel(store, _locService.GetLocalizedStringValue("successAdd"));
        }
        public async Task<BussnessResultModel> UpdateStore(UpdateStoreDto update)
        {
            var store = await _repositoryManager.User.GetTypeUserId(update.Id, UserType.Store, true);
            if(store == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            _mapper.Map(update, store);
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(store, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<BussnessResultModel> DeleteStore(int id)
        {
            var store = await _repositoryManager.User.GetTypeUserId(id, UserType.Store, false);
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
        //Admin------------------------------------------------ 
        public async Task<List<AdminDto>> GetAdminsStores()
        {
            var admins = await _repositoryManager.User.GetAdminsStors(false);
            var adminsDto = admins.Select(admin => 
            {
               var adminDto = _mapper.Map<AdminDto>(admin);
                adminDto.RoleName = admin.Role.Name;
                adminDto.Status = admin.Status == Status.Active ? _locService.GetLocalizedStringValue("active") : _locService.GetLocalizedStringValue("notActive");
                return adminDto;
            }).ToList();
            return adminsDto;
        }
        public async Task<BussnessResultModel> RemoveUser(int id)
        {
            var user = await _repositoryManager.User.GetUserId(id, false);
            if (user != null)
            {
                _repositoryManager.User.DeleteUser(user);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successDelete"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
        }
        public async Task<BussnessResultModel> RegisterUser(CreateAdminDto userRegister, int zoneId, string street, string zip)
        {
            string emailRules = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            if (!Regex.IsMatch(userRegister.Email, emailRules))
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("EnterValidEmailAddress"), false);
            }
            else
            {
                var user = _mapper.Map<User>(userRegister);
                user.PhoneNumber = userRegister.PhoneNumber;
                user.UserName = userRegister.Email;
                user.PhoneNumberConfirmed = false;
                user.TypeRegister = TypeRegister.Normal;
                user.PasswordHash = userRegister.Password;
                user.VerifiedCode = new Random().Next(1000, 9999);
                var country = await _repositoryManager.Country.GetcountryById(Convert.ToInt32(userRegister.CountryId), false);
                user.CodeMobileCountry = country == null ? null : country.MobileCode;
                var role = await _repositoryManager.Role.GetActiveRole(userRegister.RoleId, false);
                user.RoleId = userRegister.RoleId;
                if (role.Name == "Store")
                {
                    user.UserType = UserType.Store;
                }
                else
                {
                    user.UserType = UserType.Admin;
                }
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

                    //var temp = await _repositoryManager.MessageTemplate.GetTemplateById(2, false); //verify emasil
                    //var msgem = "Hello " + userRegister.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + user.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";

                    //var message = new Message(new string[] { user.Email }, temp.Subject, msgem);
                    //_emailSender.SendEmail(message);

                    await _repositoryManager.SaveAsync();
                    return new BussnessResultModel(user, "successAdd");
                }
                else
                {
                    return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ErrorOccurs"), false);
                }
            }
        }
        public async Task<BussnessResultModel> EditAdmin(UpdateAdminDto update)
        {
            var user = await _repositoryManager.User.GetUserId(update.Id, true);
            if (user == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            var role = await _repositoryManager.Role.GetActiveRole(update.RoleId, false);
            if (role.Name == "Store")
            {
                user.UserType = UserType.Store;
            }
            else
            {
                user.UserType = UserType.Admin;
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
        public async Task<UpdateAdminDto> GetMapAdmin(int id)
        {
            var user = await _repositoryManager.User.GetUserId(id, true);
           var dto =  _mapper.Map<UpdateAdminDto>(user);
            return dto;
        }
        //Customer------------------------------------------------
        public async Task<BussnessResultModel> EditSubscribeletter(string newsletter, int CustomerId)
        {
            var customer = await _repositoryManager.User.GetTypeUserId(CustomerId, UserType.Customer, true);
            if(customer == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("Error"),false);
            }
            customer.IsSubscribe = newsletter == "0" ? false : true;
            await _repositoryManager.SaveAsync();
            return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successSave"));
        }
        public async Task<PagedList<CustomerDto>> GetCustomers(string search,string filter, PostsParameters postsParameters)
        {
            var customers = await _repositoryManager.User.GetAllCustomers(search, filter, false);
            var customersDto = _mapper.Map<List<CustomerDto>>(customers);
            return PagedList<CustomerDto>.ToPagedList(customersDto, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<PagedList<CustomerDto>> GetCustomerTotal(int userId,string search , PostsParameters postsParameters)
        {
            var customers = await _repositoryManager.User.GetAllCustomers(search,"", false);
            var orders = await _repositoryManager.Order.GetsAllTransactionOrders();
            var store = await _repositoryManager.User.GetActiveUserId(userId, false);
            if (store.UserType == UserType.Store)
            {
                orders = orders.Where(c => c.StoreId == userId).ToList();
            }
            var storesTotal = customers.Select(x =>
            {
                var storeDto = _mapper.Map<CustomerDto>(x);
                var order = orders.Where(c => c.CustomerId == x.Id);
                storeDto.Total = Convert.ToInt32(order.Sum(c => c.OrderPrice));
                return storeDto;
            }).OrderByDescending(c => c.Total).ToList();
            return PagedList<CustomerDto>.ToPagedList(storesTotal, postsParameters.PageNumber, postsParameters.PageSize);
        }
        public async Task<List<CustomerDto>> GetAllCustomer()
        {
            var customers = await _repositoryManager.User.GetCustomers(false);
            var customersDto = _mapper.Map<List<CustomerDto>>(customers);
            return customersDto;
        } 
        public async Task<UpdateCustomerDto> GetCustomerId(int id)
        {
            var customer = await _repositoryManager.User.GetTypeUserId(id, UserType.Customer, false);
            var customerDto = _mapper.Map<UpdateCustomerDto>(customer);
            return customerDto;
        }
        public async Task<BussnessResultModel> VerifyCustomer (int userId, int code)
        {
            var isVerify = await _repositoryManager.User.VerifiedCodeUser(userId, code, true);
            if (isVerify != null)
            {
                isVerify.PhoneNumberConfirmed = true;
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(isVerify, "PhoneNumberConfirmed");
            }
            else
            {
                return new BussnessResultModel(null , _locService.GetLocalizedStringValue("errorCode"), false);
            }
        }
        public async Task ResendCode(string email)
        {
            var customer = await _repositoryManager.User.GetCustomerEmail(email,false);
            if (customer != null)
            {
                var temp = await _repositoryManager.MessageTemplate.GetNameTemplate(NameTemplate.VerificationEmail);
                var msgem = "Hello " + customer.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + customer.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";
                var message = new Message(new string[] { email }, temp.Subject, msgem);
                _emailSender.SendEmail(message);
            }
        }
        public async Task<BussnessResultModel> UpdateCustomerCP(UpdateCustomerDto updateDto)
        {
            var user = await _repositoryManager.User.GetUserId(updateDto.Id, true);
            if (user != null)
            {
                _mapper.Map(updateDto, user);
                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successSave"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
        }
        public async Task<BussnessResultModel> DeleteCustomer(int id)
        {
            var user = await _repositoryManager.User.GetTypeUserId(id, UserType.Customer, false);
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
              
                var notifications = await _repositoryManager.Notification.GetNotificationsToUserId(id, false);
                if (notifications != null)
                {
                    foreach (var notify in notifications)
                    {
                        _repositoryManager.Notification.DeleteNotification(notify);
                    }
                }
                _repositoryManager.User.DeleteUser(user);

                var action = await _repositoryManager.NotificationAction.GetNotificationActionByKey(NotificationKey.DeleteAccount);
                action.Template = action.Template.Replace("{userName}", user.FullName);
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
            if (userRegister.TypeRegister == TypeRegister.Facebook)
            {
                user.TypeRegister = TypeRegister.Facebook;
                // user.Avater = userRegister.SocialImage;
            }
            else if (user.TypeRegister == TypeRegister.Google)
            {
                user.TypeRegister = TypeRegister.Google;
                //user.Avater = userRegister.SocialImage;
            }
            else if (user.TypeRegister == TypeRegister.Apple)
            {
                user.TypeRegister = TypeRegister.Apple;
                //user.Avater = userRegister.SocialImage;
            }
            else
            {
                user.TypeRegister = TypeRegister.Normal;
            }
            if (userRegister.Agree == 1)
            {
                if (userRegister.Password == userRegister.ConfirmPassword)
                {
                    if (userRegister.PhoneNumber.Length > 14)
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("VerifyMobile"), false);
                    }
                    user.UserType = UserType.Customer;
                    user.PhoneNumberConfirmed = false;
                    var role = await _repositoryManager.Role.IsExistRole("Customer", false);
                    user.RoleId = role.Id;
                    user.UserName = userRegister.Email;
                    user.VerifiedCode =  Convert.ToInt32(Guid.NewGuid().ToString().Substring(0, 4));
                                        //new Random().Next(1000, 9999);
                    var country = await _repositoryManager.Country.GetcountryById(Convert.ToInt32(userRegister.CountryId), false);
                    user.CodeMobileCountry = country == null ? null : country.MobileCode;

                    var result = await _userManager.CreateAsync(user, userRegister.Password);
                    if (result.Succeeded)
                    {
                        //var temp = await _repositoryManager.MessageTemplate.GetTemplateById(2, false);
                        //var msgem = "Hello " + userRegister.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + user.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";

                        //var message = new Message(new string[] { user.Email }, temp.Subject, msgem);
                        //_emailSender.SendEmail(message);

                        await _repositoryManager.SaveAsync();
                        return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successAdd"));
                    }
                    else
                    {
                        return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ErrorOccurs"), false);
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
        public async Task<BussnessResultModel> RegisterCustomerCP(CreateCustomerCPDto userRegister)
        {

            var user = _mapper.Map<User>(userRegister);
            user.UserType = UserType.Customer;
            user.PhoneNumberConfirmed = false;
            user.TypeRegister = TypeRegister.Normal;
            user.RoleId = 2;
            user.UserName = userRegister.Email;
           
            user.VerifiedCode = new Random().Next(1000, 9999);
            var country = await _repositoryManager.Country.GetcountryById(Convert.ToInt32(userRegister.CountryId), false);
            user.CodeMobileCountry = country == null ? null : country.MobileCode;
           
            var result = await _userManager.CreateAsync(user, userRegister.Password);
            if (result.Succeeded)
            {
                var temp = await _repositoryManager.MessageTemplate.GetNameTemplate(NameTemplate.VerificationEmail);
                var msgem = "Hello " + userRegister.FirstName + " ," + "<br>" + temp.Message + "<br> Here is your code: " + user.VerifiedCode + "<br> <br> The E-Tayf account team <br> Thank You";

                var message = new Message(new string[] { user.Email }, temp.Subject, msgem);
                _emailSender.SendEmail(message);

                await _repositoryManager.SaveAsync();
                return new BussnessResultModel(user, _locService.GetLocalizedStringValue("successAdd"));
            }
            else
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("ErrorOccurs"), false);
            }
        }
        public async Task<BussnessResultModel> EditCustomer(UpdateCustomerDto update )
        {
            var user = await _repositoryManager.User.GetActiveCustomerId(update.Id, true);
            if(user == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("sureLink"), false);
            }
            
            if (!string.IsNullOrEmpty(update.ConfirmedPassword) && !String.IsNullOrEmpty(update.NewPassword))
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
        public async Task<BussnessResultModel> DeactiveCustomer(int id)
        {
            var customer = await _repositoryManager.User.GetTypeUserId(id, UserType.Customer, true);
            if (customer == null)
            {
                return new BussnessResultModel(null, _locService.GetLocalizedStringValue("correctLink"), false);
            }
            customer.Status = Status.NotActive;

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

           
            return new BussnessResultModel(customer, _locService.GetLocalizedStringValue("successDeactive"));
        }
        public async Task<BussnessResultModel> ActiveCustomer(int id)
        {
            var customer = await _repositoryManager.User.GetTypeUserId(id, UserType.Customer, true);
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
        //-------------------------
        public async Task<BussnessResultModel> ValidateUser(UserForAuthenticationDto user)
        {
            if (!await _authManager.ValidateUser(user))
            {
                return new BussnessResultModel(null, "Wrong user name or password.",false);
            }
            var token = await _authManager.CreateToken();
            return new BussnessResultModel(token);
        }  
       
    }
        
}
