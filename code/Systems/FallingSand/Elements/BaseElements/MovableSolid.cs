namespace Sand.Systems.FallingSand.Elements;

public class MovableSolid : Cell
{
	public override void Step( Sandworker worker, out bool sleep )
	{
		sleep = MoveDown( worker );
		if ( !sleep )
			return;
		sleep = MoveDirection( worker, Vector2Int.Left + Vector2Int.Down, Vector2Int.Right + Vector2Int.Down );
	}
}

