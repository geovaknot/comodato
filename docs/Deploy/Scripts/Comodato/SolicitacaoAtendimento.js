$().ready(function () {
    $("#loader").css("display", "block");

    $('#ddlCliente').select2({
        //minimumInputLength: 3,
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlTecnico').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlAtivos').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    carregarComboCliente();
    carregarComboTecnico();
    carregarAtivosCliente($("#ddlCliente option:selected").val());

    OcultarCampo($('#validaClienteObrigatorio'));

    if (nidPerfil == perfilAdministrador3M) {
        PopularGridMvc();
    }
    $("#loader").css("display", "none");

    
});

function LimparFormulario() {
    OcultarCampo($('#validaClienteObrigatorio'));
    OcultarCampo($('#validaSolicitanteObrigatorio'));
    OcultarCampo($('#validaAtivoObrigatorio'));

    $('#labelNumeroSoclititacao').text('Solicitação #');
    $('#hidCodigoSolicitacao').val('');
    $('#UsuarioSolicitante').val('');
    $('#StatusSolicitacao').val(1).trigger('change');
    $('#ddlAtivos').val(null).trigger('change');
    $('#TipoAtendimento').val(1).trigger('change');
    $('#Observacao').val('');
    $('#ID_OS').val('');
}

function carregarComboTecnico() {
    //var CD_CLIENTE = $("#ddlCliente option:selected").val();
    var URL = URLAPI;

    //if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        URL = URLAPI + "TecnicoAPI/ObterListaAtivos";

        var tecnicoEntity = {};
        tecnicoEntity = {
            usuario: {
                nidUsuario: nidUsuario
            }
        };

        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            contentType: "application/json",
            //headers: { "Authorization": "Basic " + localStorage.token },
            //data: null,
            data: JSON.stringify(tecnicoEntity),
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                //$("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                if (res.tecnicos != null) {
                    PopularTecnicos(res.tecnicos);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });
    //}
    //else {
    //    URL = URLAPI + "TecnicoXClienteAPI/ObterLista"; //?CD_CLIENTE=" + CD_CLIENTE;

    //    var tecnicoClienteEntity = {
    //        cliente: {
    //            CD_CLIENTE: CD_CLIENTE,
    //        }
    //    };

    //    $.ajax({
    //        type: 'POST',
    //        url: URL,
    //        dataType: "json",
    //        cache: false,
    //        contentType: "application/json",
    //        //headers: { "Authorization": "Basic " + localStorage.token },
    //        data: JSON.stringify(tecnicoClienteEntity),
    //        beforeSend: function () {
    //            $("#loader").css("display", "block");
    //        },
    //        complete: function () {
    //            $("#loader").css("display", "none");
    //        },
    //        success: function (res) {
    //            $("#loader").css("display", "none");
    //            if (res.tecnicos != null) {
    //                PopularTecnicosCliente(res.tecnicos);
    //            }
    //        },
    //        error: function (res) {
    //            $("#loader").css("display", "none");
    //            //atualizarPagina();
    //            Alerta("ERRO", res.responseText);
    //        }

    //    });
    //}
}

function PopularTecnicos(tecnicosJO) {
    LimparCombo($("#ddlTecnico"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#ddlTecnico"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    }
}

function PopularTecnicosCliente(tecnicos) {

    LimparCombo($("#ddlTecnico"));

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#ddlTecnico"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
    }
}

function carregarComboCliente() {
    var URL = URLAPI + "ClienteAPI/";

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaPerfilCliente";
    }
    else {
        URL = URL + "ObterListaPorUsuarioPerfil";
    }

    var camposNecessarios = ["CD_CLIENTE", "NM_CLIENTE", "EN_CIDADE", "EN_ESTADO"].join(",");

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: {
            "nidUsuario": nidUsuario,
            "camposNecessarios": camposNecessarios
        },
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            //$("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");


            if (res.clientes != null) {
                var clientes = JSON.parse(res.clientes);

                PopularClientes(clientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function PopularClientes(clientes) {
    if (nidPerfil == perfilCliente) {
        if (clientes.length == 0)
            LimparCombo($("#ddlCliente"));
        else
            $("#ddlCliente").empty();
    }
    else
        LimparCombo($("#ddlCliente"));

    var dados = clientes.map(function (cliente, i) {
        return {
            id: cliente.CD_CLIENTE,
            text: cliente.NM_CLIENTE + " (" + cliente.CD_CLIENTE + ") " + cliente.EN_CIDADE + " - " + cliente.EN_ESTADO
        };
    });
    
    $('#ddlCliente').select2({
        data: dados
    });

    //for (i = 0; i < clientes.length; i++) {
    //    var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
    //    MontarCombo($("#ddlCliente"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    //}

    //totalClientes = clientes.length;
}

$("#ddlCliente").change(function () {
    carregarComboTecnico();
    carregarAtivosCliente($("#ddlCliente option:selected").val());
    carregarPeriodo();
});

$('#btnNovaSolicitacao').click(function () {
    var codigoCliente = $('#ddlCliente option:selected').val();
    if (codigoCliente == '' || codigoCliente == 0 && null == codigoCliente) {
        ExibirCampo($('#validaClienteObrigatorio'));
        return false;
    }
    
    var codigoSolicitacao = ObterNumeroNovaSolicitacao();

    LimparFormulario();
    ExibirCampo($('#btnEnviarSolicitacao'));
    //TravarCampos($('.form-control'), false);
    $("#hboolNovaSolicitacao").val("true");

    $('#DataSolicitacao').val($('#DataHoje').val());


    AbrirModal(codigoSolicitacao);
});

function AbrirModal(codigoSolicitacao) {
    if (nidPerfil == perfilTecnicoEmpresaTerceira) {
        $('#UsuarioSolicitante').prop('disabled', true);
        $('#ddlAtivos').prop('disabled', true);
        $('#TipoAtendimento').prop('disabled', true);
    }
    if (nidPerfil == perfilCliente) {
        $('#StatusSolicitacao').prop('disabled', true);
    }
    
    $('#labelNumeroSoclititacao').text('Solicitação #' + codigoSolicitacao);
    $('#hidCodigoSolicitacao').val(codigoSolicitacao);

    $('#SolicitacaoAtendimento').modal('show');
}

function ObterNumeroNovaSolicitacao() {
    var URL = URLAPI + "SolicitacaoAtendimentoAPI/ObterCodigoSolicitacao";
    var numeroSolicitacao = '';

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.SOLIC_ATEND != null) {
                numeroSolicitacao = JSON.parse(res.SOLIC_ATEND);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

    return numeroSolicitacao;
}

$('#btnEnviarSolicitacao').click(function () {

    if ($('#UsuarioSolicitante').val() == '') {
        ExibirCampo($('#validaSolicitanteObrigatorio'));
        return false;
    }
    OcultarCampo($('#validaSolicitanteObrigatorio'));

    if ($('#ddlAtivos option:selected').val() == '' || $('#ddlAtivos option:selected').val() == null) {
        ExibirCampo($('#validaAtivoObrigatorio'));
        return false;
    }
    OcultarCampo($('#validaAtivoObrigatorio'));

    var URL = URLAPI + "SolicitacaoAtendimentoAPI/InserirSolicitacao";

    //Se for alteração da Solicitação
    if ($('#hboolNovaSolicitacao').val() == "false") {
        URL = URLAPI + "SolicitacaoAtendimentoAPI/AlterarSolicitacao";
    }

    var entity = new Object();
    entity.ID_SOLICITA_ATENDIMENTO = $('#hidCodigoSolicitacao').val();
    entity.CLIENTE = new Object();
    entity.CLIENTE.CD_CLIENTE = $('#ddlCliente option:selected').val();
    entity.CLIENTE.NM_CLIENTE = $('#ddlCliente option:selected').text();

    entity.ID_USU_SOLICITANTE = nidUsuario;
    entity.DS_OBSERVACAO = $('#Observacao').val();

    entity.TipoAtendimento = new Object();
    entity.TipoAtendimento.CD_TIPO_ATENDIMENTO = $('#TipoAtendimento option:selected').val();
    entity.TipoAtendimento.DS_TIPO_ATENDIMENTO = $('#TipoAtendimento option:selected').text();  

    entity.AtivoFixo = new Object();
    entity.AtivoFixo.CD_ATIVO_FIXO = $('#ddlAtivos option:selected').val();
    entity.AtivoFixo.DS_ATIVO_FIXO = $('#ddlAtivos option:selected').text();
    

    entity.DS_CONTATO = $('#UsuarioSolicitante').val();

    entity.StatusAtendimento = new Object();
    entity.StatusAtendimento.ID_STATUS_ATENDIMENTO = $('#StatusSolicitacao option:selected').val();


    if (entity.StatusAtendimento.ID_STATUS_ATENDIMENTO == 2) //Concluído
    {
        if (isNaN($('#ID_OS').val()) || $('#ID_OS').val() == "" ) {
            ExibirCampo($('#validaOSObrigatorio'));
            return false;
        }
        else {
            entity.OS = new Object();
            entity.OS.ID_OS = $('#ID_OS').val();
        }
    }

    entity.nidUsuarioAtualizacao = nidUsuario;

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(entity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.SOLIC_ATEND != null) {
                Alerta("SUCESSO", JSON.parse(res.SOLIC_ATEND));
                PopularGridMvc();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
});

$('#btnFiltrar').click(function () {
    PopularGridMvc();
});

function PopularGridMvc() {
    var codigoCliente = $('#ddlCliente option:selected').val();

    var filtro = new Object();
    filtro.CLIENTE = new Object();
    if (codigoCliente != '' && codigoCliente != 0 && null != codigoCliente) {
        filtro.CLIENTE.CD_CLIENTE = codigoCliente;
    } else {
        codigoCliente = 0;
    }
    atribuirParametrosPaginacao("gridMvc", actionObterSolicitacoes, JSON.stringify(filtro));

    var tecnico = $('#ddlTecnico option:selected').val();
    if (tecnico == '' || tecnico == undefined || null == tecnico) {
        tecnico = 0;
    }

    if (codigoCliente == 0 && tecnico == 0 && nidPerfil != perfilAdministrador3M) {
        Alerta("Alerta", "Preencha algum filtro!");
        return false;
    }

    $.ajax({
        type: 'POST',
        url: actionObterSolicitacoes,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(filtro),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.Status == "Success") {
                $('#gridMvc').html(data.Html);
                $(".grid-mvc").gridmvc();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function carregarAtivosCliente(CD_CLIENTE) {
    if (CD_CLIENTE != "" && CD_CLIENTE != "0" && CD_CLIENTE != 0 && CD_CLIENTE != null) {
        {
            var url = URLAPI + '/AtivoAPI/ObterListaAtivoCliente?CD_Cliente=' + CD_CLIENTE + '&SomenteATIVOSsemDTDEVOLUCAO=true';
            $.ajax({
                type: 'POST',
                url: url,
                dataType: 'json',
                cache: false,
                async: false,
                contentType: "application/json",
                beforeSend: function () {
                    $("#loader").css("display", "block");
                },
                complete: function () {
                    $("#loader").css("display", "none");
                },
                success: function (res) {
                    $("#loader").css("display", "none");
                    if (res.listaAtivosClientes != null) {
                        PopularAtivos(res.listaAtivosClientes);
                    }
                },
                error: function (res) {
                    $("#loader").css("display", "none");
                    Alerta("ERRO", res.responseText);
                }
            });
        }
    }
}

function PopularAtivos(listaAtivosClientesJO) {
    LimparCombo($("#ddlAtivos"));
    var ativos = JSON.parse(listaAtivosClientesJO);

    for (i = 0; i < ativos.length; i++) {
        MontarCombo($("#ddlAtivos"), ativos[i].CD_ATIVO_FIXO, ativos[i].CD_ATIVO_FIXO + " - " + ativos[i].DS_MODELO);
    }
}

function Detalhar(codigoSolicitacao) {

    var filtro = new Object();
    filtro.ID_SOLICITA_ATENDIMENTO = codigoSolicitacao;

    var URL = URLAPI + "SolicitacaoAtendimentoAPI/Obter";

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(filtro),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.SOLIC_LIST != null) {
                LimparFormulario();

                //TravarCampos($('.form-control'), true);
                OcultarCampo($('#btnEnviarSolicitacao'));

                var atendimentos = JSON.parse(data.SOLIC_LIST);

                if (atendimentos.length > 0) {

                    $("#hboolNovaSolicitacao").val("false");

                    carregarAtivosCliente(atendimentos[0].CLIENTE.CD_CLIENTE);


                    $('#DataSolicitacao').val(FormatarSoData(atendimentos[0].DT_DATA_SOLICITACAO));
                    $('#hidCodigoSolicitacao').val(atendimentos[0].ID_SOLICITA_ATENDIMENTO);
                    $('#UsuarioSolicitante').val(atendimentos[0].DS_CONTATO);
                    $('#Observacao').val(atendimentos[0].DS_OBSERVACAO);
                    
                    $('#labelNumeroSoclititacao').text('Solicitação #' + atendimentos[0].ID_SOLICITA_ATENDIMENTO);
                    $('#TipoAtendimento').val(atendimentos[0].TipoAtendimento.ID_TIPO_ATENDIMENTO).trigger('change');
                    $('#StatusSolicitacao').val(atendimentos[0].StatusAtendimento.ID_STATUS_ATENDIMENTO).trigger('change');

                    $('#ddlAtivos').val(atendimentos[0].AtivoFixo.CD_ATIVO_FIXO).trigger('change');

                    $('#ID_OS').val(atendimentos[0].OS.ID_OS);
                    
                    if (codigoSolicitacao > 0) {
                        if (nidPerfil == perfilTecnico || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilAdministrador3M) {
                            if (atendimentos[0].StatusAtendimento.ID_STATUS_ATENDIMENTO == 1) {
                                ExibirCampo($('#btnEnviarSolicitacao'));
                                $('#ID_OS').removeAttr('disabled');
                            }
                        }
                    }

                    ClienteAPI.carregarKatCliente(atendimentos[0].CLIENTE.CD_CLIENTE)
                        .done(function (kat) {
                            $('#txtPeriodoRealizado').val(kat.QtdPeriodosRealizados.toFixed(2));
                            $('#txtPeriodo').val(kat.Periodos.toFixed(2));
                        })
                        .catch(function (e) {
                            //console.log("Não foi possível carregar o KAT", e)
                        });
                }

                AbrirModal(atendimentos[0].ID_SOLICITA_ATENDIMENTO);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function carregarPeriodo() {

    var cliente = $("#ddlCliente option:selected").val();
    if (cliente == null || cliente == undefined || cliente == "" || cliente == 0) {
        return false;
    } else {

        //var ClienteEntity = {
        //    CD_CLIENTE: $("#ddlCliente option:selected").val()
        //};

        //var url = URLAPI + "ClienteAPI/ObterLista";
        var url = URLAPI + "ClienteAPI/Obter?CD_CLIENTE=" + cliente;

        $.ajax({

            type: 'GET',
            url: url,
            dataType: 'json',
            cache: false,
            async: false,
            contentType: "application/json",
            //data: JSON.stringify(ClienteEntity),
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                $('#txtPeriodo').val(res.cliente.QT_PERIODO);
            },
            error: function (res) {
                $('#txtPeriodo').val('');
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }
        });
    }
}