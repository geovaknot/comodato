using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class TpStatusVisitaPadraoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TpStatusVisitaPadraoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IList<TpStatusVisitaPadraoEntity> ObterLista(TpStatusVisitaPadraoEntity statusVisitaEntity)
        {
            DbConnection connection = null;
            IList<TpStatusVisitaPadraoEntity> listaStatusVisita = new List<TpStatusVisitaPadraoEntity>();
            IDataReader rdr = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusVisitaPadraoSelect");

                if (statusVisitaEntity.ID_STATUS_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_STATUS_VISITA", DbType.String, statusVisitaEntity.ID_STATUS_VISITA);

                if (statusVisitaEntity.ST_STATUS_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_VISITA", DbType.Int32, statusVisitaEntity.ST_STATUS_VISITA);

                if (!string.IsNullOrEmpty(statusVisitaEntity.DS_STATUS_VISITA))
                    _db.AddInParameter(dbCommand, "@p_DS_STATUS_VISITA", DbType.String, statusVisitaEntity.DS_STATUS_VISITA);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                rdr = _db.ExecuteReader(dbCommand);
                listaStatusVisita = mapStatusVisita(rdr);
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

            return listaStatusVisita;
        }

        private IList<TpStatusVisitaPadraoEntity> mapStatusVisita(IDataReader rdr)
        {
            IList<TpStatusVisitaPadraoEntity> listaStatusVisita = new List<TpStatusVisitaPadraoEntity>();

            while (rdr.Read())
            {
                TpStatusVisitaPadraoEntity statusVisita = new TpStatusVisitaPadraoEntity();

                statusVisita.ID_STATUS_VISITA = Convert.ToInt64(rdr["ID_STATUS_VISITA"].ToString());
                statusVisita.ST_STATUS_VISITA = Convert.ToInt32(rdr["ST_STATUS_VISITA"] is DBNull ? 0 : rdr["ST_STATUS_VISITA"]);
                statusVisita.DS_STATUS_VISITA = Convert.ToString(rdr["DS_STATUS_VISITA"] is DBNull ? "" : rdr["DS_STATUS_VISITA"]);

                listaStatusVisita.Add(statusVisita);
            }

            return listaStatusVisita;
        }
    }
}
