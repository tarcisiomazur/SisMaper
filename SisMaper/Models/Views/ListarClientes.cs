using Persistence;
using System;

namespace SisMaper.Models.Views;

[View(ViewName = "ListarClientes")]
public class ListarClientes
{
    public enum Pessoa
    {
        Juridica,
        Fisica
    }

    public Fatura.Fatura_Status Status { get; set; }

    public long Id { get; set; }

    public string Nome { get; set; }

    public string Endereco { get; set; }

    public string Cidade { get; set; }

    public string Estado { get; set; }

    public string InscricaoEstadual { get; set; }

    public string RazaoSocial { get; set; }

    public string CPF_CNPJ { get; set; }

    public Pessoa Tipo { get; set; }
}