namespace Sand.Systems.FallingSand;

public class SandChunk
{
	public Vector2Int Size, Position;


	Cell[] cells;
	public List<CellMoveInfo> Changes = new();

	public int filledcells = 0;

	public Texture Texture { get; set; }

	public SandChunk( Vector2Int size, Vector2Int position )
	{
		Size = size;
		Position = position;
		cells = new Cell[size.x * size.y];
	}

	public int GetIndex( Vector2Int pos )
	{
		return (pos.x - Position.x) +
			(pos.y - Position.y) * Size.x;
	}
	public bool InBounds( Vector2Int pos )
	{
		return pos.x >= Position.x && pos.y >= Position.y
			&& pos.x < Position.x + Size.x && pos.y < Position.y + Size.y;
	}

	public bool IsEmpty( Vector2Int pos )
	{
		//if ( !InBounds( pos ) )
		//	return false;
		return IsEmpty( GetIndex( pos ) );
	}
	bool IsEmpty( int index )
	{
		return GetCell( index ).type == 0;
	}

	public Cell GetCell( Vector2Int pos )
	{
		if ( !InBounds( pos ) )
		{
			return new Cell();
		}
		return GetCell( GetIndex( pos ) );
	}
	Cell GetCell( int index )
	{
		return cells[index];
	}


	public void SetCell( Vector2Int pos, Cell cell )
	{
		if ( !InBounds( pos ) )
			return;
		SetCell( GetIndex( pos ), cell );
	}
	void SetCell( int index, Cell cell )
	{
		if ( cell.type == 0 && cells[index].type != 0 )
		{
			filledcells--;
		}
		else if ( cell.type != 0 && cells[index].type == 0 )
		{
			filledcells++;
		}

		cells[index] = cell;
		DrawPixel( index, cell.type == 0 ? Color.Transparent : cell.color );
	}

	public void MoveCell( SandChunk src, Vector2Int From, Vector2Int To, bool Swap = false )
	{
		Changes.Add( new( src, From, To, Swap ) );
	}




	public void DrawPixel( int x, Color color )
	{
		if ( x < 0 || x >= Size.x * Size.y ) return;
		pixels ??= new Color32[Size.x * Size.y];
		pixels[x] = color;
	}
	Color32[] pixels;
	public void Draw()
	{
		pixels ??= new Color32[Size.x * Size.y];
		Texture ??= Texture.Create( Size.x, Size.y ).WithDynamicUsage().Finish();

		Texture.Update( pixels, 0, 0, Size.x, Size.y );
	}

	internal void SetVelocityCell( Vector2Int pos, Vector2Int vel )
	{
		var cell = GetCell( pos );
		cell.Velocity = vel;
		SetCell( pos, cell );
	}

	internal void SetCellVelocity( Vector2Int pos, Vector2Int vel )
	{
		var cell = GetCell( pos );
		cell.Velocity = vel;
		SetCell( pos, cell );
	}
}

