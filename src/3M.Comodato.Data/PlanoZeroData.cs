using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace _3M.Comodato.Data
{
    public class PlanoZeroData
    {

        readonly Database _db;
        DbCommand dbCommand;

        public PlanoZeroData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref PlanoZeroEntity planoZero)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPlanoZeroInsert");
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, planoZero.ccdPeca);
                _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, planoZero.ccdModelo);
                _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, planoZero.ccdGrupoModelo);
                _db.AddInParameter(dbCommand, "@p_QT_PECA_MODELO", DbType.Decimal, planoZero.nqtPecaModelo);
                _db.AddInParameter(dbCommand, "@p_NM_PERC_PONDERACAO", DbType.Decimal, planoZero.nPonderacao);
                _db.AddInParameter(dbCommand, "@p_QT_ESTOQUE_MINIMO", DbType.Decimal, planoZero.nqtEstoqueMinimo);
                _db.AddInParameter(dbCommand, "@p_CD_CRITICIDADE_ABC", DbType.String, planoZero.ccdCriticidadeABC);
                _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Int64, planoZero.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, planoZero.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_ID_PLANO_ZERO", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                planoZero.nidPlanoZero = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_PLANO_ZERO"));

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

        public void Excluir(PlanoZeroEntity planoZero)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPlanoZeroDelete");

                _db.AddInParameter(dbCommand, "@p_ID_PLANO_ZERO", DbType.Int64, planoZero.nidPlanoZero);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, planoZero.nidUsuarioAtualizacao);

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

        public bool Alterar(PlanoZeroEntity planoZero)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPlanoZeroUpdate");


                _db.AddInParameter(dbCommand, "@p_ID_PLANO_ZERO", DbType.Int64, planoZero.nidPlanoZero);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, planoZero.ccdPeca);
                _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, planoZero.ccdModelo);
                _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, planoZero.ccdGrupoModelo);
                _db.AddInParameter(dbCommand, "@p_QT_PECA_MODELO", DbType.Decimal, planoZero.nqtPecaModelo);
                _db.AddInParameter(dbCommand, "@p_NM_PERC_PONDERACAO", DbType.Decimal, planoZero.nPonderacao);
                _db.AddInParameter(dbCommand, "@p_QT_ESTOQUE_MINIMO", DbType.Decimal, planoZero.nqtEstoqueMinimo);
                _db.AddInParameter(dbCommand, "@p_CD_CRITICIDADE_ABC", DbType.String, planoZero.ccdCriticidadeABC);
                _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Int64, planoZero.nidUsuarioAtualizacao);

                if (planoZero.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, planoZero.nidUsuarioAtualizacao);

                _db.ExecuteNonQuery(dbCommand);

                blnOK = true;
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

        public DataTable ObterLista(PlanoZeroEntity planoZero)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPlanoZeroSelect");

                if (planoZero.nidPlanoZero!= 0)
                    _db.AddInParameter(dbCommand, "@p_ID_PLANO_ZERO", DbType.Int64, planoZero.nidPlanoZero);

                if (!string.IsNullOrEmpty(planoZero.ccdPeca ))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, planoZero.ccdPeca);

                if (!string.IsNullOrEmpty(planoZero.grupoModelo.CD_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, planoZero.grupoModelo.CD_GRUPO_MODELO);

                if (!string.IsNullOrEmpty(planoZero.ccdModelo))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, planoZero.ccdModelo);

                if (planoZero.nqtPecaModelo!=0)
                    _db.AddInParameter(dbCommand, "@p_QT_PECA_MODELO", DbType.Decimal, planoZero.nqtPecaModelo);

                if (planoZero.nPonderacao != 0)
                    _db.AddInParameter(dbCommand, "@p_NM_PERC_PONDERACAO", DbType.Decimal, planoZero.nPonderacao);

                if (planoZero.nqtEstoqueMinimo != 0)
                    _db.AddInParameter(dbCommand, "@p_QT_ESTOQUE_MINIMO", DbType.Decimal, planoZero.nqtEstoqueMinimo);

                if (!string.IsNullOrEmpty(planoZero.ccdCriticidadeABC))
                    _db.AddInParameter(dbCommand, "@p_CD_CRITICIDADE_ABC", DbType.String, planoZero.ccdCriticidadeABC);

                if (planoZero.nidUsuarioAtualizacao != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Decimal, planoZero.nidUsuarioAtualizacao);


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

        public DataTable ObterCriticidadeCarteira(string ccdGrupoModelo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPlanoZeroCriticidadeSelect");

                if (!String.IsNullOrEmpty(ccdGrupoModelo))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, ccdGrupoModelo);

                //if (!String.IsNullOrEmpty(ccdModelo))
                //    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String , ccdModelo);

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

        public DataTable ObterQtSugerida(string CD_TECNICO, string CD_PECA)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPlanoZeroPedidoTecnico");

                if (!string.IsNullOrEmpty(CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                if (!string.IsNullOrEmpty(CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, CD_PECA);

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

        public IList<PlanoZeroSinc> ObterListaPlanoZeroSinc()
        {
            try
            {
                IList<PlanoZeroSinc> listaPZ = new List<PlanoZeroSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select * from tbPlanoZero ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            PlanoZeroSinc pz = new PlanoZeroSinc();
                            pz.ID_PLANO_ZERO = Convert.ToInt64( SDR["id_plano_zero"]);
                            pz.CD_PECA = Convert.ToString(SDR["cd_peca"] is DBNull ? "" : SDR["cd_peca"].ToString());
                            pz.CD_GRUPO_MODELO = Convert.ToString(SDR["cd_grupo_modelo"] is DBNull ? "" : SDR["cd_grupo_modelo"].ToString());
                            pz.QT_PECA_MODELO = Convert.ToDecimal(SDR["qt_peca_modelo"] is DBNull ? "0" : SDR["qt_peca_modelo"].ToString());
                            //pz.NM_PERC_PONDERACAO = Convert.ToDecimal(SDR["NM_PERC_PONDERACAO"] is DBNull ? "0" : SDR["NM_PERC_PONDERACAO"].ToString());
                            pz.CD_CRITICIDADE_ABC = Convert.ToChar(SDR["CD_CRITICIDADE_ABC"] is DBNull ? "" : SDR["CD_CRITICIDADE_ABC"].ToString());

                            listaPZ.Add(pz);
                        }
                        cnx.Close();
                        return listaPZ;
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
