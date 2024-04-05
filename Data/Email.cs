using System.Net;
using System.Net.Mail;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace NetBlog.Data;

public class Email
{
    public void Enviar(string correo, string token)
    {
        Correo(correo, token);
    }
    void Correo(string correo_receptor,string token)
    {

        string correo_emisor = "joseedet@gmail.com";
        string clave_emisor="Morfeo68";

        MailAddress receptor = new(correo_receptor);
        MailAddress emisor = new(correo_emisor);


        MailMessage email = new(emisor, receptor);

        email.Subject = "NetBlog: Activación de cuenta";
        email.Body = @"<!DOCTYPE html>
                        <html><head><title>Activación de cuenta</title></head>
                        <body><h2>Activación de cuenta</h2><p>Para activar su cuenta haga click en el siguiente enlace:<p>
	                    <a href='https://localhost:7085/Cuenta/Token?valor=" + token + "'>Activar cuenta</a></body></html>";

        email.IsBodyHtml = true;

        SmtpClient smtp = new();

        smtp.Host = "smtp.gmail.com";
        smtp.Port = 587;
        smtp.Credentials = new NetworkCredential(correo_emisor, clave_emisor);
        smtp.DeliveryMethod =SmtpDeliveryMethod.Network;
        smtp.EnableSsl = true;

        try
        {
            smtp.Send(email);
        }
        catch
        {


        }


    }

    
}



