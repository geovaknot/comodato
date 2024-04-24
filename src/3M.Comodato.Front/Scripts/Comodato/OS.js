jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    ConfigurarCamposFormulario()

    var ID_OS = $("#ID_OS").val();
    if (ID_OS == "" || ID_OS == "0" || ID_OS == 0) {
        AlertaRedirect("Aviso", "OS inválida ou não informada!", "window.history.back();");
        return;
    }

    if (typeof ID_OS === "undefined") {
        OcultarCampo($('#tabsAdicionais'));
    }

    ConfigurarComboTecnico();
    ConfigurarBotoesAcao();
});

function ConfigurarComboTecnico() {

    if (UsuarioTecnico.toLowerCase() === 'true') {
        $("#tecnico_CD_TECNICO").prop("disabled", true);
        $('#CodigoTecnico').val($("#tecnico_CD_TECNICO").val()).trigger('change');
    }
    else {
        $("#tecnico_CD_TECNICO").prop("disabled", false);
        var cd_Tecnico = localStorage.getItem("Os_tecnico_CD_TECNICO");
        if (cd_Tecnico != null && cd_Tecnico != "" && cd_Tecnico != 0) {
            $("#tecnico_CD_TECNICO").val(cd_Tecnico).change();
            $("#tecnico_CD_TECNICO").prop("disabled", true);
            $('#CodigoTecnico').val($("#tecnico_CD_TECNICO").val()).trigger('change');
        }
        else {
            $("#tecnico_CD_TECNICO").prop("disabled", true);
            $('#CodigoTecnico').val($("#tecnico_CD_TECNICO").val()).trigger('change');
        }
    }
}

$("#tecnico_CD_TECNICO").change(function () {
    if (UsuarioTecnico.toLowerCase() !== 'true') {

    }
})

function ConfigurarCamposFormulario() {
    OcultarCampo($('#CodigoTecnico'));
    $('#CodigoTecnico').prop("disabled", false);

    var HR_FIM = $("#HR_FIM").val();
    var ST_STATUS_OS = parseInt($("#TpStatusOS_ST_STATUS_OS").val());
                                   
    if (ST_STATUS_OS == statusCancelada || ST_STATUS_OS == statusFinalizada) {
        OcultarCampo($('#btnGravar'));
    }

    if (ST_STATUS_OS == statusCancelada || ST_STATUS_OS == statusIniciar) {
        OcultarCampo($('#btnCancelar'));
    }

    if (ST_STATUS_OS != statusAberta || HR_FIM == "") {
        OcultarCampo($('#btnFinalizar'));
    }

    if (ST_STATUS_OS == statusAberta) {
        $("#cliente_CD_CLIENTE").prop("disabled", true);
        $("#ativoFixo_CD_ATIVO_FIXO").prop("disabled", true);
    }
}

$('#DT_DATA_OS-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayBtn: "linked",
    todayHighlight: true,
    orientation: "bottom"
});

$('#btnGravar').click(function (event) {

    var DT_DATA_OS = $("#DT_DATA_OS").val();

    if (CompararDataInformadaInferiorDataAtual(DT_DATA_OS) == false) {

        if (ValidarHoraInicio() == false) {
            event.preventDefault();
            ConfigurarBotoesAcao();
            return;
        }

        if (ValidarHoraTermino() == false) {
            event.preventDefault();
            ConfigurarBotoesAcao();
            return;
        }

        if (ValidarStatus_HrInicio() == false) {
            event.preventDefault();
            ConfigurarBotoesAcao();
            return;
        }
    }
    else {

        if (CompararHoraInicioSuperiorHoraTermino() == true) {
            event.preventDefault();
            Alerta("Aviso", "Horário de Início não pode ser superior ou igual ao Horário de Término.");
            ConfigurarBotoesAcao();
            return;
        }

        if (ValidarStatus_HrInicio() == false) {
            event.preventDefault();
            ConfigurarBotoesAcao();
            return;
        }
        
    }

    Gravar(event);
});

function Gravar(event) {
   
    var ID_OS = $("#ID_OS").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var DT_DATA_OS = $("#DT_DATA_OS").val();
    var CD_TIPO_OS = $("#TpOS_CD_TIPO_OS").val();
    var DS_OBSERVACAO = $("#DS_OBSERVACAO").val();
    var DS_RESPONSAVEL = $("#DS_RESPONSAVEL").val();
    var EMAIL = $("#EMAIL").val();
    var NOME_LINHA = $("#NOME_LINHA").val();

    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        Alerta("Aviso", "Cliente inválido ou não informado!");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        Alerta("Aviso", "Técnico inválido ou não informado!");
        return;
    }
    else if (ID_OS == "" || ID_OS == "0" || ID_OS == 0) {
        AlertaRedirect("Aviso", "OS inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_TIPO_OS == "" || CD_TIPO_OS == "0" || CD_TIPO_OS == 0) {
        Alerta("Aviso", "Tipo da OS inválido ou não informado!");
        return;
    }
    else if (CD_ATIVO_FIXO == "" || CD_ATIVO_FIXO == "0" || CD_ATIVO_FIXO == 0) {
        Alerta("Aviso", "Ativo inválido ou não informado!");
        return;
    }
    else if (ValidarData(DT_DATA_OS) == false) {
        Alerta("Aviso", "Data da OS inválida ou não informada!");
        return;
    }

    if (!ValidarDataInformadaSuperiorDataAtual()) {
        return;
    }

    if (UsuarioTecnico.toLowerCase() === 'true') {
        if (!ValidarApontamentoRetroativo()) {
            return;
        }

    }

    
    const horaInicio = $("#HR_INICIO").val();
    const horaTermino = $("#HR_FIM").val();

    if (horaInicio != "" || horaTermino != "") {

        ValidarOs(event);
    }

}

function ValidarDataInformadaSuperiorDataAtual() {
    var DT_DATA_OS = $("#DT_DATA_OS").val();

    if (CompararDataInformadaSuperiorDataAtual(DT_DATA_OS)) {

        Alerta("Aviso", "Data do apontamento não pode ser superior a data do dia.");
        $("#DT_DATA_OS").val("");
        return false;
    }
    else {
        return true;
    }
}

function ValidarApontamentoRetroativo() {
    let dataOs = $("#DT_DATA_OS").val();

    if (ValidarDataMinimaApontamento(dataOs) == false) {
        Alerta("Aviso", "Apontamento retroativo é válido somente até o dia 5 do mês corrente, referente ao mês anterior.");
        $("#DT_DATA_OS").val("");
        return false;
    }
    else {
        return true;
    }
}

$('#btnCancelar').click(function (event) {
    event.preventDefault();
    var el = $(this);
    el.prop('disabled', true);

    if (ValidarCancelarOs() == false) {
        el.prop('disabled', false);
        return;
    }

    var idKey = $("#idKey").val();
    const url = URLCancelar + "?idKey=" + idKey;

    $.ajax({
        type: 'GET',
        url: url,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        processData: true,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.Status == "Success") {
                OcultarCampo($("#btnCancelar"));
                AlertaRedirect("Aviso", "OS Cancelada com sucesso!");
                setTimeout(function () {// wait for 5 secs(2)
                    var urlAtual = window.location.href;
                    var arrUrl = urlAtual.split("/");
                    var novaURL = "";
                    for (var i = 0; i < arrUrl.length - 1; i++) {
                        novaURL += arrUrl[i] + "/";
                    }
                    window.location.href = novaURL;
                    //window.location.href = "/OsPadrao"; // then reload the page.(3)
                }, 5000);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
});

function ValidarCancelarOs() {

    const ST_STATUS_OS = parseInt($("#TpStatusOS_ST_STATUS_OS").val());

    if (ST_STATUS_OS != statusIniciar && ST_STATUS_OS != statusAberta && ST_STATUS_OS != statusFinalizada) {
        AlertaRedirect("Aviso", "Somente OS com status AGUARDANDO INICIO, ABERTA ou FINALIZADA pode ser cancelada.", "window.history.back();");
        return false;
    }

    const dataOs = $("#DT_DATA_OS").val();

    if (UsuarioTecnico.toLowerCase() === 'true') {

        if (ValidarDataMinimaApontamento(dataOs) == false && ST_STATUS_OS == statusFinalizada) {
            Alerta("Aviso", "Cancelamento de OS <strong>FINALIZADA</strong> é válido somente até o dia 5 do mês corrente, referente ao mês anterior.");
            return false;
        }
    }
    else {

        if (ValidarDataMinimaParaCancelamentoPerfilAdmin(dataOs) == false) {
            Alerta("Aviso", "Cancelamento de OS válido somente para o mês anterior ao mês corrente.");
            return false;
        }
    }

    return true;
}

function IniciarVisita() {
    $("#HR_INICIO").val(ObterHoraAtual());
    ConfigurarBotoesAcao();
}

function PararVisita() {
    $("#HR_FIM").val(ObterHoraAtual());
    ConfigurarBotoesAcao();
    ObterTempoTotalOs();
}

$("#HR_INICIO").blur(function () {

    var DT_DATA_OS = $("#DT_DATA_OS").val();

    if (CompararDataInformadaInferiorDataAtual(DT_DATA_OS) == false) {
        ValidarHoraInicio();
    }

    ConfigurarBotoesAcao();
    ObterTempoTotalOs();
});

function ValidarStatus_HrInicio() {
    var hrIni = $("#HR_INICIO").val();
    var ST_STATUS_OS = parseInt($("#TpStatusOS_ST_STATUS_OS").val());

    if ((hrIni == "" && ST_STATUS_OS == statusAberta) || (hrIni == null && ST_STATUS_OS == statusAberta)) {
        Alerta("Aviso", "Hora de Início não pode ser vazio quando a OS estiver aberta.");
        return false;
    }

    return true;
}

function ValidarHoraInicio() {

    var hrIni = $("#HR_INICIO").val();
    var ST_STATUS_OS = parseInt($("#TpStatusOS_ST_STATUS_OS").val()); 

    if (CompararHoraInicioSuperiorHoraTermino()) {
        Alerta("Aviso", "Horário de Início não pode ser superior ou igual ao Horário de Término.");
        $("#HR_INICIO").val("");
        return false;
    }
    else if (CompararHora($("#HR_INICIO").val(), ObterHoraAtual())) {
        Alerta("Aviso", "Hora de Início não pode ser superior a Hora atual.");
        $("#HR_INICIO").val("");
        return false;
    } else if ((hrIni == "" && ST_STATUS_OS == statusAberta) || (hrIni == null && ST_STATUS_OS == statusAberta)) {
        Alerta("Aviso", "Hora de Início não pode ser vazio quando a OS estiver aberta.");
        return false;
    }

    return true;
}

$("#HR_FIM").blur(function () {

    var DT_DATA_OS = $("#DT_DATA_OS").val();

    if (CompararDataInformadaInferiorDataAtual(DT_DATA_OS) == false) {
        ValidarHoraTermino();
    }

    ConfigurarBotoesAcao();
    ObterTempoTotalOs();
});

function ValidarHoraTermino() {

    if (CompararHoraInicioSuperiorHoraTermino()) {
        Alerta("Aviso", "Horário de Término não pode ser inferior ou igual ao Horário de Início.");
        $("#HR_FIM").val("");
        return false;
    }
    else if (CompararHora($("#HR_FIM").val(), ObterHoraAtual())) {
        Alerta("Aviso", "Hora de Término não pode ser superior a Hora atual.");
        $("#HR_FIM").val("");
        return false;
    }

    return true;
}

function CompararHoraInicioSuperiorHoraTermino() {

    const horaInicio = $("#HR_INICIO").val();
    const horaTermino = $("#HR_FIM").val();

    if (horaInicio != "" && horaTermino != "") {

        return CompararHoraTerminoInferiorIgualHoraInicio(horaInicio, horaTermino);
    }

    return false;
}

function ObterTempoTotalOs() {

    const horaInicio = $("#HR_INICIO").val();
    const horaTermino = $("#HR_FIM").val();

    if (horaInicio != "" && horaTermino != "") {

        $('#HR_TOTAL').val(ObterDiferencaEntreHora(horaInicio, horaTermino));
    }
    else {

        $('#HR_TOTAL').val("");
    }
}

function ConfigurarBotoesAcao() {
    var HR_INICIO = $("#HR_INICIO").val();

    if (HR_INICIO == "") {
        OcultarCampo($("#btnParar"));
        ExibirCampo($("#btnIniciar"))
        $("#HR_FIM").val("");
        $("#HR_FIM").prop("disabled", true);
    }
    else {
        OcultarCampo($("#btnIniciar"));
        ExibirCampo($("#btnParar"))
        $("#HR_FIM").prop("disabled", false);
    }

    var HR_FIM = $("#HR_FIM").val();
    const ST_STATUS_OS = parseInt($("#TpStatusOS_ST_STATUS_OS").val());

    if (HR_FIM != "" && ST_STATUS_OS == statusAberta) {
        ExibirCampo($("#btnFinalizar"))
    }
    else {
        OcultarCampo($("#btnFinalizar"));
    }
}

function ValidarOs(event) {

    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var CodigoCli = $("#cliente_CD_CLIENTE").val();
    var ID_OS = $("#ID_OS").val();
    var DT_DATA_OS = $("#DT_DATA_OS").val();
    var HR_INICIO = $("#HR_INICIO").val();
    var HR_FIM = $("#HR_FIM").val();
    var DS_OBSERVACAO = $("#DS_OBSERVACAO").val();

    ID_OS = (ID_OS == "") || (typeof ID_OS === "undefined") ? 0 : ID_OS;

    var url = URLPermiteIncluirOsSalvar + "?codTecnico=" + CD_TECNICO + "&idOs=" + ID_OS + "&dataOs=" + DT_DATA_OS + "&horaInicio=" + HR_INICIO + "&horaFim=" + HR_FIM + "&DS_OBS=" + DS_OBSERVACAO;

    $.ajax({
        type: 'GET',
        url: url,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        processData: true,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            if (res.Resultado != null) {
                event.preventDefault();
                Alerta("Aviso", "Você já tem Ordem de Serviço ou Visita registrada neste período, corrija as informações deste formulário, se necessário");
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

$("#btnFinalizar").click(function (event) {
    event.preventDefault();

    var el = $(this);
    el.prop('disabled', true);

    const HR_FIM = $("#HR_FIM").val();
    const ST_STATUS_OS = parseInt($("#TpStatusOS_ST_STATUS_OS").val());

    if (HR_FIM == "" || ST_STATUS_OS != statusAberta) {
        AlertaRedirect("Aviso", "Somente OS com status ABERTA e Hora de Término apontada pode ser cancelada.", "window.history.back();");
        el.prop('disabled', false);
        return;
    }

    ConfirmarSimNao('Aviso', 'Confirma finalização da OS?', 'btnFinalizacaoConfirmada()');
    el.prop('disabled', false);
});

function btnFinalizacaoConfirmada() {

    $("#TpStatusOS_ST_STATUS_OS").val(statusFinalizada);

    $("#btnGravar").trigger('click');
}

function carregarComboPeca(Obj) {
    var CD_ATIVO_FIXO = $("#ativoFixo_CD_ATIVO_FIXO option:selected").val();
    var URL = URLAPI + "PlanoZeroAPI/ObterListaModelo?CD_ATIVO_FIXO=" + CD_ATIVO_FIXO;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            if (res.PlanoZero != null) {
                LoadPecas(Obj, res.PlanoZero);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

$("#cliente_CD_CLIENTE").change(function () {

    carregarComboAtivosCliente();
});


function LoadPecas(Obj, listaPecas) {
    LimparCombo(Obj);

    for (i = 0; i < listaPecas.length; i++) {
        MontarCombo(Obj, listaPecas[i].PecaModel.CD_PECA, listaPecas[i].PecaModel.DS_PECA);
    }
}

function carregarComboAtivosCliente() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        LimparCombo($("#ativoFixo_CD_ATIVO_FIXO"));
        return;
    }

    var URL = URLAPI + "AtivoAPI/ObterListaAtivoCliente?CD_Cliente=" + CD_CLIENTE + "&SomenteATIVOSsemDTDEVOLUCAO=true";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.listaAtivosClientes != null) {
                LoadAtivosCliente(res.listaAtivosClientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadAtivosCliente(listaAtivosClientesJO) {
    LimparCombo($("#ativoFixo_CD_ATIVO_FIXO"));

    var listaAtivosClientes = JSON.parse(listaAtivosClientesJO);

    for (i = 0; i < listaAtivosClientes.length; i++) {
        MontarCombo($("#ativoFixo_CD_ATIVO_FIXO"), listaAtivosClientes[i].CD_ATIVO_FIXO, listaAtivosClientes[i].DS_ATIVO_FIXO + ' - ' + listaAtivosClientes[i].DT_ULTIMA_MANUTENCAO);
    }
}
