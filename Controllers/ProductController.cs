using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

public class ProductController(ProductService service): ControllerBase
{
    private readonly IProductService _service = service;
}
