jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#VL_PESO_CUBADO').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_ALTUR_MIN').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_ALTUR_MAX').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_LARG_MIN').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_LARG_MAX').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_COMP_MIN').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_COMP_MAX').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_ALTUR_CAIXA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_LARG_CAIXA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VL_COMP_CAIXA').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
});