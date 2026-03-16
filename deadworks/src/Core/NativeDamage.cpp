#include "NativeDamage.hpp"
#include "NativeOffsets.hpp"
#include "Deadworks.hpp"

#include "Hooks/CBaseEntity.hpp"
#include "../Memory/MemoryDataLoader.hpp"
#include "../SDK/CBaseEntity.hpp"

using namespace deadworks;
using namespace deadworks::offsets;

// --- Function pointer types ---

using CTakeDamageInfoCtorFn = void *(__fastcall *)(void *info, void *inflictor, void *attacker, void *ability, float damage, int damageType, int customDamage);
using CTakeDamageInfoDtorFn = void(__fastcall *)(void *info);

static CTakeDamageInfoCtorFn g_pCTakeDamageInfoCtor = nullptr;
static CTakeDamageInfoDtorFn g_pCTakeDamageInfoDtor = nullptr;

// ---------------------------------------------------------------------------
// Native implementations
// ---------------------------------------------------------------------------

static void __cdecl NativeHurtEntity(void *victim, void *attacker, void *inflictor, void *ability, float damage, int32_t damageType) {
    if (!victim) return;
    if (!g_pCTakeDamageInfoCtor || !g_pCTakeDamageInfoDtor) return;

    alignas(16) uint8_t infoBuffer[kCTakeDamageInfoSize]{};
    g_pCTakeDamageInfoCtor(infoBuffer, inflictor, attacker ? attacker : inflictor, ability, damage, damageType, 0);
    hooks::g_CBaseEntity_TakeDamageOld.thiscall<void>(victim, infoBuffer, nullptr);
    g_pCTakeDamageInfoDtor(infoBuffer);
}

static void *__cdecl NativeCreateDamageInfo(void *inflictor, void *attacker, void *ability, float damage, int32_t damageType) {
    if (!g_pCTakeDamageInfoCtor) return nullptr;
    auto *info = static_cast<uint8_t *>(_aligned_malloc(kCTakeDamageInfoSize, 16));
    if (!info) return nullptr;
    std::memset(info, 0, kCTakeDamageInfoSize);
    g_pCTakeDamageInfoCtor(info, inflictor, attacker ? attacker : inflictor, ability, damage, damageType, 0);
    return info;
}

static void __cdecl NativeDestroyDamageInfo(void *info) {
    if (!info) return;
    if (g_pCTakeDamageInfoDtor) g_pCTakeDamageInfoDtor(info);
    _aligned_free(info);
}

static void __cdecl NativeTakeDamage(void *victim, void *info) {
    if (!victim || !info) return;
    hooks::g_CBaseEntity_TakeDamageOld.thiscall<void>(victim, info, nullptr);
}

// ---------------------------------------------------------------------------
// Resolution & populate
// ---------------------------------------------------------------------------

void deadworks::ResolveDamageStatics() {
    auto opt = MemoryDataLoader::Get().GetOffset("CTakeDamageInfo::Ctor");
    if (opt) g_pCTakeDamageInfoCtor = reinterpret_cast<CTakeDamageInfoCtorFn>(opt.value());
    auto opt2 = MemoryDataLoader::Get().GetOffset("CTakeDamageInfo::Dtor");
    if (opt2) g_pCTakeDamageInfoDtor = reinterpret_cast<CTakeDamageInfoDtorFn>(opt2.value());
}

void deadworks::PopulateDamageNatives(NativeCallbacks &cb) {
    cb.HurtEntity = &NativeHurtEntity;
    cb.CreateDamageInfo = &NativeCreateDamageInfo;
    cb.DestroyDamageInfo = &NativeDestroyDamageInfo;
    cb.TakeDamage = &NativeTakeDamage;
}
