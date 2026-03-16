#pragma once

#include <tier0/logging.h>

#include <format>
#include <string>

#include "Logger.hpp"
#include "../Utils/Chrono.hpp"

namespace deadworks {

class S2Logger : public Logger {
public:
    explicit S2Logger(const char *name, int flags = 0, LoggingVerbosity_t verbosity = LV_DEFAULT, Color color = UNSPECIFIED_LOGGING_COLOR)
        : m_name(name) {
        m_channelId = LoggingSystem_RegisterLoggingChannel(name, nullptr, flags, verbosity, color);
    }

    void Log(LoggingVerbosity verbosity, std::string_view message) override {
        const auto ts = utils::FormattedNow();
        std::string fmtd = std::format("[{}] [{}] [{}] {}\n", ts, m_name, GetVerbosityName(verbosity), message);
        LoggingSeverity_t severity = LS_MESSAGE;
        switch (verbosity) {
        case LoggingVerbosity::Verbose:
            severity = LS_MORE_DETAILED;
            break;
        case LoggingVerbosity::Debug:
            severity = LS_DETAILED;
            break;
        case LoggingVerbosity::Info:
            severity = LS_MESSAGE;
            break;
        case LoggingVerbosity::Warning:
            severity = LS_WARNING;
            break;
        case LoggingVerbosity::Error:
            severity = LS_ERROR;
            break;
        case LoggingVerbosity::Critical:
            severity = LS_ERROR;
            break;
        }
        LoggingSystem_Log(m_channelId, LS_MESSAGE, Color(255, 0, 255, 255), fmtd.c_str());
    }

private:
    std::string m_name;
    LoggingChannelID_t m_channelId;
};

} // namespace deadworks
