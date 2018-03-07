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
                oCompany.UseTrusted = true;
                int ret = oCompany.Connect();
                string ErrMsg = oCompany.GetLastErrorDescription();
                int ErrNo = oCompany.GetLastErrorCode();
                if (ErrNo != 0)
                {

                }
                else
                {

                }

                return oCompany;
            }
            catch (Exception Errmsg) { throw Errmsg; }
        }

        [Authorize(Roles = "Administrator,Manager")]
        public bool Disconnect(Company oCompany)
        {
            if (oCompany.Connected == true) { oCompany.Disconnect(); }
            return true;
        }

        [Authorize(Roles = "Administrator,Manager")]
        internal protected static bool GenerateDL(int Id)

        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
            bool bRetVal = false;
            string sErrMsg; int lErrCode;

            Company oCompany = new Company();
            oCompany = SAPDIAPI.Connect();

            //Check connection before updating          

            if (oCompany.Connected)

            {
                Documents oDelivery = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
                oDelivery.CardCode = sz.Zakaznik.KodSAP;
                oDelivery.DocDate = DateTime.Now;
                oDelivery.DocDueDate = DateTime.Now;
                oDelivery.TaxDate = DateTime.Now;
                oDelivery.VatDate = DateTime.Now;
                oDelivery.DocType = BoDocumentTypes.dDocument_Items;
                oDelivery.DocumentSubType = BoDocumentSubType.bod_None;
                oDelivery.DocObjectCode = BoObjectTypes.oDeliveryNotes;
                //oDelivery.DocObjectCodeEx = SAPbobsCOM.BoObjectTypes.oDeliveryNotes;
                //oDelivery.DocCurrency = sz.Mena;

                oDelivery.Project = "RP00001";

                foreach (var item in sz.ServisniZasahPrvek)
                {
                    
                    var artikl = Artikl.GetArtiklById(item.ArtiklID.Value);                    
                    oDelivery.Lines.ItemCode = "SP01";
                    oDelivery.Lines.Quantity = Convert.ToDouble(item.Pocet);                    
                    oDelivery.Lines.Price = Convert.ToDouble(item.CenaZaKus);
                    oDelivery.Lines.WarehouseCode = "HLAVNI";
                    oDelivery.Lines.CostingCode = "OB";
                    oDelivery.Lines.COGSCostingCode = "OB";
                    oDelivery.Lines.Currency = "CZK";
                    oDelivery.Lines.LineTotal = 10000;
                    oDelivery.Lines.ProjectCode = "RP00001";
                    oDelivery.Lines.Rate = 1;
                    oDelivery.Lines.UnitsOfMeasurment = 1;
                    oDelivery.Lines.TaxCode = "E21T";
                    oDelivery.Lines.Add();
                }
                try
                {
                    int retVal = oDelivery.Add();
                }
                catch (Exception ex) { }
                var x = oCompany.GetLastErrorCode();
                var y = oCompany.GetLastErrorDescription();
                //oCompany.GetLastError(out ErrCode, out ErrMsg);

            }

            oCompany.Disconnect();

            return bRetVal;

        }

        [Authorize(Roles = "Administrator,Manager")]
        internal protected static string GenerateQuotation(int Id)

        {
            ServisniZasah sz = new ServisniZasah();
            sz = ServisniZasah.GetZasah(Id);
            bool bRetVal = false;
            string sErrMsg; int lErrCode;
            string docEntry = "";
            int retVal = -1;
            Company oCompany = new Company();
            oCompany = SAPDIAPI.Connect();

            //Check connection before updating          

            if (oCompany.Connected)

            {
                Documents oDelivery = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oQuotations);
                oDelivery.CardCode = sz.Zakaznik.KodSAP;
                oDelivery.DocDate = DateTime.Now;
                oDelivery.DocDueDate = DateTime.Now;
                oDelivery.TaxDate = DateTime.Now;
                oDelivery.VatDate = DateTime.Now;
                oDelivery.UserFields.Fields.Item("U_VCZ_R014").Value = "SC";
                oDelivery.UserFields.Fields.Item("U_VCZ_P343").Value = "S";
                oDelivery.UserFields.Fields.Item("U_VST_Oppor").Value = "100";
                oDelivery.DocumentsOwner = 61;
                oDelivery.SalesPersonCode = 47;
                oDelivery.DocType = BoDocumentTypes.dDocument_Items;
                oDelivery.DocumentSubType = BoDocumentSubType.bod_None;
                oDelivery.DocObjectCode = BoObjectTypes.oQuotations;
                //oDelivery.DocObjectCodeEx = SAPbobsCOM.BoObjectTypes.oDeliveryNotes;
                //oDelivery.DocCurrency = sz.Mena;

                oDelivery.Project = sz.Projekt;

                /*KM*/
                if (sz.CestaCelkem > 0)
                {
                    oDelivery.Lines.ItemCode = "SP05";
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
                try
                {
                    retVal = oDelivery.Add();
                }
                catch (Exception ex) { }

                if(retVal == 0)
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
                    oDelivery.Lines.ProjectCode = oQuotation.Lines.ProjectCode;
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