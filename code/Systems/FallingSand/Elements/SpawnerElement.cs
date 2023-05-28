namespace Sand.Systems.FallingSand.Elements;

[Element]
public class Spawner : Cell
{
	Type element;
	public Spawner()
	{
		CellColor = Color.FromBytes( 255, 255, 255, 255 );
		Density = 100000;
		element = Player.RightClick;
	}
	public override Color GetColor()
	{
		return Color.FromBytes( 255, 255, 255, 255 ).Darken( 0.5f ).Lighten( Game.Random.Float( 0.8f, 1.2f ) );
	}
	public override void PostStep( Sandworker worker, out bool sleep )
	{
		sleep = false;
		if ( element == typeof( EmptyCell ) )
		{
			worker.SetCell( Position + Vector2.Down, new EmptyCell() );
			worker.SetCell( Position + Vector2.Left, new EmptyCell() );
			worker.SetCell( Position + Vector2.Right, new EmptyCell() );
			worker.SetCell( Position + Vector2.Up, new EmptyCell() );

			return;
		}
		if ( worker.GetCell( Position + Vector2.Down ) is EmptyCell )
		{
			Cell c = TypeLibrary.Create<Cell>( element );
			worker.SetCell( Position + Vector2.Down, c );
		}
	}

}

