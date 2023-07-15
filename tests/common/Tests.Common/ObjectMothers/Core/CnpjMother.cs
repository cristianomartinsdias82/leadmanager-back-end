using System.Buffers;
using System.Text.RegularExpressions;

namespace Tests.Common.ObjectMothers.Core
{
    public static class CnpjMother
    {
        private static string maskedValidCnpj = "75.396.039/0001-79";
        private static string maskedInvalidCnpj = "33.646.178/0001-12";

        private static SpanAction<char, string> messItUp = (span, cnpj) =>
        {
            cnpj.AsSpan().CopyTo(span);
            span[2] = '-';
            span[5] = '/';
        };

        public static string MaskedWellformedValidOne()
            => maskedValidCnpj;

        public static string MaskedWellformedInvalidOne()
            => maskedInvalidCnpj;

        public static string UnmaskedValidOne()
            => Regex.Replace(maskedValidCnpj, @"\D", string.Empty);

        public static string UnmaskedInvalidOne()
            => Regex.Replace(maskedInvalidCnpj, @"\D", string.Empty);

        public static string MaskedMalformedValidOne()
            => string.Create(maskedValidCnpj.Length, maskedValidCnpj, messItUp);

        public static string MaskedMalformedInvalidOne()
            => string.Create(maskedValidCnpj.Length, maskedInvalidCnpj, messItUp);
    }
}
