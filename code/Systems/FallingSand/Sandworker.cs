namespace Sand.Systems.FallingSand;

public class Sandworker
{
	protected WeakReference<SandWorld> wworld;
	protected WeakReference<SandChunk> wchunk;


	protected Sandworker( SandWorld world, SandChunk chunk )
	{
		this.wworld = new( world );
		this.wchunk = new( chunk );
	}

	[ConVar.Client( "SandUseRandomPattern", Help = "Use random pattern for sand" )]
	public static bool UseRandomPattern { get; set; } = false;
	public void UpdateChunk()
	{
		if ( wchunk.TryGetTarget( out var nchunk ) )
		{
			bool sleep = true;
			bool shouldsleep = true;

			if ( !UseRandomPattern )
			{
				for ( int x = nchunk.rect_minX; x < nchunk.rect_maxX; x++ )
				{
					for ( int y = nchunk.rect_minY; y < nchunk.rect_maxY; y++ )
					{
						UpdateCell( new Vector2Int( x, y ) + nchunk.Position, out shouldsleep );
						if ( !shouldsleep )
						{
							sleep = false;
						}
					}
				}
			}
			else
			{
				List<(int, int)> coordinates = new();


				for ( int y = nchunk.rect_minY; y < nchunk.rect_maxY; y++ )
				{
					for ( int x = nchunk.rect_minX; x < nchunk.rect_maxX; x++ )
					{
						coordinates.Add( (x, y) );

					}
				}

				int count = coordinates.Count;
				for ( int i = count - 1; i > 0; i-- )
				{
					int j = Game.Random.Next( i + 1 );
					(coordinates[j], coordinates[i]) = (coordinates[i], coordinates[j]);
				}

				foreach ( var (x, y) in coordinates )
				{
					UpdateCell( new Vector2Int( x, y ) + nchunk.Position, out shouldsleep );
					if ( !shouldsleep )
					{
						sleep = false;
					}
				}

			}
			nchunk.sleeping = sleep;


		}

	}


	public virtual void UpdateCell( Vector2Int Position, out bool sleep )
	{
		sleep = true;
	}

	public Cell GetCell( Vector2Int pos )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return default;
		if ( chunk.InBounds( pos ) )
			return chunk.GetCell( pos );
		else
			return world.GetCell( pos );
	}

	public void SetCellVelocity( Vector2Int pos, Vector2Int vel )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;
		if ( chunk.InBounds( pos ) )
			chunk.SetCellVelocity( pos, vel );
		else
			world.SetCellVelocity( pos, vel );
	}

	public void SetCell( Vector2Int pos, ref Cell cell, bool wake = false )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;

		if ( chunk.InBounds( pos ) )
			chunk.SetCell( pos, ref cell, wake );
		else
			world.SetCell( pos, ref cell, wake );

	}


	public void MoveCell( Vector2Int From, Vector2Int To, bool Swap = false )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;

		int pingx = 0, pingy = 0;

		if ( From.x <= chunk.Position.x + 1 ) pingx = -5;
		if ( From.x >= chunk.Position.x + chunk.Size.x - 1 ) pingx = 5;
		if ( From.y <= chunk.Position.y + 1 ) pingy = -5;
		if ( From.y >= chunk.Position.y + chunk.Size.y - 1 ) pingy = 5;

		if ( pingx != 0 ) world.KeepAlive( new Vector2Int( From.x + pingx, From.y ) );
		if ( pingy != 0 ) world.KeepAlive( new Vector2Int( From.x, From.y + pingy ) );

		if ( pingx != 0 && pingy != 0 ) world.KeepAlive( new Vector2Int( From.x + pingx, From.y + pingy ) );


		if ( chunk.InBounds( From ) )
			chunk.MoveCell( chunk, From, To, Swap );
		else
			world.MoveCell( From, To, Swap );
	}

	public bool InBounds( Vector2Int pos )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return false;
		return chunk.InBounds( pos ) || world.InBounds( pos );
	}

	public bool IsEmpty( Vector2Int pos )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return false;
		if ( chunk.InBounds( pos ) )
			return chunk.IsEmpty( pos );
		else
			return world.IsEmpty( pos );
	}


}

