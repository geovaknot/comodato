$(document).ready(function () {

    $('#ddlTecnico').select2({
        placeholder: "Selecione...",
        templateSelection: function (data) { return data.id; }
    });

    $('#ddlStatus').select2({
        placeholder: "Selecione...",
        //allowClear: true,
        //templateSelection: function (data) { return data.id; }
    });
    $('#ddlCampoOrdem').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    popularComboTecnico();
    popularComboTpStatusVisitaOS();
});

$('#btnLimpar').click(function () {
    $('#ddlTecnico').val(null).trigger('change');
    $('#ddlStatus').val(null).trigger('change');

    $('#ddlCampoOrdem').val(null).trigger('change');
});

$('#btnImprimir').click(function () {
    var tecnicos = '';
    if (null != $("#ddlTecnico").val() && $("#ddlTecnico").val() != '')
        tecnicos = $("#ddlTecnico").val();

    var status = '';
    if (null != $("#ddlStatus").val() && $("#ddlStatus").val() != '')
        status = $("#ddlStatus").val();

    var ordenacao = '';
    if (null != $("#ddlCampoOrdem").val() && $("#ddlCampoOrdem").val() != '')
        ordenacao = $("#ddlCampoOrdem").val();

    var DT_INICIAL = $('#DT_INICIAL').val();
    var DT_FINAL = $('#DT_FINAL').val();

    var periodoValido = checarDatas(DT_INICIAL, DT_FINAL);
    if (periodoValido) {

        var URL = URLCriptografarChave + "?Conteudo=" + tecnicos + "|" + status + "|" + ordenacao + "|" + DT_INICIAL + "|" + DT_FINAL;

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
                if (res.idKey != null && res.idKey != '') {
                    window.open(URLSite + '/RelatorioCarteiraAtendimentoVisita.aspx?idKey=' + res.idKey, '_blank');
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

function popularComboTecnico() {
    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        contentType: "application/json",
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

function preencherComboTecnicos(tecnicosJO) {
    //LimparCombo($("#ddlTecnico"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#ddlTecnico"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    }
}

$('#DT_DATA-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true,
    orientation: 'bottom'
});

function checarDatas(DT_INICIAL, DT_FINAL) {
    var data_1 = new Date(formatarData(DT_INICIAL));
    var data_2 = new Date(formatarData(DT_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        Alerta("Aviso", "A data inicial não pode ser maior que a data final!");
        return false;
    }
    //} else if (data_1 > data) {
    //    Alerta("Aviso", "A data inicial não pode ser posterior a hoje!");
    //    return false;

    //} else if (data_2 > data) {
    //    Alerta("Aviso", "A data final não pode ser posterior a hoje!");
    //    return false;
    //}

    return true;
}

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}

function popularComboTpStatusVisitaOS() {
    var URL = URLAPI + "TpStatusVisitaOSAPI/ObterListaOsPadraoStatus";
    //var token = sessionStorage.getItem("token");

    var tpStatusVisitaOSEntity = {
        
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tpStatusVisitaOSEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tiposStatusVisitaOS != null) {
                preencherComboTpStatusVisitaOS(res.tiposStatusVisitaOS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function preencherComboTpStatusVisitaOS(tiposStatusVisitaOS) {
    //LimparCombo($("#ddlStatus"));

    for (i = 0; i < tiposStatusVisitaOS.length; i++) {
        MontarCombo($("#ddlStatus"), tiposStatusVisitaOS[i].ST_STATUS_OS, tiposStatusVisitaOS[i].DS_STATUS_OS);
    }
}