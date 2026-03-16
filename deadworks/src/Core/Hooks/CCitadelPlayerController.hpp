#pragma once

#include <safetyhook.hpp>

namespace deadworks {
namespace hooks {

inline safetyhook::InlineHook g_CCitadelPlayerController_ClientConCommand;
char __fastcall Hook_CCitadelPlayerController_ClientConCommand(void *thisptr, void *args);

} // namespace hooks
} // namespace deadworks
