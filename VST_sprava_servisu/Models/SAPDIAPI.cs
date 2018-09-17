using System;
using System.Collections.Generic;
using System.Configuration;
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
            string SAP_dtb = ConfigurationManager.ConnectionStrings["SAP_dtb"].ConnectionString;
            string RS_dtb = ConfigurationManager.ConnectionStrings["RS_dtb"].ConnectionString;

            try
            {
                Company oCompany = new Company
                {
                    CompanyDB = SAP_dtb,
                    Server = "SQL",
                    LicenseServer = "SQL:30000",
                    DbUserName = "sa",
                    DbPassword = "*2012Versino",
                    UserName = "novakp",
                    Password = "Celtic.13",
                    DbServerType = BoDataServerTypes.dst_MSSQL2008,
                    UseTrusted = false,
                };
                
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

                string docEntry = "";
                int retVal = -1;

                Company oCompany = new Company();
                oCompany = SAPDIAPI.Connect();

                //Check connection before updating          

                if (oCompany.Connected)
                {
                    Documents oDraft = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDrafts);

                    oDraft.CardCode = sz.Zakaznik.KodSAP;
                    oDraft.DocDate = DateTime.Now;
                    oDraft.DocDueDate = DateTime.Now;
                    oDraft.TaxDate = DateTime.Now;
                    oDraft.VatDate = DateTime.Now;
                    oDraft.UserFields.Fields.Item("U_VCZ_R014").Value = "SC";
                    oDraft.UserFields.Fields.Item("U_VCZ_P343").Value = "S";
                    oDraft.UserFields.Fields.Item("U_VST_Oppor").Value = "100";
                    oDraft.DocumentsOwner = 61;
                    oDraft.SalesPersonCode = 47;
                    oDraft.DocType = BoDocumentTypes.dDocument_Items;
                    oDraft.DocumentSubType = BoDocumentSubType.bod_None;
                    oDraft.DocObjectCode = BoObjectTypes.oDeliveryNotes;
                    oDraft.Project = sz.Projekt;

                    /*
                    oDelivery.SpecialLines.LineType = BoDocSpecialLineType.dslt_Text;
                    oDelivery.SpecialLines.LineText = "úvodník";
                    oDelivery.SpecialLines.Add();
                    */


                    foreach (var item in sz.ServisniZasahPrvek.Where(t => t.ArtiklID != null))
                    {
                        /*
                        oDelivery.SpecialLines.LineType = BoDocSpecialLineType.dslt_Text;
                        // oDelivery.SpecialLines.AfterLineNumber = 0;
                        oDelivery.SpecialLines.LineText = "Položka";
                        oDelivery.SpecialLines.Add();
                        */

                        var artikl = Artikl.GetArtiklById(item.ArtiklID.Value);
                        oDraft.Lines.ItemCode = artikl.KodSAP;
                        oDraft.Lines.Quantity = Convert.ToDouble(item.Pocet);
                        oDraft.Lines.Price = Convert.ToDouble(item.CenaZaKus);
                        oDraft.Lines.WarehouseCode = "Servis";
                        oDraft.Lines.CostingCode = "OB";
                        oDraft.Lines.COGSCostingCode = "OB";
                        oDraft.Lines.LineTotal = Convert.ToDouble(item.CenaCelkem);
                        oDraft.Lines.ProjectCode = sz.Projekt;
                        oDraft.Lines.UnitsOfMeasurment = 1;
                        oDraft.Lines.Add();
                    }
                    /*KM*/
                    if (sz.CestaCelkem > 0)
                    {
                        oDraft.Lines.ItemCode = "SP02";
                        oDraft.Lines.Quantity = Convert.ToDouble(sz.Km);
                        oDraft.Lines.Price = Convert.ToDouble(sz.CestaCelkem / sz.Km);
                        oDraft.Lines.WarehouseCode = "Servis";
                        oDraft.Lines.CostingCode = "OB";
                        oDraft.Lines.COGSCostingCode = "OB";
                        oDraft.Lines.LineTotal = Convert.ToDouble(sz.CestaCelkem);
                        oDraft.Lines.ProjectCode = sz.Projekt;
                        oDraft.Lines.UnitsOfMeasurment = 1;
                        oDraft.Lines.Add();
                    }
                    /*PRACE*/
                    if (sz.PraceCelkem > 0)
                    {
                        oDraft.Lines.ItemCode = "SP01";
                        oDraft.Lines.Quantity = Convert.ToDouble(sz.PraceHod * sz.Pracelidi);
                        oDraft.Lines.Price = Convert.ToDouble(sz.PraceHod);
                        oDraft.Lines.WarehouseCode = "Servis";
                        oDraft.Lines.CostingCode = "OB";
                        oDraft.Lines.COGSCostingCode = "OB";
                        oDraft.Lines.LineTotal = Convert.ToDouble(sz.PraceCelkem);
                        oDraft.Lines.ProjectCode = sz.Projekt;
                        oDraft.Lines.UnitsOfMeasurment = 1;
                        oDraft.Lines.Add();
                    }




                    try
                    {

                        retVal = oDraft.Add();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    if (retVal == 0)
                    {
                        oCompany.GetNewObjectCode(out docEntry);
                    }


                    var x = oCompany.GetLastErrorCode();
                    var y = oCompany.GetLastErrorDescription();


                    if (retVal == 0)
                    {
                        oCompany.GetNewObjectCode(out docEntry);
                    }

                    Documents oDraft2 = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDrafts);
                    oDraft2.GetByKey(Convert.ToInt32(docEntry));
                    int iLine = 0;

                    while (iLine < oDraft2.Lines.Count)

                    {
                        int iLine2 = 0;
                        oDraft2.Lines.SetCurrentLine(iLine);
                        while (iLine2 < oDraft2.Lines.Count)
                        {
                            if (oDraft2.Lines.LineNum == System.Convert.ToInt32(iLine2))

                            {
                                oDraft2.Lines.SetCurrentLine(iLine2);

                                log.Debug(oDraft2.Lines.LineNum + " - " + oDraft2.Lines.ItemCode + " - " + oDraft2.Lines.CostingCode + " - " + oDraft2.Lines.ProjectCode);
                                //oDraft2.Lines. = iLine2;
                                oDraft2.Lines.CostingCode = "OB";
                                oDraft2.Lines.COGSCostingCode = "OB";
                                oDraft2.Lines.ProjectCode = sz.Projekt;
                                log.Debug(oDraft2.Lines.LineNum + " - " + oDraft2.Lines.ItemCode + " - " + oDraft2.Lines.CostingCode + " - " + oDraft2.Lines.ProjectCode);
                            }
                            iLine2++;
                        }

                        iLine++;

                    }

                    try
                    {

                        retVal = oDraft2.Update();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                    }
                    if (retVal == 0)
                    {
                        oCompany.GetNewObjectCode(out docEntry);
                    }

                    x = oCompany.GetLastErrorCode();
                    y = oCompany.GetLastErrorDescription();


                    //oCompany.GetLastError(out ErrCode, out ErrMsg);

                }
                else
                { log.Error("else on connected to sap"); }

                oCompany.Disconnect();

                return docEntry;

            }

        [Authorize(Roles = "Administrator,Manager")]
        internal protected static string GenerateQuotation(int Id)

        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
            
            string docEntry = "";
            int retVal = -1;
            
            Company oCompany = new Company();
            oCompany = SAPDIAPI.Connect();
            
            //Check connection before updating          

            if (oCompany.Connected)
            {
                Documents oDraft = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDrafts);

                oDraft.CardCode = sz.Zakaznik.KodSAP;
                oDraft.DocDate = DateTime.Now;
                oDraft.DocDueDate = DateTime.Now;
                oDraft.TaxDate = DateTime.Now;
                oDraft.VatDate = DateTime.Now;
                oDraft.UserFields.Fields.Item("U_VCZ_R014").Value = "KC";
                oDraft.UserFields.Fields.Item("U_VCZ_P343").Value = "K";
                oDraft.UserFields.Fields.Item("U_VST_Oppor").Value = "100";
                oDraft.TransportationCode = 5;
                oDraft.DocumentsOwner = 61;
                oDraft.SalesPersonCode = 47;
                oDraft.DocType = BoDocumentTypes.dDocument_Items;
                oDraft.DocumentSubType = BoDocumentSubType.bod_None;
                oDraft.DocObjectCode = BoObjectTypes.oQuotations;
                oDraft.Project = sz.Projekt;

                /*
                oDelivery.SpecialLines.LineType = BoDocSpecialLineType.dslt_Text;
                oDelivery.SpecialLines.LineText = "úvodník";
                oDelivery.SpecialLines.Add();
                */


                foreach (var item in sz.ServisniZasahPrvek.Where(t=>t.ArtiklID !=null && (t.Reklamace == false || t.Reklamace == true && t.PoruseniZarucnichPodminek == true)))
                {
                    /*
                    oDelivery.SpecialLines.LineType = BoDocSpecialLineType.dslt_Text;
                    // oDelivery.SpecialLines.AfterLineNumber = 0;
                    oDelivery.SpecialLines.LineText = "Položka";
                    oDelivery.SpecialLines.Add();
                    */

                    var artikl = Artikl.GetArtiklById(item.ArtiklID.Value);
                    oDraft.Lines.ItemCode = artikl.KodSAP;
                    oDraft.Lines.Quantity = Convert.ToDouble(item.Pocet);
                    oDraft.Lines.Price = Convert.ToDouble(item.CenaZaKus);
                    oDraft.Lines.WarehouseCode = "Servis";
                    oDraft.Lines.CostingCode = "OB";
                    oDraft.Lines.COGSCostingCode = "OB";
                    oDraft.Lines.LineTotal = Convert.ToDouble(item.CenaCelkem);
                    oDraft.Lines.ProjectCode = sz.Projekt;
                    oDraft.Lines.UnitsOfMeasurment = 1;
                    oDraft.Lines.Add();
                }
                /*KM*/
                if (sz.Reklamace == false || (sz.Reklamace == true && sz.PoruseniZarucnichPodminek == true))
                {
                    if (sz.CestaCelkem > 0)
                    {
                        oDraft.Lines.ItemCode = "SP02";
                        oDraft.Lines.Quantity = Convert.ToDouble(sz.Km);
                        oDraft.Lines.Price = Convert.ToDouble(sz.CestaCelkem / sz.Km);
                        oDraft.Lines.WarehouseCode = "Servis";
                        oDraft.Lines.CostingCode = "OB";
                        oDraft.Lines.COGSCostingCode = "OB";
                        oDraft.Lines.LineTotal = Convert.ToDouble(sz.CestaCelkem);
                        oDraft.Lines.ProjectCode = sz.Projekt;
                        oDraft.Lines.UnitsOfMeasurment = 1;
                        oDraft.Lines.GrossBuyPrice = 5;
                        oDraft.Lines.Add();
                    }
                    /*PRACE*/
                    if (sz.PraceCelkem > 0)
                    {
                        oDraft.Lines.ItemCode = "SP01";
                        oDraft.Lines.Quantity = Convert.ToDouble(sz.PraceHod * sz.Pracelidi);
                        oDraft.Lines.Price = Convert.ToDouble(sz.PraceHod);
                        oDraft.Lines.WarehouseCode = "Servis";
                        oDraft.Lines.CostingCode = "OB";
                        oDraft.Lines.COGSCostingCode = "OB";
                        oDraft.Lines.LineTotal = Convert.ToDouble(sz.PraceCelkem);
                        oDraft.Lines.ProjectCode = sz.Projekt;
                        oDraft.Lines.UnitsOfMeasurment = 1;
                        oDraft.Lines.Add();
                    }
                }

                

                try
                {

                    retVal = oDraft.Add();
                }
                catch (Exception ex)
                {
                    log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                if (retVal == 0)
                {
                    oCompany.GetNewObjectCode(out docEntry);
                }


                var x = oCompany.GetLastErrorCode();
                var y = oCompany.GetLastErrorDescription();

                
                if (retVal == 0)
                {
                    oCompany.GetNewObjectCode(out docEntry);
                }

                Documents oDraft2 = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDrafts);
                oDraft2.GetByKey(Convert.ToInt32(docEntry));
                int iLine = 0;

                while (iLine < oDraft2.Lines.Count)

                {
                    int iLine2 = 0;
                    oDraft2.Lines.SetCurrentLine(iLine);
                    while (iLine2 < oDraft2.Lines.Count)
                    {
                        if (oDraft2.Lines.LineNum == System.Convert.ToInt32(iLine2))

                        {
                            oDraft2.Lines.SetCurrentLine(iLine2);
                            
                            log.Debug(oDraft2.Lines.LineNum + " - " + oDraft2.Lines.ItemCode + " - " + oDraft2.Lines.CostingCode + " - " + oDraft2.Lines.ProjectCode);
                            //oDraft2.Lines. = iLine2;
                            oDraft2.Lines.CostingCode = "OB";
                            oDraft2.Lines.COGSCostingCode = "OB";
                            oDraft2.Lines.ProjectCode = sz.Projekt;
                            log.Debug(oDraft2.Lines.LineNum + " - " + oDraft2.Lines.ItemCode + " - " + oDraft2.Lines.CostingCode + " - " + oDraft2.Lines.ProjectCode);
                        }
                        iLine2++;
                    }

                    iLine++;

                }

                try
                {

                    retVal = oDraft2.Update();
                }
                catch (Exception ex)
                {
                    log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException);
                }
                if (retVal == 0)
                {
                    oCompany.GetNewObjectCode(out docEntry);
                }
                
                 x = oCompany.GetLastErrorCode();
                 y = oCompany.GetLastErrorDescription();


                //oCompany.GetLastError(out ErrCode, out ErrMsg);

            }
            else
            { log.Error("else on connected to sap"); }
            
            oCompany.Disconnect();
            
            return docEntry;

        }


        [Authorize(Roles = "Administrator,Manager")]
        internal protected static string GenerateOrder(int Id)

        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);

            //bool bRetVal = false;
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
                    oDelivery.Lines.CostingCode = "OB";
                    oDelivery.Lines.COGSCostingCode = "OB";
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
                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

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