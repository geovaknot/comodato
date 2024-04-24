using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class RRStatusData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public RRStatusData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<RRStatusEntity> ObterListaEntity(RRStatusEntity entity)
        {
            Func<DataRow, RRStatusEntity> Converter = new Func<DataRow, RRStatusEntity>((r) =>
            {
                RRStatusEntity tpStatuRR = new RRStatusEntity();
                tpStatuRR.ID_RR_STATUS = Convert.ToInt64(r["ID_RR_STATUS"]);
                tpStatuRR.ST_STATUS_RR = Convert.ToInt32(r["ST_STATUS_RR"]);
                tpStatuRR.DS_STATUS_NOME_REDUZ = Convert.ToString(r["DS_STATUS_NOME_REDUZ"]).Trim();

                if (r["DS_STATUS_NOME"] != DBNull.Value)
                {
                    tpStatuRR.DS_STATUS_NOME = Convert.ToString(r["DS_STATUS_NOME"]).Trim();
                }

                if (r["DS_STATUS_DESCRICAO"] != DBNull.Value)
                {
                    tpStatuRR.DS_STATUS_DESCRICAO = Convert.ToString(r["DS_STATUS_DESCRICAO"]).Trim();
                }

              

                return tpStatuRR;
            });

            DataTable dataTable = this.ObterLista(entity);
            return (from r in dataTable.Rows.Cast<DataRow>() select Converter(r)).ToList();
        }

        public DataTable ObterLista(RRStatusEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;
            List<RRStatusEntity> listaSolicitacaoAtendimento = new List<RRStatusEntity>();
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRRStatusSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_RR_STATUS != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_RR_STATUS", DbType.Int64, entity.ID_RR_STATUS);
                    }

                    if (entity.ST_STATUS_RR != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ST_STATUS_RR", DbType.Int32, entity.ST_STATUS_RR);
                    }

                    if (!string.IsNullOrEmpty(entity.DS_STATUS_NOME_REDUZ))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_STATUS_NOME_REDUZ", DbType.String, entity.DS_STATUS_NOME_REDUZ);
                    }
                    if (!string.IsNullOrEmpty(entity.DS_STATUS_NOME))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_STATUS_NOME", DbType.String, entity.DS_STATUS_NOME);
                    }
                    if (!string.IsNullOrEmpty(entity.DS_TRANSICAO))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_TRANSICAO", DbType.String, entity.DS_TRANSICAO);
                    }
                    if (!string.IsNullOrEmpty(entity.DS_COMENTARIO))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_COMENTARIO", DbType.String, entity.DS_COMENTARIO);
                    }
                    if (!string.IsNullOrEmpty(entity.DS_STATUS_DESCRICAO))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_STATUS_DESCRICAO", DbType.String, entity.DS_STATUS_DESCRICAO);
                    }


                    if (entity.NR_ORDEM_FLUXO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_NR_ORDEM_FLUXO", DbType.Int32, entity.NR_ORDEM_FLUXO);
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


        public DataTable ObterListaStatus(string statusCarregar, int ID_GRUPOWF)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRRStatusByStatusSelect");

                _db.AddInParameter(dbCommand, "@p_StatusCarregar", DbType.String, statusCarregar);
                _db.AddInParameter(dbCommand, "@p_ID_GRUPOWF", DbType.Int32, ID_GRUPOWF);

                
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
