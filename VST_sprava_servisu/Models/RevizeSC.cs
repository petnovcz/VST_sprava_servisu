using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class RevizeSC
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("RevizeSC");


        public static List<RevizeSC> GetListByRevizeId(int RevizeId, int? SCprovozuId)
        {
            var list = new List<RevizeSC>();

            using (var dbCtx = new Model1Container())
            {
                var listp = dbCtx.RevizeSC.Where(r => r.RevizeId == RevizeId);
                if (SCprovozuId != null)
                {
                    listp = listp.Where(r => r.SCProvozuId == SCprovozuId);
                }
                list = listp.ToList();
            }
            return list;
        }

        internal protected static bool CreateUpdateSC(List<SCProvozu> SCProvozu, int RevizeId)
        {
            //exist revize?
            if (Revize.RevizeExist(RevizeId) == true)
            {
                // loop through list of sc dedicated to provoz
                foreach (var item in SCProvozu)
                {
                    //exist revizeSC
                    if (RevizeSC.ExistingRevizeSC(item, RevizeId) == true)
                    {
                        //update revize SC
                    }
                    else
                    {
                        //create revize SC
                        RevizeSC r = new RevizeSC();
                        r.CreateRevizeSC(RevizeId, item.Id, item.Umisteni.Value);

                    }
                }
            }
            return true;
        }

        internal protected static bool ExistingRevizeSC(SCProvozu scprovozu, int RevizeId)
        {
            List<RevizeSC> list = new List<RevizeSC>();
            list = RevizeSC.GetListByRevizeId(RevizeId, scprovozu.Id);
            bool exist = false;
            if (list.Count() > 0) { exist = true; }
            return exist;
        }

        internal protected void CreateRevizeSC(int RevizeId, int ScProvozu, int UmisteniId)
        {
            RevizeSC revizeSC = new RevizeSC();
            revizeSC.RevizeId = RevizeId;
            revizeSC.SCProvozuId = ScProvozu;
            revizeSC.UmisteniId = UmisteniId;


            using (var dbCtx = new Model1Container())
            {
                try
                {
                    dbCtx.RevizeSC.Add(revizeSC);
                    dbCtx.SaveChanges();
                }
                catch (Exception ex) { log.Error($"CreateRevizeSC {ex.Message} {ex.InnerException} {ex.Data}"); }

            }
        }

        internal protected static List<RevizeSC> SeznamTlakovychZkousekRevize(int RevizeId)
        {
            List<RevizeSC> list = new List<RevizeSC>();
            using (var dbCtx = new Model1Container())
            {
                list = dbCtx.RevizeSC
                    .Where(r => r.RevizeId == RevizeId)
                    .Where(r => r.TlakovaZkouska == true)
                    .ToList();

            }

            return list;
        }

        private static bool TestIfRevizeSCExistsinRevize(Revize RevizeTlkZK, RevizeSC RevizeSC)
        {
            var exist = false;
            using (var dbCtx = new Model1Container())
            {
                var x = dbCtx.RevizeSC
                    .Where(r => r.Id == RevizeTlkZK.Id)
                    .Where(r => r.SCProvozuId == RevizeSC.SCProvozuId).Count();
                if (x > 0) { exist = true; }
            }
            
            return exist;
        }


        private static void RemoveTlkZKSignFromREvizeSC(RevizeSC RevizeSC)
        {
            using (var dbCtx = new Model1Container())
            {

                var x = dbCtx.RevizeSC.Find(RevizeSC.Id);
                x.TlakovaZkouska = false;
                dbCtx.Entry(x).State = EntityState.Modified;
                try
                {
                    dbCtx.SaveChanges();
                }
                catch (Exception ex) { log.Error($"RemoveTlkZKSignFromREvizeSC {ex.Message} {ex.InnerException} {ex.Data}"); }

            }

        }

        private static void AddRevizeSCTlkZkToRevision(Revize RevizeTlkZK, RevizeSC RevizeSC)
        {
            using (var dbCtx = new Model1Container())
            {
                RevizeSC revizeSCTlkZK = new RevizeSC();
                revizeSCTlkZK.RevizeId = RevizeTlkZK.Id;
                revizeSCTlkZK.SCProvozuId = RevizeSC.SCProvozuId;
                revizeSCTlkZK.UmisteniId = RevizeSC.UmisteniId;
                revizeSCTlkZK.TlakovaZkouska = RevizeSC.TlakovaZkouska;


                try
                {
                    dbCtx.RevizeSC.Add(revizeSCTlkZK);
                    dbCtx.SaveChanges();
                    RemoveTlkZKSignFromREvizeSC(RevizeSC);
                }
                catch (Exception ex) { log.Error($"CreateRevizeSCTlkZK {ex.Message} {ex.InnerException} {ex.Data}"); }
                
            }

        }


        internal protected static void LoopRevizeSCTlakoveZkousky(Revize RevizeTlkZK, List<RevizeSC> ListRevizeSC)
        {
        foreach(var item in ListRevizeSC)
            {
                var exist = TestIfRevizeSCExistsinRevize(RevizeTlkZK, item);
                if (exist == false)
                { AddRevizeSCTlkZkToRevision(RevizeTlkZK, item); }
                else
                { RemoveTlkZKSignFromREvizeSC(item); }
                

            }

        }
        
    }
}