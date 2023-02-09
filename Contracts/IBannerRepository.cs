using Entities.Models;
using Entities.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBannerRepository
    {
        Task<Banner> GetBannerByType(int langId, string type, bool trackChanges);
        Task<List<Banner>> GetAllBanner(string search, bool trackChanges);
        Task<Banner> GetBannerById(int id, bool trackChanges);
        Task<Banner> GetBannerId(int id, bool trackChanges);
    }
    public interface IStaticPagesRepository
    {
        Task<StaticPages> GetPage(int id, bool trackChanges);
        Task<StaticPages> GetTypePage(PageType type, bool trackChanges);
        Task<IEnumerable<StaticPages>> GetAllPages(string search ,bool trackChanges);
    } 
    public interface IServicesRepository
    {
        Task<List<Service>> GetAllServices(string search, bool trackChanges);
        Task<Service> GetServiceById(int id, bool trackChanges, bool includeDetails = true);
        void DeleteService(Service service);
    }
    public interface ISliderRepository
    {
        Task<List<Sliders>> GetSliders(); 
        Task<List<Sliders>> GetSlidersForWeb(string search);
        Task<List<Sliders>> GetSlidersForMobile();
        Task<Sliders> GetSlideById(int id, bool trackChanges);
        void AddSlider(Sliders sliders);
        void DeleteSlider(Sliders sliders);
    }
    
}
