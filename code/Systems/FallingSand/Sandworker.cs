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
			for ( int x = nchunk.rect_minX; x < nchunk.rect_maxX; x++ )
			{
				for ( int y = nchunk.rect_minY; y < nchunk.rect_maxY; y++ )
				{
					UpdateCell( new Vector2Int( x, y ) + nchunk.Position, out bool shouldsleep );
					if ( !shouldsleep )
					{
						sleep = false;
						//nchunk.KeepAlive( new Vector2Int( x, y ) + nchunk.Position );
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

	public void SetCell( Vector2Int pos, ref Cell cell, bool wake = false )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;
		//PingChunk( pos, chunk, world );
		if ( chunk.InBounds( pos ) )
		{
			if ( chunk.GetCell( pos ).GetType() != cell.GetType() )
				chunk.KeepAlive( pos );
			chunk.SetCell( pos, ref cell, wake );
		}
		else
			world.SetCell( pos, ref cell, wake );

	}

	private void PingChunk( Vector2Int Position, SandChunk chunk, SandWorld world )
	{
		int pingx = 0, pingy = 0;

		if ( Position.x == chunk.Position.x ) pingx = -1;
		if ( Position.x == chunk.Position.x + chunk.Size.x - 1 ) pingx = 1;
		if ( Position.y == chunk.Position.y ) pingy = -1;
		if ( Position.y == chunk.Position.y + chunk.Size.y - 1 ) pingy = 1;

		if ( pingx != 0 ) world.KeepAlive( new Vector2Int( Position.x + pingx, Position.y ) );
		if ( pingy != 0 ) world.KeepAlive( new Vector2Int( Position.x, Position.y + pingy ) );

		if ( pingx != 0 && pingy != 0 ) world.KeepAlive( new Vector2Int( Position.x + pingx, Position.y + pingy ) );
	}

	public void PingChunk( Vector2Int Position )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) ) return;
		int pingx = 0, pingy = 0;

		if ( Position.x == chunk.Position.x ) pingx = -1;
		if ( Position.x == chunk.Position.x + chunk.Size.x - 1 ) pingx = 1;
		if ( Position.y == chunk.Position.y ) pingy = -1;
		if ( Position.y == chunk.Position.y + chunk.Size.y - 1 ) pingy = 1;

		if ( pingx != 0 ) world.KeepAlive( new Vector2Int( Position.x + pingx, Position.y ) );
		if ( pingy != 0 ) world.KeepAlive( new Vector2Int( Position.x, Position.y + pingy ) );

		if ( pingx != 0 && pingy != 0 ) world.KeepAlive( new Vector2Int( Position.x + pingx, Position.y + pingy ) );
	}


	public void MoveCell( Vector2Int From, Vector2Int To, bool Swap = false )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) || !wworld.TryGetTarget( out var world ) || From == To ) return;
		//chunk.sleeping = false;
		PingChunk( From, chunk, world );
		PingChunk( To, chunk, world );
		//chunk.KeepAlive( From );
		//chunk.KeepAlive( To );
		//world.KeepAlive( From );
		//world.KeepAlive( To );


		if ( chunk.InBounds( From ) && chunk.InBounds( To ) )
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

