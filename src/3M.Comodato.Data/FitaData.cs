using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class FitaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public FitaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<FitaEntity> ObterListaEntity(FitaEntity fita)
        {
            Func<DataRow, FitaEntity> Converter = new Func<DataRow, FitaEntity>((r) =>
            {
                FitaEntity entity = new FitaEntity();
                entity.ID_FITA = Convert.ToInt64(r["ID_FITA"]);
                entity.CD_CODIGO= r["CD_CODIGO"].ToString();
                entity.DS_FITA = r["DS_FITA"].ToString();
                return entity;
            });

            DataTable dataTable = ObterLista(fita);
            return (from r in dataTable.Rows.Cast<DataRow>()
                    select Converter(r));
        }

        public DataTable ObterLista(FitaEntity fita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFitaSelect");

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
