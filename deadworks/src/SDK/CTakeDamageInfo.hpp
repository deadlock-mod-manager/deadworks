#pragma once

#include <ehandle.h>

#include "Schema/Schema.hpp"
#include "CBaseEntity.hpp"

class CTakeDamageInfo {
    DECLARE_SCHEMA_CLASS(CTakeDamageInfo);
    SCHEMA_FIELD(CHandle<CBaseEntity>, m_hInflictor);
    SCHEMA_FIELD(CHandle<CBaseEntity>, m_hAttacker);
    SCHEMA_FIELD(float, m_flDamage);
    SCHEMA_FIELD(float, m_flTotalledDamage);
};
