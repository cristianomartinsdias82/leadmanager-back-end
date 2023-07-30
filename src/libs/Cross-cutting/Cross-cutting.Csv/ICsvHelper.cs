using System.Globalization;
using System.Text;

namespace CrossCutting.Csv
{
    public interface ICsvHelper
    {
        IReadOnlyCollection<T> Fetch<T>(
            Stream stream,
            Encoding encoding = default!,
            CultureInfo cultureInfo = default!);

        IReadOnlyCollection<T> Fetch<T>(
            string filePath,
            Encoding encoding = default!,
            CultureInfo cultureInfo = default!);
    }
}