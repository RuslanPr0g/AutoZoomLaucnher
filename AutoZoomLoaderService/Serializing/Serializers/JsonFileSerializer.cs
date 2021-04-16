using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace AutoZoomLoaderService.Serializing
{
    public class JsonFileSerializer<T> : IFileSerializable<T>
    {
        public T Deserialize(string fileString)
        {
            return JsonConvert.DeserializeObject<T>(fileString);
        }

        public async Task<byte[]> Serialize(T objectToSerialize)
        {
            return
                Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(objectToSerialize));
        }
    }
}
