using MacResourceFork;

namespace Vette
{
	public static class VetteResourceParser
	{
		/// <summary>
		/// Parse binary data from a single resource extracted from a resource fork.
		/// </summary>
		public static T Parse<T>(byte[] bytes, int id, string name) where T : ResourceBase, new()
		{
			return ParseInternal<T>(bytes, id, name);
		}

		/// <summary>
		/// Parse binary data from a Resource object extracted from a resource fork.
		/// </summary>
		public static T Parse<T>(Resource resource) where T : ResourceBase, new()
		{
			return ParseInternal<T>(resource.Data, resource.Id, resource.Name);
		}

		private static T ParseInternal<T>(byte[] bytes, int id, string name) where T : ResourceBase, new()
		{
			var resource = new T();
			var span = new ReadOnlySpan<byte>(bytes);
			resource.Parse(ref span);

			resource.id = id;
			resource.name = name;
			return resource;
		}
	}
}
