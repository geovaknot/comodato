using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using _3M.Comodato.Entity;    

namespace _3M.Comodato.Data
{
    public class LogData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LogData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");            
        }

        public DataTable ObterLista(LogEntity log,DateTime pdtmInicio, DateTime pdtmFim)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogSelect");

                if (log.nidLog != 0)
                    _db.AddInParameter(dbCommand, "@p_nidLog", DbType.Int64, log.nidLog);

                if (log.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, log.nidUsuarioAtualizacao);

                if (pdtmInicio!= DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_dtmInicio", DbType.DateTime, pdtmInicio);

                if (pdtmFim != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_dtmFim", DbType.DateTime, pdtmFim);

                if (!string.IsNullOrEmpty(log.ccdAcao))
                    _db.AddInParameter(dbCommand, "@p_ccdAcao", DbType.String, log.ccdAcao);

                if (!string.IsNullOrEmpty(log.cnmTabela))
                    _db.AddInParameter(dbCommand, "@p_cnmTabela", DbType.String, log.cnmTabela);

                if (log.nidPK != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPK", DbType.Int64, log.nidPK);

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
        /// Consulta log para BlackList
        /// </summary>
        /// <param name="logDetalhe">Entidade logDetalheEntity</param>
        /// <returns>DataTable</returns>
        public DataTable ObterLista(LogDetalheEntity logDetalhe)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogDetalheSelectBlackList");

                if (!string.IsNullOrEmpty(logDetalhe.log.cnmTabela))
                    _db.AddInParameter(dbCommand, "@p_cnmTabela", DbType.String, logDetalhe.log.cnmTabela);

                if (logDetalhe.log.nidPK != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPK", DbType.Int64, logDetalhe.log.nidPK);

                if (!string.IsNullOrEmpty(logDetalhe.cnmCampo))
                    _db.AddInParameter(dbCommand, "@p_cnmCampo", DbType.String, logDetalhe.cnmCampo);

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
