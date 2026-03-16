#include "BuildGameSessionManifest.hpp"
#include "../Deadworks.hpp"
#include "../NativeHero.hpp"

__int64 __fastcall deadworks::hooks::Hook_BuildGameSessionManifest(void *thisptr, void **a2) {
    auto result = hooks::g_BuildGameSessionManifest.call<__int64>(thisptr, a2);

    if (!IsHeroPrecacheResolved())
        ResolveHeroPrecacheFns();

    // Build resource context for hero precaching: {int128 zero, manifest_ptr}
    struct { __int64 a; __int64 b; void *manifest; } resourceCtx = { 0, 0, *a2 };
    g_pCurrentResourceCtx = &resourceCtx;

    // Let managed plugins precache resources (including heroes)
    void *manifest = *a2;
    g_Deadworks.OnBuildGameSessionManifest(manifest);

    g_pCurrentResourceCtx = nullptr;
    return result;
}
