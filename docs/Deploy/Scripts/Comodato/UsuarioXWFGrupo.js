jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    carregarComboUsuario();
    carregarGruposWF();
    carregarGridMVC();
});

function carregarComboUsuario() {

    var URL = URLAPI + "UsuarioAPI/ObterLista";

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
            if (res.usuarios != null) {
                LoadUsuarios(res.usuarios);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadUsuarios(usuariosJO) {
    LimparCombo($("#usuario_nidUsuario"));

    var usuarios = JSON.parse(usuariosJO);
    for (i = 0; i < usuarios.length; i++) {
        //$("#tecnico_CD_TECNICO").append("<option value='" + usuarios[i].tecnico.CD_TECNICO + "'>" + usuarios[i].tecnico.NM_TECNICO + "</option>");
        var cnmNome = usuarios[i].cnmNome + ' (' + usuarios[i].cdsLogin + ')';
        MontarCombo($("#usuario_nidUsuario"), usuarios[i].nidUsuario, cnmNome);
    }
}


function carregarGruposWF() {

    var URL = URLAPI + "WFGrupoAPI/ObterLista";

    $.ajax({
        type: 'GET',
        url: URL,
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
            if (res.grupos != null) {
                LoadGrupo(res.grupos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadGrupo(grupoJO) {
    LimparCombo($("#wfGrupo_ID_GRUPOWF"));

    var grupos = JSON.parse(grupoJO);
    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#wfGrupo_ID_GRUPOWF"), grupos[i].ID_GRUPOWF, grupos[i].CD_GRUPOWF);
    }
}

function carregarGridMVC() {

    var URL = URLObterLista;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            $('#gridmvc').html(res.Html);

        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

$('#btnAdicionar').click(function () {
    var URL = URLAPI + "WfGrupoUsuAPI/Incluir";
    
    var ID_GRUPOWFselecionado = $("#wfGrupo_ID_GRUPOWF option:selected").val();
    var nidUsuarioSelecionado = $("#usuario_nidUsuario option:selected").val();
    var PrioridadeSelecionada = $("#NM_PRIORIDADE option:selected").val();


    var UsuarioXWFGrupoEntity = {
        grupoWf: {
            ID_GRUPOWF: ID_GRUPOWFselecionado,
        },
        usuario: {
            nidUsuario: nidUsuarioSelecionado,
        },
        NM_PRIORIDADE: PrioridadeSelecionada,
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(UsuarioXWFGrupoEntity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

});

function atualizarPagina() {
    carregarComboUsuario();
    carregarGruposWF();
    carregarGridMVC();
    $("#txtTipoGrupo").val("");
    $("#txtDescricaoGrupo").val("");
}

function ExcluirConfirmar(ID_GRUPOWF_USU) {
    ConfirmarSimNao('Aviso', 'Confirma a exclusão do registro?', 'Excluir(' + ID_GRUPOWF_USU + ')');
}

function Excluir(ID_GRUPOWF_USU) {
    var URL = URLAPI + "WfGrupoUsuAPI/Excluir";

    var UsuarioXWFGrupoEntity = {
        ID_GRUPOWF_USU: ID_GRUPOWF_USU,
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(UsuarioXWFGrupoEntity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

$('#wfGrupo_ID_GRUPOWF').on("select2:select", function (e) {
    $('#txtDescricaoGrupo').val('');
    $('#txtTipoGrupo').val('');

    var ccdGrupo = e.params.data.id;
    if (ccdGrupo == "" || ccdGrupo == "0" || ccdGrupo == 0) {
        return;
    }

    var url = URLAPI + "WFGrupoAPI/ObterDados?idGrupoWF=" + ccdGrupo;

    $.ajax({

        type: 'GET',
        url: url,
        dataType: 'json',
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            $('#txtDescricaoGrupo').val(res.grupos[0].DS_GRUPOWF);
            if ((res.grupos[0].TP_GRUPOWF).trim() == "E") {
                $('#txtTipoGrupo').val("Envio");
            } else if ((res.grupos[0].TP_GRUPOWF).trim() == "D") {
                $('#txtTipoGrupo').val("Devolução");
            }
            //$('#txtTipoGrupo').val(res.grupos[0].TP_GRUPOWF);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

});