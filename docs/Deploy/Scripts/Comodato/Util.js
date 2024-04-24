//document.onkeydown = function (event) {
//    if (event) {
//        if (event.keyCode == '13') {
//            event.preventDefault();
//            event.stopPropagation();
//        }
//    }
//}

function getParameterByName(name, url) {

    if (!url) url = window.location.href;

    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"), results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';

    return decodeURIComponent(results[2].replace(/\+/g, " "));

}

function SetaFocoBotao(pBotao) {
    //Finalidade: Setar o foco das caixas de texto para o botão determinado

    if (window.event != null && window.event.keyCode == 13) {
        window.event.cancelBubble = true;
        window.event.returnValue = false;
        window.event.preventDefault();
        window.event.stopPropagation();
        document.getElementById(pBotao).click();
    }
}

function SelecionarItem(Obj, Valor) {
    $(Obj).val(Valor);
}

function LimparCombo(obj) {
    $(obj).empty();
    $('<option>', { value: "", text: "Selecione..." }).appendTo(obj);
}

function MontarCombo(obj, valor, texto) {
    $('<option>', { value: valor, text: texto }).appendTo(obj);
}

function DesabilitarCampo(obj) {

    ExibirCampo(obj);

    var LimparCampo = arguments[1] != undefined ? arguments[1] : false;

    obj.find("input,select,textarea").each(function (e, d) {
        $(d).css("color", "#c0c0c0").prop("disabled", true);
        if (LimparCampo == true)
            $(d).prop("value", "").prop("checked", false).prop("selectedIndex", 0);
    });

    obj.find(".lnkData").each(function (e, d) {
        $(d).unbind("click").children("img").attr("src", GetDominio() + "/images/disable-calendar.gif");
    });

}

function DesabilitarCampoLabel(obj) {

    var LimparCampo = arguments[1] != undefined ? arguments[1] : false;

    obj.find("span,label,p").each(function (e, d) {
        $(d).css("color", "#c0c0c0");
    });

    obj.find("input,select,textarea").each(function (e, d) {
        $(d).css("color", "#c0c0c0").prop("disabled", true);
        if (LimparCampo == true)
            $(d).prop("value", "").prop("checked", false).prop("selectedIndex", 0);
    });

    obj.find(".lnkData").each(function (e, d) {
        $(d).unbind("click").children("img").attr("src", GetDominio() + "/images/disable-calendar.gif");
    });

}

//function HabilitarCampo(obj) {

//    ExibirCampo(obj);

//    obj.find("span,label,p").each(function (e, d) {
//        $(d).css("color", "#000000");
//    });

//    obj.find("input,select,textarea").each(function (e, d) {
//        $(d).css("color", "#000000").prop("disabled", false);
//    });

//    obj.find(".lnkData").each(function (e, d) {
//        $(d).bind("click",
//            function () {
//                ExibirCalendario($(this));
//            }).children("img").attr("src", GetDominio() + "/images/show-calendar.gif");
//    });
//}

function FormataCamposValor() {

    $('.valor').mask('#.##0,00', {
        reverse: true
    });

}

function ExibirCalendario(obj) {
    var result = showCalendar(obj.prev().attr('id'), 'dd/mm/y');
}

function OcultarCampo(obj) {

    obj.each(function (e, d) {
        $(d).hide().prop("disabled", false);
    });

}

function ExibirCampo(obj) {

    obj.each(function (e, d) {
        $(d).show().removeAttr("disabled");
    });

}

function AbrirJanela(url, largura, altura) {
    var left = (screen.width / 2) - (largura / 2);
    var top = (screen.height / 2) - (altura / 2);
    var popup = window.open(url, '', 'scrollbars=yes, resizable=yes, width=' + largura + ',height=' + altura + ',top=' + top + ', left=' + left);
    popupBlockerChecker.check(popup);
}

var popupBlockerChecker = {
    check: function (popup_window) {
        var _scope = this;
        if (popup_window) {
            if (/chrome/.test(navigator.userAgent.toLowerCase())) {
                setTimeout(function () {
                    _scope._is_popup_blocked(_scope, popup_window);
                }, 200);
            } else {
                popup_window.onload = function () {
                    _scope._is_popup_blocked(_scope, popup_window);
                };
            }
        } else {
            _scope._displayError();
        }
    },
    _is_popup_blocked: function (scope, popup_window) {
        if ((popup_window.innerHeight > 0) == false) { scope._displayError(); }
    },
    _displayError: function () {
        Alerta("Aviso", "O Bloqueador de Popup está habilitado! Por favor adicione esse site na sua lista de exceção!");
    }
};

function AbrirModal(Titulo, Url, Largura, Altura) {

    $('#iframeModal').remove();
    var strTemplate = '<div id="iframeModal" class="modal fade" role="dialog">' +
        '<div class="modal-dialog" style="width:' + Largura + '; height:' + Altura + '">' +
        '<div class="modal-content" style="width:' + Largura + '; height:' + Altura + '">' +
        '<div class="modal-header bg-dark">' +
        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
        '<h4 class="modal-title text-white">' + Titulo + '</h4>' +
        '</div>' +
        '<div class="modal-body">' +
        '<iframe src="' + Url + '" style="width:100%;height:95%;border:0"></iframe>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>';

    $(strTemplate).modal('toggle');

}

function Alerta(Titulo, Msg) {

    if ((Titulo == "undefined" || Titulo == "Undefined" || Titulo == undefined)
        || (Msg == "undefined" || Msg == "Undefined" || Msg == undefined)) {
        Msg = 'Ocorreu um erro inesperado. Por favor tente novamente ou contate o suporte. <hr><p style="font-size:10px">Detalhes: ' + Msg + '</p>';
    }

    var strTemplate =
        '<div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">' +
        '    <div class="modal-dialog" role="document">' +
        '        <div class="modal-content">' +
        '            <div class="modal-header bg-dark">' +
        '                <h5 class="modal-title text-white" id="exampleModalLabel">' + Titulo + '</h5>' +
        //'                <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
        //'                    <span aria-hidden="true">&times;</span>' +
        //'                </button>' +
        '            </div>' +
        '            <div class="modal-body">' + Msg +
        '            </div>' +
        '            <div class="modal-footer">' +
        '                <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>' +
        '            </div>' +
        '        </div>' +
        '    </div>' +
        '</div>';

    $(strTemplate).modal('toggle');
}

//function Alerta0(Titulo, Msg, Largura, Altura) {

//    var strTemplate = '<div id="modal" class="modal fade" role="dialog">' +
//        '<div class="modal-dialog" style="width:' + Largura + '; height:' + Altura + '">' +
//        '<div class="modal-content" style="width:' + Largura + '; height:' + Altura + '">' +
//        '<div class="modal-header bg-dark">' +
//        '<h4 class="modal-title text-white"  style="text-align:Left;" > ' + Titulo + '</h4>' +
//        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
//        '</div>' +
//        '<div class="modal-body" style="text-align:center; ">' + Msg + '</div>' +
//        '</div>' +
//        '</div>' +
//        '</div>';

//    $(strTemplate).modal('toggle');

//}

//function Alerta1(Titulo, Msg, Altura) {
//    var strTemplate = '<div id="modal" class="modal fade" role="dialog">' +
//        '<div class="modal-dialog" style=" height:' + Altura + '">' +
//        '<div class="modal-content" style=" height:' + Altura + '">' +
//        '<div class="modal-header bg-dark">' +
//        '<h4 class="modal-title text-white" >' + Titulo + '</h4>' +
//        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
//        '</div>' +
//        '<div class="modal-body" style="text-align:center; ">' + Msg + '</div>' +
//        '</div>' +
//        '</div>' +
//        '</div>';

//    $(strTemplate).modal('toggle');
//}

//function AlertaOk(Titulo, Msg, Largura, Altura) {

//    var strTemplate = '<div id="modal" class="modal fade" role="dialog">' +
//        '<div class="modal-dialog" style="width:' + Largura + '; height:' + Altura + '">' +
//        '<div class="modal-content" style="width:' + Largura + '; height:' + Altura + '">' +
//        '<div class="modal-header bg-dark">' +
//        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
//        '<h4 class="modal-title text-white">' + Titulo + '</h4>' +
//        '</div>' +
//        '<div class="modal-body" style="text-align:center">' +
//        '<div>' + Msg + '</div>' +
//        '</div>' +
//        '<div class="modal-footer" style="text-align:center">' +
//        '<button type="button" class="btn btn-warning" data-dismiss="modal" style="width:200px">Ok</button>' +
//        '</div>' +
//        '</div>' +
//        '</div>' +
//        '</div>';

//    $(strTemplate).modal('toggle');

//}

function AlertaRedirect(Titulo, Msg, Method) {
    var strTemplate =
        '<div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">' +
        '    <div class="modal-dialog" role="document">' +
        '        <div class="modal-content">' +
        '            <div class="modal-header bg-dark">' +
        '                <h5 class="modal-title text-white" id="exampleModalLabel">' + Titulo + '</h5>' +
        //'                <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
        //'                    <span aria-hidden="true">&times;</span>' +
        //'                </button>' +
        '            </div>' +
        '            <div class="modal-body">' + Msg +
        '            </div>' +
        '            <div class="modal-footer">' +
        '                <button type="button" class="btn btn-secondary" data-dismiss="modal" onclick="' + Method + '">Fechar</button>' +
        '            </div>' +
        '        </div>' +
        '    </div>' +
        '</div>';

    $(strTemplate).modal('toggle');

}

function ConfirmarOkCancel(Titulo, Msg, CallBack) {

    //$('#modalOkCancel').remove();
    //var functionCancel = function () {
    //    $('#modalOkCancel').modal('toggle');
    //};

    //var strScript = '<script> ' +
        //'var functionOk = ' + CallBackOk + '; ' +
        //'var functionCancel = ' + functionCancel + '; ' +
        //'</script>';

    var strTemplate =
        '<div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">' +
        '    <div class="modal-dialog" role="document">' +
        '        <div class="modal-content">' +
        '            <div class="modal-header bg-dark">' +
        '                <h5 class="modal-title text-white" id="exampleModalLabel">' + Titulo + '</h5>' +
        //'                <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
        //'                    <span aria-hidden="true">&times;</span>' +
        //'                </button>' +
        '            </div>' +
        '            <div class="modal-body">' + Msg +
        '            </div>' +
        '            <div class="modal-footer">' +
        '                <button type="button" class="btn btn-secondary" data-dismiss="modal" onclick="' + CallBack + '">OK</button>' +
        '                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>' +
        '            </div>' +
        '        </div>' +
        '    </div>' +
        '</div>';

    $(strTemplate).modal('toggle');
}

function ConfirmarSimNao(Titulo, Msg, CallBack) {

    //$('#modalSimNao').remove();

    //var strScript = '<script> ' +
    //    'var functionSim = ' + CallBackSim + ';' +
    //    '</script>';

    var strTemplate =
        '<div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">' +
        '    <div class="modal-dialog" role="document">' +
        '        <div class="modal-content">' +
        '            <div class="modal-header bg-dark">' +
        '                <h5 class="modal-title text-white" id="exampleModalLabel">' + Titulo + '</h5>' +
        //'                <button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
        //'                    <span aria-hidden="true">&times;</span>' +
        //'                </button>' +
        '            </div>' +
        '            <div class="modal-body">' + Msg +
        '            </div>' +
        '            <div class="modal-footer">' +
        '                <button type="button" class="btn btn-secondary" data-dismiss="modal" onclick="' + CallBack + '">Sim</button>' +
        '                <button type="button" class="btn btn-secondary" data-dismiss="modal">Não</button>' +
        '            </div>' +
        '        </div>' +
        '    </div>' +
        '</div>';

    $(strTemplate).modal('toggle');
}

//function AlertaOkCancel(Titulo, Msg, Largura, Altura, CallBackOk) {

//    $('#modalOkCancel').remove();
//    var functionCancel = function () {
//        $('#modalOkCancel').modal('toggle');
//    };
//    var strScript = '<script> ' +
//        'var functionOk = ' + CallBackOk + '; ' +
//        'var functionCancel = ' + functionCancel + '; ' +
//        '</script>';

//    var strTemplate = '<div id="modalOkCancel" class="modal fade" role="dialog">' +
//        strScript +
//        '<div class="modal-dialog" style="width:' + Largura + '; height:' + Altura + '">' +
//        '<div class="modal-content" style="width:' + Largura + '; height:' + Altura + '">' +
//        '<div class="modal-header bg-dark">' +
//        '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
//        '<h4 class="modal-title text-white">' + Titulo + '</h4>' +
//        '</div>' +
//        '<div class="modal-body" style="text-align:center">' +
//        '<div class="row" style="font-size:12px;">' + Msg + '</div>' +
//        '<div class="row" style="margin-top:10px">' +
//        '<div class="col-sm-6 col-sm-offset-3">' +
//        '<input id="btnOk" onclick="functionOk()" type="button" value="Ok" class="btn btn-warning" />' +
//        '<input id="btnCancel" onclick="functionCancel()"  type="button" value="Cancelar" class="btn btn-default" style="margin-left:5px"/>' +
//        '</div>' +
//        '<div>' +
//        '</div>' +

//        '</div>' +
//        '</div>' +
//        '</div>';

//    $(strTemplate).modal('toggle');
//}

function ExibirLoader() {
    var template = '<div id="loadingDiv" class="loader-background"><div class="loader"></div></div>';
    $(template).show();
}

function EsconderLoader() {
    $("#loadingDiv").hide();
}

function LogErrorAPI(url, method, xhr, status, error) {
    console.log('---------------------------------------');
    console.log('Erro ao realizar o [' + method + '] em [' + url + ']');
    console.log('XHR: [' + xhr.responseText + ']');
    console.log('STATUS: [' + status + ']');
    console.log('ERROR: [' + error + ']');
    console.log('---------------------------------------');
    //LogErro(xhr.responseText, url, 0, 0, '');
}

function GetDataAsync(Url, CallBack, ErrorCallBack, CompleteCallBack) {
    $.ajax({
        type: 'GET',
        url: Url,
        async: true,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        //headers: {
        //    "Authorization": "Bearer " + localStorage.getItem('accessToken')
        //},
        timeout: 90000, // 90 segundos
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        success: function (data) {
            $("#loader").css("display", "none");
            return CallBack(data);
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            if (ErrorCallBack)
                ErrorCallBack(xhr, status, error);
            else
                LogErrorAPI(Url, "GET", xhr, status, error);
        },
        complete: function (data) {
            $("#loader").css("display", "none");
            if (CompleteCallBack)
                CompleteCallBack(data);
        }
    });
}

function GetData(Url, CallBack, ErrorCallBack) {
    $.ajax({
        type: 'GET',
        url: Url,
        async: false,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        cache: false,
        //headers: {
        //    "Authorization": "Bearer " + localStorage.getItem('accessToken')
        //},
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            return CallBack(data);
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            if (ErrorCallBack)
                ErrorCallBack(xhr, status, error);
            else
                LogErrorAPI(Url, "GET", xhr, status, error);
        }
    });
}

function PostData(Url, Data, CallBack, ErrorCallBack) {
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: Url,
        async: false,
        data: Data,
        dataType: 'json',
        cache: false,
        //headers: { "Authorization": "Bearer " + localStorage.getItem('accessToken') },
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            CallBack(data);
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            if (ErrorCallBack)
                ErrorCallBack(xhr, status, error);
            else
                LogErrorAPI(Url, "POST", xhr, status, error);
        }
    });
}

function PostDataAsync(Url, Data, CallBack, ErrorCallBack) {
    $.ajax({
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        url: Url,
        async: true,
        data: Data,
        dataType: 'json',
        cache: false,
        //headers: {
        //    "Authorization"
        //    : "Bearer " + localStorage.getItem('accessToken')
        //},
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (CallBack) {
                CallBack(data);
            }
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            if (ErrorCallBack)
                ErrorCallBack(xhr, status, error);
            else
                LogErrorAPI(Url, "POST", xhr, status, error);
        }
    });
}

function PutData(Url, Data, CallBack, ErrorCallBack) {
    $.ajax({
        type: 'PUT',
        contentType: 'application/json; charset=utf-8',
        url: Url,
        data: Data,
        async: false,
        dataType: 'json',
        cache: false,
        //headers: { "Authorization": "Bearer " + localStorage.getItem('accessToken') },
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            CallBack(data);
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            if (ErrorCallBack)
                ErrorCallBack(xhr, status, error);
            else
                LogErrorAPI(Url, "PUT", xhr, status, error);
        }
    });
}

function PutDataSync(Url, Data, CallBack) {
    $.ajax({
        type: 'PUT',
        contentType: 'application/json; charset=utf-8',
        url: Url,
        async: false,
        data: Data,
        dataType: 'json',
        cache: false,
        //headers: { "Authorization": "Bearer " + localStorage.getItem('accessToken') },
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            CallBack(data);
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            console.log('---------------------------------------');
            console.log('Erro ao postar dados em [' + Url + ']');
            console.log('XHR: [' + xhr + ']');
            console.log('STATUS: [' + status + ']');
            console.log('ERROR: [' + error + ']');
            console.log('---------------------------------------');
            //LogErro(xhr.responseText, Url, 0, 0, '');
        }
    });
}

function DeleteData(Url, Data, CallBack) {
    $.ajax({
        type: 'DELETE',
        contentType: 'application/json; charset=utf-8',
        url: Url,
        data: Data,
        dataType: 'json',
        cache: false,
        //headers: { "Authorization": "Bearer " + localStorage.getItem('accessToken') },
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            CallBack(data);
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            console.log('---------------------------------------');
            console.log('Erro ao postar dados em [' + Url + ']');
            console.log('XHR: [' + xhr + ']');
            console.log('STATUS: [' + status + ']');
            console.log('ERROR: [' + error + ']');
            console.log('---------------------------------------');
            //LogErro(xhr.responseText, Url, 0, 0, '');
        }
    });
}

function DeleteDataSync(Url, Data, CallBack) {
    $.ajax({
        type: 'DELETE',
        contentType: 'application/json; charset=utf-8',
        url: Url,
        async: false,
        data: Data,
        dataType: 'json',
        cache: false,
        //headers: { "Authorization": "Bearer " + localStorage.getItem('accessToken') },
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            CallBack(data);
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            console.log('---------------------------------------');
            console.log('Erro ao postar dados em [' + Url + ']');
            console.log('XHR: [' + xhr + ']');
            console.log('STATUS: [' + status + ']');
            console.log('ERROR: [' + error + ']');
            console.log('---------------------------------------');
            //LogErro(xhr.responseText, Url, 0, 0, '');
        }
    });
}

function configurarJQueryDataTable(seletor) {
    var table = $(seletor).DataTable({
        "language": {
            "lengthMenu": "Quantidade de itens por página: _MENU_",
            "zeroRecords": "Nenhum item encontrado",
            "info": "Página _PAGE_ de _PAGES_",
            "infoEmpty": "",
            "infoFiltered": "(filtrando de _MAX_ itens no total)",
            paginate: {
                first: '«',
                previous: '‹',
                next: '›',
                last: '»'
            },
            "search": "Pesquisar:"
        },
        "pagingType": "full_numbers"
    });

    $(seletor + ' tbody').on('click', 'tr', function () {
        table.$('tr.selected').removeClass('selected');
        $(this).closest('tr').addClass('selected');
    });

    $('#button').click(function () {
        table.row('.selected').remove().draw(false);
    });
}

function GetDominio() {
    var Url = "";
    if (document.location.origin)
        Url = document.location.origin;
    else
        Url = document.location.protocol + "//" + document.location.host;

    return Url;
}

function ValidarData(campo) {

    if (campo.val() != "") {
        var date = campo.val();
        var ardt = new Array;
        var ExpReg = new RegExp("(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[012])/[12][0-9]{3}");
        ardt = date.split("/");
        erro = false;
        if (date.search(ExpReg) == -1) {
            erro = true;
        }
        else if (((ardt[1] == 4) || (ardt[1] == 6) || (ardt[1] == 9) || (ardt[1] == 11)) && (ardt[0] > 30))
            erro = true;
        else if (ardt[1] == 2) {
            if ((ardt[0] > 28) && ((ardt[2] % 4) != 0))
                erro = true;
            if ((ardt[0] > 29) && ((ardt[2] % 4) == 0))
                erro = true;
        }
        if (erro) {
            Alerta("Aviso", "O valor informado não é válido", "400px", "100px");
            campo.val("");
            return false;
        }
    }
    return true;
}

//function getParameterByName(name, url) {
//    if (!url) url = window.location.href;
//    name = name.replace(/[\[\]]/g, "\\$&");
//    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
//        results = regex.exec(url);
//    if (!results) return null;
//    if (!results[2]) return '';
//    return decodeURIComponent(results[2].replace(/\+/g, " "));
//}

function validarCPF(cpf) {
    var numeros, digitos, soma, i, resultado, digitos_iguais;
    digitos_iguais = 1;
    if (cpf.length < 11)
        return false;
    for (i = 0; i < cpf.length - 1; i++)
        if (cpf.charAt(i) != cpf.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    if (!digitos_iguais) {
        numeros = cpf.substring(0, 9);
        digitos = cpf.substring(9);
        soma = 0;
        for (i = 10; i > 1; i--)
            soma += numeros.charAt(10 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(0))
            return false;
        numeros = cpf.substring(0, 10);
        soma = 0;
        for (i = 11; i > 1; i--)
            soma += numeros.charAt(11 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(1))
            return false;
        return true;
    }
    else
        return false;
}

function validarCNPJ(cnpj) {

    cnpj = cnpj.replace(/[^\d]+/g, '');

    if (cnpj == '') return false;

    if (cnpj.length != 14)
        return false;

    // Elimina CNPJs invalidos conhecidos
    if (cnpj == "00000000000000" ||
        cnpj == "11111111111111" ||
        cnpj == "22222222222222" ||
        cnpj == "33333333333333" ||
        cnpj == "44444444444444" ||
        cnpj == "55555555555555" ||
        cnpj == "66666666666666" ||
        cnpj == "77777777777777" ||
        cnpj == "88888888888888" ||
        cnpj == "99999999999999")
        return false;

    // Valida DVs
    tamanho = cnpj.length - 2;
    numeros = cnpj.substring(0, tamanho);
    digitos = cnpj.substring(tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0))
        return false;

    tamanho = tamanho + 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(1))
        return false;

    return true;

}

function FormatarData(data) {

    if (data != null && data != "") {
        var dataInput = new Date(data);
        return dataInput.toISOString().substr(0, 10).split('-').reverse().join('/');
        //.toLocaleString();
    }
    else
        return "";
}

function FormatarValor(valor) {
    if (valor)
        return valor.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1.').replace(/.([^.]*)$/, ",$1");
    else
        return "0,00";
}

function FormatarValorJson(valor) {
    if (valor)
        return valor.replace(/\./g, '').replace(/\,/g, '.');
    else
        return "";
}

function FormatarDataJson(data) {

    if (data) {
        return data.split('/').reverse().join('-');
    }
    else
        return "";
}

function ValidaVersaoIE() {
    var valido = false;
    var agent = navigator.userAgent;
    var reg = /MSIE\s?(\d+)(?:\.(\d+))?/i;
    var matches = agent.match(reg);
    //Validação IE10
    if (matches != null)
        valido = matches[1] == 10;

    //Validação IE 11
    if (!valido)
        valido = !!window.MSInputMethodContext && !!document.documentMode;

    return valido;
}

//window.salvarVeiculo = function (montadora, veiculo, modelo, motor, ano) {

//    $('#hdnMontadora').val(montadora);
//    $('#hdnVeiculo').val(veiculo);
//    $('#hdnModelo').val(modelo);
//    $('#hdnMotor').val(motor);
//    $('#hdnAno').val(ano);
//    $('#iframeModal').modal('hide');

//};

// Numeric only control handler
jQuery.fn.ForceNumericOnly =
    function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                var key = e.charCode || e.keyCode || 0;
                // allow backspace, tab, delete, enter, arrows, numbers and keypad numbers ONLY
                // home, end, period, and numpad decimal
                return (
                    key == 8 ||
                    key == 9 ||
                    key == 13 ||
                    key == 46 ||
                    key == 110 ||
                    key == 190 ||
                    (key >= 35 && key <= 40) ||
                    (key >= 48 && key <= 57) ||
                    (key >= 96 && key <= 105));
            });
        });
    };

var HlMessages = (function () {

    var setTitleAndContent = function (title, content, modal) {
        $(modal).find("#modalTitle").html(title);
        $(modal).find("#modalContent").html(content);
    };

    var showSuccessMessage = function (title, content) {
        var modal = "#hlModalSuccess";
        setTitleAndContent(title, content, modal);

        $(modal).modal("show");
    };

    var showAlertMessage = function (title, content) {
        var modal = "#hlModalAlert";
        setTitleAndContent(title, content, modal);

        $(modal).modal("show");
    };

    var showErrorMessage = function (title, content) {
        var modal = "#hlModalError";
        setTitleAndContent(title, content, modal);

        $(modal).modal("show");
    };

    var showPrimaryMessage = function (title, content) {
        var modal = "#hlModal";
        setTitleAndContent(title, content, modal);

        $(modal).modal("show");
    };

    var showModelConfirmDialog = function (callback, content) {

        var modal = "#hlModalConfirmDelete";

        setTitleAndContent("Deletar", content, modal);

        $("#hlModalConfirmDelete").find("#btnConfirmar")
            .click(function () {
                callback();
                $("#hlModalConfirmDelete").modal("close");
            });

        $(modal).modal("show");
    }

    var showModelConfirmOkDialog = function (callback, content, title) {

        var modal = "#hlModalConfirmOk";

        setTitleAndContent(title, content, modal);

        $("#hlModalConfirmOk").find("#btnConfirmar")
            .click(function () {
                callback();
                $("#hlModalConfirmOk").modal("close");
            });

        $(modal).modal("show");
    }

    var showModelUploadFile = function (callback, content) {

        var modal = "#hlModalFileUpload";

        setTitleAndContent("Upload", content, modal);

        $("#fileUploadProgress").css("width", "0%");

        $(modal).modal("show");

        $(modal).on("shown.bs.modal", function () {
            $("#btnFileUpload").click();
        });
    }

    return {
        ShowSuccessMessage: showSuccessMessage,
        ShowAlertMessage: showAlertMessage,
        ShowErrorMessage: showErrorMessage,
        ShowPrimaryMessage: showPrimaryMessage,
        ShowModelConfirmDialog: showModelConfirmDialog,
        ShowModelUploadFile: showModelUploadFile,
        ShowModelConfirmOkDialog: showModelConfirmOkDialog
    };
}());

//A função 'formatarOption' espera receber um objeto com o seguinte formato: { "text" :null, "id": null}
function configurarDropDownAsync(selectId,
    url,
    formatarOption,
    onCompleteSuccess,
    tipo,
    dados,
    selecionarPrimeiroSeHouverApenasUmItem) {
    var array = [];
    var select = document.getElementById(selectId);
    RequestAPIIdentity(url,
        dados ? dados : null,
        tipo ? tipo : 'GET',
        true,
        function (data) {
            if (data.length > 0) {
                $.each(data, function (key, item) {
                    array.push(formatarOption(item));
                });
                $('#' + selectId).select2({
                    placeholder: "Selecione...",
                    language: "pt-BR",
                    data: array,
                    allowClear: false,
                    escapeMarkup: function (markup) { return markup; },
                    minimumInputLength: 0,
                });

                if (selecionarPrimeiroSeHouverApenasUmItem == true
                    && array.length == 1) {
                    selecionarValorDropDown(selectId, array[0].id);
                }
            }
            else
                configurarDropDownVazio(selectId);


            //TODO: finalizar o tratamento de tabindex
            //var tabIndex = select.getAttribute('tabindex');
            //$('#select2-' + selectId  + '-container').prop('tabindex', tabindex);

            if (onCompleteSuccess)
                onCompleteSuccess();
        },
        function () {
            configurarDropDownVazio(selectId);
        })
}

function configurarDropDown(selectId,
    url,
    formatarOption,
    onCompleteSuccess,
    tipo,
    dados,
    selecionarPrimeiroSeHouverApenasUmItem) {
    var array = [];
    var select = document.getElementById(selectId);
    RequestAPIIdentity(url,
        dados ? dados : null,
        tipo ? tipo : 'GET',
        false,
        function (data) {
            if (data.length > 0) {
                $.each(data, function (key, item) {
                    array.push(formatarOption(item));
                });
                $('#' + selectId).select2({
                    placeholder: "Selecione...",
                    language: "pt-BR",
                    data: array,
                    allowClear: false,
                    escapeMarkup: function (markup) { return markup; },
                    minimumInputLength: 0,
                });

                if (selecionarPrimeiroSeHouverApenasUmItem == true
                    && array.length == 1) {
                    selecionarValorDropDown(selectId, array[0].id);
                }
            }
            else
                configurarDropDownVazio(selectId);




            if (onCompleteSuccess)
                onCompleteSuccess();
        },
        function () {
            configurarDropDownVazio(selectId);
        })
}

function configurarDropDownVazio(selectId) {
    $("#" + selectId).select2({
        placeholder: "Selecione...",
        language: "pt-BR",
        data: [],
        allowClear: false,
        escapeMarkup: function (markup) { return markup; },
        minimumInputLength: 0,
    });
}

function selecionarValorDropDown(selectId, itemId) {
    $("#" + selectId).val(itemId);
    $("#" + selectId).trigger('change');
}

function obterTextoDropDown(select) {
    try {
        var data = select.select2('data');
        if (data && data.length > 0)
            return data[0].text;
        else
            return "";
    }
    catch (e) {
        return "";
    }
}

//O parametro item deve ter o seguinte formato: { "text" :null, "id": null}
function selecionarValorAutoComplete(selectId, item) {
    var opt = new Option(item.text, item.id, false, false);
    $('#' + selectId).append(opt).trigger('change');
}

function RequestAPIIdentity(requestUrl, requestData, requestMethod, requestAsync, successCallBack, errorCallBack) {
    $.ajax({
        type: requestMethod,
        contentType: 'application/json; charset=utf-8',
        url: requestUrl,
        async: requestAsync,
        data: requestData,
        delay: 250,
        dataType: 'json',
        cache: false,
        headers: {
            "Authorization"
                : "Bearer " + localStorage.getItem('accessToken')
        },
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (successCallBack) {
                successCallBack(data);
            }
        },
        error: function (xhr, status, error) {
            $("#loader").css("display", "none");
            if (errorCallBack)
                errorCallBack(xhr, status, error);
            else
                LogErrorAPI(Url, "POST", xhr, status, error);
        }
    });
}

function Geradatadehoje() {
    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();

    var output = (day < 10 ? '0' : '') + day + "/"
        + (month < 10 ? '0' : '') + month + '/'
        + d.getFullYear();

    return output;
}

function FormataDataComparar(date) {
    var parts = date.split("/");
    date = new Date(parts[1] + "/" + parts[0] + "/" + parts[2]);
    return date.getTime();
}

function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function atribuirParametrosPaginacao(containerGrid, actionConsulta, jsonData) {
    //localStorage.clear();
    localStorage.setItem("container", containerGrid);
    localStorage.setItem("action", actionConsulta);
    localStorage.setItem("jsonData", jsonData);
}

function SelecionarArquivo(fileInput, labelInput, buttonUpload) {
    var texto = 'Selecionar Arquivo';
    buttonUpload.removeAttr("style").hide();

    if (fileInput.files.length > 0) {
        texto = fileInput.files[0].name;
        buttonUpload.show();
    }
    labelInput.text(texto);
}

function UploadFiles(url, callback) {
    var data = new FormData();

    //Add the Multiple selected files into the data object
    var files = $("input[type='file']").get(0).files;
    for (i = 0; i < files.length; i++) {
        data.append("files" + i, files[i]);
    }

    if (files.length > 0) {
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            dataType: "json",
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            success: function (data) {
                $("#loader").css("display", "none");
                callback(data);
            },
            error: function () {
                $("#loader").css("display", "none");
                Alerta("Falha","Não foi possível enviar o arquivo.");
            },
        });
    }
}


function UploadFilesFoto(url, callback) {
    var data = new FormData();

    //Add the Multiple selected files into the data object
    var files = $("input[type='file']").get(1).files; //segundo input file (fotos)
    for (i = 0; i < files.length; i++) {
        data.append("files" + i, files[i]);
    }

    if (files.length > 0) {
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            dataType: "json",
            cache: false,
            async: false,
            contentType: false,
            processData: false,
            success: function (data) {
                $("#loader").css("display", "none");
                callback(data);
            },
            error: function () {
                $("#loader").css("display", "none");
                Alerta("Falha", "Não foi possível enviar o arquivo.");
            },
        });
    }
}


function UploadFilesWF(url) {
    var data = new FormData();

    //Add the Multiple selected files into the data object
    var files = $("input[type='file']").get(0).files;
    for (i = 0; i < files.length; i++) {
        data.append("files" + i, files[i]);
    }

    var guid = "";

    if (files.length > 0) {
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            dataType: "json",
            contentType: false,
            processData: false,
            success: function (data) {
                $("#loader").css("display", "none");
                guid = data.file[0];
            },
            error: function () {
                $("#loader").css("display", "none");
                Alerta("Falha", "Não foi possível enviar o arquivo.");
            }
        });
    }

    return guid;
}


function FormatarDataHora(valordata) {
    var data = new Date(valordata);
    var dia = data.getDate().toString();
    var diaF = (dia.length == 1) ? '0' + dia : dia;
    var mes = (data.getMonth() + 1).toString(); //+1 pois no getMonth Janeiro começa com zero.
    var mesF = (mes.length == 1) ? '0' + mes : mes;
    var anoF = data.getFullYear();
    var hora = data.getHours().toString();
    var horaF = (hora.length == 1) ? '0' + hora : hora;
    var min = data.getMinutes().toString();
    var minF = (min.length == 1) ? '0' + min : min;
    return diaF + "/" + mesF + "/" + anoF + ' ' + horaF + ':' + minF;
}

function FormatarSoData(valordata) {
    var data = new Date(valordata);
    var dia = data.getDate().toString();
    var diaF = (dia.length == 1) ? '0' + dia : dia;
    var mes = (data.getMonth() + 1).toString(); //+1 pois no getMonth Janeiro começa com zero.
    var mesF = (mes.length == 1) ? '0' + mes : mes;
    var anoF = data.getFullYear();
    var hora = data.getHours().toString();
    var horaF = (hora.length == 1) ? '0' + hora : hora;
    var min = data.getMinutes().toString();
    var minF = (min.length == 1) ? '0' + min : min;
    return diaF + "/" + mesF + "/" + anoF;
}

