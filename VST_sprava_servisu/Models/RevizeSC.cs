using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class RevizeSC
    {

        


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
                dbCtx.RevizeSC.Add(revizeSC);
                dbCtx.SaveChanges();
            }
        }

         

        
    }
}