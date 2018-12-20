using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace ADSebraeTest
{
    class Program
    {
        #region Util

        public static string ReadPassword(char mask)
        {
            const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
            int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ };

            var pass = new Stack<char>();
            char chr = (char)0;

            while ((chr = System.Console.ReadKey(true).KeyChar) != ENTER)
            {
                if (chr == BACKSP)
                {
                    if (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0) { }
                else
                {
                    pass.Push((char)chr);
                    System.Console.Write(mask);
                }
            }

            System.Console.WriteLine();

            return new string(pass.Reverse().ToArray());
        }

        #endregion

        static void Main(string[] args)
        {

            string adUser = string.Empty;
            string adPass = string.Empty;
            string login = string.Empty;
            string user = string.Empty;
            string domain = string.Empty;
            PrincipalContext pc = null;
            UserPrincipal up = null;

            /* Usu�rio e senha que ser� utilizado para acessar o AD e buscar o usu�rio da aplica��o.
               � o mesmo usu�rio do pool do iis e o mesmo c�digo que a aplica��o usa.
               Apesar do pool, � feito o uso desse usu�rio e senha diretamente no c�digo conforme
               este exemplo, prevalecendo sobre o do pool do iis. */
            Console.WriteLine("==== Teste de autentica��o do usu�rio no AD ====");
            Console.WriteLine();
            Console.Write("Digite o nome do usu�rio no AD: ");
            adUser = Console.ReadLine();
            Console.Write("Digite a senha do usu�rio do AD: ");
            adPass = ReadPassword('*');


            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Digite o nome do usu�rio a ser encontrado: ");
                    login = Console.ReadLine();

                    try
                    {
                        domain = login.Split('\\')[0];
                        user = login.Split('\\')[1];
                    }
                    catch
                    {
                        Console.WriteLine();
                        Console.WriteLine("Formato inv�lido, use usuario\\dominio: ex: UF-NOME\\nomedousuario");
                        continue;
                    }

                    Console.WriteLine();
                    Console.WriteLine($"Tentando buscar {login} (dom�nio: {domain} user: {user})");

                    //Contexto de autentica��o no AD
                    pc = new PrincipalContext(ContextType.Domain, domain, adUser, adPass);

                    //Tenta apenas buscar o usu�rio, sem autentic�-lo, a senha do usu�rio n�o � necess�ria
                    up = UserPrincipal.FindByIdentity(pc, user);

                    /* 
                        Se exibiu encontrado ou n�o encontrado, est� tudo certo, mesmo n�o encontrado, foi poss�vel contatar o ad e fazer a busca
                        Mas o que ocorre � que o FindByIdentity gera uma exce��o
                    */
                    if (up != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $" Login {login} ENCONTRADO OK!");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $" Login {login} N�O ENCONTRADO!");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $" ERRO: " + ex.Message);
                }
                finally {
                    if (up != null) pc.Dispose();
                    if (pc != null) pc.Dispose();
                }

                Console.WriteLine();
                Console.Write("Buscar outro usu�rio? [S][n]: ");                
                bool continua = Console.ReadLine().ToLower() == "s" ? true : false;
                if (!continua)
                    break;

            }            

        }
    }
}
