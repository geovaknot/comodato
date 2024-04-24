$().ready(function () {
    LimparScrollWorkflow();
    DefinirExibicaoAtual();
    CarregarSituacaoStatus();
    DefinirFluxoStatusPedido();
    CarregarGRIDMensagem();
    CarregarGrupoResponsavel();

    $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA').mask('00/00/0000');
    $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA').mask('00/00/0000');
    $('#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS').mask('00/00/0000');
    $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M').mask('00/00/0000');
    $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO').mask('00/00/0000');

    OcultarCampo($('#btnSalvarRascunho'));
    $("#btnSalvarSubmeter").prop('value', 'Salvar');

    var ST_STATUS_PEDIDO = parseInt($('#ST_STATUS_PEDIDO').val());
    if (ST_STATUS_PEDIDO == DevolvidoPlanejam || ST_STATUS_PEDIDO == Cancelado) {
        OcultarCampo($('#btnSalvarSubmeter'));
        OcultarCampo($('#btnAcao'));
        OcultarCampo($('#btnNovoHistorico'));
        OcultarCampo($('#btnAdicionarEquipamento'));
    }

});

$('#DT_RETIRADA_AGENDADA-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_RETIRADA_REALIZADA-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_PROGRAMADA_TMS-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_DEVOLUCAO_3M-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_DEVOLUCAO_PLANEJAMENTO-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#btnNovoHistorico').click(function () {
    $("#acompanhamentoPedidoDevolucao_wfPedidoComent_DS_COMENT").val('');

    $('#HistoricoModal').modal({
        show: true
    })
});

$('#btnSalvarHistoricoModal').click(function () {
    var URL = URLAPI + "WfPedidoComentAPI/Incluir";
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    var DS_COMENT = $("#acompanhamentoPedidoDevolucao_wfPedidoComent_DS_COMENT").val();

    DS_COMENT = $.trim(DS_COMENT);

    if (DS_COMENT == "" || DS_COMENT.length == 0) {
        return;
    }

    var pedidoComentEntity = {
        pedidoEquip: {
            ID_WF_PEDIDO_EQUIP: ID_WF_PEDIDO_EQUIP,
        },
        DS_COMENT: DS_COMENT,
        usuario: {
            nidUsuario: nidUsuario,
            cnmNome: cnmNome
        }
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoComentEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            //Alerta("Aviso", MensagemGravacaoSucesso);
            CarregarGRIDMensagem();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

});

$("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO").change(function () {

    var ST_STATUS_PEDIDO = parseInt($("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val());

    OcultarCampo($('#validaSTSTATUSPEDIDO'));

    OcultarCampo($('#Transportadora'));
    OcultarCampo($('#validaCDEmpresa'));

    OcultarCampo($('#RetiradaAgendada'));
    OcultarCampo($('#validaDTRETIRADAAGENDADA'));

    OcultarCampo($('#RetiradaRealizada'));
    OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    OcultarCampo($('#ProgramadaTMS'));
    OcultarCampo($('#validaDTPROGRAMADATMS'));

    OcultarCampo($('#Devolucao3M'));
    OcultarCampo($('#validaDTDEVOLUCAO3M'));

    OcultarCampo($('#DevolucaoPlanejamento'));
    OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

    if ($("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val() == "")
        return;

    if (ST_STATUS_PEDIDO == Solicitado) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        //$('#acompanhamentoPedidoDevolucao_empresa_CD_Empresa').val('');
    }
    else if (ST_STATUS_PEDIDO == RetiradaAgendada) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA').val('');
    }
    else if (ST_STATUS_PEDIDO == Coletado) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA').val('');
    }
    else if (ST_STATUS_PEDIDO == AguardandoProgTMS) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));

        $('#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS').val('');
    }
    else if (ST_STATUS_PEDIDO == DevolucaoConcluida) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));

        ExibirCampo($('#Devolucao3M'));
        OcultarCampo($('#validaDTDEVOLUCAO3M'));

        $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M').val('');
    }
    else if (ST_STATUS_PEDIDO == DevolvidoPlanejam) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));

        ExibirCampo($('#Devolucao3M'));
        OcultarCampo($('#validaDTDEVOLUCAO3M'));

        ExibirCampo($('#DevolucaoPlanejamento'));
        OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

        $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO').val('');
    }
    else if (ST_STATUS_PEDIDO == PendenciaCliente) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        //ExibirCampo($('#RetiradaAgendada'));
        //OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        //ExibirCampo($('#RetiradaRealizada'));
        //OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        //ExibirCampo($('#ProgramadaTMS'));
        //OcultarCampo($('#validaDTPROGRAMADATMS'));

        //ExibirCampo($('#Devolucao3M'));
        //OcultarCampo($('#validaDTDEVOLUCAO3M'));

        //ExibirCampo($('#DevolucaoPlanejamento'));
        //OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

        $("#acompanhamentoPedidoDevolucao_wfPedidoComent_DS_COMENT").val('');

        $('#HistoricoModal').modal({
            show: true
        });
    }
    else if (ST_STATUS_PEDIDO == Cancelado) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));

        ExibirCampo($('#Devolucao3M'));
        OcultarCampo($('#validaDTDEVOLUCAO3M'));

        ExibirCampo($('#DevolucaoPlanejamento'));
        OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
    }
    else {
        $('#acompanhamentoPedidoDevolucao_empresa_CD_Empresa').val('');
        $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA').val('');
        $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA').val('');
        $('#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS').val('');
        $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M').val('');
        $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO').val('');
    }
});

$("#CD_GRUPO_RESPONS").change(function () {
    var CD_GRUPOWF = $("#CD_GRUPO_RESPONS").val();

    OcultarCampo($('#validaIDGRUPORESPONS'));
    OcultarCampo($('#validaIDUSUARIORESPONS'));

    if (CD_GRUPOWF == "") {
        return;
    }

    var URL = URLAPI + "WfGrupoUsuAPI/ObterLista";

    var wfGrupoUsuEntity = {
        grupoWf: {
            CD_GRUPOWF: CD_GRUPOWF
        }
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(wfGrupoUsuEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.grupos != null) {
                LoadGrupoResponsaveis(res.grupos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

    AlterarGrupoResponsavel();
});

$("#ID_USUARIO_RESPONS").change(function () {
    OcultarCampo($('#validaIDUSUARIORESPONS'));

    AlterarGrupoResponsavel();
});

$("#acompanhamentoPedidoDevolucao_empresa_CD_Empresa").change(function () {
    OcultarCampo($('#validaCDEmpresa'));
});

$("#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA").blur(function () {
    OcultarCampo($('#validaDTRETIRADAAGENDADA'));
});

$("#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA").blur(function () {
    OcultarCampo($('#validaDTRETIRADAREALIZADA'));
});

$("#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS").blur(function () {
    OcultarCampo($('#validaDTPROGRAMADATMS'));
});

$("#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M").blur(function () {
    OcultarCampo($('#validaDTDEVOLUCAO3M'));
});

$("#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO").blur(function () {
    OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
});

$("#btnAcao").click(function () {

    //var URL = URLAPI + "WorkflowAPI/AlterarStatus";
    var ST_STATUS_PEDIDO = $("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val();
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    var CD_Empresa = $('#acompanhamentoPedidoDevolucao_empresa_CD_Empresa').val();
    var DT_RETIRADA_AGENDADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA').val();
    var DT_RETIRADA_REALIZADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA').val();
    var DT_PROGRAMADA_TMS = $('#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS').val();
    var DT_DEVOLUCAO_3M = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M').val();
    var DT_DEVOLUCAO_PLANEJAMENTO = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO').val();
    var ID_USUARIO_RESPONS = $('#ID_USUARIO_RESPONS').val();
    var CD_GRUPO_RESPONS = $('#CD_GRUPO_RESPONS').val().trim();

    OcultarCampo($('#validaIDGRUPORESPONS'));
    OcultarCampo($('#validaIDUSUARIORESPONS'));
    OcultarCampo($('#validaSTSTATUSPEDIDO'));

    OcultarCampo($('#validaCDEmpresa'));
    OcultarCampo($('#validaDTRETIRADAAGENDADA'));
    OcultarCampo($('#validaDTRETIRADAREALIZADA'));
    OcultarCampo($('#validaDTPROGRAMADATMS'));
    OcultarCampo($('#validaDTDEVOLUCAO3M'));
    OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

    if (ST_STATUS_PEDIDO == "" || ST_STATUS_PEDIDO == "0" || ST_STATUS_PEDIDO == 0) {
        ExibirCampo($('#validaSTSTATUSPEDIDO'));
        return;
    }
    else if (ID_USUARIO_RESPONS == "" || ID_USUARIO_RESPONS == "0" || ID_USUARIO_RESPONS == 0) {
        ExibirCampo($('#validaIDUSUARIORESPONS'));
        return;
    }
    else if (CD_GRUPO_RESPONS == "") {
        ExibirCampo($('#validaIDGRUPORESPONS'));
        return;
    }

    // Valida os campos obrigatórios conforme o status
    if (ST_STATUS_PEDIDO == Solicitado) {
        if (CD_Empresa == "" || CD_Empresa == "0" || CD_Empresa == 0) {
            ExibirCampo($('#validaCDEmpresa'));
            return;
        }
    }
    else if (ST_STATUS_PEDIDO == RetiradaAgendada) {
        if (DT_RETIRADA_AGENDADA == "") {
            ExibirCampo($('#validaDTRETIRADAAGENDADA'));
            return;
        }
    }
    else if (ST_STATUS_PEDIDO == Coletado) {
        if (DT_RETIRADA_REALIZADA == "") {
            ExibirCampo($('#validaDTRETIRADAREALIZADA'));
            return;
        }
    }
    else if (ST_STATUS_PEDIDO == AguardandoProgTMS) {
        if (DT_PROGRAMADA_TMS == "") {
            ExibirCampo($('#validaDTPROGRAMADATMS'));
            return;
        }
    }
    else if (ST_STATUS_PEDIDO == DevolucaoConcluida) {
        if (DT_DEVOLUCAO_3M == "") {
            ExibirCampo($('#validaDTDEVOLUCAO3M'));
            return;
        }
    }
    else if (ST_STATUS_PEDIDO == DevolvidoPlanejam) {
        if (DT_DEVOLUCAO_PLANEJAMENTO == "") {
            ExibirCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
            return;
        }
    }
    //else if (ST_STATUS_PEDIDO == PendenciaCliente) {

    //}
    //else if (ST_STATUS_PEDIDO == Cancelado) {

    //}

    //ConfirmarSimNao('Aviso', 'Confirma ação no pedido?', 'btnAcaoConfirmada()');

    btnAcaoConfirmada();
});

function btnAcaoConfirmada() {
    var URL = URLAPI + "WorkflowAPI/AlterarStatus";
    var ST_STATUS_PEDIDO = $("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val();
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    var CD_Empresa = $('#acompanhamentoPedidoDevolucao_empresa_CD_Empresa').val();
    var DT_RETIRADA_AGENDADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA').val();
    var DT_RETIRADA_REALIZADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA').val();
    var DT_PROGRAMADA_TMS = $('#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS').val();
    var DT_DEVOLUCAO_3M = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M').val();
    var DT_DEVOLUCAO_PLANEJAMENTO = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO').val();
    var ID_USUARIO_RESPONS = $('#ID_USUARIO_RESPONS').val();
    var CD_GRUPO_RESPONS = $('#CD_GRUPO_RESPONS').val().trim();

    var WfPedidoEquipEntity = {
        ID_WF_PEDIDO_EQUIP: ID_WF_PEDIDO_EQUIP,
        ST_STATUS_PEDIDO: ST_STATUS_PEDIDO,
        CD_Empresa: CD_Empresa,
        DT_RETIRADA_AGENDADA_Formatada: DT_RETIRADA_AGENDADA,
        DT_RETIRADA_REALIZADA_Formatada: DT_RETIRADA_REALIZADA,
        DT_PROGRAMADA_TMS_Formatada: DT_PROGRAMADA_TMS,
        DT_DEVOLUCAO_3M_Formatada: DT_DEVOLUCAO_3M,
        DT_DEVOLUCAO_PLANEJAMENTO_Formatada: DT_DEVOLUCAO_PLANEJAMENTO,
        ID_USU_ULT_ATU: nidUsuario,
        ID_USUARIO_RESPONS: ID_USUARIO_RESPONS,
        CD_GRUPO_RESPONS: CD_GRUPO_RESPONS,
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(WfPedidoEquipEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            $("#ST_STATUS_PEDIDO").val(ST_STATUS_PEDIDO);
            ////Alerta("Aviso", MensagemGravacaoSucesso);
            ////DefinirExibicaoAtual();
            //CarregarSituacaoStatus();
            //CarregarGRIDMensagem();
            //DefinirFluxoStatusPedido();
            //AlertaRedirect("Aviso", MensagemGravacaoSucesso, "window.location = '../Workflow';");
            AlertaRedirect("Aviso", res.Mensagem, "window.location = '../Workflow';");
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

function AlterarGrupoResponsavel() {
    var URL = URLAPI + "WorkflowAPI/AlterarGrupoResponsavel";
    //var ST_STATUS_PEDIDO = $("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val();
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    //var CD_Empresa = $('#acompanhamentoPedidoDevolucao_empresa_CD_Empresa').val();
    //var DT_RETIRADA_AGENDADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_AGENDADA').val();
    //var DT_RETIRADA_REALIZADA = $('#acompanhamentoPedidoDevolucao_DT_RETIRADA_REALIZADA').val();
    //var DT_PROGRAMADA_TMS = $('#acompanhamentoPedidoDevolucao_DT_PROGRAMADA_TMS').val();
    //var DT_DEVOLUCAO_3M = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_3M').val();
    //var DT_DEVOLUCAO_PLANEJAMENTO = $('#acompanhamentoPedidoDevolucao_DT_DEVOLUCAO_PLANEJAMENTO').val();
    var ID_USUARIO_RESPONS = $('#ID_USUARIO_RESPONS').val();
    var CD_GRUPO_RESPONS = $('#CD_GRUPO_RESPONS').val().trim();

    var WfPedidoEquipEntity = {
        ID_WF_PEDIDO_EQUIP: ID_WF_PEDIDO_EQUIP,
        //ST_STATUS_PEDIDO: ST_STATUS_PEDIDO,
        //CD_Empresa: CD_Empresa,
        //DT_RETIRADA_AGENDADA_Formatada: DT_RETIRADA_AGENDADA,
        //DT_RETIRADA_REALIZADA_Formatada: DT_RETIRADA_REALIZADA,
        //DT_PROGRAMADA_TMS_Formatada: DT_PROGRAMADA_TMS,
        //DT_DEVOLUCAO_3M_Formatada: DT_DEVOLUCAO_3M,
        //DT_DEVOLUCAO_PLANEJAMENTO_Formatada: DT_DEVOLUCAO_PLANEJAMENTO,
        ID_USU_ULT_ATU: nidUsuario,
        ID_USUARIO_RESPONS: ID_USUARIO_RESPONS,
        CD_GRUPO_RESPONS: CD_GRUPO_RESPONS,
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(WfPedidoEquipEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            //$("#ST_STATUS_PEDIDO").val(ST_STATUS_PEDIDO);
            ////Alerta("Aviso", MensagemGravacaoSucesso);
            ////DefinirExibicaoAtual();
            //CarregarSituacaoStatus();
            //CarregarGRIDMensagem();
            //DefinirFluxoStatusPedido();
            //AlertaRedirect("Aviso", MensagemGravacaoSucesso, "window.location = '../Workflow';");
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

function LoadGrupoResponsaveis(gruposJO) {
    var posicionado = false;
    var grupos = JSON.parse(gruposJO);

    LimparCombo($("#ID_USUARIO_RESPONS"));

    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#ID_USUARIO_RESPONS"), grupos[i].usuario.nidUsuario, grupos[i].usuario.cnmNome);

        if (grupos.length == 1) {
            $('#ID_USUARIO_RESPONS').val(grupos[i].usuario.nidUsuario).trigger('change');
            posicionado = true;
        }
    }

    var hidID_USUARIO_RESPONS = $("#hidID_USUARIO_RESPONS").val();

    if (posicionado == false && hidID_USUARIO_RESPONS != 0) {
        $('#ID_USUARIO_RESPONS').val(hidID_USUARIO_RESPONS).trigger('change');
    }
}

function DefinirExibicaoAtual() {
    var ST_STATUS_PEDIDO = parseInt($("#ST_STATUS_PEDIDO").val());

    if (ST_STATUS_PEDIDO == Solicitado) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));
    }
    else if (ST_STATUS_PEDIDO == RetiradaAgendada) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));
    }
    else if (ST_STATUS_PEDIDO == Coletado) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));
    }
    else if (ST_STATUS_PEDIDO == AguardandoProgTMS) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));
    }
    else if (ST_STATUS_PEDIDO == DevolucaoConcluida) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));

        ExibirCampo($('#Devolucao3M'));
        OcultarCampo($('#validaDTDEVOLUCAO3M'));
    }
    else if (ST_STATUS_PEDIDO == DevolvidoPlanejam) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));

        ExibirCampo($('#Devolucao3M'));
        OcultarCampo($('#validaDTDEVOLUCAO3M'));

        ExibirCampo($('#DevolucaoPlanejamento'));
        OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
    }
    else if (ST_STATUS_PEDIDO == PendenciaCliente) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));
    }
    else if (ST_STATUS_PEDIDO == Cancelado) {
        ExibirCampo($('#Transportadora'));
        OcultarCampo($('#validaCDEmpresa'));

        ExibirCampo($('#RetiradaAgendada'));
        OcultarCampo($('#validaDTRETIRADAAGENDADA'));

        ExibirCampo($('#RetiradaRealizada'));
        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

        ExibirCampo($('#ProgramadaTMS'));
        OcultarCampo($('#validaDTPROGRAMADATMS'));

        ExibirCampo($('#Devolucao3M'));
        OcultarCampo($('#validaDTDEVOLUCAO3M'));

        ExibirCampo($('#DevolucaoPlanejamento'));
        OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
    }

}

function CarregarGRIDMensagem() {
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();

    if (ID_WF_PEDIDO_EQUIP == "" || ID_WF_PEDIDO_EQUIP == "0" || ID_WF_PEDIDO_EQUIP == 0) {
        return;
    }

    var URL = URLObterListaMensagem + "?ID_WF_PEDIDO_EQUIP=" + ID_WF_PEDIDO_EQUIP;

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
                $('#gridmvcMensagem').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });

}

function DefinirFluxoStatusPedido() {
    var ST_STATUS_PEDIDO = parseInt($("#ST_STATUS_PEDIDO").val());
    var statusCarregar = '';

    LimparCombo($("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO"));

    if (ST_STATUS_PEDIDO == PendenteAnexar) {
        statusCarregar = AnaliseLogistica;
    } else if (ST_STATUS_PEDIDO == AnaliseLogistica) {
        statusCarregar = PendenteAnexar + ',' + Solicitado + ',' + Cancelado;
    } else if (ST_STATUS_PEDIDO == Solicitado) {
        statusCarregar = PendenciaCliente + ',' + RetiradaAgendada + ',' + Cancelado;
    } else if (ST_STATUS_PEDIDO == PendenciaCliente) {
        statusCarregar = Solicitado + ',' + RetiradaAgendada;
    } else if (ST_STATUS_PEDIDO == RetiradaAgendada) {
        statusCarregar = PendenciaCliente + ',' + Solicitado + ',' + Cancelado + ',' + Coletado;
    } else if (ST_STATUS_PEDIDO == Coletado) {
        statusCarregar = Cancelado + ',' + AguardandoProgTMS;
    } else if (ST_STATUS_PEDIDO == AguardandoProgTMS) {
        statusCarregar = DevolucaoConcluida;
    } else if (ST_STATUS_PEDIDO == DevolucaoConcluida) {
        statusCarregar = DevolvidoPlanejam;
    } else {
        return;
    }

    var URL = URLAPI + "WfStatusPedidoEquipAPI/ObterListaStatus?statusCarregar=" + statusCarregar + "&TP_PEDIDO=D";
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
            if (res.tiposStatusPedido != null) {
                LoadStatusPedido(res.tiposStatusPedido);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadStatusPedido(tiposStatusPedido) {
    for (i = 0; i < tiposStatusPedido.length; i++) {
        MontarCombo($("#acompanhamentoPedidoDevolucao_wfStatusPedidoEquip_ST_STATUS_PEDIDO"), tiposStatusPedido[i].ST_STATUS_PEDIDO, tiposStatusPedido[i].DS_TRANSICAO);
    }
}

function CarregarGrupoResponsavel() {
    var ST_STATUS_PEDIDO = $("#ST_STATUS_PEDIDO").val();

    LimparCombo($("#CD_GRUPO_RESPONS"));

    var URL = URLAPI + "WfGrupoAPI/ObterListaByStatusPedido?ST_STATUS_PEDIDO=" + ST_STATUS_PEDIDO;
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
            if (res.grupos != null) {
                LoadGrupos(res.grupos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadGrupos(gruposJO) {
    var posicionado = false;
    var grupos = JSON.parse(gruposJO);

    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#CD_GRUPO_RESPONS"), grupos[i].CD_GRUPOWF, grupos[i].DS_GRUPOWF);

        if (grupos.length == 1) {
            $('#CD_GRUPO_RESPONS').val(grupos[i].CD_GRUPOWF).trigger('change');
            posicionado = true;
        }
    }

    var hidCD_GRUPO_RESPONS = $("#hidCD_GRUPO_RESPONS").val();

    if (posicionado == false && hidCD_GRUPO_RESPONS != "") {
        $('#CD_GRUPO_RESPONS').val(hidCD_GRUPO_RESPONS).trigger('change');
    }
}
