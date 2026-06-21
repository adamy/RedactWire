// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

namespace RedactWire;

/// <summary>A PII match found inside a structured document (JSON / XML / object graph),
/// paired with the location it was found at.</summary>
/// <param name="Path">Where the match sits: JSONPath (<c>$.user.email</c>),
/// XPath (<c>/root/user[1]/@email</c>), or property path (<c>User.Email</c>).</param>
/// <param name="Match">The detected PII.</param>
public sealed record StructuredPiiMatch(string Path, PiiMatch Match);
