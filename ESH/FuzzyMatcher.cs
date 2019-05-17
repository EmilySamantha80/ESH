// Title:  Fuzzy string matching
// Author: Emily Heiner
//
// MIT License
// Copyright(c) 2019 Emily Heiner (emilysamantha80@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESH
{
    /// <summary>
    /// Performs fuzzy string searches
    /// </summary>
    /// <remarks>
    /// Based on https://gist.github.com/CDillinger/2aa02128f840bdca90340ce08ee71bc2
    /// </remarks>
    public static class FuzzyMatcher
    {
        /// <summary>
        /// Does a fuzzy search for each word of stringToSearch in each word of pattern and returns a score
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <returns>The score that the search received</returns>
        public static int SearchString(string stringToSearch, string pattern)
        {
            int score = 0;

            if (string.IsNullOrWhiteSpace(pattern) || string.IsNullOrWhiteSpace(stringToSearch))
            {
                return score;
            }

            foreach (var patternWord in pattern.Split(new char[] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var stringToSearchWord in stringToSearch.Split(new char[] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int outScore;
                    bool isMatch = FuzzyMatch(stringToSearchWord.Trim(), patternWord.Trim(), out outScore);
                    // If there's a confirmed match, we want to boost the score
                    if (isMatch)
                    {
                        score += 1000;
                    }
                    // Only add up scores that are positive
                    if (outScore > 0)
                    {
                        score += outScore;
                    }
                }
            }
            return score;
        }

        /// <summary>
        /// Does a fuzzy search for a pattern within a string.
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <returns>true if each character in pattern is found sequentially within stringToSearch; otherwise, false.</returns>
        public static bool FuzzyMatch(string stringToSearch, string pattern)
        {
            var patternIdx = 0;
            var strIdx = 0;
            var patternLength = pattern.Length;
            var strLength = stringToSearch.Length;

            while (patternIdx != patternLength && strIdx != strLength)
            {
                if (char.ToLower(pattern[patternIdx]) == char.ToLower(stringToSearch[strIdx]))
                    ++patternIdx;
                ++strIdx;
            }

            return patternLength != 0 && strLength != 0 && patternIdx == patternLength;
        }

        /// <summary>
        /// Does a fuzzy search for a pattern within a string, and gives the search a score on how well it matched.
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <param name="outScore">The score which this search received, if a match was found.</param>
        /// <returns>true if each character in pattern is found sequentially within stringToSearch; otherwise, false.</returns>
        public static bool FuzzyMatch(string stringToSearch, string pattern, out int outScore)
        {
            // Score consts
            const int adjacencyBonus = 2;               // bonus for adjacent matches
            const int separatorBonus = 10;              // bonus if match occurs after a separator
            const int camelBonus = 10;                  // bonus if match is uppercase and prev is lower

            const int leadingLetterPenalty = -3;        // penalty applied for every letter in stringToSearch before the first match
            const int maxLeadingLetterPenalty = -9;     // maximum penalty for leading letters
            const int unmatchedLetterPenalty = -1;      // penalty for every letter that doesn't matter
                                                        // Loop variables
            var score = 0;
            var patternIdx = 0;
            var patternLength = pattern.Length;
            var strIdx = 0;
            var strLength = stringToSearch.Length;
            var prevMatched = false;
            var prevLower = false;
            var prevSeparator = true;                   // true if first letter match gets separator bonus

            // Use "best" matched letter if multiple string letters match the pattern
            char? bestLetter = null;
            char? bestLower = null;
            int? bestLetterIdx = null;
            var bestLetterScore = 0;

            var matchedIndices = new List<int>();

            // Loop over strings
            while (strIdx != strLength)
            {
                var patternChar = patternIdx != patternLength ? pattern[patternIdx] as char? : null;
                var strChar = stringToSearch[strIdx];

                var patternLower = patternChar != null ? char.ToLower((char)patternChar) as char? : null;
                var strLower = char.ToLower(strChar);
                var strUpper = char.ToUpper(strChar);

                var nextMatch = patternChar != null && patternLower == strLower;
                var rematch = bestLetter != null && bestLower == strLower;

                var advanced = nextMatch && bestLetter != null;
                var patternRepeat = bestLetter != null && patternChar != null && bestLower == patternLower;
                if (advanced || patternRepeat)
                {
                    score += bestLetterScore;
                    matchedIndices.Add((int)bestLetterIdx);
                    bestLetter = null;
                    bestLower = null;
                    bestLetterIdx = null;
                    bestLetterScore = 0;
                }

                if (nextMatch || rematch)
                {
                    var newScore = 0;

                    // Apply penalty for each letter before the first pattern match
                    // Note: Math.Max because penalties are negative values. So max is smallest penalty.
                    if (patternIdx == 0)
                    {
                        var penalty = Math.Max(strIdx * leadingLetterPenalty, maxLeadingLetterPenalty);
                        score += penalty;
                    }

                    // Apply bonus for consecutive bonuses
                    if (prevMatched)
                        newScore += adjacencyBonus;

                    // Apply bonus for matches after a separator
                    if (prevSeparator)
                        newScore += separatorBonus;

                    // Apply bonus across camel case boundaries. Includes "clever" isLetter check.
                    if (prevLower && strChar == strUpper && strLower != strUpper)
                        newScore += camelBonus;

                    // Update pattern index IF the next pattern letter was matched
                    if (nextMatch)
                        ++patternIdx;

                    // Update best letter in stringToSearch which may be for a "next" letter or a "rematch"
                    if (newScore >= bestLetterScore)
                    {
                        // Apply penalty for now skipped letter
                        if (bestLetter != null)
                            score += unmatchedLetterPenalty;

                        bestLetter = strChar;
                        bestLower = char.ToLower((char)bestLetter);
                        bestLetterIdx = strIdx;
                        bestLetterScore = newScore;
                    }

                    prevMatched = true;
                }
                else
                {
                    score += unmatchedLetterPenalty;
                    prevMatched = false;
                }

                // Includes "clever" isLetter check.
                prevLower = strChar == strLower && strLower != strUpper;
                prevSeparator = strChar == '_' || strChar == ' ';

                ++strIdx;
            }

            // Apply score for last match
            if (bestLetter != null)
            {
                score += bestLetterScore;
                matchedIndices.Add((int)bestLetterIdx);
            }

            outScore = score;
            return patternIdx == patternLength;
        }
    }

}
