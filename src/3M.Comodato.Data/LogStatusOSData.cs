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
    public class LogStatusOSData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LogStatusOSData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Alterar(LogStatusOSEntity logStatusOS)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogStatusOSUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_LOG_STATUS_OS", DbType.Int64, logStatusOS.ID_LOG_STATUS_OS);

                if (logStatusOS.OS.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, logStatusOS.OS.ID_OS);

                if (logStatusOS.DT_DATA_LOG_OS != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_LOG_OS", DbType.DateTime, logStatusOS.DT_DATA_LOG_OS);

                if (logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (logStatusOS.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, logStatusOS.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(LogStatusOSEntity logStatusOS)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogStatusOSSelect");

                if (logStatusOS.ID_LOG_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_LOG_STATUS_OS", DbType.Int64, logStatusOS.ID_LOG_STATUS_OS);

                if (logStatusOS.OS.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, logStatusOS.OS.ID_OS);

                if (logStatusOS.DT_DATA_LOG_OS != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_LOG_OS", DbType.DateTime, logStatusOS.DT_DATA_LOG_OS);

                if (logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int64, logStatusOS.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

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

        public DataTable ObterListaAgendaVisita(Int64? ID_VISITA = null, Int64? CD_CLIENTE = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogStatusOSSelectVisita");

                if (ID_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, ID_VISITA);

                if (CD_CLIENTE != null)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);

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
        /// Obtem lista do Log de mudanças de Status em Ordem de serviço de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<LogStatusOSSinc> ObterListaLogStatusOsSinc(Int64 idUsuario)
        {
            try
            {
                IList<LogStatusOSSinc> listaLogStatusOs = new List<LogStatusOSSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select lo.* from tbLogStatusOS lo Where lo.ID_OS in ( " +
                                         " select o.ID_OS from tbOS o Where o.ID_VISITA in ( " +
                                         "  select v.id_visita from tbvisita v " +
                                         "   inner join tb_tecnico t ON t.cd_tecnico = v.cd_tecnico " +
                                         "   WHERE(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) AND " +
                                         "        o.id_visita = v.ID_VISITA AND " +
                                         "         v.DT_DATA_VISITA >= getdate() - " +
                                         "         Convert(int, (select cvlParametro as QTD from tbParametro where ccdParametro = 'qtdDiasRetroativos')) " +
                                         " )) ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            LogStatusOSSinc logStatus = new LogStatusOSSinc();
                            logStatus.ID_LOG_STATUS_OS= Convert.ToInt64(SDR["ID_LOG_STATUS_OS"].ToString());
                            logStatus.ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString());
                            logStatus.DT_DATA_LOG_OS = Convert.ToDateTime(SDR["DT_DATA_LOG_OS"] is DBNull ? "01/01/2000" : SDR["DT_DATA_LOG_OS"]);
                            logStatus.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(SDR["ST_TP_STATUS_VISITA_OS"] is DBNull ? 0 : SDR["ST_TP_STATUS_VISITA_OS"]);
                            logStatus.dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]);
                            logStatus.nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);

                            listaLogStatusOs.Add(logStatus);
                        }
                        cnx.Close();
                        return listaLogStatusOs;
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
