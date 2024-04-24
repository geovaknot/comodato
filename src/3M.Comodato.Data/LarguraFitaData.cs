using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class LarguraFitaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public LarguraFitaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<LarguraFitaEntity> ObterListaEntity(LarguraFitaEntity larguraFita)
        {
            Func<DataRow, LarguraFitaEntity> Converter = new Func<DataRow, LarguraFitaEntity>((r) =>
            {
                LarguraFitaEntity entity = new LarguraFitaEntity();
                entity.ID_LARGURA_FITA = Convert.ToInt64(r["ID_LARGURA_FITA"]);
                entity.CD_LARGURA_FITA = r["CD_LARGURA_FITA"].ToString();
                entity.DS_LARGURA_FITA = r["DS_LARGURA_FITA"].ToString();
                return entity;
            });

            DataTable dataTable = ObterLista(larguraFita);
            return (from r in dataTable.Rows.Cast<DataRow>()
                    select Converter(r));
        }

        public DataTable ObterLista(LarguraFitaEntity larguraFita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLarguraFitaSelect");

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
