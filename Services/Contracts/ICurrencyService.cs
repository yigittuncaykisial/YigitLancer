using System.Threading.Tasks;
using Entities.DTOs;
namespace Services.Contracts;
public interface ICurrencyService
{
    Task<CurrencyResponse?> GetRatesAsync(string baseCode = "USD", int amount = 1);
}
