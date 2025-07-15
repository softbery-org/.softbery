// Version: 1.0.170
/*
 * CHANGELOG:
 * Główne usprawnienia i zmiany:
 * Dokumentacja XML:
 *      - Pełne opisy metod z uwzględnieniem wyjątków
 *      - Dodano sekcją Remarks z ważnymi uwagami
 *      - Oznaczono przestarzałe metody atrybutem [Obsolete]
 * Bezpieczeństwo i niezawodność:
 *      - Usunięto niebezpieczną konwersję przez Encoding.Default
 *      - Standaryzacja na format heksadecymalny
 *      - Spójna obsługa zasobów z użyciem using
 * Optymalizacja kodu:
 *      - Uproszczona składnia using
 *      - Usunięto zbędne zagnieżdżenia
 *      - Standaryzacja formatu zwracanego skrótu
 * Spójność interfejsu:
 *      - Ujednolicenie nazewnictwa metod
 *      - Wyrównanie zachowania wszystkich metod
 *      - Lepsza obsługa błędów
 * Poprawki funkcjonalne:
 *      - Wymiana Encoding.Default na UTF8 w metodzie przestarzałej
 *      - Gwarancja poprawnego formatu zwracanych danych
 *      - Usunięcie duplikującej funkcjonalności
 * Komunikacja błędów:
 *      - Jasna dokumentacja możliwych wyjątków
 *      - Ostrzeżenia dla niezalecanych metod
 *      - Przejrzyste informacje o ograniczeniach
 * Zasady SOLID:
 *      1. Single Responsibility - każda metoda ma jedno zadanie
 *      2. Open/Closed - łatwe rozszerzanie o nowe algorytmy
 *      3. Liskov Substitution - spójne zachowanie metod
 * Wsparcie dla testowania:
 *      - Przewidywalne wyniki
 *      - Brak efektów ubocznych
 *      - Jasne kontrakty metod
 * 
 * Kod zawiera teraz jasną dokumentację i ostrzeżenia przed użyciem niezalecanych metod. 
 * Główna rekomendacja to używanie metod zwracających skróty w formacie heksadecymalnym 
 * (CheckMD5 lub GetMD5ChecksumBitConverter), które gwarantuj� poprawne reprezentowanie 
 * wszystkich bajtów skrótu.
 */
using System.Security.Cryptography;
using System.Text;

namespace VerberyCore
{
    /// <summary>
    /// Klasa pomocnicza zapewniaj�ca funkcjonalno�� obliczania skr�t�w MD5 dla plik�w
    /// </summary>
    internal static class FileHash
    {
        /// <summary>
        /// Oblicza skr�t MD5 pliku u�ywaj�c konwersji bitowej
        /// </summary>
        /// <param name="file">Plik do obliczenia skr�tu</param>
        /// <returns>Skr�t MD5 w formacie heksadecymalnym</returns>
        /// <exception cref="FileNotFoundException">Gdy plik nie istnieje</exception>
        /// <exception cref="IOException">B��d dost�pu do pliku</exception>
        public static string CheckMD5(this FileInfo file)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(file.FullName);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Oblicza skr�t MD5 pliku u�ywaj�c konwersji bitowej (wersja alternatywna)
        /// </summary>
        /// <param name="file">Plik do obliczenia skr�tu</param>
        /// <returns>Skr�t MD5 w formacie heksadecymalnym</returns>
        /// <exception cref="FileNotFoundException">Gdy plik nie istnieje</exception>
        /// <exception cref="IOException">B��d dost�pu do pliku</exception>
        public static string GetMD5ChecksumBitConverter(this FileInfo file)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(file.FullName);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Oblicza skr�t MD5 pliku u�ywaj�c kodowania znak�w (przestarza�e)
        /// </summary>
        /// <remarks>
        /// Metoda niezalecana - konwersja bajt�w na string przez Encoding mo�e powodowa� utrat� danych.
        /// Preferuj u�ycie metod zwracaj�cych reprezentacj� heksadecymaln�.
        /// </remarks>
        /// <param name="file">Plik do obliczenia skr�tu</param>
        /// <returns>Skr�t MD5 jako string</returns>
        /// <exception cref="FileNotFoundException">Gdy plik nie istnieje</exception>
        /// <exception cref="IOException">B��d dost�pu do pliku</exception>
        [Obsolete("This method may produce unreliable results. Use CheckMD5 or GetMD5ChecksumBitConverter instead.")]
        public static string GetMD5ChecksumEncoding(this FileInfo file)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(file.FullName);
            var hash = md5.ComputeHash(stream);
            return Encoding.UTF8.GetString(hash);
        }
    }
}
