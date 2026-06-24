// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

// netstandard2.0 lacks IsExternalInit, required by `record` positional members / `init`.
// Each assembly needs its own internal copy (core's is internal to RedactWire).
#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif
