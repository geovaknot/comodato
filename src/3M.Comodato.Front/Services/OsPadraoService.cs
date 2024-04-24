using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace _3M.Comodato.Front.Services
{
    public class OsPadraoService
    {
        #region Métodos

        public void InformarStatus(OsPadrao osPadrao)
        {
            if (osPadrao.TpStatusOS.ST_STATUS_OS == Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Finalizada) ||
                osPadrao.TpStatusOS.ST_STATUS_OS == Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Cancelada))
                return;

            bool possuiDataMotivo = (!string.IsNullOrWhiteSpace(osPadrao.DT_DATA_OS.ToString()) && osPadrao.TpOS.CD_TIPO_OS > 0);
            bool possuiHoraInicio = !string.IsNullOrWhiteSpace(osPadrao.HR_INICIO);

            if (possuiDataMotivo && possuiHoraInicio)
                osPadrao.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta);
            else
                osPadrao.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOSPadrao.AguardandoInicio);
        }

        public IEnumerable<OsPadrao> ObterListaOrdemServico(DataTable dtOs)
        {
            Func<DataRow, OsPadrao> modelConverter = MapearOsEntityParaOsPadrao();

            IEnumerable<OsPadrao> listaOs = (from registroOsEntity in dtOs.Rows.Cast<DataRow>()
                                             select modelConverter(registroOsEntity)).ToList();
            return listaOs;
        }

        private static Func<DataRow, OsPadrao> MapearOsEntityParaOsPadrao()
        {
            return new Func<DataRow, OsPadrao>((registroOsEntity) =>
            {
                OsPadrao oSPadrao = new OsPadrao();

                oSPadrao.idKey = ControlesUtility.Criptografia.Criptografar(registroOsEntity["ID_OS"].ToString());
                oSPadrao.ID_OS = Convert.ToInt32(registroOsEntity["ID_OS"].ToString());
                oSPadrao.DT_DATA_OS = Convert.ToDateTime(registroOsEntity["DT_DATA_OS"]);
                oSPadrao.DATA_OS_FORMATADA = Convert.ToDateTime(registroOsEntity["DT_DATA_OS"]).ToString("dd/MM/yyyy");
                oSPadrao.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(registroOsEntity["ST_STATUS_OS"].ToString());
                oSPadrao.TpStatusOS.DS_STATUS_OS = registroOsEntity["DS_STATUS_OS"].ToString();
                oSPadrao.cliente.CD_CLIENTE = Convert.ToInt32(registroOsEntity["CD_CLIENTE"].ToString());
                oSPadrao.cliente.NM_CLIENTE = registroOsEntity["NM_CLIENTE"].ToString();
                oSPadrao.cliente.NM_CLIENTE_Codigo = registroOsEntity["NM_CLIENTE"].ToString() + " (" + registroOsEntity["CD_CLIENTE"].ToString() + ")";
                oSPadrao.cliente.regiao.DS_REGIAO = registroOsEntity["DS_REGIAO"].ToString();
                oSPadrao.cliente.regiao.CD_REGIAO = registroOsEntity["CD_REGIAO"].ToString();
                oSPadrao.tecnico.CD_TECNICO = registroOsEntity["CD_TECNICO"].ToString();
                oSPadrao.tecnico.NM_REDUZIDO = registroOsEntity["NM_REDUZIDO"].ToString();
                oSPadrao.tecnico.empresa.NM_Empresa = registroOsEntity["NM_EMPRESA"].ToString();
                oSPadrao.TpOS.DS_TIPO_OS = registroOsEntity["DS_TIPO_OS"].ToString();
                oSPadrao.TpOS.CD_TIPO_OS = Convert.ToInt32(registroOsEntity["CD_TIPO_OS"].ToString());
                oSPadrao.nidUsuarioAtualizacao = Convert.ToInt32(registroOsEntity["nidUsuarioAtualizacao"].ToString());
                oSPadrao.QT_PERIODO = Convert.ToInt32(registroOsEntity["QT_PERIODO"].ToString());
                oSPadrao.HR_INICIO = registroOsEntity["HR_INICIO"].ToString();
                oSPadrao.HR_FIM = registroOsEntity["HR_FIM"].ToString();
                oSPadrao.DS_OBSERVACAO = registroOsEntity["DS_OBSERVACAO"].ToString();
                try
                {
                    TimeSpan TempoGastoTOTAL = new TimeSpan(Convert.ToInt32(oSPadrao.HR_TOTAL.Split(':')[0]), Convert.ToInt32(oSPadrao.HR_TOTAL.Split(':')[1]), 0);

                    oSPadrao.QT_PERIODO_REALIZADO = Convert.ToDecimal(TempoGastoTOTAL.TotalHours) / 3;
                    oSPadrao.QT_PERIODO_REALIZADO_FORMATADO = oSPadrao.QT_PERIODO_REALIZADO.ToString("N2");

                    oSPadrao.PERCENTUAL = oSPadrao.QT_PERIODO > 0
                        ? oSPadrao.PERCENTUAL = Convert.ToDecimal((oSPadrao.QT_PERIODO_REALIZADO * 100) / (oSPadrao.QT_PERIODO)).ToString("N2")
                        : oSPadrao.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                }
                catch
                {
                    oSPadrao.PERCENTUAL = Convert.ToDecimal(0).ToString("N2");
                }

                return oSPadrao;
            });
        }

        public bool ValidarDataHoraOS(OsPadrao os)
        {
            var inicio = os.HR_INICIO.Replace(":", "");

            var fim = "0000";

            if (os.HR_FIM == null)
            {
                fim = "2359";
            }
            else
            {
                fim = os.HR_FIM.Replace(":", "");
            }


            OSPadraoEntity oSEntity = new OSPadraoEntity();

            if (os.CodigoTecnico == null)
            {
                oSEntity.Tecnico.CD_TECNICO = ObterCod_Tecnico(os.ID_OS);
            }
            else
            {
                oSEntity.Tecnico.CD_TECNICO = os.CodigoTecnico;
            }


            var osExistente = new OSPadraoData().ObterListaOSSincHoras(oSEntity).ToList();

            osExistente = osExistente.Where(x => x.DT_DATA_OS.Day == os.DT_DATA_OS.Value.Day
                                            && x.DT_DATA_OS.Month == os.DT_DATA_OS.Value.Month
                                            && x.DT_DATA_OS.Year == os.DT_DATA_OS.Value.Year
                                            && x.TpStatusOS.ST_STATUS_OS != 4
                                            && x.ID_OS != os.ID_OS).ToList();

            if (osExistente?.Count > 0)
            {
                foreach (var existe in osExistente)
                {
                    var inicioExiste = existe.HR_INICIO.Replace(":", "");
                    var fimExiste = "0000";
                    if (existe.HR_FIM != null)
                    {
                        fimExiste = existe.HR_FIM.Replace(":", "");
                    }
                    else
                    {
                        fimExiste = "2359";
                    }

                    if (Convert.ToInt64(inicio) >= Convert.ToInt64(inicioExiste) && Convert.ToInt64(inicio) <= Convert.ToInt64(fimExiste))
                    {
                        return true;
                    }
                    if (os.HR_FIM != null)
                    {
                        if (Convert.ToInt64(fim) >= Convert.ToInt64(inicioExiste) && Convert.ToInt64(fim) <= Convert.ToInt64(fimExiste))
                        {
                            return true;
                        }
                    }

                }
            }

            return false;

        }
        

        public OSPadraoEntity CriarOSPadraoEntity(string CD_TECNICO, string CD_REGIAO, int? ST_STATUS_OS, int? CD_TIPO_OS, int? CD_CLIENTE)
        {
            return new OSPadraoEntity()
            {
                DT_DATA_OS = DateTime.MinValue,

                Tecnico = new TecnicoEntity()
                {
                    CD_TECNICO = CD_TECNICO,
                },
                Cliente = new ClienteEntity()
                {
                    CD_CLIENTE = CD_CLIENTE == null ? 0 : (int)CD_CLIENTE,

                    regiao = new RegiaoEntity()
                    {
                        CD_REGIAO = CD_REGIAO
                    }
                },
                TpStatusOS = new TpStatusOSPadraoEntity()
                {
                    ST_STATUS_OS = ST_STATUS_OS == null ? 0 : (int)ST_STATUS_OS
                },
                TpOS = new TpOSPadraoEntity()
                {
                    CD_TIPO_OS = CD_TIPO_OS == null ? 0 : (int)CD_TIPO_OS
                }
            };
        }

        public bool VerificarExisteOsDataHora(string codTecnico, int idOs, string dataOs, string horaInicio, string horaFim)
        {
            OSPadraoEntity osPadraoEntity = new OSPadraoEntity();
            osPadraoEntity.ID_OS = idOs;
            osPadraoEntity.Tecnico.CD_TECNICO = codTecnico;
            osPadraoEntity.DT_DATA_OS = Convert.ToDateTime(dataOs);
            osPadraoEntity.HR_INICIO = horaInicio;
            osPadraoEntity.HR_FIM = horaFim;

            return ControlesUtility.Utilidade.VerificarExisteOsDataHoraSemCompararOsInformada(osPadraoEntity);
        }

        public bool VerificarExisteVisitaDataHora(string codTecnico, string dataVisita, string horaInicio, string horaFim)
        {
            VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();
            visitaPadraoEntity.Tecnico.CD_TECNICO = codTecnico;
            visitaPadraoEntity.DT_DATA_VISITA = Convert.ToDateTime(dataVisita);
            visitaPadraoEntity.HR_INICIO = horaInicio;
            visitaPadraoEntity.HR_FIM = horaFim;

            return ControlesUtility.Utilidade.VerificarExisteVisitaDataHora(visitaPadraoEntity);
        }

        public bool VerificarExisteOs(string codTecnico, ControlesUtility.Enumeradores.TpStatusOSPadrao statusOs)
        {
            OSPadraoEntity OsPadraoEntity = new OSPadraoEntity();

            OsPadraoEntity.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(statusOs);
            OsPadraoEntity.Tecnico.CD_TECNICO = codTecnico;

            DataTableReader dataTableReader = new OSPadraoData().ObterListaOs(OsPadraoEntity, null, null).CreateDataReader();

            try
            {
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (Convert.ToInt64(dataTableReader["ID_OS"]) != OsPadraoEntity.ID_OS)
                            return true;
                    }
                }
            }
            finally
            {
                if (dataTableReader != null)
                {
                    dataTableReader.Close();
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }

            return false;
        }

        public bool VerificarExisteVisita(string codTecnico, ControlesUtility.Enumeradores.TpStatusVisitaPadrao statusVisita)
        {
            VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();

            visitaPadraoEntity.TpStatusVisita.ST_STATUS_VISITA = Convert.ToInt32(statusVisita);
            visitaPadraoEntity.Tecnico.CD_TECNICO = codTecnico;

            DataTableReader dataTableReader = new VisitaPadraoData().ObterLista(visitaPadraoEntity, null, null).CreateDataReader();

            try
            {
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (Convert.ToInt64(dataTableReader["ID_VISITA"]) != visitaPadraoEntity.ID_VISITA)
                            return true;
                    }
                }
            }
            finally
            {
                if (dataTableReader != null)
                {
                    dataTableReader.Close();
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }

            return false;
        }

        public OsPadrao ObterOS(Int64 ÏD_OS)
        {
            OsPadrao osPadrao = null;

            try
            {
                OSPadraoEntity osPadraoEntity = new OSPadraoEntity();

                osPadraoEntity.ID_OS = ÏD_OS;

                DataTableReader dataTableReader = new OSPadraoData().ObterListaOs(osPadraoEntity, null, null).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        osPadrao = new OsPadrao
                        {
                            ID_OS = Convert.ToInt64("0" + dataTableReader["ID_OS"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_OS"].ToString()),
                            DT_DATA_OS = Convert.ToDateTime(dataTableReader["DT_DATA_OS"]),
                            DATA_OS_FORMATADA = Convert.ToDateTime(dataTableReader["DT_DATA_OS"]).ToString("dd/MM/yyyy"),
                            HR_INICIO = dataTableReader["HR_INICIO"].ToString(),
                            HR_FIM = dataTableReader["HR_FIM"].ToString(),
                            DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString(),
                            DS_RESPONSAVEL = dataTableReader["DS_RESPONSAVEL"].ToString(),
                            EMAIL = dataTableReader["EMAIL"].ToString(),
                            NOME_LINHA = dataTableReader["NOME_LINHA"].ToString(),

                            TpStatusOS = new TpStatusOSPadraoEntity
                            {
                                ST_STATUS_OS = Convert.ToInt32("0" + dataTableReader["ST_STATUS_OS"].ToString()),
                                DS_STATUS_OS = dataTableReader["DS_STATUS_OS"].ToString()
                            },

                            TpOS = new TpOSPadraoEntity
                            {
                                CD_TIPO_OS = Convert.ToInt32("0" + dataTableReader["CD_TIPO_OS"].ToString()),
                                DS_TIPO_OS = dataTableReader["DS_TIPO_OS"].ToString()
                            },

                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"].ToString()),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                            },

                            tecnico = new TecnicoEntity
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                                NM_TECNICO = dataTableReader["NM_TECNICO"].ToString(),

                                empresa = new EmpresaEntity
                                {
                                    NM_Empresa = dataTableReader["NM_EMPRESA"].ToString()
                                }
                            },

                            regiao = new RegiaoEntity
                            {
                                CD_REGIAO = dataTableReader["CD_REGIAO"].ToString(),
                                DS_REGIAO = dataTableReader["DS_REGIAO"].ToString()
                            },

                            ativoFixo = new AtivoFixoEntity
                            {
                                CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                                DS_ATIVO_FIXO = $"{dataTableReader["DS_MODELO"].ToString()} - {dataTableReader["TX_ANO_MÁQUINA"].ToString()}"
                            },

                            pecaOS = new PecaOS()
                            {
                                listaPecas = new List<PecaEntity>(),
                                tiposEstoqueUtilizado = ControlesUtility.Dicionarios.TipoEstoqueUtilizado()
                            },

                            pendenciaOS = new PendenciaOS()
                            {
                                listaPecas = new List<PecaEntity>(),
                                tiposEstoqueUtilizado = ControlesUtility.Dicionarios.TipoEstoqueUtilizado(),
                                tiposStatusPendenciaOS = ControlesUtility.Dicionarios.TipoStatusPendenciaOS(),
                                tiposPendenciaOS = ControlesUtility.Dicionarios.TipoPendenciaOS()
                            },

                            reclamacaoOs = new ReclamacaoOs()
                            {
                                listaPecas = new List<PecaEntity>(),
                                tiposAtendimento = ControlesUtility.Dicionarios.TipoAtendimento(),
                                tiposReclamacao = ControlesUtility.Dicionarios.TipoReclamacaoRR()
                            }
                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return osPadrao;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public OsPadrao Obter(string idKey)
        {
            OsPadrao osPadrao = null;

            try
            {
                OSPadraoEntity osPadraoEntity = new OSPadraoEntity
                {
                    ID_OS = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey))
                };

                DataTableReader dataTableReader = new OSPadraoData().ObterListaOs(osPadraoEntity, null, null).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        osPadrao = new OsPadrao
                        {
                            ID_OS = Convert.ToInt64("0" + dataTableReader["ID_OS"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_OS"].ToString()),
                            DT_DATA_OS = Convert.ToDateTime(dataTableReader["DT_DATA_OS"]),
                            DATA_OS_FORMATADA = Convert.ToDateTime(dataTableReader["DT_DATA_OS"]).ToString("dd/MM/yyyy"),
                            HR_INICIO = dataTableReader["HR_INICIO"].ToString(),
                            HR_FIM = dataTableReader["HR_FIM"].ToString(),
                            DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString(),
                            DS_RESPONSAVEL = dataTableReader["DS_RESPONSAVEL"].ToString(),
                            EMAIL = dataTableReader["EMAIL"].ToString(),
                            NOME_LINHA = dataTableReader["NOME_LINHA"].ToString(),
                            GRUPO_MODELO_ATIVO = dataTableReader["GRUPO_MODELO_ATIVO"].ToString(),
                            Origem = dataTableReader["Origem"].ToString(),

                            TpStatusOS = new TpStatusOSPadraoEntity
                            {
                                ST_STATUS_OS = Convert.ToInt32("0" + dataTableReader["ST_STATUS_OS"].ToString()),
                                DS_STATUS_OS = dataTableReader["DS_STATUS_OS"].ToString()
                            },

                            TpOS = new TpOSPadraoEntity
                            {
                                CD_TIPO_OS = Convert.ToInt32("0" + dataTableReader["CD_TIPO_OS"].ToString()),
                                DS_TIPO_OS = dataTableReader["DS_TIPO_OS"].ToString()
                            },

                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt64("0" + dataTableReader["CD_CLIENTE"].ToString()),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString()
                            },

                            tecnico = new TecnicoEntity
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                                NM_TECNICO = dataTableReader["NM_TECNICO"].ToString(),

                                empresa = new EmpresaEntity
                                {
                                    NM_Empresa = dataTableReader["NM_EMPRESA"].ToString()
                                }
                            },

                            regiao = new RegiaoEntity
                            {
                                CD_REGIAO = dataTableReader["CD_REGIAO"].ToString(),
                                DS_REGIAO = dataTableReader["DS_REGIAO"].ToString()
                            },

                            ativoFixo = new AtivoFixoEntity
                            {
                                CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                                DS_ATIVO_FIXO = $"{dataTableReader["DS_MODELO"].ToString()} - {dataTableReader["TX_ANO_MÁQUINA"].ToString()}"
                            },

                            pecaOS = new PecaOS()
                            {
                                listaPecas = new List<PecaEntity>(),
                                tiposEstoqueUtilizado = ControlesUtility.Dicionarios.TipoEstoqueUtilizado()
                            },

                            pendenciaOS = new PendenciaOS()
                            {
                                listaPecas = new List<PecaEntity>(),
                                tiposEstoqueUtilizado = ControlesUtility.Dicionarios.TipoEstoqueUtilizado(),
                                tiposStatusPendenciaOS = ControlesUtility.Dicionarios.TipoStatusPendenciaOS(),
                                tiposPendenciaOS = ControlesUtility.Dicionarios.TipoPendenciaOS()
                            },

                            reclamacaoOs = new ReclamacaoOs()
                            {
                                listaPecas = new List<PecaEntity>(),
                                tiposAtendimento = ControlesUtility.Dicionarios.TipoAtendimento(),
                                tiposReclamacao = ControlesUtility.Dicionarios.TipoReclamacaoRR()
                            }
                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return osPadrao;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public void MapearCamposOsPadraoParaOsPadraoEntity(OsPadrao osPadrao, OSPadraoEntity osPadraoEntity, long idUsuario)
        {
            osPadraoEntity.ID_OS = osPadrao.ID_OS;
            osPadraoEntity.DT_DATA_OS = Convert.ToDateTime(osPadrao.DT_DATA_OS);
            osPadraoEntity.HR_INICIO = osPadrao.HR_INICIO;
            osPadraoEntity.HR_FIM = osPadrao.HR_FIM;
            osPadraoEntity.DS_OBSERVACAO = osPadrao.DS_OBSERVACAO;
            osPadraoEntity.TpStatusOS.ST_STATUS_OS = osPadrao.TpStatusOS.ST_STATUS_OS;
            osPadraoEntity.TpOS.CD_TIPO_OS = osPadrao.TpOS.CD_TIPO_OS;
            osPadraoEntity.Cliente.CD_CLIENTE = osPadrao.cliente.CD_CLIENTE;
            osPadraoEntity.Tecnico.CD_TECNICO = osPadrao.tecnico.CD_TECNICO;
            osPadraoEntity.AtivoFixo.CD_ATIVO_FIXO = osPadrao.ativoFixo.CD_ATIVO_FIXO;
            osPadraoEntity.AtivoFixo.modelo.DS_MODELO = osPadrao.ativoFixo.modelo.DS_MODELO;
            osPadraoEntity.DS_RESPONSAVEL = osPadrao.DS_RESPONSAVEL;
            osPadraoEntity.Email = osPadrao.EMAIL;
            osPadraoEntity.NOME_LINHA = osPadrao.NOME_LINHA;
            osPadraoEntity.nidUsuarioAtualizacao = idUsuario; 
        }

        public void AdicionarValoresPadraoIncluirOs(OsPadrao osPadrao, long idUsuario)
        {
            if (ControlesUtility.Seguranca.VerificaUsuarioPerfisTecnicos(idUsuario))
            {
                var tecnicoEntity = osPadrao.tecnicos.FirstOrDefault();
                osPadrao.tecnico.CD_TECNICO = tecnicoEntity?.CD_TECNICO;
                osPadrao.tecnico.empresa.NM_Empresa = tecnicoEntity?.empresa.NM_Empresa;
            }

            osPadrao.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusOSPadrao.AguardandoInicio);
            osPadrao.TpStatusOS.DS_STATUS_OS = osPadrao.tiposStatusOsPadrao.Find(s => s.ST_STATUS_OS == osPadrao.TpStatusOS.ST_STATUS_OS).DS_STATUS_OS;
            osPadrao.DT_DATA_OS = DateTime.Now;
        }

        public void MapearCamposIncluirOs(OsPadrao osPadrao, OSPadraoEntity osPadraoEntity, long idUsuario)
        {
            MapearCamposOsPadraoParaOsPadraoEntity(osPadrao, osPadraoEntity, idUsuario);

            osPadraoEntity.TOKEN = ControlesUtility.Utilidade.ObterPrefixoTokenRegistro();

            if (!string.IsNullOrWhiteSpace(osPadrao.CodigoTecnico))
                osPadraoEntity.Tecnico.CD_TECNICO = osPadrao.CodigoTecnico;
        }

        public void ValidarExcluirOs(OsPadrao osPadrao)
        {
            if (osPadrao.TpStatusOS.ST_STATUS_OS != Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.AguardandoInicio) &&
                osPadrao.TpStatusOS.ST_STATUS_OS != Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta) &&
                osPadrao.TpStatusOS.ST_STATUS_OS != Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Finalizada))
            {
                osPadrao.JavaScriptToRun = "MensagemInformativa()";
            }
        }

        public List<PecaOS> ObterListaPecaOS(Int64 ID_OS)
        {
            PecaOSEntity pecaOSEntity = new PecaOSEntity();
            List<PecaOS> listaPecasOS = new List<PecaOS>();

            if (ID_OS == 0)
            {
                return listaPecasOS;
            }

            try
            {
                pecaOSEntity.OS.ID_OS = ID_OS;

                DataTableReader dataTableReader = new PecaOSData().ObterLista(pecaOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        #region Mapear peça

                        PecaOS pecaOS = new PecaOS
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_PECA_OS"].ToString()),
                            ID_PECA_OS = Convert.ToInt64(dataTableReader["ID_PECA_OS"]),
                            OS = new OSEntity()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                            },
                            peca = new PecaEntity()
                            {
                                CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            },
                            CD_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Value,
                            DS_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Key,
                        };

                        pecaOS.QT_PECA = pecaOS.peca.TX_UNIDADE == "MT" ? pecaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3")
                            : pecaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N0");

                        pecaOS.DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString() ?? "";

                        #endregion

                        listaPecasOS.Add(pecaOS);
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

            return listaPecasOS;
        }

        public long ObterCod_Cliente(Int64 ID_OS)
        {
            OSPadraoEntity os = new OSPadraoEntity();
            os.ID_OS = ID_OS;

            IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();
            
            listaOS = new OSPadraoData().ObterLista(os);

            return listaOS[0].Cliente.CD_CLIENTE;
        }

        public string ObterCod_Tecnico(Int64 ID_OS)
        {
            OSPadraoEntity os = new OSPadraoEntity();
            os.ID_OS = ID_OS;

            IList<OSPadraoEntity> listaOS = new List<OSPadraoEntity>();

            listaOS = new OSPadraoData().ObterLista(os);

            return listaOS[0].Tecnico.CD_TECNICO;
        }

        public List<PecaOSSinc> ObterListaPecaOSFinalizar(Int64 ID_OS)
        {
            PecaOSEntity pecaOSEntity = new PecaOSEntity();
            List<PecaOSSinc> listaPecasOS = new List<PecaOSSinc>();

            if (ID_OS == 0)
            {
                return listaPecasOS;
            }

            try
            {
                pecaOSEntity.OS.ID_OS = ID_OS;

                DataTableReader dataTableReader = new PecaOSData().ObterLista(pecaOSEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        #region Mapear peça

                        PecaOSSinc pecaOS = new PecaOSSinc
                        {
                            //idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_PECA_OS"].ToString()),
                            ID_PECA_OS = Convert.ToInt64(dataTableReader["ID_PECA_OS"]),
                            ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                            CD_PECA = dataTableReader["CD_PECA"].ToString(),
                            QT_PECA = Convert.ToInt64(dataTableReader["QT_PECA"]),
                            CD_TP_ESTOQUE_CLI_TEC = dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString().ToArray()[0]

                        };
                        
                        #endregion

                        listaPecasOS.Add(pecaOS);
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

            return listaPecasOS;
        }

        public List<ListaPendenciaOS> ObterListaPendenciaOS(Int64 CD_CLIENTE, Int64 ID_OS, string CD_TECNICO)
        {
            List<ListaPendenciaOS> listaPendenciasOS = new List<ListaPendenciaOS>();

            if (CD_CLIENTE == 0 || ID_OS == 0)
                return listaPendenciasOS;

            try
            {
                DataTableReader dataTableReader = new PendenciaOSData().ObterListaCliente(CD_CLIENTE, ID_OS, CD_TECNICO).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        #region Mapear Lista Pendencia OS

                        ListaPendenciaOS listaPendenciaOS = new ListaPendenciaOS
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_PENDENCIA_OS"].ToString()),
                            ID_PENDENCIA_OS = Convert.ToInt64(dataTableReader["ID_PENDENCIA_OS"]),
                            PENDENCIA_OS = Convert.ToInt64(dataTableReader["PENDENCIA_OS"]),
                            OsPadrao = new OsPadrao()
                            {
                                ID_OS = Convert.ToInt64(dataTableReader["ID_OS"]),
                                ID_OS_Formatado = Convert.ToInt64(dataTableReader["ID_OS"]).ToString("000000"),
                            },
                            DT_ABERTURA = Convert.ToDateTime(dataTableReader["DT_ABERTURA"]).ToString("dd/MM/yyyy"),
                            DS_DESCRICAO = dataTableReader["DS_DESCRICAO"].ToString(),
                            peca = new PecaEntity()
                            {
                                CD_PECA = dataTableReader["CD_PECA"].ToString(),
                                DS_PECA = dataTableReader["DS_PECA"].ToString(),
                                TX_UNIDADE = dataTableReader["TX_UNIDADE"].ToString(),
                            },
                            tecnico = new TecnicoEntity()
                            {
                                CD_TECNICO = dataTableReader["CD_TECNICO"].ToString(),
                            },

                            ST_STATUS_PENDENCIA = ControlesUtility.Dicionarios.TipoStatusPendenciaOS().Where(x => x.Value == dataTableReader["ST_STATUS_PENDENCIA"].ToString()).ToArray()[0].Value,
                            DS_STATUS_PENDENCIA = ControlesUtility.Dicionarios.TipoStatusPendenciaOS().Where(x => x.Value == dataTableReader["ST_STATUS_PENDENCIA"].ToString()).ToArray()[0].Key,
                        };

                        if (dataTableReader["CD_TP_ESTOQUE_CLI_TEC"] != DBNull.Value)
                        {
                            listaPendenciaOS.CD_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Value;
                            listaPendenciaOS.DS_TP_ESTOQUE_CLI_TEC = ControlesUtility.Dicionarios.TipoEstoqueUtilizado().Where(x => x.Value == dataTableReader["CD_TP_ESTOQUE_CLI_TEC"].ToString()).ToArray()[0].Key;
                        }

                        if (dataTableReader["QT_PECA"] != DBNull.Value)
                        {
                            listaPendenciaOS.QT_PECA = listaPendenciaOS.peca.TX_UNIDADE == "MT" ? listaPendenciaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N3")
                                : listaPendenciaOS.QT_PECA = Convert.ToDecimal(dataTableReader["QT_PECA"]).ToString("N0");
                        }

                        #endregion

                        listaPendenciasOS.Add(listaPendenciaOS);
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

            return listaPendenciasOS;
        }

        public List<ReclamacaoOs> ObterListaReclamacaoOS(Int64 ID_OS)
        {
            RelatorioReclamacaoEntity reclamacaoOsEntity = new RelatorioReclamacaoEntity();
            List<ReclamacaoOs> listaReclamacaoOs = new List<ReclamacaoOs>();

            if (ID_OS == 0)
                return listaReclamacaoOs;

            try
            {
                reclamacaoOsEntity.osPadraoEntity.ID_OS = ID_OS;

                DataTableReader dataTableReader = new RelatorioReclamacaoData().ObterLista(reclamacaoOsEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        #region Mapear reclamação

                        ReclamacaoOs reclamacaoOs = new ReclamacaoOs();
                        reclamacaoOs.idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_RELATORIO_RECLAMACAO"].ToString());
                        reclamacaoOs.ID_RELATORIO_RECLAMACAO = Convert.ToInt64(dataTableReader["ID_RELATORIO_RECLAMACAO"]);
                        reclamacaoOs.CD_TIPO_ATENDIMENTO= Convert.ToInt32(dataTableReader["CD_TIPO_ATENDIMENTO"]);
                        reclamacaoOs.DS_TIPO_ATENDIMENTO = ControlesUtility.Dicionarios.TipoAtendimento().Where(x => x.Key == Convert.ToInt32(dataTableReader["CD_TIPO_ATENDIMENTO"].ToString())).ToArray()[0].Value;
                        reclamacaoOs.CD_TIPO_RECLAMACAO = Convert.ToInt32(dataTableReader["CD_TIPO_RECLAMACAO"]);
                        reclamacaoOs.DS_TIPO_RECLAMACAO = ControlesUtility.Dicionarios.TipoReclamacaoRR().Where(x => x.Key == Convert.ToInt32(dataTableReader["CD_TIPO_RECLAMACAO"].ToString())).ToArray()[0].Value;
                        reclamacaoOs.pecaEntity.CD_PECA = dataTableReader["CD_PECA"].ToString();
                        reclamacaoOs.pecaEntity.DS_PECA = dataTableReader["DS_PECA"].ToString();
                        reclamacaoOs.DS_DESCRICAO = dataTableReader["DS_DESCRICAO"].ToString();
                        reclamacaoOs.DS_MOTIVO = dataTableReader["DS_MOTIVO"].ToString();
                        reclamacaoOs.rrStatusEntity.DS_STATUS_NOME = dataTableReader["DS_STATUS_NOME"].ToString();
                        reclamacaoOs.DS_ARQUIVO_FOTO = dataTableReader["DS_ARQUIVO_FOTO"].ToString();
                        reclamacaoOs.DS_TIPO_FOTO = dataTableReader["DS_TIPO_FOTO"].ToString();

                        var tempo = dataTableReader["VL_TEMPO_ATENDIMENTO"].ToString();

                        if(tempo != "0" && tempo != "" && tempo != null)
                        {
                            if(Convert.ToInt32(tempo) < 60)
                            {
                                reclamacaoOs.TEMPO_ATENDIMENTO_FORMATADO = "00:" + tempo;
                            }
                            else
                            {
                                var timehoras = (Convert.ToDouble(tempo) / 60).ToString();

                                timehoras = timehoras.Replace(",", ".");
                                if (timehoras.Contains("."))
                                {
                                    var tempogasto = timehoras.Split('.');
                                    var horasGastas = tempogasto[0].ToString();
                                    double min = Convert.ToDouble(tempogasto[1]);
                                    var minutosgastos = (min / 100) * 60;
                                    reclamacaoOs.TEMPO_ATENDIMENTO_FORMATADO = timehoras[0] + ":" + minutosgastos.ToString();
                                }
                                else
                                {
                                    reclamacaoOs.TEMPO_ATENDIMENTO_FORMATADO = timehoras + ":00";
                                }
                            }
                            
                        }

                        #endregion

                        listaReclamacaoOs.Add(reclamacaoOs);
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

            return listaReclamacaoOs;
        }
        #endregion
    }
}