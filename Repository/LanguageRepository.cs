using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class LanguageRepository : RepositoryBase<Language>, ILanguageRepository
    {
        public LanguageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public async Task<Language> GetCodeLanguage(string lange, bool trackChanges)
        => await FindByCondition(c => c.Code == lange, trackChanges).FirstOrDefaultAsync();
        public async Task<Language> GetCodeLanguageId(int id, bool trackChanges)
       => await FindByCondition(c => c.Id == id, trackChanges).FirstOrDefaultAsync();
        public async Task<Language> GetDefaultLanguage()
        => await FindByCondition(c => c.IsStatus == Status.Active,false).SingleOrDefaultAsync();
        public async Task<IEnumerable<Language>> GetListLanguage(bool trackChanges)
        => await FindAll(trackChanges).ToListAsync();
        public async Task<IEnumerable<Language>> GetAllLanguage(string search ,string filter)
        { 
            var langs =  FindAll(false);
            if (!string.IsNullOrEmpty(search))
            {
                if(filter== "0")
                {
                    langs = langs.Where(c => c.Name.Contains(search));
                }
                else if(filter == "1")
                {
                    langs = langs.Where(c => c.Code.Contains(search));
                }
                else
                {
                    langs = langs.Where(c => c.Name.Contains(search) || c.Code.Contains(search));
                }
            }
            return await langs.Include(c=>c.Image).ToListAsync();
        }
        public void DeleteLanguage(Language language) => Delete(language);
        public async Task<IEnumerable<Language>> GetListLanguageImage(int imageId,bool trackChanges)
      => await FindByCondition(c=>c.ImgId == imageId,trackChanges).ToListAsync();
    }
}
