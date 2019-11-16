using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WorkerService.Jobs
{
    public interface ITestJob
    {
        void RunTestJob();
    }
    public class TestJob : ITestJob
    {
        public void RunTestJob()
        {
            string[] commands;
            commands = new string[5] {
                "sudo apt update",
                "sudo apt-get -y install postgresql postgresql-contrib",
                "cd /tmp", 
                "sudo -u postgres createuser jdawson --superuser",
                @"sudo adduser --disabled-password --gecos """" jdawson"
                                    };
            ExecuteCommand(commands);
        }

        public void ExecuteCommand(Array commands)
        {
            //Setup Credential and Server Information
            ConnectionInfo ConnNfo = new ConnectionInfo("167.172.237.57", 22, "root",
            new AuthenticationMethod[] { 

                            //Key Based Authenication (using keys in OpenSSH Format)
                            new PrivateKeyAuthenticationMethod("root", new PrivateKeyFile[]
                            {
                                new PrivateKeyFile(@"C:\id_rsa.key", "Grandson-1")
                            }),
            });

            //Execute A (SHELL) Command
            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();

                foreach (string command in commands)
                {
                    using (var cmd = sshclient.CreateCommand(command))
                    {
                        var result = cmd.BeginExecute();
                        using (var reader =
                            new StreamReader(cmd.OutputStream, Encoding.UTF8, true, 1024, true))
                        {
                            while (!result.IsCompleted || !reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (line != null)
                                {
                                    Console.WriteLine(line + Environment.NewLine);
                                }
                            }
                        }
                        Console.WriteLine("Job Complete");
                        cmd.EndExecute(result);
                    }
                }               
            
                sshclient.Disconnect();
            }
        }
    }
}
