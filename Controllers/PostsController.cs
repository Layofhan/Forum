using Lay.Demo.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lay.Data.Services;
using Lay.Demo.Common;
using Lay.Interceptor;
using System.IO;
using Lay.Entity.Models;


namespace Lay.Controllers
{
    public class PostsController : Controller
    {
        public IPostsContract PostsContract { get; set; }
        public IUserOperateContract UserOperateContract { get; set; }
        // GET: Posts
        public ActionResult PostDetail(string TitleId)
        {
            //下面几行要取消掉
            if (TitleId == null)
            {
                return RedirectToAction("Index", "Home");///这边应该跳转到错误提示界面，或者首页
            }
            ViewBag.PostId = TitleId;
            PostsContract.UpWatchNum(TitleId);
            return View();
        }
        
        /// <summary>
        /// 返回发表文章界面
        /// </summary>
        /// <returns></returns>
        public ActionResult PostAdd()
        {
            IdentityTicket identity = IdentityManager.GetIdentFromAll();
            if (identity == null || identity.UserId == null)
            {
                return RedirectToAction("Login", "User");//这边应该跳转到登入界面
            }
            else
            {
                return View();
            }
        }
        /// <summary>
        /// 返回编辑帖子界面
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        public ActionResult PostEdit(string PostId)
        {
            ViewBag.PostId = PostId;
            return View();
        }
        /// <summary>
        /// 发表文章操作
        /// </summary>
        /// <returns></returns>
        public ActionResult PostAddC()
        {
            string title = Request["title"];
            int type = int.Parse(Request["class"].ToString());
            string content = Request["content"];
            int award = int.Parse(Request["experience"].ToString());
            IdentityTicket ident = IdentityManager.GetIdentFromAll();
            if (ident == null)
            {
                return RedirectToAction("Login", "User");//跳转至登入界面
            }
            //获取用户信息
            HANUserDetail userInfo = UserOperateContract.GetUserInfo(ident.UserId);

            HANActicleBaseInfos baseInfo = new HANActicleBaseInfos();
            baseInfo.Title = title;
            baseInfo.Time = System.DateTime.Now;
            baseInfo.Author = ident.UserName;
            baseInfo.AuthorId = ident.UserId;
            baseInfo.Img = userInfo.ImgUrl;
            baseInfo.Type = type;
            baseInfo.Grade = int.Parse(userInfo.Grade); 
            baseInfo.HasEnd = 1;
            baseInfo.IsGood = 0;
            baseInfo.AnswerNumber = 0;
            baseInfo.Award = award;
            baseInfo.Visitor = 0;
            baseInfo.HasAccept = true;
            baseInfo.IsTop = 2;

            //将信息插入文章明细表中
            PostsContract.PostAddDetail(baseInfo);
            //数据库表结构设计问题导致需要查询一遍他的自增长id
            DataResult dr = PostsContract.GetPostId(ident.UserId);
            if (dr.Success)
            {
                //将信息插入文章内容表中
                PostsContract.PostAddContent(dr.Data.ToString(), content);
                return DataProcess.Success(dr.Data).ToMvcJson();
            }
            else
            {
                if (PostsContract.DeleteNewsPost(ident.UserId).Success)
                {
                    return DataProcess.Failure("文章插入失败").ToMvcJson();
                }
                else
                {
                    return DataProcess.Failure("系统出现问题，请联系管理员").ToMvcJson();
                }
            }
        }

        /// <summary>
        /// 根据文章ID获取文章作者信息等
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        public ActionResult GetPostDetailById(string PostId)
        {
            return PostsContract.GetPostDetailById(PostId).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }
        
        /// <summary>
        /// 写入文章的回帖
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        public ActionResult WritePoastReply()
        {
            try
            {
                IdentityTicket user = IdentityManager.GetIdentFromAll();
                if (user == null || user.IsLife == false)
                {
                    //这边应该做一个跳转操作的 -- 待写
                    //return RedirectToAction("Login", "User");
                    return DataProcess.Failure("先登入吧~").ToMvcJson();
                }
                string content = Request.Form["content"];
                string pid = Request.Form["pid"];

                //如果存在@则代表为二级回复
                if (content.Contains('@'))
                {
                    //pid = pid.Split('@')[1];//由于之前做二级回帖用到
                    return PostsContract.WritePoastReplyClone(user.UserId, pid, content).ToMvcJson("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    //PostsContract.WritePoastReply(user.UserId, PostId, content);
                    return PostsContract.WritePoastReply(user.UserId, pid, content).ToMvcJson("yyyy-MM-dd HH:mm:ss");
                }
                
            }
            catch(Exception ex)
            {
                return DataProcess.Failure("写回帖出错了").ToMvcJson();
            }
        }
        /// <summary>
        /// 获取文章内容
        /// </summary>
        /// <param name="PostId">文章ID</param>
        /// <returns></returns>
        public ActionResult GetPostContent(string PostId)
        {
            try
            {
                return PostsContract.GetPostContent(PostId).ToMvcJson();
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }
        /// <summary>
        /// 文本编辑器图片上传
        /// 未完成 不能保存图片到文件夹 因为本地文件没有权限是本机的用户角色问题 -- 解决
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadPic()
        {
            HttpPostedFileBase file = Request.Files["file"];
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "PostImg";//存放文档图片的路径
            string filename = Common.CreateImageFileName() + ".jpg";
            string filepathclone = Path.Combine(filepath, Path.GetFileName(filename));
            if (!System.IO.File.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            file.SaveAs(filepathclone);
            return DataProcess.Success("../PostImg/" + filename).ToMvcJson();
        }

        //获取回帖
        public ActionResult GetPostReturnCard(string PostId,int Curr,int Limit)
        {
            string userIdCurr = string.Empty;
            IdentityTicket identity = IdentityManager.GetIdentFromAll();
            if (identity == null)
            {
                userIdCurr = "";
            }
            else
            {
                userIdCurr = identity.UserId;
            }
            return PostsContract.GetPostReturnCard(PostId, userIdCurr, Curr, Limit).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }

        //点赞
        public ActionResult Zan()
        {
            string ok = Request.Form["ok"];
            string id = Request.Form["id"];
            IdentityTicket user = IdentityManager.GetIdentFromAll();
            if (user == null || user.IsLife == false)
            {
                //这边应该做一个跳转操作的 -- 待写
                //return RedirectToAction("Login", "User");
                return DataProcess.Failure("请先登入").ToMvcJson();
            }
            {
                return PostsContract.ModifyZanState(ok, id, user.UserId).ToMvcJson();
            }
        }

        //采纳
        public ActionResult Accept()
        {
            string id = Request.Form["id"];
            IdentityTicket user = IdentityManager.GetIdentFromAll();
            if (user == null || user.IsLife == false)
            {
                //return RedirectToAction("Login", "User");
                return DataProcess.Failure("请先登入").ToMvcJson();
            }
            return PostsContract.Accept(id, user.UserId).ToMvcJson();
        }

        //删除
        public ActionResult Delete()
        {
            if (IdentityManager.GetIdentFromAll() == null)
            {
                return DataProcess.Failure("你还没有登入呢").ToMvcJson();
            }
            string id = Request.Form["id"];
            return PostsContract.Delete(id).ToMvcJson();
        }

        //编辑回帖
        public ActionResult Edit()
        {
            if (IdentityManager.GetIdentFromAll() == null)
            {
                return DataProcess.Failure("你还没有登入呢").ToMvcJson();
            }
            //回帖ID
            string id = Request.Form["id"];
            return PostsContract.Edit(id).ToMvcJson();
        }
        //编辑--更新内容
        public ActionResult UpdateReply()
        {
            if (IdentityManager.GetIdentFromAll() == null)
            {
                return DataProcess.Failure("你还没有登入呢").ToMvcJson();
            }
            string id = Request.Form["id"];
            string content = Request.Form["content"];
            return PostsContract.UpdataReply(id,content).ToMvcJson();
        }

        //编辑帖子内容
        public ActionResult PostContentEdit(string PostId)
        {
            try
            {
                IdentityTicket identity = IdentityManager.GetIdentFromAll();
                if (identity == null)
                {
                    return DataProcess.Failure("你还没有登入呢").ToMvcJson();
                }
                else
                {
                    string contnt = Request.Form["content"];
                    return PostsContract.EditPostContent(identity.UserId, PostId, contnt).ToMvcJson();
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }

        //获取文章的回帖总数
        public ActionResult GetPostReplyCount(string PostId)
        {
            return PostsContract.GetPostReplyCount(PostId).ToMvcJson();
        }

        //获取当前用户对当前帖子的收藏状态
        public ActionResult GetTitleState(string PostId)
        {
            try
            {
                if (PostId == "" || PostId == null)
                {
                    return DataProcess.Failure().ToMvcJson();
                }
                IdentityTicket identtity = IdentityManager.GetIdentFromAll();
                if (identtity != null)
                {
                    if (PostsContract.JudgePostByUserId(identtity.UserId, PostId).Success)
                    {
                        return PostsContract.GetCollectState(identtity.UserId, PostId).ToMvcJson();
                    }
                    else
                    {
                        return DataProcess.Failure().ToMvcJson();
                    }
                }
                else
                {
                    return DataProcess.Failure().ToMvcJson();
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }

        //收藏帖子
        public ActionResult InsertCollectRela(string PostId)
        {
            try
            {
                IdentityTicket identity = IdentityManager.GetIdentFromAll();
                if (identity == null)
                {
                    return DataProcess.Failure().ToMvcJson();
                }
                return PostsContract.InsertCollectRela(identity.UserId, PostId).ToMvcJson();
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }

        //取消收藏
        public ActionResult DeleteCollectRela(string UserId, string PostId)
        {
            try
            {
                IdentityTicket identity = IdentityManager.GetIdentFromAll();
                if (identity == null)
                {
                    return DataProcess.Failure().ToMvcJson();
                }
                return PostsContract.DeleteCollectRela(identity.UserId, PostId).ToMvcJson();
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }

        //获取收藏的帖子
        public ActionResult GetCollectByUserId()
        {
            try
            {
                IdentityTicket identity = IdentityManager.GetIdentFromAll();
                if(identity == null){
                    return DataProcess.Failure("没有登入").ToMvcJson();
                }
                else{
                    return PostsContract.GetCollectByUserId(identity.UserId).ToMvcJson("yyyy-MM-dd HH:mm:ss"); ;
                }
            }
            catch(Exception ex){
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }
        //获取发表的帖子
        public ActionResult GetPostByUserId()
        {
            try
            {
                IdentityTicket identity = IdentityManager.GetIdentFromAll();
                if (identity == null)
                {
                    return DataProcess.Failure("没有登入").ToMvcJson();
                }
                else
                {
                    return PostsContract.GetPostsByUserId(identity.UserId).ToMvcJson("yyyy-MM-dd HH:mm:ss"); ;
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }
        #region 测试使用函数
        
        #endregion
    }
}