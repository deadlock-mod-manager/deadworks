#pragma once

#include <cstdint>

namespace deadworks::offsets {

// --- Struct / field offsets ---

// Ability entity → slot index field
constexpr uintptr_t kAbilitySlotField = 0x630;

// CCitadelAbilityComponent → slot table
constexpr uintptr_t kAbilityCompSlotTable = 0x30;

// CModifierProperty → m_bPredictedOwner
constexpr uintptr_t kModifierPropPredictedOwner = 0x1CE;

// Entity → ability handle vector (CCitadelAbilityComponent offset + m_vecAbilities data)
constexpr uintptr_t kEntityAbilityVector = 0x10F0 + 0x80;

// CTakeDamageInfo struct size (for stack/heap allocation)
constexpr size_t kCTakeDamageInfoSize = 0x100;

// Hero data: name pointer offset within hero definition
constexpr uintptr_t kHeroDefNamePtr = 32;

// Subclass definition: type byte and disabled flag
constexpr uintptr_t kSubclassDefType = 40;
constexpr uintptr_t kSubclassDefDisabled = 42;

// --- Vtable indices ---

// CBaseEntity::Teleport
constexpr int kVtblTeleport = 163;

// CBasePlayerController::ChangeTeam (0x338 / 8)
constexpr int kVtblChangeTeam = 0x338 / 8;

// --- Instruction offsets for E8 call resolution ---
// (byte offset from start of a known function to a CALL instruction)

// CCitadelAbilityComponent::OnAbilityRemoved
constexpr uintptr_t kOnAbilityRemoved_FindSlotCall = 0x7D;
constexpr uintptr_t kOnAbilityRemoved_RemoveSlotCall = 0x8D;

// CCitadelAbilityComponent::CreateAbilityByName
constexpr uintptr_t kCreateAbility_LookupDefCall = 0x1E;
constexpr uintptr_t kCreateAbility_RegisterCall = 0x48;

// SelectHeroInternal
constexpr uintptr_t kSelectHero_GetManagerCall = 0x15;
constexpr uintptr_t kSelectHero_NameToIdCall = 0x2D;

// CCitadelGameRules::BuildGameSessionManifest
constexpr uintptr_t kBGSM_GetHeroTableCall = 0x306;
constexpr uintptr_t kBGSM_PrecacheGlobalLea = 0x33B;
constexpr uintptr_t kBGSM_PrecacheCall = 0x342;

} // namespace deadworks::offsets

namespace deadworks {

// Resolve an x86-64 E8 relative CALL instruction at `callAddr` to an absolute target.
inline uintptr_t ResolveE8Call(uintptr_t callAddr) {
    int32_t rel = *reinterpret_cast<int32_t *>(callAddr + 1);
    return callAddr + 5 + rel;
}

// Resolve a 7-byte LEA instruction (REX.W + 8D + ModRM + disp32) at `leaAddr`
// to the absolute address of the referenced data.
inline uintptr_t ResolveLea(uintptr_t leaAddr) {
    int32_t rel = *reinterpret_cast<int32_t *>(leaAddr + 3);
    return leaAddr + 7 + rel;
}

} // namespace deadworks
