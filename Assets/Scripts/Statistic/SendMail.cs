using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SendMail : MonoBehaviour
{
    string smtpServer = "smtp.gmail.com";
    int smtpPort = 587;
    string smtpUser = "skedeltd@gmail.com";
    string smtpPass = "xwghyfqvltpzodta";
    string recipientEmail = "skedeltd@gmail.com";

    public void SendEmail(string filePath)
    {
        try
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient(this.smtpServer);

            mail.From = new MailAddress(smtpUser);
            mail.To.Add(recipientEmail);
            mail.Subject = "Game Data";
            mail.Body = "Attached is the game data file.";
            
            Attachment attachment = new Attachment(filePath);
            mail.Attachments.Add(attachment);
            
            smtpServer.Port = smtpPort;
            smtpServer.Credentials = new NetworkCredential(smtpUser, smtpPass) as ICredentialsByHost;
            smtpServer.EnableSsl = true;

            smtpServer.Send(mail);
            Debug.Log("Email sent.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send email: {ex.Message}");
        }
    }
    
}
