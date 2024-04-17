using System.Collections.Generic;

namespace DWLaw.Selenium.Tests.Dtos
{
    internal class PoloDto
    {
        public string Nome { get; set; }
        public string Classificacao { get; set; }
        public string Tipo { get; set; }
        public ICollection<AdvogadoDto> Advogados { get; set; }
    }
}
