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
    public class UsuarioClienteData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public UsuarioClienteData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref UsuarioClienteEntity usuarioClienteEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcUsuarioClienteInsert");

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioClienteEntity.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, usuarioClienteEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioClienteEntity.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_nidUsuarioCliente", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                usuarioClienteEntity.nidUsuarioCliente = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidUsuarioCliente"));

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

        public void Excluir(UsuarioClienteEntity usuarioClienteEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcUsuarioClienteDelete");

                _db.AddInParameter(dbCommand, "@p_nidUsuarioCliente", DbType.Int64, usuarioClienteEntity.nidUsuarioCliente);

                if (usuarioClienteEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioClienteEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(UsuarioClienteEntity usuarioClienteEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioClienteUpdate");

                _db.AddInParameter(dbCommand, "@p_nidUsuarioCliente", DbType.Int64, usuarioClienteEntity.nidUsuarioCliente);
                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioClienteEntity.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, usuarioClienteEntity.cliente.CD_CLIENTE);

                if (usuarioClienteEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioClienteEntity.nidUsuarioAtualizacao);


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

        public DataTable ObterLista(UsuarioClienteEntity usuarioClienteEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioClienteSelect");

                if (usuarioClienteEntity.nidUsuarioCliente > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioCliente", DbType.Int64, usuarioClienteEntity.nidUsuarioCliente);

                if (usuarioClienteEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioClienteEntity.usuario.nidUsuario);

                if (usuarioClienteEntity.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, usuarioClienteEntity.cliente.CD_CLIENTE);

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

        public DataTable ObterListaQtdeUsuarios(UsuarioClienteEntity usuarioClienteEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioClienteSelectQtdeUsuarios");

                if (usuarioClienteEntity.nidUsuarioCliente != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioCliente", DbType.Int64, usuarioClienteEntity.nidUsuarioCliente);

                if (usuarioClienteEntity.usuario.nidUsuario != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioClienteEntity.usuario.nidUsuario);

                if (usuarioClienteEntity.cliente.CD_CLIENTE != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Cliente", DbType.Int64, usuarioClienteEntity.cliente.CD_CLIENTE);


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
