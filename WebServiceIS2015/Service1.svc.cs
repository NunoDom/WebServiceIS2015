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

        public List<Resultado> GetCustoMedioFuncionario(int dataInicio, int dataFim)
        {
            //checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeMedicos = xmlFile.SelectNodes("//PessoalAoServiço/Médicos/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeTecnicosDeDiagonostico = xmlFile.SelectNodes("//PessoalAoServiço/Técnicosdediagnósticoeterapêutica/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEnfermeiros = xmlFile.SelectNodes("//PessoalAoServiço/Enfermeiros/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodePessoaldeEnfermagem = xmlFile.SelectNodes("//PessoalAoServiço/Pessoaldeenfermagem/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            XmlNodeList nodeValorComPessoal = xmlFile.SelectNodes("//DespesadoSNS/Compessoal/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            for (int i = 0; i < nodeMedicos.Count; i++)
            {

                
                Resultado resultado = new Resultado();
                resultado.Ano = Int32.Parse(nodeMedicos[i].Attributes[0].Value);
                Linha linha = new Linha();

                int numeroTotalDeFuncionarios = Int32.Parse(nodeMedicos[i].InnerText) + Int32.Parse(nodePessoaldeEnfermagem[i].InnerText) + Int32.Parse(nodeTecnicosDeDiagonostico[i].InnerText) + Int32.Parse(nodeEnfermeiros[i].InnerText);
                double numeroDeDespesaComPessoal = double.Parse(nodeValorComPessoal[i].InnerText);

                linha.Tipo = "Custo total por ano em euros";
                linha.Valor = (numeroDeDespesaComPessoal/numeroTotalDeFuncionarios)*1000000;
                resultado.AddLinha(linha);
                resultados.Add(resultado);
            }


            return resultados;
        }


        public List<Resultado> GetNumeroFuncionarios(int dataInicio, int dataFim)
        {
            //checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            if (xmlFile != null)
            {
                XmlNodeList nodeMedicos = xmlFile.SelectNodes("//PessoalAoServiço/Médicos/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
                XmlNodeList nodeTecnicosDeDiagonostico = xmlFile.SelectNodes("//PessoalAoServiço/Técnicosdediagnósticoeterapêutica/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
                XmlNodeList nodeEnfermeiros = xmlFile.SelectNodes("//PessoalAoServiço/Enfermeiros/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
                XmlNodeList nodePessoaldeEnfermagem = xmlFile.SelectNodes("//PessoalAoServiço/Pessoaldeenfermagem/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");


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

        public List<Resultado> GetNumeroMedicosEnfermeirosTecnico(int dataInicio, int dataFim)
        {
            //checkAuthentication(token, false);
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
        public List<Resultado> GetPercentagemCustosMedicamentosDespesaTotal(int dataInicio, int dataFim)
        {
           // checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeEncargosMedSns = xmlFile.SelectNodes("//Encargoscommedicamentos/DoSNS/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEncargosMedUtente = xmlFile.SelectNodes("//Encargoscommedicamentos/Doutente/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
                XmlNodeList nodeDespesaTotal = xmlFile.SelectNodes("//DespesadoSNS/Total/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

                for (int i = 0; i < nodeEncargosMedSns.Count; i++)
                {
                    Resultado resultado = new Resultado();
                    resultado.Ano = Int32.Parse(nodeEncargosMedSns[i].Attributes[0].Value);
                    Linha linha = new Linha();

                    
                    double EncargosMedicamentosTotal = double.Parse(nodeEncargosMedSns[i].InnerText) + double.Parse(nodeEncargosMedUtente[i].InnerText);
                    double DespesaTotal = double.Parse(nodeDespesaTotal[i].InnerText);
                    double ResultadoPercentagem = (EncargosMedicamentosTotal * 100) / DespesaTotal;

                    if (double.Parse(nodeEncargosMedSns[i].InnerText) == 0 || double.Parse(nodeEncargosMedUtente[i].InnerText) == 0 || double.Parse(nodeDespesaTotal[i].InnerText)==0)
                    {
                        linha.Tipo = "Valores Insuficientes";
                        linha.Valor = 0;

                    }
                    else { 
                    linha.Tipo = "Percentagem de custos com Medicamentos";
                    linha.Valor = ResultadoPercentagem;
                    
                    }
                    resultado.AddLinha(linha);
                    resultados.Add(resultado);
                }
                return resultados;

        }

        //percentagem dos custos com utentes face à despesa total;

        public List<Resultado> GetPercentagemCustosPessoalDespesaTotal(int dataInicio, int dataFim)
        {
            // checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeDespesaPessoal = xmlFile.SelectNodes("//DespesadoSNS/Compessoal/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeDespesaTotal = xmlFile.SelectNodes("//DespesadoSNS/Total/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            for (int i = 0; i < nodeDespesaPessoal.Count; i++)
            {
                Resultado resultado = new Resultado();
                resultado.Ano = Int32.Parse(nodeDespesaPessoal[i].Attributes[0].Value);
                Linha linha = new Linha();


                double DespesaPessoal = double.Parse(nodeDespesaPessoal[i].InnerText);
                double DespesaTotal = double.Parse(nodeDespesaTotal[i].InnerText);
                double ResultadoPercentagem = (DespesaPessoal * 100) / DespesaTotal;

                if (double.Parse(nodeDespesaPessoal[i].InnerText) == 0 || double.Parse(nodeDespesaTotal[i].InnerText) == 0)
                {
                    linha.Tipo = "Valores Insuficientes";
                    linha.Valor = 0;

                }
                else
                {
                    linha.Tipo = "Percentagem de custos com Pessoal";
                    linha.Valor = ResultadoPercentagem;

                }
                resultado.AddLinha(linha);
                resultados.Add(resultado);
            }
            return resultados;

        }

        //número de consultas, internamentos e urgências em hospitais;

        public List<Resultado> GetNumeroConsultasInternamentosUrgencias(int dataInicio, int dataFim)
        {
            //checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeConsultas = xmlFile.SelectNodes("//Consultas/Hospitais/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeInternamentos = xmlFile.SelectNodes("//Internamentos/Hospitais/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeUrgencias = xmlFile.SelectNodes("//Urgências/Hospitais/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            for (int i = 0; i < nodeConsultas.Count; i++)
            {
                Resultado resultado = new Resultado();
                resultado.Ano = Int32.Parse(nodeConsultas[i].Attributes[0].Value);

                Linha linhaConsultas = new Linha();
                linhaConsultas.Tipo = "Total Consultas Hopitais";
                linhaConsultas.Valor = double.Parse(nodeConsultas[i].InnerText);
                resultado.AddLinha(linhaConsultas);

                Linha linhaInternamentosHopitais = new Linha();
                linhaInternamentosHopitais.Tipo = "Total Internamentos Hopitais";
                linhaInternamentosHopitais.Valor = double.Parse(nodeInternamentos[i].InnerText);
                resultado.AddLinha(linhaInternamentosHopitais);


                Linha linhaUrgênciasHopitais = new Linha();
                linhaUrgênciasHopitais.Tipo = "Total Urgências Hopitais";
                linhaUrgênciasHopitais.Valor = double.Parse(nodeUrgencias[i].InnerText);
                resultado.AddLinha(linhaUrgênciasHopitais);

                resultados.Add(resultado);
            }

            return resultados;

        }

        //percentagem de consultas, internamentos e urgências em centros de saúde e extensões face ao total de ocorrências;

        public List<Resultado> GetPercentagemConsultasInternamentosUrgenciasCentrosSaudeExtencoes(int dataInicio, int dataFim)
        {
            // checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeConsultasCS = xmlFile.SelectNodes("//Consultas/Centrosdesaúde/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeInternamentosCS = xmlFile.SelectNodes("//Internamentos/Centrosdesaúde/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeUrgenciasCS = xmlFile.SelectNodes("//Urgências/Centrosdesaúde/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            XmlNodeList nodeConsultasTotal = xmlFile.SelectNodes("//Consultas/Total/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeInternamentosTotal = xmlFile.SelectNodes("//Internamentos/Total/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeUrgenciasTotal = xmlFile.SelectNodes("//Urgências/Total/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");



            for (int i = 0; i < nodeConsultasCS.Count; i++)
            {
                Resultado resultado = new Resultado();
                resultado.Ano = Int32.Parse(nodeConsultasCS[i].Attributes[0].Value);

                


                double ConsultaCS = double.Parse(nodeConsultasCS[i].InnerText);
                double ConsultaTotal = double.Parse(nodeConsultasTotal[i].InnerText);
                double ResultadoPercentagemConsultas = (ConsultaCS * 100) / ConsultaTotal;

                Linha linhaConsulta = new Linha();
                linhaConsulta.Valor = ResultadoPercentagemConsultas;
                linhaConsulta.Tipo = "Resultado Consulta";
                resultado.AddLinha(linhaConsulta);

                double InternamentosCS = double.Parse(nodeInternamentosCS[i].InnerText);
                double InternamentosTotal = double.Parse(nodeInternamentosTotal[i].InnerText);
                double ResultadoPercentagemInternamentos = (InternamentosCS * 100) / InternamentosTotal;

                Linha linhaInternamento = new Linha();
                linhaInternamento.Valor = ResultadoPercentagemInternamentos;
                linhaInternamento.Tipo = "Resultado Internamento";
                resultado.AddLinha(linhaInternamento);

                double UrgenciasCS = double.Parse(nodeUrgenciasCS[i].InnerText);
                double UrgenciasTotal = double.Parse(nodeUrgenciasTotal[i].InnerText);
                double ResultadoPercentagemUrgencias = (UrgenciasCS * 100) / UrgenciasTotal;

                Linha linhaUrgencias = new Linha();
                linhaUrgencias.Valor = ResultadoPercentagemUrgencias;
                linhaUrgencias.Tipo = "Resultado Urgencias";
                resultado.AddLinha(linhaUrgencias);

                resultados.Add(resultado);
            }
            return resultados;

        }

        //média do número de camas disponíveis nos hospitais;

        public List<Resultado> GetMediaCamasHospital(int dataInicio, int dataFim)
        {
            //checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeLotacaoHG = xmlFile.SelectNodes("//Lotação/Hospitaisgerais/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeLotacaoHE = xmlFile.SelectNodes("//Lotação/Hospitaisespecializados/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEstabelecimentoSaudeHG = xmlFile.SelectNodes("//Estabelecimentosdesaúde/Hospitaisgerais/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEstabelecimentoSaudeHE = xmlFile.SelectNodes("//Estabelecimentosdesaúde/Hospitaisespecializados/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            for (int i = 0; i < nodeLotacaoHG.Count; i++)
            {
                Resultado resultado = new Resultado();
                resultado.Ano = Int32.Parse(nodeLotacaoHG[i].Attributes[0].Value);
                Linha linha = new Linha();

                double LotacaoHG = double.Parse(nodeLotacaoHG[i].InnerText);
                double LotacaoHE = double.Parse(nodeLotacaoHE[i].InnerText);
                double EstabelecimentoSaudeHG = double.Parse(nodeEstabelecimentoSaudeHG[i].InnerText);
                double EstabelecimentoSaudeHE = double.Parse(nodeEstabelecimentoSaudeHE[i].InnerText);
                double ResultadoMedia = (LotacaoHG + LotacaoHE) / (EstabelecimentoSaudeHG + EstabelecimentoSaudeHE);

                linha.Tipo = "Média de camas disponivéis";
                linha.Valor = ResultadoMedia;


                resultado.AddLinha(linha);
                resultados.Add(resultado);
            }

            return resultados;

        }

        // rácio entre o número de funcionários e número de estabelecimentos.

        public List<Resultado> GetRacioNumeroFuncionariosNumeroEstabelecimentos(int dataInicio, int dataFim)
        {
            //checkAuthentication(token, false);
            List<Resultado> resultados = new List<Resultado>();

            XmlNodeList nodeMedicos = xmlFile.SelectNodes("//PessoalAoServiço/Médicos/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeTecnicosDeDiagonostico = xmlFile.SelectNodes("//PessoalAoServiço/Técnicosdediagnósticoeterapêutica/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEnfermeiros = xmlFile.SelectNodes("//PessoalAoServiço/Enfermeiros/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodePessoaldeEnfermagem = xmlFile.SelectNodes("//PessoalAoServiço/Pessoaldeenfermagem/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEstabelecimentoSaudeHG = xmlFile.SelectNodes("//Estabelecimentosdesaúde/Hospitaisgerais/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEstabelecimentoSaudeHE = xmlFile.SelectNodes("//Estabelecimentosdesaúde/Hospitaisespecializados/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEstabelecimentoCS = xmlFile.SelectNodes("//Estabelecimentosdesaúde/Centrosdesaúde/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");
            XmlNodeList nodeEstabelecimentoSaudeECS = xmlFile.SelectNodes("//Estabelecimentosdesaúde/Extensõesdecentrosdesaúde/Anos/Ano[@ano>='" + dataInicio + "'and @ano<='" + dataFim + "']");

            for (int i = 0; i < nodeMedicos.Count; i++)
            {
                Resultado resultado = new Resultado();
                resultado.Ano = Int32.Parse(nodeMedicos[i].Attributes[0].Value);
                Linha linha = new Linha();

                double Medicos = double.Parse(nodeMedicos[i].InnerText);
                double TecnicosDeDiagonostico = double.Parse(nodeTecnicosDeDiagonostico[i].InnerText);
                double Enfermeiros = double.Parse(nodeEnfermeiros[i].InnerText);
                double PessoaldeEnfermagem = double.Parse(nodePessoaldeEnfermagem[i].InnerText);
                double EstabelecimentoSaudeHG = double.Parse(nodeEstabelecimentoSaudeHG[i].InnerText);
                double EstabelecimentoSaudeHE = double.Parse(nodeEstabelecimentoSaudeHE[i].InnerText);
                double EstabelecimentoCS = double.Parse(nodeEstabelecimentoCS[i].InnerText);
                double EstabelecimentoSaudeECS = double.Parse(nodeEstabelecimentoSaudeECS[i].InnerText);

                double ResultadoRacio = (Medicos + TecnicosDeDiagonostico + Enfermeiros + PessoaldeEnfermagem) / (EstabelecimentoSaudeHG + EstabelecimentoSaudeHE + EstabelecimentoCS + EstabelecimentoSaudeECS);

                linha.Tipo = "Rácio entre funcionários e estabelecimentos";
                linha.Valor = ResultadoRacio;


                resultado.AddLinha(linha);
                resultados.Add(resultado);
            }
            return resultados;

        }



    }
}
