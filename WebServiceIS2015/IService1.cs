﻿using System;
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
        [WebInvoke(Method = "GET", UriTemplate = "/GetData?value={value}")]
        [OperationContract]
        string GetData(int value);



        [WebInvoke(Method = "GET", UriTemplate = "/GetReceive?dataInicio={dataInicio}")]
        [OperationContract]
        string ReceiveData(DateTime dataInicio);


        [WebInvoke(Method = "GET", UriTemplate = "/GetXMLDATA?value={value}")]
        [OperationContract]
        Boolean GetXMLData(string value);

        // AUTHENTICATION
        [WebInvoke(Method = "POST", UriTemplate = "/signup?token={token}")]
        [OperationContract]
        void SignUp(User user, string token); // admin only


        [WebInvoke(Method = "POST", UriTemplate = "/login?username={username}&password={password}")]
        [OperationContract]
        string LogIn(string username, string password);


        [WebInvoke(Method = "POST", UriTemplate = "/logout")]
        [OperationContract]
        void LogOut(string token);

        [WebInvoke(Method = "GET", UriTemplate = "/isadmin?token={token}")]
        [OperationContract]        
        bool IsAdmin(string token);


        [WebInvoke(Method = "GET", UriTemplate = "/test?nome={name}")]
        [OperationContract]
        string TestLigacao(string name);

        

        [WebInvoke(Method = "GET", UriTemplate = "/isloggedin?token={token}")]
        [OperationContract]
        bool IsLoggedIn(string token);




        // custo médio de um funcionário;
        [WebInvoke(Method = "GET", UriTemplate = "/GetCustoMedioFuncionario?dataInicio={dataInicio}&dataFim={dataFim}")]
        [OperationContract]
        List<Resultado> GetCustoMedioFuncionario(int dataInicio, int dataFim);


        [WebInvoke(Method = "GET", UriTemplate = "/GetNumeroFuncionarios?dataInicio={dataInicio}&dataFim={dataFim}")]
        [OperationContract]
        List<Resultado> GetNumeroFuncionarios(int dataInicio, int dataFim);


        [WebInvoke(Method = "GET", UriTemplate = "/GetNumeroMedicosEnfermeirosTecnico?dataInicio={dataInicio}&dataFim={dataFim}")]
        //número de médicos, enfermeiros e técnicos;
        [OperationContract]
        List<Resultado> GetNumeroMedicosEnfermeirosTecnico(int dataInicio, int dataFim);


        [WebInvoke(Method = "GET", UriTemplate = "/GetPercentagemCustosMedicamentosDespesaTotal?dataInicio={dataInicio}&dataFim={dataFim}")]
        //percentagem dos custos com medicamentos face à despesa total;
        [OperationContract]
        List<Resultado> GetPercentagemCustosMedicamentosDespesaTotal(int dataInicio, int dataFim);


        [WebInvoke(Method = "GET", UriTemplate = "/GetPercentagemCustosPessoalDespesaTotal?dataInicio={dataInicio}&dataFim={dataFim}")]
        //percentagem dos custos com utentes face à despesa total;
        [OperationContract]
        List<Resultado> GetPercentagemCustosPessoalDespesaTotal(int dataInicio, int dataFim);

        [WebInvoke(Method = "GET", UriTemplate = "/GetNumeroConsultasInternamentosUrgencias?dataInicio={dataInicio}&dataFim={dataFim}")]
        //número de consultas, internamentos e urgências em hospitais;
        [OperationContract]
        List<Resultado> GetNumeroConsultasInternamentosUrgencias(int dataInicio, int dataFim);


        [WebInvoke(Method = "GET", UriTemplate = "/GetPercentagemConsultasInternamentosUrgenciasCentrosSaudeExtencoes?dataInicio={dataInicio}&dataFim={dataFim}")]
        //percentagem de consultas, internamentos e urgências em centros de saúde e extensões face ao total de ocorrências;
        [OperationContract]
        List<Resultado> GetPercentagemConsultasInternamentosUrgenciasCentrosSaudeExtencoes(int dataInicio, int dataFim);

        [WebInvoke(Method = "GET", UriTemplate = "/GetMediaCamasHospital?dataInicio={dataInicio}&dataFim={dataFim}")]
        //média do número de camas disponíveis nos hospitais;
        [OperationContract]
        List<Resultado> GetMediaCamasHospital(int dataInicio, int dataFim);


        [WebInvoke(Method = "GET", UriTemplate = "/GetRacioNumeroFuncionariosNumeroEstabelecimentos?dataInicio={dataInicio}&dataFim={dataFim}")]
        // rácio entre o número de funcionários e número de estabelecimentos.
        [OperationContract]
        List<Resultado> GetRacioNumeroFuncionariosNumeroEstabelecimentos(int dataInicio, int dataFim);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.

    [DataContract]
    public class User
    {
        private string username;
        private string password;
        private bool admin;
        public User(string username, string password, bool admin)
        {
            this.admin = admin;
            this.username = username;
            this.password = password;
        }
        [DataMember]
        public bool Admin
        {
            get { return admin; }
            set { admin = value; }
        }
        [DataMember]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
    
    [DataContract]
    public class Resultado
    {

        int ano;
        List<Linha> lista;

        public Resultado()
        {
            lista = new List<Linha>();
        }

        public void AddLinha(Linha linha)
        {
            lista.Add(linha);
        }


        [DataMember]
        public int Ano
        {
            get { return ano; }
            set { ano = value; }
        }

        [DataMember]
        public List<Linha> Lista
        {
            get
            { return lista; }
        }

    }


    [DataContract]
    public class Linha
    {
        string tipo;
        double valor;

        [DataMember]
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        [DataMember]
        public double Valor
        {
            get { return valor; }
            set { valor = value; }
        }

    }
}
