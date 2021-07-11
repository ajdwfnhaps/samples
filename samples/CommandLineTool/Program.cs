using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.Hosting.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommandLineTool
{
    ////Attribute API
    //class Program
    //{
    //    public static int Main(string[] args)
    //    => CommandLineApplication.Execute<Program>(args);

    //    [Option(Description = "The subject")]
    //    public string Subject { get; } = "world";

    //    [Option(ShortName = "n")]
    //    public int Count { get; } = 1;

    //    private void OnExecute()
    //    {
    //        for (var i = 0; i < Count; i++)
    //        {
    //            Console.WriteLine($"Hello {Subject}!");
    //        }
    //    }
    //}


    ////Builder API
    //class Program
    //{
    //    public static int Main(string[] args)
    //    {
    //        var app = new CommandLineApplication();

    //        app.HelpOption();
    //        var optionSubject = app.Option("-s|--subject <SUBJECT>", "The subject", CommandOptionType.SingleValue);
    //        var optionRepeat = app.Option<int>("-n|--count <N>", "Repeat", CommandOptionType.SingleValue);

    //        app.OnExecute(() =>
    //        {
    //            var subject = optionSubject.HasValue()
    //                ? optionSubject.Value()
    //                : "world";

    //            var count = optionRepeat.HasValue() ? optionRepeat.ParsedValue : 1;
    //            for (var i = 0; i < count; i++)
    //            {
    //                Console.WriteLine($"Hello {subject}!");
    //            }
    //            return 0;
    //        });

    //        return app.Execute(args);
    //    }
    //}

    #region Program
    [Command(Name = "di", Description = "Dependency Injection sample project")]
    class Program
    {

        [Argument(0, Description = "your name")]
        private string Name { get; } = "dependency injection";

        [Option("-l|--language", Description = "your desired language")]
        [AllowedValues("english", "spanish", "chinese", IgnoreCase = true)]
        public string Language { get; } = "english";

        public static async Task<int> Main(string[] args)
        {
            return await new HostBuilder()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IGreeter, Greeter>()
                        .AddSingleton<IConsole>(PhysicalConsole.Singleton);
                })
                .RunCommandLineApplicationAsync<Program>(args);
        }

        private readonly ILogger<Program> _logger;
        private readonly IGreeter _greeter;

        public Program(ILogger<Program> logger, IGreeter greeter)
        {
            _logger = logger;
            _greeter = greeter;

            _logger.LogInformation("Constructed!");
        }

        private void OnExecute()
        {
            _greeter.Greet(Name, Language);
        }
    }
    #endregion

    #region IGreeter
    interface IGreeter
    {
        void Greet(string name, string language);
    }
    #endregion

    #region Greeter
    class Greeter : IGreeter
    {
        private readonly IConsole _console;
        private readonly ILogger<Greeter> _logger;

        public Greeter(ILogger<Greeter> logger,
            IConsole console)
        {
            _logger = logger;
            _console = console;

            _logger.LogInformation("Constructed!");
        }

        public void Greet(string name, string language = "english")
        {
            string greeting;
            switch (language)
            {
                case "english": greeting = "Hello {0}"; break;
                case "spanish": greeting = "Hola {0}"; break;
                case "chinese": greeting = "您好 {0}"; break;
                default: throw new InvalidOperationException("validation should have caught this");
            }
            _console.WriteLine(greeting, name);
        }
    }
    #endregion


    //#region Program
    //[Command(Name = "di", Description = "Dependency Injection sample project")]
    //[HelpOption]
    //class Program
    //{
    //    public static int Main(string[] args)
    //    {
    //        var services = new ServiceCollection()
    //            .AddSingleton<IMyService, MyServiceImplementation>()
    //            .AddSingleton<IConsole>(PhysicalConsole.Singleton)
    //            .BuildServiceProvider();

    //        var app = new CommandLineApplication<Program>();
    //        app.Conventions
    //            .UseDefaultConventions()
    //            .UseConstructorInjection(services);
    //        return app.Execute(args);
    //    }

    //    private readonly IMyService _myService;

    //    public Program(IMyService myService)
    //    {
    //        _myService = myService;
    //    }

    //    private void OnExecute()
    //    {
    //        _myService.Invoke();
    //    }
    //}
    //#endregion

    //#region IMyService
    //interface IMyService
    //{
    //    void Invoke();
    //}
    //#endregion

    //#region MyServiceImplementation
    //class MyServiceImplementation : IMyService
    //{
    //    private readonly IConsole _console;

    //    public MyServiceImplementation(IConsole console)
    //    {
    //        _console = console;
    //    }

    //    public void Invoke()
    //    {
    //        _console.WriteLine("Hello dependency injection!");
    //    }
    //}
    //#endregion
}
