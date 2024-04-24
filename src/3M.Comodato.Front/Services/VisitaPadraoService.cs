using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace _3M.Comodato.Front.Services
{
    public class VisitaPadraoService
    {
        #region Métodos

        public IEnumerable<VisitaPadrao> ObterVisitas(DataTable dtVisita)
        {
            Func<DataRow, VisitaPadrao> modelConverter = MaperarVisitaPadraoEntityParaVisitaPadrao();

            IEnumerable<VisitaPadrao> listaVisita = (from registroVisitaEntity in dtVisita.Rows.Cast<DataRow>()
                                                     select modelConverter(registroVisitaEntity)).ToList();
            return listaVisita;
        }

        private static Func<DataRow, VisitaPadrao> MaperarVisitaPadraoEntityParaVisitaPadrao()
        {
            return new Func<DataRow, VisitaPadrao>((registroVisitaEntity) =>
            {
                VisitaPadrao visitaPadrao = new VisitaPadrao();

                visitaPadrao.idKey = ControlesUtility.Criptografia.Criptografar(registroVisitaEntity["ID_VISITA"].ToString());
                visitaPadrao.ID_VISITA = Convert.ToInt32(registroVisitaEntity["ID_VISITA"].ToString());
                visitaPadrao.DT_DATA_VISITA = Convert.ToDateTime(registroVisitaEntity["DT_DATA_VISITA"]).ToString("dd/MM/yyyy");
                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA = Convert.ToInt32(registroVisitaEntity["ST_STATUS_VISITA"].ToString());
                visitaPadrao.TpStatusVisita.DS_STATUS_VISITA = registroVisitaEntity["DS_STATUS_VISITA"].ToString();
                visitaPadrao.cliente.CD_CLIENTE = Convert.ToInt32(registroVisitaEntity["CD_CLIENTE"].ToString());
                visitaPadrao.cliente.NM_CLIENTE_Codigo = registroVisitaEntity["NM_CLIENTE"].ToString() + " (" + registroVisitaEntity["CD_CLIENTE"].ToString() + ")";
                visitaPadrao.cliente.NM_CLIENTE = registroVisitaEntity["NM_CLIENTE"].ToString();
                visitaPadrao.cliente.regiao.DS_REGIAO = registroVisitaEntity["DS_REGIAO"].ToString();
                visitaPadrao.cliente.regiao.CD_REGIAO = registroVisitaEntity["CD_REGIAO"].ToString();
                visitaPadrao.tecnico.CD_TECNICO = registroVisitaEntity["CD_TECNICO"].ToString();
                visitaPadrao.tecnico.empresa.NM_Empresa = registroVisitaEntity["NM_EMPRESA"].ToString();
                visitaPadrao.DS_OBSERVACAO = registroVisitaEntity["DS_OBSERVACAO"].ToString();
                visitaPadrao.TpMotivoVisita.DS_MOTIVO_VISITA = registroVisitaEntity["DS_MOTIVO_VISITA"].ToString();
                visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA = Convert.ToInt32(registroVisitaEntity["CD_MOTIVO_VISITA"].ToString());
                visitaPadrao.nidUsuarioAtualizacao = Convert.ToInt32(registroVisitaEntity["nidUsuarioAtualizacao"].ToString());
                visitaPadrao.EMAIL = registroVisitaEntity["EMAIL"].ToString();
                visitaPadrao.DS_RESPONSAVEL = registroVisitaEntity["DS_RESPONSAVEL"].ToString();

                return visitaPadrao;
            });
        }

        public VisitaPadraoEntity CriarVisitaPadraoEntity(string CD_TECNICO, string CD_REGIAO, int? ST_STATUS_VISITA, int? CD_MOTIVO_VISITA, int? CD_CLIENTE)
        {
            return new VisitaPadraoEntity()
            {
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
                TpStatusVisita = new TpStatusVisitaPadraoEntity()
                {
                    ST_STATUS_VISITA = ST_STATUS_VISITA == null ? 0 : (int)ST_STATUS_VISITA
                },
                TpMotivoVisita = new TpMotivoVisitaPadraoEntity()
                {
                    CD_MOTIVO_VISITA = CD_MOTIVO_VISITA == null ? 0 : (int)CD_MOTIVO_VISITA
                }
            };
        }

        public void AdicionarValoresPadraoIncluirVisita(VisitaPadrao visitaPadrao, long idUsuario)
        {
            if (ControlesUtility.Seguranca.VerificaUsuarioPerfisTecnicos(idUsuario))
            {
                var tecnicoEntity = visitaPadrao.tecnicos.FirstOrDefault();
                visitaPadrao.tecnico.CD_TECNICO = tecnicoEntity?.CD_TECNICO;
                visitaPadrao.tecnico.empresa.NM_Empresa = tecnicoEntity?.empresa.NM_Empresa;
            }
        }

        public void InformarStatus(VisitaPadrao visitaPadrao, long idUsuario)
        {
            bool possuiDataMotivo = (!string.IsNullOrWhiteSpace(visitaPadrao.DT_DATA_VISITA) && visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA > 0);
            bool possuiHoraInicio = !string.IsNullOrWhiteSpace(visitaPadrao.HR_INICIO);
            bool possuiHoraTermino = !string.IsNullOrWhiteSpace(visitaPadrao.HR_FIM);

            if (possuiDataMotivo && possuiHoraInicio && possuiHoraTermino)
                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Finalizada);
            else if (possuiDataMotivo && possuiHoraInicio)
                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Aberta);
            else if (possuiDataMotivo)
                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA = Convert.ToInt32(ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Iniciar);

            
        }

        public void EnviarEmail(VisitaPadrao visitaPadrao, long idUsuario)
        {
            VisitaPadraoEntity VisitaEmail = new VisitaPadraoEntity();

            MapearCamposVisitaPadraoParaVisitaPadraoEntity(visitaPadrao, VisitaEmail, idUsuario);

            if (VisitaEmail.Tecnico.CD_TECNICO == null)
            {
                VisitaEmail.Tecnico.CD_TECNICO = ObterCod_Tecnico(VisitaEmail.ID_VISITA);
            }

            SincronismoData sincronismoData = new SincronismoData();
            Int64? idVisita = null;
            if (visitaPadrao.ID_VISITA > 0)
            {
                idVisita = visitaPadrao.ID_VISITA;
            }
            sincronismoData.EnviarEmailVisita(VisitaEmail, idVisita);
            
        }

        public void MaperarCamposIncluirVisita(VisitaPadrao visitaPadrao, VisitaPadraoEntity visitaPadraoEntity, long idUsuario)
        {
            MapearCamposVisitaPadraoParaVisitaPadraoEntity(visitaPadrao, visitaPadraoEntity, idUsuario);

            visitaPadraoEntity.TOKEN = ControlesUtility.Utilidade.ObterPrefixoTokenRegistro();

            if (!string.IsNullOrWhiteSpace(visitaPadrao.CodigoTecnico))
                visitaPadraoEntity.Tecnico.CD_TECNICO = visitaPadrao.CodigoTecnico;
        }

        public void MapearCamposVisitaPadraoParaVisitaPadraoEntity(VisitaPadrao visitaPadrao, VisitaPadraoEntity visitaPadraoEntity, long idUsuario)
        {
            visitaPadraoEntity.ID_VISITA = visitaPadrao.ID_VISITA;
            visitaPadraoEntity.DT_DATA_VISITA = Convert.ToDateTime(visitaPadrao.DT_DATA_VISITA);
            visitaPadraoEntity.Cliente.CD_CLIENTE = visitaPadrao.cliente.CD_CLIENTE;
            visitaPadraoEntity.Tecnico.CD_TECNICO = visitaPadrao.tecnico.CD_TECNICO;
            visitaPadraoEntity.TpMotivoVisita.CD_MOTIVO_VISITA = visitaPadrao.TpMotivoVisita.CD_MOTIVO_VISITA;
            visitaPadraoEntity.TpStatusVisita.ST_STATUS_VISITA = visitaPadrao.TpStatusVisita.ST_STATUS_VISITA;
            visitaPadraoEntity.HR_INICIO = visitaPadrao.HR_INICIO;
            visitaPadraoEntity.HR_FIM = visitaPadrao.HR_FIM;
            visitaPadraoEntity.Email = visitaPadrao.EMAIL;
            visitaPadraoEntity.DS_RESPONSAVEL = visitaPadrao.DS_RESPONSAVEL;
            visitaPadraoEntity.DS_OBSERVACAO = visitaPadrao.DS_OBSERVACAO;
            visitaPadraoEntity.nidUsuarioAtualizacao = idUsuario;
        }

        public void MapearCamposVisitaRespostaParaVisitaRespostaEntity(VisitaResposta visita, VisitaRespostaEntity visitaEntity)
        {
            visitaEntity.DataResposta = visita.DataResposta;
            visitaEntity.ID_OS = visita.ID_OS;
            visitaEntity.ID_VISITA = visita.ID_VISITA;
            visitaEntity.Justificativa = visita.Justificativa;
            visitaEntity.nidUsuarioAtualizacao = visita.nidUsuarioAtualizacao;
            visitaEntity.DS_NOME_RESPONDEDOR = visita.DS_NOME_RESPONDEDOR;
            visitaEntity.NotaPesquisa = visita.NotaPesquisa;
        }


        public void ValidarExcluirVisita(VisitaPadrao visitaPadrao)
        {
            if (visitaPadrao.TpStatusVisita.ST_STATUS_VISITA != Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Iniciar) &&
                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA != Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Aberta) &&
                visitaPadrao.TpStatusVisita.ST_STATUS_VISITA != Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusVisitaPadrao.Finalizada))
            {
                visitaPadrao.JavaScriptToRun = "MensagemInformativa()";
            }
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

        public bool VerificarExisteOs(string codTecnico, ControlesUtility.Enumeradores.TpStatusOSPadrao statusOs)
        {
            OSPadraoEntity osPadraoEntity = new OSPadraoEntity();

            osPadraoEntity.TpStatusOS.ST_STATUS_OS = Convert.ToInt32(statusOs);
            osPadraoEntity.Tecnico.CD_TECNICO = codTecnico;

            DataTableReader dataTableReader = new OSPadraoData().ObterListaOs(osPadraoEntity, null, null).CreateDataReader();

            try
            {
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (Convert.ToInt64(dataTableReader["ID_OS"]) != osPadraoEntity.ID_OS)
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

        public bool VerificarExisteVisitaDataHora(string codTecnico, int idVisita, string dataVisita, string horaInicio, string horaFim)
        {
            VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();
            visitaPadraoEntity.ID_VISITA = idVisita;
            visitaPadraoEntity.Tecnico.CD_TECNICO = codTecnico;
            visitaPadraoEntity.DT_DATA_VISITA = Convert.ToDateTime(dataVisita);
            visitaPadraoEntity.HR_INICIO = horaInicio;
            visitaPadraoEntity.HR_FIM = horaFim;

            return ControlesUtility.Utilidade.VerificarExisteVisitaDataHoraSemCompararVisitaInformada(visitaPadraoEntity);
        }

        public bool VerificarExisteOSDataHora(string codTecnico, string dataOs, string horaInicio, string horaFim)
        {
            OSPadraoEntity osPadraoEntity = new OSPadraoEntity();
            osPadraoEntity.Tecnico.CD_TECNICO = codTecnico;
            osPadraoEntity.DT_DATA_OS = Convert.ToDateTime(dataOs);
            osPadraoEntity.HR_INICIO = horaInicio;
            osPadraoEntity.HR_FIM = horaFim;

            return ControlesUtility.Utilidade.VerificarExisteOSDataHora(osPadraoEntity);
        }

        public VisitaPadrao Obter(string idKey)
        {
            VisitaPadrao visitaPadrao = null;

            try
            {
                VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity
                {
                    ID_VISITA = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey))
                };

                DataTableReader dataTableReader = new VisitaPadraoData().ObterLista(visitaPadraoEntity, null, null).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        visitaPadrao = new VisitaPadrao
                        {
                            ID_VISITA = Convert.ToInt64("0" + dataTableReader["ID_VISITA"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_VISITA"].ToString()),
                            DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("dd/MM/yyyy"),
                            DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString(),
                            DS_RESPONSAVEL = dataTableReader["DS_RESPONSAVEL"].ToString(),
                            EMAIL = dataTableReader["EMAIL"].ToString(),
                            HR_INICIO = dataTableReader["HR_INICIO"].ToString(),
                            HR_FIM = dataTableReader["HR_FIM"].ToString(),
                            Origem = dataTableReader["Origem"].ToString(),

                            TpStatusVisita = new TpStatusVisitaPadraoEntity
                            {
                                ST_STATUS_VISITA = Convert.ToInt32("0" + dataTableReader["ST_STATUS_VISITA"].ToString()),
                                DS_STATUS_VISITA = dataTableReader["DS_STATUS_VISITA"].ToString()
                            },

                            TpMotivoVisita = new TpMotivoVisitaPadraoEntity
                            {
                                CD_MOTIVO_VISITA = Convert.ToInt32("0" + dataTableReader["CD_MOTIVO_VISITA"].ToString()),
                                DS_MOTIVO_VISITA = dataTableReader["DS_MOTIVO_VISITA"].ToString()
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

                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return visitaPadrao;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public VisitaPadrao ObterVisita(Int64 ID_Visita)
        {
            VisitaPadrao visitaPadrao = null;

            try
            {
                VisitaPadraoEntity visitaPadraoEntity = new VisitaPadraoEntity();

                visitaPadraoEntity.ID_VISITA = ID_Visita;

                DataTableReader dataTableReader = new VisitaPadraoData().ObterVisitaById(visitaPadraoEntity, null, null).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        visitaPadrao = new VisitaPadrao
                        {
                            ID_VISITA = Convert.ToInt64("0" + dataTableReader["ID_VISITA"].ToString()),
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_VISITA"].ToString()),
                            DT_DATA_VISITA = Convert.ToDateTime(dataTableReader["DT_DATA_VISITA"]).ToString("dd/MM/yyyy"),
                            DS_OBSERVACAO = dataTableReader["DS_OBSERVACAO"].ToString(),
                            DS_RESPONSAVEL = dataTableReader["DS_RESPONSAVEL"].ToString(),
                            EMAIL = dataTableReader["EMAIL"].ToString(),
                            HR_INICIO = dataTableReader["HR_INICIO"].ToString(),
                            HR_FIM = dataTableReader["HR_FIM"].ToString(),

                            TpStatusVisita = new TpStatusVisitaPadraoEntity
                            {
                                ST_STATUS_VISITA = Convert.ToInt32("0" + dataTableReader["ST_STATUS_VISITA"].ToString()),
                                DS_STATUS_VISITA = dataTableReader["DS_STATUS_VISITA"].ToString()
                            },

                            TpMotivoVisita = new TpMotivoVisitaPadraoEntity
                            {
                                CD_MOTIVO_VISITA = Convert.ToInt32("0" + dataTableReader["CD_MOTIVO_VISITA"].ToString()),
                                DS_MOTIVO_VISITA = dataTableReader["DS_MOTIVO_VISITA"].ToString()
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

                        };
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                return visitaPadrao;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
        }

        public bool ValidarDataHoraVisita(VisitaPadrao visita)
        {
            var inicio = visita.HR_INICIO.Replace(":", "");

            var fim = "0000";

            if (visita.HR_FIM == null)
            {
                fim = "2359";
            }
            else
            {
                fim = visita.HR_FIM.Replace(":", "");
            }


            VisitaPadraoEntity visitaEntity = new VisitaPadraoEntity();

            if (visita.CodigoTecnico == null)
            {
                visitaEntity.Tecnico.CD_TECNICO = ObterCod_Tecnico(visita.ID_VISITA);
            }
            else
            {
                visitaEntity.Tecnico.CD_TECNICO = visita.CodigoTecnico;
            }


            var visitaExistente = new VisitaPadraoData().ObterListaVisitaSincHoras(visitaEntity).ToList();

            visitaExistente = visitaExistente.Where(x => x.DT_DATA_VISITA.Day == Convert.ToDateTime(visita.DT_DATA_VISITA).Day
                                            && x.DT_DATA_VISITA.Month == Convert.ToDateTime(visita.DT_DATA_VISITA).Month
                                            && x.DT_DATA_VISITA.Year == Convert.ToDateTime(visita.DT_DATA_VISITA).Year
                                            && x.TpStatusVisita.ST_STATUS_VISITA != 5
                                            && x.ID_VISITA != visita.ID_VISITA).ToList();

            if (visitaExistente?.Count > 0)
            {
                foreach (var existe in visitaExistente)
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
                    if (visita.HR_FIM != null)
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

        public string ObterCod_Tecnico(Int64 ID_VISITA)
        {
            VisitaPadraoEntity visita = new VisitaPadraoEntity();
            visita.ID_VISITA = ID_VISITA;

            IList<VisitaPadraoEntity> listaVISITA = new List<VisitaPadraoEntity>();

            listaVISITA = new VisitaPadraoData().ObterListaVisita(visita);

            return listaVISITA[0].Tecnico.CD_TECNICO;
        }

        #endregion
    }
}