using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace _3M.Comodato.Data
{
    public class OSPadraoData
    {
        readonly Database _db;
        DbCommand dbCommand;
        private const int OS_STATUS_FINALIZADA = 3;
        private const int OS_STATUS_CANCELADO = 4;
        private const int OPERACAO_ESTOQUE_ENTRADA = 1;
        private const int OPERACAO_ESTOQUE_SAIDA = 2;

        public OSPadraoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref OSPadraoEntity osEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoInsert");

                _db.AddInParameter(dbCommand, "@p_DT_DATA_OS", DbType.DateTime, osEntity.DT_DATA_OS);
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_OS", DbType.Int32, osEntity.TpStatusOS.ST_STATUS_OS);
                _db.AddInParameter(dbCommand, "@p_CD_TIPO_OS", DbType.Int32, osEntity.TpOS.CD_TIPO_OS);
                if (osEntity.Cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, osEntity.Cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.Tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, osEntity.AtivoFixo.CD_ATIVO_FIXO);
                _db.AddInParameter(dbCommand, "@p_HR_INICIO", DbType.String, osEntity.HR_INICIO);
                _db.AddInParameter(dbCommand, "@p_HR_FIM", DbType.String, osEntity.HR_FIM);
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, osEntity.DS_OBSERVACAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, osEntity.TOKEN);
                _db.AddInParameter(dbCommand, "@p_NOME_LINHA", DbType.String, osEntity.NOME_LINHA);
                _db.AddInParameter(dbCommand, "@p_Email", DbType.String, osEntity.Email);
                _db.AddInParameter(dbCommand, "@p_DS_RESPONSAVEL", DbType.String, osEntity.DS_RESPONSAVEL);
                _db.AddInParameter(dbCommand, "@p_Origem", DbType.String, osEntity.Origem);
                if (osEntity.Criado != null)
                    _db.AddInParameter(dbCommand, "@p_create", DbType.String, osEntity.Criado);
                _db.AddOutParameter(dbCommand, "@p_ID_OS", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_TOKEN_GERADO", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                osEntity.ID_OS = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_OS"));
                osEntity.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

                if (osEntity.TpStatusOS.ST_STATUS_OS == 3)
                {
                    EnviarEmailOS(osEntity, osEntity.ID_OS);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Excluir(OSPadraoEntity osEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoDelete");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, osEntity.ID_OS);

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Alterar(OSPadraoEntity osEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.String, osEntity.ID_OS);

                if (osEntity.DT_DATA_OS != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_OS", DbType.DateTime, osEntity.DT_DATA_OS);

                if (osEntity.TpStatusOS.ST_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_OS", DbType.Int32, osEntity.TpStatusOS.ST_STATUS_OS);

                if (osEntity.TpOS.CD_TIPO_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_OS", DbType.Int32, osEntity.TpOS.CD_TIPO_OS);

                if (osEntity.Cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, osEntity.Cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(osEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.Tecnico.CD_TECNICO);

                if (!string.IsNullOrEmpty(osEntity.AtivoFixo.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, osEntity.AtivoFixo.CD_ATIVO_FIXO);

                if (!string.IsNullOrEmpty(osEntity.HR_INICIO))
                    _db.AddInParameter(dbCommand, "@p_HR_INICIO", DbType.String, osEntity.HR_INICIO);

                if (!string.IsNullOrEmpty(osEntity.HR_FIM))
                    _db.AddInParameter(dbCommand, "@p_HR_FIM", DbType.String, osEntity.HR_FIM);

                if(!string.IsNullOrEmpty(osEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, osEntity.DS_OBSERVACAO);

                if (!string.IsNullOrEmpty(osEntity.NOME_LINHA))
                    _db.AddInParameter(dbCommand, "@p_NOME_LINHA", DbType.String, osEntity.NOME_LINHA);

                if (!string.IsNullOrEmpty(osEntity.Email))
                    _db.AddInParameter(dbCommand, "@p_Email", DbType.String, osEntity.Email);

                if (!string.IsNullOrEmpty(osEntity.DS_RESPONSAVEL))
                    _db.AddInParameter(dbCommand, "@p_DS_RESPONSAVEL", DbType.String, osEntity.DS_RESPONSAVEL); 

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

                if (osEntity.TpStatusOS.ST_STATUS_OS == 3)
                {
                    EnviarEmailOS(osEntity, osEntity.ID_OS);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<OSPadraoEntity> ObterLista(OSPadraoEntity osEntity)
        {
            IList<OSPadraoEntity> listaOS = mapOS(ObterListaOs(osEntity, null, null));
            return listaOS;
        }

        
        public DataTable ObterListaOs(OSPadraoEntity osEntity, DateTime? DT_INICIO, DateTime? DT_FIM)
        {
            DbConnection connection = null;
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoSelect");

                if (osEntity.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.String, osEntity.ID_OS);

                if (osEntity.DT_DATA_OS != null && osEntity.DT_DATA_OS != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_OS", DbType.DateTime, osEntity.DT_DATA_OS);

                if (osEntity.TpStatusOS.ST_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_OS", DbType.Int32, osEntity.TpStatusOS.ST_STATUS_OS);

                if (osEntity.TpOS.CD_TIPO_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_OS", DbType.Int32, osEntity.TpOS.CD_TIPO_OS);

                if (osEntity.Cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, osEntity.Cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(osEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.Tecnico.CD_TECNICO);

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

                if (!string.IsNullOrEmpty(osEntity.Cliente.regiao.CD_REGIAO))
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, osEntity.Cliente.regiao.CD_REGIAO);

                if (DT_INICIO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIO", DbType.DateTime, DT_INICIO);

                if (DT_FIM != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FIM", DbType.DateTime, DT_FIM);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return dataTable;
        }


        public IList<OSPadraoEntity> ObterListaOSSincAbertas(OSPadraoEntity osEntity, Int32 ST_STATUS_OS)
        {
            DbConnection connection = null;
            IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoSelect");

                if (ST_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_OS", DbType.Int32, ST_STATUS_OS);

                if (!string.IsNullOrEmpty(osEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.Tecnico.CD_TECNICO);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                listaOS = mapOS(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return listaOS;
        }
        public IList<OSPadraoEntity> ObterListaOSSincHoras(OSPadraoEntity osEntity)
        {
            DbConnection connection = null;
            IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoSelectHoras");

                
                if (!string.IsNullOrEmpty(osEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.Tecnico.CD_TECNICO);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                listaOS = mapOS(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return listaOS;
        }


        public IList<OSPadraoEntity> ObterListaOSSinc(OSPadraoEntity osEntity)
        {
            DbConnection connection = null;
            IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPadraoSincSelect");

                if (osEntity.Tecnico.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int32, osEntity.Tecnico.usuario.nidUsuario);
                else
                {
                    if (osEntity.Tecnico.usuarioCoordenador.nidUsuario > 0)
                        _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int32, osEntity.Tecnico.usuarioCoordenador.nidUsuario);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                listaOS = mapOS(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return listaOS;
        }

        
        private IList<OSPadraoEntity> mapOS(DataTable dataTable)
        {
            IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();

            foreach (DataRow rdr in dataTable.Rows)
            {
                OSPadraoEntity os = new OSPadraoEntity();

                os.ID_OS = Convert.ToInt64(rdr["ID_OS"].ToString());
                os.DT_DATA_OS = Convert.ToDateTime(rdr["DT_DATA_OS"] is DBNull ? "01/01/2000" : rdr["DT_DATA_OS"]);
                os.HR_INICIO = Convert.ToString(rdr["HR_INICIO"] is DBNull ? "" : rdr["HR_INICIO"].ToString());
                os.HR_FIM = Convert.ToString(rdr["HR_FIM"] is DBNull ? "" : rdr["HR_FIM"].ToString());
                os.DS_OBSERVACAO = Convert.ToString(rdr["DS_OBSERVACAO"] is DBNull ? "" : rdr["DS_OBSERVACAO"].ToString());
                os.NOME_LINHA = Convert.ToString(rdr["NOME_LINHA"] is DBNull ? "" : rdr["NOME_LINHA"].ToString());
                os.Email = Convert.ToString(rdr["Email"] is DBNull ? "" : rdr["Email"].ToString());
                os.DS_RESPONSAVEL = Convert.ToString(rdr["DS_RESPONSAVEL"] is DBNull ? "" : rdr["DS_RESPONSAVEL"].ToString());
                os.nidUsuarioAtualizacao = Convert.ToInt64(rdr["nidUsuarioAtualizacao"] is DBNull ? 0 : rdr["nidUsuarioAtualizacao"]);
                os.dtmDataHoraAtualizacao = Convert.ToDateTime(rdr["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : rdr["dtmDataHoraAtualizacao"]);

                os.QT_PERIODO = Convert.ToInt32(rdr["QT_PERIODO"] is DBNull ? 0 : rdr["QT_PERIODO"]);

                os.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(rdr["ST_STATUS_OS"] is DBNull ? 0 : rdr["ST_STATUS_OS"]);
                os.TpStatusOS.DS_STATUS_OS = Convert.ToString(rdr["DS_STATUS_OS"] is DBNull ? "" : rdr["DS_STATUS_OS"]);

                os.TpOS.CD_TIPO_OS = Convert.ToInt32(rdr["CD_TIPO_OS"] is DBNull ? 0 : rdr["CD_TIPO_OS"]);
                os.TpOS.DS_TIPO_OS = Convert.ToString(rdr["DS_TIPO_OS"] is DBNull ? "" : rdr["DS_TIPO_OS"]);

                os.Cliente.CD_CLIENTE = Convert.ToInt64(rdr["CD_CLIENTE"] is DBNull ? 0 : rdr["CD_CLIENTE"]);
                os.Cliente.NM_CLIENTE = Convert.ToString(rdr["NM_CLIENTE"] is DBNull ? "" : rdr["NM_CLIENTE"].ToString());
                os.Cliente.EN_CIDADE = Convert.ToString(rdr["EN_CIDADE"] is DBNull ? "" : rdr["EN_CIDADE"].ToString());
                os.Cliente.EN_ESTADO = Convert.ToString(rdr["EN_ESTADO"] is DBNull ? "" : rdr["EN_ESTADO"].ToString());
                os.Cliente.EN_ENDERECO = Convert.ToString(rdr["EN_ENDERECO"] is DBNull ? "" : rdr["EN_ENDERECO"].ToString());
                os.Cliente.EN_BAIRRO = Convert.ToString(rdr["EN_BAIRRO"] is DBNull ? "" : rdr["EN_BAIRRO"].ToString());
                os.Cliente.EN_CEP = Convert.ToString(rdr["EN_CEP"] is DBNull ? "" : rdr["EN_CEP"].ToString());

                os.Cliente.regiao.CD_REGIAO = Convert.ToString(rdr["CD_REGIAO"] is DBNull ? "" : rdr["CD_REGIAO"].ToString());
                os.Cliente.regiao.DS_REGIAO = Convert.ToString(rdr["DS_REGIAO"] is DBNull ? "" : rdr["DS_REGIAO"].ToString());

                os.Tecnico.CD_TECNICO = Convert.ToString(rdr["CD_TECNICO"] is DBNull ? "" : rdr["CD_TECNICO"].ToString());
                os.Tecnico.NM_TECNICO = Convert.ToString(rdr["NM_TECNICO"] is DBNull ? "" : rdr["NM_TECNICO"].ToString());

                os.Tecnico.empresa.CD_Empresa = Convert.ToInt64(rdr["CD_EMPRESA"] is DBNull ? 0 : rdr["CD_EMPRESA"]);
                os.Tecnico.empresa.NM_Empresa = Convert.ToString(rdr["NM_EMPRESA"] is DBNull ? "" : rdr["NM_EMPRESA"].ToString());

                os.AtivoFixo.CD_ATIVO_FIXO = Convert.ToString(rdr["CD_ATIVO_FIXO"] is DBNull ? "" : rdr["CD_ATIVO_FIXO"].ToString());
                os.AtivoFixo.TX_ANO_MAQUINA = Convert.ToString(rdr["TX_ANO_MÁQUINA"] is DBNull ? "" : rdr["TX_ANO_MÁQUINA"].ToString());

                os.AtivoFixo.modelo.DS_MODELO = Convert.ToString(rdr["DS_MODELO"] is DBNull ? "" : rdr["DS_MODELO"].ToString());

                os.TecnicoCliente.CD_ORDEM = Convert.ToInt32(rdr["CD_ORDEM"] is DBNull ? 0 : rdr["CD_ORDEM"]);
                os.TOKEN = Convert.ToInt64(rdr["TOKEN"].ToString());

                listaOS.Add(os);
            }

            return listaOS;
        }

        public void RealizarAtualizacaoEstoque(PecaOSSinc pecaOS, OSPadraoEntity osPadrao, long idUsuario, int EntradaouSaida)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaOSMovimentaEstoque");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, pecaOS.ID_OS);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pecaOS.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, pecaOS.QT_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, pecaOS.CD_TP_ESTOQUE_CLI_TEC);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osPadrao.Tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, osPadrao.Cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, idUsuario);

                _db.AddInParameter(dbCommand, "@p_Tipo_Movimentacao", DbType.Byte, EntradaouSaida);

                _db.AddOutParameter(dbCommand, "@p_Mensagem", DbType.String, 8000);

                _db.AddOutParameter(dbCommand, "@p_Permite_Movimentar_Estoque", DbType.Byte, 1);

                
                _db.ExecuteNonQuery(dbCommand);

                var mensagemValidacaoMovimentaEstoque = _db.GetParameterValue(dbCommand, "@p_Mensagem").ToString();

                if (!string.IsNullOrWhiteSpace(mensagemValidacaoMovimentaEstoque))
                    throw new Exception($"{mensagemValidacaoMovimentaEstoque} OS: {pecaOS.ID_OS} Peça: {pecaOS.CD_PECA}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RealizarAtualizacaoStatusPendenciaCancelada(Int64 Id_Pendencia)
        {
            try
            {
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"update tbPendenciaOS
                                    set ST_STATUS_PENDENCIA = '2',
                                        ST_TP_PENDENCIA = 'C'
                                    where ID_PENDENCIA_OS = @Id_Pendencia";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Id_Pendencia", SqlDbType.BigInt).Value = Id_Pendencia;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CancelarReclamacaoOS(Int64 Id_OS)
        {
            try
            {
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"delete from
                                    tbRRRelatorioReclamacao
                                    where ID_OS = @Id_OS";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Id_OS", SqlDbType.BigInt).Value = Id_OS;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RealizarAtualizacaoStatusPendenciaFinalizada(Int64 Id_Pendencia)
        {
            try
            {
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"update tbPendenciaOS
                                    set ST_STATUS_PENDENCIA = '2',
                                        ST_TP_PENDENCIA = 'F'
                                    where ID_PENDENCIA_OS = @Id_Pendencia";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Id_Pendencia", SqlDbType.BigInt).Value = Id_Pendencia;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region email
        public class Parametro
        {
            /// <summary>
            /// Busca o conteúdo de cdsParametro da tabela tbParametro
            /// </summary>
            /// <param name="ccdParametro">Código</param>
            /// <returns>Descrição</returns>
            public static string ObterValorParametro(string ccdParametro)
            {
                //if (ccdParametro.ToUpper() == Constantes.URLSite.ToUpper())
                //{
                //    return ConfigurationManager.AppSettings[Constantes.URLSite];
                //}
                //else if (ccdParametro.ToUpper() == Constantes.URLAPI.ToUpper())
                //{
                //    return ConfigurationManager.AppSettings[Constantes.URLAPI];
                //}

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
                smtp.Host = Parametro.ObterValorParametro(Constantes.MailHost);
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
                public const string MensagemExclusaoSucesso = "Registro excluído com sucesso!";
                public const string MensagemInativacaoSucesso = "Registro inativado com sucesso!";

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

        public class ControlesUtility
        {

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
                public const string MensagemExclusaoSucesso = "Registro excluído com sucesso!";
                public const string MensagemInativacaoSucesso = "Registro inativado com sucesso!";

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


            public class Parametro
            {
                /// <summary>
                /// Busca o conteúdo de cdsParametro da tabela tbParametro
                /// </summary>
                /// <param name="ccdParametro">Código</param>
                /// <returns>Descrição</returns>
                public static string ObterValorParametro(string ccdParametro)
                {

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


        }

        public void EnviarEmailOS(OSPadraoEntity listOSPadrao, long? Id_Os)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");


                if (listOSPadrao.TpStatusOS.ST_STATUS_OS == 3)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = listOSPadrao.Tecnico.CD_TECNICO;
                    DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.usuario.cdsEmail = "";
                            tecnicoEntity.usuarioCoordenador.cdsEmail = "";
                            if (dataTableReader["cdsEmail"] != DBNull.Value)
                            {
                                tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                            }
                            if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                            {
                                tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                            }
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = listOSPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            _clienteEntity.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                            _clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
                            _clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }


                    // Envia a requisição de troca de senha por e-mail



                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    var pecasOS = new PecaOSData().ObterListaPecaOsEmail(listOSPadrao.ID_OS);

                    if (listOSPadrao.Email == null || listOSPadrao.Email == "")
                    {
                        //if (tecnicoEntity.usuario.cdsEmail != "")
                        //{
                        //    mailTo += tecnicoEntity.usuario.cdsEmail + ";";
                        //}
                        if (_clienteEntity.TX_EMAIL != "")
                        {
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        }
                    }
                    else if (listOSPadrao.Email != null && listOSPadrao.Email != "")
                    {
                        if (listOSPadrao.Email == tecnicoEntity.usuario.cdsEmail)
                            mailTo += _clienteEntity.TX_EMAIL + ";";
                        else
                            mailTo += listOSPadrao.Email + ";";
                    }



                    string mailSubject = "3M.Comodato - Finalização de OS";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");
                    URLSite += "/OsPadrao/Pesquisa?ID_OS=" + Id_Os;
                    Conteudo += "<p>Uma OS acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da OS:</p>";
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";

                    if (Id_Os != null && Id_Os != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(Id_Os) + "</strong><br/>";
                    }
                    else if (listOSPadrao.ID_OS != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(listOSPadrao.ID_OS) + "</strong><br/>";
                    }

                    Conteudo += "Data: " + Convert.ToDateTime(listOSPadrao.DT_DATA_OS).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora Inicio: " + listOSPadrao.HR_INICIO.ToString() + "<br/>";
                    Conteudo += "Hora de Finalização: " + listOSPadrao.HR_FIM.ToString() + "<br/>";
                    Conteudo += "Ativo Fixo: " + listOSPadrao.AtivoFixo.CD_ATIVO_FIXO + "-" + listOSPadrao.AtivoFixo.modelo.DS_MODELO + "<br/>";
                    Conteudo += "Linha: " + listOSPadrao.NOME_LINHA + "<br/>";
                    Conteudo += "Observação: " + listOSPadrao.DS_OBSERVACAO + "<br/>";
                    if (listOSPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + listOSPadrao.DS_RESPONSAVEL.ToString() + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }

                    var manutencao = "";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 1)
                        manutencao = "Preventiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 2)
                        manutencao = "Corretiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 3)
                        manutencao = "Instalação";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 4)
                        manutencao = "Outros";
                    Conteudo += "Tipo de Manutenção: " + manutencao + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/><br/>";

                    if (pecasOS?.Count > 0)
                    {
                        Conteudo += "<p><strong>Peças Utilizadas: </strong></p>";
                        int contaPc = 0;
                        foreach (var peca in pecasOS)
                        {
                            contaPc++;
                            var estoque = peca.CD_TP_ESTOQUE_CLI_TEC == 'C' ? "Cliente" : "Intermediario";
                            Conteudo += $" {contaPc}. {peca.DS_PECA} || Quantidade: {peca.QT_PECA} || Estoque: {estoque}<br/>";
                        }
                    }


                    var button = mailSender.GetConteudoHTML("button.html");

                    button.Replace("TEXTO_URL", "Avalie o Atendimento");
                    button.Replace("URL", URLSite);

                    Conteudo += button;
                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";
                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                    if (_clienteEntity.EmailsInfo != null || _clienteEntity.EmailsInfo != "")
                    {
                        EnviarEmailOSInfo(listOSPadrao, Id_Os);
                    }
                }



            }
            catch (Exception ex)
            {


            }
        }


        public void EnviarEmailOSInfo(OSPadraoEntity listOSPadrao, long? Id_Os)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");


                if (listOSPadrao.TpStatusOS.ST_STATUS_OS == 3)
                {
                    TecnicoEntity tecnicoEntity = new TecnicoEntity();
                    tecnicoEntity.CD_TECNICO = listOSPadrao.Tecnico.CD_TECNICO;
                    DataTableReader dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.empresa.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                            tecnicoEntity.usuario.cdsEmail = "";
                            tecnicoEntity.usuarioCoordenador.cdsEmail = "";
                            if (dataTableReader["cdsEmail"] != DBNull.Value)
                            {
                                tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                            }
                            if (dataTableReader["cdsEmailCoordenador"] != DBNull.Value)
                            {
                                tecnicoEntity.usuarioCoordenador.cdsEmail = dataTableReader["cdsEmailCoordenador"].ToString();
                            }
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }

                    ClienteEntity _clienteEntity = new ClienteEntity();
                    _clienteEntity.CD_CLIENTE = listOSPadrao.Cliente.CD_CLIENTE;
                    dataTableReader = new ClienteData().ObterLista(_clienteEntity).CreateDataReader();

                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            _clienteEntity.CD_CLIENTE = Convert.ToInt64(dataTableReader["CD_CLIENTE"]);
                            _clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
                            _clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                            _clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
                            _clienteEntity.EmailsInfo = dataTableReader["EmailsInfo"].ToString();
                        }
                    }

                    if (dataTableReader != null)
                    {
                        dataTableReader.Dispose();
                        dataTableReader = null;
                    }


                    // Envia a requisição de troca de senha por e-mail

                    MailSender mailSender = new MailSender();

                    string mailTo = "";

                    var pecasOS = new PecaOSData().ObterListaPecaOsEmail(listOSPadrao.ID_OS);

                    mailTo = _clienteEntity.EmailsInfo;

                    string mailSubject = "3M.Comodato - Finalização de OS";
                    string mailMessage = string.Empty;
                    System.Net.Mail.Attachment Attachments = null;
                    string mailCopy = null;

                    var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                    string Conteudo = string.Empty;
                    string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                    //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");

                    Conteudo += "<p>Uma OS acaba de ser <strong>Finalizada </strong>!</p>";
                    Conteudo += "<p>Segue dados da OS:</p>";
                    Conteudo += "Cliente: " + _clienteEntity.CD_CLIENTE + " - " + _clienteEntity.NM_CLIENTE + "<br/>";
                    Conteudo += "Cidade: " + _clienteEntity.EN_CIDADE + "<br/>";
                    Conteudo += "Técnico: " + tecnicoEntity.NM_TECNICO + " - " + tecnicoEntity.empresa.NM_Empresa + "<br/>";

                    if (Id_Os != null && Id_Os != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(Id_Os) + "</strong><br/>";
                    }
                    else if (listOSPadrao.ID_OS != 0)
                    {
                        Conteudo += "OS: <strong>" + Convert.ToInt64(listOSPadrao.ID_OS) + "</strong><br/>";
                    }

                    Conteudo += "Data: " + Convert.ToDateTime(listOSPadrao.DT_DATA_OS).ToString("dd/MM/yyyy") + "<br/>";
                    Conteudo += "Hora Inicio: " + listOSPadrao.HR_INICIO.ToString() + "<br/>";
                    Conteudo += "Hora de Finalização: " + listOSPadrao.HR_FIM.ToString() + "<br/>";
                    Conteudo += "Ativo Fixo: " + listOSPadrao.AtivoFixo.CD_ATIVO_FIXO + "-" + listOSPadrao.AtivoFixo.modelo.DS_MODELO + "<br/>";
                    Conteudo += "Linha: " + listOSPadrao.NOME_LINHA + "<br/>";
                    Conteudo += "Observação: " + listOSPadrao.DS_OBSERVACAO + "<br/>";
                    if (listOSPadrao.DS_RESPONSAVEL != null)
                    {
                        Conteudo += "Acompanhante: " + listOSPadrao.DS_RESPONSAVEL.ToString() + "<br/>";
                    }
                    else
                    {
                        Conteudo += "Acompanhante: " + "" + "<br/>";
                    }

                    var manutencao = "";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 1)
                        manutencao = "Preventiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 2)
                        manutencao = "Corretiva";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 3)
                        manutencao = "Instalação";
                    if (listOSPadrao.TpOS.CD_TIPO_OS == 4)
                        manutencao = "Outros";
                    Conteudo += "Tipo de Manutenção: " + manutencao + "<br/>";
                    Conteudo += "Status da OS: Finalizada<br/><br/>";

                    if (pecasOS?.Count > 0)
                    {
                        Conteudo += "<p><strong>Peças Utilizadas: </strong></p>";
                        int contaPc = 0;
                        foreach (var peca in pecasOS)
                        {
                            contaPc++;
                            var estoque = peca.CD_TP_ESTOQUE_CLI_TEC == 'C' ? "Cliente" : "Intermediario";
                            Conteudo += $" {contaPc}. {peca.DS_PECA} || Quantidade: {peca.QT_PECA} || Estoque: {estoque}<br/>";
                        }
                    }

                    //Conteudo += $"<a class='btn btn-danger m-1' href = '{URLSite}'>Avalie o Atendimento</a>";
                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();

                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                }



            }
            catch (Exception ex)
            {


            }
        }



        #endregion
    }
}
