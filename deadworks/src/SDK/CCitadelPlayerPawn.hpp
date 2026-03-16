#pragma once

#include "Schema/Schema.hpp"
#include "CBasePlayerPawn.hpp"
#include "Enums.hpp"

#include "../Memory/MemoryDataLoader.hpp"

class CCitadelPlayerPawn : public CBasePlayerPawn {
public:
    DECLARE_SCHEMA_CLASS(CCitadelPlayerPawn);

    void ModifyCurrency(ECurrencyType nCurrencyType, int32_t nAmount, ECurrencySource nSource, bool bSilent, bool bForceGain, bool bSpendOnly, void *pSourceAbility, void *pSourceEntity) {
        static const auto offset = deadworks::MemoryDataLoader::Get().GetOffset("CCitadelPlayerPawn::ModifyCurrency").value();
        using ModifyCurrencyFn = void(__thiscall *)(CCitadelPlayerPawn *, ECurrencyType, int32_t, ECurrencySource, bool, bool, bool, void *, void *);
        reinterpret_cast<ModifyCurrencyFn>(offset)(this, nCurrencyType, nAmount, nSource, bSilent, bForceGain, bSpendOnly, pSourceAbility, pSourceEntity);
    }
};
