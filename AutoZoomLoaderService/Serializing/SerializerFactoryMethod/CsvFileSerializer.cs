namespace AutoZoomLoaderService.Serializing
{
    public class CsvFileSerializer : FileSerializer
    {
        public override IFileSerializable<T> CreateSerializer<T>()
        {
            return new CsvFileSerializer<T>();
        }
    }
}
