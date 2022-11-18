using Entities.DataTransferObjects;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using AutoMapper;
using Contracts;

namespace BusinessLogic.ApiClasses
{
    public class UserBL
    {
        protected readonly IRepositoryManager _repositoryManager;
        protected readonly IMapper _mapper;
        private readonly ImageBL _imageBL;
        protected readonly UserManager<User> _userManager;

        public UserBL(IRepositoryManager repositoryManager, IMapper mapper, ImageBL imageBL ,UserManager<User> userManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _imageBL = imageBL;
            _userManager = userManager;
        }
        //Store------------------------------------------------
        public async Task<List<StoreDto>> GetStores()
        {
            var stores = await _repositoryManager.User.GetAllStores(false);
            var storesDto = _mapper.Map<List<StoreDto>>(stores);
            var storeDto = storesDto.First();
            foreach (var store in stores)
            {
                storeDto.Avater = await _imageBL.GetImageThumbnail(store.Avater);
            }
            return storesDto;
        }
     
        public async Task<StoreDto> GetStore(int id)
        {
            var store = await _repositoryManager.User.GetStoreId(id);
            var storeDto = _mapper.Map<StoreDto>(store);
            storeDto.Avater = await _imageBL.GetImageMedium(store.Avater);
            return storeDto;
        }
        public async Task AddStore(CreateStoreDto createStoreDto)
        {
            var store = _mapper.Map<User>(createStoreDto);
            store.RoleId = 3;
            store.Status = Status.Active;
            store.UserName = createStoreDto.FirstName + createStoreDto.LastName;
            _repositoryManager.User.AddUser(store);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateStore(int id, UpdateStoreDto updateStoreDto)
        {
            var store = await _repositoryManager.User.GetUserId(id, true);
            store.RoleId = 3;
            store.Lang = "en";
            _mapper.Map(updateStoreDto, store);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteStore(int storeId)
        {
            var store = await _repositoryManager.User.GetUserId(storeId, false);
            var customerStores = await _repositoryManager.CustomerStore.GetCustomersStoreId(storeId);
            foreach (var customerStore in customerStores)
            {
                _repositoryManager.CustomerStore.DeleteCustomerStore(customerStore);
                await _repositoryManager.SaveAsync();
            }
            var orders = await _repositoryManager.Order.GetOrdersToStore(storeId);
            foreach (var item in orders)
            {
                item.IsDeleted = true;
                await _repositoryManager.SaveAsync();
            }

            _repositoryManager.User.DeleteUser(store);
            await _repositoryManager.SaveAsync();
        }
        public async Task DeleteByStore(int storeId)
        {
            var customerStores = await _repositoryManager.CustomerStore.GetCustomersStoreId(storeId);
            foreach (var customerStore in customerStores)
            {
                _repositoryManager.CustomerStore.DeleteCustomerStore(customerStore);
                await _repositoryManager.SaveAsync();
            }
        }
        //user------------------------------------------------
        public async Task<User> GetUserById(int id)
        {
            return await _repositoryManager.User.GetUserId(id, false);
        }
        public async Task EditEmail(int userId, string email)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true);
            user.Email = email;
            await _repositoryManager.SaveAsync();
        }
        public async Task ChangePassword(int userId, string newPassword)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true);
            user.PasswordHash = newPassword;
            await _repositoryManager.SaveAsync();
        }
        public async Task VerifyUser(int userId, int code)
        {
            var isVerify = await _repositoryManager.User.VerifiedCodeUser(userId, code, true);
            if (isVerify != null)
            {
                isVerify.IsMobileVerified = true;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task ConfirmCode(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true);

            if (updateUserDto.Telephone != null)
                user.Telephone = updateUserDto.Telephone;

            if (updateUserDto.CountryId != 0)
                user.CountryId = updateUserDto.CountryId;

            if (updateUserDto.VerifiedCode != 0)
                user.VerifiedCode = updateUserDto.VerifiedCode;

            user.IsMobileVerified = true;
            await _repositoryManager.SaveAsync();
        }
        public async Task EditeUserSubscribe(bool newsletter, int userId)
        {
            var user = await _repositoryManager.User.GetCustomerId(userId, true);
            user.IsSubscribe = newsletter;
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateUser(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true);
            if (user.PasswordHash != updateUserDto.Password && user.Devices != null)
                foreach (var device in user.Devices) { device.DeviceToken = updateUserDto.Password; }
            _mapper.Map(updateUserDto, user);
            await _repositoryManager.SaveAsync();
        }
        public async Task RemoveUserData(int id)
        {
            var user = await _repositoryManager.User.GetUserId(id, false);

            if (user.Addresses != null && user.Addresses.Count > 0)
            {
                foreach (var Address in user.Addresses)
                {
                    _repositoryManager.Address.DeleteAddress(Address);
                }
            }
            if (user.Devices != null && user.Devices.Count > 0)
            {
                foreach (var Device in user.Devices)
                {
                    _repositoryManager.Device.DeleteDevice(Device);
                }
            }
            if (user.CustomerProducts != null && user.CustomerProducts.Count > 0)
            {
                foreach (var CustomerProduct in user.CustomerProducts)
                {
                    _repositoryManager.CustomerProduct.DeleteCustomerProduct(CustomerProduct);
                }
            }
            var orders = await _repositoryManager.Order.GetOrdersToCustomer(id);
            if (orders != null )
            {
                foreach (var order in orders)
                {
                    _repositoryManager.Order.DeleteOrder(order);
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
        }
        public async Task RegisterUser(CreateCustomerDto userRegister)
        {
            var user = _mapper.Map<User>(userRegister);
            
            if (userRegister.TypeRegister == TypeRegister.Facebook)
            {
                user.TypeRegister = TypeRegister.Facebook;
                user.Avater = userRegister.SocialImage;
            }
            else if (user.TypeRegister == TypeRegister.Google)
            {
                user.TypeRegister = TypeRegister.Google;
                user.Avater = userRegister.SocialImage;
            }
            else if (user.TypeRegister == TypeRegister.Apple)
            {
                user.TypeRegister = TypeRegister.Apple;
                user.Avater = userRegister.SocialImage;
            }
            else
            {
                user.TypeRegister = TypeRegister.Normal;
            }
            user.RoleId = 2;
            user.Status = Status.Active;
            user.UserName = userRegister.FirstName + userRegister.LastName;
            _repositoryManager.User.AddUser(user);
            await _repositoryManager.SaveAsync();
        }
        public async Task<User> FacebookUser(string socialId)
        {
            return await _repositoryManager.User.GetFacebookRegisterUser(socialId);
        }
        public async Task<User> GoogleUser(string socialId)
        {
            return await _repositoryManager.User.GetGoogleRegisterUser(socialId);
        }
        public async Task<User> AppleUser(string socialId)
        {
            return await _repositoryManager.User.GetAppleRegisterUser(socialId);
        }
        public async Task ResetPassword(ResetPasswordDto resetPasswordModel)
        {
            //var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            //var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            
           // return resetPassResult;
        }
      
        //Device------------------------------------------------
        public async Task AddDevice(CreateDeviceDto createStoreDto)
        {
            var device = _mapper.Map<Device>(createStoreDto);
             //device.UserId = GetCurrentUserId() ;
            device.IsStatus = Status.Active;
            _repositoryManager.Device.AddDevice(device);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateDevice(string deviceId , UpdateDeviceDto updateDeviceDto)
        {
            var user = await _repositoryManager.User.GetUserId(updateDeviceDto.UserId, true);
            //user.Lang = lang;
            await _repositoryManager.SaveAsync();
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

        public async Task UpdateFcm(int userId, string deviceId, string fcmToken)
        {
            var device = await _repositoryManager.Device.GetDeviceUser(userId, Convert.ToInt32(deviceId), true); 
            if (device != null)
            {
                device.FcmToken = fcmToken;
                await _repositoryManager.SaveAsync();
            }
        }
        public async Task ResendToken(int userId)
        {
            var user = await _repositoryManager.User.GetUserId(userId, false); 
            string token = _repositoryManager.Device.GetTokenUser(userId);
            //var code = Convert.ToInt32(GenerateRandomNo());
            //user.VerifiedCode = code;
            await _repositoryManager.SaveAsync();
        }
       // DeleteDevicesUserId
        public async Task LogOutDevice(int userId)
        {
            var devicesUser = await _repositoryManager.Device.GetDevicesUserId(userId);
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
    }
}
