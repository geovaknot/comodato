$().ready(function () {
    $('.js-select-basic-single').select2({ minimumInputLength: 3 });
    
    $('#ddtEmissao').mask('00/00/0000');

    $('#ddtEmissao-container .input-group.date').datepicker({
        language: "pt-BR",
        autoclose: true,
        todayHighlight: true
    });

    $('#ddtRecebimento').mask('00/00/0000');

    $('#ddtRecebimento-container .input-group.date').datepicker({
        language: "pt-BR",
        autoclose: true,
        todayHighlight: true
    });

    $('#ddtOk').mask('00/00/0000');

    $('#ddtOk-container .input-group.date').datepicker({
        language: "pt-BR",
        autoclose: true,
        todayHighlight: true
    });

    //$('#groupTipoContrato').hide();

    //$("input[type='checkbox']").change(function () {
    //    var selecionados = $('#cdsContratoTipo').val();
    //    if (selecionados != '') {
    //        if (selecionados.substring(selecionados.length - 1, selecionados.length) != ',') {
    //            selecionados = selecionados + ',';
    //        }
    //    }
    //    if ($(this).is(':checked')) {
    //        selecionados += $(this).val() + ',';
    //    } else {
    //        selecionados = selecionados.replace($(this).val() + ',', '');
    //    }

    //    $('#cdsContratoTipo').val(selecionados);
    //});

    $("input[type='radio']").change(function () {
        var modeloContrato = $(this).val();

        preencherClausulaModelo(modeloContrato);
        $('#cdsContratoTipo').val(modeloContrato);

        //if ($(this).val() == '') {
        //    $("#groupTipoContrato").show();
        //}
        //else {
        //    $("#groupTipoContrato").hide();
        //    $("input[type='checkbox']").prop('checked', false);
        //}
    });


    //if (modeloValue.indexOf(",") > 0) {
    //    $("input[type='radio'][value='']").prop("checked", true);
    //    $('#groupTipoContrato').show();
    //    for (var i = 0; i < modeloValue.split(',').length; i++) {
    //        var itemValue = modeloValue.split(',')[i];
    //        $("input[type='checkbox'][value='" + itemValue + "']").prop("checked", true);
    //    }
    //}
    //else {
        //if (undefined == $("input[type='radio'][value='" + modeloValue + "']").val()) {
        //    $("input[type='radio'][value='']").prop("checked", true);
        //    //$('#groupTipoContrato').show();
        //    //$("input[type='checkbox'][value='" + modeloValue + "']").prop("checked", true);

        //}
    //    else {
            $("input[type='radio'][value='" + modeloValue + "']").prop("checked", true);
    //        if (modeloValue == '') {
    //            $('#groupTipoContrato').show();
    //        }
    //    }
    //}

    OcultarCampo($('#validaCliente'));
});


function preencherClausulaModelo(modeloContrato) {
    var url = URLAPI + "ContratoAPI/ObterClausula?modeloContrato=ModeloContrato" + modeloContrato;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            var clausulaContrato = JSON.parse(data.CLAUSULA);
            if (null != clausulaContrato) {
                $('#cdsClausulas').val(clausulaContrato);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

//Função para popular Grid após pesquisa
function popularContratos(codigoCliente) {
    atribuirParametrosPaginacao("divContratos", actionPesquisaContratos, '{"codigoCliente":"' + codigoCliente + '"}');

    $.ajax({
        url: actionPesquisaContratos + '?codigoCliente=' + codigoCliente,
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
    }).done(function (e) {
        $("#loader").css("display", "none");
        if (e.Status == "Success") {
            var divContratosId = 'divContratos';
            var tagControle = '<div id="' + divContratosId + '" class="col-md-12">';

            $(".form-group").remove('#' + divContratosId);
            $(".form-group").append(tagControle);
            $('#' + divContratosId).html(e.Html);
        }
    });
}

function popularStatusContrato(seletor, cdsStatus) {
    var url = URLAPI + "ContratoAPI/ObterListaStatus";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            var listaStatus = JSON.parse(data.STATUS);
            if (null != listaStatus) {
                LimparCombo($(seletor));
                for (i = 0; i < listaStatus.length; i++) {
                    MontarCombo($(seletor), listaStatus[i].CD_STATUS_CONTRATO, listaStatus[i].DS_STATUS_CONTRATO);
                }
                if (cdsStatus > 0) {
                    $(seletor).val(cdsStatus).trigger('change');
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

//Função para preencher dropdownlist Clientes
function popularClientes(seletor, idCliente, disabled, popularSomenteSelecionado) {
    var clienteEntity = {};

    if (idCliente > 0 && popularSomenteSelecionado == true) {
        clienteEntity = { CD_CLIENTE: idCliente };
    }
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "ClienteAPI/ObterLista",
        data: JSON.stringify(clienteEntity),
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.clientes != null) {
                LimparCombo($(seletor));
                for (i = 0; i < data.clientes.length; i++) {
                    var dsContrato = data.clientes[i].NM_CLIENTE + ' (' + data.clientes[i].CD_CLIENTE + ') - ' + data.clientes[i].EN_CIDADE + ' - ' + data.clientes[i].EN_ESTADO;
                    MontarCombo($(seletor), data.clientes[i].CD_CLIENTE, dsContrato);
                }

                if (idCliente > 0) {
                    $(seletor).val(idCliente).trigger('change');
                }

                if (disabled == true) {
                    $(seletor).prop("disabled", true);
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

$('#Gravar').click(function () {
    var cliente = $('#nidCliente option:selected').val();

    if (cliente == "" || cliente == "0" || cliente == 0) {
        ExibirCampo($('#validaCliente'));
        return false;
    }
});

$("#nidCliente").change(function () {
    OcultarCampo($('#validaCliente'));
});