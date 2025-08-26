using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class RatesController : ControllerBase
{
    private readonly ICurrencyService _fx;
    public RatesController(ICurrencyService fx) { _fx = fx; }

    // /api/rates?baseCode=USD&show=TRY,EUR,GBP
    [HttpGet]
    public async Task<IActionResult> Get(string baseCode = "USD", string show = "TRY,EUR,GBP")
    {
        var resp = await _fx.GetRatesAsync(baseCode, 1);
        if (resp?.Result?.Data == null) return StatusCode(502, "FX provider error");

        var wanted = show.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                         .Select(s => s.ToUpper()).ToHashSet();

        var list = resp.Result.Data
    .Where(d => wanted.Contains(d.Code?.ToUpper()))
    .Select(d => new { code = d.Code.ToUpper(), rate = d.Rate })

    .ToList();
        return Ok(new { @base = resp.Result.Base, last = resp.Result.Lastupdate, list });

    }
}
