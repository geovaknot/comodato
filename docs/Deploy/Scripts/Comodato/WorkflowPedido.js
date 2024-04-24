$().ready(function () {
    $('.js-select-basic-single').select2({
        placeholder: "Selecione...",
        allowClear: true
    });
    $("#VisaoPedidos").val(2);

    $('#DataCadastroInicio').mask('00/00/0000');
    $('#DataCadastroFim').mask('00/00/0000');
    $('.js-select-basic-single').val(null).trigger('change');
    carregarGridMVC();
});

$('#DT_FILTRO-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#TipoPedido').on('select2:select', function () {
    PopularStatus();
});

function PopularStatus() {
    LimparCombo($('#Status'));

    var tipoPedido = $('#TipoPedido').val();
    if (null == tipoPedido || tipoPedido == '') {
        return false;
    }

    var URL = URLAPI + "WfStatusPedidoEquipAPI/ObterLista?tpPedido=" + tipoPedido;
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
            if (res.STATUS != null) {
                CarregarComboStatus(res.STATUS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function CarregarComboStatus(listaStatusJO) {
    var listaStatus = JSON.parse(listaStatusJO);

    for (i = 0; i < listaStatus.length; i++) {
        MontarCombo($('#Status'), listaStatus[i].ST_STATUS_PEDIDO, listaStatus[i].DS_STATUS_NOME);
    }
}

$('#btnConsultar').click(function () {
    carregarGridMVC();
});

function carregarGridMVC() {
    var visaoPedidos = $('#VisaoPedidos').val();
    var dataInicio = $('#DataCadastroInicio').val();
    var dataFinal = $('#DataCadastroFim').val();
    var codigoPedido = $('#CodigoPedido').val();
    var codigoSolicitante = $('#Solicitante').val();
    var tipoPedido = $('#TipoPedido').val();
    var status = $('#Status').val();
    var tipoSolicitacao = $('#TipoSolicitacao').val();

    var filtro = new Object();

    filtro.VisaoPedidos = visaoPedidos;
    if (dataInicio != null && dataInicio != '')
        filtro.DataCadastroInicio = dataInicio;

    if (dataFinal != null && dataFinal != '')
        filtro.DataCadastroFim = dataFinal;

    if (codigoPedido != null && codigoPedido != '')
        filtro.CodigoPedido = codigoPedido;

    if (codigoSolicitante != null && codigoSolicitante != '')
        filtro.Solicitante = codigoSolicitante;

    if (tipoPedido != null && tipoPedido != '')
        filtro.TipoPedido = tipoPedido;

    if (status != null && status != '')
        filtro.Status = status;

    if (tipoSolicitacao != null && tipoSolicitacao != '')
        filtro.tipoSolicitacao = tipoSolicitacao;

    filtro.usuarioEminente = nidUsuario;



    atribuirParametrosPaginacao("gridMvc", actionConsultar, JSON.stringify(filtro));

    $.ajax({
        type: 'POST',
        url: actionConsultar,
        data: JSON.stringify(filtro),
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.Status == 'Success') {
                $('#gridMvc').html(data.Html);
                $('.grid-mvc').gridmvc();

                if (data.perfil == 2) {
                    $('.fa-search, .fa-pencil-alt, .fa-trash-alt').hide();
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}

function RemoverPedido(codigoPedido) {
    var url = URLAPI + "WorkflowAPI/ExcluirPedido?id_wf_pedido_equip=" + codigoPedido + "&nidUsuario=" + nidUsuario;
    $.ajax({
        type: 'POST',
        url: url,
        async: false,
        cache: false,
        contentType: "application/json",
        dataType: "json",

        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (response) {
            $("#loader").css("display", "none");
            Alerta("Sucesso", JSON.parse(response.Mensagem));
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

$('#btnImprimir').click(function () {
    var dataInicio = $('#DataCadastroInicio').val();
    var dataFinal = $('#DataCadastroFim').val();
    var codigoPedido = $('#CodigoPedido').val();
    var codigoSolicitante = $('#Solicitante').val();
    var tipoPedido = $('#TipoPedido').val();
    var status = $('#Status').val();

    if (dataInicio == 'null' || dataInicio == null)
        dataInicio = '';

    if (dataFinal == 'null' || dataFinal == null)
        dataFinal = '';

    if (codigoPedido == 'null' || codigoPedido == null)
        codigoPedido = '';

    if (codigoSolicitante == 'null' || codigoSolicitante == null)
        codigoSolicitante = '';

    if (tipoPedido == 'null' || tipoPedido == null)
        tipoPedido = '';

    if (status == 'null' || status == null)
        status = '';

    var URL = URLCriptografarChave + "?Conteudo=" +
        dataInicio + "|" + dataFinal + "|" + codigoPedido + "|" + codigoSolicitante + "|" + tipoPedido + "|" + status;

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
                if (tipoPedido == "D") {
                    window.open(URLSite + '/RelatorioWorkflowDevolucao.aspx?idKey=' + res.idKey, '_blank');
                } else {
                    window.open(URLSite + '/RelatorioWorkflow.aspx?idKey=' + res.idKey, '_blank');
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });

});

$('#btnJOBDiario').click(function () {
    var URL = URLAPI + "WorkflowAPI/JobEmailDiario";
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
            Alerta("Aviso", "JOB executado com sucesso!");
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

});

$('#btnJOBAdministrador').click(function () {
    var URL = URLAPI + "WorkflowAPI/JobEmailAdministrador";
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
            Alerta("Aviso", "JOB executado com sucesso!");
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

});