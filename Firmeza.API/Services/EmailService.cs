using System.Net;
using System.Net.Mail;

namespace Firmeza.API.Services;

public interface IEmailService
{
    Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
    {
        var remitente = _config["Email:From"]!;
        var password = _config["Email:Password"]!;

        var mensaje = new MailMessage
        {
            From = new MailAddress(remitente, "Firmeza"),
            Subject = asunto,
            Body = cuerpo,
            IsBodyHtml = true
        };
        mensaje.To.Add(destinatario);

        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(remitente, password),
            EnableSsl = true
        };

        await smtp.SendMailAsync(mensaje);
    }
}
