#include "Scanner.hpp"

namespace deadworks {
namespace Scanner {

std::expected<Signature, std::string> ParseSignature(std::string_view sig) {
    Signature pattern;

    for (const auto part : std::views::split(sig, ' ')) {
        std::string_view token(part.begin(), part.end());
        if (token.empty()) continue;

        if (token == "?" || token == "??") {
            pattern.push_back(std::nullopt);
        } else {
            uint8_t byteValue;
            auto result = std::from_chars(token.data(), token.data() + token.size(), byteValue, 16);
            if (result.ec != std::errc()) {
                return std::unexpected("Invalid signature byte");
            }
            pattern.push_back(std::byte(byteValue));
        }
    }

    return pattern;
}

std::optional<uintptr_t> FindFirst(std::span<std::byte> memory, const Signature &pattern) {
    if (pattern.empty() || memory.size() < pattern.size()) return std::nullopt;

    while (true) {
        auto it = std::search(memory.begin(), memory.end(), pattern.begin(), pattern.end(),
                              [](std::byte memByte, const SignatureByte &sigByte) {
                                  return !sigByte.has_value() || *sigByte == memByte;
                              });

        if (it == memory.end()) return std::nullopt;

        return reinterpret_cast<uintptr_t>(&*it);
    }

    return std::nullopt;
}

} // namespace Scanner
} // namespace deadworks
