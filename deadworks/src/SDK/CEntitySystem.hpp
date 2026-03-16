#pragma once

#include <entityinstance.h>
#include <entity2/entitykeyvalues.h>
#include <tier0/memalloc.h>

#include "../Memory/MemoryDataLoader.hpp"
#include "Core.hpp"

class EntitySystemHelper {
public:
    static CEntityInstance *CreateEntityByName(const char *className, int forceEdictIndex = -1) {
        using Fn = CEntityInstance *(__thiscall *)(void *, const char *, int);
        static const auto fn = reinterpret_cast<Fn>(
            deadworks::MemoryDataLoader::Get().GetOffset("CEntitySystem::CreateEntityByName").value());
        // Implementation references the global entity system internally, thisptr is ignored.
        return fn(nullptr, className, forceEdictIndex);
    }

    static void QueueSpawnEntity(CEntityIdentity *identity, CEntityKeyValues *ekv = nullptr) {
        using Fn = void (__thiscall *)(CGameEntitySystem *, CEntityIdentity *, CEntityKeyValues *);
        static const auto fn = reinterpret_cast<Fn>(
            deadworks::MemoryDataLoader::Get().GetOffset("CEntitySystem::QueueSpawnEntity").value());
        if (!ekv) {
            void *mem = MemAlloc_Alloc(sizeof(CEntityKeyValues));
            ekv = new (mem) CEntityKeyValues();
        }
        fn(GameEntitySystem(), identity, ekv);
    }

    static void ExecuteQueuedCreation() {
        using Fn = void (__thiscall *)(CGameEntitySystem *);
        static const auto fn = reinterpret_cast<Fn>(
            deadworks::MemoryDataLoader::Get().GetOffset("CEntitySystem::ExecuteQueuedCreation").value());
        fn(GameEntitySystem());
    }

};
