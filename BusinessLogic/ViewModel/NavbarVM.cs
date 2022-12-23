using Entities.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModel
{
    public class NavbarVM
    {
        public LanguageDto DefaultLanguage { get; set; }
        public List<LanguageDto> Languages { get; set; }
        public List<CurrencyDto> Currencies { get; set; }
    }
}
