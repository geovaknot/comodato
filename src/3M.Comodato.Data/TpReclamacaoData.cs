using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class TpReclamacaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TpReclamacaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IList<TpReclamacaoEntity> ObterLista(TpReclamacaoEntity osPadraoEntity)
        {
            DbConnection connection = null;
            IList<TpReclamacaoEntity> listaTipoReclamacao = new List<TpReclamacaoEntity>();
            IDataReader rdr = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoOSPadraoSelect");

                if (osPadraoEntity.ID_TIPO_RECLAMACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_TIPO_RECLAMACAO", DbType.String, osPadraoEntity.ID_TIPO_RECLAMACAO);

                if (osPadraoEntity.CD_TIPO_RECLAMACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_RECLAMACAO", DbType.Int32, osPadraoEntity.CD_TIPO_RECLAMACAO);

                if (!string.IsNullOrEmpty(osPadraoEntity.DS_TIPO_RECLAMACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_TIPO_RECLAMACAO", DbType.String, osPadraoEntity.DS_TIPO_RECLAMACAO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                rdr = _db.ExecuteReader(dbCommand);
                listaTipoReclamacao = mapTipoReclamacao(rdr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (rdr != null && !rdr.IsClosed)
                    rdr.Close();

                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return listaTipoReclamacao;
        }

        private IList<TpReclamacaoEntity> mapTipoReclamacao(IDataReader rdr)
        {
            IList<TpReclamacaoEntity> listaTipoReclamacao = new List<TpReclamacaoEntity>();

            while (rdr.Read())
            {
                TpReclamacaoEntity tipoReclamacao = new TpReclamacaoEntity();

                tipoReclamacao.ID_TIPO_RECLAMACAO = Convert.ToInt64(rdr["ID_TIPO_RECLAMACAO"].ToString());
                tipoReclamacao.CD_TIPO_RECLAMACAO = Convert.ToInt32(rdr["CD_TIPO_RECLAMACAO"] is DBNull ? 0 : rdr["CD_TIPO_RECLAMACAO"]);
                tipoReclamacao.DS_TIPO_RECLAMACAO = Convert.ToString(rdr["DS_TIPO_RECLAMACAO"] is DBNull ? "" : rdr["DS_TIPO_RECLAMACAO"]);

                listaTipoReclamacao.Add(tipoReclamacao);
            }

            return listaTipoReclamacao;
        }
    }
}
