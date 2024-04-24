using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;    

namespace _3M.Comodato.Data
{
    public class LogTabelaCampoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LogTabelaCampoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");            
        }

        public bool Inserir(ref LogTabelaCampoEntity logTabelaCampo)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcLogTabelaCampoInsert");

                _db.AddInParameter(dbCommand, "@p_nidLogTabela", DbType.Int64, logTabelaCampo.logTabela.nidLogTabela);
                _db.AddInParameter(dbCommand, "@p_cnmCampo", DbType.String, logTabelaCampo.cnmCampo);
                _db.AddInParameter(dbCommand, "@p_cdsAlias", DbType.String, logTabelaCampo.cdsAlias);
                _db.AddOutParameter(dbCommand, "@p_nidLogTabelaCampo", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                logTabelaCampo.nidLogTabelaCampo = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidLogTabelaCampo"));

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

        public void Excluir(LogTabelaCampoEntity logTabelaCampo)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcLogTabelaCampoDelete");

                _db.AddInParameter(dbCommand, "@p_nidLogTabelaCampo", DbType.Int64, logTabelaCampo.nidLogTabelaCampo);

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

        public bool Alterar(LogTabelaCampoEntity logTabelaCampo)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogTabelaCampoUpdate");

                _db.AddInParameter(dbCommand, "@p_nidLogTabelaCampo", DbType.Int64, logTabelaCampo.nidLogTabelaCampo);

                if (logTabelaCampo.logTabela.nidLogTabela != 0)
                    _db.AddInParameter(dbCommand, "@p_nidLogTabela", DbType.Int64, logTabelaCampo.logTabela.nidLogTabela);

                if (!string.IsNullOrEmpty(logTabelaCampo.cnmCampo))
                    _db.AddInParameter(dbCommand, "@p_cnmCampo", DbType.String, logTabelaCampo.cnmCampo);

                if (!string.IsNullOrEmpty(logTabelaCampo.cdsAlias))
                    _db.AddInParameter(dbCommand, "@p_cdsAlias", DbType.String, logTabelaCampo.cdsAlias);

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

        public DataTable ObterLista(LogTabelaCampoEntity logTabelaCampo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogTabelaCampoSelect");

                _db.AddInParameter(dbCommand, "@p_nidLogTabela", DbType.Int64, logTabelaCampo.logTabela.nidLogTabela);

                if(!string.IsNullOrEmpty(logTabelaCampo.cnmCampo))
                    _db.AddInParameter(dbCommand, "@p_cnmCampo", DbType.String, logTabelaCampo.cnmCampo);

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

        public DataTable ObterListaALLCampo(LogTabelaCampoEntity logTabelaCampo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogTabelaCampoSelectALLCampo");

                _db.AddInParameter(dbCommand, "@p_TABLE_NAME", DbType.String, logTabelaCampo.logTabela.cnmTabela);

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
