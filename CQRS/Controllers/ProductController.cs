using CQRS.Application.Services.Product.Commands.CreateProduct;
using CQRS.Application.Services.Product.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ISender _mediator;

        public ProductController(ISender mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] GetProductByIdQuery req)
        {
            var result = await _mediator.Send(req);
            return result != null ? Ok(result) : NotFound();
        }
    }
}
