using System.Text.RegularExpressions;

namespace Shared.Validation;

public static class CnpjValidator
{
    private static string[] invalidCnpjs = new string[10];

    static CnpjValidator()
    {
        var c = 0;
        while (c < 10)
            invalidCnpjs[c] = string.Join(string.Empty, Enumerable.Repeat(c++, 14));
    }

    public static bool IsValidCnpj(string input)
    {
        if (!IsCnpjMatch(input))
            return false;

        var cnpj = new string(input);
        int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma;
        int resto;
        string digito;
        string tempCnpj;
        cnpj = cnpj.Trim();
        cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
        if (cnpj.Length != 14)
            return false;

        tempCnpj = cnpj.Substring(0, 12);
        soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        resto = soma % 11;

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = resto.ToString();
        tempCnpj = tempCnpj + digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;

        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;

        digito = digito + resto.ToString();
        
        return cnpj.EndsWith(digito);
    }

    private static bool IsCnpjMatch(string input)
    {
        return Regex.IsMatch(input, @"^[0-9]{2}\.[0-9]{3}\.[0-9]{3}\/[0-9]{4}\-[0-9]{2}$");
    }
}