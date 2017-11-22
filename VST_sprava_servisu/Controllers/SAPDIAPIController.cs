using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class SAPDIAPIController : Controller
    {
        // GET: SAPDIAPI
        public SAPbobsCOM.Company Connect()
        {
            try
            {
                SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
                oCompany.CompanyDB = "SBO_SKOLENI";
                oCompany.Server = "SQL";
                oCompany.LicenseServer = "SQL:30000";
                oCompany.DbUserName = "sa";
                oCompany.DbPassword = "*2012Versino";
                oCompany.UserName = "novakp";
                oCompany.Password = "Celtic.13";
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
                oCompany.UseTrusted = true;
                int ret = oCompany.Connect();
                string ErrMsg = oCompany.GetLastErrorDescription();
                int ErrNo = oCompany.GetLastErrorCode();
                if (ErrNo != 0)
                {
                    ViewBag.ErrMsg = ErrMsg;
                }
                else
                {
                    ViewBag.ErrMsg = "Connected succesfully to SAP Business One";
                }
                
                return oCompany;
            }
            catch (Exception Errmsg) { throw Errmsg; }


            
        }
        public bool Disconnect(SAPbobsCOM.Company oCompany)
        {
            if (oCompany.Connected == true) { oCompany.Disconnect(); }
            return true;
        }


        public bool UpdateContactName(string bpCardCode, string ctnPrevName, string CtnNewName)

        {
            bool bRetVal = false;
            string sErrMsg; int lErrCode;

            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            oCompany = Connect();
            
            //Check connection before updating          

            if (oCompany.Connected)

            {

                //create the BP object              

                SAPbobsCOM.BusinessPartners BP = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                SAPbobsCOM.ContactEmployees sboContacts = BP.ContactEmployees;

                try
                {
                    if (BP.GetByKey(bpCardCode))
                    {
                        sboContacts = BP.ContactEmployees;
                        //check first for one contact (always gives 1 wether have contact or not) check for no contact to add or update                      
                        if (sboContacts.Count > 0)
                        {
                            for (int i = 0; i < sboContacts.Count; i++)
                            {
                                sboContacts.SetCurrentLine(i);
                                if (sboContacts.Name == ctnPrevName)
                                {
                                    bRetVal = true;
                                    sboContacts.Name = CtnNewName;
                                }
                            }
                        }
                        if (bRetVal)
                        {
                            if (BP.Update() != 0)
                            {

                                bRetVal = false;

                                oCompany.GetLastError(out lErrCode, out sErrMsg);

                            }

                        }

                    }

                }

                catch (Exception ex)

                {

                    bRetVal = false;

                }

            }

            oCompany.Disconnect();

            return bRetVal;

        }










        /*END KONTR*/
        /////////////////
    }
}
        