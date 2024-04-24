using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class PecaData
    {

        readonly Database _db;
        private DbConnection _connection;
        private DbCommand dbCommand;
        internal DbTransaction _transaction;

        public PecaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }
        public PecaData(TransactionData transactionData)
        {
            _db = transactionData._db;
            _connection = transactionData._connection;
            _transaction = transactionData._transaction;
        }

        public bool Inserir(ref PecaEntity peca)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPecaInsert");

                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_DS_PECA", DbType.String, peca.DS_PECA);
                _db.AddInParameter(dbCommand, "@p_TX_UNIDADE", DbType.String, peca.TX_UNIDADE);
                _db.AddInParameter(dbCommand, "@p_QTD_ESTOQUE", DbType.Decimal, peca.QTD_ESTOQUE);
                _db.AddInParameter(dbCommand, "@p_QTD_MINIMA", DbType.Decimal, peca.QTD_MINIMA);
                _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, peca.VL_PECA);
                _db.AddInParameter(dbCommand, "@p_TP_PECA", DbType.String, peca.TP_PECA);
                //_db.AddInParameter(dbCommand, "@p_FL_ATIVO_PECA", DbType.String, peca.FL_ATIVO_PECA);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, peca.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_QTD_PlanoZero", DbType.Decimal, peca.QTD_PlanoZero);
                //_db.AddOutParameter(dbCommand, "@p_CD_PECA", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                //peca.CD_PECA = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_CD_PECA"));

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

        public void Excluir(PecaEntity peca)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPecaDelete");

                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);

                if (peca.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, peca.nidUsuarioAtualizacao);

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

        public bool Alterar(PecaEntity peca)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);

                if (!string.IsNullOrEmpty(peca.DS_PECA))
                    _db.AddInParameter(dbCommand, "@p_DS_PECA", DbType.String, peca.DS_PECA);

                if (!string.IsNullOrEmpty(peca.TX_UNIDADE))
                    _db.AddInParameter(dbCommand, "@p_TX_UNIDADE", DbType.String, peca.TX_UNIDADE);

                _db.AddInParameter(dbCommand, "@p_QTD_ESTOQUE", DbType.Decimal, peca.QTD_ESTOQUE);

                _db.AddInParameter(dbCommand, "@p_QTD_MINIMA", DbType.Decimal, peca.QTD_MINIMA);

                _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, peca.VL_PECA);

                if (!string.IsNullOrEmpty(peca.TP_PECA))
                    _db.AddInParameter(dbCommand, "@p_TP_PECA", DbType.String, peca.TP_PECA);

                if (!string.IsNullOrEmpty(peca.FL_ATIVO_PECA))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO_PECA", DbType.String, peca.FL_ATIVO_PECA);

                if (peca.QTD_PlanoZero >= 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_PlanoZero", DbType.Decimal, peca.QTD_PlanoZero);

                if (peca.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, peca.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(PecaEntity peca, string CD_TECNICO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaSelect");
                dbCommand.CommandTimeout = 500000;

                if (!string.IsNullOrEmpty(peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);

                if (!string.IsNullOrEmpty(peca.DS_PECA))
                    _db.AddInParameter(dbCommand, "@p_DS_PECA", DbType.String, peca.DS_PECA);

                if (!string.IsNullOrEmpty(peca.TX_UNIDADE))
                    _db.AddInParameter(dbCommand, "@p_TX_UNIDADE", DbType.String, peca.TX_UNIDADE);

                if (peca.QTD_ESTOQUE != 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_ESTOQUE", DbType.Decimal, peca.QTD_ESTOQUE);

                if (peca.QTD_MINIMA != 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_MINIMA", DbType.Decimal, peca.QTD_MINIMA);

                if (peca.VL_PECA != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, peca.VL_PECA);

                if (!string.IsNullOrEmpty(peca.TP_PECA))
                    _db.AddInParameter(dbCommand, "@p_TP_PECA", DbType.String, peca.TP_PECA);

                if (!string.IsNullOrEmpty(peca.FL_ATIVO_PECA))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO_PECA", DbType.String, peca.FL_ATIVO_PECA);

                if (!string.IsNullOrEmpty(CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                //connection.Close();
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

        public DataTable ObterListaNew(PecaEntity peca, string CD_TECNICO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaSelectNew");
                dbCommand.CommandTimeout = 500000;

                if (!string.IsNullOrEmpty(peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);

                if (!string.IsNullOrEmpty(peca.DS_PECA))
                    _db.AddInParameter(dbCommand, "@p_DS_PECA", DbType.String, peca.DS_PECA);

                if (!string.IsNullOrEmpty(peca.TX_UNIDADE))
                    _db.AddInParameter(dbCommand, "@p_TX_UNIDADE", DbType.String, peca.TX_UNIDADE);

                if (peca.QTD_ESTOQUE != 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_ESTOQUE", DbType.Decimal, peca.QTD_ESTOQUE);

                if (peca.QTD_MINIMA != 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_MINIMA", DbType.Decimal, peca.QTD_MINIMA);

                if (peca.VL_PECA != 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PECA", DbType.Decimal, peca.VL_PECA);

                if (!string.IsNullOrEmpty(peca.TP_PECA))
                    _db.AddInParameter(dbCommand, "@p_TP_PECA", DbType.String, peca.TP_PECA);

                if (!string.IsNullOrEmpty(peca.FL_ATIVO_PECA))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO_PECA", DbType.String, peca.FL_ATIVO_PECA);

                if (!string.IsNullOrEmpty(CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                //connection.Close();
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

        public DataTable ObterListaNewInativa(PecaEntity peca, string CD_TECNICO = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaSelectNew");
                dbCommand.CommandTimeout = 500000;

                if (!string.IsNullOrEmpty(peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);

                

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                //connection.Close();
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

        public DataTable ObterListaBPCS(PecaEntity peca)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaSelectBPCS");

                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                //connection.Close();
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

        public List<long> obterLotes(long ID_PEDIDO)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();            
            List<Int64> lista;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLotesSelect");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, ID_PEDIDO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);

                lista = dataSet.Tables[0].Rows.OfType<DataRow>()
                    .Select(dr => dr.Field<Int64>("LOTE")).ToList();
                //connection.Close();
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
            return lista;
        }

        public List<PecaEntity> ObterPecas(string cd_peca)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            List<PecaEntity> Lista = new List<PecaEntity>();


            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaSelect");

                if (!string.IsNullOrEmpty(cd_peca))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, cd_peca);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);

                dataTable = dataSet.Tables[0];

                if (dataTable.Rows.Count > 0)
                {

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        var dto = new PecaEntity();
                        dto.CD_PECA = dr["CD_PECA"].ToString();
                        dto.VL_PECA = Convert.ToDecimal(dr["VL_PECA"]);
                        dto.CD_PECA_RECUPERADA = dr["CD_PECA_RECUPERADA"].ToString();
                        dto.DS_PECA = dr["DS_PECA"].ToString();

                        Lista.Add(dto);
                    }


                }

                //connection.Close();

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
            return Lista;
        }

        public PecaEntity ObterPecasRecuperadas(string cd_peca)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            PecaEntity Lista = new PecaEntity();


            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaSelectRec");
                dbCommand.CommandTimeout = 500000;
                if (!string.IsNullOrEmpty(cd_peca))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, cd_peca);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);

                dataTable = dataSet.Tables[0];

                if (dataTable.Rows.Count > 0)
                {

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        Lista.CD_PECA = dr["CD_PECA"].ToString();
                        Lista.VL_PECA = Convert.ToDecimal(dr["VL_PECA"]);
                        Lista.CD_PECA_RECUPERADA = dr["CD_PECA_RECUPERADA"].ToString();
                        
                    }


                }

                //connection.Close();

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
            return Lista;
        }

        public DataTable ObterListaPecaByPedidoByTecnico(string CD_TECNICO)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                //dbCommand = _db.GetStoredProcCommand("prcPecaByPedidoByTecnicoSelect");
                dbCommand = _db.GetStoredProcCommand("prcEstoquePecaIntermediarioSelect");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                //connection.Close();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                connection.Close();
                throw ex;
            }
            catch (Exception ex)
            {
                connection.Close();
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
        /// Obtem lista de Pecas para o sincronismo Mobile
        /// </summary>
        /// <param ></param>
        /// <returns></returns>  
        public IList<PecaEntity> ObterListaPecaSinc(string codigoGrupoModelo)
        {
            try
            {
                IList<PecaEntity> listaPeca = new List<PecaEntity>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        if (string.IsNullOrWhiteSpace(codigoGrupoModelo))
                            cmd.CommandText = @"SELECT pz.ID_PLANO_ZERO, 
                                                       pz.CD_GRUPO_MODELO, 
                                                       pz.CD_CRITICIDADE_ABC, 
                                                       pe.*
                                                  FROM tb_peca pe 
                                                 LEFT JOIN tbplanozero pz
                                                    ON pe.CD_PECA = pz.CD_PECA";
                        else
                            cmd.CommandText = @"SELECT pz.ID_PLANO_ZERO, 
                                                       pz.CD_GRUPO_MODELO, 
                                                       pz.CD_CRITICIDADE_ABC, 
                                                       pe.*
                                                  FROM tb_peca pe 
                                                 LEFT JOIN tbplanozero pz
                                                    ON pe.CD_PECA = pz.CD_PECA
                                                 WHERE pz.CD_GRUPO_MODELO = '" + codigoGrupoModelo + "'";

                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PecaEntity peca = new PecaEntity();
                            peca.CD_PECA= Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString());
                            peca.DS_PECA= Convert.ToString(SDR["DS_PECA"] is DBNull ? "" : SDR["DS_PECA"].ToString());
                            peca.TX_UNIDADE= Convert.ToString(SDR["TX_UNIDADE"] is DBNull ? "" : SDR["TX_UNIDADE"].ToString());
                            peca.QTD_ESTOQUE= Convert.ToInt64(SDR["QTD_ESTOQUE"] is DBNull ? 0 : SDR["QTD_ESTOQUE"]);
                            peca.QTD_MINIMA= Convert.ToInt64(SDR["QTD_MINIMA"] is DBNull ? 0 : SDR["QTD_MINIMA"]);
                            peca.VL_PECA= Convert.ToInt64(SDR["VL_PECA"] is DBNull ? 0 : SDR["VL_PECA"]);
                            peca.TP_PECA= Convert.ToString(SDR["TP_PECA"] is DBNull ? "" : SDR["TP_PECA"].ToString());
                            peca.FL_ATIVO_PECA= Convert.ToString(SDR["FL_ATIVO_PECA"] is DBNull ? "" : SDR["FL_ATIVO_PECA"].ToString());
                            peca.CD_CRITICIDADE_ABC = Convert.ToString(SDR["CD_CRITICIDADE_ABC"] is DBNull ? "" : SDR["CD_CRITICIDADE_ABC"].ToString());
                            peca.CD_GRUPO_MODELO = Convert.ToString(SDR["CD_GRUPO_MODELO"] is DBNull ? "" : SDR["CD_GRUPO_MODELO"].ToString());
                            peca.ID_PLANO_ZERO = Convert.ToInt64(SDR["ID_PLANO_ZERO"] is DBNull ? 0 : SDR["ID_PLANO_ZERO"]);
                            listaPeca.Add(peca);
                        }
                        cnx.Close();
                        return listaPeca;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void grava_PecaRecuperada(PecaEntity peca)
        {
            
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPecaRecuperadaInsert");

                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, peca.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_PECA_RECUPERADA", DbType.String, peca.CD_PECA_RECUPERADA);
                
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

        public void RemoverRecuperada(string CD_PECA)
        {
            try
            {
                IList<PecaEntity> listaPedidoPeca = new List<PecaEntity>();
                SqlDataReader SDR = null;

                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                              @"delete from tbPecaRecuperada where cd_peca = @CD_PECA ";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CD_PECA", SqlDbType.VarChar).Value = CD_PECA;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();



                        cnx.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
