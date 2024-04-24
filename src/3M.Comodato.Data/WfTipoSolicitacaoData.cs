using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class WfTipoSolicitacaoData
    {
        Database _db;
        DbCommand dbCommand;

        public WfTipoSolicitacaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<WfTipoSolicitacaoEntity> ObterListaEntity(WfTipoSolicitacaoEntity filter)
        {
            Func<DataRow, WfTipoSolicitacaoEntity> tipoSolicitacaoConverter = new Func<DataRow, WfTipoSolicitacaoEntity>((row) =>
            {
                WfTipoSolicitacaoEntity entity = new WfTipoSolicitacaoEntity();
                entity.ID_TIPO_SOLICITACAO = Convert.ToInt32(row["ID_TIPO_SOLICITACAO"]);
                entity.CD_TIPO_SOLICITACAO = Convert.ToInt32(row["CD_TIPO_SOLICITACAO"]);
                entity.DS_TIPO_SOLICITACAO = row["DS_TIPO_SOLICITACAO"].ToString();
                entity.FL_ATIVO = row["FL_ATIVO"].ToString();



                return entity;
            });

            List<WfTipoSolicitacaoEntity> listaTipoSolicitacao = new List<WfTipoSolicitacaoEntity>();
            try
            {
                DataTable dataTable = this.ObterLista(filter);
                listaTipoSolicitacao = (from r in dataTable.Rows.Cast<DataRow>()
                                               select tipoSolicitacaoConverter(r)).ToList();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listaTipoSolicitacao;
        }

        public DataTable ObterLista(WfTipoSolicitacaoEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;
            List<SolicitacaoAtendimentoEntity> listaSolicitacaoAtendimento = new List<SolicitacaoAtendimentoEntity>();
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfTipoSolicitacaoSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_TIPO_SOLICITACAO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_TIPO_SOLICITACAO", DbType.Int32, entity.ID_TIPO_SOLICITACAO);
                    }
                    if (entity.CD_TIPO_SOLICITACAO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_TIPO_SOLICITACAO", DbType.Int32, entity.CD_TIPO_SOLICITACAO);
                    }
                    if (!string.IsNullOrEmpty(entity.DS_TIPO_SOLICITACAO))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_TIPO_SOLICITACAO", DbType.String, entity.DS_TIPO_SOLICITACAO);
                    }
                    if (!string.IsNullOrEmpty(entity.FL_ATIVO))
                    {
                        _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, entity.FL_ATIVO);
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
    }
}
