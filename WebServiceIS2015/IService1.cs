using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WebServiceIS2015  
{
    // NOTE: You can use the "Rename" command on the "Re  factor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        string ReceiveData(string value);

        [OperationContract]
        Boolean GetXMLData(string value);


        [OperationContract]
        string GetCustoMedioMedicoEnfTec(DateTime dataInicio, DateTime dataFim);

        //número de funcionários;
        [OperationContract]
        string GetNumeroFuncionarios(DateTime dataInicio, DateTime dataFim);

        //número de médicos, enfermeiros e técnicos;
        [OperationContract]
        string GetNumeroMedicosEnfermeirosTecnico(DateTime dataInicio, DateTime dataFim);

        //percentagem dos custos com medicamentos face à despesa total;
        [OperationContract]
        string GetPercentagemCustosMedicamentosDespesaTotal(DateTime dataInicio, DateTime dataFim);

        //percentagem dos custos com utentes face à despesa total;
        [OperationContract]
        string GetPercentagemCustosUtentesDespesaTotal(DateTime dataInicio, DateTime dataFim);

        //número de consultas, internamentos e urgências em hospitais;
        [OperationContract]
        string GetNumeroCOnsultasInternamentosUrgencias(DateTime dataInicio, DateTime dataFim);

        //percentagem de consultas, internamentos e urgências em centros de saúde e extensões face ao total de ocorrências;
        [OperationContract]
        string GetPercentagemConsultasIternamentosUrgenciasCentrosSaudeExtencoes(DateTime dataInicio, DateTime dataFim);

        //média do número de camas disponíveis nos hospitais;
        [OperationContract]
        string GetMediaCamasHospital(DateTime dataInicio, DateTime dataFim);

        // rácio entre o número de funcionários e número de estabelecimentos.
        [OperationContract]
        string GetRacioNumeroFuncionariosNumeroEstabelecimentos(DateTime dataInicio, DateTime dataFim);






        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
