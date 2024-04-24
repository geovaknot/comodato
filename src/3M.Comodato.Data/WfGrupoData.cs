using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class WfGrupoData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public WfGrupoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista()
        {
            return ObterLista(null);
        }

        public DataTable ObterLista(WfGrupoEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWFGrupoSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_GRUPOWF != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_GRUPOWF", DbType.Int32, entity.ID_GRUPOWF);
                    }

                    if (!string.IsNullOrEmpty(entity.CD_GRUPOWF))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_GRUPOWF", DbType.String, entity.CD_GRUPOWF);
                    }

                    if (!string.IsNullOrEmpty(entity.DS_GRUPOWF))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_GRUPOWF", DbType.String, entity.DS_GRUPOWF);
                    }

                    if (!string.IsNullOrEmpty(entity.TP_GRUPOWF))
                    {
                        _db.AddInParameter(dbCommand, "@p_TP_GRUPOWF", DbType.String, entity.TP_GRUPOWF);
                    }
                    #endregion
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
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

        public IEnumerable<WfGrupoEntity> ObterListaEntity()
        {
            return ObterListaEntity(null);
        }

        public IEnumerable<WfGrupoEntity> ObterListaEntity(WfGrupoEntity entity)
        {
            DataTable dtWFGrupo = ObterLista(entity);
            return (from r in dtWFGrupo.Rows.Cast<DataRow>()
                    select new WfGrupoEntity()
                    {
                        ID_GRUPOWF = Convert.ToInt32(r["ID_GRUPOWF"]),
                        CD_GRUPOWF = r["CD_GRUPOWF"].ToString(),
                        DS_GRUPOWF = r["DS_GRUPOWF"].ToString(),
                        TP_GRUPOWF = r["TP_GRUPOWF"].ToString()
                    }
                   ).ToList();
        }

        public DataTable ObterListaByStatusPedido(int ST_STATUS_PEDIDO)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWFGrupoByStatusPedidoSelect");
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_PEDIDO", DbType.Int32, ST_STATUS_PEDIDO);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
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

        public List<WfGrupoEntity> ObterGruposResponsaveisPorPedido(WfPedidoEquipEntity pedido)
        {
            List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();

            long CATEGORIA = pedido.ID_CATEGORIA;
            string TIPOLOCACAO = pedido.TP_LOCACAO;
            string LINHA = pedido.CD_LINHA;
            string MODELO = pedido.DS_MODELO;

            if (TIPOLOCACAO == null)
                TIPOLOCACAO = "";
            if (LINHA == null)
                LINHA = "";
            if (MODELO == null)
                MODELO = "";

            bool ARMADORA = MODELO.ToUpper().Contains("ARMADORA");

            var lista = this.ObterListaByStatusPedido(pedido.ST_STATUS_PEDIDO).Rows.Cast<DataRow>().Select(r => MontarObjeto(r)).ToList();

            foreach (var grupoEntity in lista)
            {
                string CodigoGrupo = grupoEntity.CD_GRUPOWF.ToString().ToUpper().Trim();

                //MKT
                if (TIPOLOCACAO == "A" && CodigoGrupo == "MKT3" && ARMADORA == false) // Aprova se for Aluguel
                    listaGrupos.Add(grupoEntity);
                else if ((CATEGORIA == 2 || CATEGORIA == 6) && CodigoGrupo == "MKT1") // Aprova se for identificação (PVA+InkJet+Filme Strech) OU Acessório
                    listaGrupos.Add(grupoEntity);
                else if ((CATEGORIA == 1 && CodigoGrupo == "MKT2")
                    || (TIPOLOCACAO == "A" && CodigoGrupo == "MKT2" && ARMADORA == true)) // Aprova se for fechador ou aluguel armadora
                    listaGrupos.Add(grupoEntity);
                else if (CATEGORIA == 3 && CodigoGrupo == "MKT4") // Aprova se for VHB
                    listaGrupos.Add(grupoEntity);
                else if (CATEGORIA == 4 && CodigoGrupo == "MKT5") // Aprova se for Duplaface
                    listaGrupos.Add(grupoEntity);
                else if (CATEGORIA == 5 && CodigoGrupo == "MKT6") // Aprova se for Flexo
                    listaGrupos.Add(grupoEntity);

                //ATC
                else if ((CATEGORIA == 1 || CATEGORIA == 3 || CATEGORIA == 4 || CATEGORIA == 5) && CodigoGrupo == "ATC1") // Aprova se for fechador, VHB, Duplaface ou Flexo
                    listaGrupos.Add(grupoEntity);
                else if (CATEGORIA == 1 && LINHA.ToUpper().Trim() == "E" && CodigoGrupo == "ATC2") // Aprova se for fechador (máquina especial)
                    listaGrupos.Add(grupoEntity);
                else if ((CATEGORIA == 2 || CATEGORIA == 6) && CodigoGrupo == "ATC3") // Aprova se for identificação (PVA+InkJet+Filme Strech)
                    listaGrupos.Add(grupoEntity);
                else if ((pedido.ST_STATUS_PEDIDO == 6 || pedido.ST_STATUS_PEDIDO == 7) && CodigoGrupo == "ATC4") // 
                    listaGrupos.Add(grupoEntity);
                else if (CodigoGrupo == "ATC5")
                    listaGrupos.Add(grupoEntity);

                //PLN
                else if (CodigoGrupo == "PLN1")
                    listaGrupos.Add(grupoEntity);

                //LOJ
                else if (CodigoGrupo == "LOJ1")
                    listaGrupos.Add(grupoEntity);

            }

            return listaGrupos;
        }

        private WfGrupoEntity MontarObjeto(DataRow r)
        {
            return new WfGrupoEntity()
            {
                ID_GRUPOWF = Convert.ToInt32(r["ID_GRUPOWF"]),
                CD_GRUPOWF = r["CD_GRUPOWF"].ToString(),
                DS_GRUPOWF = r["DS_GRUPOWF"].ToString(),
                TP_GRUPOWF = r["TP_GRUPOWF"].ToString()
            };
        }
    }
}
