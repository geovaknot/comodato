using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class CompletosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public CompletosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterRelatorioVendasCompleto(CompletosEntity completo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioVendasCompletosSelect");

                if (!String.IsNullOrEmpty(completo.CD_LINHA_PRODUTO))
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.String, completo.CD_LINHA_PRODUTO);

                if (completo.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, completo.CD_CLIENTE);

                if (completo.CD_VENDEDOR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, completo.CD_VENDEDOR);

                if (completo.CD_EXECUTIVO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Decimal, completo.CD_EXECUTIVO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 300;

                dataSet = _db.ExecuteDataSet(dbCommand);
            }
            catch (SqlException ex)
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

        public DataTable ObterLista(CompletosEntity completo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioCompletosSelect");

                if (!String.IsNullOrEmpty(completo.CD_LINHA_PRODUTO))
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.String, completo.CD_LINHA_PRODUTO);

                if (completo.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, completo.CD_CLIENTE);

                if (completo.CD_VENDEDOR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, completo.CD_VENDEDOR);

                if (completo.CD_EXECUTIVO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Decimal, completo.CD_EXECUTIVO);
                                
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

        public DataTable ObterListaSubReport(CompletosEntity completosEntity)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioCompletosSub1Select");

                if (completosEntity.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, completosEntity.CD_CLIENTE);

                if (!string.IsNullOrEmpty(completosEntity.CD_LINHA_PRODUTO))
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.String, completosEntity.CD_LINHA_PRODUTO);

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