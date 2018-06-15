using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class ServisniZasahPrvek
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ServisniZasahPrvek");
        public bool SIL;
        public int startindex { get; set; }
        public int endindex { get; set; }
        public int kusovnik_count { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DatumVyprseniZaruky
        {
            get
            {
                DateTime? ukonceniZaruky = null;
                using (var db = new Model1Container())
                {
                    ukonceniZaruky = db.SCProvozu
                        .Where(t => t.Id == Id)
                        .Select(t => t.UkonceniZaruky)
                        .FirstOrDefault();


                }



                return ukonceniZaruky;
            }



        }



        public List<Kusovnik> RadkyKusovniku  {
            get
            {


                // vytvoření listu kusovníku
                List<Kusovnik> radky = new List<Kusovnik>();

                //načtení řádků kusovníku pro artikl z řídku servisního zásahu
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select t1.Code, T1.Quantity, t2.ItemName from OITT t0");
                sql.Append($"  left join ITT1 t1 on t0.Code = t1.Father");
                sql.Append($"  left join OITM t2 on t1.Code = t2.ItemCode");
                sql.Append($"  where t0.Code = '{Artikl.KodSAP}'");

                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sql.ToString();
                cnn.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //MAKES IT HERE   
                    while (dr.Read())
                    {
                        Kusovnik kusovnik = new Kusovnik();
                        kusovnik.ServisniZasahId = ServisniZasahId;
                        kusovnik.ServisniZasahPrvekId = Id;
                        kusovnik.KusovnikSAPKod = Artikl.KodSAP;
                        kusovnik.Mnozstvi = Pocet;
                        kusovnik.ServisniZasahArtiklId = ArtiklID.Value;
                        try
                        {
                            kusovnik.MnozstviRadekKusovniku = dr.GetDecimal(dr.GetOrdinal("Quantity"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            kusovnik.RadekKusovnikuSAPKod = dr.GetString(dr.GetOrdinal("Code"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {

                            
                            kusovnik.NazevArtikluRadekKusovniku = dr.GetString(dr.GetOrdinal("ItemName")); ;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        radky.Add(kusovnik);
                    }
                }
                cnn.Close();



                



                
                
                


                





                
                return radky;
            }

        } 
        
        


        /*internal protected List<string> ArtiklyKusovniku
        { get
            {
                SAPItem sapItem = new SAPItem();
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select ItemCode, ItemName, t0.ItmsGrpCod as 'ItmsGrpCod', t1.ItmsGrpNam as 'ItmGrpNam' from oitm t0 left join OITB t1 on t0.ItmsGrpCod = t1.ItmsGrpCod  where ");
                sql.Append($" ItemCode = '' ");

                //log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
                SqlConnection cnn = new SqlConnection(connectionString);
                //SqlConnection con = new SqlConnection(cnn);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sql.ToString();
                cnn.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //MAKES IT HERE   
                    while (dr.Read())
                    {

                        if (dr.GetString(dr.GetOrdinal("ItemCode")) != null)
                        {
                            sapItem.ItemCode = dr.GetString(0);
                        }
                        try
                        {
                            sapItem.ItemName = dr.GetString(1);
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {
                            sapItem.ItmsGrpNam = dr.GetString(3);
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                        try
                        {

                            int codeint = dr.GetInt16(2);
                            sapItem.ItmsGrpCod = codeint;
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

                    }
                }
                cnn.Close();
                return sapItem;

            }

        }*/





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

        internal protected static ServisniZasahPrvek GetPrvekById(int Id)
        {
            ServisniZasahPrvek list = new ServisniZasahPrvek();
            using (var db = new Model1Container())
            {
                list = db.ServisniZasahPrvek.Where(t => t.Id == Id)
                    .Include(s => s.Artikl)
                    .Include(s => s.Porucha)
                    .Include(s => s.ServisniZasah)
                    .Include(s => s.SCProvozu)
                    .FirstOrDefault();


            }



            return list;
        }





    }

    public partial class Kusovnik
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Kusovnik");
        [Key]
        public int Id { get; set; }
        public int ServisniZasahId { get; set; }
        public int ServisniZasahPrvekId { get; set; }
        public int ServisniZasahArtiklId { get; set; }
        public string KusovnikSAPKod { get; set; }
        public decimal Mnozstvi { get; set; }
        public string RadekKusovnikuSAPKod { get; set; }
        public decimal MnozstviRadekKusovniku { get; set; }
        public string NazevArtikluRadekKusovniku { get; set; }
        public decimal PozadovaneMnozstvi { get
            {
                return Mnozstvi * MnozstviRadekKusovniku;

            } }
        public decimal VybraneMnozstvi { get; set; }
        public decimal OtevreneMnozstvi { get; set; }
        public int Index { get; set; }
        public bool SpravaSeriovychCisel
        {
            get
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select ManSerNum from oitm where ItemCode = '{RadekKusovnikuSAPKod}'");

                string read = "N";

                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sql.ToString();
                cnn.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //MAKES IT HERE   
                    while (dr.Read())
                    {
                        try
                        {
                            read = dr.GetString(dr.GetOrdinal("ManSerNum"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    }
                }
                cnn.Close();
                bool result = false;
                if (read.Equals("Y")) { result = true; } else { result = false; }
                return result;
            }


        }
        public bool SpravaSarzi
        {
            get
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select ManBtchNum from oitm where ItemCode = '{RadekKusovnikuSAPKod}'");

                string read = "N";

                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sql.ToString();
                cnn.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //MAKES IT HERE   
                    while (dr.Read())
                    {
                        try
                        {
                            read = dr.GetString(dr.GetOrdinal("ManBtchNum"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    }
                }
                cnn.Close();
                bool result = false;
                if (read.Equals("Y")) { result = true; } else { result = false; }
                return result;
            }
        }
        public decimal OnHand {
            get
            {
                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select OnHand from OITW where ItemCode = '{RadekKusovnikuSAPKod}' and WhsCode = 'Servis' ");

                decimal onhand = 0;

                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sql.ToString();
                cnn.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //MAKES IT HERE   
                    while (dr.Read())
                    {
                        try
                        {
                            onhand = dr.GetDecimal(dr.GetOrdinal("OnHand"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                    }
                }
                cnn.Close();
                
                return onhand;
            }
        }

    }
}