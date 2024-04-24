using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class KatTecnicoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public KatTecnicoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }



        public DataSet ObterRelatorio(DateTime? dataInicial, DateTime? dataFinal, string codigoTecnicos, string codigoClientes)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptKatPorTecnico");

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

                if (!String.IsNullOrEmpty(codigoTecnicos))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICOS", DbType.String, codigoTecnicos);

                if (!String.IsNullOrEmpty(codigoClientes))
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.String, codigoClientes);




                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                dataSet = _db.ExecuteDataSet(dbCommand);
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
        public DataTable ObterRelatorioConsolidado(DateTime? dataInicial, DateTime? dataFinal, string codigoTecnicos, string codigoClientes, string linha)
        {
            DbConnection connection = null;
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptKatPorTecnicoConsolidado");

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

                if (!String.IsNullOrEmpty(codigoTecnicos))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICOS", DbType.String, codigoTecnicos);

                if (!String.IsNullOrEmpty(codigoClientes))
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.String, codigoClientes);

                if (!String.IsNullOrEmpty(linha))
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA", DbType.Int64, Convert.ToUInt64(linha));


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

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
        public DataSet ObterRelatorioVisita(DateTime? dataInicial, DateTime? dataFinal, string codigoTecnicos, string codigoClientes)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptKatPorTecnicoVisita");

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

                if (!String.IsNullOrEmpty(codigoTecnicos))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICOS", DbType.String, codigoTecnicos);

                if (!String.IsNullOrEmpty(codigoClientes))
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.String, codigoClientes);




                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                dataSet = _db.ExecuteDataSet(dbCommand);
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

        

        public DataSet ObterByVisitaCliTec(DateTime? dataInicial, DateTime? dataFinal, string codigoTecnico, int codigoCliente, long ID_VISITA)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcKatPorTecnicoSelect");

                if (dataInicial != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

                if (dataFinal != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

                if (!String.IsNullOrEmpty(codigoTecnico))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, codigoTecnico);

                if (codigoCliente > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, codigoCliente);

                if (ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, ID_VISITA);




                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                dataSet = _db.ExecuteDataSet(dbCommand);
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

    }
}



