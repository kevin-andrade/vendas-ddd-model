using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Orders.ValueObjects;

public class ShippingAddress : ValueObject
{
    public string ZipCode { get; private set; }
    public string Street { get; private set; }
    public string Complement { get; private set; }
    public string Neighborhood { get; private set; }
    public string State { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }


    public ShippingAddress(string zipCode, string street, string complement, string neighborhood, string state, string city, string country)
    {
        Guard.AgainstNullOrEmptyString(zipCode, nameof(zipCode));
        Guard.AgainstNullOrEmptyString(street, nameof(street));
        Guard.AgainstNullOrEmptyString(neighborhood, nameof(neighborhood));
        Guard.AgainstNullOrEmptyString(state, nameof(state));
        Guard.AgainstNullOrEmptyString(city, nameof(city));
        Guard.AgainstNullOrEmptyString(country, nameof(country));
        
        ZipCode = zipCode;
        Street = street;
        Complement = complement ?? string.Empty;
        Neighborhood = neighborhood;
        State = state;
        City = city;
        Country = country;
    }

    public static ShippingAddress Create(string zipCode, string street, string complement, string neighborhood, string state, string city,
        string country)
    {
        return new ShippingAddress(zipCode, street, complement, neighborhood, state, city, country);
    }
    
    public string FormatAddress()
    {
        return $"{Street}, {Complement} - {Neighborhood}, {City} - {State}, {Country} - CEP: {ZipCode}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ZipCode;
        yield return Street;
        yield return Complement ?? string.Empty;
        yield return Neighborhood;
        yield return State;
        yield return City;
        yield return Country;
    }
}