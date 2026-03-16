#pragma once

#include <safetyhook.hpp>
#include <igameevents.h>

namespace deadworks {
namespace hooks {

inline safetyhook::VmtHook g_GameEventManager2Vmt;
inline safetyhook::VmHook g_GameEventManager2_FireEvent;

bool __fastcall Hook_GameEventManager2_FireEvent(IGameEventManager2 *thisptr, IGameEvent *event, bool bDontBroadcast);

} // namespace hooks

extern IGameEventManager2 *g_pGameEventManager2;

class DeadworksGameEventListener : public IGameEventListener2 {
public:
    ~DeadworksGameEventListener() override {}
    void FireGameEvent(IGameEvent *event) override {}
};

inline DeadworksGameEventListener g_DeadworksEventListener;

} // namespace deadworks
