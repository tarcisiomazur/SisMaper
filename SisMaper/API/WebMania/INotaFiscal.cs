using SisMaper.API.WebMania.Models;
using SisMaper.Models;

namespace SisMaper.API.WebMania
{
    public abstract class INotaFiscal
    {
        public string Json { get; set; }
        public NotaFiscal NotaFiscal { get; set; }
        public NF_Result? NF_Result { get; set; }
        public abstract string BuildJsonDefault();
        public abstract bool Emit();
    }
}