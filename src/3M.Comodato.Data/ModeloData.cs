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
    public class ModeloData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public ModeloData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref ModeloEntity modelo)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcModeloInsert");

                if (!string.IsNullOrEmpty(modelo.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, modelo.CD_MODELO);

                if (!string.IsNullOrEmpty(modelo.DS_MODELO))
                    _db.AddInParameter(dbCommand, "@p_DS_MODELO", DbType.String, modelo.DS_MODELO);

                if (!string.IsNullOrEmpty(modelo.CD_MOD_NR12))
                    _db.AddInParameter(dbCommand, "@p_CD_MOD_NR12", DbType.String, modelo.CD_MOD_NR12);

                if (!string.IsNullOrEmpty(modelo.CD_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, modelo.CD_GRUPO_MODELO);

                _db.AddInParameter(dbCommand, "@p_VL_COMPLEXIDADE_EQUIP", DbType.Int64, modelo.VL_COMPLEXIDADE_EQUIP);

                if (!string.IsNullOrEmpty(modelo.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, modelo.FL_ATIVO);

                if (!string.IsNullOrEmpty(modelo.cdsFL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_cdsFL_ATIVO", DbType.String, modelo.cdsFL_ATIVO);

                if (!string.IsNullOrEmpty(modelo.TP_EMPACOTAMENTO))
                    _db.AddInParameter(dbCommand, "@p_TP_EMPACOTAMENTO", DbType.String, modelo.TP_EMPACOTAMENTO);

                if (modelo.VL_COMP_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_MIN", DbType.Decimal, modelo.VL_COMP_MIN);

                if (modelo.VL_COMP_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_MAX", DbType.Decimal, modelo.VL_COMP_MAX);

                if (modelo.VL_LARG_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_MIN", DbType.Decimal, modelo.VL_LARG_MIN);

                if (modelo.VL_LARG_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_MAX", DbType.Decimal, modelo.VL_LARG_MAX);

                if (modelo.VL_ALTUR_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_MIN", DbType.Decimal, modelo.VL_ALTUR_MIN);

                if (modelo.VL_ALTUR_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_MAX", DbType.Decimal, modelo.VL_ALTUR_MAX);

                if (modelo.VL_LARG_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_CAIXA", DbType.Decimal, modelo.VL_LARG_CAIXA);

                if (modelo.VL_ALTUR_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_CAIXA", DbType.Decimal, modelo.VL_ALTUR_CAIXA);

                if (modelo.VL_COMP_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_CAIXA", DbType.Decimal, modelo.VL_COMP_CAIXA);

                if (modelo.VL_PESO_CUBADO > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PESO_CUBADO", DbType.Decimal, modelo.VL_PESO_CUBADO);

                _db.AddInParameter(dbCommand, "@p_VL_PROJETADO", DbType.Decimal, modelo.VL_PROJETADO);

                if (modelo.CATEGORIA.ID_CATEGORIA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_CATEGORIA", DbType.Int64, modelo.CATEGORIA.ID_CATEGORIA);

                if (modelo.LINHA_PRODUTO.CD_LINHA_PRODUTO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Int32, modelo.LINHA_PRODUTO.CD_LINHA_PRODUTO);

                if (modelo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, modelo.nidUsuarioAtualizacao);

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

        public void Excluir(ModeloEntity modelo)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcModeloDelete");

                _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, modelo.CD_MODELO);

                if (modelo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, modelo.nidUsuarioAtualizacao);

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

        public bool Alterar(ModeloEntity modelo)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcModeloUpdate");

                if (!string.IsNullOrEmpty(modelo.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, modelo.CD_MODELO);

                if (!string.IsNullOrEmpty(modelo.DS_MODELO))
                    _db.AddInParameter(dbCommand, "@p_DS_MODELO", DbType.String, modelo.DS_MODELO);

                if (!string.IsNullOrEmpty(modelo.CD_MOD_NR12))
                    _db.AddInParameter(dbCommand, "@p_CD_MOD_NR12", DbType.String, modelo.CD_MOD_NR12);

                if (!string.IsNullOrEmpty(modelo.CD_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, modelo.CD_GRUPO_MODELO);

                if (!string.IsNullOrEmpty(modelo.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, modelo.FL_ATIVO);

                //if (!string.IsNullOrEmpty(modelo.cdsFL_ATIVO))
                //    _db.AddInParameter(dbCommand, "@p_cdsFL_ATIVO", DbType.String, modelo.cdsFL_ATIVO);

                if (!string.IsNullOrEmpty(modelo.TP_EMPACOTAMENTO))
                    _db.AddInParameter(dbCommand, "@p_TP_EMPACOTAMENTO", DbType.String, modelo.TP_EMPACOTAMENTO);

                if (modelo.VL_COMP_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_MIN", DbType.Decimal, modelo.VL_COMP_MIN);

                if (modelo.VL_COMP_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_MAX", DbType.Decimal, modelo.VL_COMP_MAX);

                if (modelo.VL_LARG_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_MIN", DbType.Decimal, modelo.VL_LARG_MIN);

                if (modelo.VL_LARG_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_MAX", DbType.Decimal, modelo.VL_LARG_MAX);

                if (modelo.VL_ALTUR_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_MIN", DbType.Decimal, modelo.VL_ALTUR_MIN);

                _db.AddInParameter(dbCommand, "@p_VL_COMPLEXIDADE_EQUIP", DbType.Int64, modelo.VL_COMPLEXIDADE_EQUIP);

                _db.AddInParameter(dbCommand, "@p_VL_PROJETADO", DbType.Int64, modelo.VL_PROJETADO);

                if (modelo.VL_ALTUR_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_MAX", DbType.Decimal, modelo.VL_ALTUR_MAX);

                if (modelo.VL_LARG_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_CAIXA", DbType.Decimal, modelo.VL_LARG_CAIXA);

                if (modelo.VL_ALTUR_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_CAIXA", DbType.Decimal, modelo.VL_ALTUR_CAIXA);

                if (modelo.VL_COMP_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_CAIXA", DbType.Decimal, modelo.VL_COMP_CAIXA);

                if (modelo.VL_PESO_CUBADO > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PESO_CUBADO", DbType.Decimal, modelo.VL_PESO_CUBADO);

                if (modelo.CATEGORIA.ID_CATEGORIA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_CATEGORIA", DbType.Decimal, modelo.CATEGORIA.ID_CATEGORIA);

                if (modelo.LINHA_PRODUTO.CD_LINHA_PRODUTO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Decimal, modelo.LINHA_PRODUTO.CD_LINHA_PRODUTO);


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

        public DataTable ObterLista(ModeloEntity modelo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcModeloSelect");

                if (!string.IsNullOrEmpty(modelo.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, modelo.CD_MODELO);

                if (!string.IsNullOrEmpty(modelo.DS_MODELO))
                    _db.AddInParameter(dbCommand, "@p_DS_MODELO", DbType.String, modelo.DS_MODELO);

                if (!string.IsNullOrEmpty(modelo.CD_MOD_NR12))
                    _db.AddInParameter(dbCommand, "@p_CD_MOD_NR12", DbType.String, modelo.CD_MOD_NR12);

                if (!string.IsNullOrEmpty(modelo.CD_GRUPO_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_MODELO", DbType.String, modelo.CD_GRUPO_MODELO);

                if (!string.IsNullOrEmpty(modelo.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, modelo.FL_ATIVO);

                if (!string.IsNullOrEmpty(modelo.cdsFL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_cdsFL_ATIVO", DbType.String, modelo.cdsFL_ATIVO);

                if (modelo.VL_COMPLEXIDADE_EQUIP > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMPLEXIDADE_EQUIP", DbType.Int64, modelo.VL_COMPLEXIDADE_EQUIP);

                if (!string.IsNullOrEmpty(modelo.TP_EMPACOTAMENTO))
                    _db.AddInParameter(dbCommand, "@p_TP_EMPACOTAMENTO", DbType.String, modelo.TP_EMPACOTAMENTO);

                if (modelo.VL_COMP_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_MIN", DbType.Int32, modelo.VL_COMP_MIN);

                if (modelo.VL_COMP_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_MAX", DbType.Int32, modelo.VL_COMP_MAX);

                if (modelo.VL_LARG_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_MIN", DbType.Int32, modelo.VL_LARG_MIN);

                if (modelo.VL_LARG_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_MAX", DbType.Int32, modelo.VL_LARG_MAX);

                if (modelo.VL_ALTUR_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_MIN", DbType.Int32, modelo.VL_ALTUR_MIN);

                if (modelo.VL_ALTUR_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_MAX", DbType.Int32, modelo.VL_ALTUR_MAX);

                if (modelo.VL_LARG_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARG_CAIXA", DbType.Int32, modelo.VL_LARG_CAIXA);

                if (modelo.VL_ALTUR_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTUR_CAIXA", DbType.Int32, modelo.VL_ALTUR_CAIXA);

                if (modelo.VL_COMP_CAIXA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMP_CAIXA", DbType.Int32, modelo.VL_COMP_CAIXA);

                if (modelo.VL_PESO_CUBADO > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PESO_CUBADO", DbType.Int32, modelo.VL_PESO_CUBADO);

                if (modelo.VL_PROJETADO > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PROJETADO", DbType.Decimal, modelo.VL_PROJETADO);

                if (modelo.CATEGORIA.ID_CATEGORIA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_CATEGORIA", DbType.Decimal, modelo.CATEGORIA.ID_CATEGORIA);

                if (modelo.LINHA_PRODUTO.CD_LINHA_PRODUTO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA_PRODUTO", DbType.Decimal, modelo.LINHA_PRODUTO.CD_LINHA_PRODUTO);


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

        public DataTable ObterListaCombo(ModeloEntity modelo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcModeloComboSelect");


                if (!string.IsNullOrEmpty(modelo.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, modelo.FL_ATIVO);

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
        public IList<ModeloSinc> ObterListaModeloSinc()
        {
            try
            {
                IList<ModeloSinc> listaModelo = new List<ModeloSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select m.* from tb_modelo m " +
                                         " WHERE exists (select top 1 a.cd_modelo from TB_ATIVO_FIXO a " +
                                         " where a.FL_STATUS = 1 and a.cd_modelo = m.cd_modelo) ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            ModeloSinc modelo = new ModeloSinc();
                            modelo.CD_MODELO = Convert.ToString(SDR["CD_MODELO"]);
                            modelo.DS_MODELO = Convert.ToString(SDR["DS_MODELO"] is DBNull ? "" : SDR["DS_MODELO"].ToString());
                            modelo.FL_ATIVO = Convert.ToString(SDR["FL_ATIVO"] is DBNull ? "" : SDR["FL_ATIVO"].ToString());
                            modelo.CD_GRUPO_MODELO = Convert.ToString(SDR["CD_GRUPO_MODELO"] is DBNull ? "" : SDR["CD_GRUPO_MODELO"].ToString());
                            modelo.VL_COMPLEXIDADE_EQUIP = Convert.ToInt64("0" + SDR["VL_COMPLEXIDADE_EQUIP"]);
                            listaModelo.Add(modelo);
                        }
                        cnx.Close();
                        return listaModelo;
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
