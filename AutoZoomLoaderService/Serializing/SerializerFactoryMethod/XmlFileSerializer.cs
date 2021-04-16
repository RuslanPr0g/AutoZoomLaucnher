namespace AutoZoomLoaderService.Serializing
{
    public class XmlFileSerializer : FileSerializer
    {
        public override IFileSerializable<T> CreateSerializer<T>()
        {
            return new XmlSerializer<T>();
        }
    }
}
