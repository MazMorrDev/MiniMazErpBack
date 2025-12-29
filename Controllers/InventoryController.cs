using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

public class InventoryController(InventoryService service) : ControllerBase
{
    private readonly IInventoryService _service = service;
}
