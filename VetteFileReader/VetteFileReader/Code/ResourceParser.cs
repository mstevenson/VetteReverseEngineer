using System.IO;

namespace VetteFileReader
{
	public class ResourceParser
	{
		public static T ParseFile<T>(string filePath) where T : IResource, new()
		{
			using (var stream = File.Open(filePath, FileMode.Open))
			{
				return ParseInternal<T>(stream);
			}
		}

		public static T ParseBytes<T>(byte[] bytes) where T : IResource, new()
		{
			using (var stream = new MemoryStream(bytes))
			{
				return ParseInternal<T>(stream);
			}
		}

		private static T ParseInternal<T>(Stream stream) where T : IResource, new()
		{
			var resource = new T();
			using (var reader = new BinaryReaderBigEndian(stream))
			{
				resource.Parse(reader);
			}
			return resource;
		}
	}
}
