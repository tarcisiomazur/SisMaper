using Persistence;

namespace SisMaper.Models.Views;

[View(ViewName = "ListarClientes")]
public class ListarClientes
{
    public enum Pessoa
    {
        Null,
        Fisica,
        Juridica
    }

    public Fatura.Fatura_Status Status { get; set; }

    public long Id { get; set; }

    public string Nome { get; set; }

    public string Endereco { get; set; }

    public string Cidade { get; set; }

    public string Estado { get; set; }

    public Pessoa Tipo { get; set; }
}