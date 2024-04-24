jQuery(document).ready(function () {

    OcultarCampo($('#validaid_item_pedido_Pendencia'));
    OcultarValidacoesPendencia();

    var ID_OS = $("#ID_OS").val();

    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0 || (typeof ID_OS === "undefined")) {
        return;
    }

    carregarGridMVCPendencia();
});

function carregarGridMVCPendencia() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var ID_OS = $("#ID_OS").val();
    var VISUALIZACAO_OS = $("#visualizarOS").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    var URL = URLObterListaPendenciaOS + "?CD_CLIENTE=" + CD_CLIENTE + "&ID_OS=" + ID_OS + "&visualizarOS=" + VISUALIZACAO_OS + "&CD_TECNICO=" + CD_TECNICO;

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
                $('#gridmvcPendenciaOS').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

$('#btnNovaPendencia').click(function () {
    var ST_STATUS_OS = $("#TpStatusOS_ST_STATUS_OS").val();
    
    if (ST_STATUS_OS == "" || ST_STATUS_OS == "0" || ST_STATUS_OS == 0) {
        ST_STATUS_OS = "0";
    }

    if (parseInt(ST_STATUS_OS) != statusAberta) {
        Alerta("Aviso", "Só é permitido adicionar <strong>Nova Pendência</strong> para OS com status <strong>ABERTA</strong>!");
        return false;
    }

    carregarComboPeca($("#pendenciaOS_peca_CD_PECA"));

    
    $("#pendenciaOS_ID_PENDENCIA_OS").val(0);
    $("#pendenciaOS_ST_STATUS_PENDENCIA").val(statusTpPendenciaPendente);
    $("#pendenciaOS_ST_TP_PENDENCIA").val("O");
    $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").val(statusTpEstoqueUtilizadoIntermediario);
    $("#pendenciaOS_peca_QTD_ESTOQUE").val(0);
    $("#pendenciaOS_QT_PECA").val('');
    $("#pendenciaOS_peca_TX_UNIDADE").val('');
    $("#pendenciaOS_DS_DESCRICAO").val('');

    if ($("#pendenciaOS_ST_TP_PENDENCIA").val() == "P")
        ExibirCampo($('#tipoPeca'));
    else
        OcultarCampo($('#tipoPeca'));

    $("#pendenciaOS_OsPadrao_ID_OS").val($("#ID_OS").val());

    var ID_OS_Formatado = '#' + $("#ID_OS").val();
    $("#pendenciaOS_OsPadrao_ID_OS_Formatado").val(ID_OS_Formatado);

    const dataAtualFormatada = Geradatadehoje();
    $("#pendenciaOS_DT_ABERTURA").val(dataAtualFormatada);
    $("#pendenciaOS_OsPadrao_TpStatusOS_DS_STATUS_OS").val($("#TpStatusOS_DS_STATUS_OS").val());

    definirCorStatusOSPendencia(ST_STATUS_OS);

    OcultarCampo($('#validaid_item_pedido_Pendencia'));
    OcultarValidacoesPendencia();

    $("#pendenciaOS_peca_CD_PECA").prop("disabled", false);
    $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").prop("disabled", false);
    $("#pendenciaOS_QT_PECA").prop("disabled", false);

});

function definirCorStatusOSPendencia(ST_STATUS_OS) {
    var classColorStatus = "";

    if (ST_STATUS_OS == statusIniciar) {
        classColorStatus = "alert-info";
    }
    else if (ST_STATUS_OS == statusAberta) {
        classColorStatus = "alert-success";
    }
    else if (ST_STATUS_OS == statusFinalizada) {
        classColorStatus = "alert-primary";
    }
    else if (ST_STATUS_OS == statusCancelada) {
        classColorStatus = "alert-danger";
    }
    else {
        classColorStatus = "alert-secondary";
    }

    if (classColorStatus != "")
        classColorStatus = "font-weight-bold text-center col-12 alert " + classColorStatus;

    $('#pendenciaOS_OsPadrao_TpStatusOS_DS_STATUS_OS').removeClass();
    $('#pendenciaOS_OsPadrao_TpStatusOS_DS_STATUS_OS').addClass(classColorStatus);
}

$('#btnSalvarPendenciaModal').click(function () {
    var URL = URLAPI + "PendenciaOSAPI/";
    var ID_PENDENCIA_OS = $("#pendenciaOS_ID_PENDENCIA_OS").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_PECA = $("#pendenciaOS_peca_CD_PECA option:selected").val();
    var ST_TP_PENDENCIA = $('#pendenciaOS_ST_TP_PENDENCIA option:selected').val();
    var CD_TP_ESTOQUE_CLI_TEC = $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val();
    var QT_PECA;

    if (ID_PENDENCIA_OS == "" || ID_PENDENCIA_OS == "0" || ID_PENDENCIA_OS == 0) {
        URL = URL + "Incluir";
    }
    else {
        URL = URL + "Alterar";
    }

    if (ST_TP_PENDENCIA == 'P') {
        if (casasDecimais > 0)
            QT_PECA = $("#pendenciaOS_QT_PECA").maskMoney('unmasked')[0];
        else
            QT_PECA = $("#pendenciaOS_QT_PECA").val();

        if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
            ExibirCampo($('#validaid_item_pedido_Pendencia'));
            return false;
        }
        else if (QT_PECA == "" || QT_PECA == "0" || QT_PECA == 0) {
            ExibirCampo($('#validaQTPECA_Pendencia'));
            return false;
        }
        else if (parseFloat(QT_PECA) <= 0) {
            ExibirCampo($('#validaQTPECARange_Pendencia'));
            return false;
        }
        else if (parseFloat(QT_PECA) > 999999999) {
            ExibirCampo($('#validaQTPECARangeMax_Pendencia'));
            return false;
        }
    }
    else {
        CD_PECA = null;
        CD_TP_ESTOQUE_CLI_TEC = null;
        QT_PECA = null;
    }

    var pendenciaOSEntity = {
        ID_PENDENCIA_OS: ID_PENDENCIA_OS,
        os: {
            ID_OS: $("#pendenciaOS_OsPadrao_ID_OS").val(),
        },
        DT_ABERTURA_Formatado: $("#pendenciaOS_DT_ABERTURA").val(),
        DS_DESCRICAO: $("#pendenciaOS_DS_DESCRICAO").val(),
        peca: {
            CD_PECA: CD_PECA,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        QT_PECA_Formatado: QT_PECA,
        ST_STATUS_PENDENCIA: $("#pendenciaOS_ST_STATUS_PENDENCIA option:selected").val(),
        ST_TP_PENDENCIA: $("#pendenciaOS_ST_TP_PENDENCIA option:selected").val(),
        CD_TP_ESTOQUE_CLI_TEC: CD_TP_ESTOQUE_CLI_TEC,
        nidUsuarioAtualizacao: nidUsuario,
        TOKEN: ObterPrefixoTokenRegistro(nidUsuario)
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(pendenciaOSEntity),
        beforeSend: function (xhr) {
            $("#loader").css("display", "block");
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        complete: function () {
            
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemGravacaoSucesso);
            carregarGridMVCPendencia();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
});

function OcultarValidacoesPendencia() {
    OcultarCampo($('#validaQTPECA_Pendencia'));
    OcultarCampo($('#validaQTPECARange_Pendencia'));
    OcultarCampo($('#validaQTPECARangeMax_Pendencia'));
}

$('#pendenciaOS_ST_TP_PENDENCIA').change(function () {
    const tipoPendenciaPeca = 'P';

    var ST_TP_PENDENCIA = $('#pendenciaOS_ST_TP_PENDENCIA option:selected').val();

    if (ST_TP_PENDENCIA == tipoPendenciaPeca)
        ExibirCampo($('#tipoPeca'));
    else
        OcultarCampo($('#tipoPeca'));
});

$("#pendenciaOS_peca_CD_PECA").change(function () {
    CarregarEstoquePecaPendencia();
});

$("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").change(function () {
    CarregarEstoquePecaPendencia();
});

function CarregarEstoquePecaPendencia() {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_PECA = $("#pendenciaOS_peca_CD_PECA option:selected").val();
    var CD_TP_ESTOQUE_CLI_TEC = $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC option:selected").val();

    OcultarCampo($('#validaid_item_pedido_Pendencia'));

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        $("#pendenciaOS_peca_QTD_ESTOQUE").val(0);
        $("#pendenciaOS_peca_TX_UNIDADE").val('');
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLAPI + "EstoquePecaAPI/ObterTecnico?CD_TECNICO=" + CD_TECNICO + "&CD_PECA=" + CD_PECA;
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
                $("#pendenciaOS_peca_QTD_ESTOQUE").val(res.estoquePeca.QT_PECA_ATUAL)
                $("#pendenciaOS_peca_TX_UNIDADE").val(res.estoquePeca.peca.TX_UNIDADE);

                if (res.estoquePeca.peca.TX_UNIDADE == 'MT') {
                    $('#pendenciaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 3, allowZero: false });
                    casasDecimais = 3;
                }
                else {
                    $('#pendenciaOS_QT_PECA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
                    casasDecimais = 0;
                }

            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

    if (CD_TP_ESTOQUE_CLI_TEC != statusTpEstoqueUtilizadoIntermediario) {
        $("#pendenciaOS_peca_QTD_ESTOQUE").val(0);
    }
}

$("#pendenciaOS_QT_PECA").blur(function () {
    OcultarValidacoesPendencia();
});

$("#pendenciaOS_QT_PECA").keypress(function () {
    OcultarValidacoesPendencia();
});

function ExcluirPendenciaOS(ID_PENDENCIA_OS) {
    ConfirmarSimNao('Aviso', 'Confirma a <strong>FINALIZAÇÃO</strong> da Pendência?', 'ExcluirPendenciaOSConfirmada(' + ID_PENDENCIA_OS + ')');
}

function ExcluirPendenciaOSConfirmada(ID_PENDENCIA_OS) {
    var URL = URLAPI + "PendenciaOSAPI/Finalizar";

    if (ID_PENDENCIA_OS == "" || ID_PENDENCIA_OS == "0" || ID_PENDENCIA_OS == 0) {
        Alerta("Aviso", "Pendência inválida ou não informada!");
        return;
    }

    var pendenciaOSEntity = {
        ID_PENDENCIA_OS: ID_PENDENCIA_OS,
        nidUsuarioAtualizacao: nidUsuario
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        data: JSON.stringify(pendenciaOSEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", 'Pendência finalizada com sucesso!');
            carregarGridMVCPendencia();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

function EditarPendenciaOS(ID_PENDENCIA_OS) {
    var URL = URLAPI + "PendenciaOSAPI/Obter?ID_PENDENCIA_OS=" + ID_PENDENCIA_OS;
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
            if (res.pendenciaOS != null) {
                LoadPendenciaOS(res);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadPendenciaOS(res) {
    carregarComboPeca($("#pendenciaOS_peca_CD_PECA"));

    if (res.pendenciaOS != null) {
        $("#pendenciaOS_ID_PENDENCIA_OS").val(res.pendenciaOS.ID_PENDENCIA_OS);
        $("#pendenciaOS_ST_STATUS_PENDENCIA").val(res.pendenciaOS.ST_STATUS_PENDENCIA);
        $("#pendenciaOS_ST_TP_PENDENCIA").val(res.pendenciaOS.ST_TP_PENDENCIA).trigger('change');
        $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").val(res.pendenciaOS.CD_TP_ESTOQUE_CLI_TEC);
        $("#pendenciaOS_peca_CD_PECA").val(res.pendenciaOS.peca.CD_PECA);
        $("#pendenciaOS_DS_DESCRICAO").val(res.pendenciaOS.DS_DESCRICAO);
        $("#pendenciaOS_OsPadrao_ID_OS").val($("#ID_OS").val());
        $("#pendenciaOS_OsPadrao_TpStatusOS_DS_STATUS_OS").val($("#TpStatusOS_DS_STATUS_OS").val());
    }

    if (res.pendenciaOSModel != null) {
        const idOsFormatado = '#' + $("#ID_OS").val();
        $("#pendenciaOS_OsPadrao_ID_OS_Formatado").val(idOsFormatado);
        $("#pendenciaOS_DT_ABERTURA").val(res.pendenciaOSModel.DT_ABERTURA);
        $("#pendenciaOS_QT_PECA").val(res.pendenciaOSModel.QT_PECA);
        $("#PendenciaModal #pendenciaOS_OsPadrao_ID_OS").val(res.pendenciaOS.OS.ID_OS);
    }

    definirCorStatusOSPendencia($("#TpStatusOS_ST_STATUS_OS").val());
    CarregarEstoquePecaPendencia();

    $("#pendenciaOS_peca_CD_PECA").prop("disabled", true);
    $("#pendenciaOS_CD_TP_ESTOQUE_CLI_TEC").prop("disabled", true);
    $("#pendenciaOS_QT_PECA").prop("disabled", true);

    $('#PendenciaModal').modal('show');
}
