#pragma once

#include <igameresourceservice.h>

class CGameEntitySystem;
// must be defined in global scope
inline CGameEntitySystem *GameEntitySystem() {
    return reinterpret_cast<CGameResourceService *>(g_pGameResourceServiceServer)->m_pEntitySystem;
}
