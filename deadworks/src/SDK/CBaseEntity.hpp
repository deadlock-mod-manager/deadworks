#pragma once

#include <entityinstance.h>

#include "Schema/Schema.hpp"

class CModifierProperty;

struct CEntitySubclassVDataBase {
    void **vft;
    void *unk;
    const char *m_pszName;
    uint8_t pad[0x10];
};

class CBaseEntity : public CEntityInstance {
    DECLARE_SCHEMA_CLASS(CBaseEntity);
    SCHEMA_FIELD(int32_t, m_iHealth);
    SCHEMA_FIELD(uint8_t, m_iTeamNum);
    SCHEMA_FIELD(CModifierProperty *, m_pModifierProp);
    // m_pSubclassVData lives right after m_nSubclassID (CUtlStringToken, 4 bytes)
    SCHEMA_FIELD_OFFSET(CEntitySubclassVDataBase *, m_nSubclassID, 4);
};
