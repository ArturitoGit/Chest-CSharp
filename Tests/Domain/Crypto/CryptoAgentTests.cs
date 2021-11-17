using System.Linq;
using Core.Domain.Crypto;
using Shouldly;
using Xunit;

namespace Tests.Domain.Crypto
{
    public class CryptoAgentTests
    {
        public CryptoAgent Agent;

        public CryptoAgentTests()
        {
            Agent = new CryptoAgent();
        }

        [Fact]
        public void GenerateSalt_GeneratesSomething()
            => Agent.GenerateSalt().ShouldNotBeNull();

        [Fact]
        public void GenerateSalt_GeneratesDifferentSalts()
        {
            var salts = Enumerable.Range(0, 3)
                .Select(i => Agent.GenerateSalt())
                .ToArray();

            salts[0].ShouldNotBeSameAs(salts[1]);
            salts[1].ShouldNotBeSameAs(salts[2]);
            salts[2].ShouldNotBeSameAs(salts[0]);
        }

        [Fact]
        public void GetHash_ReturnsSomething()
        {
            var input = "something I want to Hash";
            var output = Agent.GetHash(input);

            string.IsNullOrEmpty(output).ShouldBeFalse();
        }

        [Fact]
        public void CheckHash_IsTrueForRightEntry()
        {
            var input1 = "something I want to Hash";
            var input2 = "something I want to Hash";

            var hash = Agent.GetHash(input1);

            Agent.CheckHash(input2, hash).ShouldBeTrue();
        }

        [Fact]
        public void CheckHash_IsFalseForWrongEntry()
        {
            var input1 = "GetHash_ReturnsSomething I want to hash";
            var intput2 = "GetHash_ReturnsSomething I want to hash__";

            var hash = Agent.GetHash(input1);

            Agent.CheckHash(intput2, hash).ShouldBeFalse();
        }


    }
}