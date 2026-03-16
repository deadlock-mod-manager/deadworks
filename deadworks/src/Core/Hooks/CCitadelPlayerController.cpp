#include "CCitadelPlayerController.hpp"

#include "../Deadworks.hpp"

namespace deadworks {
namespace hooks {

char __fastcall Hook_CCitadelPlayerController_ClientConCommand(void *thisptr, void *args) {
    if (g_Deadworks.OnPre_ClientConCommand(thisptr, args))
        return 1;

    return g_CCitadelPlayerController_ClientConCommand.thiscall<char>(thisptr, args);
}

} // namespace hooks
} // namespace deadworks
