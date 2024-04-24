using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class MonitoramentoWFData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public MonitoramentoWFData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(MonitoramentoWFEntity monitoramento)
        {
            if (monitoramento.DT_INICIAL is null)
                monitoramento.DT_INICIAL = DateTime.Now.AddMonths(-1);
            if (monitoramento.DT_FINAL is null)
                monitoramento.DT_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcMonitoramentoWFSelect");

                if (monitoramento.DT_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, monitoramento.DT_INICIAL);

                if (monitoramento.DT_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, monitoramento.DT_FINAL);

                if (monitoramento.TIPO != "")
                    _db.AddInParameter(dbCommand, "@p_TIPO", DbType.String, monitoramento.TIPO);


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

        public List<MonitoramentoWFEntity> ObterListaEntity(MonitoramentoWFEntity monitoramento)
        {

            Func<DataRow, MonitoramentoWFEntity> Converter = new Func<DataRow, MonitoramentoWFEntity>((r) =>
            {
                return new MonitoramentoWFEntity()
                {
                    GRUPO = r["CD_GRUPO"]?.ToString(),
                    QT_FLUXOS = Convert.ToInt32(r["QTD_ATEND"]),
                    RESPONSAVEL = r["NomeUsuario"].ToString(),
                    STATUS = r["DS_STATUS_NOME_REDUZ"].ToString(),
                    TEMPO_MEDIO = Convert.ToDecimal(r["TMA"]),
                    TIPO = r["TP_PEDIDO"].ToString()
                };
            });

            DataTable dataTable = ObterLista(monitoramento);
            return dataTable.Rows.Cast<DataRow>().Select(a=> Converter(a)).OrderBy(o => o.STATUS).ToList();
        }
    }
}