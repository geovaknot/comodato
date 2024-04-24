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

    var URL = URLCriptografarChave + "?Conteudo=" + tecnicos + "|" + status + "|" + ordenacao;

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
                window.open(URLSite + '/RelatorioCarteiraAtendimento.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

});

function popularComboTecnico() {
    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        beforeSend: function () {
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

function popularComboTpStatusVisitaOS() {
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
        MontarCombo($("#ddlStatus"), tiposStatusVisitaOS[i].ST_TP_STATUS_VISITA_OS, tiposStatusVisitaOS[i].DS_TP_STATUS_VISITA_OS);
    }
}