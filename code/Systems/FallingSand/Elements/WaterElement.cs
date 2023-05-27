using Sand.Systems.FallingSand.Elements;

namespace Sand.Systems.FallingSand;

[Element]
public class Water : Liquid
{
	public Water()
	{
		color = Color.FromBytes( 0, 0, 255, 255 ).Lighten( Game.Random.Float( 0.9f, 1.1f ) );
		Density = 2;
	}
}
