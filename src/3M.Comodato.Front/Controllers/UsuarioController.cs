using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Configuration;

namespace _3M.Comodato.Front.Controllers
{
    public class UsuarioController : BaseController
    {
        // GET: Usuario
        [_3MAuthentication]
        public ActionResult Index()
        {
            List<Models.Usuario> usuario = new List<Models.Usuario>();

            try
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.Usuario Usuario = new Models.Usuario
                        {
                            nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["nidUsuario"].ToString()),
                            cnmNome = dataTableReader["cnmNome"].ToString(),
                            cdsLogin = dataTableReader["cdsLogin"].ToString(),
                            cdsEmail = dataTableReader["cdsEmail"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            cdsAtivo = (Convert.ToBoolean(dataTableReader["bidAtivo"]) == true ? "Ativo" : "Inativo"),
                            perfil = new PerfilEntity()
                            {
                                nidPerfil = dataTableReader["nidPerfil"] == DBNull.Value ? 0 : Convert.ToInt64(dataTableReader["nidPerfil"]),
                                cdsPerfil = dataTableReader["cdsPerfil"] == DBNull.Value ? "" : dataTableReader["cdsPerfil"].ToString(),
                                ccdPerfil = dataTableReader["ccdPerfil"] == DBNull.Value ? 0 : int.Parse(dataTableReader["ccdPerfil"].ToString())
                            }
                        };
                        usuario.Add(Usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            IEnumerable<Models.Usuario> iPerfis = usuario;
            return View(iPerfis);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Models.Usuario usuario = new Models.Usuario
            {
                perfis = ObterListaPerfil(),
                nidPerfilExternoPadrao = ObterPerfilExternoPadrao(),
                cdsSenha = GerarSenhaAleatoria().Trim(),
                empresas = ObterListaEmpresa()
            };

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [_3MAuthentication]
        public ActionResult Incluir(Models.Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioEntity usuarioEntity = new UsuarioEntity();

                    usuarioEntity.cnmNome = usuario.cnmNome;
                    usuarioEntity.cdsLogin = usuario.cdsLogin;
                    usuarioEntity.cdsEmail = usuario.cdsEmail;
                    //usuarioEntity.bidAtivo = usuario.bidAtivo;
                    usuarioEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    usuarioEntity.cd_empresa = usuario.cd_empresa;

                    //if (usuario.nidPerfilExternoPadrao == usuario.perfil.nidPerfil)
                    if (usuario.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) ||
                        usuario.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica) ||
                        usuario.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente))
                    {
                        usuarioEntity.cdsSenha = ControlesUtility.Criptografia.Criptografar(usuario.cdsSenha).Trim();
                        double diasTrocaSenhaExterno = Convert.ToDouble("0" + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.diasTrocaSenhaExterno)) * -1;
                        usuarioEntity.dtmDataHoraTrocaLoginExterno = DateTime.Now.AddDays(diasTrocaSenhaExterno);
                    }

                    UsuarioEntity usuarioEntityLogin = new UsuarioEntity();
                    usuarioEntityLogin.cdsLogin = usuario.cdsLogin;
                    DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntityLogin).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            if (usuarioEntityLogin.cdsLogin == dataTableReader["cdsLogin"].ToString())
                            {
                                ViewBag.LoginDuplicado = "Este login já está sendo utilizado. Crie um novo.";
                                usuario.perfis = ObterListaPerfil();
                                usuario.empresas = ObterListaEmpresa();
                                return View(usuario);
                            }
                        }
                    }

                    UsuarioEntity usuarioEntityEmail = new UsuarioEntity();
                    usuarioEntityEmail.cdsEmail = usuario.cdsEmail;
                    dataTableReader = new UsuarioData().ObterLista(usuarioEntityEmail).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            if (usuarioEntityEmail.cdsEmail == dataTableReader["cdsEmail"].ToString())
                            {
                                ViewBag.EmailDuplicado = "Este e-mail já está sendo utilizado.";
                                usuario.perfis = ObterListaPerfil();
                                usuario.empresas = ObterListaEmpresa();
                                return View(usuario);
                            }
                        }
                    }

                    new UsuarioData().Inserir(ref usuarioEntity);

                    UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();

                    usuarioPerfilEntity.nidUsuarioPerfil = usuario.usuarioPerfil.nidUsuarioPerfil;
                    usuarioPerfilEntity.usuario.nidUsuario = usuarioEntity.nidUsuario;
                    usuarioPerfilEntity.perfil.nidPerfil = usuario.perfil.nidPerfil;
                    usuarioPerfilEntity.bidAtivo = usuario.bidAtivo;
                    usuarioPerfilEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new UsuarioPerfilData().Inserir(ref usuarioPerfilEntity);

                    // Envia senha por e-mail caso for usuário externo
                    if (usuario.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) ||
                        usuario.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica) ||
                        usuario.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente))
                    {
                        MailSender mailSender = new MailSender();

                        string mailTo = usuarioEntity.cdsEmail;
                        string mailSubject = "3M.Comodato - Primeiro acesso";
                        string mailMessage = string.Empty;
                        System.Net.Mail.Attachment Attachments = null;
                        string mailCopy = null;

                        var Mensagem = mailSender.GetConteudoHTML("EmailCorpo.html");
                        string Conteudo = string.Empty;
                        string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                        //URLSite += "/Usuario/TrocarSenhaViaChave?guid=" + usuarioEntity.ccdChaveAcessoTrocarSenha;
                        URLSite += "/Usuario/Login";

                        Conteudo += "<p>Prezado(a) usuário(a)</p>";
                        Conteudo += "<p>Conforme solicitado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ". Segue LINK para o <b>Primeiro Acesso</b> ao Sistema <b>3M.Comodato</b>.</p>";
                        Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegado WEB:</p>";
                        Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";
                        Conteudo += "<p>Utilize os dados iniciais a seguir para criação de senha própria:</p>";
                        Conteudo += "<p>Usuário: </p><b>" + usuarioEntity.cdsLogin + "</b>";
                        Conteudo += "<p>Senha: </p><b>" + ControlesUtility.Criptografia.Descriptografar(usuarioEntity.cdsSenha).Trim() + "</b>";

                        Mensagem.Replace("#Conteudo", Conteudo);
                        mailMessage = Mensagem.ToString().Trim();

                        try
                        {
                            mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                        }
                        catch (Exception ex)
                        {
                            LogUtility.LogarErro(ex);
                            ViewBag.Mensagem = "Não foi possível enviar o e-mail! Tente novamente ou contate o administrador do sistema!";
                            //return false;
                        }
                    }

                    usuario.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
                usuario.perfis = ObterListaPerfil();
                usuario.nidPerfilExternoPadrao = ObterPerfilExternoPadrao();
                usuario.cdsSenha = GerarSenhaAleatoria().Trim();
                usuario.empresas = ObterListaEmpresa();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuario); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.Usuario usuario = null;

            try
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();

                usuarioEntity.nidUsuario = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        usuario = new Models.Usuario
                        {
                            nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]),
                            cnmNome = dataTableReader["cnmNome"].ToString(),
                            cdsLogin = dataTableReader["cdsLogin"].ToString(),
                            cdsEmail = dataTableReader["cdsEmail"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]),
                            perfis = ObterListaPerfil(),
                            perfil = new PerfilEntity()
                            {
                                nidPerfil = dataTableReader["nidPerfil"] == DBNull.Value ? 0 : Convert.ToInt64(dataTableReader["nidPerfil"]),
                                cdsPerfil = dataTableReader["cdsPerfil"] == DBNull.Value ? "" : dataTableReader["cdsPerfil"].ToString()
                            },
                            usuarioPerfil = new UsuarioPerfilEntity()
                            {
                                nidUsuarioPerfil = dataTableReader["nidUsuarioPerfil"] == DBNull.Value ? 0 : Convert.ToInt64(dataTableReader["nidUsuarioPerfil"])
                            },
                            nidPerfilExternoPadrao = ObterPerfilExternoPadrao(),
                            empresas = ObterListaEmpresa()
                        };
                        if (dataTableReader["cd_empresa"] == DBNull.Value)
                            usuario.cd_empresa = 0;
                        else
                            usuario.cd_empresa = Convert.ToDecimal(dataTableReader["cd_empresa"]);

                        if (dataTableReader["cdsSenha"] == DBNull.Value)
                            usuario.cdsSenha = GerarSenhaAleatoria().Trim();
                        else
                            usuario.cdsSenha = ControlesUtility.Criptografia.Descriptografar(dataTableReader["cdsSenha"].ToString()).Trim();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (usuario == null)
                return HttpNotFound();
            else
                return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [_3MAuthentication]
        public ActionResult Editar(Models.Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioEntity usuarioEntity = new UsuarioEntity();

                    usuarioEntity.nidUsuario = usuario.nidUsuario;
                    usuarioEntity.cnmNome = usuario.cnmNome;
                    usuarioEntity.cdsLogin = usuario.cdsLogin;
                    usuarioEntity.cdsEmail = usuario.cdsEmail;
                    usuarioEntity.cd_empresa = usuario.cd_empresa;
                    usuarioEntity.bidAtivo = usuario.bidAtivo;
                    usuarioEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    //if (usuario.nidPerfilExternoPadrao == usuario.perfil.nidPerfil)
                    if (!string.IsNullOrEmpty(usuario.cdsSenha))
                        usuarioEntity.cdsSenha = ControlesUtility.Criptografia.Criptografar(usuario.cdsSenha);

                    UsuarioEntity usuarioEntityLogin = new UsuarioEntity();
                    usuarioEntityLogin.cdsLogin = usuario.cdsLogin;
                    DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntityLogin).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            if (usuarioEntityLogin.cdsLogin == dataTableReader["cdsLogin"].ToString() && usuarioEntity.nidUsuario != Convert.ToInt64(dataTableReader["nidUsuario"]))
                            {
                                ViewBag.LoginDuplicado = "Este login já está sendo utilizado. Crie um novo.";
                                usuario.perfis = ObterListaPerfil();
                                usuario.nidPerfilExternoPadrao = ObterPerfilExternoPadrao();
                                usuario.cdsSenha = GerarSenhaAleatoria().Trim();
                                usuario.empresas = ObterListaEmpresa();
                                return View(usuario);
                            }
                        }
                    }

                    new UsuarioData().Alterar(usuarioEntity);

                    UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();

                    usuarioPerfilEntity.nidUsuarioPerfil = usuario.usuarioPerfil.nidUsuarioPerfil;
                    usuarioPerfilEntity.usuario.nidUsuario = usuario.nidUsuario;
                    usuarioPerfilEntity.perfil.nidPerfil = usuario.perfil.nidPerfil;
                    usuarioPerfilEntity.bidAtivo = usuario.bidAtivo;
                    usuarioPerfilEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new UsuarioPerfilData().Alterar(usuarioPerfilEntity);

                    usuario.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
                usuario.perfis = ObterListaPerfil();
                usuario.nidPerfilExternoPadrao = ObterPerfilExternoPadrao();
                usuario.cdsSenha = GerarSenhaAleatoria().Trim();
                usuario.empresas = ObterListaEmpresa();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuario); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.Usuario usuario = null;

            try
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();

                usuarioEntity.nidUsuario = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        usuario = new Models.Usuario
                        {
                            nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]),
                            cnmNome = dataTableReader["cnmNome"].ToString(),
                            cdsLogin = dataTableReader["cdsLogin"].ToString(),
                            cdsEmail = dataTableReader["cdsEmail"].ToString(),
                            bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"])
                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (usuario == null)
                return HttpNotFound();
            else
                return View(usuario);
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.Usuario usuario = new Models.Usuario();
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioEntity usuarioEntity = new UsuarioEntity();

                    usuarioEntity.nidUsuario = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    usuarioEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new UsuarioData().Excluir(usuarioEntity);

                    usuario.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(usuario);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        protected Int64 ObterPerfilExternoPadrao()
        {
            Int64 nidPerfilExternoPadrao = 0;

            try
            {
                PerfilEntity perfilEntity = new PerfilEntity();

                perfilEntity.cdsPerfil = Utility.ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
                DataTableReader dataTableReader = new PerfilData().ObterLista(perfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        nidPerfilExternoPadrao = Convert.ToInt64(dataTableReader["nidPerfil"]);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return nidPerfilExternoPadrao;
        }

        protected string GerarSenhaAleatoria()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[12];
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString.ToString();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(Models.UsuarioLogin usuarioLogin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (ValidarLogin(usuarioLogin) == true)
                    {
                        if (ViewBag.Mensagem == "Trocar senha!")
                            return RedirectToAction("TrocarSenha", "Usuario");
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuarioLogin); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult FazerLogin(Models.UsuarioLogin userLogin)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            

            Models.UsuarioLogin usuarioLogin = new Models.UsuarioLogin();

            try
            {
                
                usuarioLogin.cdsLogin = userLogin.cdsLogin;
                usuarioLogin.cdsSenha = userLogin.cdsSenha;
                usuarioLogin.token = userLogin.token;

                if (ModelState.IsValid)
                {
                    if (ValidarLogin(usuarioLogin) == true)
                    {
                        if (ViewBag.Mensagem == "Trocar senha!")
                        {
                            jsonResult.Add("Status", "Erro");
                            jsonResult.Add("Mensagem", "TrocarSenha!");
                            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                            jsonList.MaxJsonLength = int.MaxValue;
                            return jsonList;
                        }
                        //return new JsonResult.Add("Sucesso");
                        else
                        {
                            jsonResult.Add("Status", "Sucesso");
                            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
                            jsonList.MaxJsonLength = int.MaxValue;
                            return jsonList;
                        }
                        //return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            jsonResult.Add("Status", "Erro");
            jsonResult.Add("Mensagem", "Usuário ou senha Incorreta!");
            var jsonListErro = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonListErro.MaxJsonLength = int.MaxValue;
            return jsonListErro;

            //return View(usuarioLogin); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        protected bool ValidarLogin(Models.UsuarioLogin usuarioLogin)
        {
            try
            {
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.usuario.cdsLogin = usuarioLogin.cdsLogin.Trim();

                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioPerfilEntity.usuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        //Usuario
                        usuarioPerfilEntity.usuario.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioPerfilEntity.usuario.cnmNome = dataTableReader["cnmNome"].ToString();
                        usuarioPerfilEntity.usuario.cdsLogin = dataTableReader["cdsLogin"].ToString();
                        usuarioPerfilEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        usuarioPerfilEntity.usuario.cdsSenha = dataTableReader["cdsSenha"].ToString().Trim();
                        usuarioPerfilEntity.usuario.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                            usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);
                        //Perfil
                        usuarioPerfilEntity.perfil.nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]);
                        usuarioPerfilEntity.perfil.cdsPerfil = dataTableReader["cdsPerfil"].ToString();
                        usuarioPerfilEntity.perfil.ccdPerfil = int.Parse(dataTableReader["ccdPerfil"].ToString());
                        //Verifica se é Externo
                        //usuarioPerfilEntity.bidPermitirTrocarSenha = (usuarioPerfilEntity.perfil.nidPerfil == ObterPerfilExternoPadrao() ? true : false);
                        if (usuarioPerfilEntity.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) ||
                        usuarioPerfilEntity.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica) ||
                        usuarioPerfilEntity.perfil.nidPerfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente))
                            usuarioPerfilEntity.bidPermitirTrocarSenha = true;
                        else
                            usuarioPerfilEntity.bidPermitirTrocarSenha = false;
                    }
                }
                else
                {
                    ViewBag.Mensagem = "Usuário ou senha inválidos.";
                    return false;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioPerfilEntity.usuario.bidAtivo == false)
                {
                    ViewBag.Mensagem = "O usuário está inativo no Portal Web, contate o Administrador do sistema.";
                    return false;
                }
                else if (string.IsNullOrEmpty(usuarioPerfilEntity.usuario.cdsSenha))
                {
                    string ADDomain = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADDomain);
                    string ADUser = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADUser);
                    string ADPassword = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADPassword);
                    // Senha no banco em branco - Usuário AD (Validar)
                    using (PrincipalContext principalContext = new PrincipalContext(contextType: ContextType.Domain, name: ADDomain, userName: ADUser, password: ADPassword))
                    {

                        if (principalContext.ValidateCredentials(usuarioLogin.cdsLogin.Trim(), usuarioLogin.cdsSenha.Trim(), ContextOptions.Negotiate) == false)
                        {
                            ViewBag.Mensagem = "Credenciais inválidas!";
                            return false;
                        }
                    }
                }
                else
                {
                    // Senha no banco gravada - Usuário Externo
                    if (ControlesUtility.Criptografia.Criptografar(usuarioLogin.cdsSenha.Trim()) != usuarioPerfilEntity.usuario.cdsSenha)
                    {
                        ViewBag.Mensagem = "Usuário ou senha inválidos.";
                        return false;
                    }
                    else if (usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno != null)
                    {
                        double diasTrocaSenhaExterno = Convert.ToDouble("0" + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.diasTrocaSenhaExterno));
                        DateTime dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(Convert.ToDateTime(usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno).ToShortDateString());

                        if (dtmDataHoraTrocaLoginExterno.AddDays(diasTrocaSenhaExterno) <= DateTime.Now)
                            ViewBag.Mensagem = "Senha vencida. Altere sua senha para renovar o usuário.";
                    }
                }

                if (usuarioPerfilEntity.perfil.ccdPerfil == (int)ControlesUtility.Enumeradores.Perfil.Cliente) //Cliente
                {
                    List<UsuarioClienteEntity> usuclientes = new List<UsuarioClienteEntity>();
                    UsuarioClienteEntity usuClienteEntity = new UsuarioClienteEntity();
                    usuClienteEntity.usuario.nidUsuario = usuarioPerfilEntity.usuario.nidUsuario;
                    DataTableReader dtCliente = new UsuarioClienteData().ObterLista(usuClienteEntity).CreateDataReader();

                    if (!dtCliente.HasRows)
                    {
                        ViewBag.Mensagem = "Usuário não possui cliente vinculado, contate o administrador.";
                        return false;
                    }

                    if (dtCliente != null)
                    {
                        dtCliente.Dispose();
                        dtCliente = null;
                    }
                }

                usuarioPerfilEntity.usuario.cdsSenha = string.Empty;

                CurrentUser = usuarioPerfilEntity;
                CurrentUser.token = usuarioLogin.token;

                return true;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public ActionResult LogOff()
        {
            CurrentUser = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RecuperaSenha()
        {
            ViewBag.cdsPerfilExternoPadrao = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperaSenha(Models.UsuarioLoginRecuperar usuarioLoginRecuperar)
        {
            try
            {
                ViewBag.cdsPerfilExternoPadrao = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);

                if (ModelState.IsValid)
                {
                    if (RecuperarSenhaUsuario(usuarioLoginRecuperar) == true)
                        //return RedirectToAction("Login");
                        ViewBag.MensagemOK = "E-mail enviado com sucesso!";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuarioLoginRecuperar); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        protected bool RecuperarSenhaUsuario(Models.UsuarioLoginRecuperar usuarioLogin)
        {
            try
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                usuarioEntity.cdsEmail = usuarioLogin.cdsEmail;

                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
                        usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
                        usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString().Trim();
                        usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                            usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);

                        //if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
                        if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) &&
                            Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica) &&
                            Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente))
                        {
                            //ViewBag.Mensagem = "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
                            ViewBag.Mensagem = "Usuário não pertence a nenhum perfil EXTERNO...";
                            return false;
                        }
                    }
                }
                else
                {
                    ViewBag.Mensagem = "E-mail não encontrado!";
                    return false;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioEntity.bidAtivo == false)
                {
                    ViewBag.Mensagem = "O usuário está inativo no Portal Web, contate o Administrador do sistema.";
                    return false;
                }
                else if (string.IsNullOrEmpty(usuarioEntity.cdsSenha))
                {
                    ViewBag.Mensagem = "Usuário não possui uma senha cadastrada! Contate o administrador e informe o ocorrido!";
                    return false;
                }
                else
                {
                    //Cria uma chave para trocar senha
                    usuarioEntity.ccdChaveAcessoTrocarSenha = Guid.NewGuid().ToString().ToUpper();
                    usuarioEntity.nidUsuarioAtualizacao = usuarioEntity.nidUsuario;

                    UsuarioData usuarioData = new UsuarioData();
                    usuarioData.Alterar(usuarioEntity);

                    // Envia a requisição de troca de senha por e-mail
                    MailSender mailSender = new MailSender();

                    string mailTo = usuarioEntity.cdsEmail;
                    string mailSubject = "3M.Comodato - Recuperar Senha";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var Mensagem = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    URLSite += "/Usuario/TrocarSenhaViaChave?guid=" + usuarioEntity.ccdChaveAcessoTrocarSenha;

                    Conteudo += "<p>Prezado(a) usuário(a)</p>";
                    Conteudo += "<p>Conforme solicitado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ". Segue LINK para a <b>Troca de Senha</b> do Sistema <b>3M.Comodato</b>.</p>";
                    Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegado WEB:</p>";
                    Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                    Mensagem.Replace("#Conteudo", Conteudo);
                    mailMessage = Mensagem.ToString();

                    try
                    {
                        mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogarErro(ex);
                        ViewBag.Mensagem = "Não foi possível envia o e-mail! Tente novamente ou contate o administrador e informe o ocorrido!";
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public ActionResult TrocarSenha()
        {
            Models.UsuarioLoginTrocarSenha usuarioLoginTrocarSenha = new Models.UsuarioLoginTrocarSenha();

            try
            {
                if (CurrentUser.bidPermitirTrocarSenha == false)
                    return RedirectToAction("Index", "Home");

                usuarioLoginTrocarSenha.cnmNome = CurrentUser.usuario.cnmNome;
                usuarioLoginTrocarSenha.cdsPerfil = "(" + CurrentUser.perfil.cdsPerfil + ")";
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuarioLoginTrocarSenha);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenha(Models.UsuarioLoginTrocarSenha usuarioLoginTrocarSenha)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (TrocarSenhaUsuario(usuarioLoginTrocarSenha) == true)
                        ViewBag.MensagemOK = "Senha trocada com sucesso!";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuarioLoginTrocarSenha); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        protected bool TrocarSenhaUsuario(Models.UsuarioLoginTrocarSenha usuarioLoginTrocarSenha)
        {
            try
            {
                if (!ValidaRegraSenha(usuarioLoginTrocarSenha.cdsSenha))
                {
                    //ViewBag.Mensagem = "A senha deve possuir no mínimo 12 caracteres, ao menos 1 caractere maiúsculo, minúsculo, numérico e caractere especial.";
                    return false;
                }

                UsuarioEntity usuarioEntity = new UsuarioEntity();
                usuarioEntity.nidUsuario = CurrentUser.usuario.nidUsuario;

                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
                        usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
                        usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString().Trim();
                        usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                            usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);

                        //if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
                        if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) &&
                            Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica) &&
                            Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente))
                        {
                            //ViewBag.Mensagem = "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
                            ViewBag.Mensagem = "Usuário não pertence a nenhum perfil EXTERNO...";
                            return false;
                        }
                    }
                }
                else
                {
                    ViewBag.Mensagem = "Usuário ou senha inválidos.";
                    return false;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioEntity.bidAtivo == false)
                {
                    ViewBag.Mensagem = "O usuário está inativo no Portal Web, contate o Administrador do sistema.";
                    return false;
                }
                else if (ControlesUtility.Criptografia.Criptografar(usuarioLoginTrocarSenha.cdsSenhaAtual).Trim() != usuarioEntity.cdsSenha.Trim())
                {
                    ViewBag.Mensagem = "Usuário ou senha inválidos.";
                    return false;
                }

                UsuarioData usuarioData = new UsuarioData();
                usuarioEntity.cdsSenha = ControlesUtility.Criptografia.Criptografar(usuarioLoginTrocarSenha.cdsSenha).Trim();
                usuarioEntity.dtmDataHoraTrocaLoginExterno = DateTime.Now;
                usuarioEntity.ccdChaveAcessoTrocarSenha = string.Empty;
                usuarioEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                usuarioData.Alterar(usuarioEntity);

                return true;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public ActionResult TrocarSenhaViaChave(string guid)
        {
            Models.UsuarioLoginTrocarSenhaViaChave usuarioLoginTrocarSenhaViaChave = new Models.UsuarioLoginTrocarSenhaViaChave();

            try
            {
                if (string.IsNullOrEmpty(guid))
                    ViewBag.Mensagem = "Chave inválida!";
                else
                {
                    //if (guid.Substring(guid.Length - 1, 1) != "=")
                    //    guid += "=";

                    UsuarioEntity usuarioEntity = new UsuarioEntity();
                    usuarioEntity.ccdChaveAcessoTrocarSenha = guid;
                    DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                            usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
                            usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
                            usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
                            usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString().Trim();
                            usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
                            if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                                usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);

                            //if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
                            if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) &&
                                Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica) &&
                                Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente))
                            {
                                //ViewBag.Mensagem = "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
                                ViewBag.Mensagem = "Usuário não pertence a nenhum perfil EXTERNO...";
                            }

                            usuarioLoginTrocarSenhaViaChave.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                            usuarioLoginTrocarSenhaViaChave.ccdChaveAcessoTrocarSenha = dataTableReader["ccdChaveAcessoTrocarSenha"].ToString();
                            usuarioLoginTrocarSenhaViaChave.cnmNome = dataTableReader["cnmNome"].ToString();
                            usuarioLoginTrocarSenhaViaChave.cdsPerfil = "(" + dataTableReader["cdsPerfil"].ToString() + ")";
                        }
                    }
                    else
                    {
                        ViewBag.Mensagem = "Usuário ou senha inválidos.";
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuarioLoginTrocarSenhaViaChave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrocarSenhaViaChave(Models.UsuarioLoginTrocarSenhaViaChave usuarioLoginTrocarSenhaViaChave)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (TrocarSenhaUsuarioViaChave(usuarioLoginTrocarSenhaViaChave) == true)
                        ViewBag.MensagemOK = "Senha trocada com sucesso!";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(usuarioLoginTrocarSenhaViaChave); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        protected bool TrocarSenhaUsuarioViaChave(Models.UsuarioLoginTrocarSenhaViaChave usuarioLoginTrocarSenhaViaChave)
        {
            try
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                usuarioEntity.nidUsuario = usuarioLoginTrocarSenhaViaChave.nidUsuario;
                usuarioEntity.ccdChaveAcessoTrocarSenha = usuarioLoginTrocarSenhaViaChave.ccdChaveAcessoTrocarSenha;

                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
                        usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
                        usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString().Trim();
                        usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                            usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);

                        //if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
                        if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira) &&
                            Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica) &&
                            Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Cliente))
                        {
                            //ViewBag.Mensagem = "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
                            ViewBag.Mensagem = "Usuário não pertence a nenhum perfil EXTERNO...";
                            return false;
                        }
                    }
                }
                else
                {
                    ViewBag.Mensagem = "Usuário ou senha inválidos.";
                    return false;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioEntity.bidAtivo == false)
                {
                    ViewBag.Mensagem = "O usuário está inativo no Portal Web, contate o Administrador do sistema.";
                    return false;
                }

                if (!ValidaRegraSenha(usuarioLoginTrocarSenhaViaChave.cdsSenha))
                {
                    //ViewBag.Mensagem = "A senha deve possuir no mínimo 12 caracteres, ao menos 1 caractere maiúsculo, minúsculo, numérico e caractere especial.";
                    return false;
                }

                UsuarioData usuarioData = new UsuarioData();
                usuarioEntity.cdsSenha = ControlesUtility.Criptografia.Criptografar(usuarioLoginTrocarSenhaViaChave.cdsSenha).Trim();
                usuarioEntity.dtmDataHoraTrocaLoginExterno = DateTime.Now;
                usuarioEntity.ccdChaveAcessoTrocarSenha = string.Empty;
                if (CurrentUser == null)
                    usuarioEntity.nidUsuarioAtualizacao = usuarioLoginTrocarSenhaViaChave.nidUsuario;
                else
                    usuarioEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                usuarioData.Alterar(usuarioEntity);

                return true;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        private bool ValidaRegraSenha(string senha)
        {
            return senha.Length >= 12 && senha.Any(char.IsUpper) && senha.Any(char.IsLower) && senha.Any(char.IsNumber) &&
                senha.Any(c => @"\|!#$%&/()=?»«@£§€{}.-;'<>_,".Contains(c));
        }

    }
}