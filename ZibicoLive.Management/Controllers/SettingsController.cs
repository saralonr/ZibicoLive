using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZibicoLive.Entity;
using ZibicoLive.Entity.Models;
using ZibicoLive.Management.Auth;

namespace ZibicoLive.Management.Controllers
{
    [Auth]
    public class SettingsController : Controller
    {
        BaseRepository db = new BaseRepository();
        private JsonResponse rs = new JsonResponse();
        #region PAGE
        public ActionResult List()
        {
            List<GeneralSettingPOCO> list = new List<GeneralSettingPOCO>();
            list = db.GetList<GeneralSettingPOCO>();

            return View(list);
        }
        public ActionResult Create(int? ID)
        {
            if (ID == null) return View();
            else
            {
                GeneralSettingPOCO old = db.GetById<GeneralSettingPOCO>((int)ID);
                if (old == null)
                {
                    ViewBag.ErrorMessage = "Ayar bulunamadı.";
                    return View();
                }
                return View(old);
            }
        }
        #endregion
        #region GET

        #endregion
        #region POST  
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(GeneralSettingPOCO set, int? ID)
        {
            if (string.IsNullOrWhiteSpace(set.DisplayName))
            {
                ViewBag.ErrorMessage = "Görüntülenen ad zorunludur.";
                return View();
            }
            if (string.IsNullOrWhiteSpace(set.SettingKey))
            {
                ViewBag.ErrorMessage = "Key zorunludur.";
                return View();
            }
            if (string.IsNullOrWhiteSpace(set.SettingValue))
            {
                ViewBag.ErrorMessage = "Değer zorunludur.";
                return View();
            }
            if (string.IsNullOrWhiteSpace(set.Description))
            {
                ViewBag.ErrorMessage = "Açıklama zorunludur.";
                return View();
            }


            if (ID == null)
            {
                var setid = db.Insert(set);

                TempData["SuccessMessage"] = "Yeni ayar başarıyla oluşturuldu.";
                return Redirect("/Settings/List");
            }
            else
            {
                GeneralSettingPOCO old = db.GetById<GeneralSettingPOCO>((int)ID);
                if (old == null)
                {
                    ViewBag.ErrorMessage = "Ayar bulunamadı.";
                    return View();
                }
                old.Description = set.Description;
                old.DisplayName = set.DisplayName;
                old.SettingKey = set.SettingKey;
                old.SettingValue = set.SettingValue;

                db.Update(old);

                TempData["SuccessMessage"] = "Ayar başarıyla güncellendi.";
                return Redirect("/Settings/List");
            }
        }
        [HttpPost]
        public ActionResult Delete(int ID)
        {
            GeneralSettingPOCO old = db.GetById<GeneralSettingPOCO>((int)ID);
            if (old == null)
            {
                rs.Message = "Kayıt bulunamadı.";
                rs.Status = false;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }


            old.Status = 0;
            var result = db.Update(old);
            if (result > 0)
            {
                rs.Message = "Kayıt başarıyla silindi.";
                rs.Status = true;

                return Json(rs, JsonRequestBehavior.AllowGet);
            }
            else
            {
                rs.Message = "Kayıt silinirken bir hata oluştu. Lütfen tekrar deneyiniz. Hatanın devam etmesi halinde sistem yöneticisine başvurunuz.";
                rs.Status = false;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}