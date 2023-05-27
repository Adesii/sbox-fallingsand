namespace Sand.Systems.FallingSand.Elements;

public class MovableSolid : Cell
{
	public override void Step( Sandworker worker, out bool sleep )
	{
		Vector2 gravity = Vector2.Down; //GetGravityAtPosition( worker, this );
										//Right rotated from gravity
		Vector2 right = gravity.Perpendicular;
		//Left rotated from gravity
		Vector2 left = -right;

		if ( !MoveDown( worker ) )
		{
			if ( !MoveDirection( worker, (left + gravity).Normal, (right + gravity).Normal, 1f, 1f ) )
			//if ( !MoveDown( worker ) )
			{
				Velocity *= Friction;
			}
		}

		FinalizeMove( worker, this, Velocity );

		sleep = Velocity.Length.AlmostEqual( 0 );
		if ( !sleep )
			SandWorld.Instance.KeepAlive( Position );

	}
}

