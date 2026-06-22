// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using RedactWire.Rules;

namespace RedactWire.Rules.Localized.PtBr;

/// <summary>Brazil (pt-BR) rule pack. See <c>docs/rules/localized/pt-BR.md</c>.
/// VERIFY: formats + CPF/CNPJ checksums against authoritative sources.</summary>
internal static class PtBrRules
{
    public static readonly IPiiRule[] Rules =
    {
        // CPF (personal): 11 digits, optionally formatted 000.000.000-00; mod-11 check.
        new RegexRule("Cpf", PiiType.NationalId,
            @"(?<v>\b\d{3}\.?\d{3}\.?\d{3}-?\d{2}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.Cpf(v) ? (true, 0.97) : (false, 0)),

        // CNPJ (business): 14 digits, optionally 00.000.000/0000-00; mod-11 check.
        new RegexRule("Cnpj", PiiType.TaxId,
            @"(?<v>\b\d{2}\.?\d{3}\.?\d{3}/?\d{4}-?\d{2}\b)",
            baseConfidence: 0.4,
            validate: v => Checksums.Cnpj(v) ? (true, 0.97) : (false, 0)),

        // Mobile: optional +55, 2-digit area, 9-prefixed 8-digit number.
        new RegexRule("Mobile", PiiType.Phone,
            @"(?<v>(?:\+?55\s?)?\(?\d{2}\)?\s?9\d{4}-?\d{4})",
            baseConfidence: 0.75),

        // CEP postal: 00000-000.
        new RegexRule("Cep", PiiType.PostalCode,
            @"(?<v>\b\d{5}-?\d{3}\b)",
            baseConfidence: 0.3),
    };
}
