#pragma once

#include "NativeCallbacks.hpp"

namespace deadworks {

void PopulateDamageNatives(NativeCallbacks &cb);

// Resolve CTakeDamageInfo ctor/dtor (called from ResolveNativeStatics)
void ResolveDamageStatics();

} // namespace deadworks
