using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MailServices
{
    public class MailOperaciones
    {
        public static String SenMail(Mail objMail)
        {
            string SendErro = string.Empty;

            try
            {

                Attachment Attach;
                MailMessage mailMessage = new MailMessage();
                SmtpClient SMtCliente = new SmtpClient("smtp.gmail.com");
                mailMessage.From = new MailAddress(objMail._CorreoGmailOrigen, objMail._NombreGmailOrigen);
                foreach (string s in objMail._ListaCorreos)
                {
                    mailMessage.To.Add(s);

                }
                mailMessage.Subject = objMail._Subject;
                mailMessage.IsBodyHtml = objMail._DisponibleHTMLBODy;
                mailMessage.Body = objMail._BodyHtml;
                Attach = new Attachment(objMail.File_Path_ToAtacch);
                mailMessage.Attachments.Add(Attach);
                SMtCliente.Port = int.Parse(objMail._Puerto);
                SMtCliente.Credentials = new System.Net.NetworkCredential(objMail._NetworkCredencialUser, objMail._NetworkCredencialPass);
                SMtCliente.EnableSsl = objMail._DisponibleHTMLBODy;

                SMtCliente.Send(mailMessage);
                SendErro = "Correo Envia";
                //}
            }
            catch (Exception ex)
            {

                //throw ex;
                SendErro = "Error en envio de Correo" + ex.Message;



            }
            return SendErro;
        }

        public static String SenMailNoAttachments(Mail objMail)
        {
            string SendErro = string.Empty;

            try
            {

                Attachment Attach;
                MailMessage mailMessage = new MailMessage();
                SmtpClient SMtCliente = new SmtpClient("smtp.gmail.com");
                mailMessage.From = new MailAddress(objMail._CorreoGmailOrigen, objMail._NombreGmailOrigen);
                foreach (string s in objMail._ListaCorreos)
                {
                    mailMessage.To.Add(s);

                }
                mailMessage.Subject = objMail._Subject;
                mailMessage.IsBodyHtml = objMail._DisponibleHTMLBODy;
                mailMessage.Body = objMail._BodyHtml;
                //Attach = new Attachment(objMail.File_Path_ToAtacch);
                //mailMessage.Attachments.Add(Attach);
                SMtCliente.Port = int.Parse(objMail._Puerto);
                SMtCliente.Credentials = new System.Net.NetworkCredential(objMail._NetworkCredencialUser, objMail._NetworkCredencialPass);
                SMtCliente.EnableSsl = objMail._DisponibleHTMLBODy;

                SMtCliente.Send(mailMessage);
                SendErro = "Correo Envia";
                //}
            }
            catch (Exception ex)
            {

                //throw ex;
                SendErro = "Error en envio de Correo" + ex.Message;



            }
            return SendErro;
        }
    }
}

