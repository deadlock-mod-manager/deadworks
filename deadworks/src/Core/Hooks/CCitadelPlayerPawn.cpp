#include "CCitadelPlayerPawn.hpp"

#include "../Deadworks.hpp"

namespace deadworks {
namespace hooks {

void __fastcall Hook_CCitadelPlayerPawn_ModifyCurrency(CCitadelPlayerPawn *thisptr, ECurrencyType nCurrencyType, int32_t nAmount,
                                                        ECurrencySource nSource, bool bSilent, bool bForceGain, bool bSpendOnly,
                                                        void *pSourceAbility, void *pSourceEntity) {
    if (g_Deadworks.OnPre_CCitadelPlayerPawn_ModifyCurrency(thisptr, nCurrencyType, nAmount, nSource, bSilent, bForceGain, bSpendOnly, pSourceAbility, pSourceEntity))
        return;

    g_CCitadelPlayerPawn_ModifyCurrency.thiscall<void>(thisptr, nCurrencyType, nAmount, nSource, bSilent, bForceGain, bSpendOnly, pSourceAbility, pSourceEntity);
}

} // namespace hooks
} // namespace deadworks
