using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class TpStatusOSPadraoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TpStatusOSPadraoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IList<TpStatusOSPadraoEntity> ObterLista(TpStatusOSPadraoEntity statusOSEntity)
        {
            DbConnection connection = null;
            IList<TpStatusOSPadraoEntity> listaStatusOS = new List<TpStatusOSPadraoEntity>();
            IDataReader rdr = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusOSPadraoSelect");

                if (statusOSEntity.ID_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_STATUS_OS", DbType.String, statusOSEntity.ID_STATUS_OS);

                if (statusOSEntity.ST_STATUS_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_OS", DbType.Int32, statusOSEntity.ST_STATUS_OS);

                if (!string.IsNullOrEmpty(statusOSEntity.DS_STATUS_OS))
                    _db.AddInParameter(dbCommand, "@p_DS_STATUS_OS", DbType.String, statusOSEntity.DS_STATUS_OS);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                rdr = _db.ExecuteReader(dbCommand);
                listaStatusOS = mapStatusOS(rdr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rdr != null && !rdr.IsClosed)
                    rdr.Close();

                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return listaStatusOS;
        }

        private IList<TpStatusOSPadraoEntity> mapStatusOS(IDataReader rdr)
        {
            IList<TpStatusOSPadraoEntity> listaStatusOS = new List<TpStatusOSPadraoEntity>();

            while (rdr.Read())
            {
                TpStatusOSPadraoEntity statusOS = new TpStatusOSPadraoEntity();

                statusOS.ID_STATUS_OS = Convert.ToInt64(rdr["ID_STATUS_OS"].ToString());
                statusOS.ST_STATUS_OS = Convert.ToInt32(rdr["ST_STATUS_OS"] is DBNull ? 0 : rdr["ST_STATUS_OS"]);
                statusOS.DS_STATUS_OS = Convert.ToString(rdr["DS_STATUS_OS"] is DBNull ? "" : rdr["DS_STATUS_OS"]);

                listaStatusOS.Add(statusOS);
            }

            return listaStatusOS;
        }

        public DataTable ObterListaStatusOsPadrao(TpStatusOSPadraoEntity tpStatusVisitaOSEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTpStatusOSPadraoSelectCombo");

                //if (tpStatusVisitaOSEntity.ID_TP_STATUS_VISITA_OS != 0)
                //    _db.AddInParameter(dbCommand, "@p_ID_TP_STATUS_VISITA_OS", DbType.Int64, tpStatusVisitaOSEntity.ID_TP_STATUS_VISITA_OS);

                //if (tpStatusVisitaOSEntity.ST_TP_STATUS_VISITA_OS != 0)
                //    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int64, tpStatusVisitaOSEntity.ST_TP_STATUS_VISITA_OS);

                //if (!string.IsNullOrEmpty(tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS))
                //    _db.AddInParameter(dbCommand, "@p_DS_TP_STATUS_VISITA_OS", DbType.String, tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS);

                //if (!string.IsNullOrEmpty(tpStatusVisitaOSEntity.FL_STATUS_OS))
                //    _db.AddInParameter(dbCommand, "@p_FL_STATUS_OS", DbType.String, tpStatusVisitaOSEntity.FL_STATUS_OS);

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
