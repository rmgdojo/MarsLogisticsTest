namespace MarsLogisticsDivision.Domain.Exceptions;

public class BarcodeException(string message) : FormatException(message);