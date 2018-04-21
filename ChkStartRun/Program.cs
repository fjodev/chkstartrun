/*
 * Programa : ChkStarRun.exe
 * Função   : Verificação de diferenças nos programas iniciados automáticamente no arranque do Windows.
 *            Os programas iniciados no arranque são registados num ficheiro, se existerm diferenças 
 *            relativamente ao último registo realizado, o utilizador é informado que existem alterações, 
 *            caso contrário nada é dito.
 * Autor    : Fernando Oliveira
 * Data     : Outubro 2012
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ChkStartRun
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Inicializações de varoaveis gerais
            // A inicialização de variaveis especificas ficam junto ao código onde são usadas

            string usr = Environment.UserName;
            string machine = Environment.MachineName;
            string fileTxtName = "csr-" + machine + "-" + usr + ".txt";
            string fileBakName = "csr-" + machine + "-" + usr + ".bak";
            string fileLogName = "ChkStartRun.log";
            string crlf = "\r\n";
            int totalItens = 0;
            int returnReadItem = 0;
            bool callFormShowDiff = false;
            bool writeStartProg = true;
            DialogResult dr = DialogResult.Ignore;
            bool existsBakFile;
            bool existsTxtFile;
            string frmTitleAdd = " ";

            /*
             * Fase 0 - Inicializações 
             */

            // Abertura do log

            WriteFileLine(fileLogName, "--------- " + Application.ProductName + ", versão " + Application.ProductVersion.ToString() + " --- \r\n" +
                DateTime.Now.ToString() +
                "\r\nNome de Utilizador: " + usr +
                "\r\n" + "Nome de computador: " + machine);

            // Verificação de parametros

            if (args.Length > 0)
            {
                foreach (string s in args)
                {
                    switch (s.ToLower())
                    {
                        case "/m":
                            callFormShowDiff = true;
                            writeStartProg = false;
                            frmTitleAdd = frmTitleAdd + " (modo manutenção)";
                            WriteFileLine(fileLogName, "Usado o switch /m - modo manutenção (registo programas não efetuado)");
                            break;

                        case "/v":
                            callFormShowDiff = true;
                            frmTitleAdd = frmTitleAdd + " (modo verbose)";
                            WriteFileLine(fileLogName, "Usado o switch /v - modo verbose (apresentação ecrã diferenças)");
                            break;

                        default:
                            WriteFileLine(fileLogName, "Switch não reconhecido: " + s);
                            break;
                    }
                }
            }
            else
            {
                WriteFileLine(fileLogName, "Nenhum switch usado");
            }

       

            /* 
             * Fase 1 - Registo programas iniciados automáticamente
             */

            /* 
             * Fase 1.1: Programas iniciados por chaves do Registry
             */
                                                          
            // Definição das chaves a ler.

            if (writeStartProg)
            {

                RegistryKey[] rksToCheck;

                if (Environment.Is64BitOperatingSystem)
                {
                    rksToCheck = new RegistryKey[] {
                                     RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                                        RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"),
                                     RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                                        RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"),
                                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                                        RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Wow6432node\Microsoft\Windows\CurrentVersion\Run"),
                                     RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                                        RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Wow6432node\Microsoft\Windows\CurrentVersion\RunOnce"),
                                     RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,
                                        RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"),
                                     RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,
                                        RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce")
                                                       };

                    WriteFileLine(fileLogName, "64 bit Operating System");
                }
                else
                {
                    rksToCheck = new RegistryKey[] { 
                                     RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                        RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"),
                                     RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                        RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"),
                                     RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,
                                         RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"),
                                     RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,
                                        RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce")
                                           };
                    WriteFileLine(fileLogName, "32 bit Operating System");
                }

                // Registo do valor das chaves em ficheiro

                int KeysCount = 0;
                int ItemCount = 0;
                bool DelTxtFile = false;

                foreach (RegistryKey rk in rksToCheck)
                {
                    if (KeysCount == 0)
                    {
                        DelTxtFile = true;
                    }
                    else
                    {
                        DelTxtFile = false;
                    }


                    returnReadItem = WriteRegStartRunInfo(rk, fileTxtName, DelTxtFile);
                    if (returnReadItem != -1)
                    {
                        ItemCount = ItemCount + returnReadItem;
                        WriteFileLine(fileLogName, "Sucesso na leitura da chave: " + rk.Name);
                    }
                    else
                    {
                        WriteFileLine(fileLogName, "Erro na leitura da chave " + KeysCount);

                    }
                    KeysCount++;
                }
                totalItens = totalItens + ItemCount;
                WriteFileLine(fileLogName, "Registados " + ItemCount + " itens pertencentes a " + KeysCount +
                    " chaves de registo em " + fileTxtName);

                /*
                * Fase 1.2: Registo de programas executados no arranque do menu iniciar
                */

                // Definição das diretorias a ler

                string[] StartupFolders = {
                                              Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup),
                                              Environment.GetFolderPath(Environment.SpecialFolder.Startup)
                                          };
                int DirCount = 0;
                int FileCount = 0;

                // Leitura e registo dos ficheiros 

                foreach (string smdir in StartupFolders)
                {
                    returnReadItem = WriteSMStartupInfo(smdir, fileTxtName);
                    if (returnReadItem != -1)
                    {
                        FileCount = FileCount + returnReadItem;
                        WriteFileLine(fileLogName, "Sucesso na leitura da diretoria: " + smdir);
                    }
                    else
                    {
                        WriteFileLine(fileLogName, "Erro na leitura da diretoria: " + smdir);
                    }
                    DirCount++;
                }
                totalItens = totalItens + FileCount;
                WriteFileLine(fileLogName, "Registados " + FileCount + " ficheiros pertencentes a " + DirCount +
                    " diretorias em " + fileTxtName);
                WriteFileLine(fileTxtName, "Número total de itens = " + totalItens);

                /*
                * Fase 1.3: Programas no Programador de Tarefas
                */

                // A realizar brevemente

            }
            else
            {
                WriteFileLine(fileLogName, "Registo de programas de arranque não efetuado");
            }

            /*
             * Fase 2: Análise de diferenças
             */

            // Verificação da existência de ficheiros

            if (File.Exists(fileTxtName))
            {
                existsTxtFile = true;
                WriteFileLine(fileLogName, fileTxtName + " existente");
            }
            else
            {
                existsTxtFile = false;
                WriteFileLine(fileLogName, fileTxtName + " inexistente");
            }

            if (File.Exists(fileBakName))
            {
                existsBakFile = true;
                WriteFileLine(fileLogName, fileBakName + " existente");
            }
            else
            {
                existsBakFile = false;
                WriteFileLine(fileLogName, fileBakName + " inexistente");
            }

            int diffCount = 0;
            string diffLine = null;
            string diffDescr = null;
            DateTime fileTxtDate = DateTime.Now;
            DateTime fileBakDate = DateTime.Now;
                        
            if (existsTxtFile)
            {
                FileInfo txtInfo = new FileInfo(fileTxtName);
                fileTxtDate = txtInfo.LastWriteTime;

                if (existsBakFile)
                {
                    WriteFileLine(fileLogName, "Inicio da análise de diferenças de " + fileTxtName + " com " + fileBakName);

                    FileInfo bakInfo = new FileInfo(fileBakName);
                    fileBakDate = bakInfo.LastWriteTime;

                    string[] fbn = File.ReadAllLines(fileBakName, Encoding.UTF8);
                    string[] fln = File.ReadAllLines(fileTxtName, Encoding.UTF8);

                    // Comparação de tamanhos

                    if (fbn.Length != fln.Length)
                    {
                        diffDescr = "Tamanhos diferentes";
                        WriteFileLine(fileLogName, diffDescr);
                        diffLine = diffLine + crlf + diffDescr;
                        diffCount++;
                    }

                    // Verifica se todas as linhas no ficheiro de bak existem no de registo

                    foreach (string s1 in fbn)
                    {
                        int matchCount = 0;
                        foreach (string s2 in fln)
                        {
                            if (s2.CompareTo(s1) == 0)
                                matchCount++;
                        }
                        if (matchCount == 0)
                        {
                            diffDescr = "Linha desaparecida: " + s1;
                            WriteFileLine(fileLogName, diffDescr);
                            diffLine = diffLine + crlf + diffDescr;
                            diffCount++;
                        }
                    }

                    // Verifica se todas as linhas no ficheiro de registo existem no de bak

                    foreach (string s3 in fln)
                    {
                        int matchCount = 0;
                        foreach (string s4 in fbn)
                        {
                            if (s4.CompareTo(s3) == 0)
                                matchCount++;
                        }
                        if (matchCount == 0)
                        {
                            diffDescr = "Nova linha: " + s3;
                            WriteFileLine(fileLogName, diffDescr);
                            diffLine = diffLine + crlf + diffDescr;
                            diffCount++;
                        }
                    }
                    if (diffCount == 0)
                    {
                        diffLine = "Não foram encontradas diferenças";
                        WriteFileLine(fileLogName, diffLine);
                    }
                }
                else
                {
                    File.Copy(fileTxtName, fileBakName);
                    WriteFileLine(fileLogName, "Ficheiro " + fileBakName + " inexistente: criado um novo");
                    WriteFileLine(fileLogName, "Ficheiro " + fileBakName + " inexistente: comparação não efetuada");
                    diffLine = "Comparação não efetuada";
                }
            }
            else
            {
                diffLine = "Comparaçao não efetuada";
                WriteFileLine(fileLogName, diffLine);
            }

            /*
             * Fase 3: Ecrã de gestão
             */

            if (diffCount > 0 & !callFormShowDiff)
            {
                if (MessageBox.Show("Detetada(s) diferença(s) no(s) programa(s) de arranque.\nDeseja visualizar as alterações?",
                    "ChkStartRun", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    callFormShowDiff = true;
                }
                else
                {
                    WriteFileLine(fileLogName, "Visualização das diferenças não realizada");
                }
            }
            
            if (callFormShowDiff)
            {
                using (frmDiffShow frmSD = new frmDiffShow())
                {
                    WriteFileLine(fileLogName, "Apresentação do ecrã de diferenças");

                    // Preparação da form
                    if (existsTxtFile)
                    {
                        frmSD.tbTxtFile.Text = File.ReadAllText(fileTxtName);
                    }
                    else
                    {
                        frmSD.tbTxtFile.Text = "** Não existe ficheiro de registo **";
                    }
                    if (existsBakFile)
                    {
                        frmSD.tbBakFile.Text = File.ReadAllText(fileBakName);
                    }
                    else
                    {
                        frmSD.tbBakFile.Text = "** Não existe registo anterior **";

                    }
                    frmSD.lnlblVerLog.Text = fileLogName;
                    frmSD.tbDiffLines.Text = diffLine.Trim();
                    frmSD.tbTxtFile.ReadOnly = true;
                    frmSD.tbBakFile.ReadOnly = true;
                    frmSD.tbDiffLines.ReadOnly = true;
                    if (diffCount == 0 | !writeStartProg)
                    {
                        frmSD.ckbConfDiff.Enabled = false;
                    }
                    else
                    {
                        frmSD.ckbConfDiff.Enabled = true;
                    }

                    frmSD.Text = frmSD.Text + frmTitleAdd;
                    frmSD.lblNewStat.Text = frmSD.lblNewStat.Text + " (" + fileTxtDate.ToString() + ")";
                    frmSD.lblOldStat.Text = frmSD.lblOldStat.Text + " (" + fileBakDate.ToString() + ")";
                    
                    frmSD.Enabled = true;

                    // É aqui que mostramos o ecra
                    dr = frmSD.ShowDialog();

                    // Aqui agimos de acordo com o a resposta 
                    if (dr == DialogResult.Yes)
                    {
                        File.Copy(fileTxtName, fileBakName, true);
                        WriteFileLine(fileLogName, "Ficheiro de backup foi atualizado com novos programas de arranque.");
                    }
                    else if (dr == DialogResult.No & diffCount > 0)
                    {
                        WriteFileLine(fileLogName, "Alterações aos programas de arranque não foram confirmadas.");
                    }
                }
            }
            WriteFileLine(fileLogName, "Fim de programa.");
       }

        /*
         * Tha, th, tha, that's all folks !!!
         */

        /*
         * ------------------------- Funções -------------------------------------------------------
         */
        
        static int WriteRegStartRunInfo(RegistryKey skey, string fileWRegName, bool delFile)
        {
            /* Escreve no ficheiro 'fileWRegName' o valor da chave de registo indicadas em 'skey'. 
             * Se 'delFile' for verdadeira o ficheiro de registo é apagado e recriado antes da
             * escrita.
             * Retorna o nº de items lidos se não existirem erros ou -1 em caso de erro */

            String ItemIcon = "reg -> ";
            string ItemSep = ": ";
       
            try
            {
                // Se existir e assim for ordenado pelo parametro a função apaga o ficheiro de texto
                if (File.Exists(fileWRegName) & delFile)
                    File.Delete(fileWRegName);

                WriteFileLine(fileWRegName, skey.Name);
                
                // Inicio da obtenção de valores da chave para um array
                string[] skeynames = skey.GetValueNames();

                // Os valores da array são escritos no ficheiro
                foreach (string vn in skeynames)
                {
                    WriteFileLine(fileWRegName, ItemIcon + vn.ToString() + ItemSep + skey.GetValue(vn).ToString());
                }
                return skey.ValueCount;
            }

            catch
            {
                return -1;
            }
        }

        
        static int WriteSMStartupInfo(string dir, string fileWSMSName)
        {
            /* Escreve no ficheiro 'fileWSMSName' os  nomes de ficheiros existentes na diretoria 'dir'
             * Retorna o nº de ficheiros lidos se não existirem erros ou -1 em caso de erro */

            string FileIcon = "sm -> ";
            int FileCount = 0;
            try
            {
                WriteFileLine(fileWSMSName, dir);

                foreach (string smfn in Directory.EnumerateFiles(dir))
                {
                    WriteFileLine(fileWSMSName, FileIcon + smfn);
                    FileCount++;
                }
                return FileCount;
            }
            catch
            {
                return -1;
            }

        }
                
        static bool WriteFileLine(string fileWFLName, string TxtEntry, long IntFileSize = 1000000)
        {
            /* Escreve uma linha de texto no ficheiro indicado em 'fileWFLName'.
             * Se o ficheiro tiver um tamanho superior a 'IntFileSize' será renomeado e 
             * criado um novo ficheiro.
             * Retorna true se não existirem erros, caso contrário retorna false
             */
       
            try
            {
                // Verificação do tamanho
                if (File.Exists(fileWFLName))
                {
                    FileInfo TxtFileInfo = new FileInfo(fileWFLName);

                    if (TxtFileInfo.Length > IntFileSize)
                    {
                        File.Copy(fileWFLName, "~" + fileWFLName,true);
                        File.Delete(fileWFLName);
                    }
                }

                // Escrita no ficheiro          
               using (StreamWriter swlog = File.AppendText(fileWFLName))
                {
                    swlog.WriteLine(TxtEntry);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }      
    }
}
