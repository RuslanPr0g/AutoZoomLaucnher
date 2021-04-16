using System.Threading.Tasks;

namespace LaunchService.Serializing
{
    public abstract class FileSerializer
    {
        public T Deserialize<T>(string dataToDeserialize) where T : class
        {
            IFileSerializable<T> serializer = CreateSerializer<T>();
            return serializer.Deserialize(dataToDeserialize);
        }

        public async Task<byte[]> Serialize<T>(T dataToSerialize)
        {
            IFileSerializable<T> serializer = CreateSerializer<T>();
            return await serializer.Serialize(dataToSerialize);
        }

        public abstract IFileSerializable<T> CreateSerializer<T>();
    }
}
