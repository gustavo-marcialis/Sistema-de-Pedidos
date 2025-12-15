using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaAPI.Models;
using System.Security.Claims;

namespace PizzaAPI.Controllers
{
    // [Authorize] Protege todo o controlador. 
    // Exige que a requisição tenha um Token válido do Azure AD.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        // GET: Acesso de leitura (Permitido para qualquer usuário autenticado)
        [HttpGet("pedidos")]
        public async Task<IActionResult> getAllAsync([FromServices] pizzariaContext contexto)
        {
            var pedidos = await contexto.Pedidos.AsNoTracking().ToListAsync();
            return Ok(pedidos);
        }

        // GET por ID
        [HttpGet("pedidos/{id}")]
        public async Task<IActionResult> getByIdAsync(
            [FromServices] pizzariaContext contexto,
            [FromRoute] int id)
        {
            var pedidos = await contexto.Pedidos.AsNoTracking().FirstOrDefaultAsync(p => p.id == id);
            return pedidos == null ? NotFound() : Ok(pedidos);
        }

        // PUT: Alteração de Pedido (Aplica Regras de Negócio e Segurança)
        [HttpPut("pedidos/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] pizzariaContext contexto,
            [FromBody] Pedidos pedido,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            // SC-900: Verificação de Claims (Papéis) para Autorização Granular
            bool isPizzaiolo = User.IsInRole("Pizzaiolo");
            bool isGarcom = User.IsInRole("Garcom");

            var p = await contexto.Pedidos.FirstOrDefaultAsync(x => x.id == id);
            if (p == null) return NotFound("Pedido não encontrado!");

            try
            {
                // Regra de Privilégio Mínimo: Pizzaiolo só altera o STATUS
                if (isPizzaiolo && !isGarcom)
                {
                    p.status = pedido.status;
                    // Ignora silenciosamente alterações em mesa/sabores
                }
                // Regra de Privilégio Mínimo: Garçom só altera o PEDIDO (se não estiver pronto)
                else if (isGarcom && !isPizzaiolo)
                {
                    if (p.status == "Pronto") 
                        return BadRequest("Acesso Negado: Garçom não pode editar pedido que já está pronto.");
                    
                    p.Mesa = pedido.Mesa;
                    p.Sabores = pedido.Sabores;
                    p.Obs = pedido.Obs;
                    // Garçom não pode mudar status aqui
                }
                // Admin ou usuários com múltiplos papéis (Gerente) têm acesso total
                else 
                {
                    p.Mesa = pedido.Mesa;
                    p.Sabores = pedido.Sabores;
                    p.Obs = pedido.Obs;
                    p.status = pedido.status;
                }

                contexto.Pedidos.Update(p);
                await contexto.SaveChangesAsync();
                return Ok(p);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE: Proteção extra (apenas exemplo)
        // Poderíamos exigir uma role específica como [Authorize(Roles = "Gerente")]
        [HttpDelete("pedidos/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] pizzariaContext contexto,
            [FromRoute] int id)
        {
            var p = await contexto.Pedidos.FirstOrDefaultAsync(x => x.id == id);
            if (p == null) return NotFound();

            try
            {
                contexto.Pedidos.Remove(p);
                await contexto.SaveChangesAsync();
                return Ok(p);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
