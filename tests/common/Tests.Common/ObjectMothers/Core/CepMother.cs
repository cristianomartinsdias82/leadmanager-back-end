using System.Buffers;
using System.Text.RegularExpressions;

namespace Tests.Common.ObjectMothers.Core
{
    public static class CepMother
    {
        private static string maskedValidCep = "04858-040";
        private static string maskedInvalidCep = "11111-111";

        private static SpanAction<char, string> messUp = (span, value) =>
        {
            value.AsSpan().CopyTo(span);
            span[4] = '-';
            span[5] = '1';
        };

        public static string MaskedWellformedValidOne()
            => maskedValidCep;

        public static string MaskedWellformedInvalidOne()
            => maskedInvalidCep;

        public static string MaskedMalformedValidOne()
            => string.Create(maskedValidCep.Length, maskedValidCep, messUp);

        public static string MaskedMalformedInvalidOne()
            => string.Create(maskedValidCep.Length, maskedInvalidCep, messUp);
    }
}
