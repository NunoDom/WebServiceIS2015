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


        [WebInvoke(Method = "POST", UriTemplate = "/test?nome={name}")]
        [OperationContract]
        string TestLigacao(string name);

        

        [WebInvoke(Method = "GET", UriTemplate = "/isloggedin?token={token}")]
        [OperationContract]
        bool IsLoggedIn(string token);






        // custo médio de um funcionário;
        [WebInvoke(Method = "GET", UriTemplate = "/Funcionarios?dataInicio={dataInicio}&dataFim={dataFim}")]
        string GetCustoMedioFuncionario(String dataInicio, String dataFim);


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
