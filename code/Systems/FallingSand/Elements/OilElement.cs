namespace Sand.Systems.FallingSand.Elements;

[Element]
public class OilElement : Liquid
{
	public OilElement()
	{
		color = Color.FromBytes( 15, 15, 15, 255 ).Lighten( Game.Random.Float( 0.9f, 1.1f ) );
		Velocity = Vector2Int.Zero;
		Density = 3;
	}

}

