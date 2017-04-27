using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailServices
{
    public class Mail
    {
        public string _CorreoGmailOrigen { set; get; }
        public string _NombreGmailOrigen { set; get; }
        public Queue<string> _ListaCorreos { set; get; }
        public string _Subject { set; get; }
        public bool _DisponibleHTMLBODy { set; get; }
        public string _BodyHtml { set; get; }
        public string _Puerto { set; get; }
        public string File_Path_ToAtacch { set; get; }
        public string _NetworkCredencialPass { set; get; }
        public string _NetworkCredencialUser { set; get; }
    }

}
