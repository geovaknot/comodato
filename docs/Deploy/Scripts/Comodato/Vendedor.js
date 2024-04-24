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

$(document).ready(function () {
    $('.js-example-basic-single').select2();
    $('#EN_CEP').mask('00000-000');
    //$('#TX_TELEFONE').mask('00 00000-0000');
});