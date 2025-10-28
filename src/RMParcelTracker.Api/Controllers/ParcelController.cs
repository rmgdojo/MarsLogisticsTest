using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RMParcelTracker.Api.Common.Repository;
using RMParcelTracker.Api.Features.Parcel.Register;
using RMParcelTracker.Api.Features.Parcel.Register.Validators;
using RMParcelTracker.Api.Features.Parcel.Retrieve;
using RMParcelTracker.Api.Features.Parcel.Update;

namespace RMParcelTracker.Api.Controllers;

[ApiController]
[Route("parcels")]
public class ParcelController(
    RegisterParcel registerParcelService,
    UpdateParcelStatus updateParcelService,
    ParcelRepository parcelRepository
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RegisterParcelResponse>> Register([FromBody] RegisterParcelRequest request,
        [FromServices] IValidator<RegisterParcelRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        if (parcelRepository.Get(request.Barcode) != null)
        {
            return Conflict();
        }
        
        var response = registerParcelService.Handle(request);
        if (response.IsFailed)
        {
            var errors = response.Errors.Select(e => e.Message).ToArray();
            return BadRequest(errors);
        }

        parcelRepository.Add(response.Value);
        
        var mappedResponse = RegisterParcelResponseMapper.MapFrom(response.Value);
        return CreatedAtAction(
            nameof(GetParcel),
            new { barcode = mappedResponse.Barcode },
            mappedResponse);
    }

    [HttpGet("{barcode}")]
    public async Task<ActionResult<RetrieveParcelResponse>> GetParcel(string barcode,
        [FromServices] BarcodeValidator barCodeValidator)
    {
        var validationResult = await barCodeValidator.ValidateAsync(barcode);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        var parcel = parcelRepository.Get(barcode);

        if (parcel is null) return NotFound();

        var mappedResponse = RetrieveParcelResponseMapper.MapFrom(parcel);
        return Ok(mappedResponse);
    }

    [HttpPatch("{barcode}")]
    public async Task<IActionResult> UpdateParcelStatus(
        string barcode,
        [FromBody] UpdateParcelStatusRequest request,
        [FromServices] IValidator<UpdateParcelStatusRequestWithBarCode> validator)
    {
        var requestWithBarcode = new UpdateParcelStatusRequestWithBarCode(barcode, request.NewStatus);
        var validationResult = await validator.ValidateAsync(requestWithBarcode);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        var result = updateParcelService.Handle(requestWithBarcode);

        if (result.IsFailed)
        {
            var errors = result.Errors.Select(e => e.Message).ToArray();
            return BadRequest(errors);
        }

        return new OkResult();
    }
}