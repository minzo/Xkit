using System.IO;
using System.Xml.Serialization;

namespace Corekit.Extensions
{
    public static class XmlSerializerExtensions
    {
        /// <summary>
        /// Serialize時に例外が発生してもファイルを壊さないように保存します
        /// </summary>
        public static void SafeSerialize<T>(this XmlSerializer serializer, T o, string filePath)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, o);
                File.WriteAllBytes(filePath, stream.ToArray());
            }
        }

        /// <summary>
        /// XmlSerializerでSerializeします
        /// </summary>
        public static void SerializeXml<T>(this T o, string filePath)
        {
            Cache<T>.Serializer.SafeSerialize(o, filePath);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        public static T Deserialize<T>(this XmlSerializer serializer, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// XmlSerializerCacheクラス
        /// </summary>
        private static class Cache<T>
        {
            public readonly static XmlSerializer Serializer = new XmlSerializer(typeof(T));
        }
    }
}
