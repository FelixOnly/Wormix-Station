// SPDX-FileCopyrightText: 2026 sablerti <work.feliks@proton.me>
//
// SPDX-License-Identifier: AGPL-3

using System.Text.RegularExpressions;

namespace Content.Client._Wormix.Searching;
public static class FuzzySearching
{

    public static float GetSimilarityPercent(string a, string b)
    {
        var distance = FuzzySearching.LevensteinAlgorithm(a, b);

        var maxLength = Math.Max(a.Length, b.Length);

        if (maxLength == 0)
            return 100f;

        return (1f - (float) distance / maxLength) * 100f;
    }

    public static string TrimTags(string text)
    {
        // Removes tags like:
        // [bold]
        // [/bold]
        // [color=red]
        // [size=20]
        return Regex.Replace(text, @"\[(\/)?[a-zA-Z]+(=[^\]]+)?\]", "");
    }

    public static double SequenceMatcherRatio(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            return 0;

        a = a.ToLowerInvariant();
        b = b.ToLowerInvariant();

        var matches = GetMatchingCharacters(a, b);

        return (2.0 * matches) / (a.Length + b.Length);
    }

    public static int GetMatchingCharacters(string a, string b)
    {
        if (a.Length == 0 || b.Length == 0)
            return 0;

        var longest = LongestCommonSubstring(a, b);

        if (longest.length == 0)
            return 0;

        return longest.length
               + GetMatchingCharacters(
                   a[..longest.aStart],
                   b[..longest.bStart])
               + GetMatchingCharacters(
                   a[(longest.aStart + longest.length)..],
                   b[(longest.bStart + longest.length)..]);
    }

    public static (int aStart, int bStart, int length) LongestCommonSubstring(string a, string b)
    {
        int bestLength = 0;
        int bestA = 0;
        int bestB = 0;

        var table = new int[a.Length + 1, b.Length + 1];

        for (var i = 1; i <= a.Length; i++)
        {
            for (var j = 1; j <= b.Length; j++)
            {
                if (a[i - 1] != b[j - 1])
                    continue;

                table[i, j] = table[i - 1, j - 1] + 1;

                if (table[i, j] > bestLength)
                {
                    bestLength = table[i, j];
                    bestA = i - bestLength;
                    bestB = j - bestLength;
                }
            }
        }

        return (bestA, bestB, bestLength);
    }

    public static int LevensteinAlgorithm(string source1, string source2) //O(n*m)
    {
        var source1Length = source1.Length;
        var source2Length = source2.Length;

        var matrix = new int[source1Length + 1, source2Length + 1];

        // First calculation, if one entry is empty return full length
        if (source1Length == 0)
            return source2Length;


        if (source2Length == 0)
            return source1Length;


        // Initialization of matrix with row size source1Length and columns size source2Length
        for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
        for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }


        // Calculate rows and collumns distances
        for (var i = 1; i <= source1Length; i++)
        {
            for (var j = 1; j <= source2Length; j++)
            {
                var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        // return result
        return matrix[source1Length, source2Length];
    }
}
