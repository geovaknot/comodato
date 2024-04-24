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
    public class TipoContatoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TipoContatoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(TipoContatoEntity TipoContato)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoContatoSelect");

                if (TipoContato.nidTipoContato != 0)
                    _db.AddInParameter(dbCommand, "@p_nidTipoContato", DbType.Int64, TipoContato.nidTipoContato);

                if (!string.IsNullOrEmpty(TipoContato.cdsTipoContato))
                    _db.AddInParameter(dbCommand, "@p_cdsTipoContato", DbType.String, TipoContato.cdsTipoContato);

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
