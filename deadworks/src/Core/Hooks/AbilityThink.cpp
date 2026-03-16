#include "AbilityThink.hpp"

#include "../Deadworks.hpp"

namespace deadworks {
namespace hooks {

// Offsets into game structures (from IDA analysis)
static constexpr size_t kPawn_hController = 0x3DC;       // CBasePlayerPawn::m_hController (CHandle)
static constexpr size_t kPawn_pMovementServices = 0xBD0; // CBasePlayerPawn::m_pMovementServices
static constexpr size_t kMoveSvc_nButtons = 0x50;         // CPlayer_MovementServices::m_nButtons (CInButtonState)
static constexpr size_t kButtonState_States = 0x08;       // CInButtonState::m_pButtonStates[3] (3x uint64)

void __fastcall Hook_AbilityThink(void *pPawn) {
    auto *pawn = reinterpret_cast<char *>(pPawn);

    // Only process player pawns (they have a valid controller handle)
    int hController = *reinterpret_cast<int *>(pawn + kPawn_hController);
    if (hController != -1 && hController != -2) {
        int slot = (hController & 0x7FFF) - 1;
        if (slot >= 0 && slot < 64) {
            auto *pMoveSvc = *reinterpret_cast<char **>(pawn + kPawn_pMovementServices);
            if (pMoveSvc) {
                auto *buttonStates = reinterpret_cast<uint64_t *>(pMoveSvc + kMoveSvc_nButtons + kButtonState_States);
                // buttonStates[0] = held/current, [1] = changed, [2] = scroll

                uint64_t forcedBits = 0;
                uint64_t blockedBits = g_Deadworks.OnPre_AbilityThink(
                    slot, pPawn,
                    buttonStates[0], buttonStates[1], buttonStates[2],
                    &forcedBits);

                if (blockedBits) {
                    buttonStates[0] &= ~blockedBits;
                    buttonStates[1] &= ~blockedBits;
                    buttonStates[2] &= ~blockedBits;
                }
                if (forcedBits) {
                    buttonStates[0] |= forcedBits;
                    buttonStates[1] |= forcedBits;
                }
            }
        }
    }

    g_AbilityThink.call(pPawn);
}

} // namespace hooks
} // namespace deadworks
