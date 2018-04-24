using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;


namespace VST_sprava_servisu
{
    public partial class ServisniZasahy
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ServisniZasahy");

        public virtual List<ServisniZasah> ServisniZasah { get; set; }
        public int? ZakaznikId { get; set; }
        public string Projekt { get; set; }
        public DateTime? DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
        public bool? Send { get; set; }
        public bool? Closed { get; set; }

        internal protected static ServisniZasahy GetServisniZasah(int? ZakaznikId, string Projekt, DateTime? DatumOd, DateTime? DatumDo, bool? Send, bool? Closed )
        {
            ServisniZasahy sz = new ServisniZasahy();
            sz.ZakaznikId = ZakaznikId;
            sz.Projekt = Projekt;
            sz.DatumOd = DatumOd;
            sz.DatumDo = DatumDo;
            using (var db = new Model1Container())
            {
                if (Send == true)
                {
                    var x = db.ServisniZasah.Include(s => s.Zakaznik).ToList();
                    if (ZakaznikId != null)
                    {
                        x = x.Where(s => s.ZakaznikID == ZakaznikId).ToList();
                    }
                    if (Projekt != null && Projekt != "")
                    {
                        x = x.Where(s => s.Projekt == Projekt).ToList();
                    }
                    if (DatumOd != null)
                    {
                        x = x.Where(s => s.DatumZasahu >= DatumOd).ToList();
                    }
                    if (DatumDo != null)
                    {
                        x = x.Where(s => s.DatumZasahu <= DatumDo).ToList();
                    }
                    if (Closed != null)
                    {
                        x = x.Where(s=>s.Closed == Closed).ToList(); 
                    }
                    sz.ServisniZasah = x;
                }
            }
            return sz;
        }

    }
}