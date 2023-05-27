using Sand.Systems.FallingSand;

namespace Sand.util;

public static class SandUtils
{
	public static void PointToPointFunction( Vector2Int pos1, Vector2Int pos2, Action<Vector2Int> function )
	{
		// If the two points are the same no need to iterate. Just run the provided function
		if ( pos1 == pos2 )
		{
			function( pos1 );
			return;
		}

		int matrixX1 = pos1.x;
		int matrixY1 = pos1.y;
		int matrixX2 = pos2.x;
		int matrixY2 = pos2.y;

		int xDiff = matrixX1 - matrixX2;
		int yDiff = matrixY1 - matrixY2;
		bool xDiffIsLarger = xDiff != int.MinValue && Math.Abs( xDiff ) > Math.Abs( yDiff );

		int xModifier = xDiff < 0 ? 1 : -1;
		int yModifier = yDiff < 0 ? 1 : -1;
		int longerSideLength = 0;
		int shorterSideLength = 0;
		try
		{
			longerSideLength = Math.Max( Math.Abs( xDiff ), Math.Abs( yDiff ) );
			shorterSideLength = Math.Min( Math.Abs( xDiff ), Math.Abs( yDiff ) );
		}
		catch ( System.Exception )
		{

			return;
		}


		float slope = (shorterSideLength == 0 || longerSideLength == 0) ? 0 : ((float)shorterSideLength / longerSideLength);

		int shorterSideIncrease;
		for ( int i = 1; i <= longerSideLength; i++ )
		{
			shorterSideIncrease = (int)Math.Round( i * slope );
			int yIncrease, xIncrease;
			if ( xDiffIsLarger )
			{
				xIncrease = i;
				yIncrease = shorterSideIncrease;
			}
			else
			{
				yIncrease = i;
				xIncrease = shorterSideIncrease;
			}
			int currentY = matrixY1 + (yIncrease * yModifier);
			int currentX = matrixX1 + (xIncrease * xModifier);
			Vector2Int v = new( currentX, currentY );
			function( v );
		}
	}
	public static bool CanSwap( Sandworker worker, Cell a, Vector2Int swappos, out Cell b )
	{
		var cb = worker.GetCell( swappos );
		b = cb;
		if ( cb == null )
		{
			return false;
		}
		if ( cb.IsAir() )
		{
			return true;
		}
		if ( a.GetType() == cb.GetType() )
		{
			return false;
		}
		return a.Density > cb.Density;

	}
	public static bool CanSwap( Sandworker worker, Cell a, Vector2Int swappos )
	{
		return CanSwap( worker, a, swappos, out _ );
	}

	internal static bool IsAir( this Cell leftcell )
	{
		return leftcell is EmptyCell;
	}

	public static bool IsAirOrNull( this Cell leftcell )
	{
		return leftcell == null || leftcell is EmptyCell;
	}

	public static Cell[] GetNeighbours( this Cell Origin, Sandworker worker )
	{
		Vector2Int[] Directions = new Vector2Int[] { new( 0, 1 ), new( 1, 1 ), new( 1, 0 ), new( 1, -1 ), new( 0, -1 ), new( -1, -1 ), new( -1, 0 ), new( -1, 1 ) };
		List<Cell> cells = new List<Cell>( 8 );
		foreach ( var dir in Directions )
		{
			var cell = worker.GetCell( Origin.Position + dir );
			if ( cell != null )
			{
				cells.Add( cell );
			}
		}

		return cells.ToArray();
	}

	public static void ApplyToNeightbours( this Cell Origin, Sandworker worker, int Distance, Action<Cell, int> toapply )
	{
		Queue<Cell> queue = new Queue<Cell>();
		HashSet<Cell> visited = new HashSet<Cell>();

		queue.Enqueue( Origin );
		visited.Add( Origin );

		int distance = 0;

		while ( queue.Count > 0 && distance < Distance )
		{
			int levelSize = queue.Count;

			for ( int i = 0; i < levelSize; i++ )
			{
				Cell current = queue.Dequeue();

				if ( current != Origin )
				{
					toapply( current, distance );
					//calculate distance from source and apply square root to get a falloff
					/* float heatPercent = 1 - ((float)distance / (float)Distance);
					float heatToAdd = Heat * heatPercent;
					current.Heat += heatToAdd;
					current.OnHeated( worker );
					Heat -= heatToAdd;
					OnHeated( worker ); */
				}

				foreach ( Cell neighbor in current.GetNeighbours( worker ) )
				{
					if ( !visited.Contains( neighbor ) )
					{
						queue.Enqueue( neighbor );
						visited.Add( neighbor );
					}
				}
			}

			distance++;
		}
	}
}

