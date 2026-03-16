#pragma once

#include "NativeCallbacks.hpp"

namespace deadworks {

void PopulateHeroNatives(NativeCallbacks &cb);

// Resolve hero-related statics (called from ResolveNativeStatics)
void ResolveHeroStatics();

// Hero precache resolution (called from BuildGameSessionManifest hook)
void ResolveHeroPrecacheFns();
bool IsHeroPrecacheResolved();

// Set/clear the resource context during BuildGameSessionManifest hook
extern void *g_pCurrentResourceCtx;
extern void *g_pCurrentManifest;

} // namespace deadworks
