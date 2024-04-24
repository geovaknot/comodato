$(document).ready(function () {
    $('.js-example-basic-single').select2();
    $('#VL_CUSTO_HORA').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });

    $('#EN_CEP').mask('00000-000');
    //$('#TX_TELEFONE').mask('00 00000-0000');

    ExibirOcultarTransferirCarteira();

});

$('#EN_ESTADO').keypress(function (e) {
    var regex = new RegExp("^[a-zA-Z]+$");
    var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
    if (regex.test(str)) {
        return true;
    }
    else {
        e.preventDefault();
        return false;
    }
});

$("#FL_ATIVO").change(function () {
    ExibirOcultarTransferirCarteira();
});

function ExibirOcultarTransferirCarteira() {
    var FL_ATIVO = $("#FL_ATIVO option:selected").val();

    if (FL_ATIVO == 'N')
        ExibirCampo($('#opcoesInativo'));
    else
        OcultarCampo($('#opcoesInativo'));
}