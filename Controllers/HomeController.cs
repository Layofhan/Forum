using Lay.Data.Services;
using Lay.Demo.Common;
using Lay.Demo.Contracts;
using Lay.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Lay.Controllers
{
    public class HomeController : Controller
    {
        public ItestContract testContract { get; set; }

        public IIndexContract IndexContract { get; set; }

        public IUserOperateContract UserOperateContract { get; set; }
        private IdentityTicket identity = new IdentityTicket();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Column(int Types,int State,int Mode)
        {
            ViewBag.PostType = Types;
            ViewBag.State = State;
            ViewBag.Mode = Mode;
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        [AuthorizationFilter(ActionName ="Hello")]

        /// <summary>
        /// 获取首页加载需要的信息 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIndexData()
        {
            identity = IdentityManager.GetIdentFromAll();
            if (identity.UserId == null)
            {
                return DataProcess.Failure().ToMvcJson();
            }
            return IndexContract.GetIndexInfo(identity.UserId).ToMvcJson("yyyy-MM-dd HH:mm:ss");
            //return IndexContract.GetIndexInfo(identity.UserId).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取置顶帖子
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIndexTopPost()
        {
            return IndexContract.GetIndexTopPost().ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取综合帖子
        /// </summary>
        /// <returns></returns>
        public ActionResult GetIndexPost(int Curr, int Limit, int Type = 0)
        {
            return IndexContract.GetIndexPost(Curr, Limit, Type).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 获取回帖周榜
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAnswerRank()
        {
            return IndexContract.GetAnswerRank().ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取用户登入信息 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserLoginInfo()
        {
            identity = IdentityManager.GetIdentFromAll();
            if (identity == null || identity.UserId == null)
            {
                return DataProcess.Failure("未登入").ToMvcJson();
            }
            return UserOperateContract.GetUserLoginInfo(identity.UserId).ToMvcJson();
        }

        /// <summary>
        /// 获取热议文章
        /// </summary>
        /// <returns></returns>
        public ActionResult GetHotPoasts()
        {
            return IndexContract.GetHotPoasts().ToMvcJson();
        }

        //加载未结帖子 
        //Type 0-按时间排序，1-按评论数排序 默认为0
        public ActionResult GetGetReady(int Curr,int Limit,int Type = 0)
        {
            return IndexContract.GetGetReady(Curr, Limit, Type).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }
        //加载已结帖子
        public ActionResult GetGetHasEnd(int Curr, int Limit, int Type = 0)
        {
            return IndexContract.GetGetHasEnd(Curr, Limit, Type).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }
        //加载精华帖子
        public ActionResult GetGetEssence(int Curr, int Limit, int Type = 0)
        {
            return IndexContract.GetGetEssence(Curr, Limit, Type).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }

        //获取不同种类、不同状态、不同排序模式的帖子
        public ActionResult GetPostBySyn(int PostType, int PostState, int SearchMode, int Curr, int Limit)
        {
            Curr = Curr - 1;
            return IndexContract.GetPostBySyn(PostType, PostState, SearchMode, Curr, Limit).ToMvcJson("yyyy-MM-dd HH:mm:ss");
        }

        //获取对应类型帖子的数量
        //posttype 0-综合 1-提问 2-分享 3-讨论 4-建议 5-公告 6-动态
        //poststate 0-综合 1-未结 2-已结 3-精华
        public int GetCountByType(int PostType, int PostState)
        {
            DataResult dp = IndexContract.GetCountByType(PostType, PostState);
            if (dp.Success)
            {
                return (int)dp.Data;
            }
            else
            {
                return 0;
            }
        }
    }
}