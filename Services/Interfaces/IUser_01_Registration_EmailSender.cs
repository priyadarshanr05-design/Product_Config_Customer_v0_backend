using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }
}
