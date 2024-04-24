jQuery(document).ready(function () {

    var ID_OS = $("#ID_OS").val();

    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0 || (typeof ID_OS === "undefined")) {
        return;
    }

    carregarGridMVCReclamacao();
});

function carregarGridMVCReclamacao() {
    var ID_OS = $("#ID_OS").val();
    var VISUALIZACAO_OS = $("#visualizarOS").val();
    var URL = URLObterListaReclamacaoOS + "?ID_OS=" + ID_OS + "&visualizarOS=" + VISUALIZACAO_OS;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        //complete: function () {
        //    $("#loader").css("display", "block");
        //},
        success: function (res) {
            $("#loader").css("display", "block");
            if (res.Status == "Success") {
                $('#gridmvcReclamacaoOS').html(res.Html);
                for (var i = 0; i < res.lista.length; i++) {
                    if (res.lista[i].DS_ARQUIVO_FOTO == "" || res.lista[i].DS_ARQUIVO_FOTO == null) {
                        var idFoto = res.lista[i].ID_RELATORIO_RECLAMACAO;
                        $('#foto_' + idFoto).css("display", "none");
                    }
                }
            }
            $("#loader").css("display", "none");
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}

$('#btnNovaReclamacao').click(function () {
    var ST_STATUS_OS = $("#TpStatusOS_ST_STATUS_OS").val();

    if (ST_STATUS_OS == "" || ST_STATUS_OS == "0" || ST_STATUS_OS == 0) {
        ST_STATUS_OS = "0";
    }

    if (parseInt(ST_STATUS_OS) != statusAberta) {
        Alerta("Aviso", "Só é permitido adicionar <strong>Nova Reclamação</strong> para OS com status <strong>ABERTA</strong>!");
        return false;
    }

    carregarComboPeca($("#reclamacaoOs_pecaEntity_CD_PECA"));

    $("#reclamacaoOs_ID_RELATORIO_RECLAMACAO").val(0);
    $("#reclamacaoOs_CD_TIPO_RECLAMACAO").val(statusTpReclamacaoPeca);
    $("#reclamacaoOs_CD_TIPO_ATENDIMENTO").val(statusTpAtendimentoPecaTecnico);
    $("#reclamacaoOs_rrStatusEntity_ST_STATUS_RR").val(statusStReclamacaoNovo);
    $("#reclamacaoOs_rrStatusEntity_DS_STATUS_NOME").val(statusReclamacaoNovo);
    $("#reclamacaoOs_DS_MOTIVO").val('');
    $("#reclamacaoOs_DS_DESCRICAO").val('');
    $("#validaDS_DESCRICAO").css("display", "none");
    $("#validaDS_MOTIVO").css("display", "none");

    OcultarCampo($('#validaid_item_pedido_Peca'));
});

$('#btnSalvarReclamacaoModal').click(function () {
    var URL = URLAPI + "RelatorioReclamacaoAPI/Incluir";
    var ID_RELATORIO_RECLAMACAO = $("#reclamacaoOs_ID_RELATORIO_RECLAMACAO").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO").val();
    var CD_PECA = $("#reclamacaoOs_pecaEntity_CD_PECA option:selected").val();
    var ST_STATUS_RR = $("#reclamacaoOs_rrStatusEntity_ST_STATUS_RR").val();

    var CD_TIPO_RECLAMACAO = $("#reclamacaoOs_CD_TIPO_RECLAMACAO").val();
    var CD_TIPO_ATENDIMENTO = $("#reclamacaoOs_CD_TIPO_ATENDIMENTO").val();
    var DS_MOTIVO = $("#reclamacaoOs_DS_MOTIVO").val();
    var DS_DESCRICAO = $("#reclamacaoOs_DS_DESCRICAO").val();
    var DS_ARQUIVO_FOTO = $('#DS_ARQUIVO_FOTO').val();
    var ftNr = DS_ARQUIVO_FOTO.length;
    var TEMPO_ATENDIMENTO_FORMATADO = $("#reclamacaoOs_TEMPO_ATENDIMENTO_FORMATADO").val();

    if ((CD_TIPO_RECLAMACAO == statusTpReclamacaoPeca) && (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0)) {
        ExibirCampo($('#validaid_item_pedido_Peca'));
        return false;
    }
    else if (CD_TIPO_RECLAMACAO == "" || CD_TIPO_RECLAMACAO == "0" || CD_TIPO_RECLAMACAO == 0) {
        ExibirCampo($('#validaCD_TIPO_RECLAMACAO'));
        return false;
    }
    else if (CD_TIPO_ATENDIMENTO == "" || CD_TIPO_ATENDIMENTO == "0" || CD_TIPO_ATENDIMENTO == 0) {
        ExibirCampo($('#validaCD_TIPO_ATENDIMENTO'));
        return false;
    }
    else if (DS_MOTIVO == "" || (typeof DS_MOTIVO === "undefined")) {
        ExibirCampo($('#validaDS_MOTIVO'));
        return false;
    } else if (DS_DESCRICAO == "" || (typeof DS_DESCRICAO === "undefined")) {
        ExibirCampo($('#validaDS_DESCRICAO'));
        return false;
    }

    var relatorioReclamacaoEntity = {
        ST_STATUS_RR: ST_STATUS_RR,
        CD_CLIENTE: CD_CLIENTE,
        CD_TECNICO: CD_TECNICO,
        CD_ATIVO_FIXO: CD_ATIVO_FIXO,
        CD_PECA: CD_PECA,
        TipoReclamacaoRR: CD_TIPO_RECLAMACAO,
        TipoAtendimento: CD_TIPO_ATENDIMENTO,
        DS_MOTIVO: DS_MOTIVO,
        DS_DESCRICAO: DS_DESCRICAO,
        TEMPO_ATENDIMENTO: 0,
        ID_USUARIO_RESPONS: nidUsuario,
        TOKEN: ObterPrefixoTokenRegistro(nidUsuario),
        osPadraoEntity: {
            ID_OS: $("#ID_OS").val()
        },
        DS_ARQUIVO_FOTO: DS_ARQUIVO_FOTO,
        TEMPO_ATENDIMENTO_FORMATADO: TEMPO_ATENDIMENTO_FORMATADO
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(relatorioReclamacaoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != '' || res.MENSAGEM != '')
                Alerta("Aviso", res.MENSAGEM);
            else {
                Alerta("Aviso", MensagemGravacaoSucesso);
                carregarGridMVCReclamacao();
                $("#validaDS_DESCRICAO").css("display", "none");
                $("#validaDS_MOTIVO").css("display", "none");
                $("#reclamacaoOs_TEMPO_ATENDIMENTO_FORMATADO").val("");
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
});

function ExcluirReclamacaoOS(ID_RELATORIO_RECLAMACAO) {
    ConfirmarSimNao('Aviso', 'Confirma a <strong>EXCLUSÃO</strong> da Reclamação?', 'ExcluirReclamacaoOSConfirmada(' + ID_RELATORIO_RECLAMACAO + ')');
}

function ExcluirReclamacaoOSConfirmada(ID_RELATORIO_RECLAMACAO) {
    var URL = URLAPI + "RelatorioReclamacaoAPI/Excluir";

    if (ID_RELATORIO_RECLAMACAO == "" || ID_RELATORIO_RECLAMACAO == "0" || ID_RELATORIO_RECLAMACAO == 0) {
        Alerta("Aviso", "Reclamação inválida ou não informada!");
        return;
    }

    var relatorioReclamacaoEntity = {
        ID_RELATORIO_RECLAMACAO: ID_RELATORIO_RECLAMACAO,
         nidUsuarioAtualizacao: nidUsuario
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        data: JSON.stringify(relatorioReclamacaoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemExclusaoSucesso);
            carregarGridMVCReclamacao();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function VisualizarFoto(ID_RELATORIO_RECLAMACAO) {
    var nrReclamacao = ID_RELATORIO_RECLAMACAO;
    var foto = $('#' + nrReclamacao).val();
    var nrFT = foto.length;
    if (foto == null || foto == '') {
        Alerta("ERRO", "Não existe foto para esta reclamação!");
    }
    else {
        var url = 'data:image/jpeg;base64,' + foto;
        $("#ModalFoto img").attr("src", url);
        $("#ModalFoto img").attr("style", "max-width: 450px");
        $("#ModalFoto").modal("show");
    }
}

$('#FotoUp').change(function () {
    encodeImageFileURL();
})

function encodeImageFileURL() {
    var fileselect = document.getElementById("FotoUp").files;
    if (fileselect.length > 0) {
        fileselect = fileselect[0];
        var fileReader = new FileReader();

        fileReader.onload = function (FileLoadEvent) {
            var srcData = FileLoadEvent.target.result;

            var srcData = srcData.replace('data:image/png;base64,', '')
                                 .replace('data:image/jpeg;base64,', '')
                

            $('#DS_ARQUIVO_FOTO').val(srcData);
        }

        fileReader.readAsDataURL(fileselect);
    }
}