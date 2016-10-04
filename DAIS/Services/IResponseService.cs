using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace DAIS.Services
{
    public interface IResponseService
    {
        string CreateGreetingsResponce(LuisResult request);

        string CreateNoneResponse(LuisResult request);

        string CreateCompanyInfoResponse(LuisResult request);

        string TryGetEmail(LuisResult result);

        Task<string> CreateCompanyCurrencyConverter(LuisResult result);
    }
}