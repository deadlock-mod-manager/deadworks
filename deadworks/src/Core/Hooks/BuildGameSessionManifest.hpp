#pragma once

#include <safetyhook.hpp>

namespace deadworks {
namespace hooks {

inline safetyhook::InlineHook g_BuildGameSessionManifest;
__int64 __fastcall Hook_BuildGameSessionManifest(void *thisptr, void **a2);

} // namespace hooks
} // namespace deadworks
