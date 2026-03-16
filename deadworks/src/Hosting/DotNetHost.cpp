#include "DotNetHost.hpp"

#include <nethost.h>

#include <Windows.h>

namespace deadworks {

bool DotNetHost::LoadHostFxr() {
    wchar_t buffer[MAX_PATH];
    size_t bufferSize = sizeof(buffer) / sizeof(wchar_t);

    if (get_hostfxr_path(buffer, &bufferSize, nullptr) != 0)
        return false;

    m_hostfxrLib = LoadLibraryW(buffer);
    if (!m_hostfxrLib)
        return false;

    m_initForConfig = reinterpret_cast<hostfxr_initialize_for_runtime_config_fn>(
        GetProcAddress(m_hostfxrLib, "hostfxr_initialize_for_runtime_config"));
    m_getRuntimeDelegate = reinterpret_cast<hostfxr_get_runtime_delegate_fn>(
        GetProcAddress(m_hostfxrLib, "hostfxr_get_runtime_delegate"));
    m_close = reinterpret_cast<hostfxr_close_fn>(
        GetProcAddress(m_hostfxrLib, "hostfxr_close"));

    return m_initForConfig && m_getRuntimeDelegate && m_close;
}

bool DotNetHost::InitializeRuntime(const std::filesystem::path &runtimeConfigPath) {
    int rc = m_initForConfig(runtimeConfigPath.c_str(), nullptr, &m_hostContext);
    if (rc != 0 || !m_hostContext)
        return false;

    void *delegate = nullptr;
    rc = m_getRuntimeDelegate(
        m_hostContext,
        hdt_load_assembly_and_get_function_pointer,
        &delegate);

    if (rc != 0 || !delegate)
        return false;

    m_loadAssemblyAndGetFunctionPointer =
        reinterpret_cast<load_assembly_and_get_function_pointer_fn>(delegate);

    return true;
}

bool DotNetHost::Initialize(const std::filesystem::path &runtimeConfigPath) {
    if (!LoadHostFxr())
        return false;

    if (!InitializeRuntime(runtimeConfigPath))
        return false;

    return true;
}

void DotNetHost::Close() {
    if (m_close && m_hostContext) {
        m_close(m_hostContext);
        m_hostContext = nullptr;
    }

    if (m_hostfxrLib) {
        FreeLibrary(m_hostfxrLib);
        m_hostfxrLib = nullptr;
    }

    m_loadAssemblyAndGetFunctionPointer = nullptr;
}

} // namespace deadworks
