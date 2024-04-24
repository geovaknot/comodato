using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class CategoriaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public CategoriaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista()
        {
            return ObterLista(null);
        }

        public DataTable ObterLista(CategoriaEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;
            
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcCategoriaSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_CATEGORIA != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_CATEGORIA", DbType.Int64, entity.ID_CATEGORIA);
                    }
                    
                    if (!string.IsNullOrEmpty(entity.CD_CATEGORIA))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_CATEGORIA", DbType.String, entity.CD_CATEGORIA);
                    }

                    if (!string.IsNullOrEmpty(entity.DS_CATEGORIA))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_CATEGORIA", DbType.String, entity.DS_CATEGORIA);
                    }

                    if (!string.IsNullOrEmpty(entity.bidAtivo))
                    {
                        _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, entity.bidAtivo);
                    }
                    #endregion
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
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

        public IEnumerable<CategoriaEntity> ObterListaEntity()
        {
            return ObterListaEntity(null);
        }
        public IEnumerable<CategoriaEntity> ObterListaEntity(CategoriaEntity entity)
        {
            DataTable dtCategoria = ObterLista(entity);
            return (from r in dtCategoria.Rows.Cast<DataRow>()
                                  select new CategoriaEntity()
                                  {
                                    ID_CATEGORIA = Convert.ToInt64(r["ID_CATEGORIA"]),
                                      CD_CATEGORIA = r["CD_CATEGORIA"].ToString(),
                                      DS_CATEGORIA = r["DS_CATEGORIA"].ToString(),
                                    bidAtivo = r["FL_ATIVO"].ToString()
                                  }
                                  ).ToList();
        }

    }
}
