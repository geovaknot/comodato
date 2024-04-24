using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class VisitaPadraoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public VisitaPadraoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref VisitaPadraoEntity visitaEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoInsert");

                _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_VISITA", DbType.Int32, visitaEntity.TpStatusVisita.ST_STATUS_VISITA);
                if (visitaEntity.Cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.Cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.Tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);
                _db.AddInParameter(dbCommand, "@p_HR_INICIO", DbType.String, visitaEntity.HR_INICIO);
                _db.AddInParameter(dbCommand, "@p_HR_FIM", DbType.String, visitaEntity.HR_FIM);
                _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_VISITA", DbType.Int32, visitaEntity.TpMotivoVisita.CD_MOTIVO_VISITA);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, visitaEntity.TOKEN);
                _db.AddInParameter(dbCommand, "@p_Email", DbType.String, visitaEntity.Email);
                _db.AddInParameter(dbCommand, "@p_DS_RESPONSAVEL", DbType.String, visitaEntity.DS_RESPONSAVEL);
                _db.AddInParameter(dbCommand, "@p_Origem", DbType.String, visitaEntity.Origem);
                _db.AddOutParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_TOKEN_GERADO", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                visitaEntity.ID_VISITA = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_VISITA"));
                visitaEntity.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InserirPesquisa(VisitaRespostaEntity visitaResposta)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoRespostaInsert");

                _db.AddInParameter(dbCommand, "@p_DT_DATA", DbType.DateTime, visitaResposta.DataResposta);
                _db.AddInParameter(dbCommand, "@p_ID_PESQUISA_SATISF", DbType.Int32, visitaResposta.ID_PESQUISA_SATISF);
                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, visitaResposta.ID_OS);
                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, visitaResposta.ID_VISITA);
                _db.AddInParameter(dbCommand, "@p_Justificativa", DbType.String, visitaResposta.Justificativa);
                _db.AddInParameter(dbCommand, "@p_NotaPesquisa", DbType.Decimal, Convert.ToDecimal(visitaResposta.NotaPesquisa));
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaResposta.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_DS_NOME_RESPONDEDOR", DbType.String, visitaResposta.DS_NOME_RESPONDEDOR);

                _db.ExecuteNonQuery(dbCommand);

               
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public void Excluir(VisitaPadraoEntity visitaEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoDelete");

                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, visitaEntity.ID_VISITA);

                if (visitaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Alterar(VisitaPadraoEntity visitaEntity)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.String, visitaEntity.ID_VISITA);

                if (visitaEntity.DT_DATA_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);

                if (visitaEntity.TpStatusVisita.ST_STATUS_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_VISITA", DbType.Int32, visitaEntity.TpStatusVisita.ST_STATUS_VISITA);

                if (visitaEntity.Cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.Cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(visitaEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.Tecnico.CD_TECNICO);

                if (!string.IsNullOrEmpty(visitaEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);

                if (!string.IsNullOrEmpty(visitaEntity.Email))
                    _db.AddInParameter(dbCommand, "@p_Email", DbType.String, visitaEntity.Email);

                if (!string.IsNullOrEmpty(visitaEntity.DS_RESPONSAVEL))
                    _db.AddInParameter(dbCommand, "@p_DS_RESPONSAVEL", DbType.String, visitaEntity.DS_RESPONSAVEL);

                if (!string.IsNullOrEmpty(visitaEntity.HR_INICIO))
                    _db.AddInParameter(dbCommand, "@p_HR_INICIO", DbType.String, visitaEntity.HR_INICIO);

                if (!string.IsNullOrEmpty(visitaEntity.HR_FIM))
                    _db.AddInParameter(dbCommand, "@p_HR_FIM", DbType.String, visitaEntity.HR_FIM);

                if (visitaEntity.TpMotivoVisita.CD_MOTIVO_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_VISITA", DbType.Int32, visitaEntity.TpMotivoVisita.CD_MOTIVO_VISITA);

                if (visitaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<VisitaPadraoEntity> ObterListaVisita(VisitaPadraoEntity visitaEntity)
        {
            IList<VisitaPadraoEntity> listaVisita = mapVisita(ObterLista(visitaEntity, null, null));
            return listaVisita;
        }

        public DataTable ObterVisitaById(VisitaPadraoEntity visitaEntity, DateTime? DT_INICIO, DateTime? DT_FIM)
        {
            DbConnection connection = null;
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoSelect");

                if (visitaEntity.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.String, visitaEntity.ID_VISITA);

                if (visitaEntity.DT_DATA_VISITA != null && visitaEntity.DT_DATA_VISITA != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);

                if (visitaEntity.TpStatusVisita.ST_STATUS_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_VISITA", DbType.Int32, visitaEntity.TpStatusVisita.ST_STATUS_VISITA);

                if (visitaEntity.Cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.Cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(visitaEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.Tecnico.CD_TECNICO);

                if (!string.IsNullOrEmpty(visitaEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);


                if (visitaEntity.TpMotivoVisita.CD_MOTIVO_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_VISITA", DbType.Int32, visitaEntity.TpMotivoVisita.CD_MOTIVO_VISITA);

                if (visitaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);

                if (!string.IsNullOrEmpty(visitaEntity.Cliente.regiao.CD_REGIAO))
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, visitaEntity.Cliente.regiao.CD_REGIAO);

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

        public DataTable ObterLista(VisitaPadraoEntity visitaEntity, DateTime? DT_INICIO, DateTime? DT_FIM)
        {
            DbConnection connection = null;
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoSelect");

                if (visitaEntity.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.String, visitaEntity.ID_VISITA);

                if (visitaEntity.DT_DATA_VISITA != null && visitaEntity.DT_DATA_VISITA != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);

                if (visitaEntity.TpStatusVisita.ST_STATUS_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_VISITA", DbType.Int32, visitaEntity.TpStatusVisita.ST_STATUS_VISITA);

                if (visitaEntity.Cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.Cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(visitaEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.Tecnico.CD_TECNICO);

                if (!string.IsNullOrEmpty(visitaEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);


                if (visitaEntity.TpMotivoVisita.CD_MOTIVO_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_VISITA", DbType.Int32, visitaEntity.TpMotivoVisita.CD_MOTIVO_VISITA);

                if (visitaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);

                if (!string.IsNullOrEmpty(visitaEntity.Cliente.regiao.CD_REGIAO))
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, visitaEntity.Cliente.regiao.CD_REGIAO);

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

        public IList<VisitaPadraoEntity> ObterListaVisitaSincAbertas(VisitaPadraoEntity visitaEntity, Int32 ST_STATUS_VISITA)
        {
            DbConnection connection = null;
            IList<VisitaPadraoEntity> listaVisita = new List<VisitaPadraoEntity>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoSelect");

                if (ST_STATUS_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_VISITA", DbType.Int32, ST_STATUS_VISITA);

                if (!string.IsNullOrEmpty(visitaEntity.Tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.Tecnico.CD_TECNICO);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                listaVisita = mapVisita(dataSet.Tables[0]);
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

            return listaVisita;
        }
        public IList<VisitaPadraoEntity> ObterListaVisitaSinc(VisitaPadraoEntity visitaEntity)
        {
            DbConnection connection = null;
            IList<VisitaPadraoEntity> listaVisita = new List<VisitaPadraoEntity>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoSincSelect");

                if (visitaEntity.Tecnico.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int32, visitaEntity.Tecnico.usuario.nidUsuario);
                else
                {
                    if (visitaEntity.Tecnico.usuarioCoordenador.nidUsuario > 0)
                        _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int32, visitaEntity.Tecnico.usuarioCoordenador.nidUsuario);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                listaVisita = mapVisita(dataSet.Tables[0]);
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

            return listaVisita;
        }

        public IList<VisitaPadraoEntity> ObterListaVisitaSincHoras(VisitaPadraoEntity visitaEntity)
        {
            DbConnection connection = null;
            IList<VisitaPadraoEntity> listaVisita = new List<VisitaPadraoEntity>();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaPadraoSincSelectHORAS");

                if (visitaEntity.Tecnico.CD_TECNICO != null)
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.Tecnico.CD_TECNICO);
                

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                listaVisita = mapVisita(dataSet.Tables[0]);
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

            return listaVisita;
        }

        private IList<VisitaPadraoEntity> mapVisita(DataTable dataTable)
        {
            IList<VisitaPadraoEntity> listaVisita = new List<VisitaPadraoEntity>();

            foreach (DataRow dr in dataTable.Rows)
            {
                VisitaPadraoEntity visita = new VisitaPadraoEntity();

                visita.ID_VISITA = Convert.ToInt64(dr["ID_VISITA"].ToString());
                visita.DT_DATA_VISITA = Convert.ToDateTime(dr["DT_DATA_VISITA"] is DBNull ? "01/01/2000" : dr["DT_DATA_VISITA"]);
                visita.DS_OBSERVACAO = Convert.ToString(dr["DS_OBSERVACAO"] is DBNull ? "" : dr["DS_OBSERVACAO"].ToString());
                visita.Email = Convert.ToString(dr["Email"] is DBNull ? "" : dr["Email"].ToString());
                visita.DS_RESPONSAVEL = Convert.ToString(dr["DS_RESPONSAVEL"] is DBNull ? "" : dr["DS_RESPONSAVEL"].ToString());
                visita.HR_INICIO = Convert.ToString(dr["HR_INICIO"] is DBNull ? "" : dr["HR_INICIO"].ToString());
                visita.HR_FIM = Convert.ToString(dr["HR_FIM"] is DBNull ? "" : dr["HR_FIM"].ToString());
                visita.nidUsuarioAtualizacao = Convert.ToInt64(dr["nidUsuarioAtualizacao"] is DBNull ? 0 : dr["nidUsuarioAtualizacao"]);
                visita.dtmDataHoraAtualizacao = Convert.ToDateTime(dr["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : dr["dtmDataHoraAtualizacao"]);

                visita.TpStatusVisita.ST_STATUS_VISITA = Convert.ToInt32(dr["ST_STATUS_VISITA"] is DBNull ? 0 : dr["ST_STATUS_VISITA"]);
                visita.TpStatusVisita.DS_STATUS_VISITA = Convert.ToString(dr["DS_STATUS_VISITA"] is DBNull ? "" : dr["DS_STATUS_VISITA"]);

                visita.TpMotivoVisita.CD_MOTIVO_VISITA = Convert.ToInt32(dr["CD_MOTIVO_VISITA"] is DBNull ? 0 : dr["CD_MOTIVO_VISITA"]);
                visita.TpMotivoVisita.DS_MOTIVO_VISITA = Convert.ToString(dr["DS_MOTIVO_VISITA"] is DBNull ? "" : dr["DS_MOTIVO_VISITA"]);

                visita.Cliente.CD_CLIENTE = Convert.ToInt64(dr["CD_CLIENTE"] is DBNull ? 0 : dr["CD_CLIENTE"]);
                visita.Cliente.NM_CLIENTE = Convert.ToString(dr["NM_CLIENTE"] is DBNull ? "" : dr["NM_CLIENTE"].ToString());
                visita.Cliente.EN_CIDADE = Convert.ToString(dr["EN_CIDADE"] is DBNull ? "" : dr["EN_CIDADE"].ToString());
                visita.Cliente.EN_ESTADO = Convert.ToString(dr["EN_ESTADO"] is DBNull ? "" : dr["EN_ESTADO"].ToString());
                visita.Cliente.EN_ENDERECO = Convert.ToString(dr["EN_ENDERECO"] is DBNull ? "" : dr["EN_ENDERECO"].ToString());
                visita.Cliente.EN_BAIRRO = Convert.ToString(dr["EN_BAIRRO"] is DBNull ? "" : dr["EN_BAIRRO"].ToString());
                visita.Cliente.EN_CEP = Convert.ToString(dr["EN_CEP"] is DBNull ? "" : dr["EN_CEP"].ToString());

                visita.Cliente.regiao.CD_REGIAO = Convert.ToString(dr["CD_REGIAO"] is DBNull ? "" : dr["CD_REGIAO"].ToString());
                visita.Cliente.regiao.DS_REGIAO = Convert.ToString(dr["DS_REGIAO"] is DBNull ? "" : dr["DS_REGIAO"].ToString());

                visita.Tecnico.CD_TECNICO = Convert.ToString(dr["CD_TECNICO"] is DBNull ? "" : dr["CD_TECNICO"].ToString());
                visita.Tecnico.NM_TECNICO = Convert.ToString(dr["NM_TECNICO"] is DBNull ? "" : dr["NM_TECNICO"].ToString());

                visita.Tecnico.empresa.CD_Empresa = Convert.ToInt64(dr["CD_EMPRESA"] is DBNull ? 0 : dr["CD_EMPRESA"]);
                visita.Tecnico.empresa.NM_Empresa = Convert.ToString(dr["NM_EMPRESA"] is DBNull ? "" : dr["NM_EMPRESA"].ToString());

                visita.TecnicoCliente.CD_ORDEM = Convert.ToInt32(dr["CD_ORDEM"] is DBNull ? 0 : dr["CD_ORDEM"]);
                visita.TOKEN = Convert.ToInt64(dr["TOKEN"].ToString());

                listaVisita.Add(visita);
            }

            return listaVisita;
        }

        public IList<VisitaRespostaEntity> ObterListaRespostasOS(VisitaRespostaEntity visitaResposta)
        {
            try
            {
                IList<VisitaRespostaEntity> listaEsposta = new List<VisitaRespostaEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         @"select * from tbSatisfResposta where ID_OS = @ID_OS";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_OS", SqlDbType.BigInt).Value = visitaResposta.ID_OS;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            VisitaRespostaEntity resposta = new VisitaRespostaEntity
                            {
                                ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString()),
                                ID_VISITA = Convert.ToInt32(SDR["ID_VISITA"].ToString())
                            };

                            listaEsposta.Add(resposta);
                        }
                        cnx.Close();
                        return listaEsposta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<VisitaRespostaEntity> ObterListaRespostasVISITA(VisitaRespostaEntity visitaResposta)
        {
            try
            {
                IList<VisitaRespostaEntity> listaEsposta = new List<VisitaRespostaEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         @"select * from tbSatisfResposta where ID_VISITA = @ID_VISITA";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_VISITA", SqlDbType.BigInt).Value = visitaResposta.ID_VISITA;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            VisitaRespostaEntity resposta = new VisitaRespostaEntity
                            {
                                ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString()),
                                ID_VISITA = Convert.ToInt32(SDR["ID_VISITA"].ToString())
                            };

                            listaEsposta.Add(resposta);
                        }
                        cnx.Close();
                        return listaEsposta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
