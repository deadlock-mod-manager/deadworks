#pragma once

#include <safetyhook.hpp>

#include <networksystem/inetworkserializer.h>
#include <networksystem/netmessage.h>
#include <igameeventsystem.h>

namespace deadworks {
namespace hooks {

inline safetyhook::VmtHook g_GameEventSystemVmt;
inline safetyhook::VmHook g_PostEventAbstract;

void __fastcall Hook_PostEventAbstract(IGameEventSystem *thisptr, CSplitScreenSlot nSlot,
    bool bLocalOnly, int nClientCount, const uint64 *clients,
    INetworkMessageInternal *pEvent, const CNetMessage *pData,
    unsigned long nSize, NetChannelBufType_t bufType);

} // namespace hooks
} // namespace deadworks
