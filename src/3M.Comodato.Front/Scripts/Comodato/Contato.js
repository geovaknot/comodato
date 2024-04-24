jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();
    $('#cliente_CD_CLIENTE').select2({ minimumInputLength: 3 });

    $("#tipoContato_nidTipoContato").trigger('change');

});

$("#tipoContato_nidTipoContato").change(function () {

    var tipo = $("#tipoContato_nidTipoContato option:selected").text().replace(/\s/g, "");
    if (tipo == "Cliente") {
        $("#empresa").hide();
        $("#cliente").show();
    } else {
        $("#empresa").show();
        $("#cliente").hide();
    }
});