using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace _3M.Comodato.Utility
{
    public class ControlesUtility
    {

        public class Enumeradores
        {
            // Todos os enumeradores devem ser declarados aqui
            public enum Perfil
            {
                Administrador3M = 1,
                Tecnico3M = 2,
                TecnicoEmpresaTerceira = 3,
                EquipeVendas = 4,
                AssistênciaTecnica3M = 5,
                LiderEmpresaTecnica = 6,
                EquipeMKT = 7,
                Cliente = 8,
                ControleEstoque = 9,
                GerenteRegionaldeVendas = 10,
                GerenteNacionaldeVendas = 11,
                AnalistaTecnico3M = 12
            }

            public enum RRStatusRelatorio
            {
                Novo = 0,
                AnaliseAreaTecnica = 1,
                EmCompras = 2,
                Finalizado = 3,
                PecaEnviadoTécnico = 4,

            }
            
            public enum AcompanamentoRR
            {
                Novo = 0,
                TecRegional = 1,
                AnaliseTecnica = 2,
                EmCompras = 3,
                Finalizado = 4,
                EnviadoTecnicoCampo = 5,
                Reprovado = 6

            }

            public enum TpStatusVisita
            {
                Nova = 1,
                Aberta = 2,
                Finalizada = 3,
                Confirmada = 4,
                Pausada = 5,
                Pendente = 6,
                Cancelada = 7,
                Portaria = 8,
                Integracao = 9,
                Treinamento = 10,
                Consultoria = 11
            }

            public enum TpStatusVisitaPadrao
            {
	            Iniciar = 2,
	            Aberta = 3,
	            Finalizada = 4,
	            Cancelada = 5
            }

            public enum TpStatusOS
            {
                Nova = 1,
                Aberta = 2,
                Pausada = 3,
                Finalizada = 4,
                Pendente = 5,
                Cancelada = 6,
                Confirmada = 7
            }

            public enum TpStatusOSPadrao
            {
                AguardandoInicio = 1,
                Aberta = 2,
                Finalizada = 3,
                Cancelada = 4,
                Confirmada = 5
            }

            public enum StatusPedido
            {
                NovoRascunho = 1,
                Solicitado = 2,
                Aprovado = 3,
                Recebido = 4,
                Pendente = 5,
                RecebidoComPendencia = 6,
                Cancelado = 7,
                AguardandoEnvioBPCS = 8,
                EnviadoBPCS = 9
            }

            public enum TipoAplicacao
            {
                App = 1,
                Web = 2
            }

            public enum TipoPeca
            {
                PecaComItem = 1,
                PecaSemItemEspecial = 2
            }

            public enum TpMovimentacao
            {
                MovimentacaoEstoque = 1,
                MovimentacaoPecas = 2,
                AjusteEntrada = 3,
                AjusteSaida = 4,
                Atendimento = 5,
                Remessa3M = 6
            }

            public enum WorkflowPedidoEnvio
            {
                Rascunho = 0,
                PendenteAnexar = 1,
                AnaliseMarketing = 2,
                AnaliseAreaTecnica = 3,
                AnaliseEspecial = 8,
                AnalisePlanejamento = 4,
                EnviadoCliente = 5,
                EntregueCliente = 6,
                Instalado = 7,
                EmCompras = 9,
                Cancelado = 91
            }

            public enum WorkflowPedidoDevolucao
            {
                Rascunho = 20,
                PendenteAnexar = 21,
                AnaliseLogistica = 22,
                Solicitado = 23,
                RetiradaAgendada = 24,
                Coletado = 25,
                AguardandoProgTMS = 26,
                DevolucaoConcluida = 27,
                DevolvidoPlanejam = 28,
                PendenciaCliente = 29,
                Cancelado = 92
            }

            public enum AcessoEadTecino3m
            {
            AppMobile = 0,
            Clientes =1,
            UtilizacaoRelatorios= 2,
            EquipeTecnica= 3,
            EquipeTecnica3M =4 

            }

            public enum AcessoEadTecinoExterno
            {
                AppMobile = 0,
                EquipeTecnica = 3
            }

            public enum AcessoEadLiderExterno
            {
                AppMobile = 0,
                EquipeTecnica = 3
            }

            public enum AcessoEadVendas
            {
                Dashboard = 0,
                WorkflowEnvio =1,
                WorkflowDevolucao =2
            }

            public enum AcessoEadControleEstoque
            {
                ControleEstoque = 0,
                Dashboard =1

            }

        }

        public class Constantes
        {
            // Todas as constantes devem ser declaradas aqui
            public const string ADDomain = "ADDomain";
            public const string ADPassword = "ADPassword";
            public const string ADService = "ADService";
            public const string ADUser = "ADUser";
            public const string diasTrocaSenhaExterno = "diasTrocaSenhaExterno";
            public const string MailCredentialsPassword = "MailCredentialsPassword";
            public const string MailCredentialsUserName = "MailCredentialsUserName";
            public const string MailEnableSSL = "MailEnableSSL";
            public const string MailHost = "MailHost";
            public const string MailPort = "MailPort";
            public const string MailUseDefaultCredentials = "MailUseDefaultCredentials";

            public const string perfilExternoPadrao = "perfilExternoPadrao";
            public const string perfilLiderPadrao = "perfilLiderPadrao";
            public const string perfisMeusClientes = "perfisMeusClientes";
            public const string perfisTecnicos = "perfisTecnicos";
            public const string perfilEquipeVendasPadrao = "perfilEquipeVendasPadrao";
            public const string perfilGerenteRegionalVendasPadrao = "perfilGerenteRegionalVendasPadrao";
            public const string perfilGerenteNacionalVendasPadrao = "perfilGerenteNacionalVendasPadrao";

            public const string URLSite = "URLSite";
            public const string URLAPI = "URLAPI";
            public const string MensagemGravacaoSucesso = "Registro gravado com sucesso!";
            public const string MensagemErroAtivoXCliente = "Ativo com data de devolução em aberto já cadastrado!";
            public const string MensagemGravacaoErroAtivo = "Informe se o registro está Ativo ou Inativo";
            public const string MensagemGravacaoErroFerias = "Preencha o campo Férias";
            public const string MensagemGravacaoSucessoQTDPlanoZero = "Informe a Quantidade Minima Plano Zero";
            public const string MensagemPecaInativa = "O Campo: 'CD Peça Recuperada' precisa ser um Código de Peça ativa e do tipo Especial!"; 
            public const string MensagemOSVisitaExistente = "Já existe Ordem de serviço ou visita registrado nesse período!";
            public const string MensagemExclusaoSucesso = "Registro excluído com sucesso!";
            public const string MensagemInativacaoSucesso = "Registro inativado com sucesso!";
            public const string MensagemAtivo = "Por favor selecione um Ativo antes de continuar";
            public const string MensagemExistePonderacao = "Este valor de ponderação já foi registrado!"; 
            public const string MensagemRangePonderacao = "Esses dados estão em conflito com uma faixa de Clientes já cadastrada! Por favor, revise as informações e tente incluir novamente!!";
            public const string MensagemRangePonderacaoMin = "Informe a Faixa Inicial Qtde Clientes!!";
            public const string MensagemRangePonderacaoMax = "Informe a Faixa Final Qtde Clientes!!";
            public const string MensagemRangePonderacaoMinMax = "Informe a Faixa Inicial Qtde Clientes!! Informe a Faixa Final Qtde Clientes!!";
            public const string MensagemRangePonderacaoMinMaxFator = "Fator de Ponderação está fora da faixa definida nos parâmetros do Sistema!";
            public const string MensagemRangePonderacaoMinMaiorMax = "A Faixa Inicial Qtde Clientes não pode ser maior que a Faixa Final Qtde Clientes!!";

            public const string vigenciaINICIAL = "vigenciaINICIAL";
            public const string vigenciaFINAL = "vigenciaFINAL";

            public const string MensagemEnvioAvaliacao = "PesquisaMailNotificacao";
            //public const string MensagemEnvioAvaliacao = "";
            public const string MargemDashVendas = "MargemDashVendas";
            public const string CodigoPecaAvulsa = "CodigoPecaAvulsa";

            public const string CodigoSegmentoRealocarExcluir = "SegmentoRealocarExcluir";
            public const string CodigoSegmentoDistribuidor = "SegmentoDistribuidor";

            public const string CaminhoUpload = "CaminhoArquivosUpload";
            public const string CaminhoUploadNF = "CaminhoArquivosUpload_NF";

            //public const string PastaNFLoteUpload = @"/NotaFiscal/Lote/";
            //public const string PastaFotosPecasSincronismo = @"/Sinc/";
            //public const string PastaWorkflowUploadEnvio = @"/Workflow/Envio/";
            //public const string PastaWorkflowUploadDevolucao = @"/Workflow/Devolucao/";
            //public const string PastaAtivoClienteNF = @"/NotaFiscal/AtivoCliente/";

            public const string PastaNFLoteUpload = @"\NotaFiscal\Lote\";
            public const string PastaFotosPecasSincronismo = @"\Sinc\";
            public const string PastaWorkflowUploadEnvio = @"\Workflow\Envio\";
            public const string PastaWorkflowUploadDevolucao = @"\Workflow\Devolucao\";
            public const string PastaAtivoClienteNF = @"\NotaFiscal\AtivoCliente\";

            public const string ValorEnvioMensalPecas = "ValorEnvioMensalPecas";
            public const string WorkflowCategoriaFechador = "WorkflowCategoriaFechador";
            public const string WorkflowCategoriaIdentificador = "WorkflowCategoriaIdentificador";
            public const string WorkflowCategoriaAcessorios = "WorkflowCategoriaAcessorios";
            public const string WorkflowSolicitacaoTroca = "WorkflowSolicitacaoTroca";

            public const string VideoEAD1 = "VideoEAD1";
            public const string DiasPendenciaAprovacao = "DiasPendenciaAprovacao";
        }

        public class Criptografia
        {
            protected static byte[] _IV = Encoding.UTF8.GetBytes("27100610");
            protected static byte[] _Key = Encoding.UTF8.GetBytes("96416862");
            protected static byte[] _Entrada;

            protected static System.IO.MemoryStream m_Str;
            protected static System.Security.Cryptography.CryptoStream c_Str;
            protected static System.Security.Cryptography.DESCryptoServiceProvider _des;

            /// <summary>
            /// Efetua a criptografia de um valor
            /// </summary>
            /// <param name="Valor">Valor a criptografar</param>
            /// <returns>Valor criptografado</returns>
            public static string Criptografar(string Valor)
            {
                if (!string.IsNullOrEmpty(Valor))
                {
                    // Codifica o valor de entrada.
                    _Entrada = Encoding.UTF8.GetBytes(Valor);

                    // Instancia o Serviço de Criptografia.
                    _des = new System.Security.Cryptography.DESCryptoServiceProvider();

                    // Aloca um espaço na memoria.
                    m_Str = new System.IO.MemoryStream();

                    // Criptografa o valor de entrada
                    c_Str = new System.Security.Cryptography.CryptoStream(m_Str, _des.CreateEncryptor(_Key, _IV),
                        System.Security.Cryptography.CryptoStreamMode.Write);

                    c_Str.Write(_Entrada, 0, _Entrada.Length);
                    c_Str.FlushFinalBlock();

                    return Convert.ToBase64String(m_Str.ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// Efetua a descriptografia de um valor
            /// </summary>
            /// <param name="Valor">Valor criptografado</param>
            /// <returns>Valor descriptografado</returns>
            public static string Descriptografar(string Valor)
            {
                if (!string.IsNullOrEmpty(Valor))
                {
                    Valor = Valor.Replace(" ", "+");
                    byte[] _Ent = new byte[Valor.Length];

                    _des = new System.Security.Cryptography.DESCryptoServiceProvider();

                    // Aloca um espaço na memoria.
                    m_Str = new System.IO.MemoryStream();

                    c_Str = new System.Security.Cryptography.CryptoStream(m_Str, _des.CreateDecryptor(_Key, _IV),
                        System.Security.Cryptography.CryptoStreamMode.Write);

                    _Ent = Convert.FromBase64String(Valor);

                    c_Str.Write(_Ent, 0, _Ent.Length);
                    c_Str.FlushFinalBlock();

                    return Encoding.UTF8.GetString(m_Str.ToArray());
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static class Utilidade
        {
            const int REGISTRO_SEM_HORA_INICIAL = 0;
            const int REGISTRO_EXISTENTE_SEM_HORA_FINAL = 2359;
            const int REGISTRO_INFORMADO_SEM_HORA_FINAL = 2360;

            public static int ObterPrefixoTokenRegistro(long idUsuario = 0)
            {
                var aplicacao = Convert.ToInt32(Enumeradores.TipoAplicacao.Web);

                if (idUsuario == 0)
                    idUsuario = ((UsuarioPerfilEntity)HttpContext.Current.Session["_CurrentUser"]).usuario.nidUsuario;

                return int.Parse($"{Convert.ToString(aplicacao)}{Convert.ToString(idUsuario)}"); 
            }

            public static bool VerificarExisteVisitaDataHoraSemCompararVisitaInformada(VisitaPadraoEntity visitaPadraoEntity)
            {
                var visitaPadraoValidar = ObterVisitaParaValicacao(visitaPadraoEntity);

                var lstVisita = new VisitaPadraoData().ObterListaVisita(visitaPadraoValidar);

                var visitas = lstVisita.Where(v => v.ID_VISITA != visitaPadraoEntity.ID_VISITA 
                                    && v.DT_DATA_VISITA.ToString("yyyyMMdd") == visitaPadraoValidar.DT_DATA_VISITA.ToString("yyyyMMdd")
                                    && v.TpStatusVisita.ST_STATUS_VISITA != Convert.ToInt64(Enumeradores.TpStatusVisitaPadrao.Cancelada));

                return ValidarExisteVisitaPeriodo(visitaPadraoValidar, visitas);
            }

            public static bool VerificarExisteVisitaDataHora(VisitaPadraoEntity visitaPadraoEntity)
            {
                var visitaPadraoValidar = ObterVisitaParaValicacao(visitaPadraoEntity);

                var lstVisita = new VisitaPadraoData().ObterListaVisita(visitaPadraoValidar);

                var visitas = lstVisita.Where(v => v.DT_DATA_VISITA.ToString("yyyyMMdd") == visitaPadraoValidar.DT_DATA_VISITA.ToString("yyyyMMdd")
                                    && v.TpStatusVisita.ST_STATUS_VISITA != Convert.ToInt64(Enumeradores.TpStatusVisitaPadrao.Cancelada));

                return ValidarExisteVisitaPeriodo(visitaPadraoValidar, visitas);
            }

            private static VisitaPadraoEntity ObterVisitaParaValicacao(VisitaPadraoEntity visitaPadraoEntity)
            {
                VisitaPadraoEntity visitaPadrao = new VisitaPadraoEntity();
                visitaPadrao.Tecnico.CD_TECNICO = visitaPadraoEntity.Tecnico.CD_TECNICO;
                visitaPadrao.DT_DATA_VISITA = visitaPadraoEntity.DT_DATA_VISITA.Date;
                visitaPadrao.HR_INICIO = visitaPadraoEntity.HR_INICIO;
                visitaPadrao.HR_FIM = visitaPadraoEntity.HR_FIM;

                return visitaPadrao;
            }

            private static bool ValidarExisteVisitaPeriodo(VisitaPadraoEntity visitaPadraoEntity, IEnumerable<VisitaPadraoEntity> visitasExistentes)
            {
                int inicioVisitaInformada = string.IsNullOrWhiteSpace(visitaPadraoEntity.HR_INICIO) ? REGISTRO_SEM_HORA_INICIAL
                        : Convert.ToInt32(visitaPadraoEntity.HR_INICIO.Remove(visitaPadraoEntity.HR_INICIO.IndexOf(':'), 1));

                int finalVisitaInformada = string.IsNullOrWhiteSpace(visitaPadraoEntity.HR_FIM) ? REGISTRO_INFORMADO_SEM_HORA_FINAL
                        : Convert.ToInt32(visitaPadraoEntity.HR_FIM.Remove(visitaPadraoEntity.HR_FIM.IndexOf(':'), 1));

                foreach (var visita in visitasExistentes)
                {
                    int inicialVisitaExistente = string.IsNullOrWhiteSpace(visita.HR_INICIO) ? REGISTRO_SEM_HORA_INICIAL
                            : Convert.ToInt32(visita.HR_INICIO.Remove(visita.HR_INICIO.IndexOf(':'), 1));

                    int finalVisitaExistente = string.IsNullOrWhiteSpace(visita.HR_FIM) ? REGISTRO_EXISTENTE_SEM_HORA_FINAL
                            : Convert.ToInt32(visita.HR_FIM.Remove(visita.HR_FIM.IndexOf(':'), 1));

                    if (finalVisitaExistente == REGISTRO_EXISTENTE_SEM_HORA_FINAL)
                        return true;
                    else if ((inicioVisitaInformada >= inicialVisitaExistente && inicioVisitaInformada <= finalVisitaExistente)
                            || (finalVisitaInformada >= inicialVisitaExistente && finalVisitaInformada <= finalVisitaExistente))
                        return true;
                    else if (inicioVisitaInformada < inicialVisitaExistente && finalVisitaInformada > finalVisitaExistente && finalVisitaInformada != REGISTRO_INFORMADO_SEM_HORA_FINAL)
                        return true;
                }

                return false;
            }

            public static bool VerificarExisteOsDataHoraSemCompararOsInformada(OSPadraoEntity osPadraoEntity)
            {
                var osPadraoValidar = ObterOsParaValicacao(osPadraoEntity);

                var ordensServico = new OSPadraoData().ObterLista(osPadraoValidar);

                var ordensServicoExistentes = ordensServico.Where(v => v.ID_OS != osPadraoEntity.ID_OS 
                                && v.DT_DATA_OS.ToString("yyyyMMdd") == osPadraoValidar.DT_DATA_OS.ToString("yyyyMMdd")
                                && v.TpStatusOS.ST_STATUS_OS != Convert.ToInt64(Enumeradores.TpStatusOSPadrao.Cancelada));

                return ValidarExisteOsPeriodo(osPadraoValidar, ordensServicoExistentes);
            }

            public static bool VerificarExisteOSDataHora(OSPadraoEntity osPadraoEntity)
            {
                var osPadraoValidar = ObterOsParaValicacao(osPadraoEntity);

                var ordensServico = new OSPadraoData().ObterLista(osPadraoValidar);

                var ordensServicoExistentes = ordensServico.Where(v => v.DT_DATA_OS.ToString("yyyyMMdd") == osPadraoValidar.DT_DATA_OS.ToString("yyyyMMdd")
                                 && v.TpStatusOS.ST_STATUS_OS != Convert.ToInt64(Enumeradores.TpStatusOSPadrao.Cancelada));

                return ValidarExisteOsPeriodo(osPadraoValidar, ordensServicoExistentes);
            }

            private static OSPadraoEntity ObterOsParaValicacao(OSPadraoEntity osPadraoEntity)
            {
                OSPadraoEntity osPadrao = new OSPadraoEntity();
                osPadrao.Tecnico.CD_TECNICO = osPadraoEntity.Tecnico.CD_TECNICO;
                osPadrao.DT_DATA_OS = osPadraoEntity.DT_DATA_OS.Date;
                osPadrao.HR_INICIO = osPadraoEntity.HR_INICIO;
                osPadrao.HR_FIM = osPadraoEntity.HR_FIM;

                return osPadrao;
            }

            private static bool ValidarExisteOsPeriodo(OSPadraoEntity osPadraoEntity, IEnumerable<OSPadraoEntity> ordensServicoExistentes)
            {
                int inicioOsInformada = string.IsNullOrWhiteSpace(osPadraoEntity.HR_INICIO) ? REGISTRO_SEM_HORA_INICIAL
                        : Convert.ToInt32(osPadraoEntity.HR_INICIO.Remove(osPadraoEntity.HR_INICIO.IndexOf(':'), 1));

                int finalOsInformada = string.IsNullOrWhiteSpace(osPadraoEntity.HR_FIM) ? REGISTRO_INFORMADO_SEM_HORA_FINAL
                        : Convert.ToInt32(osPadraoEntity.HR_FIM.Remove(osPadraoEntity.HR_FIM.IndexOf(':'), 1));

                int qtdOSAberta = 0;

                
                foreach (var OS in ordensServicoExistentes)
                {
                    if (OS.TpStatusOS.ST_STATUS_OS == 2)
                    {
                        qtdOSAberta += 1;
                    }
                    int inicialOsExistente = string.IsNullOrWhiteSpace(OS.HR_INICIO) ? REGISTRO_SEM_HORA_INICIAL
                            : Convert.ToInt32(OS.HR_INICIO.Remove(OS.HR_INICIO.IndexOf(':'), 1));

                    int finalOsExistente = string.IsNullOrWhiteSpace(OS.HR_FIM) ? REGISTRO_EXISTENTE_SEM_HORA_FINAL
                            : Convert.ToInt32(OS.HR_FIM.Remove(OS.HR_FIM.IndexOf(':'), 1));

                    if (finalOsExistente == REGISTRO_EXISTENTE_SEM_HORA_FINAL && osPadraoEntity.ID_OS != 0)
                        return true;
                    else if ((inicioOsInformada >= inicialOsExistente && inicioOsInformada <= finalOsExistente && osPadraoEntity.ID_OS != 0)
                            || (finalOsInformada >= inicialOsExistente && finalOsInformada <= finalOsExistente && osPadraoEntity.ID_OS != 0))
                        return true;
                    else if (inicioOsInformada < inicialOsExistente && finalOsInformada > finalOsExistente && finalOsInformada != REGISTRO_INFORMADO_SEM_HORA_FINAL && osPadraoEntity.ID_OS != 0)
                        return true;

                }
                if (qtdOSAberta > 1)
                    return true;
                else
                    return false;
            }
        }

        public class Seguranca
        {
            /// <summary>
            /// Verifica se o usuário possui acesso a uma determinada função 
            /// </summary>
            /// <param name="usuario">Entidade usuário</param>
            /// <param name="ccdFuncao">Código da função</param>
            /// <returns>Booleano (true-possui acesso, false-não possui acesso)</returns>
            public static bool VerificarAcesso(UsuarioEntity usuario, string ccdFuncao)
            {
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                UsuarioPerfilData usuarioPerfilData = new UsuarioPerfilData();

                usuarioPerfilEntity.usuario.nidUsuario = usuario.nidUsuario;
                return usuarioPerfilData.VerificarAcesso(usuarioPerfilEntity, ccdFuncao);
            }

            public static void VerificarAcesso()
            {
                UsuarioEntity usuario = ((UsuarioPerfilEntity)HttpContext.Current.Session["_CurrentUser"]).usuario;

                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                UsuarioPerfilData usuarioPerfilData = new UsuarioPerfilData();
                usuarioPerfilEntity.usuario.nidUsuario = usuario.nidUsuario;
                //return usuarioPerfilData.VerificarAcesso(usuarioPerfilEntity);

                HttpContext.Current.Session["_CurrentUserAllPerfil"] = usuarioPerfilData.VerificarAcesso(usuarioPerfilEntity);
            }

            public static bool VerificarAcesso(string ccdFuncao)
            {

                if (HttpContext.Current.Session["_CurrentUserAllPerfil"] == null)
                    VerificarAcesso();

                DataTable Cachtable = (DataTable)HttpContext.Current.Session["_CurrentUserAllPerfil"];

                foreach (DataRow item in Cachtable.Rows)
                {
                    if (item["ccdFuncao"].ToString() == ccdFuncao)
                    {
                        return true;
                    }
                }
                return false;
            }


            /// <summary>
            /// Verifica se o Perfil do usuário (niUsuario) faz parte dos perfis Técnico 3M ou Técnico Empresa Terceira
            /// </summary>
            /// <param name="nidUsuario"></param>
            /// <returns>TRUE - Usuário faz parte de um dos perfis, FALSE - Usuário não faz parte de um dos perfis</returns>
            public static bool VerificaUsuarioPerfisTecnicos(Int64 nidUsuario)
            {
                bool retorno = false;
                string cdsPerfisTecnicos = Parametro.ObterValorParametro(Constantes.perfisTecnicos);

                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                UsuarioPerfilData usuarioPerfilData = new UsuarioPerfilData();

                usuarioPerfilEntity.usuario.nidUsuario = nidUsuario;
                DataTableReader dataTableReader = usuarioPerfilData.ObterLista(usuarioPerfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (!string.IsNullOrEmpty(cdsPerfisTecnicos))
                        {
                            string[] cdsPerfil = cdsPerfisTecnicos.Split(',');
                            for (int a = 0; a < cdsPerfil.Length; a++)
                            {
                                if (cdsPerfil[a].ToLower().Trim() == dataTableReader["cdsPerfil"].ToString().ToLower().Trim())
                                {
                                    var perfil3M = cdsPerfil[a].ToLower().Trim();
                                    if (dataTableReader["cdsPerfil"].ToString().ToLower().Trim() == "técnico 3m")
                                    {
                                        retorno = false;
                                        break;

                                    }
                                    else
                                    {
                                        retorno = true;
                                        break;

                                    }

                                }
                            }
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return retorno;
            }

            /// <summary>
            /// Verifica se o Perfil do usuário (niUsuario) faz parte dos perfis Técnico 3M, Técnico Empresa Terceira ou Líder Empresa Técnica para exibir somente SEUS Clientes
            /// </summary>
            /// <param name="nidUsuario"></param>
            /// <returns>TRUE - Usuário faz parte de um dos perfis, FALSE - Usuário não faz parte de um dos perfis</returns>
            public static bool VerificaUsuarioPerfisMeusClientes(Int64 nidUsuario)
            {
                bool retorno = false;
                string cdsPerfisTecnicos = Parametro.ObterValorParametro(Constantes.perfisMeusClientes);

                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                UsuarioPerfilData usuarioPerfilData = new UsuarioPerfilData();

                usuarioPerfilEntity.usuario.nidUsuario = nidUsuario;
                DataTableReader dataTableReader = usuarioPerfilData.ObterLista(usuarioPerfilEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (!string.IsNullOrEmpty(cdsPerfisTecnicos))
                        {
                            string[] cdsPerfil = cdsPerfisTecnicos.Split(',');
                            for (int a = 0; a < cdsPerfil.Length; a++)
                            {
                                if (cdsPerfil[a].ToLower().Trim() == dataTableReader["cdsPerfil"].ToString().ToLower().Trim())
                                {
                                    retorno = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return retorno;
            }
        }

        public class Parametro
        {
            /// <summary>
            /// Busca o conteúdo de cdsParametro da tabela tbParametro
            /// </summary>
            /// <param name="ccdParametro">Código</param>
            /// <returns>Descrição</returns>
            public static string ObterValorParametro(string ccdParametro)
            {
                if (ccdParametro.ToUpper() == Constantes.URLSite.ToUpper())
                {
                    return ConfigurationManager.AppSettings[Constantes.URLSite];
                }
                else if (ccdParametro.ToUpper() == Constantes.URLAPI.ToUpper())
                {
                    return ConfigurationManager.AppSettings[Constantes.URLAPI];
                }

                string cvlParametro = string.Empty;
                ParametroEntity parametroEntity = new ParametroEntity();
                ParametroData parametroData = new ParametroData();

                parametroEntity.ccdParametro = ccdParametro;
                DataTableReader dataTableReader = parametroData.ObterLista(parametroEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        cvlParametro = dataTableReader["cvlParametro"].ToString();
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return cvlParametro;
            }

        }

        public class Dicionarios
        {
            public static Dictionary<string, string> SimNao()
            {
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("Sim", "S");
                dictionary.Add("Não", "N");
                dictionary.Add("Selecione", null);

                return dictionary;
            }

            public static Dictionary<string, int> PeridoPlanoZero()
            {
                var dictionary = new Dictionary<string, int>();
                dictionary.Add("Selecione", 0);
                dictionary.Add("Mensal", 12);
                dictionary.Add("Bimestral", 6);
                dictionary.Add("Trimestral", 4);
                dictionary.Add("Quadrimestral", 3);
                dictionary.Add("Semestral", 2);
                
                return dictionary;
            }

            public static Dictionary<string, string> TipoTecnico()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("3M", "3");
                dictionary.Add("Terceiro", "T");

                return dictionary;
            }

            public static Dictionary<string, string> TipoPeca()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Automático", "A");
                dictionary.Add("Normal", "N");
                dictionary.Add("Especial", "E");
                //dictionary.Add("Recuperada", "R");
                return dictionary;
            }

            public static Dictionary<string, string> TipoEmpresa()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("3M", "00");
                dictionary.Add("Assistência Técnica", "01");
                dictionary.Add("Transportadora", "02");

                return dictionary;
            }

            public static Dictionary<string, string> TipoManutencaoOS()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Preventiva", "P");
                dictionary.Add("Corretiva", "C");
                dictionary.Add("Instalação", "I");
                dictionary.Add("Outros", "O");

                return dictionary;
            }

            public static Dictionary<string, string> TipoEstoqueUtilizado()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Estoque Intermediário", "T");
                dictionary.Add("Estoque Cliente", "C");

                return dictionary;
            }

            public static Dictionary<string, string> TipoStatusPendenciaOS()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Pendente", "1");
                dictionary.Add("Finalizada", "2");
                dictionary.Add("Cancelada", "3");

                return dictionary;
            }

            public static Dictionary<string, string> TipoParametro()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Negócio", "N");
                dictionary.Add("Sistema", "S");

                return dictionary;
            }

            public static Dictionary<string, string> TipoPendenciaOS()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Peça", "P");
                dictionary.Add("Outros", "O");

                return dictionary;
            }

            public static Dictionary<string, string> TipoEstoque()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("3M1", "Estoque 3M A.Tec.");
                dictionary.Add("3M2", "Estoque 3M Recuperadas");
                dictionary.Add("TEC", "Estoque Técnico");
                dictionary.Add("CLI", "Estoque Cliente");

                return dictionary;
            }

            public static Dictionary<string, string> TipoEntradaSaida()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("E", "Entrada");
                dictionary.Add("S", "Saída");

                return dictionary;
            }

            public static Dictionary<string, string> TipoPedido()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Pedido p/ técnico", "T");
                dictionary.Add("Pedido avulso", "A");
                dictionary.Add("Pedido p/ cliente", "C");

                return dictionary;
            }

            public static Dictionary<string, string> StatusItem()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Novo/Rascunho", "1");
                dictionary.Add("Pendente", "2");
                dictionary.Add("Aprovado", "3");
                dictionary.Add("Cancelado", "4");
                dictionary.Add("Recebido", "5");
                dictionary.Add("Solicitado", "6");
                dictionary.Add("Recebido com Pendência", "7"); 
                dictionary.Add("Aguardando Envio BPCS", "8");
                dictionary.Add("Enviado ao BPCS", "9");
                return dictionary;
            }

            public static Dictionary<string, string> TipoPesquisaCliente()
            {
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("", "");
                dictionary.Add("1", "Enviar");
                dictionary.Add("2", "Específica");
                dictionary.Add("3", "Nunca");

                return dictionary;
            }

            public static Dictionary<int, string> TipoPesquisaSatisfacao()
            {
                var dictionary = new Dictionary<int, string>();
                dictionary.Add(1, "Geral");
                dictionary.Add(2, "Específica");

                return dictionary;
            }

            public static Dictionary<string, string> SituacaoPesquisa()
            {
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("1", "Ativo");
                dictionary.Add("0", "Finalizado");

                return dictionary;
            }

            public static Dictionary<string, string> TipoDashboard()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Dashboard Planejamento", "P");
                dictionary.Add("Dashboard Área Técnica", "T");
                dictionary.Add("Dashboard Negócios", "N");
                dictionary.Add("Dashboard Vendas", "V");

                return dictionary;
            }

            public static Dictionary<string, string> StatusDevolucao()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("", "1");
                dictionary.Add("Em Andamento", "2");
                dictionary.Add("Devolvido", "3");

                return dictionary;
            }

            #region WORKFLOW

            public static Dictionary<string, string> TipoPedidoWorkflow()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("Envio", "E");
                dictionary.Add("Devolução", "D");

                return dictionary;
            }

            public static Dictionary<string, string> TipoLocacao()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("C", "Comodato");
                dictionary.Add("A", "Aluguel");
                //dictionary.Add("E", "Empréstimo de Bem de Ativo");

                return dictionary;
            }

            public static Dictionary<string, string> TipoEmpacotamento()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("A", "Automática");
                dictionary.Add("S", "Semi-Automática");
                dictionary.Add("E", "Especial");
                dictionary.Add("N", "N/A");

                return dictionary;
            }

            public static Dictionary<int, int> Prioridade()
            {
                var dictionary = new Dictionary<int, int>();

                dictionary.Add(1, 1);
                dictionary.Add(2, 2);
                dictionary.Add(3, 3);
                dictionary.Add(4, 4);
                dictionary.Add(5, 5);

                return dictionary;
            }

            public static Dictionary<string, string> Tensao()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "110v");
                dictionary.Add("2", "220v");
                dictionary.Add("3", "380v");
                dictionary.Add("4", "440v");

                return dictionary;
            }

            public static Dictionary<string, string> UnidadeMedida()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "M²");
                dictionary.Add("2", "MH");
                dictionary.Add("3", "Kg");
                dictionary.Add("4", "Garrafas");

                return dictionary;
            }

            public static Dictionary<string, string> Limpeza()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "Limpo");
                dictionary.Add("2", "Poeira");
                dictionary.Add("3", "Sujo");
                dictionary.Add("0", "N/A");

                return dictionary;
            }

            public static Dictionary<string, string> Temperatura()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "Baixa");
                dictionary.Add("2", "Ambiente");
                dictionary.Add("3", "Alta");
                dictionary.Add("0", "N/A");

                return dictionary;
            }

            public static Dictionary<string, string> Umidade()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "Normal");
                dictionary.Add("2", "Úmido");
                dictionary.Add("0", "N/A");

                return dictionary;
            }

            public static Dictionary<string, string> TipoProduto()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "Caixa ");
                dictionary.Add("2", "Outros");

                return dictionary;
            }

            public static Dictionary<string, string> LocalInstalacao()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "Na esteira Motorizavel");
                dictionary.Add("2", "Na Esteira Roletada");
                dictionary.Add("3", "Outros");
                dictionary.Add("0", "N/A");

                return dictionary;
            }

            public static Dictionary<string, string> VelocidadeLinha()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "De 1 a 10 metros/minuto");
                dictionary.Add("2", "De 10 a 20 metros/minuto");
                dictionary.Add("3", "De 20 a 30 metros/minuto ");
                dictionary.Add("4", "Acima de 30 metros/minuto");

                return dictionary;
            }

            public static Dictionary<string, string> GuiaPosicionamento()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("1", "Existente ");
                dictionary.Add("2", "Inexistente ");
                dictionary.Add("0", "N/A");

                return dictionary;
            }

            public static Dictionary<int, string> VisaoPedidosWorkflow()
            {
                var dictionary = new Dictionary<int, string>();

                dictionary.Add(1, "Pendentes de minha ação");
                dictionary.Add(2, "Todos os Pedidos");
                dictionary.Add(3, "Meus Pedidos");

                return dictionary;
            }

            public static Dictionary<int, string> MotivoDevolucaoWorkflow()
            {
                var dictionary = new Dictionary<int, string>();

                dictionary.Add(1, "Troca por Outro Equipamento devido a desgaste e alto índice de manutenção");
                dictionary.Add(2, "Linha de produção descontinuada ou com baixo consumo");
                dictionary.Add(3, "Linha de produção transferida para outro site");
                dictionary.Add(4, "Cliente passará a usar produto da concorrência");
                dictionary.Add(5, "Modelo do equipamento não se adequa mais as necessidades da linha");
                dictionary.Add(6, "Equipamento enviado errado");
                dictionary.Add(7, "Outro motivo");

                return dictionary;
            }

            #endregion

            public static Dictionary<string, string> ClassificacaoKAT()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("A+", "A+");
                dictionary.Add("A-", "A-");
                dictionary.Add("B+", "B+");
                dictionary.Add("B-", "B-");
                dictionary.Add("C+", "C+");
                dictionary.Add("C-", "C-");
                dictionary.Add("-", "-");

                return dictionary;
            }
            public static Dictionary<int, string> TipoReclamacaoRR()
            {
                var dictionary = new Dictionary<int, string>();

                dictionary.Add(1, "Peça");
                dictionary.Add(2, "Equipamento");

                return dictionary;
            }

            public static Dictionary<int, string> TipoAtendimento()
            {
                var dictionary = new Dictionary<int, string>();

                dictionary.Add(1, "Atendimento técnico de campo");
                dictionary.Add(2, "Acionar Fornecedor");

                return dictionary;
            }

            public static Dictionary<string, string> Grupo()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("TECT", "TECT");
                dictionary.Add("TR3M", "TR3M");
                dictionary.Add("AT3M", "AT3M");
                return dictionary;
            }


            public static Dictionary<string, string> EADTecnico3m()
            {
                var dictionary = new Dictionary<string, string>();

                dictionary.Add("AppMobile", "AppMobile");
                dictionary.Add("Clientes", "Clientes");
          
                return dictionary;
            }

        }

        public class ImageUtility
        {
            public static string SaveImage(string directory, string base64String)
            {
                try
                {
                    var context = HttpContext.Current;

                    string fileName = $@"FOT_{Guid.NewGuid()}.jpeg";
                    string folder = context.Server.MapPath("~\\" + directory);
                    //string folder = System.Web.HttpContext.Current.Request.MapPath(directory);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64String)))
                    {
                        using (Bitmap bmp = new Bitmap(ms))
                        {
                            bmp.Save($"{folder}\\{fileName}", System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }

                    return fileName;
                }
                catch (Exception ex)
                {
                    LogUtility.LogarErro(ex);
                    throw ex;
                }
            }

        }
    }
}
