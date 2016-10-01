using Microsoft.Bot.Builder.Luis.Models;

namespace DAIS.Services
{
    public interface IResponseService
    {
        string CreateGreetingsResponce(LuisResult request);

        string CreateNoneResponse(LuisResult request);

        string CreateCompanyInfoResponse(LuisResult request);

        string TryGetEmail(LuisResult result);
    }
}