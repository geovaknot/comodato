using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class RRComentData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public RRComentData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref RRComentEntity entity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRRComentInsert");
                _db.AddInParameter(dbCommand, "@p_ID_RELATORIO_RECLAMACAO", DbType.Int64, entity.relatorioReclamacao.ID_RELATORIO_RECLAMACAO);
                _db.AddInParameter(dbCommand, "@p_DS_COMENT", DbType.String, entity.DS_COMENT);
                _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, entity.usuario.nidUsuario);

                _db.AddOutParameter(dbCommand, "@p_ID_RR_COMMENT", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                entity.ID_RR_COMMENT = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_RR_COMMENT"));

                retorno = true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public DataTable ObterLista()
        {
            return ObterLista(null);
        }
        public DataTable ObterLista(RRComentEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRRComentSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_RR_COMMENT != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_RR_COMMENT", DbType.Int64, entity.ID_RR_COMMENT);
                    }

                    if (entity.relatorioReclamacao.ID_RELATORIO_RECLAMACAO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_RELATORIO_RECLAMACAO", DbType.Int64, entity.relatorioReclamacao.ID_RELATORIO_RECLAMACAO);
                    }

                    if (!string.IsNullOrEmpty(entity.DS_COMENT))
                    {   
                        _db.AddInParameter(dbCommand, "@p_DS_COMENT", DbType.String, entity.DS_COMENT);
                    }

                    if (entity.nidUsuarioAtualizacao != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, entity.nidUsuarioAtualizacao);
                    }

                    if (entity.dtmDataHoraAtualizacao != DateTime.MinValue)
                    {
                        _db.AddInParameter(dbCommand, "@p_DT_REGISTRO", DbType.DateTime, entity.dtmDataHoraAtualizacao);
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

        public IEnumerable<RRComentEntity> ObterListaEntity()
        {
            return ObterListaEntity(null);
        }
        public IEnumerable<RRComentEntity> ObterListaEntity(RRComentEntity entity)
        {
            DataTable dtCategoria = ObterLista(entity);
            return (from r in dtCategoria.Rows.Cast<DataRow>()
                    select new RRComentEntity()
                    {
                        ID_RR_COMMENT = Convert.ToInt64(r["ID_RR_COMMENT"]),
                        relatorioReclamacao = new RelatorioReclamacaoEntity
                        {
                            ID_RELATORIO_RECLAMACAO = Convert.ToInt64(r["ID_RELATORIO_RECLAMACAO"]),

                        },
                        DS_COMENT = r["DS_COMENT"].ToString(),
                        nidUsuarioAtualizacao = Convert.ToInt64(r["ID_USUARIO"]),
                        dtmDataHoraAtualizacao = Convert.ToDateTime(r["DT_REGISTRO"]),
                        usuario = new UsuarioEntity
                        {
                            nidUsuario = Convert.ToInt64(r["ID_USUARIO"]),
                            cnmNome = r["cnmNome"].ToString(),
                        },
                    }).ToList();
        }


        /// <summary>
        /// Obtem lista de RRComent Ativos para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<RRComentSincEntity> ObterListaRRComentSinc(Int64? idUsuario)
        {
            try
            {
                IList<RRComentSincEntity> listaRRComent = new List<RRComentSincEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                            " select rrc.* from tbRRcoment rrc "+
                            " where rrc.ID_RELATORIO_RECLAMACAO in ( " +
                            "   select t.ID_RELATORIO_RECLAMACAO " +
                            "   from tbRRrelatorioReclamacao t " +
                            "   inner join tb_tecnico tt ON tt.CD_TECNICO = t.CD_TECNICO " +
                            "   where dtmDataHoraAtualizacao > (getdate() - 90) AND " +
                            "     (tt.id_usuario = @id_usuario OR @id_usuario = '') " +
                            " )";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.NVarChar).Value = Convert.ToString(idUsuario);

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            RRComentSincEntity rRComent = new RRComentSincEntity();
                            rRComent.ID_RR_COMMENT = Convert.ToInt64(SDR["ID_RR_COMMENT"] is DBNull ? 0 : SDR["ID_RR_COMMENT"]);
                            rRComent.ID_RELATORIO_RECLAMACAO = Convert.ToInt64(SDR["ID_RELATORIO_RECLAMACAO"] is DBNull ? 0 : SDR["ID_RELATORIO_RECLAMACAO"]);
                            rRComent.DS_COMENT = Convert.ToString(SDR["DS_COMENT"] is DBNull ? "" : SDR["DS_COMENT"].ToString());
                            rRComent.DT_REGISTRO = Convert.ToDateTime(SDR["DT_REGISTRO"] is DBNull ? "01/01/2000" : SDR["DT_REGISTRO"]);
                            rRComent.nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);

                            listaRRComent.Add(rRComent);
                        }
                        cnx.Close();
                        return listaRRComent;
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
