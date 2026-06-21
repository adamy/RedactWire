// SPDX-License-Identifier: Apache-2.0
// Copyright 2026 Adam Yang, Object IT Limited, Auckland, NZ

using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Pipeline bootstrap: register a shared PiiDetector via DI (invariant rules + en-US).
// This is the "with bootstrap" path; the static Redactor facade needs none of this.
builder.Services.AddRedactWire(b => b.AddCulture(new CultureInfo("en-US")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
