#include "Source2Server.hpp"

#include "../Deadworks.hpp"

namespace deadworks {
namespace hooks {

void Source2ServerHook::Hook_ApplyGameSettings(void *pKV) {
    g_Source2Server_ApplyGameSettings.thiscall<void>(this, pKV);
    g_Deadworks.On_ISource2Server_ApplyGameSettings();
}

void Source2ServerHook::Hook_GameFrame(bool simulating, bool bFirstTick, bool bLastTick) {
    g_Source2Server_GameFrame.thiscall<void>(this, simulating, bFirstTick, bLastTick);
    g_Deadworks.On_ISource2Server_GameFrame(simulating, bFirstTick, bLastTick);
}

} // namespace hooks
} // namespace deadworks
