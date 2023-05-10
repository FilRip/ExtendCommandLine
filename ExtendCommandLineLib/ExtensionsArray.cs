using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtendCommandLineLib
{
    /// <summary>
    /// Classe contenant des méthodes d'extension de gestion des tableaux
    /// </summary>
    public static class ExtensionsArray
    {
        /// <summary>
        /// Supprime une liste d'objet qui se suive d'un tableau
        /// </summary>
        /// <typeparam name="T">Type de la liste</typeparam>
        /// <param name="list">Liste contenant l'index à supprimer</param>
        /// <param name="index">Numéro du début dans le tableau de(s) objet(s) à supprimer de la liste</param>
        /// <param name="nb">Nombre d'éléments à supprimer de la liste</param>
        /// <returns>Un nouveau tableau avec les éléments demandé supprimé</returns>
        public static T[] RemoveRange<T>(this T[] list, int index, int nb)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

#pragma warning disable S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur
            if (index > list.Length - 1)
                throw new IndexOutOfRangeException();
#pragma warning restore S112

            T[] retour = list;
            for (int i = index; i < index + nb; i++)
                if (i < retour.Length)
                    retour = retour.RemoveAt(index);
            return retour;
        }

        /// <summary>
        /// Supprime un objet du tableau, en fonction de sa position
        /// </summary>
        /// <typeparam name="T">Type de la liste</typeparam>
        /// <param name="list">Liste contenant l'index à supprimer</param>
        /// <param name="index">Numéro index dans le tableau de l'objet à supprimer de la liste</param>
        /// <returns>Un nouveau tableau, sans l'index spécifié</returns>
        public static T[] RemoveAt<T>(this T[] list, int index)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
#pragma warning disable S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur
            if (index >= list.Length)
                throw new IndexOutOfRangeException();
            if (list.Length == 0)
                throw new IndexOutOfRangeException();
#pragma warning restore S112 // C'est pourtant la bonne exception. Il n'y en a pas de meilleur

            T[] newListe = new T[list.Length - 1];
            if (list.Length > 0)
                for (int i = 0; i < list.Length; i++)
                    if (i != index) newListe[(i > index ? i - 1 : i)] = list[i];

            return newListe;
        }

        /// <summary>
        /// Supprime un objet spécifié du tableau
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="list">Le tableau</param>
        /// <param name="objectToRemove">Objet à supprimer</param>
        /// <param name="removeAll">Vrai pour supprimer toutes les occurences de cet objet du tableau, sinon Faux pour ne supprimer que le premier trouvé</param>
        /// <returns>Un nouveau tableau, sans l'objet spécifié</returns>
        public static T[] Remove<T>(this T[] list, object objectToRemove, bool removeAll = true)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (objectToRemove == null) throw new ArgumentNullException(nameof(objectToRemove));

            if (list.Length > 0)
            {
                bool trouve = true;
                while (trouve)
                {
                    trouve = false;
                    for (int i = 0; i < list.Length; i++)
                        if ((list[i] != null) && (list[i].Equals(objectToRemove)))
                        {
                            if (removeAll) trouve = true;
                            list = list.RemoveAt(i);
                            break;
                        }
                }
            }

            return list;
        }

        /// <summary>
        /// Supprime toutes les occurences de null(nothing) du tableau
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="list">Le tableau</param>
        /// <returns>Un nouveau tableau, sans les valeurs à null</returns>
        public static T[] RemoveAllNull<T>(this T[] list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            if (list.Length > 0)
            {
                bool trouve = true;
                while (trouve)
                {
                    trouve = false;
                    for (int i = 0; i < list.Length; i++)
                        if (list[i] == null)
                        {
                            trouve = true;
                            list = list.RemoveAt(i);
                            break;
                        }
                }
            }

            return list;
        }

        /// <summary>
        /// Retourne une liste après avoir supprimé tous les éléments de la liste dont la(les) condition(s) du prédicat sont remplies
        /// </summary>
        /// <typeparam name="T">Type d'objet dans la liste</typeparam>
        /// <param name="list">Liste à parcourir</param>
        /// <param name="predicate">Predicat (conditions) à remplir pour que l'élément soit supprimé</param>
        public static T[] RemoveAll<T>(this T[] list, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (list.Length == 0)
                return list;

            for (int i = list.Length - 1; i >= 0; i--)
                if (predicate(list[i]))
                    list = list.RemoveAt(i);
            return list;
        }

        /// <summary>
        /// Ajoute un élément en fin de tableau
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="list">Le tableau</param>
        /// <param name="objectToAdd">L'objet à ajouter au tableau</param>
        /// <returns>Un nouveau tableau, avec l'objet ajouté</returns>
        public static T[] Add<T>(this T[] list, object objectToAdd)
        {
            if (objectToAdd == null)
                throw new ArgumentNullException(nameof(objectToAdd));

            if ((typeof(T) == objectToAdd.GetType()) || (objectToAdd.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref list, list.Length + 1);
                list[list.Length - 1] = (T)objectToAdd;
            }
            return list;
        }

        /// <summary>
        /// Ajoute des éléments en fin de tableau
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="list">Le tableau</param>
        /// <param name="objectsToAdds">Les objets à ajouter au tableau</param>
        /// <returns>Un nouveau tableau, avec les objets ajoutés</returns>
        public static T[] AddRange<T>(this T[] list, object[] objectsToAdds)
        {
            if (objectsToAdds == null)
                throw new ArgumentNullException(nameof(objectsToAdds));

            foreach (T objet in objectsToAdds.Select(v => (T)v))
            {
                if ((typeof(T) == objet.GetType()) || (objet.GetType().IsSubclassOf(typeof(T))))
                {
                    Array.Resize(ref list, list.Length + 1);
                    list[list.Length - 1] = objet;
                }
            }
            return list;
        }

        /// <summary>
        /// Insère un élément dans le tableau à la position indiquée
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="list">Le tableau</param>
        /// <param name="objectToInsert">L'objet à insérer au tableau</param>
        /// <param name="position">Position dans le tableau ou insérer l'objet (de base zéro). Si la position est supérieur à la taille du tableau, il sera ajouté à la fin</param>
        /// <returns>Un nouveau tableau, avec l'objet inséré</returns>
        public static T[] Insert<T>(this T[] list, object objectToInsert, int position)
        {
            if (objectToInsert == null) throw new ArgumentNullException(nameof(objectToInsert));

            if (position > list.Length - 1) position = list.Length;
            if ((typeof(T) == objectToInsert.GetType()) || (objectToInsert.GetType().IsSubclassOf(typeof(T))))
            {
                Array.Resize(ref list, list.Length + 1);
                for (int i = list.Length - 1; i > position; i--)
                    list[i] = list[i - 1];
                list[position] = (T)objectToInsert;
            }
            return list;
        }

        /// <summary>
        /// Insère des éléments dans le tableau à la position indiquée
        /// </summary>
        /// <typeparam name="T">Type d'objet du tableau</typeparam>
        /// <param name="list">Le tableau</param>
        /// <param name="objectsToRemove">Les objets à insérer au tableau</param>
        /// <param name="position">Position dans le tableau ou insérer les objets (de base zéro). Si la position est supérieur à la taille du tableau, il seront ajouté à la fin</param>
        /// <returns>Un nouveau tableau, avec les objets insérés</returns>
        public static T[] InsertRange<T>(this T[] list, object[] objectsToRemove, int position)
        {
            if (objectsToRemove == null)
                throw new ArgumentNullException(nameof(objectsToRemove));

            if (position > list.Length - 1) position = list.Length;
            foreach (T objet in objectsToRemove.Select(v => (T)v))
            {
                if ((typeof(T) == objet.GetType()) || (objet.GetType().IsSubclassOf(typeof(T))))
                {
                    Array.Resize(ref list, list.Length + 1);
                    for (int i = list.Length - 1; i > position; i--)
                        list[i] = list[i - 1];
                    list[position] = objet;
                }
            }
            return list;
        }

        /// <summary>
        /// Retourne une suite de valeur hexadecimale du buffer d'octets sous forme d'une chaine de caractère, séparées par une virgule
        /// </summary>
        /// <param name="buffer">buffer d'octet (de byte) source</param>
        public static string HexToString(this byte[] buffer)
        {
            return HexToString(buffer, ",");
        }

        /// <summary>
        /// Retourne une suite de valeur hexadecimale du buffer d'octets sous forme d'une chaine de caractère
        /// </summary>
        /// <param name="buffer">buffer d'octet (de byte) source</param>
        /// <param name="separator">Séparateur à inserer entre chaque valeur</param>
        public static string HexToString(this byte[] buffer, string separator)
        {
            if ((buffer == null) || (buffer.Length == 0))
                return "";
            StringBuilder retour = new();
            foreach (byte octet in buffer)
            {
                if (retour.Length > 0)
                    retour.Append(separator);
                retour.Append($"0x{octet:X}");
            }
            return retour.ToString();
        }

        /// <summary>
        /// Retourne le contenu du tableau en partant du premier indice donné en paramètre
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="list">Tableau source</param>
        /// <param name="start">Position de départ par rapport à zéro (par rapport au premier élement du tableau=</param>
        public static T[] FromStart<T>(this T[] list, int start)
        {
            T[] nouvelleListe = list;
            if (start > 0)
                for (int i = 1; i <= start; i++)
                    nouvelleListe = nouvelleListe.RemoveAt(0);
            return nouvelleListe;
        }

        /// <summary>
        /// Retourne le contenu du tableau en partant du premier indice jusqu'à la fin moins le nombre spécifié en paramètre
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="list">Tableau source</param>
        /// <param name="end">Nombre d'élémente du tableau, en partant de la fin, à retourner</param>
        public static T[] ToEnd<T>(this T[] list, int end)
        {
            T[] nouvelleListe = list;
            if (end < list.Length - 1)
                Array.Resize(ref nouvelleListe, list.Length - end);
            return nouvelleListe;
        }

        /// <summary>
        /// Retourne le contenu du tableau en partant de la fin moins le nombre spécifié en paramètre
        /// </summary>
        /// <typeparam name="T">Type du tableau</typeparam>
        /// <param name="list">Tableau source</param>
        /// <param name="nb">Nombre d'élémente du tableau, en partant de la fin, à retourner</param>
        public static T[] FromEnd<T>(this T[] list, int nb)
        {
            T[] nouvelleListe = list;
            if (nb < list.Length)
                for (int i = 1; i <= list.Length - nb; i++)
                    nouvelleListe = nouvelleListe.RemoveAt(0);
            return nouvelleListe;
        }

        /// <summary>
        /// Effectue un distinct sur un tableau d'élément d'après une propriété des l'éléments
        /// </summary>
        /// <typeparam name="T1">Type de l'élement</typeparam>
        /// <typeparam name="T2">Type de la propriété de l'élément à comparer</typeparam>
        /// <param name="list">Le tableau d'élément</param>
        /// <param name="unique">Function retournant la propriété de l'élément à comparer</param>
        public static T1[] Distinct<T1, T2>(this T1[] list, Func<T1, T2> unique)
        {
            T1[] nouvelleListe = new T1[0] { };
            HashSet<T2> cle = new();
            foreach (T1 item in list.Where(i => cle.Add(unique(i))))
                nouvelleListe = nouvelleListe.Add(item);
            return nouvelleListe;
        }

        /// <summary>
        /// Recherche si un objet en particulier (ou une chaine de caractère) se trouve dans le tableau donné
        /// </summary>
        /// <param name="args">Tableau d'objet en paramètre</param>
        /// <param name="objectToSearch">Objet à rechercher (ou chaine de caractères)</param>
        /// <param name="comparisonMethod">Si vous recherchez une chaine de caractères, vous pouvez indiquer ici le filtre à appliquer (ex : avec ou sans respecter la casse)</param>
        public static bool Contains(this object[] args, object objectToSearch, StringComparison comparisonMethod = StringComparison.OrdinalIgnoreCase)
        {
            if ((args != null) && (args.Length > 0))
                if (objectToSearch is string chaine)
                {
                    return args.OfType<string>().Any(s => s.Equals(chaine, comparisonMethod));
                }
                else
                {
                    return args.Any(obj => obj == objectToSearch);
                }
            return false;
        }
    }
}
