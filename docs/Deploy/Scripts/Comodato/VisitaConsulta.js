jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#DT_DATA_ABERTURA_INICIO').mask('00/00/0000');
    $('#DT_DATA_ABERTURA_FIM').mask('00/00/0000');

    popularComboClientes();

    if (nidPerfil == 8) //Cliente
    {
        $('#divTecnicoEmpresa').hide()
        $('#divOS').hide()
        $('#divStatusVisita').hide()
        //$("#cliente_CD_CLIENTE").trigger('change');
    }
    else {
        popularComboTecnico();
        carregarComboTpStatusVisitaOS();
        popularComboOSVisita();
    }

});

function SettingRateSize(divRateYo, divRateValue) {
    if (window.matchMedia('(max-width: 768px)').matches) {
        $(divRateYo).rateYo("option", "starWidth", "30px");
        $(divRateYo).rateYo("option", "spacing", "5px");
        $(divRateValue).css('font-size', '1.6em');
    }
    else {
        $(divRateYo).rateYo("option", "starWidth", "80px");
        $(divRateYo).rateYo("option", "spacing", "30px");
        $(divRateValue).css('font-size', '3.6em');
    }
}

$('#DT_DATA_ABERTURA-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$("#cliente_CD_CLIENTE").change(function () {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (nidPerfil != 8) { //Cliente
        popularComboTecnico();
        popularComboOSVisita();
    }

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        carregarComboAtivos();
    } else {
        carregarComboAtivosCliente();

        ClienteAPI.carregarKatCliente(CD_CLIENTE)
            .done(function (kat) {
                AlterarInformacoesKatCliente(kat)
            })
            .catch(function (e) {
                //console.log("Não foi possível carregar o KAT", e)
            })
    }
});

function AlterarInformacoesKatCliente(kat) {
    if (kat != undefined) {
        $('#katTotal').text(kat.Periodos)
        $('#katUtilizado').text(kat.QtdPeriodosRealizados.toFixed(2))
        var diferenca = kat.Periodos - kat.QtdPeriodosRealizados;
        $('#katDisponivel').text(diferenca.toFixed(2))
        if (kat.Periodos > 0) {
            var percentual = 100 - ((kat.QtdPeriodosRealizados.toFixed(2) * 100) / kat.Periodos)
            $('#katPercentual').text(percentual.toFixed(2)+'%')
        }
        else
            $('#katPercentual').text('0%')
    }
    else {
        $('#katTotal').text('')
        $('#katUtilizado').text('')
        $('#katDisponivel').text('')
        $('#katPercentual').text('')
    }
}

$("#tecnico_CD_TECNICO").change(function () {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        $("#tecnico_empresa_NM_Empresa").val('');
    } else {
        var URL = URLAPI + "TecnicoAPI/Obter?CD_TECNICO=" + CD_TECNICO;

        $.ajax({
            type: 'GET',
            url: URL,
            dataType: "json",
            cache: false,
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
                if (res.tecnico != null) {
                    $("#tecnico_empresa_NM_Empresa").val(res.tecnico.empresa.NM_Empresa);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }
        });
    }
});

$('#btnLimpar').click(function () {
    if (totalClientes > 1)
        $('#cliente_CD_CLIENTE').val(null).trigger('change');

    //$('#ativoFixo_CD_ATIVO_FIXO').val(null).trigger('change');
    $('#tecnico_CD_TECNICO').val(null).trigger('change');
    $('#tecnico_empresa_NM_Empresa').val('');
    $('#OS_ID_OS').val(null).trigger('change');
    $('#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS').val(null);
    //$('#DT_DATA_ABERTURA_INICIO').val(periodoINICIAL);
    //$('#DT_DATA_ABERTURA_FIM').val(periodoFINAL);
    carregarGridMVC();
});

$('#btnFiltrar').click(function () {
    carregarGridMVC();
});

function Confirmar(ID_VISITA) {
    var codigoPesquisa = ObterCodigoPequisa(ID_VISITA);

    if (codigoPesquisa > 0) {
        ExibirModalPesquisa(codigoPesquisa, ID_VISITA);
    }
    else {
        ExibirModalAvaliacao(ID_VISITA);
        SettingRateSize($("#ratingAvaliacaoVisita"), $('#notaAvaliacaoVisita'));
    }
}

function ObterCodigoPequisa(ID_VISITA) {
    var codigoPesquisa = 0;
    $.ajax({
        type: 'GET',
        url: URLAPI + 'AnaliseSatisfacaoAPI/ObterCodigoPesquisaAtiva?codigoVisita=' + ID_VISITA,
        contentType: 'application/json;',
        dataType: 'json',
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (response) {
            $("#loader").css("display", "none");
            var data = JSON.parse(response.PESQUISA_SATISF);
            codigoPesquisa = data.ID_PESQUISA_SATISF;
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

    return codigoPesquisa;
}

function ExibirModalPesquisa(idPesquisaSatisfacao, ID_VISITA) {
    $.ajax({
        type: 'POST',
        url: URL_PESQUISA + '?idPesquisaSatisfacao=' + idPesquisaSatisfacao + '&idVisita=' + ID_VISITA,
        contentType: 'application/json;',
        dataType: 'html',
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (response) {
            $("#loader").css("display", "none");
            $('#modalDinamico').html(response);

            $("#ratingAvaliacaoQuestionario").rateYo({
                onChange: function (rating) {
                    var ratingValue = parseFloat(rating).toFixed(2);
                    $('#notaAvaliacaoQuestionario').text(ratingValue);
                }
            });
            SettingRateSize($("#ratingAvaliacaoQuestionario"), $('#notaAvaliacaoQuestionario'));

            $('#notaAvaliacaoQuestionario').text('0.00');
            $("#ratingAvaliacaoQuestionario").rateYo("option", "rating", 0.00);
            $('#PesquisaSatisfacaoModal').modal('show');
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function ExibirModalAvaliacao(ID_VISITA) {
    $("#SatisfacaoResposta_Visita_ID_VISITA").val(ID_VISITA);

    $('#notaAvaliacaoVisita').text('0.00');
    $("#ratingAvaliacaoVisita").rateYo({
        onChange: function (rating) {
            var ratingValue = parseFloat(rating).toFixed(2);
            $('#notaAvaliacaoVisita').text(ratingValue);
        }
    });
    SettingRateSize($("#ratingAvaliacaoVisita"), $('#notaAvaliacaoVisita'));

    $('#notaAvaliacaoVisita').text('0.00');
    $("#ratingAvaliacaoVisita").rateYo("option", "rating", parseFloat(0).toFixed(2));
    $("#SatisfacaoResposta_Justificativa").val('');
    $('#AvaliacaoVisitaModal').modal('show');
}

function SalvarAvaliacaoVisita() {
    var URL = URLAPI + "AnaliseSatisfacaoAPI/Incluir";

    var ID_VISITA = $("#SatisfacaoResposta_Visita_ID_VISITA").val();
    var NR_GRAU_AVALIACAO = 0;
    NR_GRAU_AVALIACAO = $("#ratingAvaliacaoVisita").rateYo("rating");

    var respostaEntity = new Object();
    respostaEntity.SatisfacaoPesquisa = new Object();
    respostaEntity.Visita = new Object();
    respostaEntity.Visita.ID_VISITA = ID_VISITA;

    respostaEntity.NM_NOTA_PESQ = NR_GRAU_AVALIACAO;
    respostaEntity.DS_JUSTIFICATIVA = $("#SatisfacaoResposta_Justificativa").val();
    respostaEntity.DS_RESPOSTA1 = '';
    respostaEntity.DS_RESPOSTA2 = '';
    respostaEntity.DS_RESPOSTA3 = '';
    respostaEntity.DS_RESPOSTA4 = '';
    respostaEntity.DS_RESPOSTA5 = '';
    respostaEntity.nidUsuarioAtualizacao = nidUsuario;

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(respostaEntity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemGravacaoSucesso);
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function SalvarPesquisaSatisfacao() {

    var respostaEntity = new Object();
    respostaEntity.SatisfacaoPesquisa = new Object();
    respostaEntity.SatisfacaoPesquisa.ID_PESQUISA_SATISF = $('#Pesquisa_IdPesquisa').val();

    respostaEntity.Visita = new Object();
    respostaEntity.Visita.ID_VISITA = $('#Visita_ID_VISITA').val();

    respostaEntity.DS_NOME_RESPONDEDOR = $('#NomeRespondedor').val();
    respostaEntity.NM_NOTA_PESQ = $('#notaAvaliacaoQuestionario').text();
    respostaEntity.DS_JUSTIFICATIVA = $('#Justificativa').val();
    respostaEntity.DS_RESPOSTA1 = $('#RespostaPesquisa1').val();
    respostaEntity.DS_RESPOSTA2 = $('#RespostaPesquisa2').val();
    respostaEntity.DS_RESPOSTA3 = $('#RespostaPesquisa3').val();
    respostaEntity.DS_RESPOSTA4 = $('#RespostaPesquisa4').val();
    respostaEntity.DS_RESPOSTA5 = $('#RespostaPesquisa5').val();
    respostaEntity.nidUsuarioAtualizacao = nidUsuario;

    $.ajax({
        type: 'POST',
        url: URLAPI + "AnaliseSatisfacaoAPI/Incluir",
        dataType: "json",
        data: JSON.stringify(respostaEntity),
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemGravacaoSucesso);
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function Desfazer(ID_VISITA, NR_DIAS_CONFIRMADO) {
    if (NR_DIAS_CONFIRMADO > 3)
        Alerta("Aviso", "Só é permitido <strong>desfazer visitas com no máximo 3 dias</strong> de tolerância após sua confirmação!");
    else
        ConfirmarSimNao('Aviso', 'Desfazer Confirmação da Visita?', 'DesfazerConfirmada(' + ID_VISITA + ')');
}

function DesfazerConfirmada(ID_VISITA) {
    var URL = URLAPI + "AnaliseSatisfacaoAPI/DesfazerByVisita?ID_VISITA=" + ID_VISITA + "&nidUsuarioAtualizacao=" + nidUsuario;
    //var codigoPesquisa = ObterCodigoPesquisaResposta(ID_VISITA);
    //if (codigoPesquisa > 0) {
    $.ajax({
        type: 'POST',
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
            Alerta("Aviso", "Confirmação de Visita desfeita com sucesso!");
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
    //}
}

//function carregarComboTecnico() {
//    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
//    var URL = URLAPI;
//    var data = {}

//    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
//        URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
//    }
//    else {
//        URL = URLAPI + "TecnicoXClienteAPI/ObterLista"; //?CD_CLIENTE=" + CD_CLIENTE;

//        data = {
//            cliente: {
//                CD_CLIENTE: CD_CLIENTE,
//            }
//        };
//    }

//    data.campos = ["CD_TECNICO", "NM_TECNICO"];

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        dataType: "json",
//        cache: false,
//        contentType: "application/json",
//        //headers: { "Authorization": "Basic " + localStorage.token },
//        data: data,
//        beforeSend: function () {
//            $("#loader").css("display", "block");
//        },
//        complete: function () {
//            $("#loader").css("display", "none");
//        },
//        success: function (res) {
//            $("#loader").css("display", "none");
//            var tecnicos = {}
//            if (res.tecnicos != null) {
//                tecnicos = res.tecnicos

//                if (typeof (res.tecnicos) === 'string') {
//                    tecnicos = JSON.parse(res.tecnicos)
//                }


//                LoadTecnicosCliente(tecnicos);
//            }
//        },
//        error: function (res) {
//            $("#loader").css("display", "none");
//            //atualizarPagina();
//            Alerta("ERRO", res.responseText);
//        }

//    });
//}

function LoadTecnicos(tecnicosJO) {
    $("#tecnico_empresa_NM_Empresa").val('');
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO)

    //for (i = 0; i < tecnicos.length; i++) {
    //    MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    //}
}

//function LoadTecnicosCliente(tecnicos) {
//    $("#tecnico_empresa_NM_Empresa").val('');
//    LimparCombo($("#tecnico_CD_TECNICO"));

//    let dados = tecnicos.map(function (tecnico, i) {
//        return {
//            id: tecnico.tecnico_CD_TECNICO,
//            text: tecnico.NM_TECNICO
//        }
//    });

//    $('#cliente_CD_CLIENTE').select2({
//        data: dados
//    });
//    //for (i = 0; i < tecnicos.length; i++) {
//    //    MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
//    //}
//}

//function carregarComboCliente() {
//    $('#cliente_CD_CLIENTE').select2({ placehoder: 'Carregando', disabled: true })

//    var camposNecessarios = ["CD_CLIENTE", "NM_CLIENTE", "EN_CIDADE", "EN_ESTADO"].join(",")
//    var listaClientes = undefined


//    if (nidPerfil == perfilCliente)
//        listaClientes = ClienteAPI.ObterListaPerfilClienteAsync(nidUsuario, camposNecessarios);
//    else
//        listaClientes = ClienteAPI.ObterListaPorUsuarioPerfilAsync(nidUsuario, camposNecessarios);

//    listaClientes.done(function (retorno) {
//        LoadClientes(JSON.parse(retorno.clientes))
//    })
//    .catch(function () {
//        console.error("Não foi possível carregar a lista de clientes ", listaClientes.mensagem)
//    })
//}

//function LoadClientes(clientes) {
//    if (nidPerfil == perfilCliente) {
//        if (clientes.length == 0)
//            LimparCombo($("#cliente_CD_CLIENTE"));
//        else
//            $("#cliente_CD_CLIENTE").empty();
//    }
//    else
//        LimparCombo($("#cliente_CD_CLIENTE"));


//    let dados = clientes.map(function (cliente, i) {
//        return {
//            id: cliente.CD_CLIENTE,
//            text: cliente.NM_CLIENTE + " (" + cliente.CD_CLIENTE + ") " + cliente.EN_CIDADE + " - " + cliente.EN_ESTADO
//        }
//    })
//    $('#cliente_CD_CLIENTE').select2({
//        data: dados,
//        disabled: false
//    })

//    totalClientes = clientes.length;
//}

function carregarComboTpStatusVisitaOS() {
    var URL = URLAPI + "TpStatusVisitaOSAPI/ObterLista";

    var tpStatusVisitaOSEntity = {
        FL_STATUS_OS: 'N'
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tpStatusVisitaOSEntity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tiposStatusVisitaOS != null) {
                LoadTpStatusVisitaOS(res.tiposStatusVisitaOS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadTpStatusVisitaOS(tiposStatusVisitaOS) {
    LimparCombo($("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS"));

    for (i = 0; i < tiposStatusVisitaOS.length; i++) {
        MontarCombo($("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS"), tiposStatusVisitaOS[i].ID_TP_STATUS_VISITA_OS, tiposStatusVisitaOS[i].DS_TP_STATUS_VISITA_OS);
    }
}

function carregarComboAtivos() {
    $('#ativoFixo_CD_ATIVO_FIXO').select2({ minimumInputLength: 3 });
    var URL = URLAPI + "AtivoAPI/ObterLista";

    var AtivoFixo = {
        FL_STATUS: true
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(AtivoFixo),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.listaAtivosFixos != null) {
                LoadAtivosFixos(res.listaAtivosFixos);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadAtivosFixos(listaAtivosClientesJO) {
    LimparCombo($("#ativoFixo_CD_ATIVO_FIXO"));

    var listaAtivosClientes = JSON.parse(listaAtivosClientesJO);

    for (i = 0; i < listaAtivosClientes.length; i++) {
        var DS_ATIVO_FIXO = listaAtivosClientes[i].CD_ATIVO_FIXO + ' - ' + listaAtivosClientes[i].modelo.DS_MODELO + ' - ' + listaAtivosClientes[i].TX_ANO_MAQUINA;
        MontarCombo($("#ativoFixo_CD_ATIVO_FIXO"), listaAtivosClientes[i].CD_ATIVO_FIXO, DS_ATIVO_FIXO);
    }
}

function carregarComboAtivosCliente() {
    $('#ativoFixo_CD_ATIVO_FIXO').select2();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        AlertaRedirect("Aviso", "Cliente inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLAPI + "AtivoAPI/ObterListaAtivoCliente";
    URL = URL + "?CD_Cliente=" + CD_CLIENTE + "&SomenteATIVOSsemDTDEVOLUCAO=true";

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
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
            if (res.listaAtivosClientes != null) {
                LoadAtivosCliente(res.listaAtivosClientes);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadAtivosCliente(listaAtivosClientesJO) {
    LimparCombo($("#ativoFixo_CD_ATIVO_FIXO"));

    var listaAtivosClientes = JSON.parse(listaAtivosClientesJO);

    for (i = 0; i < listaAtivosClientes.length; i++) {
        MontarCombo($("#ativoFixo_CD_ATIVO_FIXO"), listaAtivosClientes[i].CD_ATIVO_FIXO, listaAtivosClientes[i].DS_ATIVO_FIXO);
    }
}

//function carregarComboOSVisita() {
//    var osEntityFilter = new Object();

//    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
//    var URL = URLAPI + "OSAPI/ObterListaVisita";

//    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0)
//        CD_CLIENTE = 0;

//    URL = URL + "?CD_Cliente=" + CD_CLIENTE;

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        data: JSON.stringify(osEntityFilter),
//        dataType: "json",
//        cache: false,
//        async: true,
//        contentType: "application/json",
//        //headers: { "Authorization": "Basic " + localStorage.token },
//        //data: null,
//        beforeSend: function () {
//            $("#loader").css("display", "block");
//        },
//        complete: function () {
//            $("#loader").css("display", "none");
//        },
//        success: function (res) {
//            $("#loader").css("display", "none");
//            if (res.preenchimentoOS != null) {
//                LoadOSVisita(res.preenchimentoOS);
//            }
//        },
//        error: function (res) {
//            //atualizarPagina();
//            $("#loader").css("display", "none");
//            Alerta("ERRO", JSON.parse(res.responseText).Message);
//        }

//    });

//}

//function LoadOSVisita(preenchimentoOSJO) {
//    LimparCombo($("#OS_ID_OS"));

//    var preenchimentoOS = JSON.parse(preenchimentoOSJO);

//    for (i = 0; i < preenchimentoOS.length; i++) {
//        MontarCombo($("#OS_ID_OS"), preenchimentoOS[i].ID_OS_Formatado, preenchimentoOS[i].ID_OS_Formatado);
//    }
//}

function carregarGridMVC() {
    //ExibirCampo($('#gridmvc'));
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    //var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var ID_OS = $("#OS_ID_OS option:selected").val();
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS").val();
    var DT_DATA_ABERTURA_INICIO = $('#DT_DATA_ABERTURA_INICIO').val();
    var DT_DATA_ABERTURA_FIM = $('#DT_DATA_ABERTURA_FIM').val();

    if ((CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) && (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0)) {
        Alerta("Aviso", "Por favor escolha um Cliente!");
        return;
    }

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        CD_CLIENTE = 0;
    }


    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0)
        ID_OS = 0;

    if (ST_TP_STATUS_VISITA_OS == "" || ST_TP_STATUS_VISITA_OS == "0" || ST_TP_STATUS_VISITA_OS == 0 || ST_TP_STATUS_VISITA_OS == null)
        ST_TP_STATUS_VISITA_OS = 0;

    //var URL = URLObterListaVisita + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_ATIVO_FIXO=" + CD_ATIVO_FIXO + "&CD_TECNICO=" + CD_TECNICO + "&ID_OS=" + ID_OS + "&ST_TP_STATUS_VISITA_OS=" + ST_TP_STATUS_VISITA_OS + "&DT_DATA_ABERTURA_INICIO=" + DT_DATA_ABERTURA_INICIO + "&DT_DATA_ABERTURA_FIM=" + DT_DATA_ABERTURA_FIM;

    //atribuirParametrosPaginacao("gridmvc", URLObterListaVisita, '{"CD_CLIENTE":"' + CD_CLIENTE + '", "CD_ATIVO_FIXO":"' + CD_ATIVO_FIXO + '", "CD_TECNICO":"' + CD_TECNICO + '", "ID_OS": "' + ID_OS + '", "ST_TP_STATUS_VISITA_OS": "' + ST_TP_STATUS_VISITA_OS + '", "DT_DATA_ABERTURA_INICIO": "' + DT_DATA_ABERTURA_INICIO + '", "DT_DATA_ABERTURA_FIM":"' + DT_DATA_ABERTURA_FIM + '"}');

    var URL = URLObterListaVisita + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_ATIVO_FIXO=&CD_TECNICO=" + CD_TECNICO + "&ID_OS=" + ID_OS + "&ST_TP_STATUS_VISITA_OS=" + ST_TP_STATUS_VISITA_OS + "&DT_DATA_ABERTURA_INICIO=" + DT_DATA_ABERTURA_INICIO + "&DT_DATA_ABERTURA_FIM=" + DT_DATA_ABERTURA_FIM;

    atribuirParametrosPaginacao("gridmvc", URLObterListaVisita, '{"CD_CLIENTE":"' + CD_CLIENTE + '", "CD_ATIVO_FIXO":"' + "" + '", "CD_TECNICO":"' + CD_TECNICO + '", "ID_OS": "' + ID_OS + '", "ST_TP_STATUS_VISITA_OS": "' + ST_TP_STATUS_VISITA_OS + '", "DT_DATA_ABERTURA_INICIO": "' + DT_DATA_ABERTURA_INICIO + '", "DT_DATA_ABERTURA_FIM":"' + DT_DATA_ABERTURA_FIM + '"}');

    $.ajax({
        type: 'GET',
        url: URL,
        //processData: true,
        dataType: "json",
        cache: false,
        //async: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.Status == "Success") {
                $('#gridmvc').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function popularComboClientes() {
    var URL = URLAPI + "ClienteAPI/";

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaComboPerfilCliente?nidUsuario=" + nidUsuario;
    }
    else {
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    }

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.clientes != null) {
                preencherComboClientes(res.clientes);
                var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
                if (CD_CLIENTE != "" && CD_CLIENTE != "0" && CD_CLIENTE != 0) {
                    carregarGridMVC();

                    ClienteAPI.carregarKatCliente(CD_CLIENTE)
                        .done(function (kat) {
                            AlterarInformacoesKatCliente(kat)
                        })
                        .catch(function (e) {
                            //console.log("Não foi possível carregar o KAT", e)
                        })
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function preencherComboClientes(clientes) {
    if (nidPerfil == perfilCliente)
        $("#cliente_CD_CLIENTE").empty();
    else
        LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

function popularComboTecnico() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var URL = URLAPI;

    if (null == CD_CLIENTE || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {

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
            //async: false,
            contentType: "application/json",
            data: JSON.stringify(tecnicoEntity),
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                if (res.tecnicos != null) {
                    preencherComboTecnicos(res.tecnicos);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", res.responseText);
            }

        });
    }
    else {
        URL = URLAPI + "TecnicoXClienteAPI/ObterLista";
        var tecnicoClienteEntity = {
            cliente: {
                CD_CLIENTE: CD_CLIENTE,
            }
        };

        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            contentType: "application/json",
            data: JSON.stringify(tecnicoClienteEntity),
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                if (res.tecnicos != null) {
                    preencherComboTecnicosCliente(res.tecnicos);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", res.responseText);
            }
        });
    }
}

function preencherComboTecnicos(tecnicosJO) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO + " (" + tecnicos[i].CD_TECNICO + ")");
    }
}

function preencherComboTecnicosCliente(tecnicos) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO + " (" + tecnicos[i].tecnico.CD_TECNICO + ")");
    }
}

function popularComboOSVisita() {
    var osEntityFilter = new Object();

    //var CD_CLIENTE = $("#ddlCliente").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var ID_VISITA = 0; //$("#ddlVisita option:selected").val();

    var URL = URLAPI + "OSAPI/ObterListaComboOS";

    if (CD_CLIENTE == null || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0 || CD_CLIENTE == undefined)
        CD_CLIENTE = 0;

    if (CD_TECNICO == null || CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0 || CD_TECNICO == undefined) {
        CD_TECNICO = "0";
    }

    if (CD_TECNICO == "0" && CD_CLIENTE == 0) {
        return; //não traz nenhuma OS
    }

    var DT_INICIAL = $('#DT_DATA_ABERTURA_INICIO').val();
    var parts = DT_INICIAL.split('/');
    var DT_DATA_VISITA = parts[2] + "-" + parts[1] + "-" + parts[0]; //Data no formato Universal

    if (ID_VISITA == null || ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0 || ID_VISITA == undefined)
        ID_VISITA = 0;

    DT_DATA_VISITA = '';
    URL = URL + "?CD_Cliente=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO + "&DT_DATA_VISITA=" + DT_DATA_VISITA + "&ID_VISITA=" + ID_VISITA;

    $.ajax({
        type: 'POST',
        url: URL,
        data: JSON.stringify(osEntityFilter),
        dataType: "json",
        cache: false,
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
            if (res.preenchimentoOS != null) {
                preencherOSVisita(res.preenchimentoOS);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

}

function preencherOSVisita(preenchimentoOSJO) {
    LimparCombo($("#OS_ID_OS"));

    var preenchimentoOS = JSON.parse(preenchimentoOSJO);

    for (i = 0; i < preenchimentoOS.length; i++) {
        MontarCombo($("#OS_ID_OS"), preenchimentoOS[i].ID_OS_Formatado, preenchimentoOS[i].ID_OS_Formatado);
    }
}

