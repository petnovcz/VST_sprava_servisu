using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class SCProvozu
    {

        internal protected static List<SCProvozu> GetList(int? Provoz, int? SerioveCislo, int? Status, int? Umisteni)
        {
            var list = new List<SCProvozu>();
            using (var dbCtx = new Model1Container())
            {
                var listp = dbCtx.SCProvozu.AsQueryable();
                if (Provoz != null)
                {
                    listp = listp.Where(l => l.ProvozId == Provoz);
                }
                if (SerioveCislo != null)
                {
                    listp = listp.Where(l => l.SerioveCisloId == SerioveCislo);
                }
                if (Status != null)
                {
                    listp = listp.Where(l => l.StatusId == Status);
                }
                if (Umisteni != null)
                {
                    listp = listp.Where(l => l.Umisteni == Umisteni);
                }
                list = listp.ToList();
            }
            return list;
        }
    }
}