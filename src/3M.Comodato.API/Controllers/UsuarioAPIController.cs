using _3M.Comodato.API.Service;
using _3M.Comodato.Data;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Utility;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/UsuarioAPI")]
    public class UsuarioAPIController : BaseAPIController
    {
        [HttpPost]
        [Route("GetToken")]
        [AllowAnonymous]
        public IHttpActionResult GetToken(UsuarioEntity usuarioEntity)
        {
            try
            {
                JObject JO = new JObject();
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.usuario.cdsLogin = usuarioEntity.cdsLogin;

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
                        usuarioPerfilEntity.usuario.cdsSenha = dataTableReader["cdsSenha"].ToString();
                        usuarioPerfilEntity.usuario.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                        {
                            usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);
                        }
                        //Perfil
                        usuarioPerfilEntity.perfil.nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]);
                        usuarioPerfilEntity.perfil.cdsPerfil = dataTableReader["cdsPerfil"].ToString();
                        //Verifica se é Externo
                        usuarioPerfilEntity.bidPermitirTrocarSenha = (usuarioPerfilEntity.perfil.nidPerfil == ObterPerfilExternoPadrao() ? true : false);

                    }
                }
                else
                {
                    //return Request.CreateResponse(HttpStatusCode.BadRequest, "Login inválido!");
                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário ou senha inválidos.' }", Formatting.None));
                    return Ok(JO);

                    //ViewBag.Mensagem = "Login inválido!";
                    //return false;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioPerfilEntity.perfil.nidPerfil != ObterPerfilExternoPadrao())
                {
                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Acesso não permitido para o seu perfil. Permitido somente acesso dos Técnicos.' }", Formatting.None));
                    return Ok(JO);
                }

                if (usuarioPerfilEntity.usuario.bidAtivo == false)
                {
                    //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário inativo no sistema!");
                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'O usuário está inativo no Portal Web, contate o Administrador do sistema.' }", Formatting.None));
                    return Ok(JO);

                    //ViewBag.Mensagem = "Usuário inativo no sistema!";
                    //return false;
                }
                else if (string.IsNullOrEmpty(usuarioPerfilEntity.usuario.cdsSenha))
                {
                    string ADDomain = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADDomain);
                    string ADUser = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADUser);
                    string ADPassword = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADPassword);
                    // Senha no banco em branco - Usuário AD (Validar)
                    using (PrincipalContext principalContext = new PrincipalContext(contextType: ContextType.Domain, name: ADDomain, userName: ADUser, password: ADPassword))
                    {
                        // Erasmo - GSW - Testes - Para dar  ByPass na Senha , basta tratar ou pular este IF.
                        if (principalContext.ValidateCredentials(usuarioEntity.cdsLogin, usuarioEntity.cdsSenha) == false)
                        {
                            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Credenciais inválidas!");
                            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Credenciais inválidas!' }", Formatting.None));
                            return Ok(JO);

                            //ViewBag.Mensagem = "Credenciais inválidas!";
                            //return false;
                        }
                    }
                }
                else
                {
                    // Senha no banco gravada - Usuário Externo
                    if (ControlesUtility.Criptografia.Criptografar(usuarioEntity.cdsSenha) != usuarioPerfilEntity.usuario.cdsSenha)
                    {
                        //return Request.CreateResponse(HttpStatusCode.BadRequest, "Senha inválida!");
                        JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário ou senha inválidos.' }", Formatting.None));
                        return Ok(JO);

                        //ViewBag.Mensagem = "Senha inválida!";
                        //return false;
                    }
                    else if (usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno != null)
                    {
                        double diasTrocaSenhaExterno = Convert.ToDouble("0" + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.diasTrocaSenhaExterno));
                        DateTime dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(Convert.ToDateTime(usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno).ToShortDateString());

                        if (dtmDataHoraTrocaLoginExterno.AddDays(diasTrocaSenhaExterno) <= DateTime.Now)
                        {
                            usuarioPerfilEntity.usuario.cdsSenha = string.Empty;
                            //UsuarioPerfilEntity CurrentUser = usuarioPerfilEntity;
                            //return Request.CreateResponse(HttpStatusCode.OK, new { CurrentUser = usuarioPerfilEntity, Acao = "Trocar senha!" });
                            JO.Add("CurrentUser", JsonConvert.SerializeObject(usuarioPerfilEntity, Formatting.None));
                            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Senha vencida. Altere sua senha para renovar o usuário.' }", Formatting.None));
                            return Ok(JO);

                            //ViewBag.Mensagem = "Trocar senha!";
                        }
                    }
                }

                usuarioPerfilEntity.usuario.cdsSenha = string.Empty;
                //UsuarioPerfilEntity CurrentUser = usuarioPerfilEntity;
                //return Request.CreateResponse(HttpStatusCode.OK, new { CurrentUser = usuarioPerfilEntity });

                var jwtToken = TokenService.GenerateToken(usuarioPerfilEntity);

                //string jwtToken = TokenManager

                JO.Add("CurrentUser", JsonConvert.SerializeObject(usuarioPerfilEntity, Formatting.None));
                JO.Add("Token", JsonConvert.SerializeObject(jwtToken));

                return Ok(JO);

                //return true;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return BadRequest(ex.Message);

            }

            //return Request.CreateResponse(HttpStatusCode.BadRequest);
            return BadRequest();
            //return false;
        }

        [HttpPost]
        [Route("ValidarLogin")]
        [AllowAnonymous]
        public IHttpActionResult ValidarLogin(UsuarioEntity usuarioEntity)
        {
            try
            {
                JObject JO = new JObject();
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.usuario.cdsLogin = usuarioEntity.cdsLogin;

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
                        usuarioPerfilEntity.usuario.cdsSenha = dataTableReader["cdsSenha"].ToString();
                        usuarioPerfilEntity.usuario.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                        {
                            usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);
                        }
                        //Perfil
                        usuarioPerfilEntity.perfil.nidPerfil = Convert.ToInt64(dataTableReader["nidPerfil"]);
                        usuarioPerfilEntity.perfil.cdsPerfil = dataTableReader["cdsPerfil"].ToString();
                        //Verifica se é Externo
                        usuarioPerfilEntity.bidPermitirTrocarSenha = (usuarioPerfilEntity.perfil.nidPerfil == ObterPerfilExternoPadrao() ? true : false);

                    }
                }
                else
                {
                    //return Request.CreateResponse(HttpStatusCode.BadRequest, "Login inválido!");
                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário ou senha inválidos.' }", Formatting.None));
                    return Ok(JO);

                    //ViewBag.Mensagem = "Login inválido!";
                    //return false;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioPerfilEntity.perfil.nidPerfil != ObterPerfilExternoPadrao())
                {
                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Acesso não permitido para o seu perfil. Permitido somente acesso dos Técnicos.' }", Formatting.None));
                    return Ok(JO);
                }

                if (usuarioPerfilEntity.usuario.bidAtivo == false)
                {
                    //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário inativo no sistema!");
                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'O usuário está inativo no Portal Web, contate o Administrador do sistema.' }", Formatting.None));
                    return Ok(JO);

                    //ViewBag.Mensagem = "Usuário inativo no sistema!";
                    //return false;
                }
                else if (string.IsNullOrEmpty(usuarioPerfilEntity.usuario.cdsSenha))
                {
                    string ADDomain = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADDomain);
                    string ADUser = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADUser);
                    string ADPassword = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.ADPassword);
                    // Senha no banco em branco - Usuário AD (Validar)
                    using (PrincipalContext principalContext = new PrincipalContext(contextType: ContextType.Domain, name: ADDomain, userName: ADUser, password: ADPassword))
                    {
                        // Erasmo - GSW - Testes - Para dar  ByPass na Senha , basta tratar ou pular este IF.
                        if (principalContext.ValidateCredentials(usuarioEntity.cdsLogin, usuarioEntity.cdsSenha) == false)
                        {
                            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Credenciais inválidas!");
                            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Credenciais inválidas!' }", Formatting.None));
                            return Ok(JO);

                            //ViewBag.Mensagem = "Credenciais inválidas!";
                            //return false;
                        }
                    }
                }
                else
                {
                    // Senha no banco gravada - Usuário Externo
                    if (ControlesUtility.Criptografia.Criptografar(usuarioEntity.cdsSenha) != usuarioPerfilEntity.usuario.cdsSenha)
                    {
                        //return Request.CreateResponse(HttpStatusCode.BadRequest, "Senha inválida!");
                        JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário ou senha inválidos.' }", Formatting.None));
                        return Ok(JO);

                        //ViewBag.Mensagem = "Senha inválida!";
                        //return false;
                    }
                    else if (usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno != null)
                    {
                        double diasTrocaSenhaExterno = Convert.ToDouble("0" + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.diasTrocaSenhaExterno));
                        DateTime dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(Convert.ToDateTime(usuarioPerfilEntity.usuario.dtmDataHoraTrocaLoginExterno).ToShortDateString());

                        if (dtmDataHoraTrocaLoginExterno.AddDays(diasTrocaSenhaExterno) <= DateTime.Now)
                        {
                            usuarioPerfilEntity.usuario.cdsSenha = string.Empty;
                            //UsuarioPerfilEntity CurrentUser = usuarioPerfilEntity;
                            //return Request.CreateResponse(HttpStatusCode.OK, new { CurrentUser = usuarioPerfilEntity, Acao = "Trocar senha!" });
                            JO.Add("CurrentUser", JsonConvert.SerializeObject(usuarioPerfilEntity, Formatting.None));
                            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Senha vencida. Altere sua senha para renovar o usuário.' }", Formatting.None));
                            return Ok(JO);

                            //ViewBag.Mensagem = "Trocar senha!";
                        }
                    }
                }

                usuarioPerfilEntity.usuario.cdsSenha = string.Empty;
                //UsuarioPerfilEntity CurrentUser = usuarioPerfilEntity;
                //return Request.CreateResponse(HttpStatusCode.OK, new { CurrentUser = usuarioPerfilEntity });

                var jwtToken = TokenService.GenerateToken(usuarioPerfilEntity);

                //string jwtToken = TokenManager

                JO.Add("CurrentUser", JsonConvert.SerializeObject(usuarioPerfilEntity, Formatting.None));
                //JO.Add("Token", JsonConvert.SerializeObject(jwtToken));
                
                return Ok(JO);

                //return true;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                return BadRequest(ex.Message);

            }

            //return Request.CreateResponse(HttpStatusCode.BadRequest);
            return BadRequest();
            //return false;
        }

        private async Task<string> GerarJwt(UsuarioPerfilEntity usuarioLogado)
        {
            //var secaoJwt = _configuration.GetSection("JwtToken");
            var key = Encoding.ASCII.GetBytes("my_secret_key_12345");
            var issuer = ConfigurationManager.AppSettings["URLApi"].ToString();
            var ExpireHours = 12;
            
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, usuarioLogado.usuario.cdsEmail.ToString()),
                    new Claim(ClaimTypes.Name, usuarioLogado.usuario.cnmNome.ToString())
                }),
                Issuer = issuer,
                Expires = DateTime.UtcNow.AddHours(ExpireHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            
            return await Task.FromResult(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));
        }

        public Object GetToken(UsuarioPerfilEntity usuarioPerfilEntity)
        {
            string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
            var issuer = "http://mysite.com";  //normally this will be your site URL    

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("valid", "1"));
            permClaims.Add(new Claim("userid", usuarioPerfilEntity.usuario.nidUsuario.ToString()));
            permClaims.Add(new Claim("name", usuarioPerfilEntity.usuario.cdsLogin.ToString()));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt_token;
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

        //[HttpPost]
        //[Route("RecuperarSenha")]
        //public IHttpActionResult RecuperarSenha(UsuarioEntity usuarioEntity)
        //{
        //    try
        //    {
        //        JObject JO = new JObject();
        //        UsuarioEntity usuarioEntity = new UsuarioEntity();
        //        usuarioEntity.cdsEmail = usuarioEntity.cdsEmail;

        //        DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

        //        if (dataTableReader.HasRows)
        //        {
        //            if (dataTableReader.Read())
        //            {
        //                usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
        //                usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
        //                usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
        //                usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
        //                usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString();
        //                usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
        //                if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
        //                {
        //                    usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);
        //                }

        //                if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
        //                {
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao));
        //                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao) + "' }", Formatting.None));
        //                    return Ok(JO);

        //                    ViewBag.Mensagem = "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
        //                    return false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, "E-mail não encontrado!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'E-mail não encontrado!' }", Formatting.None));
        //            return Ok(JO);

        //            ViewBag.Mensagem = "E-mail não encontrado!";
        //            return false;
        //        }

        //        if (dataTableReader != null)
        //        {
        //            dataTableReader.Dispose();
        //            dataTableReader = null;
        //        }

        //        if (usuarioEntity.bidAtivo == false)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário inativo no sistema!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário inativo no sistema!' }", Formatting.None));
        //            return Ok(JO);

        //            ViewBag.Mensagem = "Usuário inativo no sistema!";
        //            return false;
        //        }
        //        else if (string.IsNullOrEmpty(usuarioEntity.cdsSenha))
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário não possui uma senha cadastrada! Contate o administrador e informe o ocorrido!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não possui uma senha cadastrada! Contate o administrador e informe o ocorrido!' }", Formatting.None));
        //            return Ok(JO);

        //            ViewBag.Mensagem = "Usuário não possui uma senha cadastrada! Contate o administrador e informe o ocorrido!";
        //            return false;
        //        }
        //        else
        //        {
        //            Cria uma chave para trocar senha
        //            usuarioEntity.ccdChaveAcessoTrocarSenha = Guid.NewGuid().ToString().ToUpper();
        //            usuarioEntity.nidUsuarioAtualizacao = 0; //CurrentUser.usuario.nidUsuario;

        //            UsuarioData usuarioData = new UsuarioData();
        //            usuarioData.Alterar(usuarioEntity);

        //            Envia a requisição de troca de senha por e - mail
        //            MailSender mailSender = new MailSender();

        //            string mailTo = usuarioEntity.cdsEmail;
        //            string mailSubject = "3M.Comodato - Recuperar Senha";
        //            string mailMessage = string.Empty;
        //            System.Net.Mail.Attachment Attachments = null;
        //            string mailCopy = null;

        //            var Mensagem = mailSender.GetConteudoHTML("EmailCorpo.html");
        //            string Conteudo = string.Empty;
        //            string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

        //            URLSite += "/Usuario/TrocarSenhaViaChave?guid=" + usuarioEntity.ccdChaveAcessoTrocarSenha;

        //            Conteudo += "<p>Prezado(a) usuário(a)</p>";
        //            Conteudo += "<p>Conforme solicitado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ". Segue LINK para a <b>Troca de Senha</b> do Sistema <b>3M.Comodato</b>.</p>";
        //            Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB:</p>";
        //            Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

        //            Mensagem.Replace("#Conteudo", Conteudo);
        //            mailMessage = Mensagem.ToString();

        //            try
        //            {
        //                mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtility.LogarErro(ex);
        //                return Request.CreateResponse(HttpStatusCode.BadRequest, "Não foi possível envia o e-mail! Tente novamente ou contate o administrador e informe o ocorrido!");
        //                JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Não foi possível envia o e-mail! Tente novamente ou contate o administrador e informe o ocorrido!' }", Formatting.None));
        //                return Ok(JO);

        //                ViewBag.Mensagem = "Não foi possível envia o e-mail! Tente novamente ou contate o administrador e informe o ocorrido!";
        //                return false;
        //            }
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    return BadRequest();
        //}

        //[HttpPost]
        //[Route("TrocarSenha")]
        //public IHttpActionResult TrocarSenha(UsuarioEntity usuarioLoginTrocarSenha)
        //{
        //    try
        //    {
        //        JObject JO = new JObject();
        //        UsuarioEntity usuarioEntity = new UsuarioEntity();
        //        usuarioEntity.nidUsuario = usuarioLoginTrocarSenha.nidUsuario;

        //        DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

        //        if (dataTableReader.HasRows)
        //        {
        //            if (dataTableReader.Read())
        //            {
        //                usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
        //                usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
        //                usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
        //                usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
        //                usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString();
        //                usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
        //                if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
        //                {
        //                    usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);
        //                }

        //                if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
        //                {
        //                    //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao));
        //                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao) + "' }", Formatting.None));
        //                    return Ok(JO);

        //                    //ViewBag.Mensagem = "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
        //                    //return false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário não encontrado!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não encontrado!' }", Formatting.None));
        //            return Ok(JO);

        //            //ViewBag.Mensagem = "Usuário não encontrado!";
        //            //return false;
        //        }

        //        if (dataTableReader != null)
        //        {
        //            dataTableReader.Dispose();
        //            dataTableReader = null;
        //        }

        //        if (usuarioEntity.bidAtivo == false)
        //        {
        //            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário inativo no sistema!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário inativo no sistema!' }", Formatting.None));
        //            return Ok(JO);

        //            //ViewBag.Mensagem = "Usuário inativo no sistema!";
        //            //return false;
        //        }
        //        else if (ControlesUtility.Criptografia.Criptografar(usuarioLoginTrocarSenha.cdsSenha) != usuarioEntity.cdsSenha)
        //        {
        //            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Senha atual inválida!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Senha atual inválida!' }", Formatting.None));
        //            return Ok(JO);

        //            //ViewBag.Mensagem = "Senha atual inválida!";
        //            //return false;
        //        }

        //        UsuarioData usuarioData = new UsuarioData();
        //        usuarioEntity.cdsSenha = ControlesUtility.Criptografia.Criptografar(usuarioLoginTrocarSenha.cdsSenha);
        //        usuarioEntity.dtmDataHoraTrocaLoginExterno = DateTime.Now;
        //        usuarioEntity.ccdChaveAcessoTrocarSenha = string.Empty;
        //        usuarioEntity.nidUsuarioAtualizacao = usuarioLoginTrocarSenha.nidUsuario;

        //        usuarioData.Alterar(usuarioEntity);

        //        //return Request.CreateResponse(HttpStatusCode.OK);
        //        return Ok();
        //        //return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    //return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    return BadRequest();
        //    //return false;
        //}

        //[HttpPost]
        //[Route("TrocarSenhaViaChave")]
        //protected IHttpActionResult TrocarSenhaViaChave(UsuarioEntity usuarioLoginTrocarSenhaViaChave)
        //{
        //    try
        //    {
        //        JObject JO = new JObject();
        //        UsuarioEntity usuarioEntity = new UsuarioEntity();
        //        usuarioEntity.nidUsuario = usuarioLoginTrocarSenhaViaChave.nidUsuario;
        //        usuarioEntity.ccdChaveAcessoTrocarSenha = usuarioLoginTrocarSenhaViaChave.ccdChaveAcessoTrocarSenha;

        //        DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

        //        if (dataTableReader.HasRows)
        //        {
        //            if (dataTableReader.Read())
        //            {
        //                usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
        //                usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
        //                usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
        //                usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
        //                usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString();
        //                usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);
        //                if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
        //                {
        //                    usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);
        //                }

        //                if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
        //                {
        //                    //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao));
        //                    JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao) + "' }", Formatting.None));
        //                    return Ok(JO);

        //                    //ViewBag.Mensagem = "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao);
        //                    //return false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário não encontrado!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não encontrado!' }", Formatting.None));
        //            return Ok(JO);

        //            //ViewBag.Mensagem = "Usuário não encontrado!";
        //            //return false;
        //        }

        //        if (dataTableReader != null)
        //        {
        //            dataTableReader.Dispose();
        //            dataTableReader = null;
        //        }

        //        if (usuarioEntity.bidAtivo == false)
        //        {
        //            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Usuário inativo no sistema!");
        //            JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário inativo no sistema!' }", Formatting.None));
        //            return Ok(JO);

        //            //ViewBag.Mensagem = "Usuário inativo no sistema!";
        //            //return false;
        //        }

        //        UsuarioData usuarioData = new UsuarioData();
        //        usuarioEntity.cdsSenha = ControlesUtility.Criptografia.Criptografar(usuarioLoginTrocarSenhaViaChave.cdsSenha);
        //        usuarioEntity.dtmDataHoraTrocaLoginExterno = DateTime.Now;
        //        usuarioEntity.ccdChaveAcessoTrocarSenha = string.Empty;
        //        usuarioEntity.nidUsuarioAtualizacao = usuarioLoginTrocarSenhaViaChave.nidUsuario;

        //        usuarioData.Alterar(usuarioEntity);

        //        //return Request.CreateResponse(HttpStatusCode.OK);
        //        return Ok();
        //        //return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtility.LogarErro(ex);
        //        throw ex;
        //    }

        //    //return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    return BadRequest();
        //    //return false;
        //}

        [HttpPost]
        [Route("RecuperarSenha")]
        [AllowAnonymous]
        public IHttpActionResult RecuperarSenha(UsuarioEntity usuarioEntity)
        {
            try
            {
                JObject JO;
                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
                        usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
                        usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString();
                        usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);

                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                            usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);

                        if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
                        {
                            //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao) + "' }", Formatting.None));
                            //JO = JObjectMessage(false, "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao));
                            JO = JObjectMessage(false, "Acesso não permitido para o seu perfil. Permitido somente acesso dos Técnicos.");
                            return Ok(JO);
                        }
                    }
                }
                else
                {
                    //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'E-mail não encontrado!' }", Formatting.None));
                    JO = JObjectMessage(false, "Usuário não encontrado. Não é possível recuperar a senha.");
                    return Ok(JO);
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioEntity.bidAtivo == false)
                {
                    //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário inativo no sistema!' }", Formatting.None));
                    JO = JObjectMessage(false, "O usuário está inativo no Portal Web, contate o Administrador do sistema.");
                    return Ok(JO);
                }
                else if (string.IsNullOrEmpty(usuarioEntity.cdsSenha))
                {
                    //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não possui uma senha cadastrada! Contate o administrador e informe o ocorrido!' }", Formatting.None));
                    JO = JObjectMessage(false, "Usuário não possui uma senha cadastrada! Contate o administrador e informe o ocorrido!");
                    return Ok(JO);
                }
                else
                {
                    //Cria uma chave para trocar senha
                    usuarioEntity.ccdChaveAcessoTrocarSenha = new Random().Next(1000, 9999).ToString();
                    usuarioEntity.nidUsuarioAtualizacao = 0;

                    UsuarioData usuarioData = new UsuarioData();
                    usuarioData.Alterar(usuarioEntity);

                    // Envia a requisição de troca de senha por e-mail
                    MailSender mailSender = new MailSender();

                    string mailTo = usuarioEntity.cdsEmail;
                    string mailSubject = "3M.Comodato - Recuperar Senha";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    string Conteudo = string.Empty;
                    Conteudo += "<p>Prezado(a) usuário(a)</p>";
                    Conteudo += "<p>Conforme solicitado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + ". Segue CÓDIGO para a <b>Troca de Senha</b> do Sistema <b>3M.Comodato</b>.</p>";
                    Conteudo += "<p>" + usuarioEntity.ccdChaveAcessoTrocarSenha + "</p>";

                    var Mensagem = mailSender.GetConteudoHTML("EmailCorpo.html");
                    Mensagem.Replace("#Conteudo", Conteudo);
                    mailMessage = Mensagem.ToString();

                    try
                    {
                        mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.LogarErro(ex);
                        //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Não foi possível envia o e-mail! Tente novamente ou contate o administrador e informe o ocorrido!' }", Formatting.None));
                        JO = JObjectMessage(false, "Não foi possível enviar o e-mail! Tente novamente ou contate o administrador e informe o ocorrido!");
                        return Ok(JO);
                    }
                }

                JO = JObjectMessage(true, "Código de Recuperação de Senha enviado para o seu e-mail!");
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        [HttpPost]
        [Route("ValidarCodigo")]
        [AllowAnonymous]
        public IHttpActionResult ValidarCodigo(UsuarioEntity usuarioLoginTrocarSenhaViaChave)
        {
            try
            {
                JObject JO = new JObject();
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                usuarioEntity.cdsLogin = usuarioLoginTrocarSenhaViaChave.cdsLogin;
                usuarioEntity.ccdChaveAcessoTrocarSenha = usuarioLoginTrocarSenhaViaChave.ccdChaveAcessoTrocarSenha;

                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (!dataTableReader.HasRows)
                {
                    //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Código inválido, digite novamente o código recebido por e-mail.' }", Formatting.None));
                    JO = JObjectMessage(false, "Código inválido, digite novamente o código recebido por e-mail.");
                    return Ok(JO);
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                JO = JObjectMessage(true, "Código válido.");
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        [HttpPost]
        [Route("TrocarSenhaViaChave")]
        [AllowAnonymous]
        public IHttpActionResult TrocarSenhaViaChave(UsuarioEntity usuarioLoginTrocarSenhaViaChave)
        {
            try
            {
                ///**********/
                //new UsuarioData().GravaLogTrocaSenha($"[{usuarioLoginTrocarSenhaViaChave.cdsLogin}][{usuarioLoginTrocarSenhaViaChave.ccdChaveAcessoTrocarSenha}]");
                //UsuarioEntity usr = new UsuarioEntity() { cdsLogin = usuarioLoginTrocarSenhaViaChave.cdsLogin };
                //DataTableReader dt = new UsuarioData().ObterLista(usr).CreateDataReader();
                //if (dt.HasRows)
                //    new UsuarioData().GravaLogTrocaSenha($"[{usuarioLoginTrocarSenhaViaChave.ccdChaveAcessoTrocarSenha}]");
                ///**********/

                JObject JO = new JObject();
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                usuarioEntity.cdsLogin = usuarioLoginTrocarSenhaViaChave.cdsLogin;
                usuarioEntity.ccdChaveAcessoTrocarSenha = usuarioLoginTrocarSenhaViaChave.ccdChaveAcessoTrocarSenha;

                if (!ValidaRegraSenha(usuarioLoginTrocarSenhaViaChave.cdsSenha))
                {
                    //new UsuarioData().GravaLogTrocaSenha($"[Senha inválida][{usuarioLoginTrocarSenhaViaChave.cdsSenha}]"); /**********/
                    //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'A senha deve possuir no mínimo 12 caracteres, ao menos 1 caractere maiúsculo, minúsculo, numérico e caractere especial.' }", Formatting.None));
                    JO = JObjectMessage(false, "A senha deve possuir no mínimo 12 caracteres, ao menos 1 caractere maiúsculo, minúsculo, numérico e caractere especial.");
                    return Ok(JO);
                }

                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    new UsuarioData().GravaLogTrocaSenha("[Usuário encontrado]"); /**********/

                    if (dataTableReader.Read())
                    {
                        usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
                        usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();
                        usuarioEntity.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        usuarioEntity.cdsSenha = dataTableReader["cdsSenha"].ToString();
                        usuarioEntity.bidAtivo = Convert.ToBoolean(dataTableReader["bidAtivo"]);

                        if (dataTableReader["dtmDataHoraTrocaLoginExterno"] != DBNull.Value)
                            usuarioEntity.dtmDataHoraTrocaLoginExterno = Convert.ToDateTime(dataTableReader["dtmDataHoraTrocaLoginExterno"]);

                        if (Convert.ToInt64("0" + dataTableReader["nidPerfil"].ToString()) != ObterPerfilExternoPadrao())
                        {
                            //new UsuarioData().GravaLogTrocaSenha("[Perfil inválido]"); /**********/
                            //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao) + "' }", Formatting.None));
                            JO = JObjectMessage(false, "Usuário não pertence ao perfil " + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilExternoPadrao));
                            return Ok(JO);
                        }

                        //new UsuarioData().GravaLogTrocaSenha("[Perfil válido]"); /**********/
                    }
                }
                else
                {
                    //new UsuarioData().GravaLogTrocaSenha("[Usuário não encontrado]"); /**********/
                    //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário não encontrado. Não é possível recuperar a senha.' }", Formatting.None));
                    JO = JObjectMessage(false, "Usuário não encontrado. Não é possível recuperar a senha.");
                    return Ok(JO);
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                if (usuarioEntity.bidAtivo == false)
                {
                    //new UsuarioData().GravaLogTrocaSenha("[Usuário inativo]"); /**********/
                    //JO.Add("MENSAGEM", JsonConvert.SerializeObject("{ 'MENSAGEM' : 'Usuário inativo no sistema!' }", Formatting.None));
                    JO = JObjectMessage(false, "O usuário está inativo no Portal Web, contate o Administrador do sistema.");
                    return Ok(JO);
                }

                UsuarioData usuarioData = new UsuarioData();
                usuarioEntity.cdsSenha = ControlesUtility.Criptografia.Criptografar(usuarioLoginTrocarSenhaViaChave.cdsSenha);
                usuarioEntity.dtmDataHoraTrocaLoginExterno = DateTime.Now;
                usuarioEntity.ccdChaveAcessoTrocarSenha = string.Empty;
                usuarioEntity.nidUsuarioAtualizacao = usuarioEntity.nidUsuario;

                usuarioData.Alterar(usuarioEntity);

                //new UsuarioData().GravaLogTrocaSenha("[Senha alterada]"); /**********/

                JO = JObjectMessage(true, "Senha alterada com sucesso!");
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        [HttpGet]
        [Route("ObterListaUsuarioSinc")]
        [Authorize]
        public IHttpActionResult ObterListaUsuarioSinc(Int64 idUsuario)
        {
            IList<UsuarioSinc> listaUsuario = new List<UsuarioSinc>();
            try
            {
                //Int32 idUsuario = 60237;
                UsuarioData usuarioData = new UsuarioData();
                listaUsuario = usuarioData.ObterListaUsuarioSinc(idUsuario);

                JObject JO = new JObject();
                //JO.Add("USUARIO", JsonConvert.SerializeObject(listaUsuario, Formatting.None));
                JO.Add("USUARIO", JArray.FromObject(listaUsuario));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("ObterListaUsuarioRegional")]
        public IHttpActionResult ObterListaUsuarioRegional()
        {
            string cdsPerfilGerenteRegionalPadrao = string.Empty;
            List<UsuarioEntity> listaUsuarios = new List<UsuarioEntity>();

            try
            {
                PerfilEntity perfilEntity = new PerfilEntity();

                perfilEntity.cdsPerfil = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.perfilGerenteRegionalVendasPadrao);
                DataTableReader dataTableReader = new PerfilData().ObterLista(perfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        cdsPerfilGerenteRegionalPadrao = dataTableReader["cdsPerfil"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                UsuarioEntity usuarioEntity = new UsuarioEntity();

                dataTableReader = new UsuarioData().ObterLista(usuarioEntity, string.Empty, cdsPerfilGerenteRegionalPadrao).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        usuarioEntity = new UsuarioEntity();
                        usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();

                        listaUsuarios.Add(usuarioEntity);
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("usuarios", JsonConvert.SerializeObject(listaUsuarios, Formatting.None));

            return Ok(jObject);
        }


        [HttpGet]
        [Route("ObterLista")]
        [Authorize]
        public IHttpActionResult ObterLista()
        {
            List<UsuarioEntity> listaUsuarios = new List<UsuarioEntity>();

            try
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();

                DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        usuarioEntity = new UsuarioEntity();
                        usuarioEntity.nidUsuario = Convert.ToInt64(dataTableReader["nidUsuario"]);
                        usuarioEntity.cnmNome = dataTableReader["cnmNome"].ToString();
                        usuarioEntity.cdsLogin = dataTableReader["cdsLogin"].ToString();

                        listaUsuarios.Add(usuarioEntity);
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
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("usuarios", JsonConvert.SerializeObject(listaUsuarios, Formatting.None));

            return Ok(jObject);
        }

        [HttpGet]
        [Authorize]
        [Route("ObterListaPorPerfil")]
        public IHttpActionResult ObterListaPorPerfil(string perfis)
        {
            List<UsuarioEntity> listaUsuarios = new List<UsuarioEntity>();

            try
            {
                List<string> listaPerfis = perfis.Split(',').ToList();
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                DataTable dataTable = new UsuarioPerfilData().ObterListaALLUsuario(usuarioPerfilEntity, true, false);
                listaUsuarios = (from t in dataTable.Rows.Cast<DataRow>()
                                   where t.FieldOrDefault<decimal>("nidPerfil") != default(decimal) && listaPerfis.Contains(t.FieldOrDefault<decimal>("nidPerfil").ToString())
                                   select new UsuarioEntity() {
                                       nidUsuario = Convert.ToInt64(t["nidUsuario"]),
                                       cnmNome = t["cnmNome"].ToString(),
                                       cdsLogin = t["cdsLogin"].ToString()
                                   }).ToList();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject();
            jObject.Add("usuarios", JsonConvert.SerializeObject(listaUsuarios, Formatting.None));

            return Ok(jObject);
        }

        private bool ValidaRegraSenha(string senha)
        {
            return senha.Length >= 12 && senha.Any(char.IsUpper) && senha.Any(char.IsLower) && senha.Any(char.IsNumber) && 
                senha.Any(c => @"\|!#$%&/()=?»«@£§€{}.-;'<>_,".Contains(c));
        }
    }
}