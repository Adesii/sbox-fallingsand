using Sand.Systems.FallingSand.Elements;

namespace Sand.Systems.FallingSand;

[Element]
public class Sand : MovableSolid
{
	public Sand()
	{
		color = Color.FromBytes( 139, 69, 19, 255 ).Darken( 0.5f ).Lighten( Game.Random.Float( 0.8f, 1.2f ) );
		Density = 1;
	}
}
