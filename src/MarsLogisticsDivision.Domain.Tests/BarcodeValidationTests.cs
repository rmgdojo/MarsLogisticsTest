using MarsLogisticsDivision.Domain.ValidationRules;

namespace MarsLogisticsDivision.Domain.Tests;

public class BarcodeValidationTests
{
    [Theory]
    [InlineData("RMARS1234567890123456789A", true)]
    [InlineData("RMARS1234567890123456789Z", true)]
    [InlineData("", false)] 
    [InlineData("RMARS1234567890123456789m", false)] // lowercase
    [InlineData("RMARS1234567890123456789", false)] // missing letter
    [InlineData("RMARS12345678901234567890M", false)] // too many digits
    [InlineData("RMARS123456789012345678M", false)] // too few digits
    [InlineData("XMARS1234567890123456789M", false)] // wrong prefix
    [InlineData("RMARS1234567890123456789*", false)] // invalid ending
    public void IsValidBarcode_ValidatesCorrectly(string barcode, bool expected)
    {
        Assert.Equal(expected, BarcodeValidation.IsValidBarcode(barcode));
    }
}