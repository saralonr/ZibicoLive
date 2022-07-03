using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZibicoLive.Entity;
using ZibicoLive.Entity.DTO;
using ZibicoLive.Entity.Models;
using ZibicoLive.Management.Auth;

namespace ZibicoLive.Management.Controllers
{
    [Auth]
    public class StepController : Controller
    {
        BaseRepository db = new BaseRepository();
        private JsonResponse rs = new JsonResponse();
        #region PAGE
        public ActionResult List(long? page = 1)
        {
            using (db)
            {
                var list = new Page<StepPOCO>();
                var stepIdList = new List<long>();
                list = db.PageList<StepPOCO>((long)page, 30, "Select * from Steps WHERE WebSiteId=@wid ORDER BY StepNumber ASC", new { wid = UserSession.Info.WebSiteID });
                list.Items.ForEach(x => stepIdList.Add(x.ID));

                if (stepIdList.Count > 0)
                {
                    ViewBag.StepOptions = db.GetList<StepOptionPOCO>("Status=1 AND StepID IN(@list)", new { list = stepIdList }).ToList();
                }

                return View(list);
            }

        }
        public ActionResult Create(int? ID)
        {
            ViewBag.Steps = db.GetList<StepPOCO>("WebSiteId=@wid ORDER BY StepNumber ASC", new { wid = UserSession.Info.WebSiteID });
            if (ID == null) return View();
            else
            {
                using (db)
                {
                    StepPOCO old = db.GetById<StepPOCO>((int)ID);
                    if (old == null)
                    {
                        ViewBag.ErrorMessage = "Kayıt bulunamadı.";
                        return View();
                    }
                    ViewBag.StepOptions = db.GetList<StepOptionPOCO>("Status=1 AND StepID=@id", new { id = ID }).ToList();
                    return View(old);
                }


            }
        }
        #endregion
        #region GET

        #endregion
        #region POST  
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(StepPOCO step, int? ID, string[] NewStepOption,long[] NewStepOptionNextStep, long[] StepOptionID,string[] StepOption,long[] StepOptionNextStep)
        {
            ViewBag.Steps = db.GetList<StepPOCO>("WebSiteId=@wid ORDER BY StepNumber ASC", new { wid = UserSession.Info.WebSiteID });

            if (string.IsNullOrWhiteSpace(step.Question))
            {
                ViewBag.ErrorMessage = "Akış mesajı zorunludur.";
                return View();
            }
            if (Request.Form.AllKeys.Any(x => x.StartsWith("StepOption")) == false && step.IsFinished == false)
            {
                ViewBag.ErrorMessage = "Akış mesajı akış sonu değilse seçenek içermek zorundadır.";
                return View();
            }

            if (ID == null)
            {
                step.Status = 1;
                step.CreatedDate = DateTime.Now;
                step.UniqueId = Guid.NewGuid();
                step.WebSiteId = UserSession.Info.WebSiteID;

                using (db)
                {
                    var setid = db.Insert(step);

                    if (step.IsFinished==false)
                    {
                        int index = 0;
                        foreach (string item in NewStepOption)
                        {
                            var stepOption = item;
                            StepOptionPOCO option = new StepOptionPOCO();
                            option.CreatedDate = DateTime.Now;
                            option.OptionDescription = stepOption;
                            option.StepID = (long)setid;
                            option.UniqueId = Guid.NewGuid();
                            option.WebSiteId = UserSession.Info.WebSiteID;
                            option.NextStepID = Convert.ToInt64(NewStepOptionNextStep[index]);
                            db.Insert(option);
                            index++;
                        }
                    }
                }

                TempData["SuccessMessage"] = "Yeni ayar başarıyla oluşturuldu.";
                return Redirect("/Step/List");
            }
            else
            {
                StepPOCO old = db.GetById<StepPOCO>((int)ID);
                if (old == null)
                {
                    ViewBag.ErrorMessage = "Kayıt bulunamadı.";
                    return View();
                }
                old.IsFinished = step.IsFinished;
                old.Question = step.Question;
                old.StepNumber = step.StepNumber;

                db.Update(old);

                if (old.IsFinished==false)
                {
                    var stepOptions = db.GetList<StepOptionPOCO>("Status=1 AND StepID=@id", new { id = ID }).ToList();
                    var deletedStepOptions = new List<long>();
                    foreach (StepOptionPOCO optionItem in stepOptions)
                    {
                        if (!StepOptionID.Any(x => x == optionItem.ID))
                        {
                            deletedStepOptions.Add(optionItem.ID);
                        }
                        else
                        {
                            int order = Array.IndexOf(StepOptionID, optionItem.ID);
                            optionItem.NextStepID = StepOptionNextStep[order];
                            optionItem.OptionDescription = StepOption[order];
                            db.Update(optionItem);
                        }
                    }
                    if (deletedStepOptions.Count > 0)
                    {
                        db.Execute("UPDATE StepOptions SET Status=0 WHERE ID IN(@list)", new { list = deletedStepOptions });
                    }

                    int index = 0;
                    foreach (string item in NewStepOption)
                    {
                        var stepOption = item;
                        StepOptionPOCO option = new StepOptionPOCO();
                        option.CreatedDate = DateTime.Now;
                        option.OptionDescription = stepOption;
                        option.StepID = (long)ID;
                        option.UniqueId = Guid.NewGuid();
                        option.WebSiteId = UserSession.Info.WebSiteID;
                        option.NextStepID = Convert.ToInt64(NewStepOptionNextStep[index]);
                        option.Status = 1;
                        db.Insert(option);
                        index++;
                    }
                }
                else
                {
                    db.Execute("UPDATE StepOptions SET Status=0 WHERE StepID=@id", new { id = ID });
                }
                
                TempData["SuccessMessage"] = "Akış başarıyla güncellendi.";
                return Redirect("/Step/List");
            }
        }
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            StepPOCO old = db.GetById<StepPOCO>((int)ID);
            if (old == null)
            {
                rs.Message = "Kayıt bulunamadı.";
                rs.Status = false;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }

            using (db)
            {
                old.Status = 0;
                var result = db.Update(old);
                if (result > 0)
                {
                    rs.Message = "Kayıt başarıyla silindi.";
                    rs.Status = true;

                    List<StepOptionPOCO> options = db.GetList<StepOptionPOCO>("StepID=@id", new { id = ID });
                    List<long> valueList = new List<long>();
                    options.ForEach(x => valueList.Add(x.ID));
                    db.Execute("UPDATE StepOptions SET Status=0 WHERE ID IN(@list)", new { list = valueList });

                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    rs.Message = "Kayıt silinirken bir hata oluştu. Lütfen tekrar deneyiniz. Hatanın devam etmesi halinde sistem yöneticisine başvurunuz.";
                    rs.Status = false;
                    return Json(rs, JsonRequestBehavior.AllowGet);
                }
            }


        }
        #endregion
    }
}