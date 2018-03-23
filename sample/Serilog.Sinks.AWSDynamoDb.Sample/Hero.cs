using System;
using System.Collections.Generic;

namespace Serilog.Sinks.AWSDynamoDb.Sample
{
    internal class Hero
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public List<Power> Powers { get; set; } = new List<Power>();

        public static Hero ReturnDummyHero()
        {
            var hero = new Hero()
            {
                Id = Guid.NewGuid(),
                Name = "Superman",
                City = "Smallville",
                Powers = new List<Power>
                {
                    new Power("Superhuman solar energy"),
                    new Power("Solar flare and heat vision"),
                    new Power("Flight")
                }
            };

            return hero;
        }
    }

    internal class Power
    {
        public Power(string powerName)
        {
            Name = powerName;
        }

        public string Name { get; }
    }
}