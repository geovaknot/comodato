jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();
    $('#VL_ALUGUEL').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });

    $('#DT_NOTAFISCAL').mask('00/00/0000');
    $('#DT_DEVOLUCAO').mask('00/00/0000');

    if ($("#NR_NOTAFISCAL").val() == 0) {
        $("#NR_NOTAFISCAL").val('');
    }

    OcultarCampo($('#validaCD_CLIENTE'));
    OcultarCampo($('#validaCD_ATIVO_FIXO'));
    OcultarCampo($('#validaDT_DEVOLUCAO'));
    OcultarCampo($('#validaCD_MOTIVO_DEVOLUCAO'));

    OcultarCampo($('#Form_DT_FIM_GARANTIA_REFORMA_container'));

    var CD_CLIENTE = $('#cliente_CD_CLIENTE').val();

    if (CD_CLIENTE == null || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0 || CD_CLIENTE == "Selecione..." || CD_CLIENTE == undefined) {
        carregarComboCliente();
    }
});

$('#cliente_CD_CLIENTE').on('select2:select', function (e) {
    $("#tipo_CD_TIPO").empty();
    $.ajax({
        type: 'POST',
        url: URLAPI + 'AtivoClienteAPI/ObterListaTipoServico?codigoCliente=' + e.params.data.id,
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.ATIVO_CLIENTE_TIPO_SERVICO != null) {
                var listaTipos = JSON.parse(data.ATIVO_CLIENTE_TIPO_SERVICO);
                PopularTipoServico(listaTipos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

    //$('#tipo_CD_TIPO').
});
//tipo_CD_TIPO
//cliente_CD_CLIENTE


function PopularTipoServico(tipos) {
    for (i = 0; i < tipos.length; i++) {
        MontarCombo($("#tipo_CD_TIPO"), tipos[i].CD_TIPO, tipos[i].DS_TIPO);
    }
}


$('#btnGravar').click(function () {
    $('#TP_ACAO').val('');
    var validacao = ValidarCampos();

    return validacao;
});

$('#btnGravarSair').click(function () {
    $('#TP_ACAO').val('GravarSair');
    var validacao = ValidarCampos();

    return validacao;
});

$('#btnGravarContinuar').click(function () {
    $('#TP_ACAO').val('GravarContinuar');
    var validacao = ValidarCampos();

    return validacao;
});

function ValidarCampos() {
    var validacao = true;
    OcultarCampo($('#validaCD_CLIENTE'));
    OcultarCampo($('#validaCD_ATIVO_FIXO'));
    OcultarCampo($('#validaCD_MOTIVO_DEVOLUCAO'));
    OcultarCampo($('#validaDT_DEVOLUCAO'));

    var CD_CLIENTE = $('#cliente_CD_CLIENTE').val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        ExibirCampo($('#validaCD_CLIENTE'));
        validacao = false;
    }

    var CD_ATIVO_FIXO = $('#ativoFixo_CD_ATIVO_FIXO').val();

    if (CD_ATIVO_FIXO == "" || CD_ATIVO_FIXO == "0" || CD_ATIVO_FIXO == 0) {
        ExibirCampo($('#validaCD_ATIVO_FIXO'));
        validacao = false;
    }


    var DS_MODELO = $('#DS_MODELO').val();

    if (DS_MODELO == "" || DS_MODELO == "Não Encontrado") {
        ExibirCampo($('#validaCD_ATIVO_FIXO'));
        validacao = false;
    }


    var DT_DEVOLUCAO = $('#DT_DEVOLUCAO').val();
    var CD_MOTIVO_DEVOLUCAO = $('#motivoDevolucao_CD_MOTIVO_DEVOLUCAO').val();

    if (DT_DEVOLUCAO != "" && CD_MOTIVO_DEVOLUCAO == "") {
        ExibirCampo($('#validaCD_MOTIVO_DEVOLUCAO'));
        validacao = false;
    }
    else if (DT_DEVOLUCAO == "" && CD_MOTIVO_DEVOLUCAO != "") {
        ExibirCampo($('#validaDT_DEVOLUCAO'));
        validacao = false;
    }

    //var checkBox = document.getElementById("garantia");

    //if (validacao == true && checkBox.checked == false) {

    //    //var Datepicker = $.fn.datepicker.noConflict();  // return $.fn.button to previously assigned value
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker = Datepicker;  // give $().bootstrapBtn the Bootstrap functionality

    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker.val("");
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker.val();
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').val("");
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').val();
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker.value("");
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker.value();
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').value("");
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').value();
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker.value="";
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker.value=null;
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').value="";
    //    //$('#Form_DT_FIM_GARANTIA_REFORMA_container').value = null;

    //    //$('#DT_FIM_GARANTIA_REFORMA').datepicker.val("");
    //    //$('#DT_FIM_GARANTIA_REFORMA').datepicker.val();
    //    //$('#DT_FIM_GARANTIA_REFORMA').val("");
    //    //$('#DT_FIM_GARANTIA_REFORMA').val();
    //    //$('#DT_FIM_GARANTIA_REFORMA').datepicker.value("");
    //    //$('#DT_FIM_GARANTIA_REFORMA').datepicker.value();
    //    //$('#DT_FIM_GARANTIA_REFORMA').value("");
    //    //$('#DT_FIM_GARANTIA_REFORMA').value();
    //    //$('#DT_FIM_GARANTIA_REFORMA').datepicker.value = "";
    //    //$('#DT_FIM_GARANTIA_REFORMA').datepicker.value = null;
    //    //$('#DT_FIM_GARANTIA_REFORMA').value = "";
    //    //$('#DT_FIM_GARANTIA_REFORMA').value = null;
    //    ////$('#Form_DT_FIM_GARANTIA_REFORMA_container').val(null);


    //    $('#DT_FIM_GARANTIA_REFORMA').datepicker.setDate();
    //    $('#DT_FIM_GARANTIA_REFORMA').datepicker.remove();
    //    $('#DT_FIM_GARANTIA_REFORMA').datepicker.clearDates();
    //    $('#DT_FIM_GARANTIA_REFORMA').setDate();
    //    $('#DT_FIM_GARANTIA_REFORMA').remove();
    //    $('#DT_FIM_GARANTIA_REFORMA').clearDates();


    //}

    return validacao;
}

$("#cliente_CD_CLIENTE").change(function () {
    OcultarCampo($('#validaCD_CLIENTE'));
});

$("#ativoFixo_CD_ATIVO_FIXO").change(function () {
    OcultarCampo($('#validaCD_ATIVO_FIXO'));
    $('#DS_MODELO').val('');
});

$("#motivoDevolucao_CD_MOTIVO_DEVOLUCAO").change(function () {
    OcultarCampo($('#validaCD_MOTIVO_DEVOLUCAO'));
});

$("#DT_DEVOLUCAO").change(function () {
    OcultarCampo($('#validaDT_DEVOLUCAO'));
});

$('#DT_NOTAFISCAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_NOTAFISCAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_FIM_GARANTIA_REFORMA_container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});


//$("#DT_FIM_GARANTIA_REFORMA").datepicker({
//    showOn: "button",
//    buttonImage: "calendario.png",
//    buttonImageOnly: true,
//    language: "pt-BR",
//    autoclose: true,
//    todayHighlight: true
//});
//$("#DT_FIM_GARANTIA_REFORMA").datepicker({ dateFormat: 'dd-mm-yy' });

$('#DT_DEVOLUCAO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

//tipo_CD_TIPO
//cliente_CD_CLIENTE

//$('#cliente_CD_CLIENTE').on('change', function (e) {

//});


function carregarComboCliente() {
    var URL = URLAPI + "ClienteAPI/ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.clientes != null) {
                LoadClientes(res.clientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadClientes(clientes) {
    LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + ' (' + clientes[i].CD_CLIENTE + ') ' + clientes[i].EN_CIDADE + ' - ' + clientes[i].EN_ESTADO;
        //$("#cliente_CD_CLIENTE").append("<option value='" + clientes[i].CD_CLIENTE + "'>" + clientes[i].NM_CLIENTE + "</option>");
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

$('#fileUpload').click(function () {
    UploadFiles(URL_UPLOAD + "?pastaConstante=PastaAtivoClienteNF", function (data) {
        $('#DS_ARQUIVO_FOTO').val(data.file[0]);
        Alerta("Sucesso", "Upload efetuado com sucesso!");
    });
    return false;
});

$('#fileUpload2').click(function () {
    UploadFilesFoto(URL_UPLOAD + "?pastaConstante=PastaAtivoClienteNF", function (data) {
        $('#DS_ARQUIVO_FOTO2').val(data.file[0]);
        Alerta("Sucesso", "Upload efetuado com sucesso!");
    });
    return false;
});

$('#fileEraser, #fileEraser2').click(function () {

    $idAtivo = $('#ID_ATIVO_CLIENTE').val();
    $filename = this.name;
    $pastaAtivoClienteNF = "PastaAtivoClienteNF";
    $arquivo = this.getAttribute('for');

    ExcluirArquivoFoto($idAtivo, $filename, $pastaAtivoClienteNF, $arquivo);

});

function ExcluirArquivoFoto($idAtivo, $filename, $pastaAtivoClienteNF, $arquivo) {

    $url = URL_CLEAR + "?pastaConstante=" + $pastaAtivoClienteNF + "&fileName=" + $filename + "&idAtivo=" + $idAtivo + "&arquivo=" + $arquivo;

    $.ajax({
        type: 'POST',
        url: $url,
        //data: data,
        //dataType: "json",
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.confirmacao == true) {

                if ($arquivo == 'DS_ARQUIVO_FOTO')
                    $('#DS_ARQUIVO_FOTO').val('');
                else
                    $('#DS_ARQUIVO_FOTO2').val('');

                Alerta("Sucesso", "Arquivo excluido com sucesso!");
            }
        },
        error: function () {
            $("#loader").css("display", "none");
            Alerta("Falha", "Não foi possível excluir o arquivo.");
        },
    });
    return false;
}

function AlterarData(Data) {
    var URL = URLAlterarData + "?Data=" + Data + "&QtdDias=" + diasManutencao;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {

            var repartirData = res.DataFinal.split("/");
            var data = new Date(repartirData[1] + "/" + repartirData[0] + "/" + repartirData[2]);

            var dtGR = document.getElementById("DT_FIM_GARANTIA_REFORMA");
            dtGR.focus();
            var e = $.Event('keypress');
            e.which = 48; // Character 'A'
            $('DT_FIM_GARANTIA_REFORMA').trigger(e);
            //$('#DT_FIM_MANUTENCAO').datepicker('update', new Date(2011, 2, 5));
            //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker('update', res.DataFinal);

            $('#Form_DT_FIM_GARANTIA_REFORMA_container').val(data.toDateString().toString("dd/MM/yyyy"));
            //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker('update', data.toDateString("dd/MM/yyyy"));

            //$('#DT_FIM_MANUTENCAO').datepicker('update', new Date(2020, 1, 1));
        },
        error: function (res) {
            Alerta("ERRO", res.responseText);
        }
    });
}

function checarGarantia() {
    var checkBox = document.getElementById("garantia");

    if (checkBox.checked == true) {
        //$("#DT_FIM_GARANTIA_REFORMA").show();
        ExibirCampo($('#Form_DT_FIM_GARANTIA_REFORMA_container'));
        //if ($("#DT_NOTAFISCAL").val() != null && $("#DT_NOTAFISCAL").val() != '') {
        //    AlterarData($("#DT_NOTAFISCAL").val());
        //}
    } else {
        //$('#Form_DT_FIM_GARANTIA_REFORMA_container').datepicker('update', '');
        OcultarCampo($('#Form_DT_FIM_GARANTIA_REFORMA_container'));
    }

}

$('#btnConsultar').click(function () {

    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO").val();

    if (CD_ATIVO_FIXO == "" || CD_ATIVO_FIXO == undefined) {
        Alerta("Aviso", "Informe o Número do Ativo!");
        return;
    }
    else {
        if (CD_ATIVO_FIXO.length > 6) {
            Alerta("Aviso", "O Número do Ativo deve ter 6 caracteres no máximo!");
            return;

        }

    }



    var URL = URLObterModeloJson + "?CD_ATIVO_FIXO=" + CD_ATIVO_FIXO;

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

            if (res.Status == "true") {
                $('#DS_MODELO').val(res.DS_MODELO);
            }
            else
            {
                $('#DS_MODELO').val(res.DS_MODELO);
            }
        }
    });
});

