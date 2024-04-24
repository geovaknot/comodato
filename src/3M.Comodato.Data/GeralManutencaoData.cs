using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class GeralManutencaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public GeralManutencaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }


        public DataSet ObterRelatorio(string arrayFiltros, DateTime? DT_INICIAL, DateTime? DT_FINAL, string tipoRelatorio)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptGeralManutencao");

                _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.DateTime, DT_INICIAL);
                _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.DateTime, DT_FINAL);

                if (!string.IsNullOrEmpty(arrayFiltros))
                {
                    switch (tipoRelatorio)
                    {
                        case "Cliente":
                            _db.AddInParameter(dbCommand, "@p_CD_CLIENTES", DbType.String, arrayFiltros);
                            break;
                        case "Grupo":
                            _db.AddInParameter(dbCommand, "@p_CD_GRUPOS", DbType.String, arrayFiltros);
                            break;
                        case "Modelo":
                            _db.AddInParameter(dbCommand, "@p_CD_MODELOS", DbType.String, arrayFiltros);
                            break;
                        case "Técnico":
                            _db.AddInParameter(dbCommand, "@p_CD_TECNICOS", DbType.String, arrayFiltros);
                            break;
                        case "Peça":
                            _db.AddInParameter(dbCommand, "@p_CD_PECAS", DbType.String, arrayFiltros);
                            break;
                        case "Equipamento":
                            _db.AddInParameter(dbCommand, "@p_CD_ATIVOS_FIXOS", DbType.String, arrayFiltros);
                            break;
                    }
                }
                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                dataSet = _db.ExecuteDataSet(dbCommand);
                //dataTable = dataSet.Tables[0];
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
