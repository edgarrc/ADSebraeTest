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

            /* Usuário e senha que será utilizado para acessar o AD e buscar o usuário da aplicação.
               É o mesmo usuário do pool do iis e o mesmo código que a aplicação usa.
               Apesar do pool, é feito o uso desse usuário e senha diretamente no código conforme
               este exemplo, prevalecendo sobre o do pool do iis. */
            Console.WriteLine("==== Teste de autenticação do usuário no AD ====");
            Console.WriteLine();
            Console.Write("Digite o nome do usuário no AD: ");
            adUser = Console.ReadLine();
            Console.Write("Digite a senha do usuário do AD: ");
            adPass = ReadPassword('*');


            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Digite o nome do usuário a ser encontrado: ");
                    login = Console.ReadLine();

                    try
                    {
                        domain = login.Split('\\')[0];
                        user = login.Split('\\')[1];
                    }
                    catch
                    {
                        Console.WriteLine();
                        Console.WriteLine("Formato inválido, use usuario\\dominio: ex: UF-NOME\\nomedousuario");
                        continue;
                    }

                    Console.WriteLine();
                    Console.WriteLine($"Tentando buscar {login} (domínio: {domain} user: {user})");

                    //Contexto de autenticação no AD
                    pc = new PrincipalContext(ContextType.Domain, domain, adUser, adPass);

                    //Tenta apenas buscar o usuário, sem autenticá-lo, a senha do usuário não é necessária
                    up = UserPrincipal.FindByIdentity(pc, user);

                    /* 
                        Se exibiu encontrado ou não encontrado, está tudo certo, mesmo não encontrado, foi possível contatar o ad e fazer a busca
                        Mas o que ocorre é que o FindByIdentity gera uma exceção
                    */
                    if (up != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $" Login {login} ENCONTRADO OK!");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + $" Login {login} NÂO ENCONTRADO!");
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
                Console.Write("Buscar outro usuário? [S][n]: ");                
                bool continua = Console.ReadLine().ToLower() == "s" ? true : false;
                if (!continua)
                    break;

            }            

        }
    }
}
