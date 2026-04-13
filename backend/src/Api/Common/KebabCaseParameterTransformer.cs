using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Api.Common;

internal sealed partial class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value == null
            ? null
            : FindBorderRegex()
                .Replace(value.ToString()
                         ?? string.Empty, "$1-$2")
                .ToLower(CultureInfo.InvariantCulture);
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex FindBorderRegex();
}
