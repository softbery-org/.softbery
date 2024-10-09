// Version: 10.0.0.170
// Version: 10.0.0.48
// Version: 10.0.0.47
// Version: 10.0.0.46
// Version: 10.0.0.44
// Version: 10.0.0.42
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace softbery
{
    /*
                    [Section:Header]
                    key1=value1
                    key2 = " value2 "
                    ; comment
                    # comment
                    / comment
     */

    public class Conf
    {
        private HostApplicationBuilder? _builder;
        private IHostEnvironment? _environment;
        private IConfiguration? _config;
        private IHost? _host;
        private List<FileInfo> _files;


        private readonly Template _default = new();

        public IConfiguration? Configuration { 
            get => _config;
        }

        public IHost? Services
        {
            get => _host;
        }

        public Conf(string[] files)
        {
            _files = new List<FileInfo>();
            foreach (var file in files)
            {
                try
                {
                    _files.Add(new FileInfo(file));
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"[{ex.HResult}]: {ex.Message}");
                }
            }
        }

        public async Task Run(string[] args)
        {
            if (_files != null)
            {
                foreach (var item in _files)
                {
                    _builder = Host.CreateApplicationBuilder(args);

                    await IniBackgroundWorker(args, item);
                }
            }
        }

        /// <summary>
        /// Ini background worker.
        /// </summary>
        /// <param name="args">Args from command line</param>
        /// <returns>Async host task</returns>
        private async Task IniBackgroundWorker(string[] args, FileInfo file)
        {
            if (_builder != null)
            {
                _builder.Configuration.Sources.Clear();

                _environment = _builder.Environment;

                _builder.Configuration
                    .AddIniFile($"{file.Name}.ini", optional: true, reloadOnChange: true)
                    .AddIniFile($"{file.Name}.{_environment.EnvironmentName}.ini", true, true);

                // using _host = _builder.Build();
                _host = _builder.Build();

                // Application code should start here.

                await _host.RunAsync();
            }
        }

        private void OpenConfigFile()
        {
            if (!File.Exists(".sbconf"))
            {
                File.Create(".sbconf");
                var def = File.OpenWrite(".sbconf");

            }
        }
    }
}
