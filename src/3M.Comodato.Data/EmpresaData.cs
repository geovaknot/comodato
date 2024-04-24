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
    public class EmpresaData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public EmpresaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref EmpresaEntity empresa)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEmpresaInsert");

                _db.AddInParameter(dbCommand, "@p_CD_Empresa", DbType.String, empresa.CD_Empresa);
                //_db.AddInParameter(dbCommand, "@p_IIComp", DbType.String, empresa.IIComp);
                _db.AddInParameter(dbCommand, "@p_NM_Empresa", DbType.String, empresa.NM_Empresa);
                _db.AddInParameter(dbCommand, "@p_NR_CNPJ", DbType.String, empresa.NR_Cnpj);
                _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, empresa.EN_Endereco);
                _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, empresa.EN_Bairro);
                _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, empresa.EN_Cidade);
                _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, empresa.EN_Estado);
                _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, empresa.EN_Cep);
                _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, empresa.TX_Telefone);
                _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, empresa.TX_Fax);
                _db.AddInParameter(dbCommand, "@p_NM_CONTATO", DbType.String, empresa.NM_Contato);
                _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, empresa.TX_Email);
                _db.AddInParameter(dbCommand, "@p_FL_TIPO_EMPRESA", DbType.String, empresa.FL_Tipo_Empresa);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, empresa.nidUsuarioAtualizacao);

                //_db.AddOutParameter(dbCommand, "@p_CD_Empresa", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                //empresa.CD_Empresa = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_CD_Empresa"));

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

        public void Excluir(EmpresaEntity empresa)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEmpresaDelete");

                _db.AddInParameter(dbCommand, "@p_CD_Empresa", DbType.Int64, empresa.CD_Empresa);

                if (empresa.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, empresa.nidUsuarioAtualizacao);

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

        public bool Alterar(EmpresaEntity empresa)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEmpresaUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_Empresa", DbType.Int64, empresa.CD_Empresa);

                //if (!string.IsNullOrEmpty(empresa.IIComp))
                    //_db.AddInParameter(dbCommand, "@p_IIComp", DbType.String, empresa.IIComp);

                if (!string.IsNullOrEmpty(empresa.NM_Empresa))
                    _db.AddInParameter(dbCommand, "@p_NM_Empresa", DbType.String, empresa.NM_Empresa);

                if (!string.IsNullOrEmpty(empresa.NR_Cnpj))
                    _db.AddInParameter(dbCommand, "@p_NR_CNPJ", DbType.String, empresa.NR_Cnpj);

                if (!string.IsNullOrEmpty(empresa.EN_Endereco))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, empresa.EN_Endereco);

                if (!string.IsNullOrEmpty(empresa.EN_Bairro))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, empresa.EN_Bairro);

                if (!string.IsNullOrEmpty(empresa.EN_Cidade))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, empresa.EN_Cidade);

                if (!string.IsNullOrEmpty(empresa.EN_Estado))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, empresa.EN_Estado);

                if (!string.IsNullOrEmpty(empresa.EN_Cep))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, empresa.EN_Cep);

                if (!string.IsNullOrEmpty(empresa.TX_Telefone))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, empresa.TX_Telefone);

                if (!string.IsNullOrEmpty(empresa.TX_Fax))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, empresa.TX_Fax);

                if (!string.IsNullOrEmpty(empresa.NM_Contato))
                    _db.AddInParameter(dbCommand, "@p_NM_CONTATO", DbType.String, empresa.NM_Contato);

                if (!string.IsNullOrEmpty(empresa.TX_Email))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, empresa.TX_Email);

                if (!string.IsNullOrEmpty(empresa.FL_Tipo_Empresa))
                    _db.AddInParameter(dbCommand, "@p_FL_TIPO_EMPRESA", DbType.String, empresa.FL_Tipo_Empresa);

                if (empresa.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, empresa.nidUsuarioAtualizacao);

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

        public List<EmpresaEntity> ObterListaEntity(EmpresaEntity empresa)
        {
            var lista = new List<EmpresaEntity>();
            var reader = this.ObterLista(empresa).CreateDataReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    lista.Add(MontarObjeto(reader));
                }
            }

            if (reader != null)
            {
                reader.Dispose();
            }

            return lista;
        }

        public DataTable ObterLista(EmpresaEntity Empresa)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcEmpresaSelect");

                if (Empresa.CD_Empresa != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Empresa", DbType.Int64, Empresa.CD_Empresa);

                //if (!string.IsNullOrEmpty(Empresa.IIComp))
                    //_db.AddInParameter(dbCommand, "@p_IIComp", DbType.String, Empresa.IIComp);

                if (!string.IsNullOrEmpty(Empresa.NM_Empresa))
                    _db.AddInParameter(dbCommand, "@p_NM_Empresa", DbType.String, Empresa.NM_Empresa);

                if (!string.IsNullOrEmpty(Empresa.NR_Cnpj))
                    _db.AddInParameter(dbCommand, "@p_NR_CNPJ", DbType.String, Empresa.NR_Cnpj);

                if (!string.IsNullOrEmpty(Empresa.EN_Endereco))
                    _db.AddInParameter(dbCommand, "@p_EN_ENDERECO", DbType.String, Empresa.EN_Endereco);

                if (!string.IsNullOrEmpty(Empresa.EN_Bairro))
                    _db.AddInParameter(dbCommand, "@p_EN_BAIRRO", DbType.String, Empresa.EN_Bairro);

                if (!string.IsNullOrEmpty(Empresa.EN_Cidade))
                    _db.AddInParameter(dbCommand, "@p_EN_CIDADE", DbType.String, Empresa.EN_Cidade);

                if (!string.IsNullOrEmpty(Empresa.EN_Estado))
                    _db.AddInParameter(dbCommand, "@p_EN_ESTADO", DbType.String, Empresa.EN_Estado);

                if (!string.IsNullOrEmpty(Empresa.EN_Cep))
                    _db.AddInParameter(dbCommand, "@p_EN_CEP", DbType.String, Empresa.EN_Cep);

                if (!string.IsNullOrEmpty(Empresa.TX_Telefone))
                    _db.AddInParameter(dbCommand, "@p_TX_TELEFONE", DbType.String, Empresa.TX_Telefone);

                if (!string.IsNullOrEmpty(Empresa.TX_Fax))
                    _db.AddInParameter(dbCommand, "@p_TX_FAX", DbType.String, Empresa.TX_Fax);

                if (!string.IsNullOrEmpty(Empresa.NM_Contato))
                    _db.AddInParameter(dbCommand, "@p_NM_CONTATO", DbType.String, Empresa.NM_Contato);

                if (!string.IsNullOrEmpty(Empresa.TX_Email))
                    _db.AddInParameter(dbCommand, "@p_TX_EMAIL", DbType.String, Empresa.TX_Email);

                if (!string.IsNullOrEmpty(Empresa.FL_Tipo_Empresa))
                    _db.AddInParameter(dbCommand, "@p_FL_TIPO_EMPRESA", DbType.String, Empresa.FL_Tipo_Empresa);

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

        public EmpresaEntity ObterPorId(long id)
        {
            var lista = ObterListaEntity(new EmpresaEntity() { CD_Empresa = id });
            return lista.FirstOrDefault();
        }

        private EmpresaEntity MontarObjeto(DataTableReader reader)
        {
            var obj = new EmpresaEntity();
            obj.NM_Empresa = reader["NM_EMPRESA"].ToString();
            obj.CD_Empresa = Convert.ToInt64(reader["CD_Empresa"]);
            obj.NR_Cnpj = reader["NR_CNPJ"].ToString();
            obj.EN_Endereco = reader["EN_ENDERECO"].ToString();
            obj.EN_Bairro = reader["EN_BAIRRO"].ToString();
            obj.EN_Cidade = reader["EN_CIDADE"].ToString();
            obj.EN_Estado = reader["EN_ESTADO"].ToString();
            obj.EN_Cep = reader["EN_CEP"].ToString();
            obj.TX_Telefone = reader["TX_TELEFONE"].ToString();
            obj.TX_Fax = reader["TX_FAX"].ToString();
            obj.TX_Email = reader["TX_EMAIL"].ToString();
            obj.NM_Contato = reader["NM_CONTATO"].ToString();
            obj.FL_Tipo_Empresa = reader["FL_TIPO_EMPRESA"].ToString();
            return obj;
        }

    }
}
