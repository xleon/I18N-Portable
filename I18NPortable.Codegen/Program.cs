using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace I18NPortable.Codegen
{
    [Command(Description = "My global command line tool.")]
    class Program
    {
        public static int Main(string[] args) 
            => CommandLineApplication.Execute<Program>(args);

        [Argument(0, Description = "A positional parameter that must be specified.\nThe name of the person to greet.")]
        [Required]
        public string Name { get; }

        [Option(Description = "An optional parameter, with a default value.\nThe number of times to say hello.")]
        [Range(1, 1000)]
        public int Count { get; } = 1;

        private int OnExecute()
        {
            for (var i = 0; i < Count; i++)
            {
                Console.WriteLine($"Hello {Name}!");
            }
            return 0;
        }
    }
}
