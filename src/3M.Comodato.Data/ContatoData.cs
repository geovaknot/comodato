using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class ContatoData
    {

        readonly Database _db;
        DbCommand dbCommand;

        public ContatoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref ContatoEntity Contato)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcContatoInsert");

                if (!string.IsNullOrEmpty(Contato.cnmContato))
                    _db.AddInParameter(dbCommand, "@p_cnmContato", DbType.String, Contato.cnmContato);

                if (!string.IsNullOrEmpty(Contato.cnmApelido))
                    _db.AddInParameter(dbCommand, "@p_cnmApelido", DbType.String, Contato.cnmApelido);

                if (Contato.tipoContato.nidTipoContato > 0)
                    _db.AddInParameter(dbCommand, "@p_nidTipoContato", DbType.String, Contato.tipoContato.nidTipoContato);

                if (!string.IsNullOrEmpty(Contato.cdsEmail))
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, Contato.cdsEmail);

                //if (Contato.empresa.CD_Empresa > 0)
                    _db.AddInParameter(dbCommand, "@p_nidEmpresa", DbType.Decimal, Contato.empresa.CD_Empresa);
                //else
                //    _db.AddInParameter(dbCommand, "@p_nidEmpresa", DbType.Decimal, 1);

                if (!string.IsNullOrEmpty(Contato.cdsDDDTelefoneCel))
                    _db.AddInParameter(dbCommand, "@p_cdsDDDTelefoneCel", DbType.String, Contato.cdsDDDTelefoneCel);

                if (!string.IsNullOrEmpty(Contato.cdsTelefoneCel))
                    _db.AddInParameter(dbCommand, "@p_cdsTelefoneCel", DbType.String, Contato.cdsTelefoneCel);

                if (!string.IsNullOrEmpty(Contato.cdsDDDTelefone2))
                    _db.AddInParameter(dbCommand, "@p_cdsDDDTelefone2", DbType.String, Contato.cdsDDDTelefone2);

                if (!string.IsNullOrEmpty(Contato.cdsTelefone2))
                    _db.AddInParameter(dbCommand, "@p_cdsTelefone2", DbType.String, Contato.cdsTelefone2);

                if (!string.IsNullOrEmpty(Contato.cdsObservacoes))
                    _db.AddInParameter(dbCommand, "@p_cdsObservacoes", DbType.String, Contato.cdsObservacoes);

                if (!string.IsNullOrEmpty(Contato.cdsCargo))
                    _db.AddInParameter(dbCommand, "@p_cdsCargo", DbType.String, Contato.cdsCargo);

                //_db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, Contato.bidAtivo);

                //if (Contato.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, Contato.cliente.CD_CLIENTE);

                if (Contato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Contato.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_nidContato", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                Contato.nidContato = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidContato"));

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

        public void Excluir(ContatoEntity Contato)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcContatoDelete");

                _db.AddInParameter(dbCommand, "@p_nidContato", DbType.Int64, Contato.nidContato);

                if (Contato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Contato.nidUsuarioAtualizacao);

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

        public bool Alterar(ContatoEntity Contato)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcContatoUpdate");

                _db.AddInParameter(dbCommand, "@p_nidContato", DbType.Int64, Contato.nidContato);

                if (!string.IsNullOrEmpty(Contato.cnmApelido))
                    _db.AddInParameter(dbCommand, "@p_cnmApelido", DbType.String, Contato.cnmApelido);

                if (Contato.tipoContato.nidTipoContato != 0)
                    _db.AddInParameter(dbCommand, "@p_nidTipoContato", DbType.String, Contato.tipoContato.nidTipoContato);

                if (!string.IsNullOrEmpty(Contato.cdsEmail))
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, Contato.cdsEmail);

                //if (Contato.empresa.CD_Empresa != 0)
                    _db.AddInParameter(dbCommand, "@p_nidEmpresa", DbType.String, Contato.empresa.CD_Empresa);

                if (!string.IsNullOrEmpty(Contato.cdsDDDTelefoneCel))
                    _db.AddInParameter(dbCommand, "@p_cdsDDDTelefoneCel", DbType.String, Contato.cdsDDDTelefoneCel);

                if (!string.IsNullOrEmpty(Contato.cdsTelefoneCel))
                    _db.AddInParameter(dbCommand, "@p_cdsTelefoneCel", DbType.String, Contato.cdsTelefoneCel);

                if (!string.IsNullOrEmpty(Contato.cdsDDDTelefone2))
                    _db.AddInParameter(dbCommand, "@p_cdsDDDTelefone2", DbType.String, Contato.cdsDDDTelefone2);

                if (!string.IsNullOrEmpty(Contato.cdsTelefone2))
                    _db.AddInParameter(dbCommand, "@p_cdsTelefone2", DbType.String, Contato.cdsTelefone2);

                if (!string.IsNullOrEmpty(Contato.cdsObservacoes))
                    _db.AddInParameter(dbCommand, "@p_cdsObservacoes", DbType.String, Contato.cdsObservacoes);

                if (!string.IsNullOrEmpty(Contato.cdsCargo))
                    _db.AddInParameter(dbCommand, "@p_cdsCargo", DbType.String, Contato.cdsCargo);

                //if (Contato.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, Contato.cliente.CD_CLIENTE);

                if (Contato.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, Contato.bidAtivo);

                if (Contato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Contato.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(ContatoEntity Contato)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcContatoSelect");

                if (Contato.nidContato != 0)
                    _db.AddInParameter(dbCommand, "@p_nidContato", DbType.Int64, Contato.nidContato);

                if (!string.IsNullOrEmpty(Contato.cnmApelido))
                    _db.AddInParameter(dbCommand, "@p_cnmApelido", DbType.String, Contato.cnmApelido);

                if (Contato.tipoContato.nidTipoContato != 0)
                    _db.AddInParameter(dbCommand, "@p_nidTipoContato", DbType.String, Contato.tipoContato.nidTipoContato);

                if (!string.IsNullOrEmpty(Contato.cdsEmail))
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, Contato.cdsEmail);

                if (Contato.empresa.CD_Empresa != 0)
                    _db.AddInParameter(dbCommand, "@p_nidEmpresa", DbType.String, Contato.empresa.CD_Empresa);

                if (!string.IsNullOrEmpty(Contato.cdsDDDTelefoneCel))
                    _db.AddInParameter(dbCommand, "@p_cdsDDDTelefoneCel", DbType.String, Contato.cdsDDDTelefoneCel);

                if (!string.IsNullOrEmpty(Contato.cdsTelefoneCel))
                    _db.AddInParameter(dbCommand, "@p_cdsTelefoneCel", DbType.String, Contato.cdsTelefoneCel);

                if (!string.IsNullOrEmpty(Contato.cdsDDDTelefone2))
                    _db.AddInParameter(dbCommand, "@p_cdsDDDTelefone2", DbType.String, Contato.cdsDDDTelefone2);

                if (!string.IsNullOrEmpty(Contato.cdsTelefone2))
                    _db.AddInParameter(dbCommand, "@p_cdsTelefone2", DbType.String, Contato.cdsTelefone2);

                if (!string.IsNullOrEmpty(Contato.cdsObservacoes))
                    _db.AddInParameter(dbCommand, "@p_cdsObservacoes", DbType.String, Contato.cdsObservacoes);

                if (!string.IsNullOrEmpty(Contato.cdsCargo))
                    _db.AddInParameter(dbCommand, "@p_cdsCargo", DbType.String, Contato.cdsCargo);

                if (Contato.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, Contato.cliente.CD_CLIENTE);

                if (Contato.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, Contato.bidAtivo);

                if (Contato.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, Contato.nidUsuarioAtualizacao);

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
