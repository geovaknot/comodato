using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Utility
{
    public class WorkflowUtility
    {

        public List<WfGrupoEntity> IdentificaGrupo(int ST_STATUS_PEDIDO, int CATEGORIA, string TIPOLOCACAO, string LINHA, string CD_MODELO)
        {

            List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();
            try
            {

                bool ARMADORA = string.IsNullOrEmpty(CD_MODELO) ? false : CD_MODELO.ToUpper().Contains("ARMADORA");
                WfGrupoEntity grupoEntity = new WfGrupoEntity();

                DataTableReader dataTableReader;
                dataTableReader = new WfGrupoData().ObterListaByStatusPedido(ST_STATUS_PEDIDO).CreateDataReader();

                while (dataTableReader.Read())
                {
                    grupoEntity = new WfGrupoEntity();
                    grupoEntity.ID_GRUPOWF = Convert.ToInt32(dataTableReader["ID_GRUPOWF"]);
                    grupoEntity.CD_GRUPOWF = dataTableReader["CD_GRUPOWF"].ToString();
                    grupoEntity.DS_GRUPOWF = dataTableReader["DS_GRUPOWF"].ToString();
                    grupoEntity.TP_GRUPOWF = dataTableReader["TP_GRUPOWF"].ToString();

                    //MKT
                    if (TIPOLOCACAO == "A" && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT3" && ARMADORA == false) // Aprova se for Aluguel
                        listaGrupos.Add(grupoEntity);
                    else if ((CATEGORIA == 2 || CATEGORIA == 6) && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT1") // Aprova se for identificação (PVA+InkJet+Filme Strech) OU Acessório
                        listaGrupos.Add(grupoEntity);
                    //else if (CATEGORIA == 1 && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT7")
                    //    listaGrupos.Add(grupoEntity);
                    else if ((CATEGORIA == 1 && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT2")
                        || (TIPOLOCACAO == "A" && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT2" && ARMADORA == true)) // Aprova se for fechador ou aluguel armadora
                        listaGrupos.Add(grupoEntity);
                    else if (CATEGORIA == 3 && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT4") // Aprova se for VHB
                        listaGrupos.Add(grupoEntity);
                    else if (CATEGORIA == 4 && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT5") // Aprova se for Duplaface
                        listaGrupos.Add(grupoEntity);
                    else if (CATEGORIA == 5 && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "MKT6") // Aprova se for Flexo
                        listaGrupos.Add(grupoEntity);

                    //ATC
                    else if ((CATEGORIA == 1 || CATEGORIA == 3 || CATEGORIA == 4 || CATEGORIA == 5) && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "ATC1") // Aprova se for fechador, VHB, Duplaface ou Flexo
                        listaGrupos.Add(grupoEntity);
                    //else if (CATEGORIA == 1 && LINHA.ToUpper().Trim() == "E" && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "ATC2") // Aprova se for fechador (máquina especial)
                    //    listaGrupos.Add(grupoEntity);
                    else if (ST_STATUS_PEDIDO == 8 && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "ATC2") 
                        listaGrupos.Add(grupoEntity);

                    else if ((CATEGORIA == 2 || CATEGORIA == 6) && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "ATC3") // Aprova se for identificação (PVA+InkJet+Filme Strech)
                        listaGrupos.Add(grupoEntity);
                    else if (ST_STATUS_PEDIDO == 6 && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "ATC4") // 
                        listaGrupos.Add(grupoEntity);
                    else if ((ST_STATUS_PEDIDO == 27 || ST_STATUS_PEDIDO == 28) && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "ATC5") // 
                        listaGrupos.Add(grupoEntity);

                    // PLN
                    else if (dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "PLN1")
                        listaGrupos.Add(grupoEntity);

                    //LOJ
                    else if ((ST_STATUS_PEDIDO == 22 || ST_STATUS_PEDIDO == 23 || ST_STATUS_PEDIDO == 24 || ST_STATUS_PEDIDO == 25 || ST_STATUS_PEDIDO == 26 || ST_STATUS_PEDIDO == 29) && dataTableReader["CD_GRUPOWF"].ToString().ToUpper().Trim() == "LOJ1")
                        listaGrupos.Add(grupoEntity);
                }
                    dataTableReader.Dispose();
                    dataTableReader = null;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //return BadRequest(ex.Message);
            }

            return listaGrupos;

        }

    }
}
