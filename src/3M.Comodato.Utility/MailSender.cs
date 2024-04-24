using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.IO;

namespace _3M.Comodato.Utility
{
    public class MailSender
    {
        public bool Send(string mailTo, string mailSubject, string mailMessage, Attachment Attachments = null, string mailCopy = null)
        {
            MailMessage mail = new MailMessage();
            if (!string.IsNullOrEmpty(mailTo))
            {
                string[] to = mailTo.Split(';');
                foreach (string e in to)
                {
                    if ((!string.IsNullOrEmpty(e)) && (e != "  ") && (e != " "))
                        mail.To.Add(e);
                }
            }


            if (!string.IsNullOrEmpty(mailCopy))
            {
                string[] cc = mailCopy.Split(';');
                foreach (string e in cc)
                {
                    if (!string.IsNullOrEmpty(e))
                        mail.To.Add(e);
                }
            }

            //mail.From = new MailAddress(mailFrom);
            mail.Subject = mailSubject;
            mail.Body = mailMessage;
            mail.IsBodyHtml = true;

            if (Attachments != null)
                mail.Attachments.Add(Attachments);

            SmtpClient smtp = new SmtpClient();
            smtp.Host = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailHost);
            smtp.EnableSsl = Convert.ToBoolean(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailEnableSSL));

            mail.From = new MailAddress(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailCredentialsUserName));
            //smtp.Host = "smtp.office365.com";

            smtp.Port = Convert.ToInt16(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailPort));//587;
            smtp.UseDefaultCredentials = Convert.ToBoolean(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailUseDefaultCredentials));
            smtp.Credentials = new System.Net.NetworkCredential(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailCredentialsUserName), ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MailCredentialsPassword)); // Enter seders User name and password
            //smtp.EnableSsl = true;

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

//#if (DEBUG == true)
//            smtp.Host = "smtp.gmail.com";
//            smtp.Port = 587;
//            smtp.Credentials = new System.Net.NetworkCredential("a84npzz@gmail.com", "3m@gsw123");
//            //smtp.Timeout = 99999999;

//            //smtp.Host = "in-v3.mailjet.com";
//            //smtp.Port = 587;
//            //smtp.Credentials = new System.Net.NetworkCredential("0d8b0ab5e42077c94ce7a3eca9896e7c", "d7ef8166521152e2677be6d05e37aa1f");
//            //smtp.Timeout = 99999999;
//#endif

            smtp.Send(mail);

            //await smtp.SendAsync(mail,null);

            return true;
        }


        /// <summary>
        /// Obter o conteúdo do arquivo HTML (modelo) para construção do corpo do e-mail
        /// </summary>
        /// <param name="ArquivoHTML">Arquivo HTML (modelo)</param>
        /// <returns>Conteúdo (HTML) do arquivo</returns>
        public StringBuilder GetConteudoHTML(string ArquivoHTML)
        {
            StringBuilder HTML = null;
            StreamReader sr = null;

            //Obtém o caminho da aplicação WEB
            string caminho = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo diretorio = new DirectoryInfo(caminho);

            //Percorrer os diretórios da aplicação para obter o arquivo identificado em ArquivoHTML
            //e extrair o seu conteúdo para um stream

            if (null != diretorio)
            {
                DirectoryInfo[] subDiretorios = diretorio.GetDirectories();

                foreach (DirectoryInfo dir in subDiretorios)
                {
                    if (dir.Name.ToLower().Equals("htmlmail"))
                    {
                        FileInfo[] files = dir.GetFiles();

                        foreach (FileInfo file in files)
                        {
                            if (file.Name.ToLower().Equals(ArquivoHTML.ToLower()))
                            {
                                string linha = null;
                                HTML = new StringBuilder();

                                sr = new StreamReader(file.OpenRead());

                                while (null != (linha = sr.ReadLine()))
                                {
                                    HTML.Append(linha);
                                }
                            }
                        }
                    }
                }
            }

            if (sr != null)
            {
                sr.Close();
                sr.Dispose();
            }
            return HTML;
        }
    }
}
