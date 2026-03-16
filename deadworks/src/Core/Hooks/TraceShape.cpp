#include "TraceShape.hpp"
#include "../Deadworks.hpp"

namespace deadworks {
namespace hooks {

void __fastcall Hook_TraceShape(void *pPhysQuery, Ray_t *ray, Vector *start, Vector *end, CTraceFilter *filter, CGameTrace *traceOutput) {
    if (g_pPhysicsQuery == nullptr) {
        g_pPhysicsQuery = pPhysQuery;
        g_Log->Info("Captured g_pPhysicsQuery: {:p}", g_pPhysicsQuery);
    }

    g_TraceShape.fastcall<void>(pPhysQuery, ray, start, end, filter, traceOutput);
}

} // namespace hooks
} // namespace deadworks
