#include "NetworkServerService.hpp"

#include "../Deadworks.hpp"

namespace deadworks {
namespace hooks {

void NetworkServerServiceHook::Hook_StartupServer(const GameSessionConfiguration_t &config, ISource2WorldSession *pWorldSession, const char *pszMapName) {
    g_NetworkServerService_StartupServer.thiscall<void>(this, config, pWorldSession, pszMapName);
    g_Deadworks.On_StartupServer(pszMapName);
}

} // namespace hooks
} // namespace deadworks
