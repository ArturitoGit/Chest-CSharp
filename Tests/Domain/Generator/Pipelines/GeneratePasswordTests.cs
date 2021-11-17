using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Core.Domain.Generator.Pipelines;
using Shouldly;
using Xunit;

namespace Tests.Domain.Generator.Pipelines
{
    public class GeneratePasswordTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public void GeneratePassword_returnsRightLength(int length)
        {
            var request = new GeneratePassword.Request(
                length,
                true, true, true, true,
                new string[0]
            );
            var result = new GeneratePassword.Handler().Handle(request).GetAwaiter().GetResult();
            result.Password.Length.ShouldBe(length);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GeneratePassword_doesntForbiddenAlphabets(int forbidden_alphabet_index)
        {
            var alphabets = new string[] {
                GeneratePassword.Handler.UPPER_ALPHABET,
                GeneratePassword.Handler.LOWER_ALPHABET,
                GeneratePassword.Handler.NUMBERS,
                GeneratePassword.Handler.SYMBOLS
            };
            var length = 50;
            var request = new GeneratePassword.Request(
                length,
                forbidden_alphabet_index != 0,
                forbidden_alphabet_index != 1,
                forbidden_alphabet_index != 2,
                forbidden_alphabet_index != 3,
                new string[0]
            );
            var result = new GeneratePassword.Handler().Handle(request).GetAwaiter().GetResult();
            result.Password.Intersect(alphabets[forbidden_alphabet_index]).ShouldBeEmpty();
        }

        [Theory]
        [InlineData(30, new string[] { "?" })]
        [InlineData(30, new string[] { "#!" })]
        [InlineData(30, new string[] { "-", "_" })]
        [InlineData(30, new string[] { "?!", "12" })]
        [InlineData(30, new string[] { "a" })]
        public void GeneratePassword_returnsForcedChars(int length, string[] forcedSets)
        {
            var request = new GeneratePassword.Request(
                length,
                true, true, true, true,
                forcedSets
            );
            var result = new GeneratePassword.Handler().Handle(request).GetAwaiter().GetResult();

            foreach (var set in forcedSets)
            {
                result.Password.Intersect(set).ShouldNotBeEmpty();
            }
        }

        [Theory]
        [InlineData(2, "A")]
        [InlineData(4, ":")]
        [InlineData(3, "!")]
        [InlineData(3, "-")]
        [InlineData(5, "w")]
        public void GeneratePassword_returnsMultipleForcedChars(int nb_occurences, string set)
        {
            var length = 10;
            List<string> forcedChars = new();
            for (int i = 0; i < nb_occurences; i++) forcedChars.Add(set);

            var request = new GeneratePassword.Request(
                length,
                true, true, true, true,
                forcedChars.ToArray()
            );
            var result = new GeneratePassword.Handler().Handle(request).GetAwaiter().GetResult();

            Regex.Matches(result.Password, set).Count.ShouldBeGreaterThanOrEqualTo(nb_occurences);
        }
    }
}