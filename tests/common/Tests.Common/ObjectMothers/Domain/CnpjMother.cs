using System.Buffers;
using System.Text.RegularExpressions;

namespace Tests.Common.ObjectMothers.Domain
{
    public static class CnpjMother
    {
        public static IReadOnlyCollection<string> WellformedCnpjs = new string[]
        {
            "75.396.039/0001-79",
            "07.265.109/0001-08"
        };

        private static string maskedValidCnpj = WellformedCnpjs.ElementAt(0);
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
