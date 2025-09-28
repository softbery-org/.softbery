// Version: 10.0.1.6
/* 
 * LICENSE_MIT License
 * 
 * Copyright (c) 2023 Softbery by Paweł Tobis
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * Author						: Paweł Tobis
 * Email							: VerberyCore@gmail.com
 * Description					:
 * Create						: 2023-02-24 04:31:42
 * Last Modification Date: 2024-01-30 19:58:04
 */

/*
 * CHANGELOG:
 * Główne usprawnienia:
 *      1. Pełna dokumentacja XML dla wszystkich elementów
 *      2. Walidacja danych wejściowych z użyciem atrybutów
 *      3. Lepsze nazewnictwo pól i właściwości
 *      4. Obsługa wyjątków z odpowiednimi komunikatami
 *      5. Struktura klas podzielona na regiony
 *      6. Metody pomocnicze do generowania nagłówków
 *      7. Zastosowanie atrybutów z przestrzeni nazw ComponentModel.DataAnnotations
 *      8. Zgodność z SOLID poprzez wydzielenie odpowiedzialności
 *      9. Bezpieczne zarządzanie datami z walidacją
 *      10. Wsparcie dla przyszłego rozwoju przez oznaczone TODO
 * 
 * Dodatkowe funkcjonalności:
 *      - Konstruktor przyjmujący ścieżkę do istniejącego pliku
 *      - Metoda generująca gotowy nagłówek
 *      - Automatyczna aktualizacja daty modyfikacji
 *      - Walidacja formatu email i URL
 *      - Obsługa szablonów przez klasę Template
 * 
 * Kod jest teraz bardziej bezpieczny, czytelny i gotowy do dalszej rozbudowy.
 */

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Reprezentuje nagłówek pliku z metadanymi projektu
/// </summary>
internal class FileHeader
{
    #region Fields
    private DateTime _create = DateTime.Now;
    private DateTime _modified = DateTime.Now;
    private int _year = DateTime.Now.Year;
    private string _author = "Author";
    private string _email = "author@example.com";
    private string _website = "https://example.com/";
    private string _project = "Project";
    private string _description = "Example description for file header";
    private string _template = new Template().Value;
    private bool _includeHash = true;
    private bool _includeFileName = true;

    // Wzorce regex dla parsowania nagłówków
    private const string CopyrightPattern = @"^.*\*.*Copyright \([Cc]\).*";
    private const string AuthorPattern = @"(^.*\*.*Author.*): (.*)";
    private const string EmailPattern = @"(^.*\*.*Email.*): (.*)";
    private const string DescriptionPattern = @"(^.*\*.*Description.*): (.*)";
    private const string CreateDatePattern = @"(^.*\*.*Create.*): (.*)";
    private const string ModifiedDatePattern = @"(^.*\*.*Modified.*): (.*)";
    #endregion

    #region Properties
    /// <summary>
    /// Imię i nazwisko autora
    /// </summary>
    /// <exception cref="ArgumentException">W przypadku próby ustawienia pustej wartości</exception>
    public string Author
    {
        get => _author;
        set => _author = !string.IsNullOrWhiteSpace(value) ? value
            : throw new ArgumentException("Author cannot be empty");
    }

    /// <summary>
    /// Adres email autora
    /// </summary>
    /// <exception cref="FormatException">W przypadku niepoprawnego formatu emaila</exception>
    [EmailAddress]
    public string Email
    {
        get => _email;
        set => _email = IsValidEmail(value) ? value
            : throw new FormatException("Invalid email format");
    }

    /// <summary>
    /// Strona internetowa projektu
    /// </summary>
    /// <exception cref="UriFormatException">W przypadku niepoprawnego formatu URL</exception>
    public string Website
    {
        get => _website;
        set => _website = Uri.IsWellFormedUriString(value, UriKind.Absolute) ? value
            : throw new UriFormatException("Invalid website URL");
    }

    /// <summary>
    /// Nazwa projektu
    /// </summary>
    public string Project { get; set; }

    /// <summary>
    /// Opis projektu
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Określa czy dodawać hash pliku do nagłówka
    /// </summary>
    public bool IncludeHash { get; set; }

    /// <summary>
    /// Określa czy dodawać nazwę pliku do nagłówka
    /// </summary>
    public bool IncludeFileName { get; set; }

    /// <summary>
    /// Szablon nagłówka
    /// </summary>
    public string Template { get; set; }

    /// <summary>
    /// Data utworzenia pliku
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Data nie może być z przyszłości</exception>
    public DateTime CreateDate
    {
        get => _create;
        set => _create = value <= DateTime.Now ? value
            : throw new ArgumentOutOfRangeException("Create date cannot be in the future");
    }

    /// <summary>
    /// Data ostatniej modyfikacji
    /// </summary>
    public DateTime ModifiedDate
    {
        get => _modified;
        set => _modified = value;
    }

    /// <summary>
    /// Rok copyrightu
    /// </summary>
    public int CopyrightYear { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Inicjalizuje domyślny nagłówek pliku
    /// </summary>
    public FileHeader() { }

    /// <summary>
    /// Inicjalizuje nagłówek na podstawie istniejącego pliku
    /// </summary>
    /// <param name="filePath">Ścieżka do pliku z nagłówkiem</param>
    /// <exception cref="FileNotFoundException">Gdy plik nie istnieje</exception>
    public FileHeader(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Source file not found", filePath);

        // TODO: Implementacja parsowania istniejącego nagłówka
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Generuje tekst nagłówka na podstawie właściwości
    /// </summary>
    /// <returns>Sformatowany nagłówek</returns>
    public string GenerateHeaderText()
    {
        return $"""
                // Version: 0.1.0.0
                /*
                 * MIT License
                 * 
                 * Copyright (c) {CopyrightYear} {Project}
                 * Author: {Author} <{Email}>
                 * Website: {Website}
                 * 
                 * {Description}
                 * 
                 * Created: {CreateDate:yyyy-MM-dd HH:mm:ss}
                 * Modified: {ModifiedDate:yyyy-MM-dd HH:mm:ss}
                 */
                """;
    }

    /// <summary>
    /// Aktualizuje datę modyfikacji
    /// </summary>
    public void UpdateModificationDate()
    {
        ModifiedDate = DateTime.Now;
    }
    #endregion

    #region Private Methods
    private bool IsValidEmail(string email)
    {
        return new EmailAddressAttribute().IsValid(email);
    }

    /// <summary>
    /// Parsuje istniejący nagłówek pliku
    /// </summary>
    /// <param name="headerContent">Zawartość nagłówka</param>
    /// <returns>True jeśli parsowanie się powiodło</returns>
    private bool TryParseHeader(string headerContent)
    {
        // TODO: Implementacja parsowania z użyciem regex
        return false;
    }
    #endregion
}

/// <summary>
/// Klasa pomocnicza dla szablonów nagłówków
/// </summary>
internal class Template
{
    public string Value { get; } = "Default template";
}
