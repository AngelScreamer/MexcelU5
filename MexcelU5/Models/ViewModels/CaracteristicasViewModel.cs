using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MexcelU5.Models.ViewModels
{
    public class CaracteristicasViewModel
    {
        public Smartphones Smartphones { get; set; }
        public IEnumerable<Marcas> LasMarcas { get; set; }
    }
}
