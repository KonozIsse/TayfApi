using Contracts;
using Entities;
using Entities.Models;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BannerRepository : RepositoryBase<Banner>, IBannerRepository
    {
        public BannerRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Banner> GetBannerByType( int langId ,string type ,bool trackChanges)
         => await FindByCondition(c => c.LangId == langId &&  c.Type == type, trackChanges).Include(c=>c.Language).FirstOrDefaultAsync();
        public async Task<Banner> GetBannerImage(int imageId, bool trackChanges)
        => await FindByCondition(c => c.ImgId == imageId , trackChanges).FirstOrDefaultAsync();
        public async Task<List<Banner>> GetAllBanner(string search, string filter, bool trackChanges)
        {
            var banners =  FindAll(trackChanges);
            if (!string.IsNullOrEmpty(search))
            {
                if (filter == "0")
                {
                    banners = banners.Where(c => c.Title.Contains(search));
                }
                else
                {
                    banners = banners.Where(c => c.Title.Contains(search)|| c.Language.Name.Contains(search));
                }
               
            }
            return await banners.Include(c=>c.Language).Include(x => x.Image).Include(x => x.Image.ImageSettings).OrderByDescending(c => c.CreatedAt).ToListAsync();
        } 
        public async Task<Banner> GetBannerById(int id, bool trackChanges)
            => await FindByCondition(c => c.Id.Equals(id) , trackChanges).Include(x => x.Image).Include(x => x.Image.ImageSettings).FirstOrDefaultAsync();
        public async Task<Banner> GetBannerId(int id, bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(id), trackChanges).FirstOrDefaultAsync();

    }
    public class StaticPagesRepository : RepositoryBase<StaticPages>, IStaticPagesRepository
    {
        public StaticPagesRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<StaticPages> GetPage(int id, bool trackChanges)
         => await FindByCondition(c => c.Id.Equals(id), trackChanges).FirstOrDefaultAsync(); 
        public async Task<StaticPages> GetTypePage(PageType type, bool trackChanges)
         => await FindByCondition(c => c.PageType == type, trackChanges).FirstOrDefaultAsync();
        public async Task<IEnumerable<StaticPages>> GetAllPages(string search ,string filter, bool trackChanges)
        {
            var pages = FindAll(trackChanges);
            if (!string.IsNullOrEmpty(search))
            {
                if(filter == "0")
                {
                    pages =  pages.Where(c => c.Title.Contains(search));
                }
                else
                {
                    pages = pages.Where(c => c.Title.Contains(search) ||  c.Description.Contains(search));
                }
               
            }
            return await pages.ToListAsync();
        }
    }
    public class ServicesRepository : RepositoryBase<Service>, IServicesRepository
    {
        public ServicesRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Service>> GetAllServices(string search ,bool trackChanges)
        {
            var servies = FindAll(trackChanges);
            if (!string.IsNullOrEmpty(search))
            {
                servies = servies.Where(c => c.Title.Contains(search)|| c.Description.Contains(search));
            }
            return await servies.OrderByDescending(e => e.CreatedAt).Include(x => x.Image).Include(x => x.Image.ImageSettings).ToListAsync();
        }
        public async Task<Service> GetServiceById(int id, bool trackChanges, bool includeOtherModels = true)
        {
            var service = FindByCondition(c => c.Id.Equals(id), trackChanges);
            if (includeOtherModels)
            {
                service = service.Include(x => x.Image).Include(x => x.Image.ImageSettings);
            }
            return await service.SingleOrDefaultAsync();
        }
       public void DeleteService(Service service) => Delete(service);

        public async Task<Service> GetServiceImageId(int imageId, bool trackChanges)
        => await FindByCondition(c => c.ImgId == imageId, trackChanges).FirstOrDefaultAsync();
    } 
    public class SliderRepository : RepositoryBase<Sliders>, ISliderRepository
    {
        public SliderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<List<Sliders>> GetSliders()
       => await FindByCondition(c => c.IsStatus == Status.Active && c.Type == SlidersImageType.Web, false)
            .Include(x => x.Image).Include(x => x.Image.ImageSettings).OrderByDescending(e => e.CreatedAt).ToListAsync();
        
        public async Task<List<Sliders>>  GetSlidersForWeb(string search,string filter)
        {
            var sliders = FindByCondition(c => c.IsStatus == Status.Active && c.Type == SlidersImageType.Web, false);
            if (!string.IsNullOrEmpty(search))
            {
                if(filter == "0")
                {
                    sliders = sliders.Where(c => c.Title.Contains(search));
                }
                else
                {
                    sliders = sliders.Where(c => c.Title.Contains(search)|| c.Decription.Contains(search));
                }
            }
           return await sliders.Include(x => x.Image).Include(x => x.Image.ImageSettings).OrderByDescending(e => e.CreatedAt).ToListAsync();
        }
        public async Task<List<Sliders>>  GetSlidersForMobile(string search)
        {
            var sliders = FindByCondition(c => c.IsStatus == Status.Active && c.Type == SlidersImageType.Mobile, false);
            if (!string.IsNullOrEmpty(search))
            {
                sliders = sliders.Where(c => c.Title.Contains(search) || c.Decription.Contains(search));
            }
            return await sliders.Include(x => x.Image).Include(x => x.Image.ImageSettings).OrderByDescending(e => e.CreatedAt).ToListAsync();
        }
        public async Task<Sliders> GetSlideById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
        public void DeleteSlider(Sliders sliders) => Delete(sliders);
        public void AddSlider(Sliders sliders) => Create(sliders);

        public async Task<Sliders> GetSlideImageId(int imageId, bool trackChanges)
       => await FindByCondition(c => c.ImgId.Equals(imageId), trackChanges).SingleOrDefaultAsync();
    }
   
}
