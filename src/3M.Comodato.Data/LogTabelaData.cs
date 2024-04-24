using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;    

namespace _3M.Comodato.Data
{
    public class LogTabelaData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LogTabelaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");            
        }

        public bool Inserir(ref LogTabelaEntity logTabela)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcLogTabelaInsert");

                _db.AddInParameter(dbCommand, "@p_cnmTabela", DbType.String, logTabela.cnmTabela);
                _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, logTabela.bidAtivo);
                _db.AddOutParameter(dbCommand, "@p_nidLogTabela", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                logTabela.nidLogTabela = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidLogTabela"));

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

        public bool InserirAllSystemTables()
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcLogTabelaInsertAllSystemTables");

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

        public void Excluir(LogTabelaEntity logTabela)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcLogTabelaDelete");

                _db.AddInParameter(dbCommand, "@p_nidLogTabela", DbType.Int64, logTabela.nidLogTabela);

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

        public bool Alterar(LogTabelaEntity logTabela)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogTabelaUpdate");

                _db.AddInParameter(dbCommand, "@p_nidLogTabela", DbType.Int64, logTabela.nidLogTabela);

                if (!string.IsNullOrEmpty(logTabela.cnmTabela))
                    _db.AddInParameter(dbCommand, "@p_cnmTabela", DbType.String, logTabela.cnmTabela);

                if (logTabela.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, logTabela.bidAtivo);

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

        public DataTable ObterLista(LogTabelaEntity logTabela)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogTabelaSelect");

                if (logTabela.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, logTabela.bidAtivo);

                if (logTabela.nidLogTabela != 0)
                    _db.AddInParameter(dbCommand, "@p_nidLogTabela", DbType.Int64, logTabela.nidLogTabela);

                if (!string.IsNullOrEmpty(logTabela.cnmTabela))
                    _db.AddInParameter(dbCommand, "@p_cnmTabela", DbType.String, logTabela.cnmTabela);

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
