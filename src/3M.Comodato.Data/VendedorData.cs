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
    public class VendedorData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public VendedorData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref VendedorEntity vendedorEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcVendedorInsert");

                if (!string.IsNullOrEmpty(vendedorEntity.NM_VENDEDOR))
                    _db.AddInParameter(dbCommand, "@p_NM_VENDEDOR", DbType.String, vendedorEntity.NM_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.NM_APE_VENDEDOR))
                    _db.AddInParameter(dbCommand, "@p_NM_APE_VENDEDOR", DbType.String, vendedorEntity.NM_APE_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_ENDERECO))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, vendedorEntity.EN_ENDERECO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_BAIRRO))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, vendedorEntity.EN_BAIRRO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CIDADE))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, vendedorEntity.EN_CIDADE);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_ESTADO))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, vendedorEntity.EN_ESTADO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CEP))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, vendedorEntity.EN_CEP);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CX_POSTAL))
                    _db.AddInParameter(dbCommand, "@p_EN_CX_POSTAL", DbType.String, vendedorEntity.EN_CX_POSTAL);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_TELEFONE))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, vendedorEntity.TX_TELEFONE);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_FAX))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, vendedorEntity.TX_FAX);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_EMAIL))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, vendedorEntity.TX_EMAIL);

                if (!string.IsNullOrEmpty(vendedorEntity.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, vendedorEntity.FL_ATIVO);

                if (vendedorEntity.ID_USUARIO_REGIONAL > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_REGIONAL", DbType.Int64, vendedorEntity.ID_USUARIO_REGIONAL);

                if (vendedorEntity.ID_USUARIO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, vendedorEntity.ID_USUARIO);

                if (vendedorEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, vendedorEntity.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, 6);

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

        public void Excluir(VendedorEntity vendedorEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcVendedorDelete");

                _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, vendedorEntity.CD_VENDEDOR);

                if (vendedorEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, vendedorEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(VendedorEntity vendedorEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVendedorUpdate");


                _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, vendedorEntity.CD_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.NM_VENDEDOR))
                    _db.AddInParameter(dbCommand, "@p_NM_VENDEDOR", DbType.String, vendedorEntity.NM_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.NM_APE_VENDEDOR))
                    _db.AddInParameter(dbCommand, "@p_NM_APE_VENDEDOR", DbType.String, vendedorEntity.NM_APE_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_ENDERECO))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, vendedorEntity.EN_ENDERECO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_BAIRRO))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, vendedorEntity.EN_BAIRRO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CIDADE))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, vendedorEntity.EN_CIDADE);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_ESTADO))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, vendedorEntity.EN_ESTADO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CEP))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, vendedorEntity.EN_CEP);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CX_POSTAL))
                    _db.AddInParameter(dbCommand, "@p_EN_CX_POSTAL", DbType.String, vendedorEntity.EN_CX_POSTAL);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_TELEFONE))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, vendedorEntity.TX_TELEFONE);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_FAX))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, vendedorEntity.TX_FAX);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_EMAIL))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, vendedorEntity.TX_EMAIL);

                if (!string.IsNullOrEmpty(vendedorEntity.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, vendedorEntity.FL_ATIVO);

                if (vendedorEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, vendedorEntity.nidUsuarioAtualizacao);

                if (vendedorEntity.usuarioGerenteRegional.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_REGIONAL", DbType.Int64, vendedorEntity.usuarioGerenteRegional.nidUsuario);

                if (vendedorEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, vendedorEntity.usuario.nidUsuario);


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

        public DataTable ObterLista(VendedorEntity vendedorEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcVendedorSelect");

                if (vendedorEntity.CD_VENDEDOR != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Int64, vendedorEntity.CD_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.NM_VENDEDOR))
                    _db.AddInParameter(dbCommand, "@p_NM_VENDEDOR", DbType.String, vendedorEntity.NM_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.NM_APE_VENDEDOR))
                    _db.AddInParameter(dbCommand, "@p_NM_APE_VENDEDOR", DbType.String, vendedorEntity.NM_APE_VENDEDOR);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_ENDERECO))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, vendedorEntity.EN_ENDERECO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_BAIRRO))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, vendedorEntity.EN_BAIRRO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CIDADE))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, vendedorEntity.EN_CIDADE);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_ESTADO))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, vendedorEntity.EN_ESTADO);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CEP))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, vendedorEntity.EN_CEP);

                if (!string.IsNullOrEmpty(vendedorEntity.EN_CX_POSTAL))
                    _db.AddInParameter(dbCommand, "@p_EN_CX_POSTAL", DbType.String, vendedorEntity.EN_CX_POSTAL);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_TELEFONE))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, vendedorEntity.TX_TELEFONE);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_FAX))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, vendedorEntity.TX_FAX);

                if (!string.IsNullOrEmpty(vendedorEntity.TX_EMAIL))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, vendedorEntity.TX_EMAIL);

                //SL00034720
                if (!string.IsNullOrEmpty(vendedorEntity.FL_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_FL_ATIVO", DbType.String, vendedorEntity.FL_ATIVO);

                if (vendedorEntity.usuarioGerenteRegional.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_REGIONAL", DbType.Int64, vendedorEntity.usuarioGerenteRegional.nidUsuario);

                if (vendedorEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, vendedorEntity.usuario.nidUsuario);

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
