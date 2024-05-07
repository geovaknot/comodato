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
    public class TecnicoClienteData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TecnicoClienteData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(TecnicoClienteEntity tecnicoClienteEntity)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteInsert");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, tecnicoClienteEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, tecnicoClienteEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_CD_ORDEM", DbType.Int32, tecnicoClienteEntity.CD_ORDEM);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoClienteEntity.nidUsuarioAtualizacao);

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

        public void Excluir(TecnicoClienteEntity tecnicoClienteEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteDelete");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, tecnicoClienteEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_CD_ORDEM", DbType.Int32, tecnicoClienteEntity.CD_ORDEM);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, tecnicoClienteEntity.tecnico.CD_TECNICO);
                
                if (tecnicoClienteEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoClienteEntity.nidUsuarioAtualizacao);

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

        public void Inativar(TecnicoClienteInativar tecnicoClienteInativar)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteInativar");

                _db.AddInParameter(dbCommand, "@p_ID_ESCALA", DbType.Int64, tecnicoClienteInativar.ID_ESCALA);

                if (tecnicoClienteInativar.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoClienteInativar.nidUsuarioAtualizacao);

                _db.AddInParameter(dbCommand, "@p_TP_ACAO", DbType.String, "I");

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

        public void InativarTodos(string CD_TECNICO, long nidUsuarioAtualizacao)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteInativar");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, CD_TECNICO);

                if (nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);

                _db.AddInParameter(dbCommand, "@p_TP_ACAO", DbType.String, "IT");

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

        public DataTable ObterLista(TecnicoClienteEntity tecnicoClienteEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteSelect");

                if (!string.IsNullOrEmpty(tecnicoClienteEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, tecnicoClienteEntity.tecnico.CD_TECNICO);

                if (tecnicoClienteEntity.cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Cliente", DbType.Int32, tecnicoClienteEntity.cliente.CD_CLIENTE);

                if (tecnicoClienteEntity.CD_ORDEM != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Ordem", DbType.Int32, tecnicoClienteEntity.CD_ORDEM);

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

        public DataTable ObterListaQtdeTecnicos(TecnicoClienteEntity tecnicoClienteEntity, int? nvlQtdeTecnicos = null, Int64? nidUsuario = null)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteSelectQtdeTecnicos");

                if (nidUsuario != null)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int32, nidUsuario);

                if (!string.IsNullOrEmpty(tecnicoClienteEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, tecnicoClienteEntity.tecnico.CD_TECNICO);

                if (tecnicoClienteEntity.cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Cliente", DbType.Int32, tecnicoClienteEntity.cliente.CD_CLIENTE);

                if (nvlQtdeTecnicos != null)
                    _db.AddInParameter(dbCommand, "@p_nvlQtdeTecnicos", DbType.Int32, nvlQtdeTecnicos);

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

        public DataTable ObterListaEscala(TecnicoClienteEntity tecnicoClienteEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteSelectEscala");

                //if (tecnicoClienteEntity.cliente.CD_CLIENTE != 0)
                _db.AddInParameter(dbCommand, "@p_CD_Cliente", DbType.Int32, tecnicoClienteEntity.cliente.CD_CLIENTE);

                //if (!string.IsNullOrEmpty(tecnicoClienteEntity.tecnico.CD_TECNICO))
                //    _db.AddInParameter(dbCommand, "@p_CD_Tecnico", DbType.String, tecnicoClienteEntity.tecnico.CD_TECNICO);

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

        public void Reordenar(TecnicoClienteEntity tecnicoClienteEntity, string TP_ACAO)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteReordenar");

                _db.AddInParameter(dbCommand, "@p_TP_ACAO", DbType.String, TP_ACAO);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, tecnicoClienteEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_CD_ORDEM", DbType.Int32, tecnicoClienteEntity.CD_ORDEM);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tecnicoClienteEntity.nidUsuarioAtualizacao);

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

        public void ReordenarTodos(string idTecnico, long nidUsuarioAtualizacao)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTecnicoClienteReordenar");

                _db.AddInParameter(dbCommand, "@p_TP_ACAO", DbType.String, "RT");
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, 0);
                _db.AddInParameter(dbCommand, "@p_CD_ORDEM", DbType.Int32, 0);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, idTecnico);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);

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

        public IList<TecnicoClienteSinc> ObterListaTecnicoClienteSinc(long idUsuario)
        {
            try
            {
                IList<TecnicoClienteSinc> listaTecnicoCliente = new List<TecnicoClienteSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select tc.* " +
                                         " from tb_tecnico_cliente tc " +
                                         " inner join tb_tecnico t on t.cd_tecnico = tc.cd_tecnico " +
                                         " inner join TB_CLIENTE c on c.CD_CLIENTE = tc.cd_cliente " +
                                         " where c.dt_desativacao is null " +
                                         " and(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            TecnicoClienteSinc tecnicoCliente = new TecnicoClienteSinc();
                            tecnicoCliente.CD_CLIENTE = Convert.ToInt32(SDR["CD_CLIENTE"].ToString());
                            tecnicoCliente.CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"].ToString());
                            tecnicoCliente.CD_ORDEM   = Convert.ToInt32(SDR["CD_ORDEM"].ToString());
                            listaTecnicoCliente.Add(tecnicoCliente);
                        }
                        cnx.Close();
                        return listaTecnicoCliente;
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
