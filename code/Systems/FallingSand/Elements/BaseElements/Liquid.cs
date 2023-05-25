namespace Sand.Systems.FallingSand.Elements;

public class Liquid : Cell
{
	public override void Step( Sandworker worker, out bool sleep )
	{
		if ( MoveDown( worker, out sleep ) )
			return;
		if ( MoveDownSides( worker, out sleep ) )
			return;
		if ( MoveSides( worker, out sleep ) )
			return;
	}
}

