jQuery(document).ready(function () {
    $('#DT_DESATIVACAO_Edit').mask('00/00/0000');
    $('#NR_CNPJ').mask('00.000.000/0000-00');
    $('#EN_CEP').mask('00000-000');
    //$('#TX_TELEFONE').mask('00 00000-0000');
    //$('#TX_FAX').mask('00 00000-0000');

    $('#TX_TELEFONERESPONSAVELPECAS').mask('00 0000-00009', {
        onKeyPress: function (val, e, field, options) {
            // Ajusta a máscara se o usuário digitar 9 dígitos no número
            var mask = val.length > 14 ? '00 00000-0000' : '00 0000-00009';
            $('#TX_TELEFONERESPONSAVELPECAS').mask(mask, options);
        }
    });

    $('.js-example-basic-single').select2();
    
    if ($("#CD_CLIENTE").val() == 0) {
        $("#CD_CLIENTE").val('');
    }
    
    $("#FL_KAT_FIXO_SimNao").trigger('change');

    var ValidaAtivaPlanoZero = $("#FL_AtivaPlanoZero").val();

    if (ValidaAtivaPlanoZero == "Sim" || ValidaAtivaPlanoZero == "SIM" || ValidaAtivaPlanoZero == "S") {
        $("#validaPeriodoPlanoZero").prop("disabled", "false");
    } else {
        $("#validaPeriodoPlanoZero").prop("disabled", "true");
    };

});

$('#DT_DESATIVACAO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$("#CD_CLIENTE").blur(function () {
    var CD_CLIENTE = $("#CD_CLIENTE").val();

    if (CD_CLIENTE != "") {
        var URL = URLVerificarCodigo + "?CD_CLIENTE=" + CD_CLIENTE;

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
                    $('#CD_CLIENTE').val(res.cliente.CD_CLIENTE);
                    $('#CD_BCPS').val(res.cliente.CD_CLIENTE);
                    $('#EN_ENDERECO').val(res.cliente.EN_ENDERECO);
                    $('#EN_BAIRRO').val(res.cliente.EN_BAIRRO);
                    $('#EN_CIDADE').val(res.cliente.EN_CIDADE);
                    $("#TX_FAX").val(res.cliente.TX_FAX);
                    $("#TX_TELEFONE").val(res.cliente.TX_TELEFONE);
                    $("#NM_CLIENTE").val(res.cliente.NM_CLIENTE);
                    $("#EN_ESTADO").val(res.cliente.EN_ESTADO);
                    $("#EN_CEP").val(res.cliente.EN_CEP);

                    $("#CD_ABC").val(res.cliente.CD_ABC);
                    $("#regiao_CD_REGIAO").val(res.cliente.regiao.CD_REGIAO);
                    $("#CD_FILIAL").val(res.cliente.CD_FILIAL);
                    $("#CL_CLIENTE").val(res.cliente.CL_CLIENTE);
                    $("#CD_RAC").val(res.cliente.CD_RAC);

                    $("#NR_CNPJ").val(res.cliente.NR_CNPJ);
                }
            }
        });
    }
});

$("#Voltar").click(function () {
    //window.history.back();
    //window.location.href = uri;
});

$("#DS_CLASSIFICACAO_KAT").change(function () {
    var DS_CLASSIFICACAO_KAT = $("#DS_CLASSIFICACAO_KAT").val();
    var FL_KAT_FIXO_SimNao = $("#FL_KAT_FIXO_SimNao").val();

    if (FL_KAT_FIXO_SimNao == "N") {
        switch (DS_CLASSIFICACAO_KAT) {
            case '0':
                $("#QT_PERIODO").val(0);
                break;
            case 'C-':
                $("#QT_PERIODO").val(4);
                break;
            case 'C+':
                $("#QT_PERIODO").val(8);
                break;
            case 'B-':
                $("#QT_PERIODO").val(12);
                break;
            case 'B+':
                $("#QT_PERIODO").val(24);
                break;
            case 'A-':
                $("#QT_PERIODO").val(48);
                break;
            case 'A+':
                $("#QT_PERIODO").val(72);
                break;
        }
    }
});

$("#FL_KAT_FIXO_SimNao").change(function () {
    var FL_KAT_FIXO_SimNao = $("#FL_KAT_FIXO_SimNao").val();

    if (FL_KAT_FIXO_SimNao == "S")
        $("#QT_PERIODO").prop("readonly", "readonly");
    else
        $("#QT_PERIODO").prop("readonly", false);
});

$("#btnCalcular").click(function () {
    var CD_CLIENTE = $("#CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        Alerta("Aviso", "Cliente inválido ou não informado!");
        return false;
    }

    var URL = URLAPI + "ClienteAPI/CalcularKAT?CD_Cliente=" + CD_CLIENTE;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.retorno != null) {                
                Alerta("AVISO", JSON.parse(res.retorno));
                //Alerta("AVISO", res.retorno);
                //$("#DS_CLASSIFICACAO_KAT").val(JSON.parse(res.DS_CLASSIFICACAO_KAT));
                //$("#QT_PERIODO").val(JSON.parse(res.QT_PERIODO));
                location.reload();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

});

$("#FL_AtivaPlanoZero").change(function () {
    var ValidaAtivaPlanoZero = $("#FL_AtivaPlanoZero").val();

    if (ValidaAtivaPlanoZero == "Sim" || ValidaAtivaPlanoZero == "SIM" || ValidaAtivaPlanoZero == "S") {
        $("#validaPeriodoPlanoZero").prop("disabled", "false");
    } else {
        $("#validaPeriodoPlanoZero").prop("disabled", "true");
    };

});

$("#btnGravar").click(function () {
    var CD_VENDEDOR = $("#vendedor_CD_VENDEDOR").val();

    if (CD_VENDEDOR == "" || CD_VENDEDOR == "" || CD_VENDEDOR == "0") {
        ExibirCampo($("#validaCD_VENDEDOR"));
        return false;
    }
    else {
        OcultarCampo($("#validaCD_VENDEDOR"));
    }

    var ValidaAtivaPlanoZero = $("#FL_AtivaPlanoZero").val();
    var ValidaPeriodoPlanoZero = $("#QTD_PeriodoPlanoZero").val();

    if (ValidaAtivaPlanoZero == "" || ValidaAtivaPlanoZero == " " || ValidaAtivaPlanoZero == "0" || ValidaAtivaPlanoZero == undefined) {
        ExibirCampo($("#validaAtivaPlanoZero"));
        return false;
    }
    else if (ValidaAtivaPlanoZero == "Sim" || ValidaAtivaPlanoZero == "SIM" || ValidaAtivaPlanoZero == "S"){
        OcultarCampo($("#validaAtivaPlanoZero"));
        if (ValidaPeriodoPlanoZero == "" || ValidaPeriodoPlanoZero == " " || ValidaPeriodoPlanoZero == "0" || ValidaPeriodoPlanoZero == undefined) {
            ExibirCampo($("#validaPeriodoPlanoZero"));
            return false;
        }
        else {
            OcultarCampo($("#validaPeriodoPlanoZero"));

        }
    } else {
        OcultarCampo($("#validaAtivaPlanoZero"));
        OcultarCampo($("#validaPeriodoPlanoZero"));
    }

    return true;
});

$("#vendedor_CD_VENDEDOR").change(function () {
    OcultarCampo($("#validaCD_VENDEDOR"));
});