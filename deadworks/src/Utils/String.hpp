#pragma once

#include <vector>
#include <string_view>
#include <string>
#include <cctype>

namespace deadworks {
namespace utils {

[[nodiscard]] inline std::vector<std::string_view> TokenizeChat(std::string_view input) {
    std::vector<std::string_view> tokens;
    size_t pos = 0;
    const size_t len = input.length();

    while (pos < len) {
        while (pos < len && std::isspace(static_cast<unsigned char>(input[pos]))) {
            pos++;
        }
        if (pos == len) break;

        if (input[pos] == '"') {
            pos++;
            size_t start = pos;

            while (pos < len && input[pos] != '"') {
                pos++;
            }

            tokens.push_back(input.substr(start, pos - start));

            if (pos < len) pos++;
        } else {
            size_t start = pos;
            while (pos < len && !std::isspace(static_cast<unsigned char>(input[pos]))) {
                pos++;
            }
            tokens.push_back(input.substr(start, pos - start));
        }
    }
    return tokens;
}

} // namespace utils
} // namespace deadworks
