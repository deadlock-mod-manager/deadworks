namespace DeadworksManaged.Api;

internal static class NativeEntityFactory {
	private static readonly Dictionary<Type, Func<nint, CBaseEntity>> _constructors = new();
	private static readonly Dictionary<Type, HashSet<string>> _classNames = new();

	public static T Create<T>(nint handle) where T : CBaseEntity {
		if (!_constructors.TryGetValue(typeof(T), out var ctor)) {
			var ci = typeof(T).GetConstructor(
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
				null, [typeof(nint)], null)
				?? throw new InvalidOperationException($"No (nint) constructor on {typeof(T).Name}");
			ctor = h => (CBaseEntity)ci.Invoke([h]);
			_constructors[typeof(T)] = ctor;
		}
		return (T)ctor(handle);
	}

	public static bool IsMatch<T>(string dllClassName) where T : CBaseEntity {
		if (string.IsNullOrEmpty(dllClassName))
			return false;

		if (!_classNames.TryGetValue(typeof(T), out var names)) {
			names = BuildClassNames(typeof(T));
			_classNames[typeof(T)] = names;
		}
		return names.Contains(dllClassName);
	}

	private static HashSet<string> BuildClassNames(Type type) {
		var names = new HashSet<string>(StringComparer.Ordinal);

		// Add from [NativeClass] attribute on T
		var attr = type.GetCustomAttributes(typeof(NativeClassAttribute), false);
		if (attr.Length > 0) {
			foreach (var a in attr)
				foreach (var n in ((NativeClassAttribute)a).ClassNames)
					names.Add(n);
		} else {
			// Default: use the C# class name itself
			names.Add(type.Name);
		}

		// Also accept any subclass that has been loaded
		foreach (var kvp in _classNames) {
			if (type.IsAssignableFrom(kvp.Key) && kvp.Key != type) {
				foreach (var n in kvp.Value)
					names.Add(n);
			}
		}

		return names;
	}
}
