using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static UI.DependencyInjection.Service;

namespace UI.Domain.Generator.Pipelines
{
    /* 
        Generate a random password with respect of the following constraints :
            + Length of the given password is given by PasswordLength 
            + Letters of the password are picked from the allowed alphabets
            + The password will contain at least one character of each string of ForcedSubsets 
    */
    public class GeneratePassword
    {
        public record Request(
            int PasswordLength,
            bool UseUpperAlphabet,
            bool UseLowerAlphabet,
            bool UseNumbers,
            bool UseSymbols,
            string[] ForcedSubsets
        ) : IRequest<Result>
        {};

        public record Result(string Password) : IResult { };

        public class Handler : IRequestHandler<Request, Result>
        {
            private Random _random = new Random();

            public Task<Result> Handle(Request request)
            {

                if (request.PasswordLength < 0) throw new InvalidRequestException(
                    "The length of the password must be a positive integer");
                if (request.ForcedSubsets.Length > request.PasswordLength) throw new InvalidRequestException(
                    "The length of the password must be at least as high as the number of forced letters");
                if (!request.UseLowerAlphabet && !request.UseUpperAlphabet && !request.UseNumbers && !request.UseSymbols)
                    throw new InvalidRequestException("One alphabet at least must be used in order to generate a password");

                // Concat the allowed alphabets to get the entire alphabet
                string set = "";
                if (request.UseLowerAlphabet) set += LOWER_ALPHABET;
                if (request.UseUpperAlphabet) set += UPPER_ALPHABET;
                if (request.UseNumbers) set += NUMBERS;
                if (request.UseSymbols) set += SYMBOLS;

                // Generate a password from this alphabet
                string password_background =
                    Enumerable
                    .Range(1, request.PasswordLength)
                    .Select(i => _random.Next(set.Length)) // Pick <length> random ints between 0 and the size of the alphabet
                    .Select(i => set[i])                   // Convert each int into its corresponding char in the alphabet
                    .Aggregate("", (str, c) => str += c); // Add them together

                // Pick random locations for the forced characters
                var nb_forced_chars = request.ForcedSubsets.Length;
                var all_indexes = Enumerable.Range(0, request.PasswordLength).ToList();
                var picked_indexes = new List<int>();
                for (var i = 0; i < nb_forced_chars; i++)
                {
                    var element = pickRandomElement(all_indexes);
                    all_indexes.Remove(element);
                    picked_indexes.Add(element);
                }

                // Pick random chars from the given sets
                var forced_chars = new List<char>();
                for (var i = 0; i < nb_forced_chars; i++) forced_chars.Add(pickRandomLetter(request.ForcedSubsets[i]));

                // Place the forced chars at the decided locations
                var builder = password_background.ToArray();
                for (var i = 0; i < nb_forced_chars; i++)
                {
                    builder[picked_indexes[i]] = forced_chars[i];
                }
                string password = builder.Aggregate("", (password, letter) => password += letter);

                // Return the result
                return Task.FromResult(new Result(password));
            }

            private char pickRandomLetter(string s) => s[_random.Next(0, s.Length)];
            private T pickRandomElement<T>(ICollection<T> elements) => elements.ElementAt(_random.Next(0, elements.Count));

            public const string UPPER_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            public const string LOWER_ALPHABET = "abcdefghijklmnopqrstuvwxyz";
            public const string NUMBERS = "0123456789";
            public const string SYMBOLS = " !\"#$%&'()*+,-./:;<=>?@[\\]^_{|}~";

        }
    }

    [Serializable]
    internal class InvalidRequestException : Exception
    {
        public InvalidRequestException()
        {
        }

        public InvalidRequestException(string? message) : base(message)
        {
        }

        public InvalidRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}