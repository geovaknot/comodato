$(document).ready(function () {
    $('#VL_PECA').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });
    $('#QTD_MINIMA').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', precision: 3, allowZero: true });
    $('#QTD_ESTOQUE').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', precision: 3, allowZero: true });
});

$("#CD_PECA").blur(function () {
    var CD_PECA = $("#CD_PECA").val();

    if (CD_PECA != "") {
        var URL = URLVerificarCodigo + "?CD_PECA=" + CD_PECA;

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
                else {
                    $('#CD_PECA').val(res.peca.CD_PECA);
                    $('#DS_PECA').val(res.peca.DS_PECA);
                    $('#TX_UNIDADE').val(res.peca.TX_UNIDADE);
                    $('#VL_PECA').val(res.peca.VL_PECA);
                    $("#TP_PECA").val(res.peca.TP_PECA);
                    $("#FL_ATIVO_PECA").val(res.peca.FL_ATIVO_PECA);
                }
            }
        });
    }
});

