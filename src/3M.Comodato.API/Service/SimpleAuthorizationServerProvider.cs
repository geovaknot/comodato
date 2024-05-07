using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using _3M.Comodato.Utility;

namespace _3M.Comodato.API.Service
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();

            //var roles = context.Parameters.FirstOrDefault(p => p.Key.Equals("roles")).Value.FirstOrDefault();
            //context.OwinContext.Set<IEnumerable<string>>("roles", roles.Split(','));

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            bool UserValidado = false;

            
            string retorno = ValidarUsuario(context.UserName, context.Password);

            //var usuario = AutenticarUsuario(context.UserName, context.Password);
            if (retorno != "Valido")
            {
                context.SetError("invalid_grant", retorno);
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, "1"));
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            //if (usuario.Administrador)
            //    identity.AddClaim(new Claim(ClaimTypes.Role, "Administrador"));

            //foreach (var role in usuario.Permissoes)
            //    identity.AddClaim(new Claim(ClaimTypes.Role, role));

            //var ticket = new AuthenticationTicket(identity, null);
            context.Validated(identity);
        }

        private string ValidarUsuario(string login, string senha)
        {
            try
            {
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.usuario.cdsLogin = login;

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
                    
                    return "Usuário ou senha inválidos";

                    //ViewBag.Mensagem = "Login inválido!";
                    //return false;
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                //if (usuarioPerfilEntity.perfil.nidPerfil != ObterPerfilExternoPadrao())
                //{
                //    return "Acesso não permitido para o seu perfil. Permitido somente acesso dos Técnicos.";
                //}

                if (usuarioPerfilEntity.usuario.bidAtivo == false)
                {
                    return "O usuário está inativo no Portal Web, contate o Administrador do sistema.";
                    
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
                        if (principalContext.ValidateCredentials(login, senha, ContextOptions.Negotiate) == false)
                        {
                            //return Request.CreateResponse(HttpStatusCode.BadRequest, "Credenciais inválidas!");
                            return "Credenciais inválidas!";

                            //ViewBag.Mensagem = "Credenciais inválidas!";
                            //return false;
                        }
                    }
                }
                else
                {
                    var teste_senha = ControlesUtility.Criptografia.Criptografar(senha);
                    // Senha no banco gravada - Usuário Externo
                    if (ControlesUtility.Criptografia.Criptografar(senha) != usuarioPerfilEntity.usuario.cdsSenha)
                    {
                        return "Usuário ou senha inválidos.";

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
                            return "Senha vencida. Altere sua senha para renovar o usuário.";

                            //ViewBag.Mensagem = "Trocar senha!";
                        }
                    }
                }

                usuarioPerfilEntity.usuario.cdsSenha = string.Empty;
                
                return "Valido";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            
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

        //private dynamic AutenticarUsuario(string login, string senha)
        //{
        //    RepositoryDapper _conDapper = new RepositoryDapper();
        //    try
        //    {

        //        _conDapper.connect();
        //        var fld = _conDapper.dbType.ToUpper().Trim().Equals("SQL") ? " [key] " : " key ";
        //        var auth = _conDapper.Select<RFE_PARAMETROS>($"{fld} = '{Constantes.WebConfig.STANDARD_WEBSERVICE_AUTHENTICATION}'").FirstOrDefault();
        //        if (auth == null)
        //        {
        //            CustomLogging.LogMessage(TracingLevel.DEBUG, $"Key de configuração de WS {Constantes.WebConfig.STANDARD_WEBSERVICE_AUTHENTICATION} não existe no tabela de parámetros.");
        //            return null;
        //        }

        //        var usuariosFixos = new[]
        //        {
        //        new { Id = auth.ID, Login = auth.VALUE, Password = auth.VALUE2, Permissoes = new[] { "webservice"} }
        //    };

        //        return usuariosFixos.FirstOrDefault(u => u.Login.Equals(login) && u.Password.Equals(senha));
        //    }
        //    catch (Exception ex)
        //    {
        //        CustomLogging.LogMessage(TracingLevel.ERROR, ex.Message);
        //        if (ex.InnerException != null)
        //            CustomLogging.LogMessage(TracingLevel.ERROR, ex.InnerException.Message);
        //        _conDapper?.disconnect();
        //        return null;
        //    }
        //    finally
        //    {
        //        _conDapper?.disconnect();
        //    }
        //    return null;
        //}
    }
}