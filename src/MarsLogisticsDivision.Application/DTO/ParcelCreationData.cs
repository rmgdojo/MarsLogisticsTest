using MarsLogisticsDivision.Domain.Enums;

namespace MarsLogisticsDivision.Application.DTO;

public record ParcelCreationData(
    string Barcode,
    string Sender,
    string Recipient,
    string Contents,
    DeliveryService DeliveryService
);