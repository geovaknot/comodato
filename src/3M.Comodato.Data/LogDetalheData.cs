using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using _3M.Comodato.Entity;    

namespace _3M.Comodato.Data
{
    public class LogDetalheData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LogDetalheData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");            
        }

        public DataTable ObterLista(LogDetalheEntity logDetalhe, DateTime pdtmInicio, DateTime pdtmFim)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogDetalheSelect");

                if (logDetalhe.log.nidLog != 0)
                    _db.AddInParameter(dbCommand, "@p_nidLog", DbType.Int64, logDetalhe.log.nidLog);

                if (logDetalhe.nidLogDetalhe != 0)
                    _db.AddInParameter(dbCommand, "@p_nidLogDetalhe", DbType.Int64, logDetalhe.nidLogDetalhe);

                if (logDetalhe.log.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, logDetalhe.log.nidUsuarioAtualizacao);

                if (pdtmInicio != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_dtmInicio", DbType.DateTime, pdtmInicio);

                if (pdtmFim != DateTime.MinValue)
                    _db.AddInParameter(dbCommand, "@p_dtmFim", DbType.DateTime, pdtmFim);

                if (!string.IsNullOrEmpty(logDetalhe.log.ccdAcao))
                    _db.AddInParameter(dbCommand, "@p_ccdAcao", DbType.String, logDetalhe.log.ccdAcao);

                if (!string.IsNullOrEmpty(logDetalhe.log.cnmTabela))
                    _db.AddInParameter(dbCommand, "@p_cnmTabela", DbType.String, logDetalhe.log.cnmTabela);

                if (logDetalhe.log.nidPK != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPK", DbType.Int64, logDetalhe.log.nidPK);

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
