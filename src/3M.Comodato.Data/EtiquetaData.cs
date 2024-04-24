using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class EtiquetaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public EtiquetaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<EtiquetaEntity> ObterListaEntity(EtiquetaEntity etiqueta)
        {
            Func<DataRow, EtiquetaEntity> Converter = new Func<DataRow, EtiquetaEntity>((r) =>
            {
                EtiquetaEntity entity = new EtiquetaEntity();
                entity.ID_ETIQUETA = Convert.ToInt64(r["ID_ETIQUETA"]);
                entity.CD_ETIQUETA = r["CD_ETIQUETA"].ToString();
                entity.DS_ETIQUETA = r["DS_ETIQUETA"].ToString();

                if (r["VL_ALTURA"] != DBNull.Value)
                {
                    entity.VL_ALTURA = Convert.ToDecimal(r["VL_ALTURA"]);
                }

                if (r["VL_LARGURA"] != DBNull.Value)
                {
                    entity.VL_LARGURA = Convert.ToDecimal(r["VL_LARGURA"]);
                }

                return entity;
            });

            var dtEtiqueta = ObterLista(etiqueta);
            return (from r in dtEtiqueta.Rows.Cast<DataRow>()
                    select Converter(r));
        }

        public DataTable ObterLista(EtiquetaEntity etiqueta)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEtiquetaSelect");

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
