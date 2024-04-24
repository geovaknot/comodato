using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class ConsolidadoVendasData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public ConsolidadoVendasData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }



        public DataSet ObterRelatorio(string codigoGrupos, string codigoClientes)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptConsolidadoVendas");                

                if (!String.IsNullOrEmpty(codigoGrupos))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPOS", DbType.String, codigoGrupos);

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

        public DataTable ObterRelatorioConsolidado(int tipoVisao = 1, string cd_grupo = null, Int64 cd_cliente = 0, string linha = null)
        {
            DbConnection connection = null;
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptConsumoCliente");

                if (tipoVisao != 0)
                    _db.AddInParameter(dbCommand, "@pTipoVisao", DbType.Int64, tipoVisao);

                if (cd_cliente != 0)
                    _db.AddInParameter(dbCommand, "@pCD_CLIENTE", DbType.Int64, cd_cliente);

                if (!String.IsNullOrEmpty(cd_grupo))
                    _db.AddInParameter(dbCommand, "@pCD_GRUPO", DbType.String, cd_grupo);


                if (!String.IsNullOrEmpty(linha))
                    _db.AddInParameter(dbCommand, "@pCD_LINHA_PRODUTO", DbType.Int64, Convert.ToInt64(linha));

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

        //public DataSet ObterRelatorioVisita(DateTime? dataInicial, DateTime? dataFinal, string codigoTecnicos, string codigoClientes)
        //{
        //    DbConnection connection = null;
        //    DataSet dataSet = new DataSet();

        //    try
        //    {
        //        dbCommand = _db.GetStoredProcCommand("prcRptKatPorTecnicoVisita");

        //        if (dataInicial != null)
        //            _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

        //        if (dataFinal != null)
        //            _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

        //        if (!String.IsNullOrEmpty(codigoTecnicos))
        //            _db.AddInParameter(dbCommand, "@p_CD_TECNICOS", DbType.String, codigoTecnicos);

        //        if (!String.IsNullOrEmpty(codigoClientes))
        //            _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.String, codigoClientes);




        //        connection = _db.CreateConnection();
        //        dbCommand.Connection = connection;
        //        connection.Open();

        //        dbCommand.CommandTimeout = 1800;

        //        dataSet = _db.ExecuteDataSet(dbCommand);
        //    }
        //    catch (System.Data.SqlClient.SqlException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            connection.Close();
        //        }
        //    }
        //    return dataSet;
        //}



        //public DataSet ObterByVisitaCliTec(DateTime? dataInicial, DateTime? dataFinal, string codigoTecnico, int codigoCliente, long ID_VISITA)
        //{
        //    DbConnection connection = null;
        //    DataSet dataSet = new DataSet();

        //    try
        //    {
        //        dbCommand = _db.GetStoredProcCommand("prcKatPorTecnicoSelect");

        //        if (dataInicial != null)
        //            _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, dataInicial);

        //        if (dataFinal != null)
        //            _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, dataFinal);

        //        if (!String.IsNullOrEmpty(codigoTecnico))
        //            _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, codigoTecnico);

        //        if (codigoCliente > 0)
        //            _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, codigoCliente);

        //        if (ID_VISITA > 0)
        //            _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, ID_VISITA);




        //        connection = _db.CreateConnection();
        //        dbCommand.Connection = connection;
        //        connection.Open();

        //        dbCommand.CommandTimeout = 1800;

        //        dataSet = _db.ExecuteDataSet(dbCommand);
        //    }
        //    catch (System.Data.SqlClient.SqlException ex)
        //    {
        //        throw ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            connection.Close();
        //        }
        //    }
        //    return dataSet;
        //}

    }
}



