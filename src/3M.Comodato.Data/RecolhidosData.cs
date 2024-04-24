using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class RecolhidosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public RecolhidosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }
        public DataSet ObterLista(string arrayFiltros, DateTime DT_INICIAL, DateTime DT_FINAL, string tipoRelatorio)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptRecolhidosSelect");

                _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.DateTime, DT_INICIAL);
                _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.DateTime, DT_FINAL);

                if (!string.IsNullOrEmpty(arrayFiltros))
                {
                    switch (tipoRelatorio)
                    {
                        case "Cliente":
                            _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.String, arrayFiltros);
                            break;
                        case "Modelo":
                            _db.AddInParameter(dbCommand, "@p_CD_MODELOS", DbType.String, arrayFiltros);
                            break;
                    }
                }
                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 300;

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

        //[Obsolete()]
        public DataSet ObterLista(RecolhidosEntity recolhido)
        {
            if (recolhido.DT_DEV_INICIAL is null)
                //Depois limitar em 3 meses atrás
                recolhido.DT_DEV_INICIAL = DateTime.Now.AddYears(-5);
            if (recolhido.DT_DEV_FINAL is null)
                recolhido.DT_DEV_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRecolhidosSelect");

                if (recolhido.DT_DEV_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DEV_INICIAL", DbType.Date, recolhido.DT_DEV_INICIAL);

                if (recolhido.DT_DEV_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DEV_FINAL", DbType.Date, recolhido.DT_DEV_FINAL);

                //if (!string.IsNullOrEmpty(recolhido.DT_DEVOLUCAO))
                //    _db.AddInParameter(dbCommand, "@p_DT_DEVOLUCAO", DbType.String, recolhido.DT_DEVOLUCAO);

                if (!string.IsNullOrEmpty(recolhido.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, recolhido.CD_ATIVO_FIXO);

                //if (!string.IsNullOrEmpty(recolhido.DS_ATIVO))
                //    _db.AddInParameter(dbCommand, "@p_DS_ATIVO", DbType.String, recolhido.DS_ATIVO);

                if (recolhido.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, recolhido.CD_CLIENTE);

                //if (!string.IsNullOrEmpty(recolhido.NM_CLIENTE))
                //    _db.AddInParameter(dbCommand, "@p_NM_CLIENTE", DbType.String, recolhido.NM_CLIENTE);

                //if (!string.IsNullOrEmpty(recolhido.CD_MOTIVO_DEVOLUCAO))
                //    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.String, recolhido.CD_MOTIVO_DEVOLUCAO);

                //if (!string.IsNullOrEmpty(recolhido.DS_MOTIVO_DEVOLUCAO))
                //    _db.AddInParameter(dbCommand, "@p_DS_MOTIVO_DEVOLUCAO", DbType.String, recolhido.DS_MOTIVO_DEVOLUCAO);

                //if (!string.IsNullOrEmpty(recolhido.ID_ATIVO_CLIENTE))
                //    _db.AddInParameter(dbCommand, "@p_ID_ATIVO_CLIENTE", DbType.String, recolhido.ID_ATIVO_CLIENTE);

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