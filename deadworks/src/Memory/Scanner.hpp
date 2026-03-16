#pragma once

#include <optional>
#include <vector>
#include <expected>
#include <string>
#include <ranges>
#include <charconv>
#include <generator>
#include <cstdarg>
#include <algorithm>

namespace deadworks {
namespace Scanner {

using SignatureByte = std::optional<std::byte>;
using Signature = std::vector<SignatureByte>;

std::expected<Signature, std::string> ParseSignature(std::string_view sig);
std::optional<uintptr_t> FindFirst(std::span<std::byte> memory, const Signature &pattern);

} // namespace Scanner
} // namespace deadworks
