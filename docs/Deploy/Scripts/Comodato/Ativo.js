jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#DT_INCLUSAO').mask('00/00/0000');
    $('#DT_INVENTARIO').mask('00/00/0000');
    $('#DT_FIM_GARANTIA').mask('00/00/0000');
    $('#DT_MANUTENCAO').mask('00/00/0000');
    $('#DT_FIM_MANUTENCAO').mask('00/00/0000');

    $('#QTD_DIAS_GARANTIA').val(diasGarantia);
    $('#QTD_DIAS_MANUTENCAO').val(diasManutencao);

    $('#TX_ANO_MAQUINA').mask('0000');

    //var d = new Date();
    //if ($("#TX_ANO_MAQUINA").val() == 0) {
    //    $('#TX_ANO_MAQUINA').val(d.getFullYear());
    //}

    //if ($('#DT_INCLUSAO').val() == '') {
    //    $('#DT_INCLUSAO').val(formatDateToString(d));
    //}    

    OcultarCampo($('#validaSituacao'));
    OcultarCampo($('#validaStatus'));
    OcultarCampo($('#validaCD_MODELO'));
    OcultarCampo($('#validaCD_LINHA_PRODUTO'));
    validarTipoGarantia();
});

$('#btnGravar').click(function () {
    $('#TP_ACAO').val('');
    var validacao = ValidarCampos();

    return validacao;
});

$('#btnGravarSair').click(function () {
    $('#TP_ACAO').val('GravarSair');
    var validacao = ValidarCampos();

    return validacao;
});

$('#btnGravarContinuar').click(function () {
    $('#TP_ACAO').val('GravarContinuar');
    var validacao = ValidarCampos();

    return validacao;
});

$('#DT_FIM_GARANTIA').change(function () {
    validarTipoGarantia();
});

$('#DT_INCLUSAO').change(function () {
    var tipo = 'inclusao';
    //AlterarData($('#DT_INCLUSAO').val(), $('#QTD_DIAS_GARANTIA').val(), tipo);
    validarTipoGarantia();
});

$('#DT_MANUTENCAO').change(function () {

    var tipo = 'manutencao';
    //AlterarData($('#DT_MANUTENCAO').val(), $('#QTD_DIAS_MANUTENCAO').val(), tipo);
    validarTipoGarantia();
});

$('#DT_FIM_MANUTENCAO').change(function () {
    validarTipoGarantia();
});

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}

function validarTipoGarantia() {
    var tipoGarantia = "";

    var dataInclusao = new Date(formatarData($('#DT_INCLUSAO').val())); // moment($('#DT_INCLUSAO').val(), "DD-MM-YYYY");
    var dataFim = new Date(formatarData($('#DT_FIM_GARANTIA').val())); // moment($('#DT_FIM_GARANTIA').val(), "DD-MM-YYYY");

    var dataManutencao = new Date(formatarData($('#DT_MANUTENCAO').val())); // moment($('#DT_MANUTENCAO').val(), "DD-MM-YYYY");
    var dataFimManutencao = new Date(formatarData($('#DT_FIM_MANUTENCAO').val())); // moment($('#DT_FIM_MANUTENCAO').val(), "DD-MM-YYYY");

    var dataHoje = new Date();// moment();

    if (dataHoje >= dataInclusao && dataHoje <= dataFim) {
        tipoGarantia = "G";
    }

    if (dataHoje >= dataManutencao && dataHoje <= dataFimManutencao) {
        tipoGarantia = "GR";
    }

    $('#Tit_Garantia').text(tipoGarantia);
    $('#Tit_Garantia').val(tipoGarantia);

    //$('#QTD_DIAS_GARANTIA').val(dataFim - dataInclusao);
    //$('#QTD_DIAS_MANUTENCAO').val(dataFimManutencao - dataManutencao);


    //var timeDiff = Math.abs(new Date(dataFim + " 00:00") - new Date(dataInclusao + " 00:00"));
    //var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));

    //$('#QTD_DIAS_GARANTIA').val(diffDays.toString("dd/MM/yyyy"));

    //timeDiff = Math.abs(new Date(dataFimManutencao + " 00:00") - new Date(dataManutencao + " 00:00"));
    //diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));

    //$('#QTD_DIAS_MANUTENCAO').val(diffDays.toString("dd/MM/yyyy"));

    dataInclusao = $('#DT_INCLUSAO').val();
    dataFim = $('#DT_FIM_GARANTIA').val();

    dataManutencao = $('#DT_MANUTENCAO').val();
    dataFimManutencao = $('#DT_FIM_MANUTENCAO').val();

    var URL = URLAlterarDias + "?dataInclusao=" + dataInclusao + "&dataFim=" + dataFim + "&dataManutencao=" + dataManutencao + "&dataFimManutencao=" + dataFimManutencao;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
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
            if (res) {
                $('#QTD_DIAS_GARANTIA').val(res.DiasG);
                $('#QTD_DIAS_MANUTENCAO').val(res.DiasM);
            }
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }
    });
}

function ValidarCampos() {
    var validacao = true;
    OcultarCampo($('#validaSituacao'));
    OcultarCampo($('#validaStatus'));
    OcultarCampo($('#validaCD_MODELO'));
    OcultarCampo($('#validaCD_LINHA_PRODUTO'));

    var CD_MODELO = $('#modelo_CD_MODELO option:selected').val();

    if (CD_MODELO == "" || CD_MODELO == "0" || CD_MODELO == 0) {
        ExibirCampo($('#validaCD_MODELO'));
        validacao = false;
    }

    var CD_LINHA_PRODUTO = $('#linhaProduto_CD_LINHA_PRODUTO option:selected').val();

    if (CD_LINHA_PRODUTO == "" || CD_LINHA_PRODUTO == "0" || CD_LINHA_PRODUTO == 0) {
        ExibirCampo($('#validaCD_LINHA_PRODUTO'));
        validacao = false;
    }

    return validacao;
}

Date.prototype.addDias = function (dias) {
    this.setDate(this.getDate() + dias);
};

function AlterarData(Data, QtdDias, tipoData) {
    
    //var novaData = moment(Data + " 00:00", "D/M/YYYY h:m").add(QtdDias, 'days');

    ////novaData.format('L');
    ////moment(novaData, "DD/MM/YYYY");

    ////novaData = moment(novaData._d).format('L');

    ////novaData = novaData.getDate() + "/" + (novaData.getMonth() + 1) + "/" + novaData.getFullYear();
    ////var novaData = new Date(Date.now.getTime() + (QtdDias * 24 * 3600 * 1000));

    //if (tipoData == 'inclusao') {
    //    $('#DT_FIM_GARANTIA').val(moment(novaData, "DD/MM/YYYY"));
    //} else if (tipoData == 'manutencao') {
    //    $('#DT_FIM_MANUTENCAO').val(moment(novaData, "DD/MM/YYYY"));
    //}

    //alert(Data);
    var URL = URLAlterarData + "?Data=" + Data + "&QtdDias=" + QtdDias;

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
            if (tipoData == 'inclusao')
                $('#DT_FIM_GARANTIA').val(res.DataFinal);
            else
                $('#DT_FIM_MANUTENCAO').val(res.DataFinal);
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}


$("#statusAtivo_CD_STATUS_ATIVO").change(function () {
    OcultarCampo($('#validaStatus'));
});

$("#situacaoAtivo_CD_SITUACAO_ATIVO").change(function () {
    OcultarCampo($('#validaSituacao'));
});

$("#modelo_CD_MODELO").change(function () {
    OcultarCampo($('#validaCD_MODELO'));
});

$("#linhaProduto_CD_LINHA_PRODUTO").change(function () {
    OcultarCampo($('#validaCD_LINHA_PRODUTO'));
});

$('#DT_INCLUSAO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_FIM_MANUTENCAO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_MANUTENCAO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_FIM_GARANTIA-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_INVENTARIO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$("#CD_ATIVO_FIXO").blur(function () {
    var CD_ATIVO_FIXO = $("#CD_ATIVO_FIXO").val();

    if (CD_ATIVO_FIXO != "") {
        var URL = URLVerificarCodigo + "?CD_ATIVO_FIXO=" + CD_ATIVO_FIXO;

        $.ajax({
            url: URL,
            processData: true,
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
                if (res.Status == "Redirecionar") {
                    URL = URLEditar + "?idKey=" + res.idKey;
                    window.location.href = URL;
                }
            }
        });
    }
});
