#pragma once

#include "Schema/Schema.hpp"
#include "CBasePlayerController.hpp"
#include "CCitadelPlayerPawn.hpp"

class CCitadelPlayerController : public CBasePlayerController {
public:
    DECLARE_SCHEMA_CLASS(CCitadelPlayerController);
    SCHEMA_FIELD(CHandle<CCitadelPlayerPawn>, m_hHeroPawn);

    CCitadelPlayerPawn *GetHeroPawn() {
        return m_hHeroPawn.Get();
    }
};
