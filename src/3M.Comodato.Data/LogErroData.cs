using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class LogErroData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LogErroData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }
         
        public void GravarLogErro(LogErroEntity logErroEntity)
        {

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcLogErroInsert");

                _db.AddInParameter(dbCommand, "@p_DataHoraErro", DbType.DateTime, logErroEntity.Data);
                _db.AddInParameter(dbCommand, "@p_Servidor", DbType.String, logErroEntity.Servidor);
                _db.AddInParameter(dbCommand, "@p_Sistema", DbType.String, "3M.Comodato");
                _db.AddInParameter(dbCommand, "@p_Cliente", DbType.String, logErroEntity.Cliente);
                _db.AddInParameter(dbCommand, "@p_Usuario", DbType.String, logErroEntity.Usuario);
                _db.AddInParameter(dbCommand, "@p_Origem", DbType.String, logErroEntity.Local);
                _db.AddInParameter(dbCommand, "@p_Classe", DbType.String, logErroEntity.Classe);
                _db.AddInParameter(dbCommand, "@p_Metodo", DbType.String, logErroEntity.Metodo);
                _db.AddInParameter(dbCommand, "@p_URL", DbType.String, logErroEntity.Url);
                _db.AddInParameter(dbCommand, "@p_Parametros", DbType.String, logErroEntity.Parameters);
                _db.AddInParameter(dbCommand, "@p_Message", DbType.String, logErroEntity.ErrorMessage);
                _db.AddInParameter(dbCommand, "@p_StackTrace", DbType.String, logErroEntity.StackTrace);
                _db.AddInParameter(dbCommand, "@p_Linha", DbType.String, logErroEntity.Linha);
                _db.AddInParameter(dbCommand, "@p_InnerMessage", DbType.String, logErroEntity.InnerMessage);
                _db.AddInParameter(dbCommand, "@p_InnerStackTrace", DbType.String, logErroEntity.InnerStackTrace);
                _db.AddInParameter(dbCommand, "@p_Browser", DbType.String, logErroEntity.Browser);
                _db.AddInParameter(dbCommand, "@p_IP", DbType.String, logErroEntity.IP);
                _db.AddInParameter(dbCommand, "@p_Nivel", DbType.String, logErroEntity.Tipo);

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

    }
}
