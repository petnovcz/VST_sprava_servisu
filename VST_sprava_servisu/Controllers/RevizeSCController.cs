using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class RevizeSCController : Controller
    {
        private Model1Container db = new Model1Container();
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: RevizeSC
        public ActionResult Index(int id)
        {
            var revizeSC = db.RevizeSC.Include(r => r.Revize).Include(r => r.SCProvozu).Include(r => r.Umisteni).Where(r=>r.RevizeId ==id)
                .OrderBy(r=>r.SCProvozu.SerioveCislo.Artikl.SkupinaArtiklu1.PoradiZobrazeni)
                .ThenBy(r=>r.SCProvozu.Znaceni);
            return View(revizeSC.ToList());
        }

        // GET: RevizeSC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            if (revizeSC == null)
            {
                return HttpNotFound();
            }
            return View(revizeSC);
        }

        // GET: RevizeSC/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create(int RevizeId)
        {
            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav");
            int ProvozId = db.Revize.Where(t => t.Id == RevizeId).Select(t => t.ProvozId).FirstOrDefault();
            int? UmisteniId = db.Revize.Where(t => t.Id == RevizeId).Select(t => t.UmisteniId).FirstOrDefault();
            var SCProvozuIDlist = db.RevizeSC.Where(t => t.RevizeId == RevizeId).Select(t=>t.SCProvozuId).ToList(); 
            if (UmisteniId == null)
            { ViewBag.SCProvozuId = 
                    new SelectList(
                        db.SCProvozu
                            .Include(t => t.SerioveCislo)
                            .Where(t=>t.ProvozId == ProvozId)
                            .Where(t=> !SCProvozuIDlist.Contains(t.Id))
                            .Select(s => new
                            {
                                s.Id,
                                SerioveCislo = s.SerioveCislo.SerioveCislo1
                            })
                            , "Id", "SerioveCislo"); }
            else
            { ViewBag.SCProvozuId = 
                    new SelectList(
                        db.SCProvozu
                            .Include(t=>t.SerioveCislo)
                            .Where(t => t.ProvozId == ProvozId)
                            .Where(t=>t.Umisteni == UmisteniId)
                            .Where(t => !SCProvozuIDlist.Contains(t.Id))
                            .Select(s => new
                                {
                                    s.Id,
                                    SerioveCislo = s.SerioveCislo.SerioveCislo1
                                })
                                , "Id", "SerioveCislo"); }
           
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni");
            ViewBag.RevizeId = RevizeId;

            return View();
        }

        // POST: RevizeSC/Create
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create([Bind(Include = "Id,RevizeId,SCProvozuId,StavKoroze,StavZnecisteni,JineZavady,UmisteniId,Baterie,Pyro,TlakovaZkouska")] RevizeSC revizeSC)
        {
            if (ModelState.IsValid)
            {
                int RevizeId = 0;
                try
                {
                    SCProvozu scprovozu = new SCProvozu();
                    scprovozu = db.SCProvozu.Find(revizeSC.SCProvozuId);
                    revizeSC.UmisteniId = scprovozu.Umisteni;

                    db.RevizeSC.Add(revizeSC);
                    RevizeId = revizeSC.RevizeId;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                Revize revize = new Revize();
                revize = db.Revize.Find(RevizeId);
                Revize.UpdateRevizeHeader(RevizeId);
                return RedirectToAction("Details", "Revize", new { Id = RevizeId });
                //return RedirectToAction("Index");
            }

            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav", revizeSC.RevizeId);
            ViewBag.SCProvozuId = new SelectList(db.SCProvozu, "Id", "Lokace", revizeSC.SCProvozuId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", revizeSC.UmisteniId);
            return View(revizeSC);
        }

        // GET: RevizeSC/Edit/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            if (revizeSC == null)
            {
                return HttpNotFound();
            }
            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav", revizeSC.RevizeId);
            ViewBag.SCProvozuId = new SelectList(db.SCProvozu, "Id", "Lokace", revizeSC.SCProvozuId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", revizeSC.UmisteniId);
            return View(revizeSC);
        }

        // POST: RevizeSC/Edit/5
        // Chcete-li zajistit ochranu před útoky typu OVERPOST, povolte konkrétní vlastnosti, k nimž chcete vytvořit vazbu. 
        // Další informace viz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Edit([Bind(Include = "Id,RevizeId,SCProvozuId,StavKoroze,StavZnecisteni,JineZavady,UmisteniId,Baterie,Pyro,TlakovaZkouska,Stav,DobaProvozu,HodinyProvozu,DobaProvozuString")] RevizeSC revizeSC)
        {
            int RevizeId = 0;
            //revizeSC = RevizeSC.CalculateDobuProvozu(revizeSC);
            if (ModelState.IsValid)
            {
                try
                {
                    RevizeId = revizeSC.RevizeId;
                    db.Entry(revizeSC).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                Revize revize = new Revize();
                revize = db.Revize.Find(RevizeId);
                Revize.UpdateRevizeHeader(RevizeId);
                return RedirectToAction("Details","Revize",new {Id = RevizeId });
            }
            ViewBag.RevizeId = new SelectList(db.Revize, "Id", "ZjistenyStav", revizeSC.RevizeId);
            ViewBag.SCProvozuId = new SelectList(db.SCProvozu, "Id", "Lokace", revizeSC.SCProvozuId);
            ViewBag.UmisteniId = new SelectList(db.Umisteni, "Id", "NazevUmisteni", revizeSC.UmisteniId);
            return View(revizeSC);
        }

        // GET: RevizeSC/Delete/5
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            if (revizeSC == null)
            {
                return HttpNotFound();
            }
            return View(revizeSC);
        }

        // POST: RevizeSC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            RevizeSC revizeSC = db.RevizeSC.Find(id);
            int RevizeId = 0;
            try
            {
                RevizeId = revizeSC.RevizeId;
                db.RevizeSC.Remove(revizeSC);
                db.SaveChanges();
            }
            catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

            Revize revize = new Revize();
            revize = db.Revize.Find(RevizeId);
            Revize.UpdateRevizeHeader(RevizeId);
            return RedirectToAction("Details", "Revize", new { Id = RevizeId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
