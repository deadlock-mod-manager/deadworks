#include "CBaseEntity.hpp"

#include "../Deadworks.hpp"

namespace deadworks {
namespace hooks {

void __fastcall Hook_CBaseEntity_TakeDamageOld(CBaseEntity *thisptr, CTakeDamageInfo *info, CTakeDamageResult *result) {
    if (g_Deadworks.OnPre_CBaseEntity_TakeDamageOld(thisptr, info, result))
        return;

    g_CBaseEntity_TakeDamageOld.thiscall<void>(thisptr, info, result);
}

void __fastcall Hook_CBaseEntity_StartTouch(CBaseEntity *thisptr, CBaseEntity *other) {
    g_Deadworks.OnStartTouch(thisptr, other);
    g_CBaseEntity_StartTouch.thiscall<void>(thisptr, other);
}

void __fastcall Hook_CBaseEntity_EndTouch(CBaseEntity *thisptr, CBaseEntity *other) {
    g_Deadworks.OnEndTouch(thisptr, other);
    g_CBaseEntity_EndTouch.thiscall<void>(thisptr, other);
}

} // namespace hooks
} // namespace deadworks
