using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sandbox.Debug;

namespace Sand.Systems.FallingSand;

public class SandWorld
{
	private static SandWorld instance;
	public static SandWorld Instance
	{
		get
		{
			instance ??= new SandWorld();
			return instance;
		}
	}

	SandWorld()
	{
		Event.Register( this );

		Scale = 0.5f;
		ChunkWidth = 64;
		ChunkHeight = 64;
	}

	public Texture DrawTexture;
	public Texture CellTexture;


	public static int ChunkWidth;
	public static int ChunkHeight;

	public static float Scale;

	public ConcurrentDictionary<Vector2Int, SandChunk> chunks = new();

	//List<CellMoveInfo> Changes = new();

	public Cell GetCell( Vector2Int pos )
	{
		return GetChunk( pos )?.GetCell( pos ) ?? new Cell();
	}

	public void SetCell( Vector2Int pos, Cell cell )
	{
		GetChunk( pos ).SetCell( pos, cell );
	}


	public void MoveCell( Vector2Int From, Vector2Int To, bool Swap = false )
	{
		SandChunk src = GetChunk( From );
		if ( src != null )
		{
			SandChunk dst = GetChunk( To );
			if ( dst != null )
				dst.MoveCell( src, From, To, Swap );

			//Changes.Add( new( src, GetIndex( From ), GetIndex( To ), Swap ) );
		}
	}

	public bool IsEmpty( Vector2Int pos )
	{
		if ( !InBounds( pos ) ) return false;
		return GetChunk( pos ).IsEmpty( pos );
	}

	public bool InBounds( Vector2Int pos )
	{
		SandChunk chunk = GetChunk( pos );
		if ( chunk != null )
		{
			return chunk.InBounds( pos );
		}
		return false;
	}

	public SandChunk GetChunk( Vector2Int pos )
	{
		var local = GetLocalLocationPrecise( pos );
		return GetChunk( local.x, local.y );
	}

	Vector2Int GetLocalLocationPrecise( Vector2Int pos )
	{
		return new( ((float)pos.x / ChunkWidth).FloorToInt(), ((float)pos.y / ChunkHeight).FloorToInt() );
	}

	public SandChunk GetChunk( int x, int y )
	{
		if ( chunks.TryGetValue( new( x, y ), out var chunk ) )
		{
			return chunk;
		}
		if ( x <= -10 || x >= 10 || y <= -15 || y >= 10 )
		{
			return null;
		}
		SandChunk newchunk = new( new( ChunkWidth, ChunkHeight ), new( x * ChunkWidth, y * ChunkHeight ) );

		return chunks.TryAdd( new( x, y ), newchunk ) ? newchunk : null;
	}

	void RemoveEmptyChunks()
	{
		for ( int i = 0; i < chunks.Count; i++ )
		{
			SandChunk chunk = chunks.ElementAt( i ).Value;
			if ( chunk.filledcells == 0 )
			{
				chunks.Remove( chunk.Position, out _ );
				i--;
			}
		}
	}





	TimeSince LastUpdate = 0;

	[ConCmd.Client]
	public static void SetCellClient( int x, int y, int type )
	{
		if ( !Instance.InBounds( new Vector2Int( x, y ) ) )
		{
			Log.Warning( $"Invalid cell {x} {y}  chunklocal = " );
			return;
		}
		Cell cell = Instance.GetCell( new Vector2Int( x, y ) );
		cell.type = type;
		//brown sand
		if ( type == 1 )
		{
			cell.bouyancy = 9;
			cell.color = new Color( 0.5f, 0.4f, 0.3f );
		}
		//blue water
		if ( type == 2 )
		{
			cell.bouyancy = 10;
			cell.color = new Color( 0.3f, 0.4f, 0.5f );
		}

		Instance.SetCell( new Vector2Int( x, y ), cell );
		//Log.Info( $"Set cell {x} {y} to {type}" );
	}
	[ConCmd.Client]
	public static void SetCellBrush( int x, int y, int size, int type )
	{
		for ( int i = -size; i < size; i++ )
		{
			for ( int j = -size; j < size; j++ )
			{
				SetCellClient( x + i, y + j, type );
			}
		}
	}

	[ConCmd.Client]
	public static void ClearGrid()
	{
		instance.chunks.Clear();
	}

	[GameEvent.Client.Frame]
	public void Update()
	{
		//if ( LastUpdate < 0.05f ) return;
		//SetCellClient( 10, 100, 2 );
		//SetCellBrush( ChunkWidth - 1, ChunkHeight - 1, 10, 1 );
		//SetCellBrush( -100, ChunkHeight - 1, 10, 1 );


		//SetCellClient( 0, ChunkHeight - 1, 1 );

		RealUpdate();




	}

	bool updating = false;
	async void RealUpdate()
	{
		if ( updating ) return;
		using var _a = Profile.Scope( "Sandworld::Update" );
		updating = true;


		List<Task> tasks = new();
		List<Vector2Int> markedchunks = new();
		//Update Cells
		foreach ( var chunk in chunks )
		{
			tasks.Add( GameTask.RunInThreadAsync( () =>
			{
				/* if ( chunk.Value.filledcells > 0 ) */
				new SimpleSandWorker( this, chunk.Value ).UpdateChunk();
				/* else
					markedchunks.Add( chunk.Key ); */
			} ) );
		}

		/* for ( int i = 0; i < markedchunks.Count; i++ )
		{
			chunks.Remove( markedchunks[i], out _ );
			i--;
		} */

		await GameTask.WhenAll( tasks.ToArray() );
		tasks.Clear();
		foreach ( var chunk in chunks.Values )
		{
			tasks.Add( GameTask.RunInThreadAsync( () =>
			{
				new SimpleSandWorker( this, chunk ).CommitChanges();
			} ) );
		}
		await GameTask.WhenAll( tasks.ToArray() );
		tasks.Clear();
		updating = false;
	}

	internal void SetVelocityCell( Vector2Int pos, Vector2Int vel )
	{
		GetChunk( pos ).SetVelocityCell( pos, vel );
	}

	public Vector2 ToLocal( Vector2 newpos )
	{
		return newpos - new Vector2( ChunkWidth, ChunkHeight );
	}

	internal void SetCellVelocity( Vector2Int pos, Vector2Int vel )
	{
		if ( InBounds( pos ) )
			GetChunk( pos ).SetCellVelocity( pos, vel );
	}
}
