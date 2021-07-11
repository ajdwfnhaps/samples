﻿using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommandLineTool
{
    /// <summary>
    /// In this example, subcommands are defined using the builder API.
    /// Defining subcommands is possible by using the return value of app.Command().
    /// </summary>
    class Npm
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "fake-npm",
                Description = "A fake version of the node package manager",
            };

            app.HelpOption(inherited: true);
            app.Command("config", configCmd =>
            {
                configCmd.OnExecute(() =>
                {
                    Console.WriteLine("Specify a subcommand");
                    configCmd.ShowHelp();
                    return 1;
                });

                configCmd.Command("set", setCmd =>
                {
                    setCmd.Description = "Set config value";
                    var key = setCmd.Argument("key", "Name of the config").IsRequired();
                    var val = setCmd.Argument("value", "Value of the config").IsRequired();
                    setCmd.OnExecute(() =>
                    {
                        Console.WriteLine($"Setting config {key.Value} = {val.Value}");
                    });
                });

                configCmd.Command("list", listCmd =>
                {
                    var json = listCmd.Option("--json", "Json output", CommandOptionType.NoValue);
                    listCmd.OnExecute(() =>
                    {
                        if (json.HasValue())
                        {
                            Console.WriteLine("{\"dummy\": \"value\"}");
                        }
                        else
                        {
                            Console.WriteLine("dummy = value");
                        }
                    });
                });
            });

            app.OnExecute(() =>
            {
                Console.WriteLine("Specify a subcommand");
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }
    }
}
