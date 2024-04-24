using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class TpMotivoVisitaPadraoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TpMotivoVisitaPadraoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IList<TpMotivoVisitaPadraoEntity> ObterLista(TpMotivoVisitaPadraoEntity motivoVisitaEntity)
        {
            DbConnection connection = null;
            IList<TpMotivoVisitaPadraoEntity> listaMotivoVisita = new List<TpMotivoVisitaPadraoEntity>();
            IDataReader rdr = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcMotivoVisitaPadraoSelect");

                if (motivoVisitaEntity.ID_MOTIVO_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_MOTIVO_VISITA", DbType.String, motivoVisitaEntity.ID_MOTIVO_VISITA);

                if (motivoVisitaEntity.CD_MOTIVO_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_VISITA", DbType.Int32, motivoVisitaEntity.CD_MOTIVO_VISITA);

                if (!string.IsNullOrEmpty(motivoVisitaEntity.DS_MOTIVO_VISITA))
                    _db.AddInParameter(dbCommand, "@p_DS_MOTIVO_VISITA", DbType.String, motivoVisitaEntity.DS_MOTIVO_VISITA);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                rdr = _db.ExecuteReader(dbCommand);
                listaMotivoVisita = mapMotivoVisita(rdr);
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

            return listaMotivoVisita;
        }

        private IList<TpMotivoVisitaPadraoEntity> mapMotivoVisita(IDataReader rdr)
        {
            IList<TpMotivoVisitaPadraoEntity> listaMotivoVisita = new List<TpMotivoVisitaPadraoEntity>();

            while (rdr.Read())
            {
                TpMotivoVisitaPadraoEntity motivoVisita = new TpMotivoVisitaPadraoEntity();

                motivoVisita.ID_MOTIVO_VISITA = Convert.ToInt64(rdr["ID_MOTIVO_VISITA"].ToString());
                motivoVisita.CD_MOTIVO_VISITA = Convert.ToInt32(rdr["CD_MOTIVO_VISITA"] is DBNull ? 0 : rdr["CD_MOTIVO_VISITA"]);
                motivoVisita.DS_MOTIVO_VISITA = Convert.ToString(rdr["DS_MOTIVO_VISITA"] is DBNull ? "" : rdr["DS_MOTIVO_VISITA"]);

                listaMotivoVisita.Add(motivoVisita);
            }

            return listaMotivoVisita;
        }
    }
}
