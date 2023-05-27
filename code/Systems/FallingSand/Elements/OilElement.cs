namespace Sand.Systems.FallingSand.Elements;

[Element]
public class Oil : Liquid, IFlamable
{
	public override float DisperseRate => 7f;
	public override int Density => 40;
	public float Flammability => 0.5f;
	public Oil()
	{
		CellColor = Color.FromBytes( 15, 15, 15, 255 ).Lighten( Game.Random.Float( 0.9f, 1.1f ) );
	}

}

