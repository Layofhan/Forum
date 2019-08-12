using Lay.Demo.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lay.Data.Services;
using Lay.Entity.Models;
using Lay.Demo.Common;
using System.IO;

namespace Lay.Controllers
{
    public class UserController : Controller
    {
        
        public  Object ob = new object();

        public IUserOperateContract UserOperateContract { get; set; }

        //找回密码界面
        public ActionResult Forget()
        {
            return View();
        }

        //个人主页界面
        public ActionResult Home(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        //用户中心--我的帖子、收藏的帖子界面
        public ActionResult Index(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        //登入界面
        public ActionResult Login()
        {
            return View();
        }

        //注册界面
        public ActionResult Reg()
        {
            return View();
        }

        //消息界面
        public ActionResult Message(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        //基本设置界面
        public ActionResult Set(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        //注册成功提示界面
        public ActionResult Notice(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        /// <summary>
        /// 获取最近的提问
        /// </summary>
        /// <param name="UserId">用户名</param>
        /// <returns></returns>
        public ActionResult GetQuizByTimeandUser(string UserId)
        {
            if (UserId != null || UserId != "")
            {
                return UserOperateContract.GetQuizByTimeandUser(UserId, 20).ToMvcJson("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return DataProcess.Failure("用户ID为空").ToMvcJson();
            }
        }

        /// <summary>
        /// 获取最近的回答内容
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult GetAnswerByUser(string UserId)
        {
            if (UserId != null || UserId != "")
            {
                return UserOperateContract.GetAnswerByUser(UserId, 20).ToMvcJson("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return DataProcess.Failure("用户ID为空").ToMvcJson();
            }
        }

        /// <summary>
        /// 获取用户个人信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult GetUserInfo(string UserId)
        {
            /*IdentityTicket identity = IdentityManager.GetIdentFromAll();
            if (identity == null || identity.UserId == null)
            {
                return DataProcess.Failure("用户未登入").ToMvcJson();
            }*/
            HANUserDetail userDetail = UserOperateContract.GetUserInfo(UserId);
            if (userDetail != null)
            {
                return DataProcess.Success(userDetail).ToMvcJson("yyyy-MM-dd");
            }
            else
            {
                return DataProcess.Failure("不存在该ID记录").ToMvcJson();
            }
        }

        /// <summary>
        /// 登入操作
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginControll()
        {
            try
            {
                string userId = Request["userId"].Trim();
                string passWord = Request["passWord"].Trim();
                if (userId == null || userId == "" || passWord == null || passWord == "")
                {
                    return DataProcess.Failure("账号密码不能为空").ToMvcJson();
                }
                var result = UserOperateContract.Login(userId, passWord);
                if (result.Success)
                {
                    HANUserDetail userDetail = (HANUserDetail)result.Data;
                    if (!userDetail.IsLife)
                    {
                        return DataProcess.Failure("该账号被查封了").ToMvcJson();
                    }
                    IdentityTicket ident = new IdentityTicket();
                    ident.UserId = userId;
                    ident.UserName = userDetail.Name;
                    if(userDetail.IsAdmin){
                        ident.Role = 1;
                    }
                    else
                    {
                        ident.Role = 0;
                    }
                    ident.IsLife = true;
                    ident.IsForceOffline = true;
                    //将信息进行保存
                    IdentityManager.Init(ident,true);

                    lock (ob)
                    {
                        IdentityManager.Identitys.Add(ident.UserId,ident);//记录当前在线用户信息
                    }
                    return DataProcess.Success("登入成功").ToMvcJson();
                }
                else
                {
                    return DataProcess.Failure(result.Message).ToMvcJson();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("已添加了具有相同键的项"))
                {
                    return DataProcess.Failure("登过了刷新下就好了").ToMvcJson();
                }
                return DataProcess.Failure("出错啦,快联系管理员").ToMvcJson();
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassWord()
        {
            try
            {
                string userId = Request["userId"].Trim();
                string oldPassword = Request["oldPassword"].Trim();
                string newPassword = Request["newPassword"].Trim();

                IdentityTicket ident = IdentityManager.GetIdentFromSession();
                if (ident == null || ident.UserId == null)
                {
                    return DataProcess.Failure("传入的用户名存在问题，请联系管理员").ToMvcJson();
                }
                lock (ob)
                {
                    if (!IdentityManager.Identitys.ContainsKey(ident.UserId))
                    {
                        return DataProcess.Failure("登入用户中没有传入用户").ToMvcJson();
                    }
                }
                if (userId == null || oldPassword == null || newPassword == null)
                {
                    return DataProcess.Failure("传入参数不能为空").ToMvcJson();
                }
                else
                {
                    return UserOperateContract.ChangePassWord(ident.UserId, oldPassword, newPassword).ToMvcJson();
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure("系统问题，请联系管理员").ToMvcJson();
            }
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult ModifyUserInfo()
        {
            try
            {
                string UserId = Request["UserId"];
                IdentityTicket ident = IdentityManager.GetIdentFromSession();
                if (ident == null || ident.UserId == null)
                {
                    return DataProcess.Failure("传入的用户名存在问题，请联系管理员").ToMvcJson();
                }
                lock (ob)
                {
                    if (!IdentityManager.Identitys.ContainsKey(ident.UserId))
                    {
                        return DataProcess.Failure("登入用户中没有传入用户").ToMvcJson();
                    }
                }
                string userName = Request["username"];
                string City = Request["city"];
                string Sigh = Request["Sigh"];
                int Sex = int.Parse(Request["Sex"])+1;
                if (userName == null || userName == "")
                {
                    return DataProcess.Failure("用户名不能为空").ToMvcJson();
                }
                //这边调用更新信息接口
                return UserOperateContract.ModifyUserInfo(ident.UserId, Sigh, City, userName, Sex).ToMvcJson();
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }

        /// <summary>
        /// 退出登入
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit()
        {
            try
            {
                IdentityTicket identity = IdentityManager.GetIdentFromAll();
                if (identity == null)
                {
                    return DataProcess.Failure("你在乱搞事 要给你逮起来了").ToMvcJson();
                }
                lock (ob)
                {
                    IdentityManager.Identitys.Remove(identity.UserId);
                }
                if (IdentityManager.ClearIndetForAll())
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return DataProcess.Failure("你退不出去了 没得咯。快叫网管吧").ToMvcJson();
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }

        /// <summary>
        /// 判断用户当前是否登入
        /// </summary>
        /// <returns></returns>
        public ActionResult HasLogin()
        {
            try
            {
                if (IdentityManager.GetIdentFromAll() != null)
                {
                    return DataProcess.Success().ToMvcJson();
                }
                else
                {
                    return DataProcess.Failure().ToMvcJson();
                }
            }
            catch (Exception ex)
            {
                return DataProcess.Failure().ToMvcJson();
            }
        }

        /// <summary>
        /// 用户头像上传
        /// 未完成 不能保存图片到文件夹 因为本地文件没有权限是本机的用户角色问题
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadPic()
        {
            IdentityTicket identity = IdentityManager.GetIdentFromAll();
            if (identity == null)
            {
                return DataProcess.Failure("请先登入").ToMvcJson();
            }
            HttpPostedFileBase file = Request.Files["file"];
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "HeadImage";//存放文档图片的路径
            string filename = Common.CreateImageFileName() + ".jpg";
            string filepathclone = Path.Combine(filepath, Path.GetFileName(filename));
            if (!System.IO.File.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            file.SaveAs(filepathclone);
            //保存路径到数据库
            DataResult dp = UserOperateContract.SaveImage(identity.UserId, "../HeadImage/" + filename);
            if (dp.Success)
            {
                return DataProcess.Success("../HeadImage/" + filename).ToMvcJson();
            }
            else
            {
                return dp.ToMvcJson();
            }
        }

        /// <summary>
        /// 注册界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Regedit()
        {
            try
            {
                string email = Request["email"];
                string userName = Request["username"];
                string pass = Request["pass"];
                return UserOperateContract.Register(email,userName,pass).ToMvcJson();
            }
            catch (Exception ex)
            {
                return DataProcess.Failure(ex.Message).ToMvcJson();
            }
        }
    }
}