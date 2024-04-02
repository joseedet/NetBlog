using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
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

        string correo_emisor = "@gmail.com";
        string clave_emisor="";

        MailAddress receptor = new(correo_receptor);
         MailAddress emisor = new(correo_emisor);


        MailMessage email = new(emisor, receptor);

        email.Subject = "NetBlog: Activación de cuenta";
        email.Body = "";

        email.IsBodyHtml = true;

        SmtpClient smtp = new();

        smtp.Host = " smtp.gmail.com";
        smtp.Port = 587;
        smtp.Credentials = new NetworkCredential(correo_emisor, clave_emisor);


    }

    
}



