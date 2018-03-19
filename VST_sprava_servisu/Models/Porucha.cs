﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Porucha
    {

        internal protected static List<Porucha> GetPoruchyProSkupinu(int SkupinaId)
        {
            List<Porucha> list = new List<Porucha>();
            using (var db = new Model1Container())
            {
                list = db.Porucha.Where(t => t.SkupinaArtikluId == null || t.SkupinaArtikluId == SkupinaId).ToList();


            }
            return list;
        }

        internal protected static bool ReklamaceById(int Id)
        {
            bool decision;
            using (var db = new Model1Container())
            {
                decision = db.Porucha.Where(t => t.Id == Id).Include(t=>t.KategoriePoruchy).Select(t=>t.KategoriePoruchy.ReklamaceServisniZasah).FirstOrDefault();
                    }
            return decision;
        }


    }
}