using System;

namespace TesteTBFORTE.Models
{
    public class ProjetoVM : LogVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string CriadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public string AtualizadoPor { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public string UrlJira { get; set; }
    }
}
