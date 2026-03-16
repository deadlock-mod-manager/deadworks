#include "GameEvents.hpp"

#include "../Deadworks.hpp"

#include <igameevents.h>

namespace deadworks {

IGameEventManager2 *g_pGameEventManager2 = nullptr;

namespace hooks {

bool __fastcall Hook_GameEventManager2_FireEvent(IGameEventManager2 *thisptr, IGameEvent *event, bool bDontBroadcast) {
    if (!event)
        return g_GameEventManager2_FireEvent.thiscall<bool>(thisptr, event, bDontBroadcast);

    const char *name = event->GetName();

    int result = g_Deadworks.OnPre_GameEvent(name, event);
    if (result == 1) {
        g_pGameEventManager2->FreeEvent(event);
        return false;
    }

    return g_GameEventManager2_FireEvent.thiscall<bool>(thisptr, event, bDontBroadcast);
}

} // namespace hooks
} // namespace deadworks
