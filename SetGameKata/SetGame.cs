using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;

namespace SetGameKata
{

    public enum Shape
    {
        Oval, Squiggle, Diamond
    }

    public enum Shading
    {
        Solid, Striped, Outlined 
    }

    public enum Number
    {
        One, Two, Three
    }

    public enum Color
    {
        Red, Purple, Green
    }

    public class Card
    {
        public Color Color = Color.Green;
        public Number Number = Number.One;
        public Shading Shading = Shading.Outlined;
        public Shape Shape = Shape.Diamond;

        protected bool Equals(Card other)
        {
            return Color == other.Color && Shading == other.Shading && Number == other.Number && Shape == other.Shape;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Card) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Color;
                hashCode = (hashCode*397) ^ (int) Shading;
                hashCode = (hashCode*397) ^ (int) Number;
                hashCode = (hashCode*397) ^ (int) Shape;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Color + " " + Number + " " + Shading + " " + Shape;
        }
    }


    
    public static class SetGame
    {

        public static bool IsACardSet(IList<Card> cards)
        {
           
            var shadingCount = cards.Select(card => card.Shading).Distinct().Count();
            var shapeCount = cards.Select(card => card.Shape).Distinct().Count();
            var colorCount = cards.Select(card => card.Color).Distinct().Count();
            var numberCount = cards.Select(card => card.Number).Distinct().Count();

            return (shadingCount == 3 || shadingCount == 1)
                   && (shapeCount == 3 || shapeCount == 1)
                   && (colorCount == 3 || colorCount == 1)
                   && (numberCount == 3 || numberCount == 1);

        }

        public static IEnumerable<IList<Card>> FindSets(IList<Card> cards)
        {
            for (int i = 0; i < cards.Count-2; i++)
            {
                for (int j = i+1; j < cards.Count-1; j++)
                {
                    for (int k = j+1; k < cards.Count; k++)
                    {
                        var candidates = new List<Card>() {cards[i], cards[j], cards[k]};
                        if (IsACardSet(candidates))
                        {
                            yield return candidates;
                        }
                    }    
                }    
            }
        }
    }


    public class SetGameTest
    {
      
        [Test]
        public void Should_find_sets_within_twelve_cards()
        {
            var cards = new List<Card>()
            {
                new Card() { Shape = Shape.Squiggle, Color = Color.Green, Number = Number.Three, Shading = Shading.Striped}, 
                new Card() { Shape = Shape.Oval, Color = Color.Red, Number = Number.Two, Shading = Shading.Striped}, 
                new Card() { Shape = Shape.Diamond, Color = Color.Green, Number = Number.One, Shading = Shading.Solid}, 
                new Card() { Shape = Shape.Oval, Color = Color.Purple, Number = Number.Three, Shading = Shading.Striped}, 
                new Card() { Shape = Shape.Squiggle, Color = Color.Green, Number = Number.Two, Shading = Shading.Solid}, 
                new Card() { Shape = Shape.Squiggle, Color = Color.Green, Number = Number.Three, Shading = Shading.Solid}, 
                new Card() { Shape = Shape.Oval, Color = Color.Green, Number = Number.One, Shading = Shading.Solid}, 
                new Card() { Shape = Shape.Diamond, Color = Color.Red, Number = Number.Three, Shading = Shading.Outlined}, 
                new Card() { Shape = Shape.Oval, Color = Color.Purple, Number = Number.One, Shading = Shading.Striped}, 
                new Card() { Shape = Shape.Squiggle, Color = Color.Purple, Number = Number.One, Shading = Shading.Striped}, 
                new Card() { Shape = Shape.Oval, Color = Color.Red, Number = Number.One, Shading = Shading.Striped}, 
                new Card() { Shape = Shape.Diamond, Color = Color.Red, Number = Number.One, Shading = Shading.Striped}
            };

            var sets = SetGame.FindSets(cards);
            Check.That(sets).HasSize(2);
            /*Check.That(sets).IsOnlyMadeOf(
                new List<Card>()
                {
                    new Card() { Shape = Shape.Oval, Color = Color.Purple, Number = Number.Three, Shading = Shading.Striped}, 
                    new Card() { Shape = Shape.Squiggle, Color = Color.Green, Number = Number.Three, Shading = Shading.Solid}, 
                    new Card() { Shape = Shape.Diamond, Color = Color.Red, Number = Number.Three, Shading = Shading.Outlined}
                },
                new List<Card>()
                {
                    new Card() { Shape = Shape.Squiggle, Color = Color.Green, Number = Number.Two, Shading = Shading.Solid}, 
                    new Card() { Shape = Shape.Diamond, Color = Color.Red, Number = Number.Three, Shading = Shading.Outlined},
                    new Card() { Shape = Shape.Oval, Color = Color.Purple, Number = Number.One, Shading = Shading.Striped}
                }
            );*/
        }

        [Test]
        public void Should_be_a_set_when_3_cards_are_equals()
        {
            var card1 = new Card();
            var card2 = new Card();
            var card3 = new Card();

            Check.That(SetGame.IsACardSet(new List<Card>() {card1, card2, card3})).IsTrue();
        }

        [Test]
        public void Should_be_a_set_when_3_cards_have_same_color_same_number_same_shape_but_different_shading()
        {
            var card1 = new Card() { Shading = Shading.Outlined };
            var card2 = new Card() { Shading = Shading.Solid };
            var card3 = new Card() { Shading = Shading.Striped };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsTrue();
        }

        [Test]
        public void Should_not_be_a_set_when_2_cards_have_same_shading_but_not_the_third_one()
        {
            var card1 = new Card() { Shading = Shading.Outlined };
            var card2 = new Card() { Shading = Shading.Outlined };
            var card3 = new Card() { Shading = Shading.Striped };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsFalse();
        }

        [Test]
        public void Should_not_be_a_set_when_2_cards_have_same_shading_but_not_the_third_one_and_colors_are_all_different()
        {
            var card1 = new Card() { Color = Color.Green, Shading = Shading.Outlined };
            var card2 = new Card() { Color = Color.Red, Shading = Shading.Outlined };
            var card3 = new Card() { Color = Color.Purple, Shading = Shading.Striped };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsFalse();
        }

        [Test]
        public void Should_be_a_set_when_3_cards_have_same_color_same_number_same_shading_but_different_shape()
        {
            var card1 = new Card() { Shape = Shape.Diamond };
            var card2 = new Card() { Shape = Shape.Oval };
            var card3 = new Card() { Shape = Shape.Squiggle };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsTrue();
        }

        [Test]
        public void Should_not_be_a_set_when_2_cards_have_same_shape_but_not_the_third_one()
        {
            var card1 = new Card() { Shape = Shape.Diamond };
            var card2 = new Card() { Shape = Shape.Diamond };
            var card3 = new Card() { Shape = Shape.Squiggle };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsFalse();
        }

        [Test]
        public void Should_be_a_set_when_3_cards_have_same_shape_same_number_same_shading_but_different_color()
        {
            var card1 = new Card() { Color = Color.Green };
            var card2 = new Card() { Color = Color.Purple };
            var card3 = new Card() { Color = Color.Red };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsTrue();
        }

        [Test]
        public void Should_not_be_a_set_when_2_cards_have_same_color_but_not_the_third_one()
        {
            var card1 = new Card() { Color = Color.Green };
            var card2 = new Card() { Color = Color.Green };
            var card3 = new Card() { Color = Color.Red };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsFalse();
        }

        [Test]
        public void Should_be_a_set_when_3_cards_have_same_shape_same_color_same_shading_but_different_number()
        {
            var card1 = new Card() { Number = Number.One };
            var card2 = new Card() { Number = Number.Two };
            var card3 = new Card() { Number = Number.Three };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsTrue();
        }

        [Test]
        public void Should_not_be_a_set_when_2_cards_have_same_number_but_not_the_third_one()
        {
            var card1 = new Card() { Number = Number.One };
            var card2 = new Card() { Number = Number.One };
            var card3 = new Card() { Number = Number.Two };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsFalse();
        }

        [Test]
        public void Should_be_a_set_when_3_cards_have_different_colors_shapes_numbers_shading()
        {
            var card1 = new Card() { Color = Color.Green, Number = Number.One, Shading = Shading.Outlined, Shape = Shape.Diamond };
            var card2 = new Card() { Color = Color.Purple, Number = Number.Two, Shading = Shading.Solid, Shape = Shape.Oval };
            var card3 = new Card() { Color = Color.Red, Number = Number.Three, Shading = Shading.Striped, Shape = Shape.Squiggle };

            Check.That(SetGame.IsACardSet(new List<Card>() { card1, card2, card3 })).IsTrue();
        }

    }

}
