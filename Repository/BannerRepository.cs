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
        public async Task<Banner> GetBannerByType(int langId, string type ,bool trackChanges)
         => await FindByCondition(c => c.LangId.Equals(langId) && c.Type == type, trackChanges).FirstOrDefaultAsync();
        public async Task<List<Banner>> GetAllBanner(bool trackChanges)
         => await FindAll(trackChanges).Include(x => x.Image).Include(x => x.Image.ImageSettings).OrderByDescending(c=>c.CreatedAt).ToListAsync();
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
        public async Task<IEnumerable<StaticPages>> GetAllPages(bool trackChanges)
        => await FindAll(trackChanges).ToListAsync();
    }
    public class ServicesRepository : RepositoryBase<Service>, IServicesRepository
    {
        public ServicesRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public List<Service> GetAllServices(bool trackChanges)
          =>  FindAll(trackChanges).OrderByDescending(e => e.CreatedAt).Include(x => x.Image).Include(x => x.Image.ImageSettings).ToList();

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

    } 
    public class SliderRepository : RepositoryBase<Sliders>, ISliderRepository
    {
        public SliderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public List<Sliders> GetSlidersForWeb()
        =>  FindByCondition(c => c.IsStatus == Status.Active && c.Type == SlidersImageType.Web, false)
                .Include(x => x.Image).Include(x => x.Image.ImageSettings).OrderByDescending(e => e.CreatedAt).ToList();
        public List<Sliders> GetSlidersForMobile()
      =>  FindByCondition(c => c.IsStatus == Status.Active && c.Type == SlidersImageType.Mobile, false)
                .Include(x => x.Image).Include(x => x.Image.ImageSettings).OrderByDescending(e => e.CreatedAt).ToList();
        public async Task<Sliders> GetSlideById(int id, bool trackChanges)
        => await FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
        public void DeleteSlider(Sliders sliders) => Delete(sliders);
        public void AddSlider(Sliders sliders) => Create(sliders);
    }
   
}
