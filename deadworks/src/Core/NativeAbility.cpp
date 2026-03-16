#include "NativeAbility.hpp"
#include "NativeOffsets.hpp"
#include "Deadworks.hpp"

#include "../Memory/MemoryDataLoader.hpp"
#include "../SDK/CBaseEntity.hpp"
#include "../SDK/CEntitySystem.hpp"
#include "../SDK/Core.hpp"
#include "../SDK/Util.hpp"

#include <tier1/keyvalues3.h>
#include <tier1/utlvector.h>

using namespace deadworks;
using namespace deadworks::offsets;

// --- Ability system types ---

using FindAbilityByNameFn = void *(__fastcall *)(void *comp, const char *name);
using FindSlotEntryFn = int(__fastcall *)(void *slotTable, uint16_t *slot);
using RemoveSlotEntryFn = void(__fastcall *)(void *slotTable, int entryIndex);
using LookupSubclassDefFn = void *(__fastcall *)(int typeIndex, const char *name);
using CreateAndRegisterAbilityFn = void *(__fastcall *)(void *comp, void *def, uint16_t slot, int flags, int upgradeLevel, int arg6);

// --- Ability execution types (resolved from IDA) ---
using ExecuteAbilityBySlotFn = int(__fastcall *)(void *comp, int16_t slot, char altCast, uint8_t flags);
using ExecuteAbilityByIDFn = int(__fastcall *)(void *comp, int abilityID, char altCast, uint8_t flags);
using ExecuteAbilityFn = int(__fastcall *)(void *comp, void *ability, char altCast, uint8_t flags);
using GetAbilityBySlotFn = void *(__fastcall *)(void *comp, int16_t slot);
using ToggleActivateFn = void(__fastcall *)(void *ability, char activate);

// --- Item system types ---

using AddItemFn = void *(__fastcall *)(void *pawn, const char *itemName, int arg3, int upgradeType);
using SellItemFn = uint8_t(__fastcall *)(void *pawn, const char *itemName, uint8_t bFullRefund, uint8_t bForceSellPrice);

// --- Modifier system types ---

enum class EntitySubclassScope_t : uint32_t {
    SUBCLASS_SCOPE_NONE = 0xFFFFFFFF,
    SUBCLASS_SCOPE_MISC = 0x0,
    SUBCLASS_SCOPE_PRECIPITATION = 0x1,
    SUBCLASS_SCOPE_MODIFIERS = 0x2,
    SUBCLASS_SCOPE_NPC_UNITS = 0x3,
    SUBCLASS_SCOPE_ABILITIES = 0x4,
    SUBCLASS_SCOPE_SCALE_FUNCTIONS = 0x5,
    SUBCLASS_SCOPE_LOOT_TABLES = 0x6,
    SUBCLASS_SCOPE_COUNT = 0x7,
};

namespace {
struct ModifierVDataBase {};
struct CModifierVData : ModifierVDataBase {};
} // namespace

using AddModifierFn = void *(__thiscall *)(void *modifierProp, CBaseEntity *pCaster, uint32_t hAbility, int iTeam, CModifierVData *vdata, KeyValues3 *pModifierParams, KeyValues3 *pKeyValues);

// ---------------------------------------------------------------------------
// Resolved wrappers
// ---------------------------------------------------------------------------

static void *LookupSubclassDefinitionByName(EntitySubclassScope_t scope, const char *name) {
    static const auto fn = reinterpret_cast<LookupSubclassDefFn>(
        MemoryDataLoader::Get().GetOffset("GetVDataInstanceByName").value());
    return fn(static_cast<int>(scope), name);
}

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

static void ResolveSlotTableFns(FindSlotEntryFn &findFn, RemoveSlotEntryFn &removeFn) {
    auto addr = MemoryDataLoader::Get().GetOffset("CCitadelAbilityComponent::OnAbilityRemoved").value();
    findFn = reinterpret_cast<FindSlotEntryFn>(ResolveE8Call(addr + kOnAbilityRemoved_FindSlotCall));
    removeFn = reinterpret_cast<RemoveSlotEntryFn>(ResolveE8Call(addr + kOnAbilityRemoved_RemoveSlotCall));
    g_Log->Info("SlotTable: FindEntry={} RemoveEntry={}", (void *)findFn, (void *)removeFn);
}

// ---------------------------------------------------------------------------
// Native implementations
// ---------------------------------------------------------------------------

static uint8_t __cdecl NativeRemoveAbility(void *pawn, const char *abilityName) {
    if (!pawn || !abilityName)
        return 0;

    static auto compKey = schema::GetOffset("CCitadelPlayerPawn",
        hash_32_fnv1a_const("CCitadelPlayerPawn"),
        "m_CCitadelAbilityComponent",
        hash_32_fnv1a_const("m_CCitadelAbilityComponent"));
    auto compAddr = reinterpret_cast<uintptr_t>(pawn) + compKey.Offset;

    static auto findFn = reinterpret_cast<FindAbilityByNameFn>(
        MemoryDataLoader::Get().GetOffset("CCitadelAbilityComponent::FindAbilityByName").value());
    auto *ability = findFn(reinterpret_cast<void *>(compAddr), abilityName);
    if (!ability) {
        g_Log->Info("RemoveAbility: FindAbilityByName('{}') returned null", abilityName);
        return 0;
    }

    uint32_t rawHandle = static_cast<uint32_t>(
        static_cast<CBaseEntity *>(ability)->GetRefEHandle().ToInt());
    g_Log->Info("RemoveAbility: found '{}' handle=0x{:X}", abilityName, rawHandle);

    static FindSlotEntryFn findSlotFn = nullptr;
    static RemoveSlotEntryFn removeSlotFn = nullptr;
    if (!findSlotFn)
        ResolveSlotTableFns(findSlotFn, removeSlotFn);
    uint16_t slot = *reinterpret_cast<uint16_t *>(reinterpret_cast<uintptr_t>(ability) + kAbilitySlotField);
    auto *slotTable = reinterpret_cast<void *>(compAddr + kAbilityCompSlotTable);
    int entryIdx = findSlotFn(slotTable, &slot);
    g_Log->Info("RemoveAbility: slot={} entry={}", slot, entryIdx);
    if (entryIdx != -1)
        removeSlotFn(slotTable, entryIdx);

    static auto vecAbilitiesKey = schema::GetOffset("CCitadelAbilityComponent",
        hash_32_fnv1a_const("CCitadelAbilityComponent"),
        "m_vecAbilities",
        hash_32_fnv1a_const("m_vecAbilities"));
    static auto vecThinkableKey = schema::GetOffset("CCitadelAbilityComponent",
        hash_32_fnv1a_const("CCitadelAbilityComponent"),
        "m_vecThinkableAbilities",
        hash_32_fnv1a_const("m_vecThinkableAbilities"));

    auto &vecAbilities = *reinterpret_cast<CUtlVectorBase<uint32_t> *>(compAddr + vecAbilitiesKey.Offset);
    for (int i = 0; i < vecAbilities.Count(); i++) {
        if (vecAbilities[i] == rawHandle) {
            vecAbilities.Remove(i);
            break;
        }
    }

    auto &vecThinkable = *reinterpret_cast<CUtlVectorBase<uint32_t> *>(compAddr + vecThinkableKey.Offset);
    for (int i = 0; i < vecThinkable.Count(); i++) {
        if (vecThinkable[i] == rawHandle) {
            vecThinkable.Remove(i);
            break;
        }
    }

    static auto chainOffset = schema::FindChainOffset("CCitadelAbilityComponent",
        hash_32_fnv1a_const("CCitadelAbilityComponent"));
    if (chainOffset != 0) {
        ChainNetworkStateChanged(compAddr + chainOffset, vecAbilitiesKey.Offset);
        ChainNetworkStateChanged(compAddr + chainOffset, vecThinkableKey.Offset);
    }

    UTIL_Remove(static_cast<CEntityInstance *>(ability));
    g_Log->Info("RemoveAbility: done for '{}'", abilityName);
    return 1;
}

static void *__cdecl NativeAddAbility(void *pawn, const char *abilityName, uint16_t slot) {
    if (!pawn || !abilityName)
        return nullptr;

    static auto compKey = schema::GetOffset("CCitadelPlayerPawn",
        hash_32_fnv1a_const("CCitadelPlayerPawn"),
        "m_CCitadelAbilityComponent",
        hash_32_fnv1a_const("m_CCitadelAbilityComponent"));
    auto compAddr = reinterpret_cast<uintptr_t>(pawn) + compKey.Offset;

    static const auto registerFn = reinterpret_cast<CreateAndRegisterAbilityFn>(
        MemoryDataLoader::Get().GetOffset("CCitadelAbilityComponent::CreateAndRegisterAbility").value());

    g_Log->Info("AddAbility: name='{}' slot={} comp={}", abilityName, slot, (void *)compAddr);

    auto *def = LookupSubclassDefinitionByName(EntitySubclassScope_t::SUBCLASS_SCOPE_ABILITIES, abilityName);
    if (!def) {
        g_Log->Info("AddAbility: LookupSubclassDefinitionByName returned null for '{}'", abilityName);
        return nullptr;
    }
    if (*reinterpret_cast<uint8_t *>(reinterpret_cast<uintptr_t>(def) + kSubclassDefDisabled)) {
        g_Log->Info("AddAbility: ability '{}' is disabled (def+{} != 0)", abilityName, kSubclassDefDisabled);
        return nullptr;
    }
    g_Log->Info("AddAbility: def={} type={}", def, *reinterpret_cast<uint8_t *>(reinterpret_cast<uintptr_t>(def) + kSubclassDefType));

    auto *result = registerFn(reinterpret_cast<void *>(compAddr), def, slot, 0, -1, 1);
    g_Log->Info("AddAbility: CreateAndRegister result={}", result);
    return result;
}

// upgradeTier: pass -1 for base item, or a specific tier (0-based) for upgraded versions.
// The tier maps to the item's bucket_id which determines which upgrade level to create.
static void *__cdecl NativeAddItem(void *pawn, const char *itemName, int32_t upgradeTier) {
    if (!pawn || !itemName)
        return nullptr;

    static AddItemFn addItemFn = nullptr;
    if (!addItemFn) {
        auto opt = MemoryDataLoader::Get().GetOffset("CCitadelPlayerPawn::AddItem");
        if (!opt) {
            g_Log->Error("AddItem: failed to resolve CCitadelPlayerPawn::AddItem signature");
            return nullptr;
        }
        addItemFn = reinterpret_cast<AddItemFn>(opt.value());
        g_Log->Info("AddItem: resolved at {:p}", reinterpret_cast<void *>(addItemFn));
    }

    g_Log->Info("AddItem: pawn={} item='{}' upgradeTier={}", pawn, itemName, upgradeTier);
    auto *result = addItemFn(pawn, itemName, 0, upgradeTier);
    g_Log->Info("AddItem: result={}", result);
    return result;
}

static uint8_t __cdecl NativeSellItem(void *pawn, const char *itemName, uint8_t bFullRefund, uint8_t bForceSellPrice) {
    if (!pawn || !itemName)
        return 0;

    static SellItemFn sellItemFn = nullptr;
    if (!sellItemFn) {
        auto opt = MemoryDataLoader::Get().GetOffset("CCitadelPlayerPawn::SellItem");
        if (!opt) {
            g_Log->Error("SellItem: failed to resolve CCitadelPlayerPawn::SellItem signature");
            return 0;
        }
        sellItemFn = reinterpret_cast<SellItemFn>(opt.value());
        g_Log->Info("SellItem: resolved at {:p}", reinterpret_cast<void *>(sellItemFn));
    }

    g_Log->Info("SellItem: pawn={} item='{}' fullRefund={} forceSellPrice={}", pawn, itemName, bFullRefund, bForceSellPrice);
    auto result = sellItemFn(pawn, itemName, bFullRefund, bForceSellPrice);
    g_Log->Info("SellItem: result={}", result);
    return result;
}

static void *__cdecl NativeAddModifier(void *entity, const char *modifierName, void *kv3, void *caster, void *ability, int32_t team) {
    if (!entity || !modifierName)
        return nullptr;

    static const auto addModifier = reinterpret_cast<AddModifierFn>(
        MemoryDataLoader::Get().GetOffset("CModifierProperty::AddModifier").value());

    auto *vdata = reinterpret_cast<CModifierVData *>(
        LookupSubclassDefinitionByName(EntitySubclassScope_t::SUBCLASS_SCOPE_MODIFIERS, modifierName));
    if (!vdata) {
        g_Log->Error("AddModifier: VData not found for '{}'", modifierName);
        return nullptr;
    }

    auto *ent = static_cast<CBaseEntity *>(entity);
    CModifierProperty *modProp = ent->m_pModifierProp;
    if (!modProp) {
        g_Log->Error("AddModifier: Entity has no modifier property");
        return nullptr;
    }

    auto *pPredictedOwner = reinterpret_cast<uint8_t *>(reinterpret_cast<uintptr_t>(modProp) + kModifierPropPredictedOwner);
    uint8_t savedPredictedOwner = *pPredictedOwner;
    *pPredictedOwner = 1;

    auto *pCaster = caster ? static_cast<CBaseEntity *>(caster) : ent;
    uint32_t hAbility = 0xFFFFFFFF;
    if (ability) {
        auto *pAbility = static_cast<CBaseEntity *>(ability);
        hAbility = static_cast<uint32_t>(pAbility->GetRefEHandle().ToInt());
    } else {
        auto vecAddr = reinterpret_cast<uintptr_t>(ent) + kEntityAbilityVector;
        int count = *reinterpret_cast<int *>(vecAddr);
        auto *pHandles = *reinterpret_cast<uint32_t **>(vecAddr + 0x08);
        if (pHandles && count > 0 && count < 64) {
            for (int i = 0; i < count; i++) {
                if (pHandles[i] != 0xFFFFFFFF) {
                    hAbility = pHandles[i];
                    break;
                }
            }
        }
    }
    auto *result = addModifier(modProp, pCaster, hAbility, team, vdata, static_cast<KeyValues3 *>(kv3), nullptr);

    *pPredictedOwner = savedPredictedOwner;

    return result;
}

// ---------------------------------------------------------------------------
// Ability execution natives
// ---------------------------------------------------------------------------

static int32_t __cdecl NativeExecuteAbilityBySlot(void *abilityComponent, int16_t slot, uint8_t altCast, uint8_t flags) {
    if (!abilityComponent)
        return -1;

    static ExecuteAbilityBySlotFn fn = nullptr;
    if (!fn) {
        auto opt = MemoryDataLoader::Get().GetOffset("CCitadelAbilityComponent::ExecuteAbilityBySlot");
        if (!opt) {
            g_Log->Error("ExecuteAbilityBySlot: failed to resolve signature");
            return -1;
        }
        fn = reinterpret_cast<ExecuteAbilityBySlotFn>(opt.value());
    }
    return fn(abilityComponent, slot, altCast, flags);
}

static int32_t __cdecl NativeExecuteAbilityByID(void *abilityComponent, int32_t abilityID, uint8_t altCast, uint8_t flags) {
    if (!abilityComponent)
        return -1;

    static ExecuteAbilityByIDFn fn = nullptr;
    if (!fn) {
        auto opt = MemoryDataLoader::Get().GetOffset("CCitadelAbilityComponent::ExecuteAbilityByID");
        if (!opt) {
            g_Log->Error("ExecuteAbilityByID: failed to resolve signature");
            return -1;
        }
        fn = reinterpret_cast<ExecuteAbilityByIDFn>(opt.value());
    }
    return fn(abilityComponent, abilityID, altCast, flags);
}

static int32_t __cdecl NativeExecuteAbility(void *abilityComponent, void *ability, uint8_t altCast, uint8_t flags) {
    if (!abilityComponent || !ability)
        return -1;

    static ExecuteAbilityFn fn = nullptr;
    if (!fn) {
        auto opt = MemoryDataLoader::Get().GetOffset("CCitadelAbilityComponent::ExecuteAbility");
        if (!opt) {
            g_Log->Error("ExecuteAbility: failed to resolve signature");
            return -1;
        }
        fn = reinterpret_cast<ExecuteAbilityFn>(opt.value());
    }
    return fn(abilityComponent, ability, altCast, flags);
}

static void *__cdecl NativeGetAbilityBySlot(void *abilityComponent, int16_t slot) {
    if (!abilityComponent)
        return nullptr;

    static GetAbilityBySlotFn fn = nullptr;
    if (!fn) {
        auto opt = MemoryDataLoader::Get().GetOffset("CCitadelAbilityComponent::GetAbilityBySlot");
        if (!opt) {
            g_Log->Error("GetAbilityBySlot: failed to resolve signature");
            return nullptr;
        }
        fn = reinterpret_cast<GetAbilityBySlotFn>(opt.value());
    }
    return fn(abilityComponent, slot);
}

static void __cdecl NativeToggleActivate(void *ability, uint8_t activate) {
    if (!ability)
        return;

    static ToggleActivateFn fn = nullptr;
    if (!fn) {
        auto opt = MemoryDataLoader::Get().GetOffset("CCitadelBaseAbility::ToggleActivate");
        if (!opt) {
            g_Log->Error("ToggleActivate: failed to resolve signature");
            return;
        }
        fn = reinterpret_cast<ToggleActivateFn>(opt.value());
    }
    fn(ability, activate);
}

// ---------------------------------------------------------------------------
// Populate
// ---------------------------------------------------------------------------

void deadworks::PopulateAbilityNatives(NativeCallbacks &cb) {
    cb.RemoveAbility = &NativeRemoveAbility;
    cb.AddAbility = &NativeAddAbility;
    cb.AddItem = &NativeAddItem;
    cb.SellItem = &NativeSellItem;
    cb.AddModifier = &NativeAddModifier;
    cb.ExecuteAbilityBySlot = &NativeExecuteAbilityBySlot;
    cb.ExecuteAbilityByID = &NativeExecuteAbilityByID;
    cb.ExecuteAbility = &NativeExecuteAbility;
    cb.GetAbilityBySlot = &NativeGetAbilityBySlot;
    cb.ToggleActivate = &NativeToggleActivate;
}
