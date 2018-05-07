using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAPbobsCOM;

namespace VST_sprava_servisu
{
    public class SAPDIAPI
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("SAPDIAPI");

        [Authorize(Roles = "Administrator,Manager")]
        internal protected static Company Connect()
        {
            //Company oCompany = new Company();
            //SAPbobsCOM.Attachments2 oATT = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2) as SAPbobsCOM.Attachments2;
            

            try
            {
                Company oCompany = new Company();
                oCompany.CompanyDB = "SBO_TEST";
                oCompany.Server = "SQL";
                oCompany.LicenseServer = "SQL:30000";
                oCompany.DbUserName = "sa";
                oCompany.DbPassword = "*2012Versino";
                oCompany.UserName = "novakp";
                oCompany.Password = "Celtic.13";
                oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2008;
                oCompany.UseTrusted = false;
                int ret = oCompany.Connect();
                string ErrMsg = oCompany.GetLastErrorDescription();
                int ErrNo = oCompany.GetLastErrorCode();
                if (ErrNo != 0)
                {

                }
                else
                {

                }
                log.Error("connected - ErrMsg" + ErrMsg);
                return oCompany;
                
            }
            catch (Exception Errmsg) {
                log.Error("not connected to SAP via DI API");
                log.Error("Error number: " + Errmsg);
                throw Errmsg;
            }
        }

        [Authorize(Roles = "Administrator,Manager")]
        public bool Disconnect(Company oCompany)
        {
            if (oCompany.Connected == true) { oCompany.Disconnect(); }
            return true;
        }

        [Authorize(Roles = "Administrator,Manager")]
        internal protected static string GenerateDL(int Id)

        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
            var index = 0;
            var xx = sz.ServisniZasahPrvek.ToList();
            for (int i = 0; i < xx.Count; i++)
            {
                xx[i].startindex = index;
                xx[i].kusovnik_count = xx[i].RadkyKusovniku.Count;
                xx[i].endindex = index + xx[i].RadkyKusovniku.Count;
                index = xx[i].endindex + 1;
            }


            //var xx = sz.ServisniZasahPrvek.ToList();
            //var z = xx[0];

            bool bRetVal = false;
            string docEntry = "";
            int retVal = -1;
            string OrdDocEntry = sz.ZakazkaDocNUm;
            Company oCompany = new Company();
            oCompany = SAPDIAPI.Connect();

            // Get the quotation
            Documents oOrder = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oOrders);
            oOrder.GetByKey(Convert.ToInt32(sz.Zakazka));

            //Check connection before updating          

            if (oCompany.Connected)

            {
                Documents oDelivery = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
                oDelivery.CardCode = oOrder.CardCode;
                oDelivery.DocDate = DateTime.Now;
                oDelivery.DocDueDate = DateTime.Now;
                oDelivery.TaxDate = DateTime.Now;
                oDelivery.VatDate = DateTime.Now;
                oDelivery.DocumentsOwner = 61;
                oDelivery.SalesPersonCode = 47;
                oDelivery.DocType = BoDocumentTypes.dDocument_Items;
                oDelivery.DocumentSubType = BoDocumentSubType.bod_None;
                oDelivery.DocObjectCode = BoObjectTypes.oOrders;
                oDelivery.Project = sz.Projekt;
                var snindex = 0;
                for (int i = 0; i < oOrder.Lines.Count; i++)

                {
                    oOrder.Lines.SetCurrentLine(i);
                    oDelivery.Lines.BaseEntry = oOrder.DocEntry;
                    oDelivery.Lines.BaseLine = oOrder.Lines.LineNum;
                    oDelivery.Lines.BaseType = 17;
                    oDelivery.Lines.ItemCode = oOrder.Lines.ItemCode;
                    oDelivery.Lines.Quantity = oOrder.Lines.Quantity;
                    oDelivery.Lines.Price = oOrder.Lines.Price;
                    oDelivery.Lines.WarehouseCode = oOrder.Lines.WarehouseCode;
                    oDelivery.Lines.CostingCode = oOrder.Lines.CostingCode;
                    oDelivery.Lines.COGSCostingCode = oOrder.Lines.COGSCostingCode;
                    //oDelivery.Lines.Currency = sz.Mena;
                    oDelivery.Lines.LineTotal = oOrder.Lines.LineTotal;
                    oDelivery.Lines.ProjectCode = sz.Projekt;
                    //oDelivery.Lines.Rate = 1;
                    oDelivery.Lines.UnitsOfMeasurment = oOrder.Lines.UnitsOfMeasurment;
                    //oDelivery.Lines.TaxCode = "E21T";

                    foreach (var item in xx)
                    {
                        if (oOrder.Lines.LineNum > item.startindex && oOrder.Lines.LineNum <= item.endindex)
                        {
                            var z = item.RadkyKusovniku.ToList();
                            var ind = oOrder.Lines.LineNum - item.startindex;
                            var polozka = z[ind-1];

                            using (var db = new Model1Container())
                            {
                                var itemid = item.Id;
                                var kusovniksapkod = z[ind-1].KusovnikSAPKod;
                                var xxx = db.ServisniZasahPrvekSerioveCislo.Where(t => t.ServisniZasahPrvekId == itemid).Where(t=>t.SAPKod == oOrder.Lines.ItemCode).ToList();
                                foreach (var itemx in xxx)
                                {
                                    //oDelivery.Lines.SerialNumbers.SetCurrentLine(snindex);
                                    //snindex++;
                                    oDelivery.Lines.SerialNumbers.Quantity = 1;
                                    //oDelivery.Lines.SerialNumbers.BaseLineNumber = oOrder.Lines.LineNum;
                                    oDelivery.Lines.SerialNumbers.SystemSerialNumber = Convert.ToInt16(itemx.SysSerial);
                                    //oDelivery.Lines.SerialNumbers.
                                    oDelivery.Lines.SerialNumbers.Add();
                                }
                            }

                        }
                    }



                    oDelivery.Lines.Add();


                }


                try
                {
                    retVal = oDelivery.Add();
                }
                catch (Exception ex) { }

                if (retVal == 0)
                {

                    oCompany.GetNewObjectCode(out docEntry);
                }

                var x = oCompany.GetLastErrorCode();
                var y = oCompany.GetLastErrorDescription();
                //oCompany.GetLastError(out ErrCode, out ErrMsg);

            }

            oCompany.Disconnect();

            return docEntry;

        }

        [Authorize(Roles = "Administrator,Manager")]
        internal protected static string GenerateQuotation(int Id)

        {
            log.Error("1");
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
            bool bRetVal = false;
            string sErrMsg; int lErrCode;
            string docEntry = "";
            int retVal = -1;
            log.Error("2");
            Company oCompany = new Company();
            oCompany = SAPDIAPI.Connect();
            log.Error("3");
            //Check connection before updating          

            if (oCompany.Connected)

            {
                log.Error("3aa");
                Documents oDelivery = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oQuotations);
                log.Error("3a");
                oDelivery.CardCode = sz.Zakaznik.KodSAP;
                log.Error("3b");
                oDelivery.DocDate = DateTime.Now;
                log.Error("3c");
                oDelivery.DocDueDate = DateTime.Now;
                log.Error("3d");
                oDelivery.TaxDate = DateTime.Now;
                log.Error("3e");
                oDelivery.VatDate = DateTime.Now;
                log.Error("3f");
                oDelivery.UserFields.Fields.Item("U_VCZ_R014").Value = "SC";
                log.Error("3g");
                oDelivery.UserFields.Fields.Item("U_VCZ_P343").Value = "S";
                log.Error("3h");
                oDelivery.UserFields.Fields.Item("U_VST_Oppor").Value = "100";
                log.Error("3j");
                oDelivery.DocumentsOwner = 61;
                log.Error("3k");
                oDelivery.SalesPersonCode = 47;
                log.Error("3l");
                oDelivery.DocType = BoDocumentTypes.dDocument_Items;
                log.Error("3m");
                oDelivery.DocumentSubType = BoDocumentSubType.bod_None;
                log.Error("3n");
                oDelivery.DocObjectCode = BoObjectTypes.oQuotations;
                log.Error("3o");
                //oDelivery.DocObjectCodeEx = SAPbobsCOM.BoObjectTypes.oDeliveryNotes;
                //oDelivery.DocCurrency = sz.Mena;

                oDelivery.Project = sz.Projekt;
                log.Error("3p");

                log.Error("4");

                foreach (var item in sz.ServisniZasahPrvek)
                {

                    var artikl = Artikl.GetArtiklById(item.ArtiklID.Value);
                    oDelivery.Lines.ItemCode = artikl.KodSAP;
                    oDelivery.Lines.Quantity = Convert.ToDouble(item.Pocet);
                    oDelivery.Lines.Price = Convert.ToDouble(item.CenaZaKus);
                    oDelivery.Lines.WarehouseCode = "Servis";
                    oDelivery.Lines.CostingCode = "OB";
                    oDelivery.Lines.COGSCostingCode = "OB";
                    //oDelivery.Lines.Currency = sz.Mena;
                    oDelivery.Lines.LineTotal = Convert.ToDouble(item.CenaCelkem);
                    oDelivery.Lines.ProjectCode = sz.Projekt;
                    //oDelivery.Lines.Rate = 1;
                    oDelivery.Lines.UnitsOfMeasurment = 1;
                    //oDelivery.Lines.TaxCode = "E21T";
                    oDelivery.Lines.Add();
                }
                /*KM*/
                if (sz.CestaCelkem > 0)
                {
                    oDelivery.Lines.ItemCode = "SP02";
                    oDelivery.Lines.Quantity = Convert.ToDouble(sz.Km);
                    oDelivery.Lines.Price = Convert.ToDouble(sz.CestaCelkem / sz.Km);
                    oDelivery.Lines.WarehouseCode = "Servis";
                    oDelivery.Lines.CostingCode = "OB";
                    oDelivery.Lines.COGSCostingCode = "OB";
                    //oDelivery.Lines.Currency = sz.Mena;
                    oDelivery.Lines.LineTotal = Convert.ToDouble(sz.CestaCelkem);
                    oDelivery.Lines.ProjectCode = sz.Projekt;
                    //oDelivery.Lines.Rate = 1;
                    oDelivery.Lines.UnitsOfMeasurment = 1;
                    //oDelivery.Lines.TaxCode = "E21T";
                    oDelivery.Lines.Add();
                }
                /*PRACE*/
                if (sz.PraceCelkem > 0)
                {
                    oDelivery.Lines.ItemCode = "SP01";
                    oDelivery.Lines.Quantity = Convert.ToDouble(sz.PraceHod * sz.Pracelidi);
                    oDelivery.Lines.Price = Convert.ToDouble(sz.PraceHod);
                    oDelivery.Lines.WarehouseCode = "Servis";
                    oDelivery.Lines.CostingCode = "OB";
                    oDelivery.Lines.COGSCostingCode = "OB";
                    //oDelivery.Lines.Currency = sz.Mena;
                    oDelivery.Lines.LineTotal = Convert.ToDouble(sz.PraceCelkem);
                    oDelivery.Lines.ProjectCode = sz.Projekt;
                    //oDelivery.Lines.Rate = 1;
                    oDelivery.Lines.UnitsOfMeasurment = 1;
                    //oDelivery.Lines.TaxCode = "E21T";
                    oDelivery.Lines.Add();
                }
                try
                {
                    log.Error("5");
                    retVal = oDelivery.Add();
                }
                catch (Exception ex)
                {
                    log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }

                if (retVal == 0)
                {

                    oCompany.GetNewObjectCode(out docEntry);
                }





                log.Error("6");

                var x = oCompany.GetLastErrorCode();
                var y = oCompany.GetLastErrorDescription();
                //oCompany.GetLastError(out ErrCode, out ErrMsg);

            }
            else
            { log.Error("else on connected to sap"); }
            log.Error("7");
            oCompany.Disconnect();
            log.Error("8");
            return docEntry;

        }


        [Authorize(Roles = "Administrator,Manager")]
        internal protected static string GenerateOrder(int Id)

        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);

            bool bRetVal = false;
            string docEntry = "";
            int retVal = -1;
            string QuotDocEntry = sz.NabidkaDocNum;
            Company oCompany = new Company();
            oCompany = SAPDIAPI.Connect();

            // Get the quotation
            Documents oQuotation = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oQuotations);
            oQuotation.GetByKey(Convert.ToInt32(sz.Nabidka));

            //Check connection before updating          

            if (oCompany.Connected)

            {
                Documents oDelivery = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oOrders);
                oDelivery.CardCode = oQuotation.CardCode;
                oDelivery.DocDate = DateTime.Now;
                oDelivery.DocDueDate = DateTime.Now;
                oDelivery.TaxDate = DateTime.Now;
                oDelivery.VatDate = DateTime.Now;                
                oDelivery.DocumentsOwner = 61;
                oDelivery.SalesPersonCode = 47;
                oDelivery.DocType = BoDocumentTypes.dDocument_Items;
                oDelivery.DocumentSubType = BoDocumentSubType.bod_None;
                oDelivery.DocObjectCode = BoObjectTypes.oOrders;             
                oDelivery.Project = sz.Projekt;

                for (int i = 0; i < oQuotation.Lines.Count; i++)

                {
                    oQuotation.Lines.SetCurrentLine(i);
                    oDelivery.Lines.BaseEntry = oQuotation.DocEntry;
                    oDelivery.Lines.BaseLine = oQuotation.Lines.LineNum;
                    oDelivery.Lines.BaseType = 23;
                    oDelivery.Lines.ItemCode = oQuotation.Lines.ItemCode;
                    oDelivery.Lines.Quantity = oQuotation.Lines.Quantity;
                    oDelivery.Lines.Price = oQuotation.Lines.Price;
                    oDelivery.Lines.WarehouseCode = oQuotation.Lines.WarehouseCode;
                    oDelivery.Lines.CostingCode = oQuotation.Lines.CostingCode;
                    oDelivery.Lines.COGSCostingCode = oQuotation.Lines.COGSCostingCode;
                    //oDelivery.Lines.Currency = sz.Mena;
                    oDelivery.Lines.LineTotal = oQuotation.Lines.LineTotal;
                    oDelivery.Lines.ProjectCode = sz.Projekt;
                    //oDelivery.Lines.Rate = 1;
                    oDelivery.Lines.UnitsOfMeasurment = oQuotation.Lines.UnitsOfMeasurment;
                    //oDelivery.Lines.TaxCode = "E21T";
                    oDelivery.Lines.Add();


                }

                
                try
                {
                    retVal = oDelivery.Add();
                }
                catch (Exception ex) { }

                if (retVal == 0)
                {

                    oCompany.GetNewObjectCode(out docEntry);
                }







                var x = oCompany.GetLastErrorCode();
                var y = oCompany.GetLastErrorDescription();
                //oCompany.GetLastError(out ErrCode, out ErrMsg);

            }

            oCompany.Disconnect();

            return docEntry;

        }


        internal protected static string GenerateOrder(int Id, string Path)
        {
            var result = "";

            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
                        
            Company oCompany = new Company();
            oCompany = SAPDIAPI.Connect();

            // Get the quotation
            Documents oOrder = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oOrders);
            oOrder.GetByKey(Convert.ToInt32(sz.Zakazka));

            //Check connection before updating          

            if (oCompany.Connected)

            {
                
                
            }

            oCompany.Disconnect();
            return result;
        }


    }

    




    




    public class SAPDIAPI_CP
    {
        public int CntctCode { get; set; }



    }
}