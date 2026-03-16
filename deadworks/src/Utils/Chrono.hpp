#pragma once
#include <string>
#include <chrono>

namespace deadworks {
namespace utils {

inline std::string FormattedNow() {
    using namespace std::chrono;

    const auto now = system_clock::now();
    const auto ms = duration_cast<milliseconds>(now.time_since_epoch()) % 1000;
    auto now_time_t = system_clock::to_time_t(now);
    std::tm local_tm = *std::localtime(&now_time_t);

    std::ostringstream oss;
    oss << std::put_time(&local_tm, "%Y-%m-%d %H:%M:%S")
        << "." << std::setw(3) << std::setfill('0') << ms.count();

    return oss.str();
}

} // namespace utils
} // namespace deadworks
