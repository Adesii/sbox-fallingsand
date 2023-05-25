using Sand.Systems.FallingSand.Elements;

namespace Sand.Systems.FallingSand;

public class WaterElement : Liquid
{
	public WaterElement()
	{
		color = Color.FromBytes( 0, 0, 255, 255 ).Lighten( Game.Random.Float( 0.9f, 1.1f ) );
		Velocity = Vector2Int.Zero;
		Density = 2;
	}
}
