using System.Text.RegularExpressions;

namespace MarsLogisticsDivision.Domain.ValidationRules;

public partial class BarcodeValidation
{
    private static readonly Regex Regex = BarcodeRegex();

    public static bool IsValidBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return false;
        return Regex.IsMatch(barcode);
    }

    [GeneratedRegex(@"^RMARS[0-9]{19}[A-Z]$")]
    public static partial Regex BarcodeRegex();
}