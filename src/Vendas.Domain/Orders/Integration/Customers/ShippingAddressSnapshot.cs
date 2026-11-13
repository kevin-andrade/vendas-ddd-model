using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Orders;

public class ShippingAddressSnapshot : ValueObject
{
    public string ZipCode { get; }
    public string Street { get; }
    public string Number { get; }
    public string Neighborhood { get; }
    public string City { get; }
    public string State { get; }
    public string Country { get; }
    public string Complement { get; }

    public ShippingAddressSnapshot(string zipCode, string street, string number, string neighborhood,
        string city, string state, string country, string complement)
    {
        Guard.AgainstNullOrEmptyString(zipCode, nameof(zipCode), "Zip code is required.");
        Guard.AgainstNullOrEmptyString(street, nameof(street), "Street is required.");
        Guard.AgainstNullOrEmptyString(number, nameof(number), "Number is required.");
        Guard.AgainstNullOrEmptyString(neighborhood, nameof(neighborhood), "Neighborhood is required.");
        Guard.AgainstNullOrEmptyString(city, nameof(city), "City is required.");
        Guard.AgainstNullOrEmptyString(state, nameof(state), "State is required.");
        Guard.AgainstNullOrEmptyString(country, nameof(country), "Country is required.");
        
        ZipCode = zipCode;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
        Complement = complement ?? string.Empty;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ZipCode;
        yield return Street;
        yield return Number;
        yield return Neighborhood;
        yield return City;
        yield return State;
        yield return Country;
        yield return Complement;
    }
}