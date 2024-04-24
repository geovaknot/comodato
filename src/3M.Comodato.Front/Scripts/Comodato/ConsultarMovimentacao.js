$(document).ready(function () {
    $('.js-select-basic-single').select2();

    OcultarCampo($('#validaComboEstoque'));

    $('#PeriodoDe').mask('00/00/0000');
    $('#PeriodoAte').mask('00/00/0000');

    $('#ddtDe-container .input-group.date').datepicker({
        language: "pt-BR",
        autoclose: true,
        todayHighlight: true
    });

    $('#ddtAte-container .input-group.date').datepicker({
        language: "pt-BR",
        autoclose: true,
        todayHighlight: true
    });

    if ($('#CodigoEstoque > option').length == 2) {
        $("#CodigoEstoque").prop("selectedIndex", 1).trigger('change');
    }
});

$('#btnConsultar').click(function (e) {
    e.preventDefault();
    OcultarCampo($('#validaComboEstoque'));

    var nidEstoque = $('#CodigoEstoque').val();
    if (nidEstoque === "" || nidEstoque === "0" || nidEstoque === 0) {
        ExibirCampo($('#validaComboEstoque'));
        return false;
    }

    var filtroMovimentacao = new Object();
    filtroMovimentacao.CodigoEstoque = nidEstoque;

    var periodoDe = $('#PeriodoDe').val();
    if (periodoDe !== '' || periodoDe === "0" || periodoDe === 0) {
        filtroMovimentacao.PeriodoDe = periodoDe;
    }

    var periodoAte = $('#PeriodoAte').val();
    if (periodoAte !== '' || periodoAte === "0" || periodoAte === 0) {
        filtroMovimentacao.PeriodoAte = periodoAte;
    }

    var codigoPeca = $('#CodigoPeca').val();
    if (codigoPeca !== '' || codigoPeca === "0" || codigoPeca === 0) {
        filtroMovimentacao.CodigoPeca = codigoPeca;
    }

    var tipoMovimentacao = $('#TipoMovimentacao').val();
    if (tipoMovimentacao !== '' || tipoMovimentacao === "0" || tipoMovimentacao === 0) {
        filtroMovimentacao.TipoMovimentacao = tipoMovimentacao;
    }

    atribuirParametrosPaginacao("gridMovimentacao", actionConsultar, JSON.stringify(filtroMovimentacao));
    $.ajax({
        type: 'POST',
        url: actionConsultar,
        data: JSON.stringify(filtroMovimentacao),
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.Status === 'Success') {
                $('#gridMovimentacao').html(data.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
});
