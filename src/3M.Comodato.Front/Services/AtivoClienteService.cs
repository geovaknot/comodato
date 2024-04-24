using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace _3M.Comodato.Front.Services
{
    public class AtivoClienteService
    {
        public List<DadosFaturamento> ObterListaDadosFaturamento(Int64 ID_ATIVO_CLIENTE)
        {
            DadosFaturamentoEntity dadosFaturamentoEntity = new DadosFaturamentoEntity();
            List<DadosFaturamento> listaDadosFaturamento = new List<DadosFaturamento>();

            if (ID_ATIVO_CLIENTE == 0)
            {
                return listaDadosFaturamento;
            }

            try
            {
                dadosFaturamentoEntity.ID_ATIVO_CLIENTE = ID_ATIVO_CLIENTE;

                DataTableReader dataTableReader = new FaturamentoData().ObterLista(dadosFaturamentoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        #region Mapear faturamento

                        DadosFaturamento faturamento = new DadosFaturamento
                        {
                            //idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_PECA_OS"].ToString()),
                            CD_Material = Convert.ToString(dataTableReader["CD_Material"]),
                            DepartamentoVenda = Convert.ToString(dataTableReader["DepartamentoVenda"]),
                            AluguelApos3anos = Convert.ToDouble(dataTableReader["AluguelApos3Anos"]),
                            DT_UltimoFaturamento = Convert.ToDateTime(dataTableReader["DT_UltimoFaturamento"]),
                            ID = Convert.ToInt64(dataTableReader["ID"]),
                            Ativo = Convert.ToBoolean(dataTableReader["Ativo"]),
                            SituacaoBpcs = Convert.ToString(dataTableReader["SituacaoBpcs"])

                        };
                        faturamento.Data_Fat = faturamento.DT_UltimoFaturamento.ToString("dd/MM/yyyy");
                        if (faturamento.SituacaoBpcs[0] == 'A')
                        {
                            faturamento.DSStatus = "Aguardando Processamento";
                        }else if (faturamento.SituacaoBpcs[0] == 'E')
                        {
                            faturamento.DSStatus = "Erro ao processar";
                        }
                        else if (faturamento.SituacaoBpcs[0] == 'F')
                        {
                            faturamento.DSStatus = "Faturado";
                        }
                        else if (faturamento.SituacaoBpcs[0] == 'P')
                        {
                            faturamento.DSStatus = "Pendente";
                        }
                        else if (faturamento.SituacaoBpcs[0] == 'S')
                        {
                            faturamento.DSStatus = "Solicitado";
                        }

                        #endregion

                        listaDadosFaturamento.Add(faturamento);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return listaDadosFaturamento;
        }


        public List<DadosPagamento> ObterListaDadosPagamento(string ID_FAT)
        {
            DadosPagamentoEntity dadosPagamentoEntity = new DadosPagamentoEntity();
            List<DadosPagamento> listaDadosPagamento = new List<DadosPagamento>();

            if (ID_FAT == null)
            {
                return listaDadosPagamento;
            }

            try
            {

                DataTableReader dataTableReader = new PagamentoData().ObterLista(ID_FAT).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        #region Mapear pagamento

                        //DadosPagamento pagamento = new DadosPagamento
                        //{
                        //    //idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_PECA_OS"].ToString()),
                        //    NRSolicitacaoSESM = Convert.ToInt64(dataTableReader["NRSolicitacaoSESM"]),
                        //    //DT_Solicitacao = Convert.ToDateTime(dataTableReader["DT_Solicitacao"]),
                        //    NR_NotaFiscal = Convert.ToInt64(dataTableReader["NR_NotaFiscal"]),
                        //    SerieNF = Convert.ToString(dataTableReader["SerieNF"]),
                        //    //DataEmissaoNF = Convert.ToDateTime(dataTableReader["DataEmissaoNF"]),
                        //    SituacaoBpcs = Convert.ToString(dataTableReader["SituacaoBpcs"])
                        //};

                        DadosPagamento pagamento = new DadosPagamento();
                        if (dataTableReader["NRSolicitacaoSESM"] != null)
                        {
                            pagamento.NRSolicitacaoSESM = Convert.ToInt64(dataTableReader["NRSolicitacaoSESM"]);
                        }
                        if (dataTableReader["DT_Solicitacao"] != null)
                        {
                            pagamento.DT_Solicitacao = Convert.ToDateTime(dataTableReader["DT_Solicitacao"]);
                        }
                        if (dataTableReader["SerieNF"] != null)
                        {
                            pagamento.SerieNF = Convert.ToString(dataTableReader["SerieNF"]);
                        }
                        
                        if (dataTableReader["SituacaoBpcs"] != null)
                        {
                            pagamento.SituacaoBpcs = Convert.ToString(dataTableReader["SituacaoBpcs"]);
                        }
                        if (pagamento.SituacaoBpcs[0] == 'F')
                        {
                            if (dataTableReader["NR_NotaFiscal"] != null)
                            {
                                pagamento.NR_NotaFiscal = Convert.ToInt64(dataTableReader["NR_NotaFiscal"]);
                            }
                            if (dataTableReader["DataEmissaoNF"] != null)
                            {
                                pagamento.DataEmissaoNF = Convert.ToDateTime(dataTableReader["DataEmissaoNF"]);
                            }
                        }


                        if (pagamento.DT_Solicitacao != null)
                        {
                            pagamento.DT_DATA_Solicitacao = pagamento.DT_Solicitacao.ToString("dd/MM/yyyy");
                        }
                        if (pagamento.DataEmissaoNF != null)
                        {
                            pagamento.DT_DATA_Emissao = pagamento.DataEmissaoNF.ToString("dd/MM/yyyy");
                        }
                        
                        if (pagamento.SituacaoBpcs[0] == 'A')
                        {
                            pagamento.DSStatus = "Aguardando Processamento";
                        }
                        else if (pagamento.SituacaoBpcs[0] == 'E')
                        {
                            pagamento.DSStatus = "Erro ao processar";
                        }
                        else if (pagamento.SituacaoBpcs[0] == 'F')
                        {
                            pagamento.DSStatus = "Faturado";
                        }
                        else if (pagamento.SituacaoBpcs[0] == 'P')
                        {
                            pagamento.DSStatus = "Pendente";
                        }
                        else if (pagamento.SituacaoBpcs[0] == 'S')
                        {
                            pagamento.DSStatus = "Solicitado";
                        }
                        #endregion

                        listaDadosPagamento.Add(pagamento);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return listaDadosPagamento;
        }

        public string[] ObterDeptoVenda()
        {
            //DadosFaturamentoEntity dadosFaturamentoEntity = new DadosFaturamentoEntity();
            //List<String> listaDeptoVenda = new List<String>();

            DataTable deptoVenda = new FaturamentoData().ObterDeptoVenda();
            string strDeptoVenda = ConvertDataTableToString(deptoVenda);
            strDeptoVenda += ";";
            string[] listaDeptoVenda = strDeptoVenda.Split(';');
            return listaDeptoVenda;
        }

        public string[] ObterCodigoMaterial()
        {
            //DadosFaturamentoEntity dadosFaturamentoEntity = new DadosFaturamentoEntity();
            //List<String> listaDeptoVenda = new List<String>();

            DataTable codogiMaterial = new FaturamentoData().ObterCodigoMaterial();
            string strCodigoMaterial = ConvertDataTableToString2(codogiMaterial);
            string[] listaCodigoMaterial = strCodigoMaterial.Split(';');
            return listaCodigoMaterial;
        }

        public static string ConvertDataTableToString(DataTable dt)
        {
            StringBuilder stringBuilder = new StringBuilder();
            dt.Rows.Cast<DataRow>().ToList().ForEach(dataRow =>
            {
                dt.Columns.Cast<DataColumn>().ToList().ForEach(column =>
                {
                    stringBuilder.AppendFormat("{0}", dataRow[column]);
                });
                stringBuilder.Append(Environment.NewLine);
            });
            return stringBuilder.ToString();
        }

        public static string ConvertDataTableToString2(DataTable dt)
        {
            StringBuilder stringBuilder = new StringBuilder();
            dt.Rows.Cast<DataRow>().ToList().ForEach(dataRow =>
            {
                stringBuilder.AppendFormat("{0} - {1};", dataRow["CD_Material"], dataRow["CD_Descricao"]);

                stringBuilder.Append(Environment.NewLine);
            });

            return stringBuilder.ToString();
        }

        public void MapearCamposDadosFaturamentoParaDadosFaturamentoEntity(DadosFaturamento dadosFaturamento, DadosFaturamentoEntity dadosFaturamentoEntity)
        {
            dadosFaturamentoEntity.CD_Material = dadosFaturamento.CD_Material.Substring(0, dadosFaturamento.CD_Material.IndexOf(" ")).Trim();
            dadosFaturamentoEntity.DepartamentoVenda = dadosFaturamento.DepartamentoVenda.Substring(0,3).Trim();
            dadosFaturamentoEntity.AluguelApos3anos = dadosFaturamento.AluguelApos3anos;
            dadosFaturamentoEntity.CD_Cliente = dadosFaturamento.CD_Cliente;
            dadosFaturamentoEntity.NRAtivo = dadosFaturamento.NRAtivo;
            dadosFaturamentoEntity.DT_UltimoFaturamento = dadosFaturamento.DT_UltimoFaturamento;
            dadosFaturamentoEntity.ID_ATIVO_CLIENTE = dadosFaturamento.ID_ATIVO_CLIENTE;
            dadosFaturamentoEntity.AtivoFixo = dadosFaturamento.AtivoFixo;
            if (dadosFaturamento.EnviadoBcps == null)
            {
                dadosFaturamentoEntity.EnviadoBcps = false;
            }
            else
            {
                dadosFaturamentoEntity.EnviadoBcps = dadosFaturamento.EnviadoBcps;
            }
            
        }

        public void MapearCamposDadosPagamentoParaDadosPagamentoEntity(DadosPagamento dadosPagamento, DadosPagamentoEntity dadosPagamentoEntity)
        {
            dadosPagamentoEntity.NRSolicitacaoSESM = dadosPagamento.NRSolicitacaoSESM;
            dadosPagamentoEntity.DT_Solicitacao = dadosPagamento.DT_Solicitacao;
            dadosPagamentoEntity.NR_NotaFiscal = dadosPagamento.NR_NotaFiscal;
            dadosPagamentoEntity.SerieNF = dadosPagamento.SerieNF;
            dadosPagamentoEntity.DataEmissaoNF = dadosPagamento.DataEmissaoNF;
            dadosPagamentoEntity.ID_DADOS_FATURAMENTO = dadosPagamento.ID_DADOS_FATURAMENTO;
        }
    }
}