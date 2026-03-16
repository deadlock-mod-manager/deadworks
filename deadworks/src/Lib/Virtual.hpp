#pragma once

template <typename Fn>
Fn GetVirtual(void *base, int index) {
    auto **pvft = *static_cast<void ***>(base);
    return reinterpret_cast<Fn>(pvft[index]);
}

template <typename T, typename... Args>
T CallVirtual(void *base, uint32_t index, Args... args) {
#ifdef _WIN32
    auto pfn = GetVirtual<T(__thiscall *)(void *, Args...)>(base, index);
#else
    static_assert(false, "Not implemented");
#endif
    return pfn(base, args...);
}
