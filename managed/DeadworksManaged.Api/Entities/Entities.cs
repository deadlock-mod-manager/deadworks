namespace DeadworksManaged.Api;

/// <summary>
/// Provides enumeration over all server entities.
/// Uses the existing GetEntityByIndex native callback.
/// </summary>
public static class Entities {
	// Source 2: MAX_ENTITY_LISTS (64) * MAX_ENTITIES_IN_LIST (512) = 32768
	private const int MaxTotalEntities = 32768;

	/// <summary>Enumerates all valid entities on the server.</summary>
	public static IEnumerable<CBaseEntity> All {
		get {
			var list = new List<CBaseEntity>();
			for (int i = 0; i < MaxTotalEntities; i++) {
				nint ptr;
				unsafe { ptr = (nint)NativeInterop.GetEntityByIndex(i); }
				if (ptr != nint.Zero)
					list.Add(new CBaseEntity(ptr));
			}
			return list;
		}
	}

	/// <summary>Enumerates all entities whose native DLL class matches <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">A <see cref="CBaseEntity"/>-derived type corresponding to a native entity class.</typeparam>
	/// <returns>All live entities that pass an <c>Is&lt;T&gt;</c> check, wrapped as <typeparamref name="T"/>.</returns>
	public static IEnumerable<T> ByClass<T>() where T : CBaseEntity {
		var list = new List<T>();
		for (int i = 0; i < MaxTotalEntities; i++) {
			nint ptr;
			unsafe { ptr = (nint)NativeInterop.GetEntityByIndex(i); }
			if (ptr == nint.Zero) continue;
			var entity = new CBaseEntity(ptr);
			if (entity.Is<T>())
				list.Add(NativeEntityFactory.Create<T>(ptr));
		}
		return list;
	}

	/// <summary>Enumerates all entities with the given designer name (e.g. <c>"trigger_multiple"</c>, <c>"npc_boss_tier3"</c>).</summary>
	/// <param name="name">The exact designer name to match, compared ordinally.</param>
	/// <returns>All live entities whose <c>DesignerName</c> equals <paramref name="name"/>.</returns>
	public static IEnumerable<CBaseEntity> ByDesignerName(string name) {
		var list = new List<CBaseEntity>();
		for (int i = 0; i < MaxTotalEntities; i++) {
			nint ptr;
			unsafe { ptr = (nint)NativeInterop.GetEntityByIndex(i); }
			if (ptr == nint.Zero) continue;
			var entity = new CBaseEntity(ptr);
			if (string.Equals(entity.DesignerName, name, System.StringComparison.Ordinal))
				list.Add(entity);
		}
		return list;
	}
}
