#pragma once

#include <safetyhook.hpp>

#include <eiface.h>

namespace deadworks {
namespace hooks {

inline safetyhook::VmtHook g_Source2ServerVmt;
inline safetyhook::VmHook g_Source2Server_ApplyGameSettings;
inline safetyhook::VmHook g_Source2Server_GameFrame;

class Source2ServerHook : public ISource2Server {
public:
    void Hook_ApplyGameSettings(void *pKV);
    void Hook_GameFrame(bool simulating, bool bFirstTick, bool bLastTick);
};

} // namespace hooks
} // namespace deadworks
