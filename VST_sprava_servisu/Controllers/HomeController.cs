using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;


namespace VST_sprava_servisu.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Administrator,Manager,Uživatel")]
        public ActionResult Index()
        {
            string[] args = { "fdsfdsfsd", "fdfs" };
            //Main(args);
            //Main2(args);

            /*try
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
                if (oCompany.Connected == true) { oCompany.Disconnect(); }
            }
            catch (Exception Errmsg) { throw Errmsg; }
            */



            return RedirectToAction("Nahled","Revize",null);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        static void SAP()
        {
            

        }

        static void Main(string[] args)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";
            // Create an empty page
            PdfPage page = document.AddPage();
            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            // Create a font
            XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
            // Draw the text
            gfx.DrawString("Hello, World!", font, XBrushes.Black,
            new XRect(0, 0, page.Width, page.Height),
            XStringFormats.Center);
            XImage image = XImage.FromFile(@"c:\users\novakp\documents\visual studio 2017\Projects\VST_sprava_servisu\VST_sprava_servisu\Content\header.png");
            // Left position in point
            double x = (250 - image.PixelWidth * 72 / image.HorizontalResolution) / 2;
            gfx.DrawImage(image, x, 0);
            //XImage image = XImage.FromFile();
            //gfx.DrawImage(image, 50, 50, 200, 100);
            // Save the document...
            const string filename = "HelloWorld.pdf";
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }
        void DrawImageScaled(XGraphics gfx, int number)
        {
        
        
        

}



    static void Main2(string[] args)
        {
            PdfDocument document = new PdfDocument();
            // Sample uses DIN A4, page height is 29.7 cm. We use margins of 2.5 cm.
            LayoutHelper helper = new LayoutHelper(document, XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(29.7 - 2.5));
            XUnit left = XUnit.FromCentimeter(2.5);
            // Random generator with seed value, so created document will always be the same.
            Random rand = new Random(42);
            const int headerFontSize = 20;
            const int normalFontSize = 10;
            XFont fontHeader = new XFont("Verdana", headerFontSize, XFontStyle.BoldItalic);
            XFont fontNormal = new XFont("Verdana", normalFontSize, XFontStyle.Regular);
            const int totalLines = 666;
            bool washeader = false;
            for (int line = 0; line < totalLines; ++line)
                {
                bool isHeader = line == 0 || !washeader && line < totalLines - 1 && rand.Next(15) == 0;
                washeader = isHeader;
                // We do not want a single header at the bottom of the page, so if we have a header we require space for header and a normal text line.
                XUnit top = helper.GetLinePosition(isHeader ? headerFontSize + 5 : normalFontSize + 2, isHeader ? headerFontSize + 5 + normalFontSize : normalFontSize);
                helper.Gfx.DrawString(isHeader ? "Sed massa libero, semper a nisi nec" : "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                isHeader ? fontHeader : fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
                }
            // Save the document...

            const string filename = "MultiplePages.pdf";
            document.Save(filename);
            // ...and start a viewer.

            Process.Start(filename);
        }

        


}

    public class LayoutHelper

{

private readonly PdfDocument _document;

private readonly XUnit _topPosition;

private readonly XUnit _bottomMargin;

private XUnit _currentPosition;

 

public LayoutHelper(PdfDocument document, XUnit topPosition, XUnit bottomMargin)

{

_document = document;

_topPosition = topPosition;

_bottomMargin = bottomMargin;

// Set a value outside the page - a new page will be created on the first request.

_currentPosition = bottomMargin + 10000;

}

 

public XUnit GetLinePosition(XUnit requestedHeight)

{

return GetLinePosition(requestedHeight, -1f);

}

 

public XUnit GetLinePosition(XUnit requestedHeight, XUnit requiredHeight)

{

XUnit required = requiredHeight == -1f ? requestedHeight : requiredHeight;

if (_currentPosition + required > _bottomMargin)

CreatePage();

XUnit result = _currentPosition;

_currentPosition += requestedHeight;

return result;

}

 

public XGraphics Gfx { get; private set; }

public PdfPage Page { get; private set; }

 

void CreatePage()

{

Page = _document.AddPage();

Page.Size = PageSize.A4;

Gfx = XGraphics.FromPdfPage(Page);

_currentPosition = _topPosition;

}

}
}