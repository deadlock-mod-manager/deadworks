#include "ProcessUsercmds.hpp"

#include "../Deadworks.hpp"
#include "../../SDK/CBaseEntity.hpp"

#include <google/protobuf/message_lite.h>

namespace deadworks {
namespace hooks {

// CUserCmd layout (from IDA reverse engineering):
// 0x00: vtable ptr (CUserCmdBase/CUserCmd)
// 0x08: int (command tick/state)
// 0x10: CCitadelUserCmdPB (MessageLite-derived, inherits through CUserCmdBaseHost)
// Total stride: 0xA8 (168 bytes)
static constexpr size_t kCUserCmdStride = 0xA8;
static constexpr size_t kProtobufOffset = 0x10;

void *__fastcall Hook_ProcessUsercmds(void *pController, void *cmds, int numcmds, unsigned char paused, float margin) {
    auto *entity = static_cast<CBaseEntity *>(reinterpret_cast<CEntityInstance *>(pController));
    int slot = entity->GetRefEHandle().GetEntryIndex() - 1;

    if (slot >= 0 && slot < 64 && numcmds > 0) {
        // Build a buffer with all serialized usercmds: [len1][bytes1][len2][bytes2]...
        static thread_local std::vector<uint8_t> batchBuf;
        batchBuf.clear();

        // Track which native cmd indices had valid protos (for deserialization back)
        static thread_local std::vector<int> validCmdIndices;
        validCmdIndices.clear();

        for (int i = 0; i < numcmds; i++) {
            auto *cmdBase = reinterpret_cast<char *>(cmds) + (i * kCUserCmdStride);
            auto *pb = reinterpret_cast<google::protobuf::MessageLite *>(cmdBase + kProtobufOffset);

            int size = static_cast<int>(pb->ByteSizeLong());
            if (size <= 0)
                continue;

            // Append length prefix (4 bytes, little-endian)
            size_t offset = batchBuf.size();
            batchBuf.resize(offset + 4 + size);
            memcpy(batchBuf.data() + offset, &size, 4);

            if (!pb->SerializeToArray(batchBuf.data() + offset + 4, size)) {
                batchBuf.resize(offset); // rollback on failure
                continue;
            }

            validCmdIndices.push_back(i);
        }

        if (!validCmdIndices.empty()) {
            static thread_local uint8_t outBuf[65536];
            int outLen = 0;

            g_Deadworks.OnPre_ProcessUsercmds(slot, batchBuf.data(),
                                               static_cast<int>(batchBuf.size()),
                                               static_cast<int>(validCmdIndices.size()),
                                               paused != 0, margin,
                                               outBuf, &outLen);

            // If managed code returned modified bytes, deserialize back into native protos
            if (outLen > 0) {
                int offset = 0;
                for (size_t idx = 0; idx < validCmdIndices.size() && offset + 4 <= outLen; idx++) {
                    int len;
                    memcpy(&len, outBuf + offset, 4);
                    offset += 4;

                    if (len <= 0 || offset + len > outLen)
                        break;

                    auto *cmdBase = reinterpret_cast<char *>(cmds) + (validCmdIndices[idx] * kCUserCmdStride);
                    auto *pb = reinterpret_cast<google::protobuf::MessageLite *>(cmdBase + kProtobufOffset);
                    pb->ParseFromArray(outBuf + offset, len);

                    offset += len;
                }
            }
        }
    }

    return g_ProcessUsercmds.thiscall<void *>(pController, cmds, numcmds, paused, margin);
}

} // namespace hooks
} // namespace deadworks
