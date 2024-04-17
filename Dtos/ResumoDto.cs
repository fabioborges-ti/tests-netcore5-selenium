using System.Collections.Generic;

namespace DWLaw.Selenium.Tests.Dtos
{
    internal class ResumoDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public ICollection<AdvogadoDto> Advogados { get; set; } = new List<AdvogadoDto>();
    }
}
