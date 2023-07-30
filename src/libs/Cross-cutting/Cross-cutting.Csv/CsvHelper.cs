using CsvHelper;
using System.Globalization;
using System.Text;

namespace CrossCutting.Csv
{
    internal class CsvHelper : ICsvHelper
    {
        public virtual IReadOnlyCollection<T> Fetch<T>(Stream stream, Encoding encoding = default!, CultureInfo cultureInfo = default!)
        {
            var items = new List<T>();
            using var reader = encoding is not null ? new StreamReader(stream, encoding) : new StreamReader(stream);
            using var csv = new CsvReader(reader, cultureInfo ?? CultureInfo.InvariantCulture);
            csv.GetRecords<T>().ToList().ForEach(items.Add);

            return items;
        }

        public virtual IReadOnlyCollection<T> Fetch<T>(string filePath, Encoding encoding = default!, CultureInfo cultureInfo = default!)
        {
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return Fetch<T>(stream, encoding, cultureInfo);
        }
    }
}
