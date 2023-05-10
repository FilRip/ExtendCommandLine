using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace ExtendCommandLineLib
{
    /// <summary>
    /// Classe contenant des extensions pour la classe String
    /// </summary>
    public static class ExtensionsString
    {
        /// <summary>
        /// Efface toutes les occurences d'une chaine de caractère, d'une autre chaine de caractère
        /// </summary>
        /// <param name="str">Chaine source, contenant les caractères à effacer</param>
        /// <param name="strToRemove">Caractère ou chaine de caractères à supprimer</param>
        public static string RemoveString(this string str, string strToRemove)
        {
            return str.Replace(strToRemove, "");
        }
        /// <summary>
        /// Efface toutes les occurences d'un caractère, d'une autre chaine de caractère
        /// </summary>
        /// <param name="str">Chaine source, contenant le caractère à effacer</param>
        /// <param name="charToRemove">Caractère à supprimer</param>
        public static string RemoveChar(this string str, char charToRemove)
        {
            return RemoveString(str, charToRemove.ToString());
        }

        /// <summary>
        /// Décrypte une chaine de caractère crypté en AES 256 bits
        /// </summary>
        /// <param name="str">Chaine de caractère à décrypter</param>
        /// <param name="key">Clé AES 256 bits</param>
        /// <param name="vector">Vecteur AES 256 bits</param>
        /// <returns>Chaine de caractères décryptée</returns>
        [System.Reflection.Obfuscation()]
        public static string DecrypteChaineAES(this string str, byte[] key, byte[] vector)
        {
            string result = "";

#pragma warning disable S5542
            AesManaged aes = new()
            {
                Key = key,
            };
#pragma warning restore S5542
            if (vector.Length > 16) Array.Resize(ref vector, 16);
            aes.IV = vector;
            ICryptoTransform decrypteur = aes.CreateDecryptor(key, vector);

            using (MemoryStream msDecrypt = new(Convert.FromBase64String(str)))
            {
                using CryptoStream csDecrypt = new(msDecrypt, decrypteur, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Crypte une chaine de caractère en AES 256 bits
        /// </summary>
        /// <param name="str">Chaine de caractère à crypter</param>
        /// <param name="key">Clé AES 256 bits</param>
        /// <param name="vector">Vecteur AES 256 bits</param>
        /// <returns>Chaine de caractères cryptée</returns>
        [System.Reflection.Obfuscation()]
        public static string EncrypteChaineAES(this string str, byte[] key, byte[] vector)
        {
            byte[] result = null;

#pragma warning disable S5542
            AesManaged aes = new()
            {
                Key = key,
            };
#pragma warning restore S5542
            if (vector.Length > 16) Array.Resize(ref vector, 16);
            aes.IV = vector;
            ICryptoTransform encrypteur = aes.CreateEncryptor(key, vector);

            using (MemoryStream msEncrypt = new())
            {
                using CryptoStream csEncrypt = new(msEncrypt, encrypteur, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(str);
                }
                result = msEncrypt.ToArray();
            }

            return (result != null ? Convert.ToBase64String(result) : "");
        }

        /// <summary>
        /// Retourne le hashage SHA256 d'une chaine de caractères encodée en UTF8 (encodage fr par défaut)
        /// </summary>
        /// <param name="str">Chaine de caractère</param>
        public static byte[] GetHashSHA256(this string str)
        {
            return GetHashSHA256(str, Encoding.UTF8);
        }
        /// <summary>
        /// Retourne le hashage SHA256 d'une chaine de caractères en spécifiant l'encodage de la chaine de caractères source
        /// </summary>
        /// <param name="str">Chaine de caractères</param>
        /// <param name="encoder">Encodage de la chaine de caractères</param>
        public static byte[] GetHashSHA256(this string str, Encoding encoder)
        {
            SHA256Managed sha = new();
            return sha.ComputeHash(encoder.GetBytes(str));
        }

        /// <summary>
        /// Encode une chaine de caractères au format UTF8 (format fr par défaut) en format Base64
        /// </summary>
        /// <param name="str">Chaine de caractères UTF8 à encoder</param>
        public static string ToBase64(this string str)
        {
            return ToBase64(str, Encoding.UTF8);
        }
        /// <summary>
        /// Encode une chaine de caractères en format Base64
        /// </summary>
        /// <param name="str">Chaine de caractères à encoder</param>
        /// <param name="encoder">Format de la chaine de caractères à encoder</param>
        public static string ToBase64(this string str, Encoding encoder)
        {
            return Convert.ToBase64String(encoder.GetBytes(str));
        }

        /// <summary>
        /// Décode une chaine de caractères en format Base64 vers un format UTF8 (fr par défaut)
        /// </summary>
        /// <param name="str">Chaine de caractères au format Base64</param>
        public static string FromBase64(this string str)
        {
            return FromBase64(str, Encoding.UTF8);
        }
        /// <summary>
        /// Décode une chaine de caractères en format Base64 vers un format spécifique
        /// </summary>
        /// <param name="str">Chaine de caractères au format Base64</param>
        /// <param name="encoder">Format de destination de la chaine de caractères décodée</param>
        public static string FromBase64(this string str, Encoding encoder)
        {
            return encoder.GetString(Convert.FromBase64String(str));
        }

        /// <summary>
        /// Retourne une représentation en chaine de caractères d'un tableau de chaine de caractères
        /// (Remplace le ToString d'un tableau. Le ToString d'un tableau ne retourne pas les valeurs de ce tableau, cette méthode retourne les valeurs du tableau)
        /// </summary>
        /// <param name="list">Tableau de chaines de caractères</param>
        /// <param name="separator">Caractère (ou chaine de caractères) à utiliser comme séparateur entre chaque valeur du tableau</param>
        public static string ToListOfString(this string[] list, string separator = ",")
        {
            StringBuilder retour = new();

            if (list.Length > 0)
                foreach (string item in list)
                    retour.Append(separator + item);

            if (retour.ToString().Length > 0)
                return retour.ToString().Substring(1);
            else
                return retour.ToString();
        }

        /// <summary>
        /// Indique si une chaine de caractères précise est présente dans une chaine de charactères
        /// </summary>
        /// <param name="str">Chaine de caratères source</param>
        /// <param name="subString">Chaine de caractères à rechercher</param>
        /// <param name="comparator">Comparateur à utiliser (généralement type de chaine et spécifie si l'on doit ignorer la casse ou pas)</param>
        /// <returns>True si chaine trouvée, sinon False</returns>
        public static bool Contains(this string str, string subString, StringComparison comparator)
        {
            if ((!string.IsNullOrWhiteSpace(str)) && (!string.IsNullOrWhiteSpace(subString)))
                return str.IndexOf(subString, comparator) >= 0;
            return false;
        }

        /// <summary>
        /// Remplace une chaine de caractère(s) par une autre avec prise en compte de la culture et de la casse ou pas
        /// </summary>
        /// <param name="str">Chaine source contenant la(les) chaine(s) de caractère(s) à remplacer</param>
        /// <param name="subString">Chaine de caractère(s) à remplacer</param>
        /// <param name="Remplacement">Nouvelle chaine de caractère(s) de remplacement</param>
        /// <param name="comparator">Comparateur à utiliser</param>
        public static string Replace(this string str, string subString, string Remplacement, StringComparison comparator)
        {
            StringBuilder retour = new();

            if (str == null) throw new ArgumentNullException(nameof(str));
            if (str.Trim().Length == 0) return str;

            if (string.IsNullOrWhiteSpace(subString)) throw new ArgumentNullException(nameof(subString));

            bool boucle = true;
            while (boucle)
            {
                boucle = false;
                int trouve;
                if ((trouve = str.IndexOf(subString, 0, comparator)) >= 0)
                {
                    retour.Clear();
                    retour = new(str.Substring(0, trouve));
                    retour.Append(Remplacement);
                    retour.Append(str.Substring(trouve + subString.Length));
                    str = retour.ToString();
                    boucle = true;
                }
            }

            return retour.Length == 0 ? str : retour.ToString();
        }

        /// <summary>
        /// Supprime les caractères spécifié au début et à la fin d'une chaine
        /// </summary>
        /// <param name="str">Chaine de caractères sources, contenant les caractères à supprimer</param>
        /// <param name="strToRemove">Chaine de caractères à supprimer</param>
        public static string Trim(this string str, string strToRemove)
        {
            string retour = str;

            retour = retour.TrimStart(strToRemove).TrimEnd(strToRemove);

            return retour;
        }

        /// <summary>
        /// Supprime les caractères spécifié au début d'une chaine
        /// </summary>
        /// <param name="str">Chaine de caractères sources, contenant les caractères à supprimer</param>
        /// <param name="strToRemove">Chaine de caractères à supprimer</param>
        public static string TrimStart(this string str, string strToRemove)
        {
            string retour = str;
            bool boucle = true;

            while (boucle)
            {
                boucle = false;
                if (retour.StartsWith(strToRemove))
                {
                    boucle = true;
                    retour = retour.Substring(strToRemove.Length);
                }
            }

            return retour;
        }

        /// <summary>
        /// Supprime les caractères spécifié à la fin d'une chaine
        /// </summary>
        /// <param name="str">Chaine de caractères sources, contenant les caractères à supprimer</param>
        /// <param name="strToRemove">Chaine de caractères à supprimer</param>
        public static string TrimEnd(this string str, string strToRemove)
        {
            string retour = str;
            bool boucle = true;

            while (boucle)
            {
                boucle = false;
                if (retour.EndsWith(strToRemove))
                {
                    boucle = true;
                    retour = retour.Substring(0, retour.Length - strToRemove.Length);
                }
            }

            return retour;
        }

        /// <summary>
        /// Supprime tous les doublons, boucle tant qu'ils y en a
        /// Exemple : supprime 3, 4, 5, ... même caractère spécifié, à la suite jusqu'à ce qu'il n'en reste plus qu'un
        /// </summary>
        /// <param name="str">Chaine de caractères contenant les doublons à supprimer</param>
        /// <param name="charDuplicatedToRemove">Caractère dont il faut supprimer les doulons à la suite</param>
        public static string RemoveDuplicate(this string str, char charDuplicatedToRemove)
        {
            string retour = str;
            while (retour.IndexOf(charDuplicatedToRemove.ToString() + charDuplicatedToRemove.ToString()) >= 0)
                retour = retour.Replace(charDuplicatedToRemove.ToString() + charDuplicatedToRemove.ToString(), charDuplicatedToRemove.ToString());
            return retour;
        }

        /// <summary>
        /// Supprime tous les doublons, boucle tant qu'ils y en a
        /// Exemple : supprime 3, 4, 5, ... même chaine spécifiée, à la suite jusqu'à ce qu'il n'en reste plus qu'une
        /// </summary>
        /// <param name="src">Chaine de caractères contenant les doublons à supprimer</param>
        /// <param name="strDuplicatedToRemove">Chaine de caractères dont il faut supprimer les doulons à la suite</param>
        public static string RemoveDuplicate(this string src, string strDuplicatedToRemove)
        {
            string retour = src;
            while (retour.IndexOf(strDuplicatedToRemove + strDuplicatedToRemove) >= 0)
                retour = retour.Replace(strDuplicatedToRemove + strDuplicatedToRemove, strDuplicatedToRemove);
            return retour;
        }

        /// <summary>
        /// Convertie une chaine en SecureString en mémoire
        /// </summary>
        /// <param name="src">Chaine à convertir en SecureString</param>
        public static SecureString ToSecureString(this string src)
        {
            SecureString retour = new();

            if (!string.IsNullOrEmpty(src))
                src.ToCharArray().ToList().ForEach(c => retour.AppendChar(c));

            return retour;
        }

        /// <summary>
        /// Retourne la même chaine mais tronquée si jamais elle dépasse la taille maximum donnée en paramètre
        /// </summary>
        /// <param name="src">Chaine a tronquer</param>
        /// <param name="maximumSize">Taille maximum acceptée de la chaine</param>
        public static string ReturnStringWithMaxSize(this string src, int maximumSize)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (src.Length > maximumSize)
                return src.Substring(0, maximumSize);
            else
                return src;
        }

        /// <summary>
        /// Retourne une chaine de caractères, avec des caractères aléatoire (lettres et chiffres), de 10 caractères taille spécifiée
        /// </summary>
        public static string RandomString()
        {
            return RandomString(10, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        }

        /// <summary>
        /// Retourne une chaine de caractères, avec des caractères aléatoire (lettres et chiffres), d'une taille spécifiée
        /// </summary>
        /// <param name="size">Taille de la chaine de caractère</param>
        public static string RandomString(int size)
        {
            return RandomString(size, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        }

        /// <summary>
        /// Retourne une chaine de caractères, avec des caractères aléatoire (lettres et chiffres), d'une taille spécifiée
        /// </summary>
        /// <param name="size">Taille de la chaine de caractère</param>
        /// <param name="charToUse">Liste des caractères possibles pour générer la chaine de caractère aléatoire</param>
        public static string RandomString(int size, string charToUse)
        {
            return new string(Enumerable.Repeat(charToUse, size).Select(s => s[new Random(Guid.NewGuid().GetHashCode()).Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Retourne un tableau de chaines qui contient les sous-chaines de cette chaine, délimitées par la chaine spécifiée
        /// </summary>
        /// <param name="str">Chaine à séparer</param>
        /// <param name="separator">Chaine séparateur</param>
        public static string[] Split(this string str, string separator)
        {
            return Split(str, separator, StringSplitOptions.None);
        }

        /// <summary>
        /// Retourne un tableau de chaines qui contient les sous-chaines de cette chaine, délimitées par la chaine spécifiée
        /// </summary>
        /// <param name="str">Chaine à séparer</param>
        /// <param name="separator">Chaine séparateur</param>
        /// <param name="splitOptions">Option pour retourner ou non les sous-chaines vides</param>
        public static string[] Split(this string str, string separator, StringSplitOptions splitOptions)
        {
            List<string> retour = new();

            while (str.IndexOf(separator) >= 0)
            {
                retour.Add(str.Substring(0, str.IndexOf(separator)));
                str = str.Substring(str.IndexOf(separator) + separator.Length);
            }
            retour.Add(str);
            if (splitOptions == StringSplitOptions.RemoveEmptyEntries)
                retour.RemoveAll(ligne => string.IsNullOrWhiteSpace(ligne));

            return retour.ToArray();
        }

        /// <summary>
        /// Retourne le mot précédent dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="str">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot précédent</param>
        public static string PreviousWord(this string str, ref int position)
        {
            return str.PreviousWord(ref position, true);
        }

        /// <summary>
        /// Retourne le mot précédent dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="str">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot précédent</param>
        /// <param name="ignoreNewlineChar">Ignore ou non les retours à la ligne</param>
        public static string PreviousWord(this string str, ref int position, bool ignoreNewlineChar)
        {
            string retour = "";
            if (string.IsNullOrWhiteSpace(str))
                return null;
            if (position > str.Length - 1)
                position = str.Length - 1;
            if (position == 0)
                return retour;
            while ((str[position] != ' ' && position > 0 && ignoreNewlineChar && str[position] != '\r' && str[position] != '\n') || retour == "")
            {
                retour = str[position] + retour.Trim();
                position--;
            }
            return retour;
        }

        /// <summary>
        /// Retourne le mot suivant dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="str">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot suivant</param>
        public static string NextWord(this string str, ref int position)
        {
            return str.NextWord(ref position, true);
        }

        /// <summary>
        /// Retourne le mot suivant dans une chaine de caractères, à partir de la position donnée
        /// </summary>
        /// <param name="str">Chaine de caractères</param>
        /// <param name="position">Position dans la chaine de caractère ou commencer à rechercher le mot suivant</param>
        /// <param name="ignoreNewlineChar">Ignore ou non les retours à la ligne</param>
        public static string NextWord(this string str, ref int position, bool ignoreNewlineChar)
        {
            StringBuilder retour = new();
            if (string.IsNullOrWhiteSpace(str))
                return null;
            if (position > str.Length - 1)
                return retour.ToString();
            while ((str[position] != ' ' && position < str.Length - 1 && ignoreNewlineChar && str[position] != '\r' && str[position] != '\n') || retour.Length == 0)
            {
                retour.Append(str[position].ToString().Trim());
                position++;
            }
            return retour.ToString();
        }

        /// <summary>
        /// Retourne si oui ou non la chaine commence par la caractère spécifié
        /// </summary>
        /// <param name="str">chaine source dans laquelle rechercher</param>
        /// <param name="charToStart">Caractère à rechercher</param>
        public static bool StartsWith(this string str, char charToStart)
        {
            return str.StartsWith(charToStart.ToString());
        }

        /// <summary>
        /// Retourne si oui ou non la chaine commence par la caractère spécifié
        /// </summary>
        /// <param name="str">chaine source dans laquelle rechercher</param>
        /// <param name="charToStart">Caractère à rechercher</param>
        /// <param name="comparator">Comparateur à utiliser pour la recherche du caractère dans la chaine</param>
        public static bool StartsWith(this string str, char charToStart, StringComparison comparator)
        {
            return str.StartsWith(charToStart.ToString(), comparator);
        }

        /// <summary>
        /// Retourne si oui ou non la chaine commence par la caractère spécifié
        /// </summary>
        /// <param name="str">chaine source dans laquelle rechercher</param>
        /// <param name="charToStart">Caractère à rechercher</param>
        /// <param name="ignoreCase">Faut-il ignorer la casse</param>
        /// <param name="culture">Quelle culture/langage doit être utilisée</param>
        public static bool StartsWith(this string str, char charToStart, bool ignoreCase, CultureInfo culture)
        {
            return str.StartsWith(charToStart.ToString(), ignoreCase, culture);
        }

        /// <summary>
        /// Retourne si oui ou non la chaine se termine par la caractère spécifié
        /// </summary>
        /// <param name="str">chaine source dans laquelle rechercher</param>
        /// <param name="charToEnd">Caractère à rechercher</param>
        public static bool EndsWith(this string str, char charToEnd)
        {
            return str.EndsWith(charToEnd.ToString());
        }

        /// <summary>
        /// Retourne si oui ou non la chaine se termine par la caractère spécifié
        /// </summary>
        /// <param name="str">chaine source dans laquelle rechercher</param>
        /// <param name="charToEnd">Caractère à rechercher</param>
        /// <param name="comparator">Comparateur à utiliser pour la recherche du caractère dans la chaine</param>
        public static bool EndsWith(this string str, char charToEnd, StringComparison comparator)
        {
            return str.EndsWith(charToEnd.ToString(), comparator);
        }
        /// <summary>
        /// Retourne si oui ou non la chaine se termine par la caractère spécifié
        /// </summary>
        /// <param name="str">chaine source dans laquelle rechercher</param>
        /// <param name="charToEnd">Caractère à rechercher</param>
        /// <param name="ignoreCase">Faut-il ignorer la casse</param>
        /// <param name="culture">Quelle culture/langage doit être utilisée</param>
        public static bool EndsWith(this string str, char charToEnd, bool ignoreCase, CultureInfo culture)
        {
            return str.EndsWith(charToEnd.ToString(), ignoreCase, culture);
        }

        public static string ToNumberFormat(this long @int)
        {
            return @int.ToNumberFormat(" ", 0, null);
        }
        public static string ToNumberFormat(this long @int, string thousandSeparator, int numberDecimal, string decimalSeparator)
        {
            NumberFormatInfo nfi = new()
            {
                NumberGroupSeparator = thousandSeparator,
                NumberDecimalDigits = numberDecimal,
                NumberDecimalSeparator = decimalSeparator ?? CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
            };
            return @int.ToString("n", nfi);
        }
    }
}
