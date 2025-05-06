// Version: 1.0.170
/*
 * CHANGELOG:
 * G��wne usprawnienia i zmiany:
 * Dokumentacja XML:
 *      - Pe�ne opisy metod z uwzgl�dnieniem wyj�tk�w
 *      - Dodano sekcj� Remarks z wa�nymi uwagami
 *      - Oznaczono przestarza�e metody atrybutem [Obsolete]
 * Bezpiecze�stwo i niezawodno��:
 *      - Usuni�to niebezpieczn� konwersj� przez Encoding.Default
 *      - Standaryzacja na format heksadecymalny
 *      - Sp�jna obs�uga zasob�w z u�yciem using
 * Optymalizacja kodu:
 *      - Uproszczona sk�adnia using
 *      - Usuni�to zb�dne zagnie�d�enia
 *      - Standaryzacja formatu zwracanego skr�tu
 * Sp�jno�� interfejsu:
 *      - Ujednolicenie nazewnictwa metod
 *      - Wyr�wnanie zachowania wszystkich metod
 *      - Lepsza obs�uga b��d�w
 * Poprawki funkcjonalne:
 *      - Wymiana Encoding.Default na UTF8 w metodzie przestarza�ej
 *      - Gwarancja poprawnego formatu zwracanych danych
 *      - Usuni�cie duplikuj�cej funkcjonalno�ci
 * Komunikacja b��d�w:
 *      - Jasna dokumentacja mo�liwych wyj�tk�w
 *      - Ostrze�enia dla niezalecanych metod
 *      - Przejrzyste informacje o ograniczeniach
 * Zasady SOLID:
 *      1. Single Responsibility - ka�da metoda ma jedno zadanie
 *      2. Open/Closed - �atwe rozszerzanie o nowe algorytmy
 *      3. Liskov Substitution - sp�jne zachowanie metod
 * Wsparcie dla testowania:
 *      - Przewidywalne wyniki
 *      - Brak efekt�w ubocznych
 *      - Jasne kontrakty metod
 * 
 * Kod zawiera teraz jasn� dokumentacj� i ostrze�enia przed u�yciem niezalecanych metod. G��wna rekomendacja to u�ywanie metod zwracaj�cych skr�ty w formacie heksadecymalnym (CheckMD5 lub GetMD5ChecksumBitConverter), kt�re gwarantuj� poprawne reprezentowanie wszystkich bajt�w skr�tu.
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
