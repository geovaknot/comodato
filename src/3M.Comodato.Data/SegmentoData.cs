using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class SegmentoData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public SegmentoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref SegmentoEntity segmento)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcSegmentoInsert");

                _db.AddInParameter(dbCommand, "@p_ds_segmentomin", DbType.String, segmento.DS_SEGMENTO_MIN);
                _db.AddInParameter(dbCommand, "@p_ds_segmento", DbType.String, segmento.DS_SEGMENTO);
                _db.AddInParameter(dbCommand, "@p_nm_criticidade", DbType.Int32, segmento.NM_CRITICIDADE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, segmento.nidUsuarioAtualizacao);

                if (!string.IsNullOrEmpty(segmento.DS_DESCRICAO))
                {
                    _db.AddInParameter(dbCommand, "@p_ds_descricao", DbType.String, segmento.DS_DESCRICAO);
                }

                _db.AddOutParameter(dbCommand, "@p_id_segmento", DbType.Int64, 18);
                _db.ExecuteNonQuery(dbCommand);

                segmento.ID_SEGMENTO = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_id_segmento"));

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

        public void Excluir(SegmentoEntity segmento)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcSegmentoDelete");

                _db.AddInParameter(dbCommand, "@p_id_segmento", DbType.Int64, segmento.ID_SEGMENTO);

                if (segmento.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, segmento.nidUsuarioAtualizacao);
                }

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

        public bool Alterar(SegmentoEntity segmento)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSegmentoUpdate");

                _db.AddInParameter(dbCommand, "@p_id_segmento", DbType.Int64, segmento.ID_SEGMENTO);
                _db.AddInParameter(dbCommand, "@p_ds_segmentomin", DbType.String, segmento.DS_SEGMENTO_MIN);
                _db.AddInParameter(dbCommand, "@p_ds_segmento", DbType.String, segmento.DS_SEGMENTO);
                _db.AddInParameter(dbCommand, "@p_nm_criticidade", DbType.Int32, segmento.NM_CRITICIDADE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, segmento.nidUsuarioAtualizacao);

                if (!string.IsNullOrEmpty(segmento.DS_DESCRICAO))
                {
                    _db.AddInParameter(dbCommand, "@p_ds_descricao", DbType.String, segmento.DS_DESCRICAO);
                }

                _db.ExecuteNonQuery(dbCommand);
                blnOK = true;
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

        public IEnumerable<SegmentoEntity> ObterLista(SegmentoEntity segmento)
        {
            List<SegmentoEntity> listaSegmento = new List<SegmentoEntity>();
            DbConnection connection = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSegmentoSelect");

                if (segmento.ID_SEGMENTO != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_id_segmento", DbType.Int64, segmento.ID_SEGMENTO);
                }
                if (!string.IsNullOrEmpty(segmento.DS_SEGMENTO_MIN))
                {
                    _db.AddInParameter(dbCommand, "@p_ds_segmentomin", DbType.String, segmento.DS_SEGMENTO_MIN);
                }
                if (!string.IsNullOrEmpty(segmento.DS_SEGMENTO))
                {
                    _db.AddInParameter(dbCommand, "@p_ds_segmento", DbType.String, segmento.DS_SEGMENTO);
                }
                if (segmento.NM_CRITICIDADE != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nm_criticidade", DbType.Int32, segmento.NM_CRITICIDADE);
                }
                if (!string.IsNullOrEmpty(segmento.DS_DESCRICAO))
                {
                    _db.AddInParameter(dbCommand, "@p_ds_descricao", DbType.String, segmento.DS_DESCRICAO);
                }

                if (segmento.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, segmento.nidUsuarioAtualizacao);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                IDataReader reader = _db.ExecuteReader(dbCommand);
                while (reader.Read())
                {
                    SegmentoEntity segmentoEntity = new SegmentoEntity();
                    segmentoEntity.ID_SEGMENTO = Convert.ToInt64(reader["ID_SEGMENTO"]);
                    segmentoEntity.DS_SEGMENTO_MIN = Convert.ToString(reader["ds_segmentomin"]);
                    segmentoEntity.DS_SEGMENTO = Convert.ToString(reader["ds_segmento"]);
                    segmentoEntity.NM_CRITICIDADE = Convert.ToInt32(reader["nm_criticidade"]);

                    if (reader["ds_descricao"] != DBNull.Value)
                    {
                        segmentoEntity.DS_DESCRICAO = Convert.ToString(reader["ds_descricao"]);
                    }

                    listaSegmento.Add(segmentoEntity);
                }

            }
            catch (SqlException ex)
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
            return listaSegmento;
        }

        public void AtualizarSegmento(long cd_cliente, long id_segmento)
        {
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAtualizarClienteSegmento");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, cd_cliente);

                if (id_segmento > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_id_segmento", DbType.String, id_segmento);
                }

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
