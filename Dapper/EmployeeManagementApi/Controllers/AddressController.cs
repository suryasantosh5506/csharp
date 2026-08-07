using EmployeeManagementApi.Dtos.Address;
using EmployeeManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.Controllers;

public class AddressController(IAddressService addressService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAddresses()
    {
        var addresses = await addressService.GetAllAddressAsync();
        return Ok(addresses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AddressDto>> GetAddress(int id)
    {
        var address = await addressService.GetAddressByIdAsync(id);
        return Ok(address);
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<ActionResult<AddressDto>> GetAddressByEmployee(int employeeId)
    {
        var address = await addressService.GetAddressByEmployeeIdAsync(employeeId);
        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> CreateAddress(CreateAddressDto dto)
    {
        var address = await addressService.CreateAddressAsync(dto);

        return CreatedAtAction(
            nameof(GetAddress),
            new { id = address.Id },
            address);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateAddress(int id, UpdateAddressDto dto)
    {
        await addressService.UpdateAddressAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAddress(int id)
    {
        await addressService.DeleteAddressAsync(id);
        return NoContent();
    }
}