namespace DeadworksManaged.Api;

/// <summary>
/// Resource precaching. Call <see cref="AddResource"/> during <see cref="IDeadworksPlugin.OnPrecacheResources"/>
/// to ensure particles, models, and other resources are loaded before use.
/// </summary>
public static unsafe class Precache {
	/// <summary>
	/// Precaches a resource by path (e.g. "particles/abilities/bull_drain.vpcf").
	/// Must be called during OnPrecacheResources.
	/// </summary>
	public static void AddResource(string path) {
		Span<byte> utf8 = Utf8.Encode(path, stackalloc byte[Utf8.Size(path)]);
		fixed (byte* ptr = utf8) {
			NativeInterop.PrecacheResource(ptr);
		}
	}

	/// <summary>
	/// Precaches a single hero by internal name (e.g. "hero_inferno").
	/// Must be called during OnPrecacheResources.
	/// </summary>
	public static void AddHero(string heroName) {
		Span<byte> utf8 = Utf8.Encode(heroName, stackalloc byte[Utf8.Size(heroName)]);
		fixed (byte* ptr = utf8) {
			NativeInterop.PrecacheHero(ptr);
		}
	}

	/// <summary>
	/// Precaches a hero by enum value.
	/// Must be called during OnPrecacheResources.
	/// </summary>
	public static void AddHero(Heroes hero) => AddHero(hero.ToHeroName());

}
