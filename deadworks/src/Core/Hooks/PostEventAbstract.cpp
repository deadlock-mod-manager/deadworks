#include "PostEventAbstract.hpp"

#include "../Deadworks.hpp"

#include <igameeventsystem.h>
#include <networksystem/inetworkserializer.h>
#include <networksystem/netmessage.h>

namespace deadworks {
namespace hooks {

void __fastcall Hook_PostEventAbstract(IGameEventSystem *thisptr, CSplitScreenSlot nSlot,
    bool bLocalOnly, int nClientCount, const uint64 *clients,
    INetworkMessageInternal *pEvent, const CNetMessage *pData,
    unsigned long nSize, NetChannelBufType_t bufType)
{
    if (pEvent && pData && clients) {
        auto *info = pEvent->GetNetMessageInfo();
        if (info) {
            int msgId = info->m_MessageId;
            uint64 clientsMask = *clients;
            if (g_Deadworks.OnPre_PostEventAbstract(msgId, pData, &clientsMask)) {
                return; // blocked
            }
            // Allow modified recipient mask
            if (clientsMask != *clients) {
                g_PostEventAbstract.thiscall<void>(thisptr, nSlot, bLocalOnly, nClientCount,
                    &clientsMask, pEvent, pData, nSize, bufType);
                return;
            }
        }
    }

    g_PostEventAbstract.thiscall<void>(thisptr, nSlot, bLocalOnly, nClientCount,
        clients, pEvent, pData, nSize, bufType);
}

} // namespace hooks
} // namespace deadworks
