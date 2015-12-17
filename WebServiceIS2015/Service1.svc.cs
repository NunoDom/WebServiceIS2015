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


        public string GetCustoMedioMedicoEnfTec(DateTime dataInicio, DateTime dataFim)
        {

            return null;

        }

        //número de funcionários;

        public string GetNumeroFuncionarios(DateTime dataInicio, DateTime dataFim)
        {
            if (xmlFile != null)
            {
                XmlNodeList nodeMedicos = xmlFile.SelectNodes("//PessoalAoServiço/Médicos/Anos/Ano[@ano>='" + dataInicio.Year + "'and @ano<='" + dataFim.Year + "']");
                XmlNodeList nodeTecnicosDeDiagonostico = xmlFile.SelectNodes("//PessoalAoServiço/Técnicosdediagnósticoeterapêutica/Anos/Ano[@ano>='" + dataInicio.Year + "'and @ano<='" + dataFim.Year + "']");
                XmlNodeList nodeEnfermeiros = xmlFile.SelectNodes("//PessoalAoServiço/Enfermeiros/Anos/Ano[@ano>='" + dataInicio.Year + "'and @ano<='" + dataFim.Year + "']");
                XmlNodeList nodePessoaldeEnfermagem = xmlFile.SelectNodes("//PessoalAoServiço/Pessoaldeenfermagem/Anos/Ano[@ano>='" + dataInicio.Year + "'and @ano<='" + dataFim.Year + "']");

                int x1 = nodeMedicos.Count;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < nodeMedicos.Count; i++)
                {
                   // sb.Append(nodeMedicos[i].InnerText.ToString());
                }
                sb.AppendLine("Numero de Medicos: "+nodeMedicos[0].InnerText);
                sb.AppendLine("Numero de Tecnico de Diagonostico: "+nodeTecnicosDeDiagonostico[0].InnerText);
                sb.AppendLine("Numero de Enfermeiros: "+nodeEnfermeiros[0].InnerText);
                sb.AppendLine("Numero de Pessoal de Enfermagem: "+nodePessoaldeEnfermagem[0].InnerText);
                return sb.ToString();
            }
            else
                return "Numoro de Funcionarios : "+"";

        }

        //número de médicos, enfermeiros e técnicos;

        public string GetNumeroMedicosEnfermeirosTecnico(DateTime dataInicio, DateTime dataFim) 
        {
            return null;
        }



        //percentagem dos custos com medicamentos face à despesa total;
        public string GetPercentagemCustosMedicamentosDespesaTotal(DateTime dataInicio, DateTime dataFim)
        {
            return null;

        }

        //percentagem dos custos com utentes face à despesa total;

        public string GetPercentagemCustosUtentesDespesaTotal(DateTime dataInicio, DateTime dataFim)
        {
            return null;

        }

        //número de consultas, internamentos e urgências em hospitais;

        public string GetNumeroCOnsultasInternamentosUrgencias(DateTime dataInicio, DateTime dataFim)
        {
            return null;

        }

        //percentagem de consultas, internamentos e urgências em centros de saúde e extensões face ao total de ocorrências;

        public string GetPercentagemConsultasIternamentosUrgenciasCentrosSaudeExtencoes(DateTime dataInicio, DateTime dataFim)
        {
            return null;

        }

        //média do número de camas disponíveis nos hospitais;

        public string GetMediaCamasHospital(DateTime dataInicio, DateTime dataFim)
        {
            return null;

        }

        // rácio entre o número de funcionários e número de estabelecimentos.

        public string GetRacioNumeroFuncionariosNumeroEstabelecimentos(DateTime dataInicio, DateTime dataFim)
        {
            return null;

        }






    }
}
