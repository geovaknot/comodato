using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class InventariosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public InventariosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterLista(InventariosEntity inventario)
        {
            //if (inventario.DT_INICIAL is null)
            //    inventario.DT_INICIAL = DateTime.Now.AddMonths(-1);
            //if (inventario.DT_FINAL is null)
            //    inventario.DT_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcInventarioSelect");

                if (inventario.DT_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, inventario.DT_INICIAL);

                if (inventario.DT_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, inventario.DT_FINAL);

                if (inventario.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, inventario.CD_CLIENTE);

                if (inventario.CD_VENDEDOR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, inventario.CD_VENDEDOR);

                if (!String.IsNullOrEmpty(inventario.CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, inventario.CD_GRUPO);
                                
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

        //SL00033191
        public DataSet ObterRptLista(DateTime? DT_INICIAL, DateTime? DT_FINAL, List<String> listacdClientes, Int64 CD_VENDEDOR, string CD_GRUPO, string CD_TECNICO,int CD_LINHA_PRODUTO, int sitUltManutencao)
        {
            //if (inventario.DT_INICIAL is null)
            //    inventario.DT_INICIAL = DateTime.Now.AddMonths(-1);
            //if (inventario.DT_FINAL is null)
            //    inventario.DT_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioInventario");

                if (DT_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, DT_INICIAL);

                if (DT_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, DT_FINAL);

                if (listacdClientes.Count > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, string.Join(",", listacdClientes));

                if (CD_VENDEDOR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, CD_VENDEDOR);

                if (!String.IsNullOrEmpty(CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, CD_GRUPO);

                if (!String.IsNullOrEmpty(CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                if (CD_LINHA_PRODUTO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int32, CD_LINHA_PRODUTO);

                if (sitUltManutencao > 0)
                    _db.AddInParameter(dbCommand, "@p_SITUACAO_MANUT", DbType.Int32, sitUltManutencao);

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



    }
}