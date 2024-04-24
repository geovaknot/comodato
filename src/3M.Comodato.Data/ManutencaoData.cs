using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class ManutencaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public ManutencaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterRelatorio(DateTime? dataInicial, DateTime? dataFinal, string codigoGrupo, decimal? codigoCliente, string codigoTecnico, long? codigoVisita, long? codigoOS)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptManutencao");

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@pDtInicial", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@pDtFinal", DbType.Date, dataFinal);

                if (!String.IsNullOrEmpty(codigoGrupo))
                    _db.AddInParameter(dbCommand, "@pCD_GRUPO", DbType.String, codigoGrupo);

                if (codigoCliente != null)
                    _db.AddInParameter(dbCommand, "@pCD_CLIENTE", DbType.Decimal, codigoCliente);

                if (!String.IsNullOrEmpty(codigoTecnico))
                    _db.AddInParameter(dbCommand, "@pCD_TECNICO", DbType.String, codigoTecnico);

                if (codigoVisita != null)
                    _db.AddInParameter(dbCommand, "@pID_VISITA", DbType.Int64, codigoVisita);

                if (codigoOS != null)
                    _db.AddInParameter(dbCommand, "@pID_OS", DbType.Int64, codigoOS);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                var tp1 = _db.ExecuteDataSet(dbCommand);

                dataSet = _db.ExecuteDataSet(dbCommand);
                //dataTable = dataSet.Tables[0];
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
            return dataSet;
        }
        public DataTable ObterRelatorio(DateTime? dataInicial, DateTime? dataFinal, string codigoGrupo, Int64 codigoCliente)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptGeralManutencao");

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

                if (!String.IsNullOrEmpty(codigoGrupo))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPOS", DbType.String, codigoGrupo);

                if (codigoCliente != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.Decimal, codigoCliente);                

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                var tp1 = _db.ExecuteDataSet(dbCommand);

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

        public DataTable ObterRelatorioConsolidado(DateTime? dataInicial, DateTime? dataFinal, string codigoGrupo, Int64 codigoCliente, string linha)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptGeralManutencaoConsolidado");

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

                if (!String.IsNullOrEmpty(codigoGrupo))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPOS", DbType.String, codigoGrupo);

                if (codigoCliente != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.Decimal, codigoCliente);

                if (linha != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA", DbType.Int64, Convert.ToInt64(linha));

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                var tp1 = _db.ExecuteDataSet(dbCommand);

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
