namespace DeadworksManaged.Api;

/// <summary>Internal interface for entity-keyed data stores, allowing cleanup on entity deletion.</summary>
public interface IEntityData {
	void Remove(uint handle);
}

/// <summary>Dictionary-like store that associates arbitrary per-entity data with entities by their handle. Automatically removes entries when an entity is deleted.</summary>
public sealed class EntityData<T> : IEntityData {
	private readonly Dictionary<uint, T> _data = new();

	static EntityData() { }

	public EntityData() {
		EntityDataRegistry.Register(this);
	}

	public T? this[CBaseEntity entity] {
		get => TryGet(entity, out var val) ? val : default;
		set {
			if (value is null)
				_data.Remove(entity.EntityHandle);
			else
				_data[entity.EntityHandle] = value;
		}
	}

	public bool TryGet(CBaseEntity entity, out T value) => _data.TryGetValue(entity.EntityHandle, out value!);

	public T GetOrAdd(CBaseEntity entity, T defaultValue) {
		uint handle = entity.EntityHandle;
		if (!_data.TryGetValue(handle, out var value)) {
			value = defaultValue;
			_data[handle] = value;
		}
		return value;
	}

	public T GetOrAdd(CBaseEntity entity, Func<T> factory) {
		uint handle = entity.EntityHandle;
		if (!_data.TryGetValue(handle, out var value)) {
			value = factory();
			_data[handle] = value;
		}
		return value;
	}

	public bool Has(CBaseEntity entity) => _data.ContainsKey(entity.EntityHandle);

	public void Remove(CBaseEntity entity) => _data.Remove(entity.EntityHandle);

	void IEntityData.Remove(uint handle) => _data.Remove(handle);

	public void Clear() => _data.Clear();
}

/// <summary>Global registry of all active <see cref="EntityData{T}"/> stores. Notifies them when an entity is deleted to purge stale entries.</summary>
public static class EntityDataRegistry {
	private static readonly List<WeakReference<IEntityData>> _stores = new();

	internal static void Register(IEntityData store) {
		lock (_stores) _stores.Add(new WeakReference<IEntityData>(store));
	}

	internal static void OnEntityDeleted(uint handle) {
		lock (_stores) {
			for (int i = _stores.Count - 1; i >= 0; i--) {
				if (_stores[i].TryGetTarget(out var store))
					store.Remove(handle);
				else
					_stores.RemoveAt(i);
			}
		}
	}
}
