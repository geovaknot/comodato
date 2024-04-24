using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class ResumosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public ResumosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterLista(ResumosEntity resumo)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioResumosSelect");

                if (resumo.CD_LINHA_PRODUTO > 0)

                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int32, resumo.CD_LINHA_PRODUTO);

                if (resumo.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, resumo.CD_CLIENTE);

                if (resumo.CD_VENDEDOR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, resumo.CD_VENDEDOR);

                if (!String.IsNullOrEmpty(resumo.CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, resumo.CD_GRUPO);                

                if (!String.IsNullOrEmpty(resumo.CD_REGIAO))
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, resumo.CD_REGIAO);

                if (resumo.CD_EXECUTIVO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_EXECUTIVO", DbType.Decimal, resumo.CD_EXECUTIVO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 600;

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
        public DataTable ObterRelatorio(string codigoGrupo, Int64 codigoCliente, string linha = null)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioResumosSelect");

                if (!String.IsNullOrEmpty(codigoGrupo))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, codigoGrupo);

                if (codigoCliente != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, codigoCliente);

                if (linha != null)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int64, Convert.ToInt64(linha));

                

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