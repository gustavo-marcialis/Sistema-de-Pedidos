using Microsoft.AspNetCore.Authorization; // Necessário para o AllowAnonymous
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaAPI.Models;

namespace PizzaAPI.Controllers
{
    // [AllowAnonymous] torna explícito que esta parte da API é pública (Guest Access)
    // Isso faz parte da estratégia de "Zero Trust": defina explicitamente o acesso.
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        // GET: Permite que o cliente consulte o status do seu pedido pela mesa
        [HttpGet("pedidosCliente/{Mesa}")]
        public async Task<IActionResult> getByIdAsync(
            [FromServices] pizzariaContext contexto,
            [FromRoute] string Mesa)
        {
            var pedidos = await contexto.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Mesa == Mesa);

            return pedidos == null ? NotFound() : Ok(pedidos);
        }

        // POST: Permite criar um novo pedido sem estar logado
        [HttpPost("pedidosCliente")]
        public async Task<IActionResult> PostAsync(
            [FromServices] pizzariaContext contexto,
            [FromBody] Pedidos pedido)
        {
            try
            {
                // Define status inicial padrão para garantir integridade
                pedido.status = "Na fila"; 
                
                await contexto.Pedidos.AddAsync(pedido);
                await contexto.SaveChangesAsync();
                return Created($"api/pedidos/{pedido.id}", pedido);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
