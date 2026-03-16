namespace DeadworksManaged.Api;

/// <summary>Extension methods for converting <see cref="Heroes"/> enum values to/from hero name strings and fetching VData.</summary>
public static class HeroTypeExtensions {
	private static readonly Dictionary<Heroes, string> _toName;
	private static readonly Dictionary<string, Heroes> _fromName;

	static HeroTypeExtensions() {
		_toName = new Dictionary<Heroes, string>();
		_fromName = new Dictionary<string, Heroes>(StringComparer.OrdinalIgnoreCase);
		foreach (var value in Enum.GetValues<Heroes>()) {
			// Convert PascalCase enum name to hero_lowercase format
			var name = "hero_" + System.Text.RegularExpressions.Regex.Replace(
				value.ToString(), "(?<!^)([A-Z])", "_$1").ToLowerInvariant();
			_toName[value] = name;
			_fromName[name] = value;
		}
	}

	/// <summary>Converts a <see cref="Heroes"/> value to its internal hero name string (e.g. "hero_inferno").</summary>
	public static string ToHeroName(this Heroes hero) => _toName[hero];

	/// <summary>Tries to parse a hero name string (e.g. "hero_inferno") back to a <see cref="Heroes"/> enum value.</summary>
	public static bool TryParse(string heroName, out Heroes hero) => _fromName.TryGetValue(heroName, out hero);

	/// <summary>Get the native CitadelHeroData_t VData for this hero type. Returns null if not found.</summary>
	public static unsafe CitadelHeroData? GetHeroData(this Heroes hero) {
		var name = hero.ToHeroName();
		Span<byte> utf8 = Utf8.Encode(name, stackalloc byte[Utf8.Size(name)]);
		fixed (byte* ptr = utf8) {
			var result = NativeInterop.GetHeroData(ptr);
			return result == null ? null : new CitadelHeroData((nint)result);
		}
	}
}
