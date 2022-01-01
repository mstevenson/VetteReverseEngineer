using System;
using System.IO;
using MacResourceFork;

namespace VetteFileReader
{
	public class VetteResourceParser
	{
		/// <summary>
		/// Parse binary data from a single resource extracted from a resource fork.
		/// </summary>
		public static T Parse<T>(byte[] bytes) where T : IResource, new()
		{
			using (var stream = new MemoryStream(bytes))
			{
				return ParseInternal<T>(stream);
			}
		}

		/// <summary>
		/// Parse binary data from a Resource object extracted from a resource fork.
		/// </summary>
		public static T Parse<T>(Resource resource) where T : IResource, new()
		{
			using (var stream = new MemoryStream(resource.data))
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
