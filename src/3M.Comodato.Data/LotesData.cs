using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class LotesData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LotesData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterLista(Int64 ID_PEDIDO, Int64 ID_LOTE_PEDIDO)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptLotesSelect");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);

                if(ID_LOTE_PEDIDO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_LOTE_PEDIDO", DbType.Int64, ID_LOTE_PEDIDO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

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

        public DataTable obterLotes(long ID_PEDIDO)
        {
            DbConnection connection = null;
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            //List<Int64> lista;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLotesSelect");
                dbCommand.CommandTimeout = 500000;
                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);

                //lista = dataSet.Tables[0].Rows.OfType<DataRow>()
                //    .Select(dr => dr.Field<Int64>("LOTE")).ToList();

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

        public void AtualizaRefNF(Int64 ID_LOTE, string GUID)
        {

            DbConnection connection = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLoteUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_LOTE_APROVACAO", DbType.Int64, ID_LOTE);
                _db.AddInParameter(dbCommand, "@p_DS_ANEXO_NF_GUID", DbType.String, GUID);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                _db.ExecuteNonQuery(dbCommand);
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
        }
    }
}