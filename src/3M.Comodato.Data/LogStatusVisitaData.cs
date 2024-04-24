using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class LogStatusVisitaData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LogStatusVisitaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Alterar(LogStatusVisitaEntity logStatusVisita)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogStatusVisitaUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_LOG_STATUS_VISITA", DbType.Int64, logStatusVisita.ID_LOG_STATUS_VISITA);

                if (logStatusVisita.visita.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, logStatusVisita.visita.ID_VISITA);

                if (logStatusVisita.DT_DATA_LOG_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_LOG_VISITA", DbType.DateTime, logStatusVisita.DT_DATA_LOG_VISITA);

                if (logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (logStatusVisita.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, logStatusVisita.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(LogStatusVisitaEntity logStatusVisita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogStatusVisitaSelect");

                if (logStatusVisita.ID_LOG_STATUS_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_LOG_STATUS_VISITA", DbType.Int64, logStatusVisita.ID_LOG_STATUS_VISITA);

                if (logStatusVisita.visita.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, logStatusVisita.visita.ID_VISITA);

                if (logStatusVisita.DT_DATA_LOG_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_LOG_VISITA", DbType.DateTime, logStatusVisita.DT_DATA_LOG_VISITA);

                if (logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int64, logStatusVisita.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

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

        public DataTable ObterListaOsPadrao(LogStatusOSPadraoEntity logStatusVisita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogStatusOSPadraoSelect");

                if (logStatusVisita.ID_LOG_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_LOG_STATUS_OS", DbType.Int64, logStatusVisita.ID_LOG_STATUS_OS);

                if (logStatusVisita.OS.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, logStatusVisita.OS.ID_OS);

                if (logStatusVisita.DT_DATA_LOG_OS != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_LOG_OS", DbType.DateTime, logStatusVisita.DT_DATA_LOG_OS);

                if (logStatusVisita.tpStatusOSPadrao.ST_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_OS", DbType.Int64, logStatusVisita.tpStatusOSPadrao.ST_STATUS_OS);

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
        /// Obtem lista de Logs de Visita de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<LogStatusVisitaSinc> ObterListaLogStatusVisitaSinc(Int64 idUsuario)
        {
            try
            {
                IList<LogStatusVisitaSinc> listaLogStatusVisita = new List<LogStatusVisitaSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select * " +
                                         " from tblogstatusvisita " +
                                         " where id_visita in " +
                                         " (select v.id_visita from tbvisita v " +
                                         "  inner join tb_tecnico t ON t.cd_tecnico = v.cd_tecnico " +
                                         "  WHERE(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) and " +
                                         "        v.DT_DATA_VISITA >= getdate() - " +
                                         "        Convert(int, (select cvlParametro as QTD from tbParametro where ccdParametro = 'qtdDiasRetroativos'))) ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            LogStatusVisitaSinc log = new LogStatusVisitaSinc();
                            log.ID_LOG_STATUS_VISITA = Convert.ToInt64(SDR["ID_LOG_STATUS_VISITA"].ToString());
                            log.ID_VISITA = Convert.ToInt64(SDR["ID_VISITA"] is DBNull ? 0 : SDR["ID_VISITA"]);
                            log.DT_DATA_LOG_VISITA = Convert.ToDateTime(SDR["DT_DATA_LOG_VISITA"] is DBNull ? "01/01/2000" : SDR["DT_DATA_LOG_VISITA"]);
                            log.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(SDR["ST_TP_STATUS_VISITA_OS"] is DBNull ? 0 : SDR["ST_TP_STATUS_VISITA_OS"]);
                            log.nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);
                            log.dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]);

                            listaLogStatusVisita.Add(log);
                        }
                        cnx.Close();
                        return listaLogStatusVisita;
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
