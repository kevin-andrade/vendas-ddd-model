using System.Text.RegularExpressions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Customers;

public sealed class Address : Entity
{
    public string ZipCode { get; private set; }
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string Complement { get; private set; }

    public Address(
        string zipCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string country,
        string complement = "")
    {
        Validate(zipCode, street, number, neighborhood, city, state, country);

        ZipCode = zipCode;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
        Complement = complement;
    }

    internal void Update(
        string zipCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string country,
        string complement = "")
    {
        Validate(zipCode, street, number, neighborhood, city, state, country);

        ZipCode = zipCode;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
        Complement = complement;
    }

    private static void Validate(
        string zipCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string country)
    {
        Guard.AgainstNullOrEmptyString(zipCode, nameof(zipCode), "Zip code is required.");
        Guard.Against<DomainException>(!Regex.IsMatch(zipCode, @"^\d{8}$"), "Invalid zip code.");
        Guard.AgainstNullOrEmptyString(street, nameof(street), "Street is required.");
        Guard.Against<DomainException>(street.Length < 3, "Street name is too short.");
        Guard.AgainstNullOrEmptyString(number, nameof(number), "Number is required.");
        Guard.Against<DomainException>(number.Length == 0, "Invalid number.");
        Guard.AgainstNullOrEmptyString(neighborhood, nameof(neighborhood), "Neighborhood is required.");
        Guard.AgainstNullOrEmptyString(city, nameof(city), "City is required.");
        Guard.AgainstNullOrEmptyString(state, nameof(state), "State is required.");
        Guard.AgainstNullOrEmptyString(country, nameof(country), "Country is required.");
    }
}