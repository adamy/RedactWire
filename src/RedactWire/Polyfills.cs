// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

// netstandard2.0 lacks IsExternalInit, which the compiler requires for `record`
// positional members and `init`-only setters. Provide it so we keep a clean,
// dependency-free target. No effect on net5.0+ consumers (type is internal).
#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif
