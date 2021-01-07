using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MexcelU5.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<char> BusquedaLetra { get; set; }
        public IEnumerable<CelViewModel> Celulares { get; set; }
    }
}
