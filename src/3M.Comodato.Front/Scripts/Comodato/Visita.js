jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    ConfigurarCamposFormulario()

    var ID_VISITA = $("#ID_VISITA").val();

    if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        AlertaRedirect("Aviso", "Visita inválida ou não informada!", "window.history.back();");
        return;
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
        var cd_Tecnico = localStorage.getItem("Visita_tecnico_CD_TECNICO");
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

function ConfigurarCamposFormulario() {
    OcultarCampo($('#CodigoTecnico'));
    $('#CodigoTecnico').prop("disabled", false);

    var ST_TP_STATUS_VISITA = parseInt($("#TpStatusVisita_ST_STATUS_VISITA").val());

    if (ST_TP_STATUS_VISITA == statusCancelada || ST_TP_STATUS_VISITA == statusFinalizada) {
        OcultarCampo($('#btnGravar'));
    }

    if (ST_TP_STATUS_VISITA == statusCancelada) {
        OcultarCampo($('#btnCancelar'));
    }

    var DT_DATA_VISITA = $("#DT_DATA_VISITA").val();

    if (DT_DATA_VISITA == "") {
        $("#DT_DATA_VISITA").val(Geradatadehoje());
    }
}

$('#DT_DATA_VISITA-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayBtn: "linked",
    todayHighlight: true,
    orientation: "bottom"
});

$('#DT_DATA_LOG_VISITA-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayBtn: "linked",
    todayHighlight: true,
    orientation: "bottom"
});

$('#btnGravar').click(function (event) {

    var DT_DATA_VISITA = $("#DT_DATA_VISITA").val();

    if (CompararDataInformadaInferiorDataAtual(DT_DATA_VISITA) == false) {

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
    }
    else {

        if (CompararHoraInicioSuperiorHoraTermino() == true) {
            event.preventDefault();
            Alerta("Aviso", "Horário de Início não pode ser superior ou igual ao Horário de Término.");
            ConfigurarBotoesAcao();
            return;
        }
    }

    Gravar(event);
});

function Gravar(event) {
   
    var ID_VISITA = $("#ID_VISITA").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var DT_DATA_VISITA = $("#DT_DATA_VISITA").val();
    var CD_MOTIVO_VISITA = $("#TpMotivoVisita_CD_MOTIVO_VISITA").val();

    var DS_RESPONSAVEL = $("#DS_RESPONSAVEL").val();

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        Alerta("Aviso", "Cliente inválido ou não informado!");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        Alerta("Aviso", "Técnico inválido ou não informado!");
        return;
    }
    else if (ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0) {
        AlertaRedirect("Aviso", "Visita inválida ou não informada!", "window.history.back();");
        return;
    }
    else if (CD_MOTIVO_VISITA == "" || CD_MOTIVO_VISITA == "0" || CD_MOTIVO_VISITA == 0) {
        Alerta("Aviso", "Motivo da Visita inválido ou não informado!");
        return;
    }
    else if (ValidarData(DT_DATA_VISITA) == false) {
        Alerta("Aviso", "Data da Visita inválida ou não informada!");
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

    if (horaInicio != "" && horaTermino != "") {

        ValidarVisita(event);

    }
}

function ValidarDataInformadaSuperiorDataAtual() {
    var DT_DATA_VISITA = $("#DT_DATA_VISITA").val();

    if (CompararDataInformadaSuperiorDataAtual(DT_DATA_VISITA)) {

        Alerta("Aviso", "Data do apontamento não pode ser superior a data do dia.");
        $("#DT_DATA_VISITA").val("");
        return false;
    }
    else {
        return true;
    }
}

function ValidarApontamentoRetroativo() {
    let dataVisita = $("#DT_DATA_VISITA").val();

    if (ValidarDataMinimaApontamento(dataVisita) == false) {
        Alerta("Aviso", "Apontamento retroativo é válido somente até o dia 5 do mês corrente, referente ao mês anterior.");
        $("#DT_DATA_VISITA").val("");
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

    if (ValidarCancelarVisita() == false) {
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
                AlertaRedirect("Aviso", "Visita Cancelada com sucesso!", "window.history.back();");
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
});

function ValidarCancelarVisita() {

    const ST_TP_STATUS_VISITA = parseInt($("#TpStatusVisita_ST_STATUS_VISITA").val());

    if (ST_TP_STATUS_VISITA != statusIniciar && ST_TP_STATUS_VISITA != statusAberta && ST_TP_STATUS_VISITA != statusFinalizada) {
        AlertaRedirect("Aviso", "Somente visita com status INICIAR, ABERTA ou FINALIZADA pode ser cancelada.", "window.history.back();");
        return false;
    }

    const dataVisita = $("#DT_DATA_VISITA").val();

    if (UsuarioTecnico.toLowerCase() === 'true') {

        if (ValidarDataMinimaApontamento(dataVisita) == false && ST_TP_STATUS_VISITA == statusFinalizada) {
            Alerta("Aviso", "Cancelamento de Visita <strong>FINALIZADA</strong> é válido somente até o dia 5 do mês corrente, referente ao mês anterior.");
            return false;
        }
    } 
    else {

        if (ValidarDataMinimaParaCancelamentoPerfilAdmin(dataVisita) == false) {
            Alerta("Aviso", "Cancelamento de Visita válido somente para o mês anterior ao mês corrente.");
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
    ObterTempoTotalVisita();
}

$("#HR_INICIO").blur(function () {

    var DT_DATA_VISITA = $("#DT_DATA_VISITA").val();

    if (CompararDataInformadaInferiorDataAtual(DT_DATA_VISITA) == false) {
        ValidarHoraInicio();
    }

    ConfigurarBotoesAcao();
    ObterTempoTotalVisita();
});

function ValidarHoraInicio(){

    if (CompararHoraInicioSuperiorHoraTermino()) {
        Alerta("Aviso", "Horário de Início não pode ser superior ou igual ao Horário de Término.");
        $("#HR_INICIO").val("");
        return false;
    }
    else if (CompararHora($("#HR_INICIO").val(), ObterHoraAtual())) {
        Alerta("Aviso", "Hora de Início não pode ser superior a Hora atual.");
        $("#HR_INICIO").val("");
        return false;
    }

    return true;
}

$("#HR_FIM").blur(function () {

    var DT_DATA_VISITA = $("#DT_DATA_VISITA").val();

    if (CompararDataInformadaInferiorDataAtual(DT_DATA_VISITA) == false) {
        ValidarHoraTermino();
    }

    ConfigurarBotoesAcao();
    ObterTempoTotalVisita();
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

function ObterTempoTotalVisita() {

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
}

$("#tecnico_CD_TECNICO").change(function(){

    var CD_TECNICO = $("#tecnico_CD_TECNICO").val()
    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
    var tecnicoEntity = {};
    ////var token = sessionStorage.getItem("token");

    tecnicoEntity = {
        CD_TECNICO: CD_TECNICO,
        usuario: {
            nidUsuario: nidUsuario
        }
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(tecnicoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            ExibirCampo($('#loader'));
        },
        complete: function () {
            OcultarCampo($('#loader'));
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tecnicos != null) {
                var tecnicos = JSON.parse(res.tecnicos);
                var tecnico = tecnicos[0];
                $('#tecnico_empresa_NM_Empresa').val(tecnico.empresa.NM_Empresa);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
});

function ValidarVisita(event) {

    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_VISITA = $("#ID_VISITA").val();
    var DT_DATA_VISITA = $("#DT_DATA_VISITA").val();
    var HR_INICIO = $("#HR_INICIO").val();
    var HR_FIM = $("#HR_FIM").val();

    ID_VISITA = (ID_VISITA == "") || (typeof ID_VISITA === "undefined") ? 0 : ID_VISITA;

    var url = URLPermiteIncluirVisitaSalvar + "?codTecnico=" + CD_TECNICO + "&idVisita=" + ID_VISITA + "&dataVisita=" + DT_DATA_VISITA + "&horaInicio=" + HR_INICIO + "&horaFim=" + HR_FIM;

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
                Alerta("Aviso", "Você já tem Visita ou Ordem de Serviço registrada neste período, corrija as informações deste formulário, se necessário");
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

$("#tecnico_CD_TECNICO").change(function () {

    var CD_TECNICO = $("#tecnico_CD_TECNICO").val()
    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
    var tecnicoEntity = {};

    tecnicoEntity = {
        CD_TECNICO: CD_TECNICO,
        usuario: {
            nidUsuario: nidUsuario
        }
    };

    ////var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        data: JSON.stringify(tecnicoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            ExibirCampo($('#loader'));
        },
        complete: function () {
            OcultarCampo($('#loader'));
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tecnicos != null) {
                var tecnicos = JSON.parse(res.tecnicos);
                var tecnico = tecnicos[0];
                $('#tecnico_empresa_NM_Empresa').val(tecnico.empresa.NM_Empresa);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
});