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
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código y en el archivo de configuración a la vez.
    public class Service1 : IService1
    {
        ///<summary>
        ///Descarga PDFs contenidos dentro de sitio web
        ///</summary>
        ///<return>
        ///void - (Se descargan archivos PDF en "ruta" dada)
        ///</return>
        ///<param name="URL">
        ///Dirección desde donde se captura código HTML (ej: http://192.321.315.09/INT/Service/Default.aspx?p=12311)
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

                DownloadAllPDFs(links, w, ruta);
            }
            catch (Exception ex) {}
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
        private void DownloadAllPDFs(List<String> links, WebClient w, string ruta)
        {
            DirectoryInfo di;
            String file, fullPath;

            file = "File_" + Guid.NewGuid().ToString("N") + ".pdf";

            if (!Directory.Exists(ruta))
            {
                di = Directory.CreateDirectory(ruta);
            }

            foreach (var item in links)
            {
                fullPath = ruta + file;
                w.DownloadFile(HREFtoURL(item), fullPath);
            }
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
            return str.Substring(6, str.Count() - 7);
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
