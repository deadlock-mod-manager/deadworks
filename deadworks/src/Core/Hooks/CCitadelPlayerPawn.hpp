#pragma once

#include <safetyhook.hpp>

#include "../../SDK/Enums.hpp"

class CCitadelPlayerPawn;

namespace deadworks {
namespace hooks {

inline safetyhook::InlineHook g_CCitadelPlayerPawn_ModifyCurrency;
void __fastcall Hook_CCitadelPlayerPawn_ModifyCurrency(CCitadelPlayerPawn *thisptr, ECurrencyType nCurrencyType, int32_t nAmount,
                                                        ECurrencySource nSource, bool bSilent, bool bForceGain, bool bSpendOnly,
                                                        void *pSourceAbility, void *pSourceEntity);

} // namespace hooks
} // namespace deadworks
