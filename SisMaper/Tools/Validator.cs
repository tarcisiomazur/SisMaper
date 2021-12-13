using System.Linq;

namespace SisMaper.Tools;

public static class Validator
{
    public static bool IsCpf(this string cpf)
    {
        int[] multiplicador1 = {10, 9, 8, 7, 6, 5, 4, 3, 2};
        int[] multiplicador2 = {11, 10, 9, 8, 7, 6, 5, 4, 3, 2};

        cpf = cpf.Trim().Replace(".", "").Replace("-", "");
        if (cpf.Length != 11)
        {
            return false;
        }

        if (cpf.All(c => c == cpf[0])) return false; // Todos os chars são iguais
        if (cpf.Any(c => !char.IsDigit(c))) return false; // Se tiver algum não dígito

        var tempCpf = cpf[..9];
        var soma = 0;

        for (var i = 0; i < 9; i++)
            soma += (tempCpf[i] - '0') * multiplicador1[i];

        var resto = soma % 11;
        if (resto < 2)
        {
            resto = 0;
        }
        else
        {
            resto = 11 - resto;
        }

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += (tempCpf[i] - '0') * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2)
        {
            resto = 0;
        }
        else
        {
            resto = 11 - resto;
        }

        digito += resto;

        return cpf.EndsWith(digito);
    }

    public static bool IsCnpj(this string cnpj)
    {
        int[] multiplicador1 = {5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};
        int[] multiplicador2 = {6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};

        cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
        if (cnpj.Length != 14)
        {
            return false;
        }

        if (cnpj.Any(c => !char.IsDigit(c))) return false; // Se tiver algum não dígito

        var tempCnpj = cnpj[..12];
        var soma = 0;

        for (var i = 0; i < 12; i++)
            soma += (tempCnpj[i] - '0') * multiplicador1[i];

        var resto = soma % 11;
        if (resto < 2)
        {
            resto = 0;
        }
        else
        {
            resto = 11 - resto;
        }

        var digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;
        for (var i = 0; i < 13; i++)
            soma += (tempCnpj[i] - '0') * multiplicador2[i];

        resto = soma % 11;
        if (resto < 2)
        {
            resto = 0;
        }
        else
        {
            resto = 11 - resto;
        }

        digito += resto;

        return cnpj.EndsWith(digito);
    }
}