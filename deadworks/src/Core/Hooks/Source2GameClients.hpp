#pragma once

#include <safetyhook.hpp>

#include <eiface.h>

namespace deadworks {
namespace hooks {

inline safetyhook::VmtHook g_Source2GameClientsVmt;
inline safetyhook::VmHook g_Source2GameClients_ClientPutInServer;
inline safetyhook::VmHook g_Source2GameClients_ClientConnect;
inline safetyhook::VmHook g_Source2GameClients_ClientDisconnect;

class Source2GameClientsHook : public ISource2GameClients {
public:
    void Hook_ClientPutInServer(CPlayerSlot slot, const char *pszName, int type, uint64 xuid);
    bool Hook_ClientConnect(CPlayerSlot slot, const char *pszName, uint64 xuid, const char *pszNetworkID, bool unk1, CBufferString *pRejectReason);
    void Hook_ClientDisconnect(CPlayerSlot slot, ENetworkDisconnectionReason reason, const char *pszName, uint64 xuid, const char *pszNetworkID);
};

} // namespace hooks
} // namespace deadworks
