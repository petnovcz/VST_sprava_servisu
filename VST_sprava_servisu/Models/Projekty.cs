using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace VST_sprava_servisu
{
    public partial class Projekty
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("Projekty");
        public bool Send { get; set; }
        public List<Roky> RokyList
        {
            get
            {
                List<Roky> list = new List<Roky>();
                for (int i = 2013; i <= DateTime.Now.Year; i++)
                {
                    Roky rok = new Roky();
                    rok.Id = i;
                    list.Add(rok);
                }
                return list;
            }
        }
        public List<Roky> VybraneRokyList { get; set; }
        public Roky AktualniRok
        {
            get
            {
                Roky rok = new Roky();
                rok.Id = DateTime.Now.Year;
                return rok;
            }
        }

        public List<TypProjektu> VybraneTypyProjektuList { get; set; }
        public List<TypProjektu> TypProjektuList
        {
            get
            {
                List<TypProjektu> list = new List<TypProjektu>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" SELECT TOP 1000 [Code],[Name] FROM [dbo].[@VCZ_CT_TYPES]");
                

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
                        TypProjektu tp = new TypProjektu();                        
                        try
                        {
                            tp.Kod = dr.GetString(dr.GetOrdinal("Code"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            tp.Nazev = dr.GetString(dr.GetOrdinal("Name"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        list.Add(tp);
                    }
                }
                cnn.Close();
                return list;
            }
        }
        public List<CiselnaRadaProjektu> VybranaCiselnaRadaList { get; set; }
        public List<CiselnaRadaProjektu> CiselnaRadaList
        {
            get
            {
                List<CiselnaRadaProjektu> list = new List<CiselnaRadaProjektu>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" SELECT Name, U_VCZ_5020 FROM[SBO].[dbo].[@VCZ_DOKL_SETP] where[U_VCZ_5024] = 1005");


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
                        CiselnaRadaProjektu tp = new CiselnaRadaProjektu();
                        try
                        {
                            tp.Kod = dr.GetString(dr.GetOrdinal("U_VCZ_5020"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            tp.Nazev = dr.GetString(dr.GetOrdinal("Name"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        list.Add(tp);
                    }
                }
                cnn.Close();
                return list;
            }
        }
        public List<StavyProjektu> VybraneStavyProjektuList { get; set; }
        public List<StavyProjektu> StavyProjektuList
        {
            get
            {
                List<StavyProjektu> list = new List<StavyProjektu>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" SELECT Code, Name FROM [dbo].[@VCZ_CT_STATUS] where Code not in ('2','6','8') order by Code asc");


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
                        StavyProjektu tp = new StavyProjektu();
                        try
                        {
                            tp.Kod = dr.GetString(dr.GetOrdinal("Code"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            tp.Nazev = dr.GetString(dr.GetOrdinal("Name"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        list.Add(tp);
                    }
                }
                cnn.Close();
                return list;
            }
        }
        public List<Regiony> VybraneRegionyList { get; set; }
        public List<Regiony> RegionyList
        {
            get
            {
                List<Regiony> list = new List<Regiony>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" SELECT territryID , descript FROM [dbo].[OTER] where inactive = 'N' order by descript asc");


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
                        Regiony tp = new Regiony();
                        try
                        {
                            tp.TerritryID = dr.GetInt32(dr.GetOrdinal("TerritryID"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            tp.Descript = dr.GetString(dr.GetOrdinal("Descript"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        list.Add(tp);
                    }
                }
                cnn.Close();
                return list;
            }
        }
        public List<Pracovnici> VybraniPracovniciList { get; set; }
        public List<Pracovnici> PracovniciList
        {
            get
            {
                List<Pracovnici> list = new List<Pracovnici>();
                string vybraneroky = "";
                if (VybraneRokyList != null)
                {
                    vybraneroky = "'" + string.Join("','", VybraneRokyList.Select(t => t.Id).ToArray()) + "'";
                }
                else { vybraneroky = "''"; }

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select t0.empID, t0.lastName as 'Name'"
                    + " from Ohem t0 where /*t0.Active = 'Y' and*/ (select COUNT(*) from HEM6 tx where tx.empID = t0.empID and tx.roleID not in ('14', '17', '18', '9', '10', '11')) > 0 "
                    + $" and ((select count(*) from [@VCZ_CT_PRJ] tx where tx.[U_EmpNo] = t0.empID and tx.[U_status] not in ('2','6','8') and year(coalesce(U_ActEndDt,U_EndDate,(select max(docdate) from oinv where project = tx.code))) in ({vybraneroky}) ) > 0)"
                    +
                    "order by lastName asc");


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
                        Pracovnici tp = new Pracovnici();
                        try
                        {
                            tp.Kod = dr.GetInt32(dr.GetOrdinal("empID"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            tp.Nazev = dr.GetString(dr.GetOrdinal("Name"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        list.Add(tp);
                    }
                }
                cnn.Close();
                return list;
            }
        }

        public List<SAPSkupinaArtiklu> VybranaSkupinaArtikluList { get; set; }
        public List<SAPSkupinaArtiklu> SkupinaArtikluList
        {
            get
            {
                List<SAPSkupinaArtiklu> list = new List<SAPSkupinaArtiklu>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" select ItmsGrpCod, ItmsGrpNam from OITB where itmsgrpcod not in ('142','100','137','138','139','142','144','145','146') order by ItmsGrpNam asc ");


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
                        SAPSkupinaArtiklu tp = new SAPSkupinaArtiklu();
                        try
                        {
                            tp.Id = dr.GetInt16(dr.GetOrdinal("ItmsGrpCod"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            tp.Nazev = dr.GetString(dr.GetOrdinal("ItmsGrpNam"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        list.Add(tp);
                    }
                }
                cnn.Close();
                return list;
            }
        }

        public List<Projekt> projektyList { get; set; }

        public static Projekty GetProjectList (Projekty projekty)

        {
            List<Projekt> list = new List<Projekt>();
            string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            StringBuilder sql = new StringBuilder();
            string vybranetypyp = "";
            string vybraniprac = "";
            string vybraneregiony = "";
            string vybranestavyp = "";
            string vybraneroky = "";
            string vybranecr = "";
            string vybranesa = "";

            if (projekty.VybraniPracovniciList != null)
            {
                vybraniprac = "'" + string.Join("','", projekty.VybraniPracovniciList.Select(t => t.Kod).ToArray()) + "'";
            }
            if (projekty.VybraneRegionyList != null)
            {
                vybraneregiony = "'" + string.Join("','", projekty.VybraneRegionyList.Select(t => t.TerritryID).ToArray()) + "'";
            }
            if (projekty.VybraneStavyProjektuList != null)
            {
                vybranestavyp = "'" + string.Join("','", projekty.VybraneStavyProjektuList.Select(t => t.Kod).ToArray()) + "'";
            }
            if(projekty.VybraneTypyProjektuList != null)
            { 
                vybranetypyp = "'" + string.Join("','", projekty.VybraneTypyProjektuList.Select(t => t.Kod).ToArray()) + "'";
            }
            if (projekty.VybraneRokyList != null)
            {
                vybraneroky = "'" + string.Join("','", projekty.VybraneRokyList.Select(t => t.Id).ToArray()) + "'";
            }
            if (projekty.VybranaCiselnaRadaList != null)
            {
                vybranecr = "'" + string.Join("','", projekty.VybranaCiselnaRadaList.Select(t => t.Kod).ToArray()) + "'";
            }
            if (projekty.VybranaSkupinaArtikluList != null)
            {
                vybranesa = "'" + string.Join("','", projekty.VybranaSkupinaArtikluList.Select(t => t.Id).ToArray()) + "'";
            }

            sql.Append(" select t0.Code, t0.U_Descript, T0.U_EmpNo, t0.U_Status, t4.Name as 'Status_descript', t0.U_Series, t0.U_Type, t5.Name as 'TypeName'" +
                    " ,t2.territryID, t2.descript, t1.CardCode, t1.cardname," +
                    " t0.U_DocCurr ,t0.U_PlaRev, t0.U_PlaRevFC, t0.U_PlaRevSC, t0.U_ActRev, t0.U_ActRevFC, t0.U_ActRevSC, t0.U_PlaExp, t0.U_PlaExpFC, t0.U_PlaExpSC, t0.U_ActExp, t0.U_ActExpFC, t0.U_ActExpSC" +
                    " ,t3.empID, t3.lastName + ', ' + t3.firstName as 'EmpName'" +
                    " from [@VCZ_CT_PRJ] t0 left join OCRD t1 on t0.U_CardCode = t1.CardCode" +
                    " left join OTER t2 on t1.Territory = t2.territryID" +
                    " left join OHEM t3 on t0.U_EmpNo = t3.empID" +
                    " left join [@VCZ_CT_STATUS] t4 on t4.code = t0.U_status" +
                    " left join [@VCZ_CT_TYPES] t5 on t5.code = t0.U_Type "
                    );
            if (projekty.VybraniPracovniciList != null)
            {
                sql.Append($" where t0.U_EmpNo in ({vybraniprac})");
            }
            else
            {
                sql.Append($" where t0.U_EmpNo in ('')");
            }
            if (projekty.VybraneRegionyList != null)
            {
                sql.Append($" and t2.territryID in ({vybraneregiony})");
            }
            else
            {
                sql.Append($" and t2.territryID in ('')");
            }
            if (projekty.VybranaCiselnaRadaList != null)
            {
                sql.Append($" and t0.U_Series in ({vybranecr})");
            }
            else
            {
                sql.Append($" and t0.U_Series in ('')");
            }
            if (projekty.VybraneStavyProjektuList != null)
            {
                sql.Append($" and t0.U_Status in ({vybranestavyp})");
            }
            else
            {
                sql.Append($" and t0.U_Status in ('')");
            }
            if (projekty.VybraneTypyProjektuList != null)
            {
                sql.Append($" and T0.U_Type in ({vybranetypyp})");
            }
            else
            {
                sql.Append($" and T0.U_Type in ('')");
            }
            if (projekty.VybraneRokyList != null)
            {
                sql.Append($" and year(coalesce(t0.U_ActEndDt,t0.U_EndDate,(select max(docdate) from oinv where project = t0.code))) in ({vybraneroky}) ");
            }
            else
            {
                sql.Append($" and year(coalesce(t0.U_ActEndDt,t0.U_EndDate,(select max(docdate) from oinv where project = t0.code))) in ('') ");
            }
            if (projekty.VybranaSkupinaArtikluList != null)
            {
                sql.Append($" and (select COUNT(*) from ORDR tx0 inner join RDR1 tx1 on tx0.DocEntry = tx1.DocEntry inner join OITM tx2 on tx1.ItemCode = tx2.itemcode where tx2.ItmsGrpCod in ({vybranesa}) and tx0.Project = t0.code) > 0  ");
            }
            else
            {
                sql.Append($" ");
            }



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
                        Projekt tp = new Projekt();
                        try
                        {
                            tp.Code = dr.GetString(dr.GetOrdinal("Code"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                        try
                        {
                            tp.Name = dr.GetString(dr.GetOrdinal("U_Descript"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                        try
                        {
                            tp.Status = dr.GetString(dr.GetOrdinal("U_Status"));
                        }
                        catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.Status_descript = dr.GetString(dr.GetOrdinal("Status_descript"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.Series = dr.GetString(dr.GetOrdinal("U_Series"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.Type = dr.GetString(dr.GetOrdinal("U_Type"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.TypeName = dr.GetString(dr.GetOrdinal("TypeName"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.Territory = dr.GetInt32(dr.GetOrdinal("territryID"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.TeritoryName = dr.GetString(dr.GetOrdinal("descript"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.EmpNo = dr.GetInt32(dr.GetOrdinal("EmpId"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.EmpName = dr.GetString(dr.GetOrdinal("EmpName"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        tp.CardName = dr.GetString(dr.GetOrdinal("CardName"));
                    }
                    catch (Exception ex) {// log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }

                    try
                    {
                        tp.U_PlaRev = dr.GetDecimal(dr.GetOrdinal("U_PlaRev"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }

                    try
                    {
                        tp.U_PlaRevFC = dr.GetDecimal(dr.GetOrdinal("U_PlaRevFC"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }

                    try
                    {
                        tp.U_PlaRevSC = dr.GetDecimal(dr.GetOrdinal("U_PlaRevSC"));
                    }
                    catch (Exception ex) {// log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.U_ActRev = dr.GetDecimal(dr.GetOrdinal("U_ActRev"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.U_ActRevFC = dr.GetDecimal(dr.GetOrdinal("U_ActRevFC"));
                    }
                    catch (Exception ex) {// log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                            tp.U_ActRevSC = dr.GetDecimal(dr.GetOrdinal("U_ActRevSC"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.U_PlaExp = dr.GetDecimal(dr.GetOrdinal("U_PlaExp"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                            {
                                tp.U_PlaExpFC = dr.GetDecimal(dr.GetOrdinal("U_PlaExpFC"));
                            }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                            {
                                tp.U_PlaExpFC = dr.GetDecimal(dr.GetOrdinal("U_PlaExpFC"));
                            }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.U_PlaExpSC = dr.GetDecimal(dr.GetOrdinal("U_PlaExpSC"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.U_ActExp = dr.GetDecimal(dr.GetOrdinal("U_ActExp"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        tp.U_ActExpFC = dr.GetDecimal(dr.GetOrdinal("U_ActExpFC"));
                    }
                    catch (Exception ex) {// log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }
                    try
                    {
                        tp.U_ActExpSC = dr.GetDecimal(dr.GetOrdinal("U_ActExpSC"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    try
                    {
                        tp.DocCurr = dr.GetString(dr.GetOrdinal("U_DocCurr"));
                    }
                    catch (Exception ex) { //log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                    }

                    list.Add(tp);
                    }
                }
                cnn.Close();
            projekty.projektyList = list;
                return projekty;
            }
        

        
    }

    

    public class Pracovnici
    {
        public int Kod { get; set; }
        public string Nazev { get; set; }
    }

    public class TypProjektu
    {
        public string Kod { get; set; }
        public string Nazev { get; set; }
    }
    public class StavyProjektu
    {
        public string Kod { get; set; }
        public string Nazev { get; set; }
    }
    public class CiselnaRadaProjektu
    {
        public string Kod { get; set; }
        public string Nazev { get; set; }
    }

    public class Regiony
    {
        public int TerritryID { get; set; }
        public string Descript { get; set; }
    }

    public class SAPSkupinaArtiklu
    {
        public int Id { get; set; }
        public string Nazev { get; set; }
    }

    public class Roky
    {
        public int Id { get; set; }
    }

    public class ORDRforProject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ORDRforProject");
        public string Projekt { get; set; }
        public string ProjektName { get; set; }
        public virtual List<SAPORDR> ORDRList {
            get {

                List<SAPORDR> list = new List<SAPORDR>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select * from ORDR where project = '{Projekt}' and Canceled = 'N'");


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
                        SAPORDR item = new SAPORDR();
                        try
                        {
                            item.DocEntry = dr.GetInt32(dr.GetOrdinal("DocEntry"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocEntry" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Docnum = dr.GetInt32(dr.GetOrdinal("Docnum"));
                        }
                        catch (Exception ex) {// log.Error("Error number: Docnum" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Canceled = dr.GetString(dr.GetOrdinal("Canceled"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Cancelled" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DocStatus = dr.GetString(dr.GetOrdinal("DocStatus"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocStatus" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                        }
                        catch (Exception ex) { //log.Error("Error number: CardCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.CardName = dr.GetString(dr.GetOrdinal("CardName"));
                        }
                        catch (Exception ex) {// log.Error("Error number: CardName " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.VatSum = dr.GetDecimal(dr.GetOrdinal("VatSum"));
                        }
                        catch (Exception ex) { //log.Error("Error number: VatSum" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.VatSumFC = dr.GetDecimal(dr.GetOrdinal("VatSumFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: VatSumFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocCur = dr.GetString(dr.GetOrdinal("DocCur"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocCur" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocRate = dr.GetDecimal(dr.GetOrdinal("DocRate"));
                        }
                        catch (Exception ex) {// log.Error("Error number: DocRate" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DocTotal = dr.GetDecimal(dr.GetOrdinal("DocTotal"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocTotal" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DocTotalFC = dr.GetDecimal(dr.GetOrdinal("DocTotalFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocTotalFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrosProfit = dr.GetDecimal(dr.GetOrdinal("GrosProfit"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrosProfit" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.GrosProfFC = dr.GetDecimal(dr.GetOrdinal("GrosProfFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrosProfFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.SlpCode = dr.GetInt16(dr.GetOrdinal("SlpCode"));
                        }
                        catch (Exception ex) { //log.Error("Error number: SlpCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        /*try
                        {
                            item.SLpName = dr.GetString(dr.GetOrdinal("SLpName"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }*/
                        
                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;


            }

        }

    }

    public class OINVforProject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("OINVforProject");
        public string Projekt { get; set; }
        public string ProjektName { get; set; }
        public virtual List<SAPOINV> OINVList
        {
            get
            {

                List<SAPOINV> list = new List<SAPOINV>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select * from OINV where project = '{Projekt}' and Canceled = 'N'");


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
                        SAPOINV item = new SAPOINV();
                        try
                        {
                            item.DocEntry = dr.GetInt32(dr.GetOrdinal("DocEntry"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocEntry" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Docnum = dr.GetInt32(dr.GetOrdinal("Docnum"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Docnum" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Canceled = dr.GetString(dr.GetOrdinal("Canceled"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Cancelled" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DocStatus = dr.GetString(dr.GetOrdinal("DocStatus"));
                        }
                        catch (Exception ex) {// log.Error("Error number: DocStatus" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                        }
                        catch (Exception ex) { //log.Error("Error number: CardCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.CardName = dr.GetString(dr.GetOrdinal("CardName"));
                        }
                        catch (Exception ex) { //log.Error("Error number: CardName " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.VatSum = dr.GetDecimal(dr.GetOrdinal("VatSum"));
                        }
                        catch (Exception ex) { //log.Error("Error number: VatSum" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.VatSumFC = dr.GetDecimal(dr.GetOrdinal("VatSumFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: VatSumFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocCur = dr.GetString(dr.GetOrdinal("DocCur"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocCur" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DocRate = dr.GetDecimal(dr.GetOrdinal("DocRate"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocRate" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocTotal = dr.GetDecimal(dr.GetOrdinal("DocTotal"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocTotal" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocTotalFC = dr.GetDecimal(dr.GetOrdinal("DocTotalFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocTotalFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrosProfit = dr.GetDecimal(dr.GetOrdinal("GrosProfit"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrosProfit" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrosProfFC = dr.GetDecimal(dr.GetOrdinal("GrosProfFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrosProfFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.SlpCode = dr.GetInt16(dr.GetOrdinal("SlpCode"));
                        }
                        catch (Exception ex) { //log.Error("Error number: SlpCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        /*try
                        {
                            item.SLpName = dr.GetString(dr.GetOrdinal("SLpName"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }*/

                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;


            }

        }

    }

    public class OPCHforProject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("OPCHforProject");
        public string Projekt { get; set; }
        public string ProjektName { get; set; }
        public virtual List<SAPOPCH> OPCHList
        {
            get
            {

                List<SAPOPCH> list = new List<SAPOPCH>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select * from OPCH where project = '{Projekt}' and Canceled = 'N'");


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
                        SAPOPCH item = new SAPOPCH();
                        try
                        {
                            item.DocEntry = dr.GetInt32(dr.GetOrdinal("DocEntry"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocEntry" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Docnum = dr.GetInt32(dr.GetOrdinal("Docnum"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Docnum" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Canceled = dr.GetString(dr.GetOrdinal("Canceled"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Cancelled" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocStatus = dr.GetString(dr.GetOrdinal("DocStatus"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocStatus" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.CardCode = dr.GetString(dr.GetOrdinal("CardCode"));
                        }
                        catch (Exception ex) { //log.Error("Error number: CardCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.CardName = dr.GetString(dr.GetOrdinal("CardName"));
                        }
                        catch (Exception ex) { //log.Error("Error number: CardName " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.VatSum = dr.GetDecimal(dr.GetOrdinal("VatSum"));
                        }
                        catch (Exception ex) { //log.Error("Error number: VatSum" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.VatSumFC = dr.GetDecimal(dr.GetOrdinal("VatSumFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: VatSumFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocCur = dr.GetString(dr.GetOrdinal("DocCur"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DocCur" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocRate = dr.GetDecimal(dr.GetOrdinal("DocRate"));
                        }
                        catch (Exception ex) {// log.Error("Error number: DocRate" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocTotal = dr.GetDecimal(dr.GetOrdinal("DocTotal"));
                        }
                        catch (Exception ex) {// log.Error("Error number: DocTotal" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.DocTotalFC = dr.GetDecimal(dr.GetOrdinal("DocTotalFC"));
                        }
                        catch (Exception ex) {// log.Error("Error number: DocTotalFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrosProfit = dr.GetDecimal(dr.GetOrdinal("GrosProfit"));
                        }
                        catch (Exception ex) {// log.Error("Error number: GrosProfit" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrosProfFC = dr.GetDecimal(dr.GetOrdinal("GrosProfFC"));
                        }
                        catch (Exception ex) {// log.Error("Error number: GrosProfFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.SlpCode = dr.GetInt16(dr.GetOrdinal("SlpCode"));
                        }
                        catch (Exception ex) {// log.Error("Error number: SlpCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        /*try
                        {
                            item.SLpName = dr.GetString(dr.GetOrdinal("SLpName"));
                        }
                        catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }*/

                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;


            }

        }

    }


    public class CashFlowforProject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("CashFlowforProject");
        public string Projekt { get; set; }
        public string ProjektName { get; set; }
        public virtual List<CashList> CashList
        {
            get
            {

                List<CashList> list = new List<CashList>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append(" create table #TempTabPoptavky ([Week] nvarchar(10) COLLATE DATABASE_DEFAULT,[PlanovaneVynosy] numeric(19,6) ,[PlanovaneNaklady] numeric(19,6),[SkutecneVynosy] numeric(19,6),[SkutecneNaklady] numeric(19,6),[ZISK] numeric(19,6))");
                sql.Append(" insert #TempTabPoptavky ");
                sql.Append($" select x.Week, (Select coalesce(sum(tx.DocTotal - tx.VatSum), 0) from ORDR tx where tx.Project = '{Projekt}' ) as 'PlanovaneVynosy'," +
                    $"(Select coalesce(sum(tx.DocTotal - tx.VatSum - tx.GrosProfit), 0) from ORDR tx where tx.Project = '{Projekt}' ) as 'PlanovaneNaklady' ," +
                    $"SUM(x.[Výnosy období]) as 'SkutecneVynosy', SUM(X.[Náklady období]) as 'SkutecneNaklady'");
                sql.Append(" ,SUM(x.[Výnosy období]) - SUM(X.[Náklady období]) as 'ZISK' " +
                    "from(");
                sql.Append(" SELECT  " +
                    "CAST(YEAR(refdate) AS varchar(5)) + ' - ' + RIGHT(Replicate('0', 2) + CAST(DATEPART(wk, refdate) AS varchar(5)), 2) as 'Week',");
                sql.Append(" case WHEN T0.Account like '6%' OR T0.Account = '799600' then (T0.Credit -  T0.Debit) WHEN T0.Account like '8%' and ContraAct like '6%' then(t0.Credit - t0.Debit) ELSE 0 END 'Výnosy období' ,");
                sql.Append(" CASE WHEN T0.Account like '5%'  OR T0.Account = '799500' then (T0.Debit - T0.Credit) WHEN T0.Account like '8%' and ContraAct like '6%'  then(t0.Credit - t0.Debit) ELSE 0 END 'Náklady období'");
                sql.Append(" FROM JDT1 T0" +
                    " left OUTER JOIN OPRJ T1 ON T0.Project = T1.PrjCode " +
                    "WHERE(T0.Account like '6%' OR T0.Account like '5%' OR T0.Account = '799500' OR T0.Account = '799600' OR T0.Account like '813%')");
                sql.Append($" AND(isnull(T0.Project, '') >= '{Projekt}' OR '{Projekt}' = '') AND(isnull(T0.Project, '') <= '{Projekt}' OR '{Projekt}' = '') AND T0.TransType NOT IN('-2', '-3')" +
                    $" union all" +
                    $" select CAST(YEAR(t0.docDate) AS varchar(5)) +' - ' + RIGHT(Replicate('0', 2) + CAST(DATEPART(wk, t0.docDate) AS varchar(5)), 2) as 'Week', 0,0" +
                    $" from ORCT T0 left join rct1 T1 on T0.DocNum = T1.DocNum left join ocrd T2 on T2.CardCode = T0.CardCode left outer join ODSC T3 on T3.BankCode = T0.BankCode left join RCT2 T4 on T0.DocNum = T4.DocNum left join OINV T5 on T4.DocEntry = T5.DocEntry" +
                    $" where T5.Project = '{Projekt}' and(CASE when T0.CheckSum = 0 and T0.TrsfrSum = 0 and T0.CashSum = 0 then T0.CreditSum when T0.CheckSum = 0 and T0.TrsfrSum = 0 and T0.CreditSum = 0 then T0.CashSum when T0.CheckSum = 0 and T0.CreditSum = 0 and T0.CashSum = 0 then T0.TrsfrSum when T0.CreditSum = 0 and T0.TrsfrSum = 0 and T0.CashSum = 0 then T0.CheckSum end) > 0" +
                    $" union all" +
                    $" SELECT CAST(YEAR(t0.ReconDate) AS varchar(5)) + ' - ' + RIGHT(Replicate('0', 2) + CAST(DATEPART(wk, t0.ReconDate) AS varchar(5)), 2) as 'Week', 0, 0 FROM[dbo].[OITR] T0 INNER JOIN[dbo].[ITR1] T1 ON T0.ReconNum = T1.ReconNum  INNER JOIN OJDT T2 ON T1.TransId = T2.TransId left join OINV T3 on t3.DocEntry = t1.SrcObjAbs and t3.ObjType = t1.SrcObjTyp WHERE t3.Project = '{Projekt}'" +
                    $"" +
                    $") X group by x.Week order by x.Week asc");
                sql.Append(" Select y.Week, y.PlanovaneNaklady,y.PlanovaneVynosy ,(select SUM(SkutecneNaklady) from #TempTabPoptavky x where x.Week <= y.week) as 'SkutecneNaklady',(select SUM(SkutecneVynosy) from #TempTabPoptavky x where x.Week <= y.week) as 'SkutecneVynosy' " +
                    " ,( select SUM(z.[AmountPaid]) from( select" +
                    " CASE when T0.CheckSum = 0 and T0.TrsfrSum = 0 and T0.CashSum = 0 then T0.CreditSum when T0.CheckSum = 0 and T0.TrsfrSum = 0 and T0.CreditSum = 0 then T0.CashSum when T0.CheckSum = 0 and T0.CreditSum = 0 and T0.CashSum = 0 then T0.TrsfrSum when T0.CreditSum = 0 and T0.TrsfrSum = 0 and T0.CashSum = 0 then T0.CheckSum end as 'AmountPaid'" +
                    " from ORCT T0 left join rct1 T1 on T0.DocNum = T1.DocNum left join ocrd T2 on T2.CardCode = T0.CardCode left outer join ODSC T3 on T3.BankCode = T0.BankCode left join RCT2 T4 on T0.DocNum = T4.DocNum" +
                    $" left join OINV T5 on T4.DocEntry = T5.DocEntry where T5.Project = '{Projekt}' and CAST(YEAR(t0.DocDate) AS varchar(5)) + ' - ' + RIGHT(Replicate('0', 2) + CAST(DATEPART(wk, t0.DocDate) AS varchar(5)), 2) <= y.week" +
                    $" union all" +
                    $" SELECT sum(t1.ReconSum) FROM[dbo].[OITR] T0 INNER JOIN[dbo].[ITR1] T1 ON T0.ReconNum = T1.ReconNum INNER JOIN OJDT T2 ON T1.TransId = T2.TransId left join OINV T3 on t3.DocEntry = t1.SrcObjAbs and t3.ObjType = t1.SrcObjTyp WHERE t3.Project = '{Projekt}' and ReconType = '0' and CAST(YEAR(t0.ReconDate) AS varchar(5)) + ' - ' + RIGHT(Replicate('0', 2) + CAST(DATEPART(wk, t0.ReconDate) AS varchar(5)), 2) <= y.Week" +
                    

        
                    $") z ) as 'Uhrady'" +
                    "" +
                    "" +
                    " from #TempTabPoptavky y DROP TABLE #TempTabPoptavky");
                sql.Append("");
                sql.Append("");




                

            

            

            






                //log.Debug($"Nacteni dat pri importu artiklu z SAP {sql.ToString()}");
                SqlConnection cnn = new SqlConnection(connectionString);
                //SqlConnection con = new SqlConnection(cnn);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = sql.ToString();
                cnn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { log.Error("Error number: DocEntry" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //MAKES IT HERE   
                    while (dr.Read())
                    {
                        CashList item = new CashList();
                        try
                        {
                            item.Week = dr.GetString(dr.GetOrdinal("Week"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: DocEntry" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.PlanovaneVynosy = dr.GetDecimal(dr.GetOrdinal("PlanovaneVynosy"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: Docnum" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.PlanovaneNaklady = dr.GetDecimal(dr.GetOrdinal("PlanovaneNaklady"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: Cancelled" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.SkutecneNaklady = dr.GetDecimal(dr.GetOrdinal("SkutecneNaklady"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: DocStatus" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.SkutecneVynosy = dr.GetDecimal(dr.GetOrdinal("SkutecneVynosy"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: CardCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Uhrady = dr.GetDecimal(dr.GetOrdinal("Uhrady"));
                        }
                        catch (Exception ex)
                        { //log.Error("Error number: CardCode" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        item.PlanovanyZisk = item.PlanovaneVynosy - item.PlanovaneNaklady;
                        item.SkutecnyZisk = item.SkutecneVynosy - item.SkutecneNaklady;
                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;


            }

        }
        public String[] Week {
            get
            {
                string[] array = CashList.Select(t => t.Week).ToArray();
                return array;
            }
        }
        public decimal[] Uhrady
        {
            get
            {
                decimal[] array = CashList.Select(t => t.Uhrady).ToArray();
                return array;
            }
        }
        public decimal[] PlanovaneVynosy {
            get
            {
                decimal[] array = CashList.Select(t => t.PlanovaneVynosy).ToArray();
                return array;
            }
        }
        public decimal[] PlanovaneNaklady {
            get
            {
                decimal[] array = CashList.Select(t => t.PlanovaneNaklady).ToArray();
                 return array;
            }
        }
        public decimal[] PlanovanyZisk {
            get
            {
                decimal[] array = CashList.Select(t => t.PlanovanyZisk).ToArray();
                return array;
            }
        }
        public decimal[] SkutecneNaklady {
            get
            {
                decimal[] array = CashList.Select(t => t.SkutecneNaklady).ToArray();
                return array;
            }
        }
        public decimal[] SkutecneVynosy {
            get
            {
                decimal[] array = CashList.Select(t => t.SkutecneVynosy).ToArray();
                return array;
            }
        }
        public decimal[] SkutecnyZisk {
            get
            {
                decimal[] array = CashList.Select(t => t.SkutecnyZisk).ToArray();
                return array;
            }
        }



        

    }

    public class CashList
    {
        public string Week { get; set; }
        public decimal PlanovaneVynosy { get; set; }
        public decimal PlanovaneNaklady { get; set; }
        public decimal PlanovanyZisk { get; set; }

        public decimal SkutecneNaklady { get; set; }
        public decimal SkutecneVynosy { get; set; }
        public decimal SkutecnyZisk { get; set; }

        public decimal Uhrady { get; set; }

    }

    public class SAPORDR
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPORDR");
        public int DocEntry { get; set; }
        public int Docnum { get; set; }
        public string Canceled { get; set; }
        public string DocStatus { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumFC { get; set; }
        public string DocCur { get; set; }
        public decimal DocRate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocTotalFC { get; set; }
        public decimal GrosProfit { get; set; }
        public decimal GrosProfFC { get; set; }
        public decimal CelkemBezDane { get { return DocTotal - VatSum; } }
        public decimal CelkemBezDaneFC { get { return DocTotalFC - VatSumFC; } }
        public decimal PrcZisku { get { var x = CelkemBezDane / DocTotal * 100; return x; } }
        public int SlpCode { get; set; }
        public string SLpName { get; set; }
        public virtual List<SAPRDR1> RDR1
        {
            get
            {

                List<SAPRDR1> list = new List<SAPRDR1>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select * from RDR1 where DocEntry = '{this.DocEntry}'");


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
                        SAPRDR1 item = new SAPRDR1();
                        try
                        {
                            item.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                        }
                        catch (Exception ex) { //log.Error("Error number: ItemCode " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Dscription = dr.GetString(dr.GetOrdinal("Dscription"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Dscription" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Quantity = dr.GetDecimal(dr.GetOrdinal("Quantity"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Quantity" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Price = dr.GetDecimal(dr.GetOrdinal("Price"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Price" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Currency = dr.GetString(dr.GetOrdinal("Currency"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Currency" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DiscPrcnt = dr.GetDecimal(dr.GetOrdinal("DiscPrcnt"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DiscPrcnt" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.LineTotal = dr.GetDecimal(dr.GetOrdinal("LineTotal"));
                        }
                        catch (Exception ex) { //log.Error("Error number: LineTotal" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.TotalFrgn = dr.GetDecimal(dr.GetOrdinal("TotalFrgn"));
                        }
                        catch (Exception ex) { //log.Error("Error number: TotalFrgn" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrossBuyPr = dr.GetDecimal(dr.GetOrdinal("GrossBuyPr"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrossBuyPr" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrssProfit = dr.GetDecimal(dr.GetOrdinal("GrssProfit"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrssProfit" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrssProfFC = dr.GetDecimal(dr.GetOrdinal("GrssProfFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrssProfFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.GrssProfSC = dr.GetDecimal(dr.GetOrdinal("GrssProfSC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrssProfSC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }


                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;
            }
        }
    }

    public class SAPOINV
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPOINV");
        public int DocEntry { get; set; }
        public int Docnum { get; set; }
        public string Canceled { get; set; }
        public string DocStatus { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumFC { get; set; }
        public string DocCur { get; set; }
        public decimal DocRate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocTotalFC { get; set; }
        public decimal GrosProfit { get; set; }
        public decimal GrosProfFC { get; set; }
        public decimal CelkemBezDane { get { return DocTotal - VatSum; } }
        public decimal CelkemBezDaneFC { get { return DocTotalFC - VatSumFC; } }
        public decimal PrcZisku { get
            {
                decimal x;
                if (DocTotal != 0)
                {
                    x = CelkemBezDane / DocTotal * 100;
                }
                else { x = 0; }
                return x; } }
        public int SlpCode { get; set; }
        public string SLpName { get; set; }
        public virtual List<SAPINV1> INV1
        {
            get
            {

                List<SAPINV1> list = new List<SAPINV1>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select * from INV1 where DocEntry = '{this.DocEntry}'");


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
                        SAPINV1 item = new SAPINV1();
                        try
                        {
                            item.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                        }
                        catch (Exception ex) {// log.Error("Error number: ItemCode " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Dscription = dr.GetString(dr.GetOrdinal("Dscription"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Dscription" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Quantity = dr.GetDecimal(dr.GetOrdinal("Quantity"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Quantity" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Price = dr.GetDecimal(dr.GetOrdinal("Price"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Price" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Currency = dr.GetString(dr.GetOrdinal("Currency"));
                        }
                        catch (Exception ex) {// log.Error("Error number: Currency" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DiscPrcnt = dr.GetDecimal(dr.GetOrdinal("DiscPrcnt"));
                        }
                        catch (Exception ex) {// log.Error("Error number: DiscPrcnt" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.LineTotal = dr.GetDecimal(dr.GetOrdinal("LineTotal"));
                        }
                        catch (Exception ex) {// log.Error("Error number: LineTotal" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.TotalFrgn = dr.GetDecimal(dr.GetOrdinal("TotalFrgn"));
                        }
                        catch (Exception ex) { //log.Error("Error number: TotalFrgn" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.GrossBuyPr = dr.GetDecimal(dr.GetOrdinal("GrossBuyPr"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrossBuyPr" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrssProfit = dr.GetDecimal(dr.GetOrdinal("GrssProfit"));
                        }
                        catch (Exception ex) {// log.Error("Error number: GrssProfit" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.GrssProfFC = dr.GetDecimal(dr.GetOrdinal("GrssProfFC"));
                        }
                        catch (Exception ex) {// log.Error("Error number: GrssProfFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.GrssProfSC = dr.GetDecimal(dr.GetOrdinal("GrssProfSC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrssProfSC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }


                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;
            }
        }
    }

    public class SAPOPCH
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPOPCH");
        public int DocEntry { get; set; }
        public int Docnum { get; set; }
        public string Canceled { get; set; }
        public string DocStatus { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal VatSum { get; set; }
        public decimal VatSumFC { get; set; }
        public string DocCur { get; set; }
        public decimal DocRate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocTotalFC { get; set; }
        public decimal GrosProfit { get; set; }
        public decimal GrosProfFC { get; set; }
        public decimal CelkemBezDane { get { return DocTotal - VatSum; } }
        public decimal CelkemBezDaneFC { get { return DocTotalFC - VatSumFC; } }
        public decimal PrcZisku { get { var x = CelkemBezDane / DocTotal * 100; return x; } }
        public int SlpCode { get; set; }
        public string SLpName { get; set; }
        public virtual List<SAPPCH1> PCH1
        {
            get
            {

                List<SAPPCH1> list = new List<SAPPCH1>();

                string connectionString = ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
                StringBuilder sql = new StringBuilder();

                sql.Append($" select * from PCH1 where DocEntry = '{this.DocEntry}'");


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
                        SAPPCH1 item = new SAPPCH1();
                        try
                        {
                            item.ItemCode = dr.GetString(dr.GetOrdinal("ItemCode"));
                        }
                        catch (Exception ex) { //log.Error("Error number: ItemCode " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Dscription = dr.GetString(dr.GetOrdinal("Dscription"));
                        }
                        catch (Exception ex) {// log.Error("Error number: Dscription" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Quantity = dr.GetDecimal(dr.GetOrdinal("Quantity"));
                        }
                        catch (Exception ex) { //log.Error("Error number: Quantity" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.Price = dr.GetDecimal(dr.GetOrdinal("Price"));
                        }
                        catch (Exception ex) {// log.Error("Error number: Price" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.Currency = dr.GetString(dr.GetOrdinal("Currency"));
                        }
                        catch (Exception ex) {// log.Error("Error number: Currency" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.DiscPrcnt = dr.GetDecimal(dr.GetOrdinal("DiscPrcnt"));
                        }
                        catch (Exception ex) { //log.Error("Error number: DiscPrcnt" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.LineTotal = dr.GetDecimal(dr.GetOrdinal("LineTotal"));
                        }
                        catch (Exception ex) { //log.Error("Error number: LineTotal" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.TotalFrgn = dr.GetDecimal(dr.GetOrdinal("TotalFrgn"));
                        }
                        catch (Exception ex) {// log.Error("Error number: TotalFrgn" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrossBuyPr = dr.GetDecimal(dr.GetOrdinal("GrossBuyPr"));
                        }
                        catch (Exception ex) {// log.Error("Error number: GrossBuyPr" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrssProfit = dr.GetDecimal(dr.GetOrdinal("GrssProfit"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrssProfit" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); 
                        }
                        try
                        {
                            item.GrssProfFC = dr.GetDecimal(dr.GetOrdinal("GrssProfFC"));
                        }
                        catch (Exception ex) { //log.Error("Error number: GrssProfFC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }
                        try
                        {
                            item.GrssProfSC = dr.GetDecimal(dr.GetOrdinal("GrssProfSC"));
                        }
                        catch (Exception ex) {// log.Error("Error number: GrssProfSC" + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                        }


                        list.Add(item);
                    }
                }
                cnn.Close();
                return list;
            }
        }
    }

    public class SAPRDR1
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TotalFrgn { get; set; }
        public decimal GrossBuyPr { get; set; }
        public decimal GrssProfit { get; set; }
        public decimal GrssProfFC { get; set; }
        public decimal GrssProfSC { get; set; }
        public decimal PrcZisku { get { var x = GrssProfit / LineTotal * 100; return x; } }
    }

    public class SAPINV1
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TotalFrgn { get; set; }
        public decimal GrossBuyPr { get; set; }
        public decimal GrssProfit { get; set; }
        public decimal GrssProfFC { get; set; }
        public decimal GrssProfSC { get; set; }
        public decimal PrcZisku { get
            { decimal x;
                if (LineTotal != 0)
                {
                    x = GrssProfit / LineTotal * 100;
                }
                else
                {
                    x = 0;
                }
                return x; } }
    }

    public class SAPPCH1
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal DiscPrcnt { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TotalFrgn { get; set; }
        public decimal GrossBuyPr { get; set; }
        public decimal GrssProfit { get; set; }
        public decimal GrssProfFC { get; set; }
        public decimal GrssProfSC { get; set; }
        public decimal PrcZisku { get { var x = GrssProfit / LineTotal * 100; return x; } }
    }
}