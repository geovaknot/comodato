$("#CD_CONSUMIVEL").blur(function () {
    var CD_CONSUMIVEL = $("#CD_CONSUMIVEL").val();

    if (CD_CONSUMIVEL != "") {
        var URL = URLVerificarCodigo + "?CD_CONSUMIVEL=" + CD_CONSUMIVEL;

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
                    $('#CD_CONSUMIVEL').val(res.consumivel.CD_CONSUMIVEL);
                    //$('#linhaProduto_CD_LINHA_PRODUTO').val(res.consumivel.linhaProduto.CD_LINHA_PRODUTO);
                    $("#linhaProduto_CD_LINHA_PRODUTO").prop("selectedIndex", 0);
                    $('#DS_CONSUMIVEL').val(res.consumivel.DS_CONSUMIVEL);
                    $('#CD_MAJOR').val(res.consumivel.CD_MAJOR);
                    $('#DS_MAJOR').val(res.consumivel.DS_MAJOR);
                    $('#CD_COMMODITY').val(res.consumivel.CD_COMMODITY);
                    $('#DS_COMMODITY').val(res.consumivel.DS_COMMODITY);
                    $('#TX_UNIDADE').val(res.consumivel.TX_UNIDADE);
                    $('#CD_FAMILY').val(res.consumivel.CD_FAMILY);
                    $('#DS_FAMILY').val(res.consumivel.DS_FAMILY);
                    $('#CD_SUB_FAMILY').val(res.consumivel.CD_SUB_FAMILY);
                    $('#DS_SUB_FAMILY').val(res.consumivel.DS_SUB_FAMILY);
                    $('#TX_UNIDADE_CONV').val(res.consumivel.TX_UNIDADE_CONV);
                    $('#CUSTO').val(res.consumivel.CUSTO);
                }
            }
        });
    }
});
