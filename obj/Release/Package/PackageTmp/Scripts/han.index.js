
$(function () {
    //加载界面首页需要的数据
    //4条置顶内容、20条最新的综合帖子、5条温馨通道（这边可以放办公常用网站链接,暂时为静态链接）、回帖周榜、10片本周热议、用户登入信息（包括连续签到天数）
    //han.ajax({
    //    url: '/Home/GetIndexData',//need add
    //    success: function (data) {
    //        if (data.Success) {
    //            //执行json数据解析逻辑
    //            var _isOk = ParseIndexData(data.Data);
    //            //if (!_isOk) {
    //            //    //这边应该是跳转到错误页面
    //            //    han.message("数据解析错误，请联系管理员");
    //            //}
    //        }
    //        else {
    //            //这边应该需要跳转到错误页面
    //            han.message("数据请求错误，请联系管理员");
    //        }
    //    }
    //});

    //加载首页置顶帖子
    han.ajax({
        url: '/Home/GetIndexTopPost',
            success: function (data) {
                if (data.Success) {
                    ParseStickData(data.Data);
                    GetIndexPost();
                }
                else {
                    //这边应该需要跳转到错误页面
                    //han.message("数据请求错误，请联系管理员");
                }
            }
    });

    //加载首页综合帖子
    function GetIndexPost() {
        han.ajax({
            url: '/Home/GetIndexPost',
            data:{Curr:0,Limit:10},
            success: function (data) {
                if (data.Success) {
                    ParseComprehensiveData(data.Data);
                    GetAnswerRank();
                }
                else {
                    //这边应该需要跳转到错误页面
                    //han.message("数据请求错误，请联系管理员");
                }
            }
        });
    }
    
    //加载周榜
    function GetAnswerRank() {
        han.ajax({
            url: '/Home/GetAnswerRank',
            success: function (data) {
                if (data.Success) {
                    ParseAnswerRank(data.Data);
                    GetHotPoasts();
                }
                else {
                    //这边应该需要跳转到错误页面
                    //han.message("数据请求错误，请联系管理员");
                }
            }
        });
    }

    //加载热议
    function GetHotPoasts() {
        han.ajax({
            url: '/Home/GetHotPoasts',
            success: function (data) {
                if (data.Success) {
                    ParseHostPotData(data.Data);
                }
                else {
                    //这边应该需要跳转到错误页面
                    //han.message("数据请求错误，请联系管理员");
                }
            }
        });
    }

    //解析后台传过来的首页数据
    function ParseIndexData(data) {
        //渲染4条置顶内容
        ParseStickData(data.StickDatas)
        //渲染20条最新的综合帖子
        ParseComprehensiveData(data.ComprehensiveDatas);
        //渲染回帖周榜
        ParseAnswerRank(data.AnswerRanks);
        //渲染10条本周热议
        ParseHostPotData(data.HostPotDatas);
        //渲染用户登入信息
        //ParseUserLoginData(data.UserLoginDatas);
    }

    //渲染4条置顶内容
    function ParseStickData(data) {
        for (var i = 0; i < data.length; i++) {
            
            var type = "其他";
            switch (data[i].Type)
            {
                case 1:
                    type = "提问";
                    break;
                case 2:
                    type = "分享";
                    break;
                case 3:
                    type = "讨论";
                    break;
                case 4:
                    type = "建议";
                    break;
                case 5:
                    type = "公告";
                    break;
                case 6:
                    type = "动态";
                    break;
            }
            var hasEnd = "";
            if (data[i].HasEnd == 1 || data[i].HasEnd == 3) {
                hasEnd = "none";
            }
            var idGood = "";
            if (data[i].IsGood == 0) {
                idGood = "none";
            }
            han.tpl({
                data: { //数据
                    "Img": data[i].Img
                    , "Type": type
                    , "Title": data[i].Title
                    , "Author": data[i].Author
                    , "Grade": data[i].Grade
                    , "Time": data[i].Time
                    , "Award": data[i].Award
                    , "HasEnd": hasEnd
                    , "AnswerNumber": data[i].AnswerNumber
                    , "IsGood": idGood
                    , "AuthorId": data[i].AuthorId
                    , "TitleId": data[i].TitleId
                },
                view: TitleTpl.innerHTML,
                id: '#StickContainer'
            });
        }
    }

    //渲染20条最新的综合帖子
    function ParseComprehensiveData(data) {
        for (var i = 0; i < data.length; i++) {
            var type = "其他";
            switch (data[i].Type) {
                case 1:
                    type = "提问";
                    break;
                case 2:
                    type = "分享";
                    break;
                case 3:
                    type = "讨论";
                    break;
                case 4:
                    type = "建议";
                    break;
                case 5:
                    type = "公告";
                    break;
                case 6:
                    type = "动态";
                    break;
            }
            var hasEnd = "";
            if (data[i].HasEnd == 1 || data[i].HasEnd == 3) {
                hasEnd = "none";
            }
            var idGood = "";
            if (data[i].IsGood == 0) {
                idGood = "none";
            }
            han.tpl({
                data: { //数据
                    "Img": data[i].Img
                    , "Type": type
                    , "Title": data[i].Title
                    , "Author": data[i].Author
                    , "Grade": data[i].Grade
                    , "Time": data[i].Time
                    , "Award": data[i].Award
                    , "HasEnd": hasEnd
                    , "AnswerNumber": data[i].AnswerNumber
                    , "IsGood": idGood
                    , "AuthorId": data[i].AuthorId
                    , "TitleId": data[i].TitleId
                },
                view: TitleTpl.innerHTML,
                id: '#ComprehensiveContainer'
            });
        }
    }

    //渲染回帖周榜
    function ParseAnswerRank(data) {
        for (var i = 0; i < data.length; i++) {
            han.tpl({
                data: { //数据
                    "UserId": data[i].UserId
                    , "Img": data[i].Img
                    , "UserName": data[i].UserName
                    , "AnswerCount": data[i].AnswerCount
                },
                view: AnswerRankTpl.innerHTML,
                id: '#AnswerRank'
            });
        }
    }

    //渲染10条本周热议
    function ParseHostPotData(data) {
        for (var i = 0; i < data.length; i++) {
            han.tpl({
                data: { //数据
                    "TitleId": data[i].TitleId
                    , "TitleName": data[i].TitleName
                    , "AnswerCount": data[i].AnswerCount
                },
                view: HostPotTpl.innerHTML,
                id: '#HostPot'
            });
        }
    }

    //渲染用户登入信息
    function ParseUserLoginData(data) {
        common.loadUserInfo(data);
        ////登入状态
        //if (data.IsLogin) {
        //    //显示登入状态
        //    $('#Login').show();
        //    //隐藏非登入状态
        //    $('#NotLogin').hide();
        //    //设置用户昵称
        //    $('#UserName').val(data.UserName);
        //    //设置用户等级
        //    $('#UserLevel').val(data.UserLevel);
        //    //设置用户头像
        //    $('#UserImg').attr("src", data.UserImg)
        //    //显示我发表的帖子链接
        //    $('#MyPost').addClass("layui-show-md-inline-block");
        //    $('#MyPost').removeClass("layui-hidden-lg");
        //    $('#MyPost').removeClass("layui-hidden-md");
        //    //显示我收藏的帖子链接
        //    $('#MyFavoritePost').addClass("layui-show-md-inline-block");
        //    $('#MyFavoritePost').removeClass("layui-hidden-lg");
        //    $('#MyFavoritePost').removeClass("layui-hidden-md");
        //    //竖线
        //    $('#Line').addClass("layui-show-md-inline-block");
        //    $('#Line').removeClass("layui-hidden-lg");
        //    $('#Line').removeClass("layui-hidden-md");
        //}
        //else {
        //    //隐藏登入状态
        //    $('#Login').hide();
        //    //显示非登入状态
        //    $('#NotLogin').show();
        //    //隐藏我发表的帖子链接
        //    $('#MyPost').removeClass("layui-show-md-inline-block");
        //    $('#MyPost').addClass("layui-hidden-lg");
        //    $('#MyPost').addClass("layui-hidden-md");
        //    //隐藏我收藏的帖子链接
        //    $('#MyFavoritePost').removeClass("layui-show-md-inline-block");
        //    $('#MyFavoritePost').addClass("layui-hidden-lg");
        //    $('#MyFavoritePost').addClass("layui-hidden-md");
        //    //竖线
        //    $('#Line').removeClass("layui-show-md-inline-block");
        //    $('#Line').addClass("layui-hidden-lg");
        //    $('#Line').addClass("layui-hidden-md");
        //}
    }
})