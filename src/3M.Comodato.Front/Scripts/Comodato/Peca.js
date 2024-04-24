$(document).ready(function () {
    $('#VL_PECA').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });
    $('#QTD_MINIMA').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', precision: 3, allowZero: true });
    $('#QTD_PlanoZero').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', precision: 3, allowZero: true });
    $('#QTD_ESTOQUE').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', precision: 3, allowZero: true });
    $("#DivPecaRec").show();
    
});

$("#CD_PECA").blur(function () {
    console.log("Entrou")
    var CD_PECA = $("#CD_PECA").val();

    var tipo = $("#TP_PECA").val();
    if (tipo == "A") {
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
    }


    
});

$("#TP_PECA").change(function () {
    console.log("Entrou2")
    var tipo = $("#TP_PECA").val();
    if (tipo == "A") {
        $("#DS_PECA").attr("readonly", true);
        $("#TX_UNIDADE").attr("readonly", true);
        $("#VL_PECA").attr("readonly", true);
        $("#FL_ATIVO_PECA").prop("disabled", true); 

        $("#DS_PECA").val("");
        $("#TX_UNIDADE").val("");
        $("#VL_PECA").val("");
        $("#CD_PECA").val("");
        $("#QTD_MINIMA").val("");
        $("#QTD_ESTOQUE").val("");
        $("#FL_ATIVO_PECA").val("S");
        $("#DivPecaRec").show();
    }
    else if (tipo == "E") {
        $("#DS_PECA").attr("readonly", false); 
        $("#TX_UNIDADE").attr("readonly", false); 
        $("#VL_PECA").attr("readonly", false);
        $("#FL_ATIVO_PECA").prop("disabled", false); 

        $("#DS_PECA").val("");
        $("#TX_UNIDADE").val("");
        $("#VL_PECA").val("");
        $("#CD_PECA").val("");
        $("#QTD_MINIMA").val("");
        $("#QTD_ESTOQUE").val("");
        $("#FL_ATIVO_PECA").val("S");
        $("#DivPecaRec").hide();
    }
    else if (tipo == "R") {
        $("#DS_PECA").attr("readonly", false);
        $("#TX_UNIDADE").attr("readonly", false);
        $("#VL_PECA").attr("readonly", false);
        $("#FL_ATIVO_PECA").prop("disabled", false);

        $("#DS_PECA").val("");
        $("#TX_UNIDADE").val("");
        $("#VL_PECA").val("");
        $("#CD_PECA").val("");
        $("#QTD_MINIMA").val("");
        $("#QTD_ESTOQUE").val("");
        $("#FL_ATIVO_PECA").val("S");
        $("#DivPecaRec").hide();
    }
});

