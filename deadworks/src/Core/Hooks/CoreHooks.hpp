#pragma once

#include <safetyhook.hpp>

#include <set>
#include <string>

class CAppSystemDict;

namespace deadworks {
namespace hooks {

inline safetyhook::InlineHook g_OnAppSystemLoaded;
inline std::set<std::string> g_AppSystemHookLoadList;
void Hook_OnAppSystemLoaded(CAppSystemDict *pThis);

inline safetyhook::InlineHook g_ServerCreateInterface;
void *Hook_ServerCreateInterface(const char *pszName, int *pReturnCode);

inline safetyhook::InlineHook g_CGCClientSystem_OnServerVersionCheck;
bool Hook_CGCClientSystem_OnServerVersionCheck(void *a1, int a2);

} // namespace hooks
} // namespace deadworks