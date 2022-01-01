using System.IO;
using MacResourceFork;

namespace VetteFileReader
{
	public class VetteResourceParser
	{
		/// <summary>
		/// Parse file that contains binary data from a single resource extracted from a resource fork.
		/// </summary>
		public static T ConvertFile<T>(string filePath) where T : IResource, new()
		{
			using (var stream = File.Open(filePath, FileMode.Open))
			{
				return ConvertInternal<T>(stream);
			}
		}

		/// <summary>
		/// Parse binary data from a single resource extracted from a resource fork.
		/// </summary>
		public static T Parse<T>(byte[] bytes) where T : IResource, new()
		{
			using (var stream = new MemoryStream(bytes))
			{
				return ConvertInternal<T>(stream);
			}
		}

		/// <summary>
		/// Parse binary data from a Resource object extracted from a resource fork.
		/// </summary>
		public static T Parse<T>(Resource resource) where T : IResource, new()
		{
			using (var stream = new MemoryStream(resource.data))
			{
				return ConvertInternal<T>(stream);
			}
		}

		private static T ConvertInternal<T>(Stream stream) where T : IResource, new()
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
