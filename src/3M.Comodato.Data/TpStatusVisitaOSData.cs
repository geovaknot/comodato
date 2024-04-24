using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class TpStatusVisitaOSData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TpStatusVisitaOSData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(TpStatusVisitaOSEntity tpStatusVisitaOSEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTpStatusVisitaOSSelect");

                if (tpStatusVisitaOSEntity.ID_TP_STATUS_VISITA_OS != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_TP_STATUS_VISITA_OS", DbType.Int64, tpStatusVisitaOSEntity.ID_TP_STATUS_VISITA_OS);

                if (tpStatusVisitaOSEntity.ST_TP_STATUS_VISITA_OS != 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int64, tpStatusVisitaOSEntity.ST_TP_STATUS_VISITA_OS);

                if (!string.IsNullOrEmpty(tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS))
                    _db.AddInParameter(dbCommand, "@p_DS_TP_STATUS_VISITA_OS", DbType.String, tpStatusVisitaOSEntity.DS_TP_STATUS_VISITA_OS);

                if (!string.IsNullOrEmpty(tpStatusVisitaOSEntity.FL_STATUS_OS))
                    _db.AddInParameter(dbCommand, "@p_FL_STATUS_OS", DbType.String, tpStatusVisitaOSEntity.FL_STATUS_OS);

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

        public DataTable ObterListaStatus(string statusCarregar, string FL_STATUS_OS)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTpStatusVisitaOSSelectStatus");

                _db.AddInParameter(dbCommand, "@p_StatusCarregar", DbType.String, statusCarregar);

                if (!string.IsNullOrEmpty(FL_STATUS_OS))
                    _db.AddInParameter(dbCommand, "@p_FL_STATUS_OS", DbType.String, FL_STATUS_OS);

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
