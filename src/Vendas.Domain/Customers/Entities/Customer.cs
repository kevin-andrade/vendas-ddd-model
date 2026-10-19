using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;
using Vendas.Domain.Customers.Enums;
using Vendas.Domain.Customers.Events;
using Vendas.Domain.Customers.ValueObjects;

namespace Vendas.Domain.Customers;

public sealed class Customer : AggregateRoot
{
    public FullName Name { get; private set; }
    public Cpf Cpf { get; private set; }
    public Email Email { get; private set; }
    public Phone Phone { get; private set; }
    public CustomerStatus Status { get; private set; }
    public Gender Gender { get; private set; }
    public MaritalStatus MaritalStatus { get; private set; }

    public Guid PrimaryAddressId { get; private set; }

    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    public Customer(
        FullName name,
        Cpf cpf,
        Email email,
        Phone phone,
        Address primaryAddress,
        Gender gender = Gender.Unspecified,
        MaritalStatus maritalStatus = MaritalStatus.Unspecified)
    {
        Validate(name, cpf, email, phone, primaryAddress);

        Name = name;
        Cpf = cpf;
        Email = email;
        Phone = phone;
        Status = CustomerStatus.Active;

        Gender = gender;
        MaritalStatus = maritalStatus;

        _addresses.Add(primaryAddress);
        PrimaryAddressId = primaryAddress.Id;

        AddDomainEvent(new CustomerRegisteredEvent(
            CustomerId: Id,
            Name: Name.FormattedFullName,
            Cpf: Cpf.Number,
            Email: Email.Address));
    }

    public void AddAddress(Address address)
    {
        Guard.AgainstNull(address, nameof(address), "Invalid address.");
        _addresses.Add(address);
        SetUpdatedAt();
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        Guard.AgainstNull(address, nameof(address), "Address not found.");

        Guard.Against<DomainException>(
            _addresses.Count == 1,
            "The customer must have at least one address.");

        _addresses.Remove(address!);

        // If the primary address was removed, select another one automatically
        if (addressId == PrimaryAddressId)
        {
            PrimaryAddressId = _addresses.First().Id;

            AddDomainEvent(new PrimaryAddressChangedEvent(
                CustomerId: Id,
                NewAddressId: PrimaryAddressId));
        }

        SetUpdatedAt();
    }

    public void UpdateAddress(
        Guid addressId,
        string zipCode,
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string country,
        string complement = "")
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        Guard.AgainstNull(address, nameof(address), "Address not found.");

        address!.Update(zipCode, street, number, neighborhood, city, state, country, complement);

        SetUpdatedAt();
    }

    public void SetPrimaryAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        Guard.AgainstNull(address, nameof(address), "Address not found.");

        PrimaryAddressId = address!.Id;

        AddDomainEvent(new PrimaryAddressChangedEvent(
            CustomerId: Id,
            NewAddressId: PrimaryAddressId));

        SetUpdatedAt();
    }

    public Address GetPrimaryAddress()
    {
        return _addresses.First(a => a.Id == PrimaryAddressId);
    }

    public void UpdateProfile(
        FullName name,
        Email email,
        Phone phone,
        Gender gender,
        MaritalStatus maritalStatus)
    {
        Guard.Against<DomainException>(
            Status == CustomerStatus.Blocked,
            "Blocked customers cannot update their profile.");

        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNull(phone, nameof(phone));

        Name = name;
        Email = email;
        Phone = phone;

        Gender = gender;
        MaritalStatus = maritalStatus;

        SetUpdatedAt();
    }

    public void Block()
    {
        if (Status == CustomerStatus.Blocked)
            return;

        Status = CustomerStatus.Blocked;

        AddDomainEvent(new CustomerBlockedEvent(
            CustomerId: Id,
            Cpf: Cpf.Number));

        SetUpdatedAt();
    }

    public void Activate()
    {
        Status = CustomerStatus.Active;
        SetUpdatedAt();
    }

    private static void Validate(
        FullName name,
        Cpf cpf,
        Email email,
        Phone phone,
        Address address)
    {
        Guard.AgainstNull(name, nameof(name));
        Guard.AgainstNull(cpf, nameof(cpf));
        Guard.AgainstNull(email, nameof(email));
        Guard.AgainstNull(phone, nameof(phone));
        Guard.AgainstNull(address, nameof(address), "The customer must have a primary address.");
    }
}