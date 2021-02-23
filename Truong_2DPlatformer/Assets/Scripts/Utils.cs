using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// helper methods and extensions
/// </summary>
public static class Utils
{
    public static bool IsValidGuid(this string value)
    {
        return !String.IsNullOrEmpty(value) && value != System.Guid.Empty.ToString();
    }
}