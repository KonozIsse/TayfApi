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
        public UserBL(IRepositoryManager repositoryManager, IMapper mapper, ImageBL imageBL , UserManager<User> userManager)
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
           // var storeDto = storesDto.First();
            //foreach (var store in stores)
            //{
            //    storeDto.Avater = await _imageBL.GetImageThumbnail(store.Avater);
            //}
            return storesDto;
        }
     
        public async Task<StoreDto> GetStore(int id)
        {
            var store = await _repositoryManager.User.GetStoreId(id);
            var storeDto = _mapper.Map<StoreDto>(store);
           // storeDto.Avater = await _imageBL.GetImageMedium(store.Avater);
            return storeDto;
        }
        public async Task AddStore(CreateStoreDto createStoreDto)
        {
            var store = _mapper.Map<User>(createStoreDto);
            store.RoleId = 3;
            store.PhoneNumber = createStoreDto.PhoneNumber;
            store.UserName = createStoreDto.FirstName + createStoreDto.LastName;
            _repositoryManager.User.AddUser(store);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateStore(int id, UpdateStoreDto updateStoreDto)
        {
            var store = await _repositoryManager.User.GetUserId(id, true);
            store.RoleId = 3;
            _mapper.Map(updateStoreDto, store);
            await _repositoryManager.SaveAsync();
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
        public async Task EditeUserSubscribe(bool newsletter, int userId)
        {
            var user = await _repositoryManager.User.GetCustomerId(userId, true);
            user.IsSubscribe = newsletter;
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateUser(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _repositoryManager.User.GetUserId(userId, true);
            user.UserName = updateUserDto.FirstName + updateUserDto.LastName;
            var devices = await _repositoryManager.Device.GetDevicesUserId(userId, true);
            if (user.PasswordHash != updateUserDto.Password && devices != null)
            {
                foreach (var device in devices) 
                {
                    user.PasswordHash = updateUserDto.Password;
                    device.DeviceToken = user.PasswordHash;
                }
                _mapper.Map(updateUserDto, user);
            }
            await _repositoryManager.SaveAsync();
        }
        public async Task RemoveUserData(int id)
        {
            var user = await _repositoryManager.User.GetUserId(id, false);
            var addresses = await _repositoryManager.Address.GetAllAddressesByCustomerId(id);
            if (addresses != null)
            {
                foreach (var address in addresses)
                {
                    _repositoryManager.Address.DeleteAddress(address);
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
            user.PhoneNumber = userRegister.PhoneNumber;
            user.UserName = userRegister.FirstName + userRegister.LastName;
            var result = await _userManager.CreateAsync(user, userRegister.Password);
          //  _repositoryManager.User.AddUser(user);
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
                var code = Convert.ToInt32(GenerateRandomNo());
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
        public string GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            string rrd = _rdm.Next(_min, _max).ToString();
            return rrd;
        }
    }
}
