$().ready(function () {
    LimparScrollWorkflow();
    //DefinirExibicaoAtual();
    CarregarSituacaoStatus();
    DefinirFluxoStatusPedido();
    CarregarGRIDMensagem();
    CarregarGrupoResponsavel();

    //$('#acompanhamentoPedidoEnvio_DT_RETIRADA_AGENDADA').mask('00/00/0000');
    //$('#acompanhamentoPedidoEnvio_DT_RETIRADA_REALIZADA').mask('00/00/0000');
    //$('#acompanhamentoPedidoEnvio_DT_PROGRAMADA_TMS').mask('00/00/0000');
    //$('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_3M').mask('00/00/0000');
    //$('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_PLANEJAMENTO').mask('00/00/0000');

    OcultarCampo($('#btnSalvarRascunho'));
    $("#btnSalvarSubmeter").prop('value', 'Salvar');

    var ST_STATUS_PEDIDO = parseInt($('#ST_STATUS_PEDIDO').val());
    if (ST_STATUS_PEDIDO == Instalado || ST_STATUS_PEDIDO == Cancelado) {
        OcultarCampo($('#btnSalvarSubmeter'));
        OcultarCampo($('#btnAcao'));
        OcultarCampo($('#btnNovoHistorico'));
    }

    CarregarAnexos(idWFEnvio);
});

//$('#DT_RETIRADA_AGENDADA-container .input-group.date').datepicker({
//    language: "pt-BR",
//    autoclose: true,
//    todayHighlight: true
//});

//$('#DT_RETIRADA_REALIZADA-container .input-group.date').datepicker({
//    language: "pt-BR",
//    autoclose: true,
//    todayHighlight: true
//});

//$('#DT_PROGRAMADA_TMS-container .input-group.date').datepicker({
//    language: "pt-BR",
//    autoclose: true,
//    todayHighlight: true
//});

//$('#DT_DEVOLUCAO_3M-container .input-group.date').datepicker({
//    language: "pt-BR",
//    autoclose: true,
//    todayHighlight: true
//});

//$('#DT_DEVOLUCAO_PLANEJAMENTO-container .input-group.date').datepicker({
//    language: "pt-BR",
//    autoclose: true,
//    todayHighlight: true
//});

$('#btnNovoHistorico').click(function () {
    $("#acompanhamentoPedidoEnvio_wfPedidoComent_DS_COMENT").val('');

    $('#HistoricoModal').modal({
        show: true
    });
});

$('#btnSalvarHistoricoModal').click(function () {
    var URL = URLAPI + "WfPedidoComentAPI/Incluir";
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    var DS_COMENT = $("#acompanhamentoPedidoEnvio_wfPedidoComent_DS_COMENT").val();

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
    //var token = sessionStorage.getItem("token");
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

$("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO").change(function () {

    //var ST_STATUS_PEDIDO = parseInt($("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val());

    OcultarCampo($('#validaSTSTATUSPEDIDO'));

    //OcultarCampo($('#Transportadora'));
    //OcultarCampo($('#validaCDEmpresa'));

    //OcultarCampo($('#AnalisePlanejamento'));
    //OcultarCampo($('#validaDTAnalisePlanejamento'));

    //OcultarCampo($('#RetiradaRealizada'));
    //OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    //OcultarCampo($('#ProgramadaTMS'));
    //OcultarCampo($('#validaDTPROGRAMADATMS'));

    //OcultarCampo($('#Devolucao3M'));
    //OcultarCampo($('#validaDTDEVOLUCAO3M'));

    //OcultarCampo($('#DevolucaoPlanejamento'));
    //OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

    if ($("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val() == "")
        return;

    //if (ST_STATUS_PEDIDO == AnaliseAreaTecnica) {
    //    ExibirCampo($('#Transportadora'));
    //    OcultarCampo($('#validaCDEmpresa'));

    //    //$('#acompanhamentoPedidoEnvio_empresa_CD_Empresa').val('');
    //}
    //else if (ST_STATUS_PEDIDO == AnalisePlanejamento) {
    //    ExibirCampo($('#Transportadora'));
    //    OcultarCampo($('#validaCDEmpresa'));

    //    ExibirCampo($('#AnalisePlanejamento'));
    //    OcultarCampo($('#validaDTAnalisePlanejamento'));

    //    $('#acompanhamentoPedidoEnvio_DT_RETIRADA_AGENDADA').val('');
    //}
    //else if (ST_STATUS_PEDIDO == EnviadoCliente) {
    //    ExibirCampo($('#Transportadora'));
    //    OcultarCampo($('#validaCDEmpresa'));

    //    ExibirCampo($('#AnalisePlanejamento'));
    //    OcultarCampo($('#validaDTAnalisePlanejamento'));

    //    ExibirCampo($('#RetiradaRealizada'));
    //    OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    //    $('#acompanhamentoPedidoEnvio_DT_RETIRADA_REALIZADA').val('');
    //}
    //else if (ST_STATUS_PEDIDO == EntregueCliente) {
    //    ExibirCampo($('#Transportadora'));
    //    OcultarCampo($('#validaCDEmpresa'));

    //    ExibirCampo($('#AnalisePlanejamento'));
    //    OcultarCampo($('#validaDTAnalisePlanejamento'));

    //    ExibirCampo($('#RetiradaRealizada'));
    //    OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    //    ExibirCampo($('#ProgramadaTMS'));
    //    OcultarCampo($('#validaDTPROGRAMADATMS'));

    //    $('#acompanhamentoPedidoEnvio_DT_PROGRAMADA_TMS').val('');
    //}
    //else if (ST_STATUS_PEDIDO == Instalado) {
    //    ExibirCampo($('#Transportadora'));
    //    OcultarCampo($('#validaCDEmpresa'));

    //    ExibirCampo($('#AnalisePlanejamento'));
    //    OcultarCampo($('#validaDTAnalisePlanejamento'));

    //    ExibirCampo($('#RetiradaRealizada'));
    //    OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    //    ExibirCampo($('#ProgramadaTMS'));
    //    OcultarCampo($('#validaDTPROGRAMADATMS'));

    //    ExibirCampo($('#Devolucao3M'));
    //    OcultarCampo($('#validaDTDEVOLUCAO3M'));

    //    $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_3M').val('');
    //}
    ////else if (ST_STATUS_PEDIDO == DevolvidoPlanejam) {
    ////    ExibirCampo($('#Transportadora'));
    ////    OcultarCampo($('#validaCDEmpresa'));

    ////    ExibirCampo($('#AnalisePlanejamento'));
    ////    OcultarCampo($('#validaDTAnalisePlanejamento'));

    ////    ExibirCampo($('#RetiradaRealizada'));
    ////    OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    ////    ExibirCampo($('#ProgramadaTMS'));
    ////    OcultarCampo($('#validaDTPROGRAMADATMS'));

    ////    ExibirCampo($('#Devolucao3M'));
    ////    OcultarCampo($('#validaDTDEVOLUCAO3M'));

    ////    ExibirCampo($('#DevolucaoPlanejamento'));
    ////    OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

    ////    $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_PLANEJAMENTO').val('');
    ////}
    //else if (ST_STATUS_PEDIDO == EmCompras) {
    //    ExibirCampo($('#Transportadora'));
    //    OcultarCampo($('#validaCDEmpresa'));

    //    //ExibirCampo($('#AnalisePlanejamento'));
    //    //OcultarCampo($('#validaDTAnalisePlanejamento'));

    //    //ExibirCampo($('#RetiradaRealizada'));
    //    //OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    //    //ExibirCampo($('#ProgramadaTMS'));
    //    //OcultarCampo($('#validaDTPROGRAMADATMS'));

    //    //ExibirCampo($('#Devolucao3M'));
    //    //OcultarCampo($('#validaDTDEVOLUCAO3M'));

    //    //ExibirCampo($('#DevolucaoPlanejamento'));
    //    //OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

    //    $("#acompanhamentoPedidoEnvio_wfPedidoComent_DS_COMENT").val('');

    //    $('#HistoricoModal').modal({
    //        show: true
    //    })
    //}
    //else if (ST_STATUS_PEDIDO == Cancelado) {
    //    ExibirCampo($('#Transportadora'));
    //    OcultarCampo($('#validaCDEmpresa'));

    //    ExibirCampo($('#AnalisePlanejamento'));
    //    OcultarCampo($('#validaDTAnalisePlanejamento'));

    //    ExibirCampo($('#RetiradaRealizada'));
    //    OcultarCampo($('#validaDTRETIRADAREALIZADA'));

    //    ExibirCampo($('#ProgramadaTMS'));
    //    OcultarCampo($('#validaDTPROGRAMADATMS'));

    //    ExibirCampo($('#Devolucao3M'));
    //    OcultarCampo($('#validaDTDEVOLUCAO3M'));

    //    ExibirCampo($('#DevolucaoPlanejamento'));
    //    OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
    //}
    //else {
    //    $('#acompanhamentoPedidoEnvio_empresa_CD_Empresa').val('');
    //    $('#acompanhamentoPedidoEnvio_DT_RETIRADA_AGENDADA').val('');
    //    $('#acompanhamentoPedidoEnvio_DT_RETIRADA_REALIZADA').val('');
    //    $('#acompanhamentoPedidoEnvio_DT_PROGRAMADA_TMS').val('');
    //    $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_3M').val('');
    //    $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_PLANEJAMENTO').val('');
    //}
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

$("#btnAcao").click(function () {

    //var URL = URLAPI + "WorkflowAPI/AlterarStatus";
    var ST_STATUS_PEDIDO = $("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val();
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    //var CD_Empresa = $('#acompanhamentoPedidoEnvio_empresa_CD_Empresa').val();
    //var DT_RETIRADA_AGENDADA = $('#acompanhamentoPedidoEnvio_DT_RETIRADA_AGENDADA').val();
    //var DT_RETIRADA_REALIZADA = $('#acompanhamentoPedidoEnvio_DT_RETIRADA_REALIZADA').val();
    //var DT_PROGRAMADA_TMS = $('#acompanhamentoPedidoEnvio_DT_PROGRAMADA_TMS').val();
    //var DT_DEVOLUCAO_3M = $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_3M').val();
    //var DT_DEVOLUCAO_PLANEJAMENTO = $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_PLANEJAMENTO').val();
    var ID_USUARIO_RESPONS = $('#ID_USUARIO_RESPONS').val();
    var CD_GRUPO_RESPONS = $('#CD_GRUPO_RESPONS').val().trim();

    OcultarCampo($('#validaIDGRUPORESPONS'));
    OcultarCampo($('#validaIDUSUARIORESPONS'));
    OcultarCampo($('#validaSTSTATUSPEDIDO'));

    //OcultarCampo($('#validaCDEmpresa'));
    //OcultarCampo($('#validaDTAnalisePlanejamento'));
    //OcultarCampo($('#validaDTRETIRADAREALIZADA'));
    //OcultarCampo($('#validaDTPROGRAMADATMS'));
    //OcultarCampo($('#validaDTDEVOLUCAO3M'));
    //OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));

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

    //// Valida os campos obrigatórios conforme o status
    //if (ST_STATUS_PEDIDO == AnaliseAreaTecnica) {
    //    if (CD_Empresa == "" || CD_Empresa == "0" || CD_Empresa == 0) {
    //        ExibirCampo($('#validaCDEmpresa'));
    //        return;
    //    }
    //}
    //else if (ST_STATUS_PEDIDO == AnalisePlanejamento) {
    //    if (DT_RETIRADA_AGENDADA == "") {
    //        ExibirCampo($('#validaDTAnalisePlanejamento'));
    //        return;
    //    }
    //}
    //else if (ST_STATUS_PEDIDO == EnviadoCliente) {
    //    if (DT_RETIRADA_REALIZADA == "") {
    //        ExibirCampo($('#validaDTRETIRADAREALIZADA'));
    //        return;
    //    }
    //}
    //else if (ST_STATUS_PEDIDO == EntregueCliente) {
    //    if (DT_PROGRAMADA_TMS == "") {
    //        ExibirCampo($('#validaDTPROGRAMADATMS'));
    //        return;
    //    }
    //}
    //else if (ST_STATUS_PEDIDO == Instalado) {
    //    if (DT_DEVOLUCAO_3M == "") {
    //        ExibirCampo($('#validaDTDEVOLUCAO3M'));
    //        return;
    //    }
    //}
    ////else if (ST_STATUS_PEDIDO == DevolvidoPlanejam) {
    ////    if (DT_DEVOLUCAO_PLANEJAMENTO == "") {
    ////        ExibirCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
    ////        return;
    ////    }
    ////}
    ////else if (ST_STATUS_PEDIDO == EmCompras) {

    ////}
    ////else if (ST_STATUS_PEDIDO == Cancelado) {

    ////}

    //ConfirmarSimNao('Aviso', 'Confirma ação no pedido?', 'btnAcaoConfirmada()');

    btnAcaoConfirmada();
});

function btnAcaoConfirmada() {
    var URL = URLAPI + "WorkflowAPI/AlterarStatus";
    var ST_STATUS_PEDIDO = $("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val();
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    //var CD_Empresa = $('#acompanhamentoPedidoEnvio_empresa_CD_Empresa').val();
    //var DT_RETIRADA_AGENDADA = $('#acompanhamentoPedidoEnvio_DT_RETIRADA_AGENDADA').val();
    //var DT_RETIRADA_REALIZADA = $('#acompanhamentoPedidoEnvio_DT_RETIRADA_REALIZADA').val();
    //var DT_PROGRAMADA_TMS = $('#acompanhamentoPedidoEnvio_DT_PROGRAMADA_TMS').val();
    //var DT_DEVOLUCAO_3M = $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_3M').val();
    //var DT_DEVOLUCAO_PLANEJAMENTO = $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_PLANEJAMENTO').val();
    var ID_USUARIO_RESPONS = $('#ID_USUARIO_RESPONS').val();
    var CD_GRUPO_RESPONS = $('#CD_GRUPO_RESPONS').val().trim();
    var ID_CATEGORIA = $('#Categoria').val();
    //var token = sessionStorage.getItem("token");
    var WfPedidoEquipEntity = {
        ID_WF_PEDIDO_EQUIP: ID_WF_PEDIDO_EQUIP,
        ST_STATUS_PEDIDO: ST_STATUS_PEDIDO,
        //CD_Empresa: CD_Empresa,
        //DT_RETIRADA_AGENDADA: DT_RETIRADA_AGENDADA,
        //DT_RETIRADA_REALIZADA: DT_RETIRADA_REALIZADA,
        //DT_PROGRAMADA_TMS: DT_PROGRAMADA_TMS,
        //DT_DEVOLUCAO_3M: DT_DEVOLUCAO_3M,
        //DT_DEVOLUCAO_PLANEJAMENTO: DT_DEVOLUCAO_PLANEJAMENTO,
        ID_USU_ULT_ATU: nidUsuario,
        ID_USUARIO_RESPONS: ID_USUARIO_RESPONS,
        CD_GRUPO_RESPONS: CD_GRUPO_RESPONS,
        ID_CATEGORIA: ID_CATEGORIA,
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(WfPedidoEquipEntity),
        cache: false,
        async: true,
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
            //AlertaRedirect("Aviso", res.MensagemGravacaoSucesso, "window.location = '../Workflow';");

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
    //var ST_STATUS_PEDIDO = $("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO").val();
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    //var CD_Empresa = $('#acompanhamentoPedidoEnvio_empresa_CD_Empresa').val();
    //var DT_RETIRADA_AGENDADA = $('#acompanhamentoPedidoEnvio_DT_RETIRADA_AGENDADA').val();
    //var DT_RETIRADA_REALIZADA = $('#acompanhamentoPedidoEnvio_DT_RETIRADA_REALIZADA').val();
    //var DT_PROGRAMADA_TMS = $('#acompanhamentoPedidoEnvio_DT_PROGRAMADA_TMS').val();
    //var DT_DEVOLUCAO_3M = $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_3M').val();
    //var DT_DEVOLUCAO_PLANEJAMENTO = $('#acompanhamentoPedidoEnvio_DT_DEVOLUCAO_PLANEJAMENTO').val();
    var ID_USUARIO_RESPONS = $('#ID_USUARIO_RESPONS').val();
    var CD_GRUPO_RESPONS = $('#CD_GRUPO_RESPONS').val().trim();

    var WfPedidoEquipEntity = {
        ID_WF_PEDIDO_EQUIP: ID_WF_PEDIDO_EQUIP,
        //ST_STATUS_PEDIDO: ST_STATUS_PEDIDO,
        //CD_Empresa: CD_Empresa,
        //DT_RETIRADA_AGENDADA: DT_RETIRADA_AGENDADA,
        //DT_RETIRADA_REALIZADA: DT_RETIRADA_REALIZADA,
        //DT_PROGRAMADA_TMS: DT_PROGRAMADA_TMS,
        //DT_DEVOLUCAO_3M: DT_DEVOLUCAO_3M,
        //DT_DEVOLUCAO_PLANEJAMENTO: DT_DEVOLUCAO_PLANEJAMENTO,
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
        async: true,
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
        //if ((grupos[i].NM_PRIORIDADE == 1 && grupos[i].grupoWf.ID_GRUPOWF == 8) || grupos[i].grupoWf.ID_GRUPOWF != 8)
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

//function DefinirExibicaoAtual() {
//    var ST_STATUS_PEDIDO = parseInt($("#ST_STATUS_PEDIDO").val());

//    if (ST_STATUS_PEDIDO == AnaliseAreaTecnica) {
//        ExibirCampo($('#Transportadora'));
//        OcultarCampo($('#validaCDEmpresa'));
//    }
//    else if (ST_STATUS_PEDIDO == AnalisePlanejamento) {
//        ExibirCampo($('#Transportadora'));
//        OcultarCampo($('#validaCDEmpresa'));

//        ExibirCampo($('#AnalisePlanejamento'));
//        OcultarCampo($('#validaDTAnalisePlanejamento'));
//    }
//    else if (ST_STATUS_PEDIDO == EnviadoCliente) {
//        ExibirCampo($('#Transportadora'));
//        OcultarCampo($('#validaCDEmpresa'));

//        ExibirCampo($('#AnalisePlanejamento'));
//        OcultarCampo($('#validaDTAnalisePlanejamento'));

//        ExibirCampo($('#RetiradaRealizada'));
//        OcultarCampo($('#validaDTRETIRADAREALIZADA'));
//    }
//    else if (ST_STATUS_PEDIDO == EntregueCliente) {
//        ExibirCampo($('#Transportadora'));
//        OcultarCampo($('#validaCDEmpresa'));

//        ExibirCampo($('#AnalisePlanejamento'));
//        OcultarCampo($('#validaDTAnalisePlanejamento'));

//        ExibirCampo($('#RetiradaRealizada'));
//        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

//        ExibirCampo($('#ProgramadaTMS'));
//        OcultarCampo($('#validaDTPROGRAMADATMS'));
//    }
//    else if (ST_STATUS_PEDIDO == Instalado) {
//        ExibirCampo($('#Transportadora'));
//        OcultarCampo($('#validaCDEmpresa'));

//        ExibirCampo($('#AnalisePlanejamento'));
//        OcultarCampo($('#validaDTAnalisePlanejamento'));

//        ExibirCampo($('#RetiradaRealizada'));
//        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

//        ExibirCampo($('#ProgramadaTMS'));
//        OcultarCampo($('#validaDTPROGRAMADATMS'));

//        ExibirCampo($('#Devolucao3M'));
//        OcultarCampo($('#validaDTDEVOLUCAO3M'));
//    }
//    //else if (ST_STATUS_PEDIDO == DevolvidoPlanejam) {
//    //    ExibirCampo($('#Transportadora'));
//    //    OcultarCampo($('#validaCDEmpresa'));

//    //    ExibirCampo($('#AnalisePlanejamento'));
//    //    OcultarCampo($('#validaDTAnalisePlanejamento'));

//    //    ExibirCampo($('#RetiradaRealizada'));
//    //    OcultarCampo($('#validaDTRETIRADAREALIZADA'));

//    //    ExibirCampo($('#ProgramadaTMS'));
//    //    OcultarCampo($('#validaDTPROGRAMADATMS'));

//    //    ExibirCampo($('#Devolucao3M'));
//    //    OcultarCampo($('#validaDTDEVOLUCAO3M'));

//    //    ExibirCampo($('#DevolucaoPlanejamento'));
//    //    OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
//    //}
//    else if (ST_STATUS_PEDIDO == EmCompras) {
//        ExibirCampo($('#Transportadora'));
//        OcultarCampo($('#validaCDEmpresa'));
//    }
//    else if (ST_STATUS_PEDIDO == Cancelado) {
//        ExibirCampo($('#Transportadora'));
//        OcultarCampo($('#validaCDEmpresa'));

//        ExibirCampo($('#AnalisePlanejamento'));
//        OcultarCampo($('#validaDTAnalisePlanejamento'));

//        ExibirCampo($('#RetiradaRealizada'));
//        OcultarCampo($('#validaDTRETIRADAREALIZADA'));

//        ExibirCampo($('#ProgramadaTMS'));
//        OcultarCampo($('#validaDTPROGRAMADATMS'));

//        ExibirCampo($('#Devolucao3M'));
//        OcultarCampo($('#validaDTDEVOLUCAO3M'));

//        ExibirCampo($('#DevolucaoPlanejamento'));
//        OcultarCampo($('#validaDTDEVOLUCAOPLANEJAMENTO'));
//    }

//}

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

    LimparCombo($("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO"));

    if (ST_STATUS_PEDIDO == PendenteAnexar) {
        statusCarregar = AnaliseMarketing;
    } else if (ST_STATUS_PEDIDO == AnaliseMarketing) {
        statusCarregar = PendenteAnexar + ',' + AnaliseAreaTecnica + ',' + Cancelado;
    } else if (ST_STATUS_PEDIDO == AnaliseAreaTecnica) {
        statusCarregar = AnalisePlanejamento + ',' + PendenteAnexar + ',' + AnaliseEspecial + ',' + Cancelado;
    } else if (ST_STATUS_PEDIDO == AnaliseEspecial) {
        statusCarregar = AnaliseAreaTecnica + ',' + AnalisePlanejamento + ',' + Cancelado;
    } else if (ST_STATUS_PEDIDO == AnalisePlanejamento) {
        statusCarregar = EmCompras + ',' + Cancelado + ',' + EnviadoCliente;
    } else if (ST_STATUS_PEDIDO == EmCompras) {
        statusCarregar = AnalisePlanejamento + ',' + EnviadoCliente;
    } else if (ST_STATUS_PEDIDO == EnviadoCliente) {
        statusCarregar = Cancelado + ',' + AnalisePlanejamento + ',' + EntregueCliente + ',' + EmCompras;
    } else if (ST_STATUS_PEDIDO == EntregueCliente) {
        statusCarregar = Instalado + ',' + Cancelado;
        //} else if (ST_STATUS_PEDIDO == Instalado) {
        //    statusCarregar = DevolvidoPlanejam;
    } else {
        return;
    }

    var URL = URLAPI + "WfStatusPedidoEquipAPI/ObterListaStatus?statusCarregar=" + statusCarregar + "&TP_PEDIDO=E";
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
        MontarCombo($("#acompanhamentoPedidoEnvio_wfStatusPedidoEquip_ST_STATUS_PEDIDO"), tiposStatusPedido[i].ST_STATUS_PEDIDO, tiposStatusPedido[i].DS_TRANSICAO);
    }
}

function CarregarGrupoResponsavel() {
    var ST_STATUS_PEDIDO = parseInt($("#ST_STATUS_PEDIDO").val());
    var Categoria = parseInt('0' + $("#Categoria").val());
    var TipoLocacao = $("#TipoLocacao").val();
    var Linha = $("#Linha").val();
    var Modelo = $("#Modelo option:selected").val();

    LimparCombo($("#CD_GRUPO_RESPONS"));
    //var token = sessionStorage.getItem("token");
    var tipoSolicitacao = $("#TipoSolicitacao option:selected").val();
    if (tipoSolicitacao == 5 && ST_STATUS_PEDIDO == 2) {
        LoadGrupos('[{"ID_GRUPOWF":0,"CD_GRUPOWF":"MKT7","DS_GRUPOWF":"MKT7 - Troca","TP_GRUPOWF":"E","nidUsuarioAtualizacao":0,"dtmDataHoraAtualizacao":"0001 - 01 - 01T00: 00: 00","bidAtivo":null}]');
    } else {

        var URL = URLAPI + "WfGrupoAPI/ObterListaByStatusPedidoEnvio?ST_STATUS_PEDIDO=" + ST_STATUS_PEDIDO + "&CATEGORIA=" + Categoria + "&TIPOLOCACAO=" + TipoLocacao + "&LINHA=" + Linha + "&MODELO=" + Modelo;

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

                    // se tipo for Aluguel, grupo responsável pré-selecionado passa a ser MKT3
                    var CD_GRUPO_RESPONS = $("#CD_GRUPO_RESPONS option:selected").val();
                    if (TipoLocacao == "A" && CD_GRUPO_RESPONS == "")
                        $('#CD_GRUPO_RESPONS').val("MKT3      ").trigger('change');
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });

    }
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
