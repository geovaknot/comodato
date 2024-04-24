jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#DT_DATA_VISITA').mask('00/00/0000');
    $('#DT_DATA_VISITA_FIM').mask('00/00/0000');
    $('#HR_INICIO').mask('00:00:00');
    $('#HR_FIM').mask('00:00:00');
    $('#HR_TOTAL').mask('00:00:00');
    $('#DT_DATA_ABERTURA').mask('00/00/0000');
    $('#DT_DATA_ABERTURA_INICIO').mask('00/00/0000');
    $('#DT_DATA_ABERTURA_FIM').mask('00/00/0000');

    OcultarCampo($('#validaTecnico'));
    OcultarCampo($('#btnReordenarTop'));
    OcultarCampo($('#btnReordenarBottom'));

    carregarComboTecnico();
    carregarComboTpStatusVisitaOS();
    carregarComboRegiao();

    if (localStorage['Agenda_tecnico_CD_TECNICO'] != undefined && localStorage['Agenda_tecnico_CD_TECNICO'] != "") {
        $('#tecnico_CD_TECNICO').val(localStorage['Agenda_tecnico_CD_TECNICO']).trigger('change');
    }
    if (localStorage['Agenda_tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS'] != undefined && localStorage['Agenda_tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS'] != "") {
        $('#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS').val(localStorage['Agenda_tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS']).trigger('change');
    }
    if (localStorage['Agenda_regiao_CD_REGIAO'] != undefined && localStorage['Agenda_regiao_CD_REGIAO'] != "") {
        $('#regiao_CD_REGIAO').val(localStorage['Agenda_regiao_CD_REGIAO']).trigger('change');
    }

    carregarGridMVC();
});

$('#btnLimpar').click(function () {
    OcultarCampo($('#validaTecnico'));

    $('#tecnico_CD_TECNICO').val(null).trigger('change');
    $('#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS').val(null).trigger('change');
    $('#regiao_CD_REGIAO').val(null).trigger('change');
    $('#OS_ID_OS').val('');

    localStorage.removeItem('Agenda_tecnico_CD_TECNICO');
    localStorage.removeItem('Agenda_tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS');
    localStorage.removeItem('Agenda_regiao_CD_REGIAO');

    carregarGridMVC();
});

$('#btnFiltrar').click(function () {
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    carregarGridMVC();

    localStorage['Agenda_tecnico_CD_TECNICO'] = $("#tecnico_CD_TECNICO option:selected").val();
    localStorage['Agenda_tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS'] = $("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS option:selected").val();
    localStorage['Agenda_regiao_CD_REGIAO'] = $("#regiao_CD_REGIAO option:selected").val();
});

$('#btnReordenarTop').click(function () {
    btnReordenarConfirmar();
});

$('#btnReordenarBottom').click(function () {
    btnReordenarConfirmar();
});

function btnReordenarConfirmar() {
    ConfirmarSimNao('Aviso', 'Confirma a reordenação da Agenda?', 'btnReordenar()');
}

function btnReordenar() {
    var URL = URLAPI + "AgendaAPI/ReordenarTotal";
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    var AgendaReordenar = {
        cliente: {
            CD_CLIENTE: 0,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        NR_ORDENACAO: 0,
        TP_ACAO: '',
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(AgendaReordenar),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.mensagem != null || res.MENSAGEM != '') {
                Alerta("Aviso", res.mensagem)
                carregarGridMVC();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
    
};

$("#tecnico_CD_TECNICO").change(function () {
    OcultarCampo($('#validaTecnico'));

    OcultarCampo($('#gridmvc'));
    OcultarCampo($('#btnReordenarTop'));
    OcultarCampo($('#btnReordenarBottom'));
});

function carregarComboTecnico() {
    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
    var tecnicoEntity = {};

    //if (nidPerfil == perfilLiderEmpresaTecnica) {
    //    tecnicoEntity = {
    //        usuarioCoordenador: {
    //            nidUsuario: nidUsuario,
    //        },
    //    };
    //}

    tecnicoEntity = {
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
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tecnicoEntity),
        beforeSend: function () {
            ExibirCampo($('#loader'));
        },
        complete: function () {
            OcultarCampo($('#loader'));
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tecnicos != null) {
                LoadTecnicos(res.tecnicos);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
        
    });
}

function LoadTecnicos(tecnicosJO) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    //if (UsuarioTecnico == "True") {
    //    var cdTecnico = '';
    //    var contador = 0;

    //    for (i = 0; i < tecnicos.length; i++) {
    //        if (nidUsuario == tecnicos[i].usuario.nidUsuario) {
    //            MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);

    //            cdTecnico = tecnicos[i].CD_TECNICO;
    //            contador++;
    //        }
    //    }

    //    if (contador == 1) {
    //        $('#tecnico_CD_TECNICO').val(cdTecnico).trigger('change');
    //    }
    //}
    //else {
        for (i = 0; i < tecnicos.length; i++) {
            MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
        }
    //}
}

function carregarComboTpStatusVisitaOS() {
    var URL = URLAPI + "TpStatusVisitaOSAPI/ObterLista";

    var tpStatusVisitaOSEntity = {
        FL_STATUS_OS: 'N'
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tpStatusVisitaOSEntity),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tiposStatusVisitaOS != null) {
                LoadTpStatusVisitaOS(res.tiposStatusVisitaOS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadTpStatusVisitaOS(tiposStatusVisitaOS) {
    LimparCombo($("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS"));

    for (i = 0; i < tiposStatusVisitaOS.length; i++) {
        MontarCombo($("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS"), tiposStatusVisitaOS[i].ID_TP_STATUS_VISITA_OS, tiposStatusVisitaOS[i].DS_TP_STATUS_VISITA_OS);
    }
}

function carregarComboRegiao() {
    var URL = URLAPI + "RegiaoAPI/ObterLista";

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
            $("#loader").css("display", "none");
            //if (res.REGIAO != null) {
            //    LoadRegioes(res.REGIAO);
            //}
            if (res != null) {
                LoadRegioes(res);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO",JSON.parse(res.responseText).Message );
        }

    });
}

function LoadRegioes(regioes) {
    LimparCombo($("#regiao_CD_REGIAO"));
    //var regioes = JSON.parse(regioesJO);
    for (i = 0; i < regioes.regioes.length; i++) {
        MontarCombo($("#regiao_CD_REGIAO"), regioes.regioes[i].CD_REGIAO, regioes.regioes[i].DS_REGIAO);
    }
}

function carregarGridMVC() {
    //ExibirCampo($('#gridmvc'));
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var ST_TP_STATUS_VISITA_OS = $("#tpStatusVisitaOS_ST_TP_STATUS_VISITA_OS option:selected").val();
    var CD_REGIAO = $("#regiao_CD_REGIAO option:selected").val();
    //var ID_OS = $('#OS_ID_OS').val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0)
        return;

    var URL = URLObterLista + "?CD_TECNICO=" + CD_TECNICO + "&CD_REGIAO=" + CD_REGIAO + "&ST_TP_STATUS_VISITA_OS=" + ST_TP_STATUS_VISITA_OS; //+ "&ID_OS=" + ID_OS;

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
            if (res.Status == "Success") {
                ExibirCampo($('#gridmvc'));
                $('#gridmvc').html(res.Html);
                ExibirCampo($('#btnReordenarTop'));
                ExibirCampo($('#btnReordenarBottom'));
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function Subir(NR_ORDENACAO) {
    var URL = URLAPI + "AgendaAPI/Reordenar";
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    var AgendaReordenar = {
        cliente: {
            CD_CLIENTE: 0,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        NR_ORDENACAO: NR_ORDENACAO,
        TP_ACAO: 'S',
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(AgendaReordenar),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
            Alerta("ERRO", res.responseText);
        }
    });

}

function Descer(NR_ORDENACAO) {
    var URL = URLAPI + "AgendaAPI/Reordenar";
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    var AgendaReordenar = {
        cliente: {
            CD_CLIENTE: 0,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        NR_ORDENACAO: NR_ORDENACAO,
        TP_ACAO: 'D',
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(AgendaReordenar),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
            Alerta("ERRO", res.responseText);
        }
    });

}

function Primeiro(NR_ORDENACAO) {
    var URL = URLAPI + "AgendaAPI/Reordenar";
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    var AgendaReordenar = {
        cliente: {
            CD_CLIENTE: 0,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        NR_ORDENACAO: NR_ORDENACAO,
        TP_ACAO: 'P',
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(AgendaReordenar),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
            Alerta("ERRO", res.responseText);
        }
    });

}

function Ultimo(NR_ORDENACAO) {
    var URL = URLAPI + "AgendaAPI/Reordenar";
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    var AgendaReordenar = {
        cliente: {
            CD_CLIENTE: 0,
        },
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        NR_ORDENACAO: NR_ORDENACAO,
        TP_ACAO: 'U',
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(AgendaReordenar),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            carregarGridMVC();
            Alerta("ERRO", res.responseText);
        }
    });

}
