using Sand.Systems.FallingSand.Elements;
using Sandbox.Utility;

namespace Sand.Systems.FallingSand;

[Element]
public class Water : Liquid, IFlamable
{
	public override int Density => 50;

	public float Flammability => 0.2f;

	public Water()
	{
		CellColor = Color.FromBytes( 0, 0, 255, 255 ).Lighten( Game.Random.Float( 0.9f, 1.1f ) );
	}

	TimeSince timeSinceLastColorChange;

	public void Ignite( Sandworker worker, Cell Target, Cell Origin )
	{
		if ( Game.Random.Float( 0f, 1f ) >= Flammability )
			return;
		worker.SetCell( Target.Position, new Steam(), true );
		worker.SetCell( Origin.Position, null, true );
	}

	public override Color GetColor()
	{
		//change color ever 0.1 seconds
		if ( timeSinceLastColorChange > 0.1f )
		{
			CellColor = Color.FromBytes( 0, 0, 255, 255 )
						.Desaturate( Noise.Fbm( 4, Position.x + Time.Now * 10, Position.y + Time.Now * 10 ) * 0.3f )
						.Darken( Game.Random.Float( 0.05f, 0.1f ) )
						.Lighten( (Velocity.Length * 0.5f).Clamp( 0, 3f ) );
			timeSinceLastColorChange = 0;
		}
		return CellColor;
	}
}
