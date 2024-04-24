using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class TpAtendimentoReclamacaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public TpAtendimentoReclamacaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IList<TpAtendimentoReclamacaoEntity> ObterLista(TpAtendimentoReclamacaoEntity osPadraoEntity)
        {
            DbConnection connection = null;
            IList<TpAtendimentoReclamacaoEntity> listaTipoAtendimentoReclamacao = new List<TpAtendimentoReclamacaoEntity>();
            IDataReader rdr = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoAtendimentoReclamacaoSelect");

                if (osPadraoEntity.ID_TIPO_ATEND_RECLAMACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_TIPO_ATEND_RECLAMACAO", DbType.String, osPadraoEntity.ID_TIPO_ATEND_RECLAMACAO);

                if (osPadraoEntity.CD_TIPO_ATEND_RECLAMACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_ATEND_RECLAMACAO", DbType.Int32, osPadraoEntity.CD_TIPO_ATEND_RECLAMACAO);

                if (!string.IsNullOrEmpty(osPadraoEntity.DS_TIPO_ATEND_RECLAMACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_TIPO_ATEND_RECLAMACAO", DbType.String, osPadraoEntity.DS_TIPO_ATEND_RECLAMACAO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                rdr = _db.ExecuteReader(dbCommand);
                listaTipoAtendimentoReclamacao = mapTipoAtendimentoReclamacao(rdr);
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

            return listaTipoAtendimentoReclamacao;
        }

        private IList<TpAtendimentoReclamacaoEntity> mapTipoAtendimentoReclamacao(IDataReader rdr)
        {
            IList<TpAtendimentoReclamacaoEntity> listaTipoAtendimentoReclamacao = new List<TpAtendimentoReclamacaoEntity>();

            while (rdr.Read())
            {
                TpAtendimentoReclamacaoEntity tipoAtendimentoReclamacao = new TpAtendimentoReclamacaoEntity();

                tipoAtendimentoReclamacao.ID_TIPO_ATEND_RECLAMACAO = Convert.ToInt64(rdr["ID_TIPO_ATEND_RECLAMACAO"].ToString());
                tipoAtendimentoReclamacao.CD_TIPO_ATEND_RECLAMACAO = Convert.ToInt32(rdr["CD_TIPO_ATEND_RECLAMACAO"] is DBNull ? 0 : rdr["CD_TIPO_ATEND_RECLAMACAO"]);
                tipoAtendimentoReclamacao.DS_TIPO_ATEND_RECLAMACAO = Convert.ToString(rdr["DS_TIPO_ATEND_RECLAMACAO"] is DBNull ? "" : rdr["DS_TIPO_ATEND_RECLAMACAO"]);

                listaTipoAtendimentoReclamacao.Add(tipoAtendimentoReclamacao);
            }

            return listaTipoAtendimentoReclamacao;
        }
    }
}
