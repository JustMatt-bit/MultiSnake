using SnakeMultiplayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SnakeMultiplayer.Services.Appearance
{
     public interface IColor
    {
        PlayerColor  GetColor();
    }

    public class Red : IColor
    {
        public PlayerColor GetColor() => PlayerColor.crimson;
    }

    public class Blue : IColor
    {
        public PlayerColor GetColor() => PlayerColor.lightblue;
    }

     public class GreenColor : IColor
    {
        public PlayerColor GetColor() => PlayerColor.greenyellow;
    }

    public class RandomColor : IColor
{
    private readonly PlayerColor colorName;

    public RandomColor()
    {
        var colors = Enum.GetValues(typeof(PlayerColor));
        this.colorName = (PlayerColor)colors.GetValue(new Random().Next(colors.Length));
    }

    public PlayerColor GetColor() => colorName;
}

    public class CustomColor : IColor
    {
        private readonly PlayerColor colorName;

        public CustomColor(PlayerColor colorName)
        {
            this.colorName = colorName;
        }

        public PlayerColor GetColor() => colorName;
    }

    //------------------------------------------------------------------

    public abstract class SnakeAppearance
    {
        protected IColor color;

        public abstract Shapes Shape { get; }  // Using Shapes enum

        public string ShapeName => Shape.ToString();

        public PlayerColor Color => color.GetColor();

        protected SnakeAppearance(IColor color)
        {
            this.color = color;
        }

    }

    public class CircleShape : SnakeAppearance
    {
        public CircleShape(IColor color) : base(color) { }

        public override Shapes Shape => Shapes.circle;

    }

    public class SquareShape : SnakeAppearance
    {
        public SquareShape(IColor color) : base(color) { }

         public override Shapes Shape => Shapes.square;


    }

    public class EllipseShape : SnakeAppearance
    {
        public EllipseShape(IColor color) : base(color) { }

        public override Shapes Shape => Shapes.ellipse;

    }

    public class PolygonShape : SnakeAppearance
    {
        public PolygonShape(IColor color) : base(color) { }

        public override Shapes Shape => Shapes.polygon;


    }

    public class TriangleShape : SnakeAppearance
    {
        public TriangleShape(IColor color) : base(color) { }

        public override Shapes Shape => Shapes.triangle;


    }

   public class RandomShape : SnakeAppearance
    {
        private static readonly Random random = new Random();
        private Shapes randomShape;

        public RandomShape(IColor color) : base(color)
        {
            randomShape = (Shapes)Enum.GetValues(typeof(Shapes)).GetValue(random.Next(Enum.GetValues(typeof(Shapes)).Length));
        }

        public override Shapes Shape => randomShape;
    }


}