using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class AgendaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public AgendaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref AgendaEntity agendaEntity)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAgendaInsert");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, agendaEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, agendaEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_NR_ORDENACAO", DbType.Int32, agendaEntity.NR_ORDENACAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, agendaEntity.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_ID_AGENDA", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                agendaEntity.ID_AGENDA = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_AGENDA"));

                retorno = true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public bool Alterar(AgendaEntity agendaEntity)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAgendaUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_AGENDA", DbType.Int64, agendaEntity.ID_AGENDA);

                if (agendaEntity.cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, agendaEntity.cliente.CD_CLIENTE);
                }

                if (!string.IsNullOrEmpty(agendaEntity.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, agendaEntity.tecnico.CD_TECNICO);
                }

                if (agendaEntity.NR_ORDENACAO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_ORDENACAO", DbType.Int32, agendaEntity.NR_ORDENACAO);
                }

                if (agendaEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, agendaEntity.nidUsuarioAtualizacao);
                }

                _db.ExecuteNonQuery(dbCommand);

                retorno = true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public void Excluir(AgendaEntity agendaEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAgendaDelete");

                _db.AddInParameter(dbCommand, "@p_ID_AGENDA", DbType.Int64, agendaEntity.ID_AGENDA);

                if (agendaEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, agendaEntity.nidUsuarioAtualizacao);
                }

                _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ObterLista(AgendaEntity agendaEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAgendaSelect");

                if (agendaEntity.ID_AGENDA != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_AGENDA", DbType.Int64, agendaEntity.ID_AGENDA);
                }

                if (!string.IsNullOrEmpty(agendaEntity.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, agendaEntity.tecnico.CD_TECNICO);
                }

                if (agendaEntity.cliente.CD_CLIENTE != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_Cliente", DbType.Int32, agendaEntity.cliente.CD_CLIENTE);
                }

                if (agendaEntity.NR_ORDENACAO != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_NR_ORDENACAO", DbType.Int32, agendaEntity.NR_ORDENACAO);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaAtendimentoRelatorio(List<string> listaTecnico, List<string> listaStatus, Int64? nidUsuario = null, DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptAgendaSelectAtendimento");

                if (listaTecnico.Count > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CDS_TECNICO", DbType.String, string.Join(",", listaTecnico));
                }
                if (listaStatus.Count > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_STS_TP_STATUS_VISITA_OS", DbType.String, string.Join(",", listaStatus));
                }

                if (nidUsuario != null)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int32, nidUsuario);
                }

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@pDtInicial", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@pDtFinal", DbType.Date, dataFinal);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable ObterListaAtendimentoVisitaRelatorio(List<string> listaTecnico, List<string> listaStatus, Int64? nidUsuario = null, DateTime? dataInicial = null, DateTime? dataFinal = null)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptAgendaVisitaSelectAtendimento");

                if (listaTecnico.Count > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CDS_TECNICO", DbType.String, string.Join(",", listaTecnico));
                }
                if (listaStatus.Count > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_STS_TP_STATUS_VISITA_OS", DbType.String, string.Join(",", listaStatus));
                }

                if (nidUsuario != null)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int32, nidUsuario);
                }

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@pDtInicial", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@pDtFinal", DbType.Date, dataFinal);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }
        public DataTable ObterListaAtendimento(AgendaEntity agendaEntity, string CD_REGIAO, int? nvlQtdeTecnicos = null, Int64? nidUsuario = null, int? ST_TP_STATUS_VISITA_OS = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAgendaSelectAtendimento2Tec");

                if (nidUsuario != null)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int32, nidUsuario);
                }

                if (!string.IsNullOrEmpty(agendaEntity.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, agendaEntity.tecnico.CD_TECNICO);
                }

                if (agendaEntity.cliente.CD_CLIENTE != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_Cliente", DbType.Int32, agendaEntity.cliente.CD_CLIENTE);
                }

                if (nvlQtdeTecnicos != null)
                {
                    _db.AddInParameter(dbCommand, "@p_nvlQtdeTecnicos", DbType.Int32, nvlQtdeTecnicos);
                }

                if (ST_TP_STATUS_VISITA_OS != null)
                {
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, ST_TP_STATUS_VISITA_OS);
                }

                if (!string.IsNullOrEmpty(CD_REGIAO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, CD_REGIAO);
                }

                //if (ID_OS != null)
                //    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int32, ID_OS);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public void Reordenar(AgendaEntity agendaEntity, string TP_ACAO)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAgendaReordenar");

                _db.AddInParameter(dbCommand, "@p_TP_ACAO", DbType.String, TP_ACAO);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, agendaEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_NR_ORDENACAO", DbType.Int32, agendaEntity.NR_ORDENACAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, agendaEntity.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Obtem lista de Agenda de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<AgendaSinc> ObterListaAgendaSinc(Int64 idUsuario)
        {
            try
            {
                IList<AgendaSinc> listaAgenda = new List<AgendaSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                          //" select a.* from tbagenda a " +
                                          //" inner join tb_tecnico t ON t.cd_tecnico = a.cd_tecnico " +
                                          //" WHERE(t.ID_USUARIO = @ID_USUARIO) ";
                                          " select distinct a.* " +
                                          " from tbagenda a " +
                                          " inner join tb_tecnico t ON t.cd_tecnico = a.cd_tecnico " +
                                          " inner join tb_ATIVO_CLIENTE ac ON ac.CD_CLIENTE = a.CD_CLIENTE " +
                                          " inner join tb_ATIVO_FIXO at ON at.CD_ATIVO_FIXO = ac.CD_ATIVO_FIXO "+
                                          " inner join tb_tecnico_cliente tc ON tc.cd_cliente = a.cd_cliente and tc.cd_tecnico = a.cd_tecnico  " +
                                          " WHERE(t.ID_USUARIO = @ID_USUARIO) " +
                                          " AND (DT_DEVOLUCAO is null OR DT_DEVOLUCAO > getdate() -30)  ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            AgendaSinc agenda = new AgendaSinc();
                            agenda.ID_AGENDA = Convert.ToInt64(SDR["ID_AGENDA"].ToString());
                            agenda.CD_CLIENTE = Convert.ToInt64(SDR["CD_CLIENTE"] is DBNull ? 0 : SDR["CD_CLIENTE"]);
                            agenda.CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"] is DBNull ? "" : SDR["CD_TECNICO"].ToString());
                            agenda.NR_ORDENACAO = Convert.ToInt32(SDR["NR_ORDENACAO"] is DBNull ? 0 : SDR["NR_ORDENACAO"]);
                            listaAgenda.Add(agenda);
                        }
                        cnx.Close();
                        return listaAgenda;
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
