using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace WSUrlDownloadPDFs
{
    public class Service1 : IService1
    {
        ///<summary>
        ///Descarga PDFs contenidos dentro de sitio web
        ///</summary>
        ///<return>
        ///void - (Se descargan archivos PDF en "ruta" dada)
        ///</return>
        ///<param name="URL">
        ///Dirección desde donde se captura código HTML (ej: http://192.321.315.09/T/Service/Default.aspx?p=12311)
        ///</param>
        ///<param name="selectHREFContains">
        ///Filtro que permite seleccionar HREF específicos dentro del HTML
        ///</param>
        ///<param name="ruta">
        ///Ruta donde se descargan los PDFs (ej: c:\\newfolder\\)
        ///</param>
        public void getPDFs(string URL, string selectHREFContains, string ruta)
        {
            List<String> links;
            MatchCollection matches;
            WebClient w = new WebClient();
            string capHTML, filterHREF;

            filterHREF = "href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))";
            try
            {
                capHTML = w.DownloadString(URL);
                matches = Regex.Matches(capHTML, filterHREF);

                links = matches.Cast<Match>()
                        .Select(m => m.Groups[0].Value)
                        .Where(f => f.Contains(selectHREFContains))
                        .ToList();

                DownloadAllPDFs(links, w, ruta, URL);
            }
            catch (Exception ex) { }
        }

        ///<summary>
        ///Proceso de descarga de PDFs desde HTML
        ///</summary>
        ///<return>
        ///void
        ///</return>
        ///<param name="links">
        ///Lista de String que guarda los HREF capturados
        ///</param>
        ///<param name="w">
        ///WebClient que permite la descarga del HTML y de los archivos PDF
        ///</param>
        ///<param name="ruta">
        ///Ruta donde se descargan los PDFs
        ///</param>
        private void DownloadAllPDFs(List<String> links, WebClient w, string ruta, string URL)
        {
            DirectoryInfo di;
            String fullPath, pathPaciente, rut, capFileField;

            rut = CaptureRut(URL);
            pathPaciente = ruta + rut + "\\";

            try
            {
                if (!Directory.Exists(ruta))
                    di = Directory.CreateDirectory(ruta);

                if (!Directory.Exists(pathPaciente))
                    di = Directory.CreateDirectory(pathPaciente);

                foreach (var item in links)
                {
                    capFileField = CaptureFileFieldInUrl(item);
                    fullPath = pathPaciente + capFileField + ".pdf";
                    w.DownloadFile(HREFtoURL(item), fullPath);
                }
            }
            catch (Exception ex) { }
        }

        ///<summary>
        ///Convierte HREF en una URL
        ///</summary>
        ///<return>
        ///void
        ///</return>
        ///<param name="str">
        ///String correspondiente a HREF
        ///</param>        
        private string HREFtoURL(string str)
        {
            try
            {
                return str.Substring(6, str.Count() - 7);
            }
            catch (Exception)
            {
                return "";
            }

        }

        ///<summary>
        ///Captura RUT de paciente desde URL
        ///</summary>
        ///<return>
        ///string
        ///</return>
        ///<param name="URL">
        ///Dirección desde donde se captura código HTML (ej: http://192.321.315.09/INT/Service/Default.aspx?p=12311)
        ///</param>
        private string CaptureRut(string URL)
        {
            string rut = String.Empty;
            int initialPosition;
            try
            {
                initialPosition = URL.IndexOf("param1");
                rut = URL.Substring(initialPosition + 7);

                return rut;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        ///<summary>
        ///Captura campo FILE de URL de un PDF
        ///</summary>
        ///<return>
        ///string
        ///</return>
        ///<param name="UrlPDf">
        ///Dirección desde donde se captura código HTML (ej: http://192.321.315.09/abc/InfoemPDF?id=230269&idprestacion=1872&file=230269.1872.20161019211411)
        ///</param>
        private string CaptureFileFieldInUrl(string UrlPDF)
        {
            string field = String.Empty;
            int initialPosition;

            try
            {
                initialPosition = UrlPDF.IndexOf("file");
                field = UrlPDF.Substring(initialPosition + 5).Replace("'", "");

                return field;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
