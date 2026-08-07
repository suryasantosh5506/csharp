using EmployeeManagementApi.Dtos.Employee;
using EmployeeManagementApi.Dtos.Transactions;
using EmployeeManagementApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.Controllers;

public class EmployeeController(IEmployeeService employeeService) : BaseApiController
{
    [HttpGet("department/{departmentId:int}")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(int departmentId)
    {
        var employees = await employeeService.GetAllEmployeesAsync(departmentId);
        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(id);
        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto dto)
    {
        var employee = await employeeService.CreateEmployeeAsync(dto);

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employee.Id },
            employee);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateEmployee(int id, UpdateEmployeeDto dto)
    {
        await employeeService.UpdateEmployeeAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteEmployee(int id)
    {
        await employeeService.DeleteEmployeeAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<EmployeeDetailsDto>> GetEmployeeDetails(int id)
    {
        var employee = await employeeService.GetEmployeeDetailsAsync(id);
        return Ok(employee);
    }

    [HttpPost("complete")]
    public async Task<ActionResult<EmployeeDetailsDto>> CreateEmployeeWithAddress(CreateEmployeeWithAddressDto dto)
    {
        var employee = await employeeService.CreateEmployeeWithAddressAsync(dto);
        return CreatedAtAction(nameof(GetEmployeeDetails), new { id = employee.Id }, employee);
    }
}