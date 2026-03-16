#pragma once

#include <ehandle.h>

#include "Schema/Schema.hpp"
#include "CBaseEntity.hpp"
#include "CBasePlayerPawn.hpp"

#include "../Memory/MemoryDataLoader.hpp"

class CBasePlayerController : public CBaseEntity {
    DECLARE_SCHEMA_CLASS(CBasePlayerController);
    SCHEMA_FIELD(CHandle<CBasePlayerPawn>, m_hPawn);

    void SetPawn(CBasePlayerPawn *pPawn, bool bRetainOldPawnTeam, bool bCopyMovementState, bool bAllowTeamMismatch, bool bPreserveMovementState) {
        static const auto offset = deadworks::MemoryDataLoader::Get().GetOffset("CBasePlayerController::SetPawn").value();
        using SetPawnFn = void(__thiscall *)(CBasePlayerController *, CBasePlayerPawn *, bool, bool, bool, bool);
        return reinterpret_cast<SetPawnFn>(offset)(this, pPawn, bRetainOldPawnTeam, bCopyMovementState, bAllowTeamMismatch, bPreserveMovementState);
    }
};
