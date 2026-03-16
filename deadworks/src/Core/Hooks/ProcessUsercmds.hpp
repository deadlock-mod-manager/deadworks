#pragma once

#include <safetyhook.hpp>

namespace deadworks {
namespace hooks {

inline safetyhook::InlineHook g_ProcessUsercmds;
void *__fastcall Hook_ProcessUsercmds(void *pController, void *cmds, int numcmds, unsigned char paused, float margin);

} // namespace hooks
} // namespace deadworks
