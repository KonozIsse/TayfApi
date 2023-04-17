using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILanguageRepository
    {
        Task<Language> GetCodeLanguage(string lange, bool trackChanges);
        Task<Language> GetCodeLanguageId(int id, bool trackChanges);
        Task<Language> GetDefaultLanguage();
        Task<IEnumerable<Language>> GetListLanguage(bool trackChanges);
        Task<IEnumerable<Language>> GetAllLanguage(string search , string filter);
        void DeleteLanguage(Language language);
        Task<IEnumerable<Language>> GetListLanguageImage(int imageId, bool trackChanges);
    }
}
