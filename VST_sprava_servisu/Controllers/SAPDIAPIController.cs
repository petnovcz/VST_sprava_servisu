using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VST_sprava_servisu
{
    public class SAPDIAPIController : Controller
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: SAPDIAPI
        

        [Authorize(Roles = "Administrator,Manager")]
        public bool UpdateContactName(string bpCardCode, string ctnPrevName, string CtnNewName)

        {
            bool bRetVal = false;
            string sErrMsg; int lErrCode;

            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
            oCompany = SAPDIAPI.Connect();
            
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

                catch (Exception ex) { log.Error("Error number: " + ex.HResult + " - " + ex.Message + " - " + ex.Data + " - " + ex.InnerException); }

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
        