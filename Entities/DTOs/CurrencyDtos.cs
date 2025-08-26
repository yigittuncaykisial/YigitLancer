using System.Collections.Generic;
namespace Entities.DTOs;
public class CurrencyResponse
{
    public bool Success { get; set; }
    public CurrencyResult Result { get; set; }
}

public class CurrencyResult
{
    public string Base { get; set; }
    public string Lastupdate { get; set; }
    public List<CurrencyRow> Data { get; set; } = new();
}

public class CurrencyRow
{
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Rate { get; set; }
    public decimal Calculated { get; set; }
}
