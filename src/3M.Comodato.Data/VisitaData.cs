using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class VisitaData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public VisitaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref VisitaEntity visitaEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcVisitaInsert");

                _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);
                _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (visitaEntity.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.cliente.CD_CLIENTE);

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);
                _db.AddInParameter(dbCommand, "@p_DS_NOME_RESPONSAVEL", DbType.String, visitaEntity.DS_NOME_RESPONSAVEL);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                visitaEntity.ID_VISITA = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_VISITA"));
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

        public void Excluir(VisitaEntity visitaEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcVisitaDelete");

                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, visitaEntity.ID_VISITA);

                if (visitaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(VisitaEntity visitaEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.String, visitaEntity.ID_VISITA);

                if (visitaEntity.DT_DATA_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);

                if (visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (visitaEntity.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.cliente.CD_CLIENTE);

                if (!string.IsNullOrEmpty(visitaEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.tecnico.CD_TECNICO);

                if (!string.IsNullOrEmpty(visitaEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);

                if (!string.IsNullOrEmpty(visitaEntity.DS_NOME_RESPONSAVEL))
                    _db.AddInParameter(dbCommand, "@p_DS_NOME_RESPONSAVEL", DbType.String, visitaEntity.DS_NOME_RESPONSAVEL);

                if (!string.IsNullOrEmpty(visitaEntity.FL_ENVIO_EMAIL_PESQ))
                    _db.AddInParameter(dbCommand, "@p_FL_ENVIO_EMAIL_PESQ", DbType.String, visitaEntity.FL_ENVIO_EMAIL_PESQ);

                if (visitaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);

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
            return blnOK;
        }

        public DataTable ObterLista(VisitaEntity visitaEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaSelect");

                if (visitaEntity.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.String, visitaEntity.ID_VISITA);

                if (visitaEntity.DT_DATA_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);

                if (visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (visitaEntity.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.cliente.CD_CLIENTE);

                if (!String.IsNullOrEmpty(visitaEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.tecnico.CD_TECNICO);

                if (!String.IsNullOrEmpty(visitaEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);

                if (!String.IsNullOrEmpty(visitaEntity.DS_NOME_RESPONSAVEL))
                    _db.AddInParameter(dbCommand, "@p_DS_NOME_RESPONSAVEL", DbType.String, visitaEntity.DS_NOME_RESPONSAVEL);

                if (visitaEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, visitaEntity.nidUsuarioAtualizacao);

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

        public DataTable ObterListaVisitaOS(VisitaEntity visitaEntity, string CD_ATIVO_FIXO, Int64? ID_OS, DateTime? DT_DATA_ABERTURA_INICIO, DateTime? DT_DATA_ABERTURA_FIM)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVisitaSelectOS");

                if (visitaEntity.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.String, visitaEntity.ID_VISITA);

                if (visitaEntity.DT_DATA_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, visitaEntity.DT_DATA_VISITA);

                if (visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (visitaEntity.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, visitaEntity.cliente.CD_CLIENTE);

                if (!String.IsNullOrEmpty(visitaEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, visitaEntity.tecnico.CD_TECNICO);

                if (!String.IsNullOrEmpty(visitaEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, visitaEntity.DS_OBSERVACAO);

                if (!String.IsNullOrEmpty(visitaEntity.DS_NOME_RESPONSAVEL))
                    _db.AddInParameter(dbCommand, "@p_DS_NOME_RESPONSAVEL", DbType.String, visitaEntity.DS_NOME_RESPONSAVEL);

                if (!String.IsNullOrEmpty(CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, CD_ATIVO_FIXO);

                if (ID_OS != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, ID_OS);

                if (DT_DATA_ABERTURA_INICIO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_ABERTURA_INICIO", DbType.DateTime, DT_DATA_ABERTURA_INICIO);

                if (DT_DATA_ABERTURA_FIM != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_ABERTURA_FIM", DbType.DateTime, DT_DATA_ABERTURA_FIM);

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

        /// <summary>
        /// Obtem lista de movimentações de um estoque de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<VisitaSinc> ObterListaVisitaSinc(Int64 idUsuario)
        {
            try
            {
                IList<VisitaSinc> listaVisita = new List<VisitaSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         "select * from tbvisita v " +
                                         "  inner join tb_tecnico t ON t.cd_tecnico = v.cd_tecnico " +
                                         "  WHERE(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) and " +
                                         "        v.DT_DATA_VISITA >= getdate() - "+
                                         "        Convert(int, (select cvlParametro as QTD from tbParametro where ccdParametro = 'qtdDiasRetroativos')) ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            VisitaSinc visita = new VisitaSinc();
                            visita.ID_VISITA = Convert.ToInt64(SDR["ID_VISITA"].ToString());
                            visita.DT_DATA_VISITA = Convert.ToDateTime(SDR["DT_DATA_VISITA"] is DBNull ? "01/01/2000" : SDR["DT_DATA_VISITA"]);
                            visita.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(SDR["ST_TP_STATUS_VISITA_OS"] is DBNull ? 0 : SDR["ST_TP_STATUS_VISITA_OS"]);
                            visita.CD_CLIENTE= Convert.ToInt64(SDR["CD_CLIENTE"] is DBNull ? 0 : SDR["CD_CLIENTE"]);
                            visita.CD_TECNICO= Convert.ToString(SDR["CD_TECNICO"] is DBNull ? "" : SDR["CD_TECNICO"].ToString());
                            visita.DS_OBSERVACAO= Convert.ToString(SDR["DS_OBSERVACAO"] is DBNull ? "" : SDR["DS_OBSERVACAO"].ToString());
                            visita.DS_NOME_RESPONSAVEL= Convert.ToString(SDR["DS_NOME_RESPONSAVEL"] is DBNull ? "" : SDR["DS_NOME_RESPONSAVEL"].ToString());
                            visita.nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);
                            visita.dtmDataHoraAtualizacao= Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]);
                            listaVisita.Add(visita);
                        }
                        cnx.Close();
                        return listaVisita;
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

        public DataTable ObterListaHistoricoVisita(int CD_CLIENTE, string CD_TECNICO, string DT_INICIO, string DT_FIM)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcHistoricoVisitaSelect");

                if (CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);

                if (!string.IsNullOrEmpty(CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                if (DT_INICIO != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIO", DbType.DateTime, DT_INICIO);

                if (DT_FIM != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FIM", DbType.DateTime, DT_FIM);

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
    }
}
