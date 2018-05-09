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

        internal protected static RevizeSC CalculateDobuProvozu(RevizeSC revizeSC)
        {
            revizeSC.HodinyProvozu = 1000;


            return revizeSC;
        }





        internal protected static void DeleteRevizeSCFromRevize(int RevizeID)
        {
            using (var dbCtx = new Model1Container())
            {
                List<RevizeSC> list = new List<RevizeSC>();
                list = dbCtx.RevizeSC.Where(r => r.RevizeId == RevizeID).ToList();

                foreach (var item in list)
                {
                    var revizesc = dbCtx.RevizeSC.Find(item.Id);
                    try
                    {
                        dbCtx.RevizeSC.Remove(revizesc);
                        dbCtx.SaveChanges();
                    }
                    catch (Exception ex) { }

                }
            }
        }


        internal protected static void Remove(int RevizeSCId)
        {
            using (var dbCtx = new Model1Container())
            {
                RevizeSC revizeSC = dbCtx.RevizeSC.Find(RevizeSCId);
                try
                {

                    dbCtx.RevizeSC.Remove(revizeSC);
                    dbCtx.SaveChanges();
                }
                catch (Exception ex) { }
            }
        }

        internal protected static void ChangeRevizeSCForUpcomingOpenRevision(SCProvozu oldSCProvozu, SCProvozu newSCProvozu, DateTime PocatecniDatum)
        {
            List<RevizeSC> list = new List<RevizeSC>();
            list = GetNextOpenRevizeForRevizeSC(oldSCProvozu, PocatecniDatum);
            SwitchSCprovozu(list, newSCProvozu);

        }

        private static void SwitchSCprovozu(List<RevizeSC> list, SCProvozu newSCProvozu)
        {
            foreach (var item in list)
            {
                using (var dbCtx = new Model1Container())
                {
                    RevizeSC revizeSC = new RevizeSC();
                    revizeSC = dbCtx.RevizeSC.Find(item.Id);
                    revizeSC.SCProvozuId = newSCProvozu.Id;
                    try
                    {
                        dbCtx.Entry(revizeSC).State = EntityState.Modified;
                        dbCtx.SaveChanges();
                    }
                    catch (Exception ex) { }

                }


            }


        }


        private static List<RevizeSC> GetNextOpenRevizeForRevizeSC(SCProvozu scPRovozu, DateTime pocatecniDatum)
        {
            List<RevizeSC> list = new List<RevizeSC>();
            
            using (var dbCtx = new Model1Container())
            {
                var uzavrena = dbCtx.StatusRevize.Where(s => s.Realizovana != true).Select(r=>r.Id).FirstOrDefault();
                var x = dbCtx.RevizeSC.Where(r => r.SCProvozuId == scPRovozu.Id).Include(r => r.Revize)
                    .Where(r => r.Revize.DatumRevize > pocatecniDatum)
                    .Where(r => r.Revize.StatusRevizeId == uzavrena).ToList();

                list = x;
            }
                return list;
        }



        public static RevizeSC GetRevizeSCByRevizeSCid(int RevizeSCId)
        {
            RevizeSC revizeSC = new RevizeSC();
            using (var dbCtx = new Model1Container())
            {
                revizeSC = dbCtx.RevizeSC
                    .Where(r => r.Id == RevizeSCId)
                    //.Include(r => r.SCProvozu)                
                    .FirstOrDefault();
                revizeSC.SCProvozu = dbCtx.SCProvozu.Where(r => r.Id == revizeSC.SCProvozuId).FirstOrDefault();
                revizeSC.SCProvozu.SerioveCislo = dbCtx.SerioveCislo.Where(r => r.Id == revizeSC.SCProvozu.SerioveCisloId).FirstOrDefault();
                revizeSC.SCProvozu.SerioveCislo.Artikl = dbCtx.Artikl.Where(r => r.Id == revizeSC.SCProvozu.SerioveCislo.ArtiklId).FirstOrDefault();
                
            }
            return revizeSC;
        }

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