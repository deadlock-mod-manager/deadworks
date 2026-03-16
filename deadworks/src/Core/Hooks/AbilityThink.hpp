#pragma once

#include <safetyhook.hpp>

namespace deadworks {
namespace hooks {

inline safetyhook::InlineHook g_AbilityThink;
void __fastcall Hook_AbilityThink(void *pPawn);

} // namespace hooks
} // namespace deadworks
