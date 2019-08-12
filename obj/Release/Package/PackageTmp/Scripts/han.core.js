/*
该文件用于前端框架的一些封装函数
*/
var han = {
    //模板引擎
    //传入参数：view-模板内容templateid.innerHTML，data-模板绑定数据，id-容器ID
    tpl: function (param) {
        layui.use('laytpl', function () {
            var laytpl = layui.laytpl;
            //模板渲染
            laytpl(param.view).render(param.data, function (html) {
                $(param.id).append(html);
            });
        });
    },

    //消息弹层
    //传入参数：content-弹框显示内容
    message: function (content) {
        layui.use('layer', function () {
            var layer = layui.layer;
            if (content == null) {
                content = "Han，你好像忘了点什么<i class='layui-icon'>&#xe664;</i> ";
            }
            layer.msg(content);
        });   
    },
    //对css样式重新渲染
    render:function(){
        layui.use(['element', 'form'], function ()
        {
            var element = layui.element;
            var form = layui.form;
            element.init();
            form.render();
        });
    },

    //富文本编辑器
    edit: function (param) {
        var defaultOptions = {
            id: 'edit',
            tool:[  'strong' //加粗
                   ,'italic' //斜体
                   ,'underline' //下划线
                   ,'del' //删除线  
                   ,'|' //分割线
                   ,'left' //左对齐
                   ,'center' //居中对齐
                   ,'right' //右对齐
                   ,'link' //超链接
                   ,'unlink' //清除链接
                   ,'face' //表情
                   ,'image' //插入图片
                   , 'help' //帮助
            ],
            height: 180,
            url:'',//待写
            type:'post'
        };
        //对于重复的字段，后面选项会覆盖前者
        var option = $.extend({}, defaultOptions, param);
        layedit.set({
            uploadImage: {
                url: option.url //接口url
              , type: option.type //默认post
            }
        });
        layui.use('layedit', function () {
            var layedit = layui.layedit;
            layedit.build(option.id,option.tool,option.height); //建立编辑器
        });
    },

    //分页组件
    page:function(param){
        var defaultOptions = {
            elem: "han_page"
           , limit: 10
           , groups: 5
           , prev: "耿耿"
           , next: "余淮"
        };
        //对于重复的字段，后面选项会覆盖前者
        var option = $.extend({}, defaultOptions, param);

        layui.use(['laypage', 'layer'], function () {
            var laypage = layui.laypage
            , layer = layui.layer;
            laypage.render({
                elem: option.elem
              , count: option.count //数据总数
              , limit: option.limit
              , groups: option.groups
              , jump: function (obj, first) {
                  //obj包含了当前分页的所有参数，比如：
                  console.log(obj.curr); //得到当前页，以便向服务端请求对应页的数据。
                  console.log(obj.limit); //得到每页显示的条数
                  
                  //首次不执行
                  if (!first) {
                      //do something
                      console.log(first);
                  }
              }
            });
        });
    },

    //ajax请求
    //传入参数：url-请求路径，async-是否异步默认true异步，data-路径参数，dataType-返回数据格式默认json，beforesend-请求前触发的事件，success-请求成功触发事件
    ajax: function (param) {
        var defaultOptions = {
            url: null,
            async: true,
            data: null,
            dataType: "json",
            beforeSend:null,
            success: null,
            type:"get"
        };
        //对于重复的字段，后面选项会覆盖前者
        var option = $.extend({}, defaultOptions, param);

        $.ajax({
            url: option.url,
            type:option.type,
            async:option.async,
            data: option.data,
            dataType: option.dataType,
            beforeSend:option.beforeSend,
            success: option.success
        });     
    },

    //加载层
    waiting: function (param) {
        var defaultOptions = {
            type: 2,
            shade: 0,
            shadeClose:false
        };
        //对于重复的字段，后面选项会覆盖前者
        var option = $.extend({}, defaultOptions, param);

        layui.use('layer', function () {
            layer.load(option.type, { shade: option.shade, shadeClose: option.shadeClose });
        });
    },

    //关闭弹层
    closewaiting: function (index) {
        switch (index) {
            case 0:
                layer.closeAll(); //疯狂模式，关闭所有层
                break;
            case 1:
                layer.closeAll('dialog'); //关闭信息框
                break;
            case 2:
                layer.closeAll('page'); //关闭所有页面层
                break;
            case 3:
                layer.closeAll('iframe'); //关闭所有的iframe层
                break;
            case 4:
                layer.closeAll('loading'); //关闭加载层
                break;
            case 5:
                layer.closeAll('tips'); //关闭所有的tips层    
                break;
            default:
                layer.closeAll(); //疯狂模式，关闭所有层
        }
    }
}

