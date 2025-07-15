// Version: 10.0.0.171
/*
 * CHANGES:
 * G��wne zmiany i usprawnienia:
 * Struktura klas:
 *     - Wyodr�bnienie nowych metod dla zachowania zasady pojedynczej odpowiedzialno�ci
 * 	    - Lepsze rozdzielenie logiki inicjalizacji i uruchamiania
 * Dokumentacja XML:
 *      - Pe�ne opisy wszystkich klas i metod
 * 	    - Dokumentacja parametr�w i zwracanych warto�ci
 * 	    - Opisy wyj�tk�w i b��d�w
 * Bezpiecze�stwo:
 *      - Poprawiona obs�uga wyj�tk�w
 * 	    - W�a�ciwe zarz�dzanie zasobami (using dla strumieni plik�w)
 *  	- Walidacja wej��
 * Czytelno�� kodu:
 *      - Sp�jne formatowanie
 *      - Opisowe nazwy metod
 *      - Logiczny podzia� funkcjonalno�ci
 * Rozszerzalno��:
 *      - Oznaczone miejsca do dalszego rozwoju (TODO)
 * 	    - Mo�liwo�� �atwego dodawania nowych format�w konfiguracji
 * Zasady SOLID:
 *      1. Single Responsibility - ka�da metoda ma jedno zadanie
 * 	     2. Open/Closed - �atwe rozszerzanie przez dziedziczenie
 * 	     3. Dependency Inversion - abstrakcja konfiguracji
 * Optymalizacja:
 * - Unikanie powtarzaj�cego si� kodu
 * 	- Efektywne wykorzystanie zasob�w
 * 	- Asynchroniczne operacje
 * 	
 * Kod wymaga jeszcze implementacji brakuj�cych funkcjonalno�ci 
 * (klasa Template, domy�lna zawarto�� konfiguracji), ale g��wna 
 * struktura jest przygotowana pod dalszy rozw�j.
**/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace VerberyCore
{
    /// <summary>
    /// Reprezentuje g��wn� klas� konfiguracyjn� aplikacji odpowiedzialn� za zarz�dzanie plikami konfiguracyjnymi i hostem
    /// </summary>
    public class Conf
    {
        private HostApplicationBuilder? _builder;
        private IHostEnvironment? _environment;
        private IConfiguration? _config;
        private IHost? _host;
        private List<FileInfo> _files;
        private readonly Template _default = new();

        /// <summary>
        /// Aktualna konfiguracja aplikacji
        /// </summary>
        public IConfiguration? Configuration => _config;

        /// <summary>
        /// Dost�pne us�ugi hosta
        /// </summary>
        public IHost? Services => _host;

        /// <summary>
        /// Inicjalizuje now� instancj� klasy Conf z podanymi plikami konfiguracyjnymi
        /// </summary>
        /// <param name="files">Lista �cie�ek do plik�w konfiguracyjnych</param>
        public Conf(string[] files)
        {
            _files = new List<FileInfo>();
            if (files != null)
            {
                InitializeConfigurationFiles(files);
            }
        }

        /// <summary>
        /// G��wna metoda uruchamiaj�ca proces konfiguracji
        /// </summary>
        /// <param name="args">Argumenty wiersza polece�</param>
        /// <returns>Zadanie asynchroniczne</returns>
        public async Task Run(string[] args)
        {
            if (_files != null && _files.Any())
            {
                await ProcessConfigurationFiles(args);
            }
        }

        /// <summary>
        /// Inicjalizuje pliki konfiguracyjne z podanych �cie�ek
        /// </summary>
        /// <param name="filePaths">Tablica �cie�ek do plik�w</param>
        private void InitializeConfigurationFiles(string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                try
                {
                    _files.Add(new FileInfo(filePath));
                }
                catch (Exception ex)
                {
                    HandleConfigurationError(ex, filePath);
                }
            }
        }

        /// <summary>
        /// Przetwarza wszystkie pliki konfiguracyjne
        /// </summary>
        /// <param name="args">Argumenty wiersza polece�</param>
        /// <returns>Zadanie asynchroniczne</returns>
        private async Task ProcessConfigurationFiles(string[] args)
        {
            foreach (var file in _files)
            {
                await InitializeHostForFile(args, file);
            }
        }

        /// <summary>
        /// Inicjalizuje host dla pojedynczego pliku konfiguracyjnego
        /// </summary>
        /// <param name="args">Argumenty wiersza polece�</param>
        /// <param name="file">Plik konfiguracyjny</param>
        /// <returns>Zadanie asynchroniczne</returns>
        private async Task InitializeHostForFile(string[] args, FileInfo file)
        {
            try
            {
                _builder = Host.CreateApplicationBuilder(args);
                await ConfigureAndRunHost(args, file);
            }
            catch (Exception ex)
            {
                HandleHostInitializationError(ex, file);
            }
        }

        /// <summary>
        /// Konfiguruje i uruchamia hosta
        /// </summary>
        /// <param name="args">Argumenty wiersza polece�</param>
        /// <param name="file">Plik konfiguracyjny</param>
        /// <returns>Zadanie asynchroniczne</returns>
        private async Task ConfigureAndRunHost(string[] args, FileInfo file)
        {
            ConfigureBuilder(file);
            BuildHost();
            await RunHostAsync();
        }

        /// <summary>
        /// Konfiguruje builder hosta
        /// </summary>
        /// <param name="file">Plik konfiguracyjny</param>
        private void ConfigureBuilder(FileInfo file)
        {
            if (_builder == null) return;

            _builder.Configuration.Sources.Clear();
            _environment = _builder.Environment;

            _builder.Configuration
                .AddIniFile($"{file.Name}.ini", optional: true, reloadOnChange: true)
                .AddIniFile($"{file.Name}.{_environment.EnvironmentName}.ini", true, true);
        }

        /// <summary>
        /// Buduje hosta
        /// </summary>
        private void BuildHost()
        {
            _host = _builder?.Build();
        }

        /// <summary>
        /// Uruchamia hosta
        /// </summary>
        /// <returns>Zadanie asynchroniczne</returns>
        private async Task RunHostAsync()
        {
            if (_host != null)
            {
                await _host.RunAsync();
            }
        }

        /// <summary>
        /// Tworzy domy�lny plik konfiguracyjny je�li nie istnieje
        /// </summary>
        private void OpenConfigFile()
        {
            const string configFile = ".sb/.sbconf";

            if (!File.Exists(configFile))
            {
                try
                {
                    using var fileStream = File.Create(configFile);
                    // TODO: Doda� domy�ln� zawarto�� konfiguracyjn�
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating config file: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Obs�uguje b��dy inicjalizacji konfiguracji
        /// </summary>
        /// <param name="ex">Wyj�tek</param>
        /// <param name="filePath">�cie�ka pliku</param>
        private void HandleConfigurationError(Exception ex, string filePath)
        {
            Console.WriteLine($"[{ex.GetType().Name}] Error loading config file '{filePath}': {ex.Message}");
        }

        /// <summary>
        /// Obs�uguje b��dy inicjalizacji hosta
        /// </summary>
        /// <param name="ex">Wyj�tek</param>
        /// <param name="file">Plik konfiguracyjny</param>
        private void HandleHostInitializationError(Exception ex, FileInfo file)
        {
            Console.WriteLine($"[{ex.GetType().Name}] Error initializing host for '{file.Name}': {ex.Message}");
        }
    }

    /// <summary>
    /// Klasa pomocnicza dla domy�lnych ustawie� konfiguracyjnych
    /// </summary>
    public class Template
    {
        // TODO: Doda� implementacj� domy�lnego szablonu konfiguracji
        /// <summary>
        /// Domy�lny szablon licencji LICENSE_MIT
        /// </summary>
        public string Value = "/* " +
                                               " * LICENSE_MIT License" +
                                               " * " +
                                               " * Copyright (c) {year} {company} by {author}" +
                                               " * " +
                                               " * Permission is hereby granted, free of charge, to any person obtaining a copy" +
                                               " * of this software and associated documentation files (the \"Software\"), to deal" +
                                               " * in the Software without restriction, including without limitation the rights" +
                                               " * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell" +
                                               " * copies of the Software, and to permit persons to whom the Software is" +
                                               " * furnished to do so, subject to the following conditions:" +
                                               " * " +
                                               " * The above copyright notice and this permission notice shall be included in all" +
                                               " * copies or substantial portions of the Software." +
                                               " * " +
                                               " * THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR" +
                                               " * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY," +
                                               " * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE" +
                                               " * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER" +
                                               " * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM," +
                                               " * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE" +
                                               " * SOFTWARE." +
                                               " *" +
                                               " * Project						    : {project} " +
                                               " * " +
                                               " * file							    : {file}" +
                                               " * Description				    : {description}" +
                                               " *" +
                                               " *" +
                                               " * Author						    : {author}" +
                                               " * Email							: {email}" +
                                               " * Website						: {website}" +
                                               " * Create	    				    : {create}" +
                                               " * Last Modification Date: {modified}" +
                                               " */";
        /// <summary>
        /// Domy�lny szablon licencji LICENSE_MIT
        /// </summary>
        public string LICENSE_MIT =
               "/* \r\n * LICENSE_MIT License\r\n " +
                "* \r\n " +
                "* Copyright (c) {year} {company} by {author}\r\n " +
                "* \r\n " +
                "* Permission is hereby granted, free of charge, to any person obtaining a copy\r\n " +
                "* of this software and associated documentation files (the \"Software\"), to deal\r\n " +
                "* in the Software without restriction, including without limitation the rights\r\n " +
                "* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell\r\n " +
                "* copies of the Software, and to permit persons to whom the Software is\r\n " +
                "* furnished to do so, subject to the following conditions:\r\n * \r\n " +
                "* The above copyright notice and this permission notice shall be included in all\r\n " +
                "* copies or substantial portions of the Software.\r\n * \r\n " +
                "* THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\r\n " +
                "* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\r\n " +
                "* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\r\n " +
                "* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\r\n " +
                "* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\r\n " +
                "* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE\r\n " +
                "* SOFTWARE.\r\n " +
                "* \r\n " +
                "* Author\t\t\t\t\t\t: {author}\r\n " +
                "* Email\t\t\t\t\t\t\t: {email}\r\n " +
                "* Description\t\t\t\t\t: {description}\r\n " +
                "* Create\t\t\t\t\t\t: {create}\r\n " +
                "* Last Modification Date: {modified} \r\n " +
                "*/";
    }
}
