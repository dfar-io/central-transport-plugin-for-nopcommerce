namespace Nop.Plugin.Shipping.CentralTransport.Services
{
    public interface IRateQuoteService
    {
        decimal GetRateQuote(string zip, int weightInLbs);
    }
}