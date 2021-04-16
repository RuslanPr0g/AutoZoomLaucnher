using System.Threading.Tasks;

namespace LaunchService.Serializing
{
    public interface IFileSerializable<T>
    {
        T Deserialize(string fileString);
        Task<byte[]> Serialize(T dataToSerialize);
    }
}
