using ServiceStack.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchService.Serializing
{
    public class CsvFileSerializer<T> : IFileSerializable<T>
    {
        public T Deserialize(string fileString)
        {
            IEnumerable<T> deserializedTransactions =
                CsvSerializer.DeserializeFromString<IEnumerable<T>>(fileString);

            // this thing could return many (list of) transactions, it needs to think about one.

            return deserializedTransactions.First();
        }

        public async Task<byte[]> Serialize(T fileString)
        {
            return Encoding.UTF8.GetBytes(CsvSerializer.SerializeToCsv(new List<T>() { fileString }));
        }
    }
}
