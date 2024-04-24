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
    public class WfGrupoUsuData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public WfGrupoUsuData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref WfGrupoUsuEntity usuarioXWFGrupoEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcWfGrupoUsuInsert");

                _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, usuarioXWFGrupoEntity.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_ID_GRUPOWF", DbType.Int64, usuarioXWFGrupoEntity.grupoWf.ID_GRUPOWF);
                _db.AddInParameter(dbCommand, "@p_NM_PRIORIDADE", DbType.Int32, usuarioXWFGrupoEntity.NM_PRIORIDADE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioXWFGrupoEntity.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

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

        public void Excluir(WfGrupoUsuEntity usuarioXWFGrupoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcWfGrupoUsuDelete");

                _db.AddInParameter(dbCommand, "@p_ID_GRUPOWF_USU", DbType.Int64, usuarioXWFGrupoEntity.ID_GRUPOWF_USU);

                if (usuarioXWFGrupoEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioXWFGrupoEntity.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<WfGrupoUsuEntity> ObterListaEntity(WfGrupoUsuEntity usuarioXWFGrupoEntity)
        {
            var data = this.ObterLista(usuarioXWFGrupoEntity);

            return (from r in data.Rows.Cast<DataRow>()
                    select new WfGrupoUsuEntity()
                    {
                        ID_GRUPOWF_USU = Convert.ToInt64(r["ID_GRUPOWF_USU"]),
                        usuario = new UsuarioEntity()
                        {
                            nidUsuario = Convert.ToInt64(r["ID_USUARIO"]),
                            cnmNome = r["cnmNome"].ToString(),
                            cdsEmail = r["cdsEmail"].ToString()
                        },
                        grupoWf = new WfGrupoEntity()
                        {
                            ID_GRUPOWF = Convert.ToInt32(r["ID_GRUPOWF"]),
                            CD_GRUPOWF = r["CD_GRUPOWF"].ToString(),
                            DS_GRUPOWF = r["DS_GRUPOWF"].ToString(),
                            TP_GRUPOWF = r["TP_GRUPOWF"].ToString(),
                        },
                        NM_PRIORIDADE = Convert.ToInt32(r["NM_PRIORIDADE"])
                    }
            ).ToList();

            //grupoEntity = new WfGrupoUsuEntity();
         
            //listaGrupos.Add(grupoEntity);
        }

        public DataTable ObterLista(WfGrupoUsuEntity usuarioXWFGrupoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfGrupoUsuSelect");

                if (usuarioXWFGrupoEntity.ID_GRUPOWF_USU > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_GRUPOWF_USU", DbType.Int64, usuarioXWFGrupoEntity.ID_GRUPOWF_USU);

                if (usuarioXWFGrupoEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, usuarioXWFGrupoEntity.usuario.nidUsuario);

                if (usuarioXWFGrupoEntity.grupoWf.ID_GRUPOWF > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_GRUPOWF", DbType.Int64, usuarioXWFGrupoEntity.grupoWf.ID_GRUPOWF);

                if (!string.IsNullOrEmpty(usuarioXWFGrupoEntity.grupoWf.CD_GRUPOWF))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPOWF", DbType.String, usuarioXWFGrupoEntity.grupoWf.CD_GRUPOWF);

                if (usuarioXWFGrupoEntity.NM_PRIORIDADE > 0)
                    _db.AddInParameter(dbCommand, "@p_NM_PRIORIDADE", DbType.Int64, usuarioXWFGrupoEntity.NM_PRIORIDADE);

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
