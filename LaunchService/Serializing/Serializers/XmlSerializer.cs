using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LaunchService.Serializing
{
    public class XmlSerializer<T> : IFileSerializable<T>
    {
        public T Deserialize(string fileString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using StringReader stringReader = new StringReader(fileString);

            return (T)xmlSerializer.Deserialize(stringReader);
        }

        public async Task<byte[]> Serialize(T dataToSerialize)
        {
            var serializer = new XmlSerializer(dataToSerialize.GetType());

            string xmlTransactionString = await GetXML(serializer, dataToSerialize);

            return Encoding.ASCII.GetBytes(xmlTransactionString);
        }

        private static async Task<string> GetXML(XmlSerializer serializer, T transactionFileModel)
        {
            using
            var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, transactionFileModel);

            memoryStream.Position = 0;
            return await new StreamReader(memoryStream).ReadToEndAsync();
        }
    }
}
