namespace ArchitecturalWebsite.Application.Interfaces
{
    public interface IPaystackService
    {
        Task<string> CreatePaymentLink(string email, decimal amount, string reference, object metadata);
    }
}
