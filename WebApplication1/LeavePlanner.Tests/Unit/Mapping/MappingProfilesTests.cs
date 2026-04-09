using AutoMapper;
using Common.DTOs;
using FluentAssertions;
using LeavePlanner.Api.MappingProfiles;
using LeavePlanner.Domain.Models;
using LeavePlanner.Domain.MappingProfiles;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LeavePlanner.Tests.Unit.Mapping;

public class MappingProfilesTests
{
    private readonly IMapper _mapper;

    public MappingProfilesTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BackendMappingProfiles>();
            cfg.AddProfile<LeavePlanner.Domain.MappingProfiles.MappingProfiles>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void BackendMappingProfiles_ShouldMapCustomerDtoToCustomer()
    {
        var dto = new CustomerDto
        {
            Name = "John",
            Email = "john@test.com",
            PhoneNumber = "123-456-7890",
            Country = "RO",
            City = "Cluj",
            PostalCode = "400000",
            Street = "Main",
            BillingType = "SRL",
            Date = DateTime.Today
        };

        var customer = _mapper.Map<Customer>(dto);

        customer.Name.Should().Be("John");
        customer.Email.Should().Be("john@test.com");
    }

    [Fact]
    public void DomainMappingProfiles_ShouldMapCustomerToInfrastructureEntity()
    {
        var customer = new Customer
        {
            Id = 1,
            Name = "John",
            Email = "john@test.com",
            PhoneNumber = "123-456-7890",
            Country = "RO",
            City = "Cluj",
            PostalCode = "400000",
            Street = "Main",
            BillingType = "SRL",
            Date = DateTime.Today
        };

        var entity = _mapper.Map<LeavePlanner.Infrastructure.Entities.Customer>(customer);

        entity.Name.Should().Be("John");
        entity.Email.Should().Be("john@test.com");
    }
}
