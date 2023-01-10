using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Entities.Models.Enums;
using Entities.Models;
using Entities.Models.CorePushModels;
using Contracts;

namespace BusinessLogic.Services
{
    public class LangaugeService : ILanguageService
    {
        private readonly IRepositoryManager _repositoryManager;
        public LangaugeService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<Language> GetCodeOrDefaultLanguage(string code)
        {
           var codeLang = await _repositoryManager.Language.GetCodeLanguage(code,false);
           var defaultLang = await _repositoryManager.Language.GetDefaultLanguage(false);
           return codeLang ?? defaultLang;
        }
    }
}
