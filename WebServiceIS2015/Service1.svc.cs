using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Hosting;
using System.Xml;

namespace WebServiceIS2015
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service1 : IService1
    {
        XmlDocument xmlFile = new XmlDocument();
        private Dictionary<string, User> users;
        private Dictionary<string, Token> tokens;
        private static string FILEPATH;

        /// <summary>
        /// /asdasndkjasdfjksdfhsjdkhfsjdkfgsdhfgsdhfgshjgasdhjfgsjfhsk
        /// </summary>

        public Service1()
        {
            this.users = new Dictionary<string, User>();
            this.tokens = new Dictionary<string, Token>();
            // default administrator
            users.Add("admin", new User("admin", "admin", true));
            FILEPATH = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", "XMLFILE.xml");
            loadXML();
            

        }

        private void loadXML()
        {
            xmlFile.Load(FILEPATH);
        }


        public string TestLigacao(string name)
        {
            if (name.Equals("Nuno"))
            {
                return "Certo";
            }
            else
                return "ERRADO";

        }


        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        private class Token
        {
            private string value;
            private long timeout;
            private User user;
            public Token(User user)
                : this(user, 240000) // token válido por 4 minutos
            { }
            public Token(User user, long timeout)
            {
                this.value = Guid.NewGuid().ToString();
                this.timeout = Environment.TickCount + timeout;
                this.user = user;
            }
            public string Value
            {
                get { return value; }
            }
            public long Timeout
            {
                get { return timeout; }
            }
            public User User
            {
                get { return user; }
            }
            public string Username
            {
                get { return user.Username; }
            }
            public void UpdateTimeout()
            {
                UpdateTimeout(240000); // token renovado por 4 minutos
            }
            public void UpdateTimeout(long timeout)
            {
                this.timeout = Environment.TickCount + timeout;
            }
            public Boolean isTimeoutExpired()
            {
                return Environment.TickCount > timeout;
            }
        }

        public void SignUp(User user, string token)
        {
            checkAuthentication(token, true);
            if (users.Keys.Contains(user.Username))
            {
                throw new ArgumentException("ERROR: username already exists: " + user.Username);
            }
            users.Add(user.Username, user);
        }
        public string LogIn(string username, string password)
        {
            cleanUpTokens();
            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password) &&
           password.Equals(users[username].Password))
            {
                Token tokenObject = new Token(users[username]);
                tokens.Add(tokenObject.Value, tokenObject);
                return tokenObject.Value;
            }
            else
            {
                throw new ArgumentException("ERROR: invalid username/password combination.");
            }
        }
        public void LogOut(string token)
        {
            tokens.Remove(token);
            cleanUpTokens();
        }
        public bool IsAdmin(string token)
        {
            return tokens[token].User.Admin;
        }
        public bool IsLoggedIn(string token)
        {
            bool res = true;
            try
            {
                checkAuthentication(token, false);
            }
            catch (ArgumentException)
            {
                res = false;
            }
            return res;
        }
        private void cleanUpTokens()
        {
            foreach (Token tokenObject in tokens.Values)
            {
                if (tokenObject.isTimeoutExpired())
                {
                    tokens.Remove(tokenObject.Username);
                }
            }
        }
        private Token checkAuthentication(string token, bool mustBeAdmin)
        {
            Token tokenObject;
            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentException("ERROR: invalid token value.");
            }
            try
            {
                tokenObject = tokens[token];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("ERROR: user is not logged in (expired session?).");
            }
            if (tokenObject.isTimeoutExpired())
            {
                tokens.Remove(tokenObject.Username);
                throw new ArgumentException("ERROR: the session has expired. Please log in again.");
            }
            if (mustBeAdmin && !tokens[token].User.Admin)
            {
                throw new ArgumentException("ERROR: only admins are allowed to perform this operation.");
            }
            tokenObject.UpdateTimeout();
            return tokenObject;
        }


        public string ReceiveData(DateTime dataInicio)
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
                XmlDocument novo = new XmlDocument();
                novo.LoadXml(value);
                novo.Save(FILEPATH);
                loadXML();
                return true;
            }
            catch (Exception)
            {

                return false;
            }

           
        }

        public String GetCustoMedioFuncionario(String dataInicio, String dataFim)
        {

            return null;
        }

        public String GetCustoMedioMedicoEnfTec(DateTime dataInicio, DateTime dataFim) {
           
            return null;
        }


        public List<Resultado> GetNumeroFuncionarios(int dataInicio, int dataFim, string token)
        {
            checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            if (xmlFile != null)
            {
                XmlNodeList nodeMedicos = xmlFile.SelectNodes("//PessoalAoServiço/Médicos/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
                XmlNodeList nodeTecnicosDeDiagonostico = xmlFile.SelectNodes("//PessoalAoServiço/Técnicosdediagnósticoeterapêutica/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
                XmlNodeList nodeEnfermeiros = xmlFile.SelectNodes("//PessoalAoServiço/Enfermeiros/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
                XmlNodeList nodePessoaldeEnfermagem = xmlFile.SelectNodes("//PessoalAoServiço/Pessoaldeenfermagem/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");


                //XmlNode nodeAno = nodeMedicos[0];
                //string nodname = nodeAno.Attributes[0].Value;

                //int x1 = nodeMedicos.Count;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < nodeMedicos.Count; i++)
                {
                    Resultado resultado = new Resultado();
                    resultado.Ano = Int32.Parse(nodeMedicos[i].Attributes[0].Value);
                    Linha linha = new Linha();

                    linha.Tipo = "Total Funcionarios";
                    linha.Valor = Int32.Parse(nodeMedicos[i].InnerText) + Int32.Parse(nodePessoaldeEnfermagem[i].InnerText) + Int32.Parse(nodeTecnicosDeDiagonostico[i].InnerText) + Int32.Parse(nodeEnfermeiros[i].InnerText);
                    resultado.AddLinha(linha);
                    resultados.Add(resultado);
                }
                return resultados;
            }                
            else
                return null;

        }






        //número de médicos, enfermeiros e técnicos;

        public List<Resultado> GetNumeroMedicosEnfermeirosTecnico(string dataInicio, string dataFim, string token)
        {
            checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeMedicos = xmlFile.SelectNodes("//PessoalAoServiço/Médicos/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeTecnicosDeDiagonostico = xmlFile.SelectNodes("//PessoalAoServiço/Técnicosdediagnósticoeterapêutica/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEnfermeiros = xmlFile.SelectNodes("//PessoalAoServiço/Enfermeiros/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodePessoaldeEnfermagem = xmlFile.SelectNodes("//PessoalAoServiço/Pessoaldeenfermagem/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            for (int i = 0; i < nodeMedicos.Count; i++)
            {
                Resultado resultado = new Resultado();
                resultado.Ano = Int32.Parse(nodeMedicos[i].Attributes[0].Value);

                Linha linhaMedico = new Linha();
                linhaMedico.Tipo = "Total Medicos";
                linhaMedico.Valor = Int32.Parse(nodeMedicos[i].InnerText);
                resultado.AddLinha(linhaMedico);

                Linha linhaPessoalDeEnfermagem = new Linha();
                linhaPessoalDeEnfermagem.Tipo = "Total Pessoal De Enfermagem";
                linhaPessoalDeEnfermagem.Valor = Int32.Parse(nodePessoaldeEnfermagem[i].InnerText);
                resultado.AddLinha(linhaPessoalDeEnfermagem);


                Linha linhaTecnicosDeDiagonostico = new Linha();
                linhaTecnicosDeDiagonostico.Tipo = "Total Tecnicos de Diagonostivo";
                linhaTecnicosDeDiagonostico.Valor = Int32.Parse(nodeTecnicosDeDiagonostico[i].InnerText);
                resultado.AddLinha(linhaTecnicosDeDiagonostico);


                Linha linhaEnfermeiros = new Linha();
                linhaEnfermeiros.Tipo = "Total Enfermeiros";
                linhaEnfermeiros.Valor = Int32.Parse(nodeEnfermeiros[i].InnerText);
                resultado.AddLinha(linhaEnfermeiros);



                resultados.Add(resultado);
            }

            return resultados;
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

        public string GetNumeroConsultasInternamentosUrgencias(DateTime dataInicio, DateTime dataFim)
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
