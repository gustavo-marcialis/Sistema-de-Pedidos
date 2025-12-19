using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaAPI.Models;
using System.Security.Claims;

namespace PizzaAPI.Controllers
{
    // [Authorize] -> Tranca todas as portas. Só entra com crachá (Token).
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        // GET: Leitura permitida para qualquer funcionário logado
        [HttpGet("pedidos")]
        public async Task<IActionResult> getAllAsync([FromServices] pizzariaContext contexto)
        {
            var pedidos = await contexto.Pedidos.AsNoTracking().ToListAsync();
            return Ok(pedidos);
        }

        [HttpGet("pedidos/{id}")]
        public async Task<IActionResult> getByIdAsync(
            [FromServices] pizzariaContext contexto, 
            [FromRoute] int id)
        {
            var pedidos = await contexto.Pedidos.AsNoTracking().FirstOrDefaultAsync(p => p.id == id);
            return pedidos == null ? NotFound() : Ok(pedidos);
        }

        // PUT: Atualização com Regras de Segurança (RBAC)
        [HttpPut("pedidos/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] pizzariaContext contexto,
            [FromBody] Pedidos pedido,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            // Verifica qual o cargo do usuário (Claims)
            bool isPizzaiolo = User.IsInRole("Pizzaiolo");
            bool isGarcom = User.IsInRole("Garcom");

            var p = await contexto.Pedidos.FirstOrDefaultAsync(x => x.id == id);
            if (p == null) return NotFound("Pedido não encontrado!");

            try
            {
                // Regra 1: Pizzaiolo só mexe no STATUS
                if (isPizzaiolo && !isGarcom)
                {
                    p.status = pedido.status;
                    // Ignora mudanças de sabor/mesa
                }
                // Regra 2: Garçom só edita o PEDIDO (se não estiver pronto)
                else if (isGarcom && !isPizzaiolo)
                {
                    if (p.status == "Pronto") 
                        return BadRequest("Acesso Negado: Garçom não edita pedido pronto.");
                    
                    p.Mesa = pedido.Mesa;
                    p.Sabores = pedido.Sabores;
                    p.Obs = pedido.Obs;
                }
                // Admin ou Gerente (quem tem os dois papeis) pode tudo
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

        // DELETE: Pode manter restrito ou aberto a todos logados (conforme seu requisito)
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
