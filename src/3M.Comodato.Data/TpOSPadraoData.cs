using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class TpOSPadraoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TpOSPadraoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IList<TpOSPadraoEntity> ObterLista(TpOSPadraoEntity osPadraoEntity)
        {
            DbConnection connection = null;
            IList<TpOSPadraoEntity> listaTipoOS = new List<TpOSPadraoEntity>();
            IDataReader rdr = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoOSPadraoSelect");

                if (osPadraoEntity.ID_TIPO_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_TIPO_OS", DbType.String, osPadraoEntity.ID_TIPO_OS);

                if (osPadraoEntity.CD_TIPO_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_OS", DbType.Int32, osPadraoEntity.CD_TIPO_OS);

                if (!string.IsNullOrEmpty(osPadraoEntity.DS_TIPO_OS))
                    _db.AddInParameter(dbCommand, "@p_DS_TIPO_OS", DbType.String, osPadraoEntity.DS_TIPO_OS);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                rdr = _db.ExecuteReader(dbCommand);
                listaTipoOS = mapTipoOS(rdr);
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

            return listaTipoOS;
        }

        private IList<TpOSPadraoEntity> mapTipoOS(IDataReader rdr)
        {
            IList<TpOSPadraoEntity> listaTipoOS = new List<TpOSPadraoEntity>();

            while (rdr.Read())
            {
                TpOSPadraoEntity tipoOS = new TpOSPadraoEntity();

                tipoOS.ID_TIPO_OS = Convert.ToInt64(rdr["ID_TIPO_OS"].ToString());
                tipoOS.CD_TIPO_OS = Convert.ToInt32(rdr["CD_TIPO_OS"] is DBNull ? 0 : rdr["CD_TIPO_OS"]);
                tipoOS.DS_TIPO_OS = Convert.ToString(rdr["DS_TIPO_OS"] is DBNull ? "" : rdr["DS_TIPO_OS"]);

                listaTipoOS.Add(tipoOS);
            }

            return listaTipoOS;
        }
    }
}
