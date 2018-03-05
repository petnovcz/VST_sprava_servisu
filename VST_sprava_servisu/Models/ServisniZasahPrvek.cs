using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class ServisniZasahPrvek
    {

        internal protected static List<ServisniZasahPrvek> GetPrvkyById(int Id)
        {
            List<ServisniZasahPrvek> list = new List<ServisniZasahPrvek>();
            using (var db = new Model1Container())
            {
                list = db.ServisniZasahPrvek.Where(t => t.ServisniZasahId == Id)
                    .Include(s => s.Artikl)
                    .Include(s => s.Porucha)
                    .Include(s => s.ServisniZasah)
                    .Include(s=>s.SCProvozu)
                    .ToList();


            }



            return list;
        }





    }
}