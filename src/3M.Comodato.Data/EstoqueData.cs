using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class EstoqueData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public EstoqueData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref EstoqueEntity estoque)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoqueInsert");
                _db.AddInParameter(dbCommand, "@p_CD_ESTOQUE", DbType.String, estoque.CD_ESTOQUE);
                _db.AddInParameter(dbCommand, "@p_DS_ESTOQUE", DbType.String, estoque.DS_ESTOQUE);
                _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Int64, estoque.ID_USU_RESPONSAVEL);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, estoque.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_TP_ESTOQUE_TEC_3M", DbType.String, estoque.TP_ESTOQUE_TEC_3M);
                _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.FL_ATIVO);

                if (estoque.Cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, estoque.Cliente.CD_CLIENTE);
                }

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, estoque.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                estoque.ID_ESTOQUE = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_ESTOQUE"));

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

        public bool Alterar(EstoqueEntity estoque)
        {
            bool blnOK = false;
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoqueUpdate");
                _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, estoque.ID_ESTOQUE);
                _db.AddInParameter(dbCommand, "@p_CD_ESTOQUE", DbType.String, estoque.CD_ESTOQUE);
                _db.AddInParameter(dbCommand, "@p_DS_ESTOQUE", DbType.String, estoque.DS_ESTOQUE);
                _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Int64, estoque.ID_USU_RESPONSAVEL);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, estoque.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_TP_ESTOQUE_TEC_3M", DbType.String, estoque.TP_ESTOQUE_TEC_3M);
                _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.FL_ATIVO);
                if (estoque.Cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@P_CD_CLIENTE", DbType.Int64, estoque.Cliente.CD_CLIENTE);
                }
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, estoque.nidUsuarioAtualizacao);

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

        public void Excluir(EstoqueEntity estoque)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoqueDelete");
                _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, estoque.ID_ESTOQUE);

                if (estoque.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, estoque.nidUsuarioAtualizacao);
                }

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

        public DataTable ObterLista(EstoqueEntity estoque)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoqueSelectTodos");
                dbCommand.CommandTimeout = 500000;
                if (estoque.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, estoque.ID_ESTOQUE);
                }

                if (!string.IsNullOrEmpty(estoque.CD_ESTOQUE))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ESTOQUE", DbType.String, estoque.CD_ESTOQUE);
                }

                if (!string.IsNullOrEmpty(estoque.DS_ESTOQUE))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_ESTOQUE", DbType.String, estoque.DS_ESTOQUE);
                }

                //if (estoque.ID_USU_RESPONSAVEL > 0)
                //{
                //    _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Int64, estoque.ID_USU_RESPONSAVEL);
                //}

                if (!estoque.dtmDataHoraAtualizacao.Equals(DateTime.MinValue))
                {
                    _db.AddInParameter(dbCommand, "@p_DT_CRIACAO", DbType.DateTime, estoque.dtmDataHoraAtualizacao);
                }

                if (!string.IsNullOrEmpty(estoque.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, estoque.tecnico.CD_TECNICO);
                }

                //if (!string.IsNullOrEmpty(estoque.TP_ESTOQUE_TEC_3M))
                //{
                //    _db.AddInParameter(dbCommand, "@p_TP_ESTOQUE_TEC_3M", DbType.String, estoque.TP_ESTOQUE_TEC_3M);
                //}

                if (estoque.bidAtivo.HasValue && string.IsNullOrEmpty(estoque.FL_ATIVO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.bidAtivo.Value ? "S" : "N");
                }

                if (!estoque.bidAtivo.HasValue && !string.IsNullOrEmpty(estoque.FL_ATIVO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.FL_ATIVO);
                }

                if (estoque.Cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, estoque.Cliente.CD_CLIENTE);
                }

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

        public DataTable ObterEstoqueTecnico(EstoqueEntity estoque)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoqueSelectTecnico");

                if (estoque.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, estoque.ID_ESTOQUE);
                }

                if (!string.IsNullOrEmpty(estoque.CD_ESTOQUE))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ESTOQUE", DbType.String, estoque.CD_ESTOQUE);
                }

                if (!string.IsNullOrEmpty(estoque.DS_ESTOQUE))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_ESTOQUE", DbType.String, estoque.DS_ESTOQUE);
                }

                //if (estoque.ID_USU_RESPONSAVEL > 0)
                //{
                //    _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Int64, estoque.ID_USU_RESPONSAVEL);
                //}

                if (!estoque.dtmDataHoraAtualizacao.Equals(DateTime.MinValue))
                {
                    _db.AddInParameter(dbCommand, "@p_DT_CRIACAO", DbType.DateTime, estoque.dtmDataHoraAtualizacao);
                }

                if (!string.IsNullOrEmpty(estoque.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, estoque.tecnico.CD_TECNICO);
                }

                //if (!string.IsNullOrEmpty(estoque.TP_ESTOQUE_TEC_3M))
                //{
                //    _db.AddInParameter(dbCommand, "@p_TP_ESTOQUE_TEC_3M", DbType.String, estoque.TP_ESTOQUE_TEC_3M);
                //}

                if (estoque.bidAtivo.HasValue && string.IsNullOrEmpty(estoque.FL_ATIVO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.bidAtivo.Value ? "S" : "N");
                }

                if (!estoque.bidAtivo.HasValue && !string.IsNullOrEmpty(estoque.FL_ATIVO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.FL_ATIVO);
                }

                if (estoque.Cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, estoque.Cliente.CD_CLIENTE);
                }

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

        public DataTable ObterListaEstoqueResponsavel(EstoqueEntity estoque)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEstoqueSelectTodosUser");

                if (estoque.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ESTOQUE", DbType.Int64, estoque.ID_ESTOQUE);
                }

                if (!string.IsNullOrEmpty(estoque.CD_ESTOQUE))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ESTOQUE", DbType.String, estoque.CD_ESTOQUE);
                }

                if (!string.IsNullOrEmpty(estoque.DS_ESTOQUE))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_ESTOQUE", DbType.String, estoque.DS_ESTOQUE);
                }

                if (estoque.ID_USU_RESPONSAVEL > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_USU_RESPONSAVEL", DbType.Int64, estoque.ID_USU_RESPONSAVEL);
                }

                if (!estoque.dtmDataHoraAtualizacao.Equals(DateTime.MinValue))
                {
                    _db.AddInParameter(dbCommand, "@p_DT_CRIACAO", DbType.DateTime, estoque.dtmDataHoraAtualizacao);
                }

                if (!string.IsNullOrEmpty(estoque.tecnico.CD_TECNICO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, estoque.tecnico.CD_TECNICO);
                }

                if (!string.IsNullOrEmpty(estoque.TP_ESTOQUE_TEC_3M))
                {
                    _db.AddInParameter(dbCommand, "@p_TP_ESTOQUE_TEC_3M", DbType.String, estoque.TP_ESTOQUE_TEC_3M);
                }

                if (estoque.bidAtivo.HasValue && string.IsNullOrEmpty(estoque.FL_ATIVO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.bidAtivo.Value ? "S" : "N");
                }

                if (!estoque.bidAtivo.HasValue && !string.IsNullOrEmpty(estoque.FL_ATIVO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.FL_ATIVO);
                }

                if (estoque.Cliente.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, estoque.Cliente.CD_CLIENTE);
                }

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

        public DataTable ObterListaUsuario(EstoqueEntity estoque)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                if (estoque.Web == 1)
                {
                    if (estoque.Tecnico == 1)
                    {
                        dbCommand = _db.GetStoredProcCommand("prcEstoqueUsuarioSelect");
                    }
                    else
                    {
                        dbCommand = _db.GetStoredProcCommand("prcEstoqueUsuarioSelectADM");
                    }
                    
                }
                else
                {
                    dbCommand = _db.GetStoredProcCommand("prcEstoqueUsuarioSelectADM");
                }
                

                if (estoque.ID_ESTOQUE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidEstoque", DbType.Int64, estoque.ID_ESTOQUE);
                }

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, estoque.ID_USU_RESPONSAVEL);

                if (!string.IsNullOrEmpty(estoque.FL_ATIVO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, estoque.FL_ATIVO);
                }

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
            //
            //throw new NotImplementedException();
        }

        public IList<EstoqueSinc> ObterListaEstoqueSinc()
        {
            try
            {

                IList<EstoqueSinc> listaEstoque = new List<EstoqueSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SELECT * FROM TBESTOQUE where FL_ATIVO = 'S' ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        //cmd.Parameters.Add("@ID_ENDERECO", System.Data.SqlDbType.BigInt).Value = _IDAdress;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            EstoqueSinc estoque = new EstoqueSinc();
                            estoque.ID_ESTOQUE = Convert.ToInt64(SDR["ID_ESTOQUE"]);
                            estoque.CD_ESTOQUE = Convert.ToString(SDR["CD_ESTOQUE"] is DBNull ? "" : SDR["CD_ESTOQUE"].ToString());
                            estoque.DS_ESTOQUE = Convert.ToString(SDR["DS_ESTOQUE"] is DBNull ? "01/01/2000" : SDR["DS_ESTOQUE"].ToString());
                            estoque.ID_USU_RESPONSAVEL = Convert.ToInt64(SDR["ID_USU_RESPONSAVEL"] is DBNull ? 0 : SDR["ID_USU_RESPONSAVEL"]);
                            estoque.DT_CRIACAO = Convert.ToDateTime(SDR["DT_CRIACAO"] is DBNull ? "01/01/2000" : SDR["DT_CRIACAO"].ToString());
                            estoque.CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"] is DBNull ? 0 : SDR["CD_TECNICO"]);
                            estoque.CD_CLIENTE = Convert.ToString(SDR["CD_CLIENTE"] is DBNull ? 0 : SDR["CD_CLIENTE"]);
                            estoque.TP_ESTOQUE_TEC_3M = Convert.ToString(SDR["TP_ESTOQUE_TEC_3M"] is DBNull ? 0 : SDR["TP_ESTOQUE_TEC_3M"]);
                            estoque.FL_ATIVO = Convert.ToString(SDR["FL_ATIVO"] is DBNull ? "S" : SDR["FL_ATIVO"]);
                            listaEstoque.Add(estoque);
                        }
                        cnx.Close();
                        return listaEstoque;
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

        public DataSet ObterRelatorioEstoqueIntermediario(string CD_PECA, string FL_ATIVO_PECA, string CD_ESTOQUE, string FL_ATIVO_ESTOQUE)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptEstoqueIntermediario");

                if (!string.IsNullOrEmpty(CD_PECA))
                    _db.AddInParameter(dbCommand, "@pCD_PECA", DbType.String, CD_PECA);

                if (!string.IsNullOrEmpty(FL_ATIVO_PECA))
                    _db.AddInParameter(dbCommand, "@pFL_ATIVO_PECA", DbType.String, FL_ATIVO_PECA);

                if (!string.IsNullOrEmpty(CD_ESTOQUE))
                    _db.AddInParameter(dbCommand, "@pCD_ESTOQUE", DbType.String, CD_ESTOQUE);

                if (!string.IsNullOrEmpty(FL_ATIVO_ESTOQUE))
                    _db.AddInParameter(dbCommand, "@pFL_ATIVO_ESTOQUE", DbType.String, FL_ATIVO_ESTOQUE);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                //dataTable = dataSet.Tables[0];
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
            return dataSet;
        }

        public DataSet ObterRelatorioEstoqueIntermediarioPorPeca(string CD_PECA, string FL_ATIVO_PECA, string CD_ESTOQUE, string FL_ATIVO_ESTOQUE)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptEstoqueIntermediarioPorPeca");

                if (!string.IsNullOrEmpty(CD_PECA))
                    _db.AddInParameter(dbCommand, "@pCD_PECA", DbType.String, CD_PECA);

                if (!string.IsNullOrEmpty(FL_ATIVO_PECA))
                    _db.AddInParameter(dbCommand, "@pFL_ATIVO_PECA", DbType.String, FL_ATIVO_PECA);

                if (!string.IsNullOrEmpty(CD_ESTOQUE))
                    _db.AddInParameter(dbCommand, "@pCD_ESTOQUE", DbType.String, CD_ESTOQUE);

                if (!string.IsNullOrEmpty(FL_ATIVO_ESTOQUE))
                    _db.AddInParameter(dbCommand, "@pFL_ATIVO_ESTOQUE", DbType.String, FL_ATIVO_ESTOQUE);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                //dataTable = dataSet.Tables[0];
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
            return dataSet;
        }
    }
}
