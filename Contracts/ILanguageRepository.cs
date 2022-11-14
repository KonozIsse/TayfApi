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
        Task<Language> GetDefaultLanguage(bool trackChanges);
        Task<IEnumerable<Language>> ListLanguage(bool trackChanges);
        bool IsExistLang(string code);
        void DeleteLanguage(Language language);
    }
}
