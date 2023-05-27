namespace Sand.Systems.FallingSand.Elements;

public class MovableSolid : Cell
{
	public override void Step( Sandworker worker, out bool sleep )
	{
		if ( !MoveDown( worker ) )
		{
			//Velocity *= 0.9f;
			if ( !MoveDirection( worker, Vector2Int.Left + Vector2Int.Down, Vector2Int.Right + Vector2Int.Down, 1f, 1f ) )
			{
				Velocity *= Friction;
			}
		}

		FinalizeMove( worker, this, Velocity );

		//Velocity *= 0.9f;

		//if ( !sleep )
		//	return;


		sleep = Velocity.Length.AlmostEqual( 0 );
		if ( !sleep )
			SandWorld.Instance.KeepAlive( Position );
		//Log.Info( $"Velocity: {Velocity} Sleep: {sleep} lenght: {Velocity.Length} almost equal: {Velocity.Length.AlmostEqual( 0 )}" );
	}
}

