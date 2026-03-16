using DeadworksManaged.Api;

namespace ExampleCheckpoint;

public struct SavedLocation
{
    public float PosX, PosY, PosZ;
    public float AngX, AngY, AngZ;
    public float VelX, VelY, VelZ;
}

public class ExampleCheckpoint : IDeadworksPlugin
{
    public string Name => "ExampleCheckpoint";

    // Per-player saved locations: slot -> list of checkpoints
    private readonly Dictionary<int, List<SavedLocation>> _savedLocations = new();
    // Per-player current checkpoint index
    private readonly Dictionary<int, int> _currentIndex = new();

    public void OnLoad(bool isReload) => Console.WriteLine(isReload ? "ExampleCheckpoint reloaded!" : "ExampleCheckpoint loaded!");

    public void OnUnload()
    {
        _savedLocations.Clear();
        _currentIndex.Clear();
        Console.WriteLine("ExampleCheckpoint unloaded!");
    }

    [Command("cp")]
    [Command("save")]
    [Command("saveloc")]
    public void OnSaveLocation(ICommandContext ctx)
    {
        var pawn = ctx.Controller.GetHeroPawn();
        if (pawn == null) return;

        var loc = new SavedLocation();
        pawn.GetAbsOrigin(out loc.PosX, out loc.PosY, out loc.PosZ);
        pawn.GetEyeAngles(out loc.AngX, out loc.AngY, out loc.AngZ);
        pawn.GetAbsVelocity(out loc.VelX, out loc.VelY, out loc.VelZ);

        if (!_savedLocations.TryGetValue(ctx.Slot, out var locations))
        {
            locations = new List<SavedLocation>();
            _savedLocations[ctx.Slot] = locations;
        }

        if (locations.Count >= 100)
        {
            locations.RemoveAt(0);
            ctx.Controller.PrintToChat("Saveloc limit reached, removed oldest");
        }

        locations.Add(loc);
        _currentIndex[ctx.Slot] = locations.Count - 1;
        ctx.Controller.PrintToChat($"Saved location #{locations.Count}");
    }

    [Command("tele")]
    [Command("tp")]
    public void OnTeleport(ICommandContext ctx)
    {
        TeleportToSavedLocation(ctx);
    }

    [Command("prevcp")]
    [Command("prevloc")]
    public void OnPrevLocation(ICommandContext ctx)
    {
        if (!_savedLocations.TryGetValue(ctx.Slot, out var locations) || locations.Count == 0)
        {
            ctx.Controller.PrintToChat("No saved locations");
            return;
        }

        var index = _currentIndex.GetValueOrDefault(ctx.Slot, 0);
        index--;
        if (index < 0) index = locations.Count - 1;
        _currentIndex[ctx.Slot] = index;

        TeleportToSavedLocation(ctx);
    }

    [Command("nextcp")]
    [Command("nextloc")]
    public void OnNextLocation(ICommandContext ctx)
    {
        if (!_savedLocations.TryGetValue(ctx.Slot, out var locations) || locations.Count == 0)
        {
            ctx.Controller.PrintToChat("No saved locations");
            return;
        }

        var index = _currentIndex.GetValueOrDefault(ctx.Slot, 0);
        index = (index + 1) % locations.Count;
        _currentIndex[ctx.Slot] = index;

        TeleportToSavedLocation(ctx);
    }

    [Command("clearcp")]
    public void OnClearLocations(ICommandContext ctx)
    {
        _savedLocations.Remove(ctx.Slot);
        _currentIndex.Remove(ctx.Slot);
        ctx.Controller.PrintToChat("Cleared all saved locations");
    }

    private void TeleportToSavedLocation(ICommandContext ctx)
    {
        var pawn = ctx.Controller.GetHeroPawn();
        if (pawn == null) return;

        if (!_savedLocations.TryGetValue(ctx.Slot, out var locations) || locations.Count == 0)
        {
            ctx.Controller.PrintToChat("No saved locations");
            return;
        }

        var index = _currentIndex.GetValueOrDefault(ctx.Slot, 0);
        var loc = locations[index];

        pawn.Teleport(loc.PosX, loc.PosY, loc.PosZ,
                      loc.AngX, loc.AngY, loc.AngZ,
                      loc.VelX, loc.VelY, loc.VelZ);

        ctx.Controller.PrintToChat($"Teleported to saveloc #{index + 1}");
    }
}
