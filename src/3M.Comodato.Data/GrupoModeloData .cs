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
    public class GrupoModeloData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public GrupoModeloData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref GrupoModeloEntity grupoModelo)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcGrupoModeloInsert");

                //_db.AddInParameter(dbCommand, "@p_ID_GRUPO_MODELO", DbType.String, grupoModelo.ID_GRUPO_MODELO);
                _db.AddInParameter(dbCommand, "@p_DS_GRUPO_MODELO", DbType.String, grupoModelo.DS_GRUPO_MODELO);
                _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, grupoModelo.CD_GRUPO_MODELO);

                if (grupoModelo.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, grupoModelo.nidUsuarioAtualizacao);
                }

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

        public void Excluir(GrupoModeloEntity grupoModelo)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcGrupoModeloDelete");

                _db.AddInParameter(dbCommand, "@p_ID_GRUPO_MODELO", DbType.Int64, grupoModelo.ID_GRUPO_MODELO);

                if (grupoModelo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, grupoModelo.nidUsuarioAtualizacao);

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

        public bool Alterar(GrupoModeloEntity grupoModelo)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcGrupoModeloUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_GRUPO_MODELO", DbType.Int64, grupoModelo.ID_GRUPO_MODELO);

                _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, grupoModelo.CD_GRUPO_MODELO);

                _db.AddInParameter(dbCommand, "@p_DS_GRUPO_MODELO", DbType.String, grupoModelo.DS_GRUPO_MODELO);

                if (grupoModelo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, grupoModelo.nidUsuarioAtualizacao);


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
            return blnOK;
        }

        public DataTable ObterLista(GrupoModeloEntity grupoModelo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcGrupoModeloSelect");

                if (grupoModelo.ID_GRUPO_MODELO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_GRUPO_MODELO", DbType.String, grupoModelo.ID_GRUPO_MODELO);

                if (!string.IsNullOrEmpty(grupoModelo.CD_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, grupoModelo.CD_GRUPO_MODELO);

                if (!string.IsNullOrEmpty(grupoModelo.DS_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_DS_GRUPO_MODELO", DbType.String, grupoModelo.DS_GRUPO_MODELO);

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

        /// <summary>
        /// Obtem lista de Modelos de equipamentos para o sincronismo Mobile
        /// </summary>
        /// <param ></param>
        /// <returns></returns>  
        public IList<GrupoModeloEntity> ObterListaGrupoModeloSinc()
        {
            try
            {
                IList<GrupoModeloEntity> listaGrupoModelo = new List<GrupoModeloEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select * from tbGrupoModelo";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            GrupoModeloEntity grupoModelo = new GrupoModeloEntity();
                            grupoModelo.ID_GRUPO_MODELO = Convert.ToInt32(SDR["id_grupoModelo"]);
                            grupoModelo.CD_GRUPO_MODELO= Convert.ToString(SDR["cd_grupoModelo"] is DBNull ? "" : SDR["cd_grupoModelo"].ToString());
                            grupoModelo.DS_GRUPO_MODELO= Convert.ToString(SDR["ds_grupoModelo"] is DBNull ? "" : SDR["ds_grupoModelo"].ToString());

                            listaGrupoModelo.Add(grupoModelo);
                        }
                        cnx.Close();
                        return listaGrupoModelo;
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
