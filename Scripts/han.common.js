
var common ={
    loadUserInfo: function (data) {
        //登入状态
        if (data.IsLogin) {
            ShowLoginState(data);
        }
        else {
            HidenLoginState();
        }
    }
}
function ShowLoginState(data) {
    //显示登入状态
    $('#Login').css('display', '');
    $('#NotLogin').css('display', '');
    //隐藏非登入状态
    $('#NotLogin').hide();
    //设置用户昵称
    $('#UserName').text(data.UserName);
    //设置用户等级
    $('#UserLevel').text(data.UserLevel);
    //设置用户头像
    $('#UserImg').attr("src", data.UserImg);
    //显示我发表的帖子链接
    $('#MyPost').addClass("layui-show-md-inline-block");
    $('#MyPost').removeClass("layui-hidden-lg");
    $('#MyPost').removeClass("layui-hidden-md");
    //显示我收藏的帖子链接
    $('#MyFavoritePost').addClass("layui-show-md-inline-block");
    $('#MyFavoritePost').removeClass("layui-hidden-lg");
    $('#MyFavoritePost').removeClass("layui-hidden-md");
    //竖线
    $('#Line').addClass("layui-show-md-inline-block");
    $('#Line').removeClass("layui-hidden-lg");
    $('#Line').removeClass("layui-hidden-md");
    //设置二级菜单跳转地址
    $('#user_Set').attr("href", '/User/Set?UserId=' + data.UserId);
    $('#user_Msg').attr("href", '/User/Message?UserId=' + data.UserId);
    $('#user_Home').attr("href", '/User/Home?UserId=' + data.UserId);
    $('#user_LogOut').attr("href", '/User/Exit/');
    //设置我发表和收藏帖子的跳转地址
    $('#main_PublishPost').attr("href", '/User/Index?UserId=' + data.UserId);
    $('#main_CollectPost').attr("href", '/User/Index?UserId=' + data.UserId);
}

function HidenLoginState() {
    //隐藏登入状态
    $('#Login').css('display', 'none');
    $('#NotLogin').css('display', 'block');
    //显示非登入状态
    $('#NotLogin').show();
    //隐藏我发表的帖子链接
    $('#MyPost').removeClass("layui-show-md-inline-block");
    $('#MyPost').addClass("layui-hidden-lg");
    $('#MyPost').addClass("layui-hidden-md");
    //隐藏我收藏的帖子链接
    $('#MyFavoritePost').removeClass("layui-show-md-inline-block");
    $('#MyFavoritePost').addClass("layui-hidden-lg");
    $('#MyFavoritePost').addClass("layui-hidden-md");
    //竖线
    $('#Line').removeClass("layui-show-md-inline-block");
    $('#Line').addClass("layui-hidden-lg");
    $('#Line').addClass("layui-hidden-md");
}