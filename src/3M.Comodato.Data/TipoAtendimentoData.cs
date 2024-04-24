using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class TipoAtendimentoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TipoAtendimentoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }


        public IEnumerable<TipoAtendimentoEntity> ObterLista()
        {
            return this.ObterLista(null);
        }
        public IEnumerable<TipoAtendimentoEntity> ObterLista(TipoAtendimentoEntity tipoAtendimento)
        {
            Func<DataRow, TipoAtendimentoEntity> tipoAtendimentoConverter = new Func<DataRow, TipoAtendimentoEntity>((row) => {
                TipoAtendimentoEntity entity = new TipoAtendimentoEntity();
                entity.ID_TIPO_ATENDIMENTO = Convert.ToInt32(row["ID_TIPO_ATENDIMENTO"]);
                entity.CD_TIPO_ATENDIMENTO = row["CD_TIPO_ATENDIMENTO"].ToString();
                entity.DS_TIPO_ATENDIMENTO = row["DS_TIPO_ATENDIMENTO"].ToString();

                return entity;
            });

            DbConnection connection = null;
            List<TipoAtendimentoEntity> listaTipoAtendimento = new List<TipoAtendimentoEntity>();
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoAtendimentoSelect");
                if (null != tipoAtendimento)
                {
                    if (tipoAtendimento.ID_TIPO_ATENDIMENTO != 0)
                        _db.AddInParameter(dbCommand, "@p_ID_TIPO_ATENDIMENTO", DbType.Int32, tipoAtendimento.ID_TIPO_ATENDIMENTO);

                    if (!string.IsNullOrEmpty(tipoAtendimento.CD_TIPO_ATENDIMENTO))
                        _db.AddInParameter(dbCommand, "@p_CD_TIPO_ATENDIMENTO", DbType.String, tipoAtendimento.CD_TIPO_ATENDIMENTO);

                    if (!string.IsNullOrEmpty(tipoAtendimento.DS_TIPO_ATENDIMENTO))
                        _db.AddInParameter(dbCommand, "@p_DS_TIPO_ATENDIMENTO", DbType.String, tipoAtendimento.DS_TIPO_ATENDIMENTO);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet =  _db.ExecuteDataSet(dbCommand);
                DataTable dataTable = dataSet.Tables[0];

                listaTipoAtendimento = (from r in dataTable.Rows.Cast<DataRow>()
                                       select tipoAtendimentoConverter(r)).ToList();
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
            return listaTipoAtendimento;
        }

    }
}
