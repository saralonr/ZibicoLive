var connection = $.hubConnection(zbcSocket+"/signalr", { useDefaultPath: false });
var hub = connection.createHubProxy('liveHub');

hub.on('chatLogin', function (response, name, dto) {
    if (response === true) {
        var cid = $('#livechat-datavalue').val();
        var session = $('#livechat-session').val();

        $("#livechat-username").html(name);
        $("#live-chat-login-area").fadeOut(300);
        setTimeout(function () {
            $("#live-chat-login-area").remove();
        }, 1000);
        $("#live-chat-history-area").fadeIn(400);

        if (dto !== null) {
            var last = false;
            $.each(dto.Steps, function (i, v) {
                //console.log("step=" + v.ID);
                $("#live-chat-history-area").append('<div class="chat-message cs-clearfix"></div>');
                $(".chat-message").append('<img src="' + zbcScript+'/tags/images/chatbot.png" alt = "" width = "32" height = "32">');
                $(".chat-message").append('<div class="chat-message-content cs-clearfix"></div>');
                $(".chat-message-content").append('<span class="chat-time"></span>');
                $(".chat-time").append(moment().format("HH:mm"));
                $(".chat-message-content").append('<b>Robot</b>');
                $(".chat-message-content").append('<p class="" data-step="' + v.StepNumber + '">' + v.Question + '</p>');
                $(".chat-message-content").append('<ul class="live-chat-step"></ul>');
                dto.Options.forEach(function (opt) {
                    if (opt.StepID === v.ID) {
                        $(".live-chat-step").append('<li class="live-chat-option" data-option-li="' + opt.ID + '"></li>');

                        if (dto.Values.length === 0) {
                            last = true;
                            $("[data-option-li='" + opt.ID + "']").html('<button class="btn btn-primary btn-xs live-step-option" type="button" data-step-option="' + opt.UniqueId + '">' + opt.OptionDescription + '</button> ');
                        }
                        else {
                            dto.Values.forEach(function (optVal) {
                                if (optVal.OptionID === opt.ID && optVal.StepID === v.ID) {
                                    $("[data-option-li='" + opt.ID + "']").html('<button class="btn btn-success disabled btn-xs live-step-option" type="button" data-step-option="' + opt.UniqueId + '" disabled>' + opt.OptionDescription + '</button> ');
                                }
                                else if (optVal.OptionID !== opt.ID && optVal.StepID === v.ID) {
                                    $("[data-option-li='" + opt.ID + "']").html('<button class="btn btn-danger disabled btn-xs live-step-option" type="button" data-step-option="' + opt.UniqueId + '" disabled>' + opt.OptionDescription + '</button> ');
                                }
                                else {
                                    last = true;
                                    $("[data-option-li='" + opt.ID + "']").html('<button class="btn btn-primary btn-xs live-step-option" type="button" data-step-option="' + opt.UniqueId + '">' + opt.OptionDescription + '</button> ');
                                }
                            });
                        }
                    }
                });
                $("#live-chat-history-area").append("<hr>");

                if (last) {
                    return false;
                }

            });
        }

        $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
        $('.live-step-option').on('click', function () {
            var optionUniqueId = $(this).attr("data-step-option");
            var cid = $('#livechat-datavalue').val();
            var session = $('#livechat-session').val();

            $.each($(this).closest(".live-chat-step").find("button"), function (i, v) {
                $(v).removeClass("btn-primary");
                if ($(v).attr("data-step-option") !== optionUniqueId) {
                    $(v).addClass("btn-danger").addClass("disabled");
                    $(v).attr("disabled", "disabled");
                } else {
                    $(v).addClass("btn-success").addClass("disabled");
                    $(v).attr("disabled", "disabled");
                }
            });

            //TEST---MUSTERI MESAJI
            $(this).closest(".live-chat-step").remove();

            $("#live-chat-history-area").append('<div class="chat-message cs-clearfix"></div>');
            $(".chat-message:last").append('<img src="' + zbcScript+'/tags/images/users.png" alt = "" width = "32" height = "32">');
            $(".chat-message:last").append('<div class="chat-message-content cs-clearfix"></div>');
            $(".chat-message-content:last").append('<span class="chat-time">' + moment().format("HH:mm") + '</span>');

            $(".chat-message-content:last").append('<b>' + $("#livechat-username").html() + '</b>');
            $(".chat-message-content:last").append('<p class="">' + $(this).text() + '</p>');

            $("#live-chat-history-area").append("<hr>");
            $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
            //TEST---MUSTERI MESAJI

            hub.invoke('SetStepOption', optionUniqueId, cid, session);
        });

        if ($('.finish-chat-step').length === 0) {
            $(".chat").append('<button class="btn btn-warning btn-xs pull-right finish-chat-step">Sohbeti Bitir</button>');
            $('.finish-chat-step').on('click', function () {
                var cid = $('#livechat-datavalue').val();
                var session = $('#livechat-session').val();
                hub.invoke('FinishChatStep', cid, session);
            });
        }
    }
});
hub.on('setStepOption', function (dto) {

    if (dto !== null && dto.Step.IsFinished === false) {
        $("#live-chat-history-area").append('<div class="chat-message cs-clearfix"></div>');
        $(".chat-message:last").append('<img src="' + zbcScript+'/tags/images/chatbot.png" alt = "" width = "32" height = "32">');
        $(".chat-message:last").append('<div class="chat-message-content cs-clearfix"></div>');
        $(".chat-message-content:last").append('<span class="chat-time"></span>');
        $(".chat-time:last").append(moment().format("HH:mm"));

        $(".chat-message-content:last").append('<b>Robot</b>');
        $(".chat-message-content:last").append('<p class="" data-step="' + dto.Step.StepNumber + '">' + dto.Step.Question + '</p>');
        $(".chat-message-content:last").append('<ul class="live-chat-step"></ul>');
        dto.Options.forEach(function (opt) {
            if (opt.StepID === dto.Step.ID) {
                $(".live-chat-step:last").append('<li class="live-chat-option" data-option-li="' + opt.ID + '"></li>');
                $("[data-option-li='" + opt.ID + "']").html('<button class="btn btn-primary btn-xs live-step-option" type="button" data-step-option="' + opt.UniqueId + '">' + opt.OptionDescription + '</button> ');
            }
        });

        $("#live-chat-history-area").append("<hr>");
        $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);

        $('.live-step-option').on('click', function () {
            var optionUniqueId = $(this).attr("data-step-option");
            var cid = $('#livechat-datavalue').val();
            var session = $('#livechat-session').val();

            $.each($(this).closest(".live-chat-step").find("button"), function (i, v) {
                $(v).removeClass("btn-primary");
                if ($(v).attr("data-step-option") !== optionUniqueId) {
                    $(v).addClass("btn-danger").addClass("disabled");
                    $(v).attr("disabled", "disabled");
                } else {
                    $(v).addClass("btn-success").addClass("disabled");
                    $(v).attr("disabled", "disabled");
                }
            });

            //TEST---MUSTERI MESAJI
            $(this).closest(".live-chat-step").remove();

            $("#live-chat-history-area").append('<div class="chat-message cs-clearfix"></div>');
            $(".chat-message:last").append('<img src="' + zbcScript+'/tags/images/users.png" alt = "" width = "32" height = "32">');
            $(".chat-message:last").append('<div class="chat-message-content cs-clearfix"></div>');
            $(".chat-message-content:last").append('<span class="chat-time">' + moment().format("HH:mm") + '</span>');

            $(".chat-message-content:last").append('<b>' + $("#livechat-username").html() + '</b>');
            $(".chat-message-content:last").append('<p class="">' + $(this).text() + '</p>');

            $("#live-chat-history-area").append("<hr>");
            $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
            //TEST---MUSTERI MESAJI

            hub.invoke('SetStepOption', optionUniqueId, cid, session);
        });
    }
    else if (dto !== null && dto.Step.IsFinished === true) {
        $("#live-chat-history-area").append('<div class="chat-message cs-clearfix"></div>');
        $(".chat-message:last").append('<img src="' + zbcScript+'/tags/images/chatbot.png" alt = "" width = "32" height = "32">');
        $(".chat-message:last").append('<div class="chat-message-content cs-clearfix"></div>');
        $(".chat-message-content:last").append('<span class="chat-time"></span>');
        $(".chat-time:last").append(moment().format("HH:mm"));

        $(".chat-message-content:last").append('<b>Robot</b>');
        $(".chat-message-content:last").append('<p class="" data-step="' + dto.Step.StepNumber + '">' + dto.Step.Question + '</p>');

        $("#live-chat-history-area").append("<hr>");
        $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
    }
    else {
        $("#live-chat-history-area").append('<div class="chat-message cs-clearfix"></div>');
        $(".chat-message:last").append('<center><b>İyi günler dileriz...</b></center>');
        $("#live-chat-history-area").append("<hr>");
        $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
    }

    if ($('.finish-chat-step').length === 0) {
        $(".chat").append('<button class="btn btn-warning btn-xs pull-right finish-chat-step">Sohbeti Bitir</button>');
    }
});
hub.on('finishChatStep', function (response) {
    if (response === true) {
        $("#live-chat-history-area").html("");
        setTimeout(function () {
            var name = $("#livechat-username").html();
            var cid = $('#livechat-datavalue').val();
            var session = $('#livechat-session').val();
            var ipAddress = "";

            $.get("https://json.geoiplookup.io/api", function () { }).done(function (data) {
                ipAddress = data.ip;
                hub.invoke('ChatLogin', name, cid, ipAddress, session);
            });
        }, 500);
    }
});

connection.start({ jsonp: true }, function () {
    var session = $('#livechat-session').val();
    hub.invoke('OnConnectSession', session);
});

$(function () {
    var lastConnection = $("#livechat-last").val();
    console.log(lastConnection);
    if (lastConnection !== "") {
        $("#live-chat-history-area").show();
        $("#live-chat-login-area").remove();
        $("#livechat-username").html(lastConnection);

        if ($('.finish-chat-step').length === 0) {
            $(".chat").append('<button class="btn btn-warning btn-xs pull-right finish-chat-step">Sohbeti Bitir</button>');
        }
    }

    $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
    $('#live-chat .chat-header').on('click', function () {
        $('.chat').slideToggle(300, 'swing');
        $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
    });
    $('.chat-close').on('click', function (e) {
        e.preventDefault();
        $('#live-chat').fadeOut(300);
    });
    $('#login-chat-button').on('click', function () {
        var name = $('#login-chat-name').val();
        var cid = $('#livechat-datavalue').val();
        var session = $('#livechat-session').val();
        var ipAddress = "";

        $.get("https://json.geoiplookup.io/api", function () { }).done(function (data) {
            ipAddress = data.ip;
            hub.invoke('ChatLogin', name, cid, ipAddress, session);
        });

    });
    $('.finish-chat-step').on('click', function () {
        var cid = $('#livechat-datavalue').val();
        var session = $('#livechat-session').val();
        hub.invoke('FinishChatStep', cid, session);
    });
    $('.live-step-option').on('click', function () {
        var optionUniqueId = $(this).attr("data-step-option");
        var cid = $('#livechat-datavalue').val();
        var session = $('#livechat-session').val();

        $.each($(this).closest(".live-chat-step").find("button"), function (i, v) {
            $(v).removeClass("btn-primary");
            if ($(v).attr("data-step-option") !== optionUniqueId) {
                $(v).addClass("btn-danger").addClass("disabled");
                $(v).attr("disabled", "disabled");
            } else {
                $(v).addClass("btn-success").addClass("disabled");
                $(v).attr("disabled", "disabled");
            }
        });

        //TEST---MUSTERI MESAJI
        $(this).closest(".live-chat-step").remove();

        $("#live-chat-history-area").append('<div class="chat-message cs-clearfix"></div>');
        $(".chat-message:last").append('<img src="' + zbcScript+'/tags/images/users.png" alt = "" width = "32" height = "32">');
        $(".chat-message:last").append('<div class="chat-message-content cs-clearfix"></div>');
        $(".chat-message-content:last").append('<span class="chat-time">' + moment().format("HH:mm") + '</span>');

        $(".chat-message-content:last").append('<b>' + $("#livechat-username").html() + '</b>');
        $(".chat-message-content:last").append('<p class="">' + $(this).text() + '</p>');

        $("#live-chat-history-area").append("<hr>");
        $('.chat-history').scrollTop($('.chat-history')[0].scrollHeight);
        //TEST---MUSTERI MESAJI

        hub.invoke('SetStepOption', optionUniqueId, cid, session);
    });
});