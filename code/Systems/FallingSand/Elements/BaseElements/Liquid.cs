namespace Sand.Systems.FallingSand.Elements;

public class Liquid : Cell
{
	public override void Step( Sandworker worker, out bool sleep )
	{
		sleep = MoveDown( worker );
		if ( !sleep )
			return;
		sleep = MoveDirection( worker, Vector2Int.Left + Vector2Int.Down, Vector2Int.Right + Vector2Int.Down, 5, 5 );
		if ( !sleep )
			return;
		sleep = MoveDirection( worker, Vector2Int.Left, Vector2Int.Right, 5, 5 );
		if ( !sleep )
			return;

	}
}

