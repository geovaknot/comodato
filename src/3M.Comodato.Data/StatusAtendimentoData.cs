using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class StatusAtendimentoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public StatusAtendimentoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }


        public IEnumerable<StatusAtendimentoEntity> ObterLista()
        {
            return this.ObterLista(null);
        }
        public IEnumerable<StatusAtendimentoEntity> ObterLista(StatusAtendimentoEntity statusAtendimento)
        {
            Func<DataRow, StatusAtendimentoEntity> statusAtendimentoConverter = new Func<DataRow, StatusAtendimentoEntity>((row) => {
                StatusAtendimentoEntity entity = new StatusAtendimentoEntity();
                entity.ID_STATUS_ATENDIMENTO = Convert.ToInt32(row["ID_STATUS_ATENDIMENTO"]);
                entity.DS_STATUS_ATENDIMENTO = row["DS_STATUS_ATENDIMENTO"].ToString();

                return entity;
            });

            DbConnection connection = null;
            List<StatusAtendimentoEntity> listaStatusAtendimento = new List<StatusAtendimentoEntity>();
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusAtendimentoSelect");
                if (null != statusAtendimento)
                {
                    if (statusAtendimento.ID_STATUS_ATENDIMENTO != 0)
                        _db.AddInParameter(dbCommand, "@p_ID_TIPO_ATENDIMENTO", DbType.Int32, statusAtendimento.ID_STATUS_ATENDIMENTO);

                    if (!string.IsNullOrEmpty(statusAtendimento.DS_STATUS_ATENDIMENTO))
                        _db.AddInParameter(dbCommand, "@p_DS_STATUS_ATENDIMENTO", DbType.String, statusAtendimento.DS_STATUS_ATENDIMENTO);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                DataTable dataTable = dataSet.Tables[0];

                listaStatusAtendimento = (from r in dataTable.Rows.Cast<DataRow>()
                                        select statusAtendimentoConverter(r)).ToList();
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
            return listaStatusAtendimento;
        }
    }
}
