#include "Logger.hpp"

#include "../Utils/Chrono.hpp"

#include <string>
#include <print>

namespace deadworks {

class ConsoleLogger : public Logger {
public:
    ConsoleLogger(std::string name)
        : m_name(std::move(name)) {}

    void Log(LoggingVerbosity verbosity, std::string_view message) override {
        const auto ts = utils::FormattedNow();
        std::println("[{}] [{}] [{}] {}", ts.c_str(), m_name.c_str(), GetVerbosityName(verbosity).data(), message.data());
    }

private:
    std::string m_name;
};

} // namespace deadworks