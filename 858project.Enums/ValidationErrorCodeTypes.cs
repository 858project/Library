using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace project858.Enums
{
    /// <summary>
    /// Validation error
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ValidationErrorCodeTypes : UInt32
    {
        Unknown = 0x0000,
        // model = 0001
        ModelIsNotValid,
        SerialNumberIsNotValid,
        EntryTypeIsNotValid,
        CardSchemeTypeIsNotValid,
        CardTypeIsNotValid,
        TransactionTypeIsNotValid,
        CardholderIdTypeIsNotValid,
        PanIsNotValid,
        CardPresentTypeIsNotValid,
        SequenceNumberIsNotValid,
        StanIsNotValid,
        AmountIsNotValid,
        AmountIsOutOfRange,
        AuthorizedAmountIsNotValid,
        TipIsNotValid,
        DateIsNotValid,
        ParentIsNotValid,
        CashBackAmountIsNotValid,
        RentalOfDurationIsNotValid,
        RentalOfDurationIsOutOfRange,
        StayOfDurationIsNotValid,
        StayOfDurationIsOutOfRange,
        AutoRentalDataIsNotValid,
        HotelDataIsNotValid,
        TransportDataIsNotValid,
        IdIsNotValid,
        TransactionIdIsNotValid,
        AgreementNumberIsNotValid,
        PickupLocationIsNotValid,
        PickupCityNameIsNotValid,
        PickupRegionCodeIsNotValid,
        PickupCountryCodeIsNotValid,
        PickupLocationIdIsNotValid,
        PickupDateIsNotValid,
        ReturnLocationIsNotValid,
        ReturnCityNameIsNotValid,
        ReturnRegionCodeIsNotValid,
        ReturnCountryCodeIsNotValid,
        ReturnLocationIdIsNotValid,
        ReturnDateIsNotValid,
        RenterNameIsNotValid,
        VehicleClassIsNotValid,
        DistanceIsNotValid,
        DistanceUnitIsNotValid,
        AuditAdjustmentIndicatorIsNotValid,
        AuditAdjustmentAmountIsNotValid,
        ExtraChargeIsNotValid,
        TransportationTypeIsNotValid,
        NetworkIndicationCodeIsNotValid,
        ApprovalCodeIsNotValid,
        StateIsNotValid,
        NameIsNotValid,
        TotalAmountIsNotValid,
        UnitIsNotValid,
        ColorIsNotValid,
        QuantityIsNotValid,
        DeviceIsNotValid,
        NumberIsNotValid,
        TypeIsNotValid,
        ContentTypeIsNotValid,
        DataIsNotValid,
        CurrencyNumberNotValid,

        // operation = 10000
        OperationIsNotAllowed = 0x2710
    }
}
