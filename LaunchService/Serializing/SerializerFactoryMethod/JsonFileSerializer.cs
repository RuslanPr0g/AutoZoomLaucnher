namespace LaunchService.Serializing
{
    public class JsonFileSerializer : FileSerializer
    {
        public override IFileSerializable<T> CreateSerializer<T>()
        {
            return new JsonFileSerializer<T>();
        }
    }
}
