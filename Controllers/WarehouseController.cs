using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[Controller]")]
public class WarehouseController(WarehouseService warehouseService) : ControllerBase
{
    private readonly WarehouseService _service = warehouseService;

    [HttpGet("[id:int]")]
    public async Task<IActionResult> GetWarehouseById(int id)
    {
        try
        {
            return Ok(await _service.GetWarehouseByIdAsync(id));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseDto warehouseDto)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(warehouseDto);

            return Ok(await _service.CreateWarehouseAsync(warehouseDto));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("[id:int]")]
    public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] UpdateWarehouseDto updateWarehouse)
    {
        try
        {
            return Ok(await _service.UpdateWarehouseAsync(id, updateWarehouse));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
