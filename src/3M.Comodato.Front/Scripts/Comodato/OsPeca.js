jQuery(document).ready(function () {

    OcultarCampo($('#validaid_item_pedido_Peca'));
    OcultarValidacoesPeca();

    var ID_OS = $("#ID_OS").val();

    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0 || (typeof ID_OS === "undefined")) {
        return;
    }

    carregarGridMVCPeca();
});

function carregarGridMVCPeca() {
    var ID_OS = $("#ID_OS").val();
    var VISUALIZACAO_OS = $("#visualizarOS").val();
    var URL = URLObterListaPecaOS + "?ID_OS=" + ID_OS + "&visualizarOS=" + VISUALIZACAO_OS;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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
                $('#gridmvcPecaOS').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}

$('#btnNovaPeca').click(function () {
    var ST_STATUS_OS = $("#TpStatusOS_ST_STATUS_OS").val();

    if (ST_STATUS_OS == "" || ST_STATUS_OS == "0" || ST_STATUS_OS == 0) {
        ST_STATUS_OS = "0";
    }

    if (parseInt(ST_STATUS_OS) != statusAberta) {
        Alerta("Aviso", "Só é permitido adicionar <strong>Nova Peça</strong> para OS com status <strong>ABERTA</strong>!");
        return false;
    }

    carregarComboPecaOS($("#pecaOS_peca_CD_PECA"));

    $("#pecaOS_ID_PECA_OS").val(0);
    $("#pecaOS_CD_TP_ESTOQUE_CLI_TEC").val(statusTpEstoqueUtilizadoIntermediario);
    $("#pecaOS_peca_QTD_ESTOQUE").val(0);
    $("#pecaOS_QT_PECA").val('');
    $("#pecaOS_peca_TX_UNIDADE").val('');
    $("#pecaOS_DS_OBSERVACAO").val('');

    OcultarCampo($('#validaid_item_pedido_Peca'));
    OcultarValidacoesPeca();
});

function OcultarValidacoesPeca() {
    OcultarCampo($('#validaQTPECA_Peca'));
    OcultarCampo($('#validaQTPECARange_Peca'));
    OcultarCampo($('#validaQTPECARangeMax_Peca'));
}

$("#pecaOS_QT_PECA").blur(function () {
    OcultarValidacoesPeca();

    ValidarQuantidadeInformada();
});

function ValidarQuantidadeInformada() {

    const QT_PECA = parseFloat($("#pecaOS_QT_PECA").val());
    const QTD_ESTOQUE = parseFloat($("#pecaOS_peca_QTD_ESTOQUE").val());

    if (QT_PECA > QTD_ESTOQUE) {
        Alerta("Aviso", "Quantidade informada não pode ser superior a quantidade em estoque.");
        $("#pecaOS_QT_PECA").val(0);
        return;
    }
}

$("#pecaOS_QT_PECA").keypress(function () {
    OcultarValidacoesPeca();
});

$("#pecaOS_peca_CD_PECA").change(function () {
    CarregarEstoquePeca();
});

$("#pecaOS_CD_TP_ESTOQUE_CLI_TEC").change(function () {
    CarregarEstoquePeca();
});

function CarregarEstoquePeca() {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_PECA = $("#pecaOS_peca_CD_PECA option:selected").val();
    var CD_TP_ESTOQUE_CLI_TEC = $("#pecaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val();
    var CD_MODELO = $('#GRUPO_MODELO_ATIVO').val();

    OcultarCampo($('#validaid_item_pedido_Peca'));

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        $("#pecaOS_peca_QTD_ESTOQUE").val(0);
        $("#pecaOS_peca_TX_UNIDADE").val('');
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLAPI;
    if (CD_TP_ESTOQUE_CLI_TEC == 'C') {
        URL = URL + "EstoquePecaAPI/ObterCliente?CD_CLIENTE=" + $("#cliente_CD_CLIENTE").val() + "&CD_PECA=" + CD_PECA + "&CD_MODELO=" + CD_MODELO;
    }
    else {
        URL = URL + "EstoquePecaAPI/ObterTecnico?CD_TECNICO=" + CD_TECNICO + "&CD_PECA=" + CD_PECA + "&CD_MODELO=" + CD_MODELO;
    }
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.estoquePeca != null) {
                $("#pecaOS_peca_QTD_ESTOQUE").val(res.estoquePeca.QT_PECA_ATUAL);
                $("#pecaOS_peca_TX_UNIDADE").val(res.estoquePeca.peca.TX_UNIDADE);
                $("#pecaOS_QTD_RECEBIDA_NAO_APROVADA").val(res.estoquePeca.QTD_RECEBIDA_NAO_APROVADA);
                
                if (res.estoquePeca.peca.TX_UNIDADE == 'MT') {
                    $('#pecaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 3, allowZero: false });
                    casasDecimais = 3;
                }
                else {
                    $('#pecaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
                    casasDecimais = 0;
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function carregarComboPecaOS(Obj) {
    var Estoque = $('#pecaOS_CD_TP_ESTOQUE_CLI_TEC').val();
    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_GRUPO_MODELO = $('#GRUPO_MODELO_ATIVO').val();

    if (Estoque == 'C') {
        URL = URLAPI + "EstoquePecaAPI/ObterPecasCliente?CD_CLIENTE=" + CD_CLIENTE + "&CD_GRUPO_MODELO=" + CD_GRUPO_MODELO;
    } else if (Estoque == 'T') {
        URL = URLAPI + "EstoquePecaAPI/ObterPecasTecnico?CD_TECNICO=" + CD_TECNICO + "&CD_GRUPO_MODELO=" + CD_GRUPO_MODELO;
    }
    else 
        URL = URLAPI + "PlanoZeroAPI/ObterListaModelo?CD_ATIVO_FIXO=" + CD_ATIVO_FIXO;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            if (res.PlanoZero != null) {
                LoadPecasOS(Obj, res.PlanoZero);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadPecasOS(Obj, listaPecas) {
    LimparCombo(Obj);

    for (i = 0; i < listaPecas.length; i++) {
        MontarCombo(Obj, listaPecas[i].peca.CD_PECA, listaPecas[i].peca.DS_PECA);
    }
}


$('#pecaOS_CD_TP_ESTOQUE_CLI_TEC').change(function () {
    var ST_STATUS_OS = $("#TpStatusOS_ST_STATUS_OS").val();

    if (ST_STATUS_OS == "" || ST_STATUS_OS == "0" || ST_STATUS_OS == 0) {
        ST_STATUS_OS = "0";
    }

    if (parseInt(ST_STATUS_OS) != statusAberta) {
        Alerta("Aviso", "Só é permitido adicionar <strong>Nova Peça</strong> para OS com status <strong>ABERTA</strong>!");
        return false;
    }

    carregarComboPecaOS($("#pecaOS_peca_CD_PECA"));

    $("#pecaOS_ID_PECA_OS").val(0);$("#pecaOS_peca_QTD_ESTOQUE").val(0);
    $("#pecaOS_QT_PECA").val('');
    $("#pecaOS_peca_TX_UNIDADE").val('');
    $("#pecaOS_DS_OBSERVACAO").val('');

    OcultarCampo($('#validaid_item_pedido_Peca'));
    OcultarValidacoesPeca();
});

function ValidarPecaClienteOuTecnico() {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_PECA = $("#pecaOS_peca_CD_PECA option:selected").val();
    var Estoque = $('#pecaOS_CD_TP_ESTOQUE_CLI_TEC').val();
    var CD_MODELO = $('#GRUPO_MODELO_ATIVO').val();

    var URL = URLAPI;
    if (Estoque == 'C') {
        URL = URL + "EstoquePecaAPI/ObterCliente?CD_CLIENTE=" + $("#cliente_CD_CLIENTE").val() + "&CD_PECA=" + CD_PECA + "&CD_MODELO=" + CD_MODELO;
    }
    else {
        URL = URL + "EstoquePecaAPI/ObterTecnico?CD_TECNICO=" + CD_TECNICO + "&CD_PECA=" + CD_PECA + "&CD_MODELO=" + CD_MODELO;
    }
    //var token = sessionStorage.getItem("token");
    if (Estoque == 'T') {
        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            async: false,
            contentType: "application/json",
            data: { CD_TECNICO: CD_TECNICO, CD_PECA: CD_PECA, CD_MODELO: CD_MODELO },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                if (res == true)
                    return true;
                else return false;
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO");
            }

        });
    } else if (Estoque == 'C') {
        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            async: false,
            contentType: "application/json",
            data: { CD_CLIENTE: CD_CLIENTE, CD_PECA: CD_PECA, CD_MODELO: CD_MODELO },
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                if (res == true)
                    return true;
                else return false;
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO");
            }

        });
    }

}

$('#btnSalvarPecaModal').click(function () {

    var URL = URLAPI + "PecaOSAPI/Incluir";
    var ID_PECA_OS = $("#pecaOS_ID_PECA_OS").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_PECA = $("#pecaOS_peca_CD_PECA option:selected").val();
    var QT_PECA;

    if (casasDecimais > 0)
        QT_PECA = $("#pecaOS_QT_PECA").maskMoney('unmasked')[0];
    else
        QT_PECA = $("#pecaOS_QT_PECA").val();

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        ExibirCampo($('#validaid_item_pedido_Peca'));
        return false;
    }
    else if (QT_PECA == "" || QT_PECA == "0" || QT_PECA == 0) {
        ExibirCampo($('#validaQTPECA_Peca'));
        return false;
    }
    else if (parseFloat(QT_PECA) <= 0) {
        ExibirCampo($('#validaQTPECARange_Peca'));
        return false;
    }
    else if (parseFloat(QT_PECA) > 999999999) {
        ExibirCampo($('#validaQTPECARangeMax_Peca'));
        return false;
    }

    var pecaOSDetalhamentoEntity = {
        ID_PECA_OS: ID_PECA_OS,
        OS: {
            ID_OS: $("#ID_OS").val(),
        },
        peca: {
            CD_PECA: CD_PECA,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        QT_PECA_Formatado: QT_PECA,
        CD_TP_ESTOQUE_CLI_TEC: $("#pecaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val(),
        nidUsuarioAtualizacao: nidUsuario,
        TOKEN: ObterPrefixoTokenRegistro(nidUsuario),
        DS_OBSERVACAO: $("#pecaOS_DS_OBSERVACAO").val()
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(pecaOSDetalhamentoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != '' || res.MENSAGEM != '')
                Alerta("Aviso", res.MENSAGEM);
            else {
                Alerta("Aviso", MensagemGravacaoSucesso);
                carregarGridMVCPeca();

            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
    
});

function ExcluirPecaOS(ID_PECA_OS) {
    ConfirmarSimNao('Aviso', 'Confirma a <strong>EXCLUSÃO</strong> da Peça?', 'ExcluirPecaOSConfirmada(' + ID_PECA_OS + ')');
}

function ExcluirPecaOSConfirmada(ID_PECA_OS) {
    var URL = URLAPI + "PecaOSAPI/Excluir";
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (ID_PECA_OS == "" || ID_PECA_OS == "0" || ID_PECA_OS == 0) {
        Alerta("Aviso", "Peça inválida ou não informada!");
        return;
    }
    //var token = sessionStorage.getItem("token");
    var pecaOSDetalhamentoEntity = {
        ID_PECA_OS: ID_PECA_OS,
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        data: JSON.stringify(pecaOSDetalhamentoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemExclusaoSucesso);
            carregarGridMVCPeca();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}