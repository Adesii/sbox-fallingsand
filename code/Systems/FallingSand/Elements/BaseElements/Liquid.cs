namespace Sand.Systems.FallingSand.Elements;

public class Liquid : Cell
{
	public virtual float DisperseRate => 5f;
	public override void Step( Sandworker worker, out bool sleep )
	{
		Vector2 gravity = Vector2.Down;// GetGravityAtPosition( worker, this );
									   //Right rotated from gravity
		Vector2 right = gravity.Perpendicular;
		//Left rotated from gravity
		Vector2 left = -right;

		if ( !MoveDown( worker ) )
		{
			if ( !MoveDirection( worker, left + gravity, right + gravity, 1f, 1f ) )
			{
				if ( !MoveDirection( worker, left, right, DisperseRate, DisperseRate ) )
				{
					Velocity *= Friction;
				}
				Velocity.x = Velocity.x.Clamp( -DisperseRate, DisperseRate );
			}
		}

		FinalizeMove( worker, this, Velocity );

		sleep = Velocity.Length.AlmostEqual( 0 );
		if ( !sleep )
			SandWorld.Instance.KeepAlive( Position );

	}
}

