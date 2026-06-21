# PII severity

Every detected match carries a `PiiSeverity`. It is the **primary key for overlap
resolution**: when two matches cover the same span, the higher-severity one wins —
*even if it has lower confidence*. Confidence (or length, depending on
`OverlapStrategy`) only orders matches of equal severity.

Higher enum value = more severe: `Critical(3) > High(2) > Medium(1) > Low(0)`.

## Default tiers

| Severity | Types |
|---|---|
| **Critical** | SocialSecurity, NationalId, CreditCard, BankAccount, Iban, Passport, DriverLicense, TaxId |
| **High** | Email, Phone, Address, Person |
| **Medium** | IpAddress, PostalCode |
| **Low** | Organization |

Source of truth: `PiiSeverities.For(PiiType)` in `src/RedactWire/PiiSeverity.cs`.

## Overriding

A rule may set its own severity at construction, independent of the type default:

```csharp
new RegexRule("InternalEmployeeId", PiiType.NationalId, pattern,
    severity: PiiSeverity.Critical);
```

If a rule doesn't specify one, it inherits the type default from the table above.
