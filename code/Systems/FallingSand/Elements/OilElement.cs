namespace Sand.Systems.FallingSand.Elements;

[Element]
public class Oil : Liquid
{
	public override float DisperseRate => 7f;
	public Oil()
	{
		color = Color.FromBytes( 15, 15, 15, 255 ).Lighten( Game.Random.Float( 0.9f, 1.1f ) );
		Density = 3;
	}

}

