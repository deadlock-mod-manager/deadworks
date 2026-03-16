#pragma once

#include <string>
#include <span>

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <Psapi.h>
#else
#include <dlfcn.h>
#endif
#include <map>
#include <functional>

namespace deadworks {

class Module {
private:
    struct Section {
        std::string Name;
        uintptr_t Base = 0;
        size_t Size = 0;

        [[nodiscard]] std::span<std::byte> GetMemory() const {
            return {reinterpret_cast<std::byte *>(Base), Size};
        }
    };

public:
    Module(std::string_view moduleName)
        : m_name(moduleName) {
#ifdef _WIN32
        m_handle = LoadLibraryA(moduleName.data());

        if (!m_handle || m_handle == INVALID_HANDLE_VALUE)
            return;

        MODULEINFO moduleInfo;
        GetModuleInformation(GetCurrentProcess(), m_handle, &moduleInfo, sizeof(moduleInfo));

        m_base = reinterpret_cast<uintptr_t>(moduleInfo.lpBaseOfDll);
        m_size = moduleInfo.SizeOfImage;
#else
        m_handle = dlopen(moduleName.data(), RTLD_LAZY);
#endif
        LoadSections();
    }

    [[nodiscard]] bool IsValid() const { return m_handle != nullptr; }
    [[nodiscard]] std::string_view GetName() const { return m_name; }
#ifdef _WIN32
    [[nodiscard]] HMODULE GetHandle() const { return m_handle; }
#else
    [[nodiscard]] void *GetHandle() const { return m_handle; }
#endif

#ifdef _WIN32
    template <typename T>
    [[nodiscard]] T GetSymbol(std::string_view symbolName) const {
        return reinterpret_cast<T>(GetProcAddress(m_handle, symbolName.data()));
    }
#else
    template <typename T>
    [[nodiscard]] T GetSymbol(std::string_view symbolName) const {
        return reinterpret_cast<T>(dlsym(m_handle, symbolName.data()));
    }
#endif

    [[nodiscard]] const Section &GetSection(const std::string &name) const { return m_sections.at(name); }
    [[nodiscard]] std::span<std::byte> GetSectionMemory(const std::string &name) const { return GetSection(name).GetMemory(); }

    void LoadSections() {
#ifdef _WIN32
        auto *dosHdr = reinterpret_cast<PIMAGE_DOS_HEADER>(m_handle);
        auto *ntHeader = reinterpret_cast<PIMAGE_NT_HEADERS64>(reinterpret_cast<uintptr_t>(m_handle) + dosHdr->e_lfanew);

        auto *sectionHeader = IMAGE_FIRST_SECTION(ntHeader);
        for (auto i = 0; i < ntHeader->FileHeader.NumberOfSections; ++i, ++sectionHeader) {
            std::string name{reinterpret_cast<char *>(sectionHeader->Name)};
            m_sections[name] = {
                .Name = name,
                .Base = m_base + sectionHeader[i].VirtualAddress,
                .Size = sectionHeader->Misc.VirtualSize,
            };
        }
#else
        static_assert(false, "Not implemented");
#endif
    }

private:
    std::string m_name;
#ifdef _WIN32
    HMODULE m_handle;
#else
    void *m_handle;
#endif
    uintptr_t m_base;
    uintptr_t m_size;
    std::map<std::string, Section> m_sections;
};

} // namespace deadworks
