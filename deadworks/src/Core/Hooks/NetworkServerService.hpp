#pragma once

#include <safetyhook.hpp>

#include <iserver.h>

namespace deadworks {
namespace hooks {

inline safetyhook::VmtHook g_NetworkServerServiceVmt;
inline safetyhook::VmHook g_NetworkServerService_StartupServer;

class NetworkServerServiceHook : public INetworkServerService {
public:
    void Hook_StartupServer(const GameSessionConfiguration_t &config, ISource2WorldSession *pWorldSession, const char *pszMapName);
};

} // namespace hooks
} // namespace deadworks
