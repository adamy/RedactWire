// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

namespace RedactWire;

/// <summary>Outcome of <see cref="PiiDetector.Validate(string, PiiType, string?)"/>.
/// <c>Unsupported</c> is kept distinct from <c>Invalid</c> so callers can tell
/// "this isn't a valid value" apart from "we don't have a rule for this type/culture yet".</summary>
public enum ValidationResult
{
    /// <summary>No rule of the requested type exists for the culture(s) considered.</summary>
    Unsupported = 0,
    /// <summary>A rule exists, but the value is not a valid item of that type.</summary>
    Invalid = 1,
    /// <summary>The value is, in full, a valid item of that type.</summary>
    Valid = 2,
}
