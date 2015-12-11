using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace WebServiceIS2015
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service1 : IService1
    {
        XmlDocument xmlFile = new XmlDocument();
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composi  te");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suf fix";
            }
            return composite;
        }




        public string ReceiveData(string value)
        {
            if (xmlFile != null)
            {
                XmlNodeList node = xmlFile.SelectNodes("//Consultas/Total/Anos/Ano[@ano='2000']");

                //Consultas/Total/Anos/Ano[@ano="2000"]
                return node[0].InnerText;
            }
            else

                return "";
        }

        public Boolean GetXMLData(string value)
        {
            try
            {
                xmlFile.LoadXml(value);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

           
        }
    }
}
