using Sand.Systems.FallingSand.Elements;

namespace Sand.Systems.FallingSand;

public class SandElement : MovableSolid
{
	public SandElement()
	{
		//brown
		color = Color.FromBytes( 139, 69, 19, 255 ).Darken( 0.5f ).Lighten( Game.Random.Float( 0.8f, 1.2f ) );
		Velocity = Vector2Int.Zero;
		Density = 1;
	}
}
