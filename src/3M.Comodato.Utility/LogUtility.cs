using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using _3M.Comodato.Data;

namespace _3M.Comodato.Utility
{
    public enum TipoOrigem
    {
        Frontend,
        Backend
    }

    public enum TipoMensagem
    {
        Erro,
        Alerta,
        Info
    }
    
    public class LogUtility
    {
        public static void LogarErro(string msg, string url, string line, string error, string parameters, TipoMensagem tipo = TipoMensagem.Erro)
        {
            LogarErro(new Exception(msg), null, null, null, url, parameters, TipoOrigem.Frontend, TipoMensagem.Erro, line);
        }

        public static void LogarErro(string msg, TipoMensagem tipo = TipoMensagem.Erro) // chamar via web api
        {
            LogarErro(new Exception(msg), null, null, null, null, null, TipoOrigem.Backend);
        }

        public static void LogarErro(Exception ex, TipoMensagem tipo = TipoMensagem.Erro)
        {
            LogarErro(ex, null, null, null, null, null, TipoOrigem.Backend);
        }

        public static void LogarErro(Exception ex, string parameters, string url, TipoMensagem tipo = TipoMensagem.Erro)
        {
            LogarErro(ex, null, null, null, url, parameters, TipoOrigem.Backend);
        }

        public static void LogarErro(Exception ex, System.Web.HttpContext context, TipoMensagem tipo = TipoMensagem.Erro) // chamar via global.asax
        {
            LogarErro(ex, context, null, null, null, null, TipoOrigem.Backend);
        }

        public static void LogarErro(Exception ex, System.Web.UI.Page page, TipoMensagem tipo = TipoMensagem.Erro) // chamar via páginas aspx.cs
        {
            LogarErro(ex, null, null, page, null, null, TipoOrigem.Backend);
        }

        private static void LogarErro(Exception ex, HttpContext context, HttpRequest request, System.Web.UI.Page page, string Url, string parameters, TipoOrigem EnumLocal, TipoMensagem tipo = TipoMensagem.Erro, string line = null)
        {
            try
            {
                LogErroEntity clsParm = new LogErroEntity();

                clsParm.Data = DateTime.Now;
                clsParm.Sistema = "";

                System.Web.HttpRequest clientRequest = null;

                try
                {
                    clsParm.Cliente = "3M";
                }
                catch
                {
                    // Não faz nada
                }


                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["UsuarioInfo"] != null)
                {
                    clsParm.Usuario = "LoginUsuario";

                    try
                    {
                        if (string.IsNullOrEmpty(clsParm.Usuario.Trim()))
                        {
                            clsParm.Usuario = context.User.Identity.Name;
                        }
                    }
                    catch
                    {
                    }
                }

                StackFrame frame = new StackFrame(2);
                var method = frame.GetMethod();

                clsParm.Local = EnumLocal.ToString();
                clsParm.Classe = method.ReflectedType.Name;
                clsParm.Metodo = method.Name;
                clsParm.Servidor = Environment.MachineName;

                if (page == null && context != null && context.Request != null && HttpContext.Current.Handler != null)
                {
                    page = HttpContext.Current.Handler as System.Web.UI.Page;
                }

                try
                {
                    if (page.Request != null)
                    {
                        clsParm.Url = page.Request.Url.ToString();
                        GetBrowser(page.Request, ref clsParm);
                        if (clientRequest == null) clientRequest = page.Request;
                    }
                }
                catch
                {
                    // Não faz nada
                }

                if (request != null)
                {
                    clsParm.Url = request.Url.ToString();
                    GetBrowser(request, ref clsParm);
                    if (clientRequest == null) clientRequest = request;
                }
                else if (Url != null)
                {
                    clsParm.Url = Url;
                }
                else if (context != null && context.Request != null)
                {
                    clsParm.Url = context.Request.Url.ToString();
                    GetBrowser(context.Request, ref clsParm);
                    if (clientRequest == null) clientRequest = context.Request;
                }

                if (page != null)
                {
                    string strParameters = "";
                    ListaControles(page, ref strParameters);
                    if (strParameters.Length > 1) strParameters = strParameters.Substring(0, strParameters.Length - 1);
                    clsParm.Parameters = strParameters;
                }

                if (parameters != null)
                {
                    clsParm.Parameters = parameters;
                }

                clsParm.ErrorMessage = ex.Message;
                clsParm.Linha = frame.GetFileLineNumber().ToString();
                clsParm.StackTrace = ex.StackTrace;

                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message != null) clsParm.InnerMessage = ex.InnerException.Message;
                    if (ex.InnerException.StackTrace != null) clsParm.InnerStackTrace = ex.InnerException.StackTrace;
                }

                try
                {
                    clsParm.StackTrace = ex.GetType().ToString() + " - " + clsParm.StackTrace;
                }
                catch { } // Não faz nada

                clsParm.Tipo = tipo.ToString();

                try
                {
                    if (clientRequest != null)
                    {
                        string ipAddress = clientRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

                        if (!string.IsNullOrEmpty(ipAddress))
                        {
                            string[] addresses = ipAddress.Split(',');
                            if (addresses.Length != 0)
                            {
                                clsParm.IP = addresses[0];
                            }
                        }

                        if (string.IsNullOrEmpty(clsParm.IP)) clsParm.IP = clientRequest.ServerVariables["REMOTE_ADDR"];

                        if (!string.IsNullOrEmpty(clsParm.IP) && clsParm.IP.Length > 15) clsParm.IP = clsParm.IP.Substring(0, 14);
                    }
                }
                catch { } // Não faz nada

                if (!string.IsNullOrEmpty(clsParm.StackTrace))
                {
                    clsParm.StackTrace = clsParm.StackTrace.Replace(" at ", "\r\n at ");
                }

                if (!string.IsNullOrEmpty(line))
                {
                    clsParm.Linha = line;
                }

                // Não gravar log para os seguintes:
                if (!string.IsNullOrEmpty(clsParm.Url) && clsParm.Url.Contains("jquery.min.js")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Usuário não tem permissão pra efetuar a operação")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Usuário não possui permissão para realizar operação!")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Thread was being aborted")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Este usuário possui Histórico de Operação e não pode ser excluído.")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Este usuário está associado a uma Empresa e não pode ser excluído.")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Para gerar versão o processo deve estar com status analisado")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Ja existe um usuário cadastrado com este apelido!")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Valor total de serviços é superior ao valor total de mão de obra do processo")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Valor total de peças é superior ao valor total de peças do processo")) return;
                if (!string.IsNullOrEmpty(clsParm.ErrorMessage) && clsParm.ErrorMessage.Contains("Não existem imagens com índice de Nota Fiscal para serem vinculadas.")) return;

                //chamada assincrona comentada
                //Thread objTH = new Thread(new ParameterizedThreadStart(LogErroAsync)) { IsBackground = true, Priority = ThreadPriority.Lowest };
                //objTH.Start(clsParm);

                LogErroData Erro = new LogErroData();
                Erro.GravarLogErro(clsParm);
            }
            catch 
            {
                //gravarArquivo(exLog);
            }

        }

        private static void LogErroAsync(LogErroEntity clsParametro)
        {
            try
            {
               
                
                LogErroEntity cl = (LogErroEntity)clsParametro;

                using (SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        SqlConnectionStringBuilder strDataSistema = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

                        cmd.CommandText = "sp_ins_tbLogError";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("DataHoraErro", (object)cl.Data ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Servidor", (object)cl.Servidor ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Sistema", (object)strDataSistema.ApplicationName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Cliente", (object)cl.Cliente ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Usuario", (object)cl.Usuario ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Origem", (object)cl.Local ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Classe", (object)cl.Classe ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Metodo", (object)cl.Metodo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("URL", (object)cl.Url ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Parametros", (object)cl.Parameters ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Message", (object)cl.ErrorMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("StackTrace", (object)cl.StackTrace ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Linha", (object)cl.Linha ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("InnerMessage", (object)cl.InnerMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("InnerStackTrace", (object)cl.InnerStackTrace ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Browser", (object)cl.Browser ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("IP", (object)cl.IP ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Level", (object)cl.Tipo ?? DBNull.Value);

                        cmd.Connection = cnn;
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch 
            {
                //gravarArquivo(exLog);
            }
        }

        private static void ListaControles(Control bt, ref string strVlrParametros)
        {
            if (bt.Controls.Count > 1 || (bt is MasterPage) || (bt is Page))
            {
                foreach (Control btCtls in bt.Controls)
                {
                    ListaControles(btCtls, ref strVlrParametros);
                }
            }
            else
            {
                if (bt is TextBox && ((TextBox)bt).Text != "")
                {
                    strVlrParametros += string.Format("{0}: {1},", ((TextBox)bt).ID, ((TextBox)bt).Text) + "\r\n";
                }

                else if (bt is RadioButton && ((RadioButton)bt).Checked)
                {
                    strVlrParametros += string.Format("{0}: {1},", ((RadioButton)bt).ID, ((RadioButton)bt).Checked) + "\r\n";
                }

                else if (bt is DropDownList && ((DropDownList)bt).SelectedValue != "" && !((DropDownList)bt).SelectedValue.Contains("Selecione"))
                {
                    strVlrParametros += string.Format("{0}: {1},", ((DropDownList)bt).ID, ((DropDownList)bt).SelectedValue) + "\r\n";
                }

                else if (bt is CheckBox && ((CheckBox)bt).Checked)
                {
                    strVlrParametros += string.Format("{0}: {1},", ((CheckBox)bt).ID, ((CheckBox)bt).Checked) + "\r\n";
                }

                else if (bt is HiddenField && ((HiddenField)bt).Value != "")
                {
                    strVlrParametros += string.Format("{0}: {1},", ((HiddenField)bt).ID, ((HiddenField)bt).Value + "\r\n");
                }
            }

        }

        public static string GetBrowser(HttpRequest request)
        {
            try
            {
                if (request.Browser != null)
                {
                    return request.Browser.Browser + " " + request.Browser.Version;
                }
            }
            catch { }
            return "";
        }

        public static string GetBrowser(HttpRequestBase request)
        {
            try
            {
                if (request.Browser != null)
                {
                    return request.Browser.Browser + " " + request.Browser.Version;
                }
            }
            catch { }
            return "";
        }

        public static void GetBrowser(HttpRequest request, ref LogErroEntity clsParm)
        {
            try
            {
                clsParm.Browser = GetBrowser(request);
            }
            catch { }
        }

    }


}
