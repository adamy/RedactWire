// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

namespace RedactWire.Rules.Localized.EnUs;

/// <summary>USPS two-letter codes — 50 states + DC. Used to validate the state in an
/// address so a random "ZZ" doesn't pass. Territories (PR, GU, VI, ...) are an open item.</summary>
internal static class UsStates
{
    public static readonly HashSet<string> Codes = new(StringComparer.OrdinalIgnoreCase)
    {
        "AL","AK","AZ","AR","CA","CO","CT","DE","FL","GA",
        "HI","ID","IL","IN","IA","KS","KY","LA","ME","MD",
        "MA","MI","MN","MS","MO","MT","NE","NV","NH","NJ",
        "NM","NY","NC","ND","OH","OK","OR","PA","RI","SC",
        "SD","TN","TX","UT","VT","VA","WA","WV","WI","WY",
        "DC",
    };
}
