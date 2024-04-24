using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class TipoFitaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public TipoFitaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<TipoFitaEntity> ObterListaEntity(TipoFitaEntity tipoFita)
        {
            Func<DataRow, TipoFitaEntity> Converter = new Func<DataRow, TipoFitaEntity>((r) =>
            {
                TipoFitaEntity entity = new TipoFitaEntity();
                entity.ID_TIPO_FITA = Convert.ToInt64(r["ID_TIPO_FITA"]);
                entity.CD_CODIGO_TIPO_FITA = r["CD_CODIGO_TIPO_FITA"].ToString();
                entity.DS_CODIGO_TIPO_FITA = r["DS_CODIGO_TIPO_FITA"].ToString();
                return entity;
            });

            DataTable dataTable = ObterLista(tipoFita);
            return (from r in dataTable.Rows.Cast<DataRow>()
                    select Converter(r));
        }

        public DataTable ObterLista(TipoFitaEntity tipoFita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoFitaSelect");

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